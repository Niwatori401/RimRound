using HarmonyLib;
using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(MassUtility))]
    [HarmonyPatch(nameof(MassUtility.Capacity))]
    public class MassUtility_Capacity_AlterForPerks
    {
        public static void Postfix(ref float __result, Pawn __0)
        {
            if (!__0?.RaceProps?.Humanlike ?? false)
                return;

            FullnessAndDietStats_ThingComp comp = __0.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (comp is null)
                return;

            int packWhaleLevel = comp?.perkLevels?.PerkToLevels?["RR_PackWhale_Title"] ?? 0;

            if (packWhaleLevel >= 1 && Utilities.BodyTypeUtility.PawnIsOverWeightThreshold(__0, Defs.BodyTypeDefOf.F_040_Obese))
            {
                __result *= 2;
            }

            return;
        }
    }
}
