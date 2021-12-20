using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.FeedingTube.Patches
{
    [HarmonyPatch(typeof(ThingListGroupHelper))]
    [HarmonyPatch(nameof(ThingListGroupHelper.Includes))]
    public class ThingListGroupHelper_Includes_AddSupportForFaucet
    {
        public static void Postfix(ThingRequestGroup group, ThingDef def, ref bool __result) 
        {
            //(group == ThingRequestGroup.FoodDispenser && def.thingClass == typeof(Building_FoodFaucet)) ||
            if ((group == ThingRequestGroup.FoodSource && def.thingClass == typeof(Building_FoodFaucet)) ||
                (group == ThingRequestGroup.FoodSourceNotPlantOrTree && def.thingClass == typeof(Building_FoodFaucet))) 
            {
                __result = true;
            }
        }
    }
}
