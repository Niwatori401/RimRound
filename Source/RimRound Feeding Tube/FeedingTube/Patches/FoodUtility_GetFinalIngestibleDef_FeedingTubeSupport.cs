using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.Patches
{
    [HarmonyPatch(typeof(FoodUtility))]
    [HarmonyPatch(nameof(FoodUtility.GetFinalIngestibleDef))]
    public class FoodUtility_GetFinalIngestibleDef_FeedingTubeSupport
    {
        public static void Postfix(Thing __0, ref ThingDef __result) 
        {
            Building_FoodFaucet feedingTube = __0 as Building_FoodFaucet;
            if (feedingTube != null)
                __result = Defs.ThingDefOf.RR_FeedingTubeFluid;

            return; 
        }
    }
}
