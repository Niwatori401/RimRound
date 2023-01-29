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
    [HarmonyPatch(typeof(Verse.GasUtility))]
    [HarmonyPatch(nameof(Verse.GasUtility.PawnGasEffectsTick))]
    public static class GasUtility_PawnGasEffectsTick_AddSupportForGas
    {
        public static void Postfix(Pawn pawn) 
        {
            if (!pawn.IsHashIntervalTick(150) || !pawn.RaceProps.Humanlike)
                return;

            foreach (var gasHediffCombo in Utilities.GasUtility.gasToHediff)
            {
                byte gasDensity = pawn.Position.GasDentity(pawn.Map, gasHediffCombo.Key);

                if (gasDensity <= 0)
                    return;

                float gasDensityPercent = gasDensity / 255f;

                Hediff hediff = Utilities.HediffUtility.GetHediffOfDefFrom(gasHediffCombo.Value, pawn);

                if (hediff != null && hediff.CurStageIndex == hediff.def.stages.Count - 1)
                    gasDensityPercent *= 0.1f; // Dampen additional severity for extended exposure


                if (hediff is null)
                    hediff = Utilities.HediffUtility.AddHediffOfDefTo(gasHediffCombo.Value, pawn);


                Utilities.HediffUtility.AddHediffSeverity(hediff, pawn, gasDensityPercent * 0.1f);


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
