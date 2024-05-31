using Steamworks;
using Steamworks.Data;
using System;
using System.Text;
using UnityEngine;

namespace Entwined.Tests
{
    internal class MainTest : MonoBehaviour
    {
        static PacketType helloWorldPacket;
        private void Awake()
        {
            helloWorldPacket = new PacketType(Entwined.instance);

            helloWorldPacket.OnMessage += PacketType_OnMessage;
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(15, 120, 100, 40), "Run Test"))
            {
                helloWorldPacket.SendMessage(Encoding.ASCII.GetBytes("Hello World!"));
                Entwined.StaticLogger.LogInfo("Sent message!");
            }
        }

        private static void PacketType_OnMessage(byte[] payload)
        {
            Entwined.StaticLogger.LogInfo(Encoding.ASCII.GetString(payload));
        }
    }
}
