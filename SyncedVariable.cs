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

        /// <summary>
        /// Creates a new <c>SyncedVariable</c> based off of the given 
        /// <see cref="global::Entwined.PacketChannel"/> and
        /// <see cref="IEntwiner{T}"/>.
        /// </summary>
        /// <param name="channel">The <see cref="global::Entwined.PacketChannel"/></param>
        /// <param name="entwiner">The <see cref="IEntwiner{T}"/></param>
        /// <param name="initialValue">The initial value for the variable</param>
        public SyncedVariable(PacketChannel channel, IEntwiner<T> entwiner, T initialValue = default)
        {
            Entwiner = entwiner;
            PacketChannel = channel;
            PacketChannel.OnMessage += OnMessage;
            internalValue = initialValue;
        }

        /// <summary>
        /// Should only be ran in your plugin's <c>Awake</c> function.
        /// Creates a new <see cref="global::Entwined.PacketChannel"/> for the plugin 
        /// and initialized the <c>SyncedVariable</c> 
        /// to use the given <see cref="IEntwiner{T}"/>.
        /// </summary>
        /// <param name="plugin">Your plugin</param>
        /// <param name="entwiner">The <see cref="IEntwiner{T}"/></param>
        /// <param name="initialValue">The initial value for the variable</param>
        public SyncedVariable(BaseUnityPlugin plugin, IEntwiner<T> entwiner, T initialValue = default)
        {
            Entwiner = entwiner;
            PacketChannel = new PacketChannel(plugin);
            PacketChannel.OnMessage += OnMessage;
            internalValue = initialValue;
        }

        private void OnMessage(byte[] payload, PacketSourceInfo sourceInfo)
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
