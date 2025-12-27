using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace AethermancerTomeDeclutter.Patches
{
    /// <summary>
    /// Patches SkillPicker.DetermineWeightedActions() to filter or reduce weight of Lance/Totem skills
    /// </summary>
    [HarmonyPatch(typeof(SkillPicker), nameof(SkillPicker.DetermineWeightedActions))]
    public static class SkillPickerPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ref List<SkillPicker.WeightedSkill> __result, SkillPicker __instance)
        {
            if (!Plugin.Enabled.Value)
                return;

            bool isWildMonster = __instance.Monster.SkillManager.GetElements().Contains(EElement.Wild);

            switch (Plugin.Mode.Value)
            {
                case FilterMode.RemoveAll:
                    if (Plugin.IsLivingTomeContext)
                    {
                        // Living Tome uses useWeights=false, so we must REMOVE from list
                        RemoveLanceTotems(__result);
                    }
                    else
                    {
                        // Level-up uses weights, so setting to 0 works
                        ApplyLanceTotemMultiplier(__result, 0f);
                    }
                    break;

                case FilterMode.HalveWild:
                    if (!isWildMonster)
                        return;

                    if (Plugin.IsLivingTomeContext)
                    {
                        // Living Tome: remove each Lance/Totem with 50% chance
                        RemoveLanceTotemsWithChance(__result, 0.5f);
                    }
                    else
                    {
                        // Level-up: halve the weight
                        ApplyLanceTotemMultiplier(__result, 0.5f);
                    }
                    break;
            }
        }

        private static void ApplyLanceTotemMultiplier(List<SkillPicker.WeightedSkill> result, float multiplier)
        {
            int modified = 0;

            for (int i = 0; i < result.Count; i++)
            {
                var ws = result[i];
                if (IsLanceOrTotem(ws))
                {
                    result[i] = new SkillPicker.WeightedSkill(ws.Skill, ws.Weight * multiplier);
                    modified++;
                }
            }

            if (modified > 0)
                Plugin.Log.LogDebug($"Applied {multiplier}x weight to {modified} Lance/Totem skills");
        }

        private static void RemoveLanceTotems(List<SkillPicker.WeightedSkill> result)
        {
            int removed = result.RemoveAll(ws => IsLanceOrTotem(ws));

            if (removed > 0)
                Plugin.Log.LogDebug($"[LivingTome] Removed {removed} Lance/Totem skills from pool");
        }

        private static void RemoveLanceTotemsWithChance(List<SkillPicker.WeightedSkill> result, float removeChance)
        {
            int removed = result.RemoveAll(ws => IsLanceOrTotem(ws) && Random.value < removeChance);

            if (removed > 0)
                Plugin.Log.LogDebug($"[LivingTome] Removed {removed} Lance/Totem skills (50% chance each)");
        }

        private static bool IsLanceOrTotem(SkillPicker.WeightedSkill ws)
        {
            return ws.Action != null &&
                   (ws.Action.Name.Contains("Lance") || ws.Action.Name.Contains("Totem"));
        }
    }
}
