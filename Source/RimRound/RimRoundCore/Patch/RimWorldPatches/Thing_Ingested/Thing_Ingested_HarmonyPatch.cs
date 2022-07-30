using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("Ingested", new Type[] { typeof(Pawn), typeof(float) })]
    public class Thing_Ingested_HarmonyPatch
    {
        public static void Postfix(Thing __instance, Pawn __0,
            ref float __result)
        {
            FullnessAndDietStats_ThingComp comp = __0?.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (GeneralUtility.IsNotNull(comp)) 
            {
                Thing_Ingested_AddFullness.Postfix(__instance, __0, ref __result, comp);
                Thing_Ingested_StomachBurstCheck.Postfix(comp);
            }

        }
    }
}
