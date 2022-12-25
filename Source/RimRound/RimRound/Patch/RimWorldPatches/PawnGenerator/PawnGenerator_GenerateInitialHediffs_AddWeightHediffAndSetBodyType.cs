using HarmonyLib;
using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("GenerateInitialHediffs")]
    public class PawnGenerator_GenerateInitialHediffs_AddWeightHediffAndSetBodyType
    {
        public static void Postfix(Pawn pawn) 
        {
            if (pawn.TryGetComp<FullnessAndDietStats_ThingComp>() is null)
                return;

            Utilities.PawnGeneratorUtility.AddHediffsToPawn(pawn);
        }
    }
}
