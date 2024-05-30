using BepInEx;
using HarmonyLib;

namespace Entwined
{
    [BepInPlugin("com.entwinedteam.entwined", "Entwined", "1.0.0")]
    [BepInProcess("BoplBattle.exe")]
    public class Entwined : BaseUnityPlugin
    {
        public Harmony harmony;
        public static Entwined instance;
        private void Awake()
        {
            instance = this;
            harmony = new Harmony(Info.Metadata.GUID);
            harmony.PatchAll(typeof(Entwined));
        }
    }
}
