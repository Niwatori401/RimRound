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
    public class FoodUtility_GetFinalIngestibleDef_AddFoodFaucetSupport
    {
        public static bool Prefix(Thing __0, ref ThingDef __result) 
        {
            if (__0 is Building_FoodFaucet)
            {
                __result = Defs.ThingDefOf.RR_FeedingTubeFluid;
                return false;
            }
            return true;
        }
    }
}
