using BepInEx;
using BepInEx.Logging;
using Entwined.Tests;
using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Entwined
{
    /// <summary>
    /// The main plugin class. Contains the barebones static methods 
    /// required for a packet system.
    /// </summary>
    [BepInPlugin("com.entwinedteam.entwined", "Entwined", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Entwined : BaseUnityPlugin
    {
        internal static Harmony harmony;
        internal static Entwined instance;

        internal static ManualLogSource StaticLogger => instance.Logger;

        /// <summary>
        /// The signature to help avoid clashes with built-in packets. Can be any length.
        /// </summary>
        internal static readonly byte[] signature = new byte[]
        {
            0b11101110,
            0b10011011,
            0b10010010,
            0b01101100
        };

        /// <summary>
        /// The main entrypoint. Performs all the harmony patches.
        /// </summary>
        private void Awake()
        {
            instance = this;
            harmony = new Harmony(Info.Metadata.GUID);

            harmony.PatchAll(Assembly.GetExecutingAssembly()); // Patch the entire assembly

            // Patch the SteamManager.Awake function to run after it gets initialized
            var func = AccessTools.Method(typeof(SteamManager), "Awake");
            var patch = AccessTools.Method(
                typeof(EntwinedUtilities), 
                nameof(EntwinedUtilities.SteamManager_Awake));

            harmony.Patch(func, postfix: new HarmonyMethod(patch));

            // Patch SteamSocket.OnMessage
            func = AccessTools.Method(typeof(SteamSocket), nameof(SteamSocket.OnMessage));
            patch = AccessTools.Method(typeof(Entwined), nameof(SteamSocket_OnMessage));

            harmony.Patch(func, prefix: new HarmonyMethod(patch));

            EntwinedUtilities.SteamManagerLoaded += SteamManagerLoaded;

            // Comment this out during production
            var mainTest = new GameObject("EntwinedMainTest", typeof(MainTest));
            DontDestroyOnLoad(mainTest);
        }

        /// <summary>
        /// Runs when the <c>SteamManager</c> loads (after all other mods have loaded).
        /// </summary>
        private void SteamManagerLoaded()
        {
            StaticLogger.LogInfo($"Generating PacketIdentifierIDs!");
            IdentifierRegister.GeneratePacketIdentifierIDs();
        }


        /// <summary>
        /// <c>SteamSocket.OnMessage</c> handles all incoming messages. 
        /// If a custom packet is detected the patch method will prevent the 
        /// original method from executing and handle the packet accordingly.
        /// </summary>
        /// <returns><c>true</c> to allow the original method to execute and 
        /// <c>false</c> to prevent it from executing.</returns>
        private static bool SteamSocket_OnMessage(
            SteamSocket __instance, Connection connection,
            NetIdentity identity, IntPtr data, int size,
            long messageNum, long recvTime, int channel)
        {
            // Copy the incoming packet to a buffer
            byte[] rawPacketBuffer = new byte[size];
            Marshal.Copy(data, rawPacketBuffer, 0, size);

            // Verify the packet is not custom
            if (!IsCustomPacket(rawPacketBuffer)) return true;

            // Check for invalid SteamIds
            if (!identity.SteamId.IsValid)
            {
                instance.Logger.LogWarning("Received message from invalid SteamId!");
                return false;
            }
            // Create a Friend object and verify the connection is in the same lobby
            Friend friend = new Friend(identity.SteamId);
            if (!friend.IsIn(SteamManager.instance.currentLobby.Id))
            {
                instance.Logger.LogWarning(
                    $"{friend.Name} ({identity.SteamId}) sent a custom packet from outside of the current lobby!"
                    );
                return false;
            }

            var deconstructedPacket = DeconstructedPacket.DeconstructPacket(rawPacketBuffer);

            deconstructedPacket.PacketIdentifier.PacketType.ReceiveMessage(deconstructedPacket.Payload);

            return false;
        }


        /// <summary>
        /// A basic check to verify if a packet is custom.
        /// </summary>
        /// <param name="packet">The full packet data to verify</param>
        /// <returns><c>true</c> if the packet is a custom packet</returns>
        internal static bool IsCustomPacket(byte[] packet)
        {
            if (packet.Length < signature.Length + PacketIdentifier.EncodedSize)
            {
                return false; // The packet is too small to be a custom packet
            }
            var packetSignature = packet.Take(signature.Length).ToArray();
            if (packetSignature.SequenceEqual(signature))
            {
                StaticLogger.LogInfo($"Received valid packet {packet.ToFormattedString()}!");
                return true; // The packet has the signature
            }
            StaticLogger.LogInfo($"Received message {packet.ToFormattedString()}, but it signature, {packetSignature.ToFormattedString()}, is invalid. Expected the signature {signature.ToFormattedString()}.");
            return false; // The packet does not have the signature
        }

        /// <summary>
        /// Broadcasts a message to all players
        /// </summary>
        /// <param name="identifier">The PacketIdentifier</param>
        /// <param name="payload">The payload</param>
        internal static void SendMessage(PacketIdentifier identifier, byte[] payload)
        {
            var msg = new byte[signature.Length + PacketIdentifier.EncodedSize + payload.Length];
            signature.CopyTo(msg, 0);
            identifier.Encode().CopyTo(msg, signature.Length);
            payload.CopyTo(msg, signature.Length + PacketIdentifier.EncodedSize);

            foreach (var player in SteamManager.instance.connectedPlayers)
            {
                StaticLogger.LogInfo($"Sending message {msg.ToFormattedString()} to {player.steamName}");
                player.Connection.SendMessage(msg);
            }
        }

        internal struct DeconstructedPacket
        {
            public byte[] Payload { get; set; }
            public PacketIdentifier PacketIdentifier { get; set; }

            public static DeconstructedPacket DeconstructPacket(byte[] packet)
            {
                return new DeconstructedPacket
                {
                    PacketIdentifier = IdentifierRegister.GetPacketIdentifier(
                        packet.Skip(signature.Length) // Skip the signature
                        .Take(PacketIdentifier.EncodedSize).ToArray() // Grab only the data needed
                        ),
                    // Grab the data after the signature and identifier
                    Payload = packet.Skip(signature.Length + PacketIdentifier.EncodedSize).ToArray()
                };
            }
        }
    }
}
