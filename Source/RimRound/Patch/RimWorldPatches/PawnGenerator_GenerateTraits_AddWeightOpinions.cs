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
    [HarmonyPatch("GenerateTraits")]
    public class PawnGenerator_GenerateTraits_AddWeightOpinions
    {
        public static void Postfix(Pawn pawn)
        {
            Utilities.PawnGeneratorUtility.AddWeightOpinion(pawn);
        }

    }
}
