using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimRound.Defs;
using Verse;
using RimWorld;
using HarmonyLib;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Pawn_AgeTracker))]
    [HarmonyPatch("CurLifeStage", MethodType.Getter)]
    public class RimRound_CurLifeStageIndexPatch
    {
        [HarmonyAfter(new string[] { "rimworld.erdelf.alien_race.main" })]
        public static bool Prefix(LifeStageDef __result) 
        {
            __result = GenericDefOf.testLifeStage;
            return false;
        }
    }
}
