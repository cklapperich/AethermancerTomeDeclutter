using HarmonyLib;

namespace AethermancerTomeDeclutter.Patches
{
    /// <summary>
    /// Patches ActionUnleashTome.GetSubSkills() to set context flag for Living Tome skill rolling
    /// </summary>
    [HarmonyPatch(typeof(ActionUnleashTome), nameof(ActionUnleashTome.GetSubSkills))]
    public static class ActionUnleashTomePatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            Plugin.IsLivingTomeContext = true;
        }

        [HarmonyFinalizer]
        public static void Finalizer()
        {
            // Use Finalizer to ensure flag is always reset, even if exception occurs
            Plugin.IsLivingTomeContext = false;
        }
    }
}
