using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AethermancerTomeDeclutter
{
    public enum FilterMode
    {
        RemoveAll,    // Option 1: Remove all Lances/Totems from level-up pool
        HalveWild     // Option 2: Halve drop rate for Wild element monsters
    }

    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static ManualLogSource Log { get; private set; }
        public static Harmony Harmony { get; private set; }

        // Config entries
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<FilterMode> Mode;

        // Context flag for Living Tome ability
        public static bool IsLivingTomeContext = false;

        private void Awake()
        {
            Instance = this;
            Log = Logger;

            // Bind config options
            Enabled = Config.Bind(
                "General",
                "Enabled",
                true,
                "Enable the Tome Declutter mod"
            );

            Mode = Config.Bind(
                "General",
                "Mode",
                FilterMode.HalveWild,
                "RemoveAll: Remove all Lances/Totems from level-up pool for all monsters\n" +
                "HalveWild: Only reduce drop rate (50%) for Wild element monsters (like Grimoire Shifted)"
            );

            Log.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} loaded!");
            Log.LogInfo($"Mode: {Mode.Value}");

            // Apply Harmony patches
            Harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            Harmony.PatchAll();

            Log.LogInfo("Patches applied successfully!");
        }

        private void OnDestroy()
        {
            Harmony?.UnpatchSelf();
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.klappec.aethermancer.tomedeclutter";
        public const string PLUGIN_NAME = "Tome Declutter";
        public const string PLUGIN_VERSION = "1.0.0";
    }
}
