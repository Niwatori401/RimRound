using HarmonyLib;
using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using System.Reflection;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("GetBodyPos")]
    public class PawnRenderer_GetBodyPos_HideBlankets
    {
        static FieldInfo pawnFieldInfo = typeof(PawnRenderer).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
        static Dictionary<string, HideCovers_ThingComp> pawnIdToComp = new Dictionary<string, HideCovers_ThingComp>();

        public static void Postfix(PawnRenderer __instance, ref bool __1) 
        {
            Pawn pawn = (Pawn)pawnFieldInfo.GetValue(__instance);

            if (pawn is null || !(pawn?.RaceProps?.Humanlike is bool b && b))
                return;

            HideCovers_ThingComp comp = pawnIdToComp.TryGetValue(pawn.ThingID);

            if (comp is null)
            {
                comp = pawn.TryGetComp<HideCovers_ThingComp>();
                
                pawnIdToComp.Add(pawn.ThingID, comp);
            }

            if (comp is null)
                return;


            if (pawn.CurrentBed() is Building_Bed bed) 
            {
                if (bed.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_z.defName ||
                    bed.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_I.defName ||
                    bed.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_II.defName ||
                    bed.def.defName == Defs.ThingDefOf.BlobBed_FoldsOfHeaven_III.defName ||
                    bed.def.defName == ThingDefOf.SleepingSpot.defName ||
                    bed.def.defName == Defs.ThingDefOf.DoubleSleepingSpot.defName)
                {
                    __1 = true;
                    return;
                }
                __1 = comp.HideCovers;
            }
        }
    }
}
