using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(HistoryAutoRecorderGroup))]
    [HarmonyPatch(nameof(HistoryAutoRecorderGroup.AddOrRemoveHistoryRecorders))]
    public class HistoryAutoRecorderGroup_AddOrRemoveHistoryRecorders_AddAGraphForEachPawn
    {
        public static bool Prefix(HistoryAutoRecorderGroup __instance) 
        {
            if (__instance.def.defName != "RR_PawnWeightHistory")
                return true;


            return false;
        }
    }
}
