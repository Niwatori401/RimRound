using HarmonyLib;
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
            Utilities.PawnGeneratorUtility.AddHediffsToPawn(pawn);
        }
    }
}
