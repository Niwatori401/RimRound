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
    [HarmonyPatch(nameof(HistoryAutoRecorderGroup.DrawGraph))]
    public class HistoryAutoRecorderGroup_DrawGraph_OnlyShowSelectedPawns
    {
        public static bool Prefix(HistoryAutoRecorderGroup __instance) 
        {
            if (__instance.def.defName != "RR_PawnWeightHistory")
                return true;


            return false;
        }
    }
}
