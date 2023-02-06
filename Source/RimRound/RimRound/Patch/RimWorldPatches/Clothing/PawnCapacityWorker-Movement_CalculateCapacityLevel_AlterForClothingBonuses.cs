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
    [HarmonyPatch(typeof(PawnCapacityWorker_Moving))]
    [HarmonyPatch(nameof(PawnCapacityWorker_Moving.CalculateCapacityLevel))]
    public class PawnCapacityWorker_Movement_CalculateCapacityLevel_AlterForClothingBonuses
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

            __result += comp.clothingBonuses.flatMoveBonus;
            return;
        }
    }
}
