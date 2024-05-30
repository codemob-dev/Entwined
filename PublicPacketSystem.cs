using BepInEx;

namespace Entwined
{
    /// <summary>
    /// An event fired when a packet received data.
    /// </summary>
    /// <param name="payload">The data received</param>
    public delegate void PacketReceiveEvent(byte[] payload);

    /// <summary>
    /// The simplest component in Entwined. 
    /// <c>PacketType</c> represents a self-contained 
    /// type of packet that can broadcast and receive data.
    /// 
    /// Note that different <c>PacketType</c>s cannot interact, 
    /// each functions as its own message channel.
    /// </summary>
    public class PacketType
    {
        internal PacketIdentifier packetIdentifier;
        public PacketType(BaseUnityPlugin plugin)
        {
            packetIdentifier = IdentifierRegister.GenerateNewPacketIdentifier(plugin);
            packetIdentifier.PacketType = this;
        }

        /// <summary>
        /// Fired when the packet receives data.
        /// </summary>
        public event PacketReceiveEvent OnMessage;

        internal void ReceiveMessage(byte[] payload)
        {
            OnMessage.Invoke(payload);
        }

        public void SendMessage(byte[] payload)
        {
            Entwined.SendMessage(packetIdentifier, payload);
        }
    }
}
