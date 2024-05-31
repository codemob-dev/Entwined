using System.Text;
using UnityEngine;

namespace Entwined.Tests
{
    internal class MainTest : MonoBehaviour
    {
        static EntwinedPacketChannel<string> helloWorldChannel;
        private void Awake()
        {
            helloWorldChannel = new EntwinedPacketChannel<string>(Entwined.instance, new StringEntwiner());

            helloWorldChannel.OnMessage += OnMessage;
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(15, 120, 100, 40), "Run Test"))
            {
                helloWorldChannel.SendMessage("Hello World!");
                Entwined.StaticLogger.LogInfo("Sent message!");
            }
        }

        private static void OnMessage(string payload)
        {
            Entwined.StaticLogger.LogInfo(payload);
        }
    }
}