using HarmonyLib;
using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    /// <summary>
    /// Let RR gas spread through vents.
    /// </summary>
    [HarmonyPatch(typeof(Building_Vent))]
    [HarmonyPatch(nameof(Building_Vent.TickRare))]
    public class Building_Vent_TickRare_DiffuseRRGasses
    {
        public static void Postfix(Building_Vent __instance) 
        {
            if (FlickUtility.WantsToBeOn(__instance))
            {
                var mapComp = Find.CurrentMap.GetComponent<MapComp_RRGasGrid>();

                if (mapComp is null)
                    return;

                mapComp.EqualizeGasThroughBuilding(__instance, true);
            }
        }
    }
}
