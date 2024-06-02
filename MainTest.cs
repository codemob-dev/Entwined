using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

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

        string msg = "Hello World!";
        SyncedVariable<int> syncedVariable;
        private void OnGUI()
        {
            msg = GUI.TextField(new Rect(15, 120, 220, 35), msg);
            if (GUI.Button(new Rect(15, 170, 100, 40), "Send Message"))
            {
                helloWorldChannel.SendMessage(msg);
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
        }

        private static void OnMessage(string payload, PacketSourceInfo sourceInfo)
        {
            Entwined.StaticLogger.LogInfo($"{sourceInfo.SteamName}: {payload}");
        }
    }
}