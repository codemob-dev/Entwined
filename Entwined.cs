using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using System;
using System.Runtime.InteropServices;

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
        public Harmony harmony;
        public static Entwined instance;

        /// <summary>
        /// The main entrypoint. Performs all the harmony patches.
        /// </summary>
        private void Awake()
        {
            instance = this;
            harmony = new Harmony(Info.Metadata.GUID);

            harmony.PatchAll(); // Patch the entire assembly
        }


        /// <summary>
        /// <c>SteamSocket.OnMessage</c> handles all incoming messages. 
        /// If a custom packet is detected the patch method will prevent the 
        /// original method from executing and handles that packet accordingly.
        /// </summary>
        /// <returns><c>true</c> to allow the original method to execute and 
        /// <c>false</c> to prevent it from executing.</returns>
        [HarmonyPatch(typeof(SteamSocket), nameof(SteamSocket.OnMessage))]
        [HarmonyPrefix]
        private static bool SteamSocket_OnMessage(
            SteamSocket __instance, Connection connection, 
            NetIdentity identity, IntPtr data, int size, 
            long messageNum, long recvTime, int channel)
        {
            // Copy the incoming packet to a buffer
            byte[] rawPacketBuffer = new byte[size];
            Marshal.Copy(data, rawPacketBuffer, 0, size);

            // Check if the packet is custom
            if (IsCustomPacket(rawPacketBuffer))
            {
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

                // TODO: make the actual advanced code part

                return false;
            } else
            {
                return true;
            }
        }


        /// <summary>
        /// A basic check to verify if a packet is custom.
        /// </summary>
        /// <param name="packet">The full packet data to verify</param>
        /// <returns><c>true</c> if the packet is a custom packet</returns>
        private static bool IsCustomPacket(byte[] packet)
        {
            return false;
        }
    }
}
