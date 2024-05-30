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

            EntwinedUtilities.SteamManagerLoaded += EntwinedUtilities_SteamManagerLoaded;
        }

        private static void PacketType_OnMessage(byte[] payload)
        {
            Entwined.StaticLogger.LogInfo(Encoding.ASCII.GetString(payload));
        }

        private static void EntwinedUtilities_SteamManagerLoaded()
        {
            packetType.SendMessage(Encoding.ASCII.GetBytes("Hello World!"));
        }
    }
}
