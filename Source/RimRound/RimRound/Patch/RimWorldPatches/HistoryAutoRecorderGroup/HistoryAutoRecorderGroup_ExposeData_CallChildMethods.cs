using HarmonyLib;
using RimRound.AI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(HistoryAutoRecorderGroup))]
    [HarmonyPatch(nameof(HistoryAutoRecorderGroup.ExposeData))]
    public class HistoryAutoRecorderGroup_ExposeData_CallChildMethods
    {
        public static bool Prefix(HistoryAutoRecorderGroup __instance)
        {
            if (__instance.def != Defs.HistoryAutoRecorderGroupDefOf.RR_PawnWeightHistory)
                return true;

            HistoryAutoRecorderGroupWeight.Instance().ExposeData();
            return false;
        }
    }
}
