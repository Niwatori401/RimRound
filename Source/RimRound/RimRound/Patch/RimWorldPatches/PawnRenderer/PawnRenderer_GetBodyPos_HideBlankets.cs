using HarmonyLib;
using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("GetBodyPos")]
    public class PawnRenderer_GetBodyPos_HideBlankets
    {
        public static void Postfix(PawnRenderer __instance, ref bool __1) 
        {
            if (((Pawn)Traverse.Create(__instance).Field("pawn").GetValue() is Pawn pawn) && pawn.TryGetComp<HideCovers_ThingComp>() is HideCovers_ThingComp comp) 
            {
                if (pawn.CurrentBed() is Building_Bed b) 
                {
                    if (b.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_z.defName ||
                        b.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_I.defName ||
                        b.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_II.defName ||
                        b.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_III.defName ||
                        b.def.defName == ThingDefOf.SleepingSpot.defName ||
                        b.def.defName == Defs.ThingDefOf.DoubleSleepingSpot.defName)
                    {
                        __1 = true;
                        return;
                    }
                    __1 = comp.HideCovers;
                }
                    
            }
        }
    }
}
