using System.Collections.Generic;

namespace Entwined
{
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
