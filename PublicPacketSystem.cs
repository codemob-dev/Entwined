using BepInEx;
using System.Collections.Generic;

namespace Entwined
{
    /// <summary>
    /// An event fired when a packet received data.
    /// </summary>
    /// <param name="payload">The data received</param>
    public delegate void PacketReceiveEvent(byte[] payload);

    /// <summary>
    /// The simplest component in Entwined. 
    /// <c>PacketChannel</c> represents a self-contained 
    /// type of packet that can broadcast and receive data.
    /// 
    /// Note that different <c>PacketChannel</c>s cannot interact, 
    /// each functions as its own message channel.
    /// </summary>
    public class PacketChannel
    {
        internal PacketIdentifier packetIdentifier;

        /// <summary>
        /// Run in your awake function.
        /// Creates a new PacketType to transmit and receive data.
        /// <example>
        /// <code>
        /// void Awake() {
        ///     var packetType = new PacketType(this);
        /// }
        /// </code>
        /// </example>
        /// </summary>
        /// <param name="plugin">Your current plugin.</param>
        public PacketChannel(BaseUnityPlugin plugin)
        {
            packetIdentifier = IdentifierRegister.GenerateNewPacketIdentifier(plugin);
            packetIdentifier.PacketType = this;
        }

        /// <summary>
        /// Fired when the packet receives data. (A client ran <c>packetType.SendMessage</c>)
        /// </summary>
        public event PacketReceiveEvent OnMessage;

        internal void ReceiveMessage(byte[] payload)
        {
            OnMessage.Invoke(payload);
        }

        /// <summary>
        /// Send data to all clients
        /// </summary>
        /// <param name="payload">The data to send</param>
        public void SendMessage(byte[] payload)
        {
            Entwined.SendMessage(packetIdentifier, payload);
        }
    }

    /// <summary>
    /// Fires when <c>SteamManager</c> loads.
    /// </summary>
    public delegate void SteamManagerLoadEvent();

    /// <summary>
    /// A static utility class for operations related to Entwined.
    /// </summary>
    public static class EntwinedUtilities
    {

        internal static bool loaded = false;

        /// <summary>
        /// Fires when <c>SteamManager</c> loads.
        /// </summary>
        public static event SteamManagerLoadEvent SteamManagerLoaded;
        internal static void SteamManager_Awake()
        {
            if (!loaded)
            {
                loaded = true;
                SteamManagerLoaded.Invoke();
            }
        }

        /// <summary>
        /// Converts an IEnumerable to a formatted string
        /// </summary>
        /// <param name="enumerable">The IEnumerable to convert</param>
        /// <returns>The formatted string</returns>
        public static string ToFormattedString<T>(this IEnumerable<T> enumerable)
        {
            return $"{{ {string.Join(", ", enumerable)} }}";
        }
    }
}
