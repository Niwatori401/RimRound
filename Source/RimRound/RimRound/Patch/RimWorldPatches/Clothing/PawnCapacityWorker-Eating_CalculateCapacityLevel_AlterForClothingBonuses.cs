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
    [HarmonyPatch(typeof(PawnCapacityWorker_Eating))]
    [HarmonyPatch(nameof(PawnCapacityWorker_Eating.CalculateCapacityLevel))]
    public class PawnCapacityWorker_Eating_CalculateCapacityLevel_AlterForClothingBonuses
    {
        public static void Postfix(HediffSet diffSet, ref float __result)
        {
            if (__result <= 0)
                return;

            Pawn pawn = diffSet.pawn;
            if (!pawn.RaceProps.Humanlike)
                return;

            var comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp is null)
                return;

            __result += comp.clothingBonuses.flatEatingSpeedBonus;
            return;
        }
    }
}
