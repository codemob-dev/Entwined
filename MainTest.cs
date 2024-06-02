using Steamworks;
using System.Collections.Generic;
using UnityEngine;

namespace Entwined.Tests
{
    internal class MainTest : MonoBehaviour
    {
        static EntwinedPacketChannel<string> helloWorldChannel;
        private void Awake()
        {
            helloWorldChannel = new EntwinedPacketChannel<string>(Entwined.instance, new StringEntwiner());
            syncedVariable = new SyncedVariable<int>(Entwined.instance, new IntEntwiner());
            helloWorldChannel.OnMessage += OnMessage;
        }

        static string msg = "Hello World!";
        static List<string> messages = new List<string>();
        SyncedVariable<int> syncedVariable;
        private void OnGUI()
        {
            msg = GUI.TextField(new Rect(15, 120, 220, 35), msg);
            if (GUI.Button(new Rect(15, 170, 100, 40), "Send Message"))
            {
                helloWorldChannel.SendMessage(msg);
                AddMessage(msg, SteamClient.Name);
                Entwined.StaticLogger.LogInfo("Sent message!");
            }
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(15, 225, 35, 35), syncedVariable.Value.ToString());
            GUI.contentColor = Color.white;

            if (GUI.Button(new Rect(15, 275, 100, 40), "Increment"))
            {
                syncedVariable.Value++;
            }
            if (GUI.Button(new Rect(15, 330, 100, 40), "Decrement"))
            {
                syncedVariable.Value--;
            }
            GUI.contentColor = Color.black;
            GUI.Label(new Rect(15, 385, 1600, 1600), string.Join("\n", messages));
            GUI.contentColor = Color.white;
        }

        private static void OnMessage(string payload, PacketSourceInfo sourceInfo)
        {
            AddMessage(payload, sourceInfo.SenderSteamName);
        }
        private static void AddMessage(string msg, string user)
        {
            messages.Insert(0, $"[{user}] {msg}");
            if (messages.Count > 24) messages.RemoveAt(messages.Count - 1);
        }
    }
}