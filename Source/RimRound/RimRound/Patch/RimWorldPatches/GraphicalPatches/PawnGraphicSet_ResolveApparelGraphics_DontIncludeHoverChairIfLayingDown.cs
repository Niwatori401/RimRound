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
    //[HarmonyPatch(typeof(PawnGraphicSet))]
    //[HarmonyPatch(nameof(PawnGraphicSet.ResolveApparelGraphics))]
    //internal class PawnGraphicSet_ResolveApparelGraphics_DontIncludeHoverChairIfLayingDown
    //{
    //    public static void Postfix(ref PawnGraphicSet __instance) 
    //    {
    //        if (__instance.pawn.InBed()) 
    //        {
    //            ApparelGraphicRecord? agr = __instance?.apparelGraphics?.Find(
    //                x => x.sourceApparel.def.defName == Defs.ThingDefOf.RR_HoverChair.defName || 
    //                x.sourceApparel.def.defName == Defs.ThingDefOf.RR_HoverChairArm.defName);

    //            if (agr.GetValueOrDefault() is ApparelGraphicRecord a) 
    //            {
    //                __instance.apparelGraphics.Remove(a);
    //            }
    //        }
    //    }
    //}
}
