using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    /// <summary>
    /// This patch actually applies the gas effects to pawns in the gas.
    /// </summary>
    [HarmonyPatch(typeof(Verse.Pawn))]
    [HarmonyPatch(nameof(Verse.Pawn.Tick))]
    public static class Pawn_Tick_AddSupportForGas
    {
        public static void Postfix(Pawn __instance) 
        {
            if (__instance is null)
                return;

            if (!__instance.IsHashIntervalTick(150) || __instance.Suspended || !__instance.RaceProps.Humanlike)
                return;

            foreach (var gasHediffCombo in Utilities.GasUtility.gasToHediff)
            {
                byte gasDensity = __instance.Position.GasDentity(__instance.Map, gasHediffCombo.Key);

                if (gasDensity <= 0)
                    return;

                float gasDensityPercent = gasDensity / 255f;

                Hediff hediff = Utilities.HediffUtility.GetHediffOfDefFrom(gasHediffCombo.Value, __instance);

                if (hediff != null && hediff.CurStageIndex == hediff.def.stages.Count - 1)
                    gasDensityPercent *= 0.1f; // Dampen additional severity for extended exposure


                if (hediff is null)
                    hediff = Utilities.HediffUtility.AddHediffOfDefTo(gasHediffCombo.Value, __instance);


                Utilities.HediffUtility.AddHediffSeverity(hediff, __instance, gasDensityPercent * 0.1f);


            }
        }


        public static byte GasDentity(this IntVec3 cell, Map map, RRGasType gasType)
        {
            if (map is null)
                return 0;

            return map.GetComponent<MapComp_RRGasGrid>().DensityAt(cell, gasType);
        }
    }
}
