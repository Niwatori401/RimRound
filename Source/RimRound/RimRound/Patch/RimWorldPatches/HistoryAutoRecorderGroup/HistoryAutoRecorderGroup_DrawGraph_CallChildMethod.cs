using HarmonyLib;
using RimRound.AI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(HistoryAutoRecorderGroup))]
    [HarmonyPatch(nameof(HistoryAutoRecorderGroup.DrawGraph))]
    internal class HistoryAutoRecorderGroup_DrawGraph_CallChildMethod
    {
        public static bool Prefix(HistoryAutoRecorderGroup __instance, Rect graphRect, Rect legendRect, FloatRange section, List<CurveMark> marks)
        {
            if (__instance.def != Defs.HistoryAutoRecorderGroupDefOf.RR_PawnWeightHistory)
                return true;

            HistoryAutoRecorderGroupWeight.Instance().DrawGraph(graphRect, legendRect, section, marks);
            return false;
        }
    }
}
