using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimRound.Utilities;
using RimRound.Hediffs;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnDownedWiggler))]
    [HarmonyPatch("RandomDownedAngle", MethodType.Getter)]
    public class PawnDownedWiggler_RandomDownedAngle_AdjustByWeight
    {
        public static void Postfix(PawnDownedWiggler __instance, ref float __result) 
        {
            Pawn pawn = (Pawn)Traverse.Create(__instance).Field("pawn").GetValue();


            if (Functions.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn)?.Severity is float s && s >= minSeverityToBeNotWiggle)
                __result = Rot4.South.AsAngle;

        }


        static float minSeverityToBeNotWiggle = 0.3f;
    }
}
