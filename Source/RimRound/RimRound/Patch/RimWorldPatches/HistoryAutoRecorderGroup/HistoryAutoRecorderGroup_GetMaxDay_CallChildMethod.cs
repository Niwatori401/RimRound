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
    [HarmonyPatch(nameof(HistoryAutoRecorderGroup.GetMaxDay))]
    public class HistoryAutoRecorderGroup_GetMaxDay_CallChildMethod
    {
        public static bool Prefix(HistoryAutoRecorderGroup __instance, ref float __result)
        {
            if (__instance.def != Defs.HistoryAutoRecorderGroupDefOf.RR_PawnWeightHistory)
                return true;

            __result = HistoryAutoRecorderGroupWeight.Instance().GetMaxDay();
            return false;
        }
    }
}
