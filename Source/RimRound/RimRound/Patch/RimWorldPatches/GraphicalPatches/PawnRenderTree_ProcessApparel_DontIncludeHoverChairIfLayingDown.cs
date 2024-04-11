using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnRenderTree))]
    [HarmonyPatch("ProcessApparel")]
    internal class PawnRenderTree_ProcessApparel_DontIncludeHoverChairIfLayingDown
    {
        public static bool Prefix(PawnRenderTree __instance, Apparel ap, PawnRenderNode headApparelNode, PawnRenderNode bodyApparelNode)
        {
            if (__instance.pawn.InBed() && (ap.def.defName == Defs.ThingDefOf.RR_HoverChair.defName || ap.def.defName == Defs.ThingDefOf.RR_HoverChairArm.defName))
            {
                return false;
            }

            return true;
        }
    }
}
