using BepInEx;

namespace Entwined
{
    /// <summary>
    /// A variable synced across all clients. 
    /// To avoid collisions only one client should modify the variable at a time, 
    /// otherwise there may be desyncing.
    /// </summary>
    /// <typeparam name="T">Variable Type</typeparam>
    public class SyncedVariable<T>
    {
        public IEntwiner<T> Entwiner { get; private set; }
        public PacketChannel PacketChannel { get; private set; }
        public SyncedVariable(PacketChannel channel, IEntwiner<T> entwiner)
        {
            Entwiner = entwiner;
            PacketChannel = channel;
            PacketChannel.OnMessage += OnMessage;
        }

        /// <summary>
        /// Creates a new <c>PacketChannel</c> for the plugin and 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="entwiner"></param>
        public SyncedVariable(BaseUnityPlugin plugin, IEntwiner<T> entwiner)
        {
            Entwiner = entwiner;
            PacketChannel = new PacketChannel(plugin);
            PacketChannel.OnMessage += OnMessage;
        }

        private void OnMessage(byte[] payload)
        {
            lock (internalValue)
            {
                internalValue = Entwiner.Detwine(payload);
            }
        }

        private T internalValue;

        /// <summary>
        /// The actual variable.
        /// </summary>
        public T Value
        {
            get
            {
                lock (internalValue)
                {
                    return internalValue;
                }
            }
            set
            {
                lock (internalValue)
                {
                    internalValue = value;
                    PacketChannel.SendMessage(Entwiner.Entwine(value));
                }
            }
        }
    }
}
