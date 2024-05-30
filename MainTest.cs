using Steamworks;
using Steamworks.Data;
using System;
using System.Text;

namespace Entwined.Tests
{
    internal static class MainTest
    {
        static PacketType packetType;
        public static void Run()
        {
            packetType = new PacketType(Entwined.instance);

            packetType.OnMessage += PacketType_OnMessage;

            SteamMatchmaking.OnLobbyMemberJoined += LobbyMemberJoined;
        }

        private static void LobbyMemberJoined(Lobby lobby, Friend friend)
        {
            packetType.SendMessage(Encoding.ASCII.GetBytes("Hello World!"));
            Entwined.StaticLogger.LogInfo("Sent message!");
        }

        private static void PacketType_OnMessage(byte[] payload)
        {
            Entwined.StaticLogger.LogInfo(Encoding.ASCII.GetString(payload));
        }
    }
}
