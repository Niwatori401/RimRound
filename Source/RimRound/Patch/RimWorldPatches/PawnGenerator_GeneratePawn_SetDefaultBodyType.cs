using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("GeneratePawn", typeof(PawnGenerationRequest))]
    public class PawnGenerator_GeneratePawn_SetDefaultBodyType
    {
        public static void Postfix(Pawn __result)
        {
            PawnGeneratorUtility.SetDefaultBodyType(__result);
        }
    }
}
