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
    [HarmonyPatch(typeof(Pawn_HealthTracker))]
    [HarmonyPatch("MakeDowned")]
    public class Pawn_HealthTracker_MakeDowned_SetCustomWiggleAngleInitial
    {
        static float minSeverityToBeNotWiggle = 0.3f;

        public static void Postfix(Pawn ___pawn) 
        {
            if (Functions.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, ___pawn)?.Severity is float s && s >= minSeverityToBeNotWiggle)
                ___pawn.Drawer.renderer.wiggler.SetToCustomRotation(Rot4.North.AsAngle);

        }
    }
}
