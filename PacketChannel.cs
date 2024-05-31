using BepInEx;

namespace Entwined
{
    /// <summary>
    /// An event fired when a packet received data.
    /// </summary>
    /// <param name="payload">The data received</param>
    public delegate void PacketReceiveEvent<T>(T payload);

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
        /// Creates a new <c>PacketChannel</c> to transmit and receive data.
        /// <example>
        /// <code>
        /// void Awake() {
        ///     var packetChannel = new PacketChannel(this);
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
        /// Fired when the packet receives data. (A client ran <c>packetChannel.SendMessage</c>)
        /// </summary>
        public event PacketReceiveEvent<byte[]> OnMessage;

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
    public class EntwinedPacketChannel<T>
    {
        public PacketChannel PacketChannel { get; private set; }
        public IEntwiner<T> Entwiner { get; private set; }

        /// <summary>
        /// Creates a new <c>EntwinedPacketChannel</c> from the 
        /// given <c>PacketChannel</c> and <c>IEntwiner</c>
        /// </summary>
        /// <param name="packetChannel">The packet channel</param>
        /// <param name="entwiner">The entwiner</param>
        public EntwinedPacketChannel(PacketChannel packetChannel, IEntwiner<T> entwiner)
        {
            Init(packetChannel, entwiner);
        }
        /// <summary>
        /// Creates a new <c>PacketChannel</c> and <c>EntwinedPacketChannel</c> from the 
        /// given plugin and <c>IEntwiner</c>
        /// </summary>
        /// <param name="plugin">The plugin</param>
        /// <param name="entwiner">The entwiner</param>
        public EntwinedPacketChannel(BaseUnityPlugin plugin, IEntwiner<T> entwiner)
        {
            Init(new PacketChannel(plugin), entwiner);
        }
        private void Init(PacketChannel packetChannel, IEntwiner<T> entwiner)
        {
            PacketChannel = packetChannel;
            Entwiner = entwiner;
            PacketChannel.OnMessage += ReceiveMessage;
        }

        /// <summary>
        /// Fired when the packet receives data. (A client ran <c>packetChannel.SendMessage</c>)
        /// </summary>
        public event PacketReceiveEvent<T> OnMessage;

        internal void ReceiveMessage(byte[] payload)
        {
            OnMessage.Invoke(Entwiner.Detwine(payload));
        }

        /// <summary>
        /// Send data to all clients
        /// </summary>
        /// <param name="payload">The data to send</param>
        public void SendMessage(T payload)
        {
            PacketChannel.SendMessage(Entwiner.Entwine(payload));
        }
    }
}
