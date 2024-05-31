using BepInEx;
using System;
using System.Collections.Generic;

namespace Entwined
{
    /// <summary>
    /// An identifier for a packet.
    /// </summary>
    internal class PacketIdentifier
    {
        public ushort PluginId { get; set; } = ushort.MaxValue; // Which plugin the packet is created in
        public ushort PacketId { get; set; } = ushort.MaxValue; // The id of the packet

        /// <summary>
        /// Checks if the PacketIdentifier is ready for use
        /// </summary>
        public bool IsLoaded => PluginId != ushort.MaxValue || PacketId != ushort.MaxValue;

        public PacketType PacketType { get; set; }

        public static int EncodedSize => 4;

        /// <summary>
        /// Encode to a byte array
        /// </summary>
        /// <returns>The byte array</returns>
        public byte[] Encode()
        {

            var PluginIdBytes = BitConverter.GetBytes(PluginId);
            var PacketIdBytes = BitConverter.GetBytes(PacketId);

            return new byte[] { PluginIdBytes[0], PluginIdBytes[1], 
                                PacketIdBytes[0], PacketIdBytes[1] };
        }

        /// <summary>
        /// Decode from a byte array
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <returns>The decoded <c>PacketIdentifier</c></returns>
        public static PacketIdentifier Decode(byte[] bytes)
        {
            return new PacketIdentifier
            {
                PluginId = BitConverter.ToUInt16(bytes, 0),
                PacketId = BitConverter.ToUInt16(bytes, 2)
            };
        }

        /// <summary>
        /// Check if the PacketIdentifier matches the data in the bytes
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool Matches(byte[] bytes)
        {
            return Encode() == bytes;
        }
    }


    /// <summary>
    /// Sorts the plugings by GUID to keep them consistent
    /// </summary>
    internal class PluginSorter : IComparer<BaseUnityPlugin>
    {
        public int Compare(BaseUnityPlugin x, BaseUnityPlugin y)
        {
            return x.Info.Metadata.GUID.CompareTo(y.Info.Metadata.GUID);
        }
    }

    internal static class IdentifierRegister
    {
        /// <summary>
        /// Sorts the plugins using <c>PluginSorter</c>
        /// </summary>
        internal static SortedList<BaseUnityPlugin, List<PacketIdentifier>> PluginPacketPairs = 
            new SortedList<BaseUnityPlugin, List<PacketIdentifier>>(new PluginSorter());

        internal static List<PacketIdentifier> packetIdentifiers = new List<PacketIdentifier>();

        /// <summary>
        /// Generates a packet identifier
        /// </summary>
        /// <param name="plugin">The plugin to generate the identifier under</param>
        /// <returns>The packet identitier</returns>
        internal static PacketIdentifier GenerateNewPacketIdentifier(BaseUnityPlugin plugin)
        {
            Entwined.StaticLogger.LogInfo($"Generating new PacketIdentifier for {plugin.Info.Metadata.GUID}!");
            var identifier = new PacketIdentifier { };
            if (!PluginPacketPairs.ContainsKey(plugin))
            {
                PluginPacketPairs.Add(plugin, new List<PacketIdentifier>());
            }
            PluginPacketPairs[plugin].Add(identifier);
            return identifier;
        }

        /// <summary>
        /// Sets up all of the packet identifiers to use the appropriate IDs
        /// </summary>
        internal static void GeneratePacketIdentifierIDs()
        {
            ushort pluginId = 0;
            foreach (var pluginPackets in PluginPacketPairs)
            {
                var plugin = pluginPackets.Key;
                var identifiers = pluginPackets.Value;

                Entwined.StaticLogger.LogInfo($"Found plugin {plugin.Info.Metadata.GUID} with {identifiers.Count} packet(s)!");

                for (ushort packetId = 0; packetId < identifiers.Count; packetId++)
                {
                    var identifier = identifiers[packetId];
                    identifier.PluginId = pluginId;
                    identifier.PacketId = packetId;

                    Entwined.StaticLogger.LogInfo($"Generating ID set {pluginId};{packetId}!");
                    packetIdentifiers.Add(identifier);
                }

                pluginId++;
            }
        }

        /// <summary>
        /// Find the packet identifier matching the data.
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns>The decoded packet identifier</returns>
        internal static PacketIdentifier GetPacketIdentifier(byte[] data) => packetIdentifiers.Find(id => id.Matches(data));
    }
}
