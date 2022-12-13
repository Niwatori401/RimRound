using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch(nameof(Pawn.GetGizmos))]
    internal class Pawn_GetGizmos_AddRRGizmosForPrisoners
    {
        static void Postfix(ref IEnumerable<Gizmo> __result, Pawn __instance) 
        {
            if (__instance.IsPrisonerOfColony) 
            {
                List<Gizmo> gizmos = __result.ToList();

                var compSleepingPosition = __instance.TryGetComp<RimRound.Comps.SleepingPosition_ThingComp>();
                var compHideCovers = __instance.TryGetComp<RimRound.Comps.HideCovers_ThingComp>();
                var compDietSlider = __instance.TryGetComp<RimRound.Comps.FullnessAndDietStats_ThingComp>();

                gizmos.AddRange(AddGizmosIfNotVisible(compSleepingPosition));
                gizmos.AddRange(AddGizmosIfNotVisible(compHideCovers));
                gizmos.AddRange(AddGizmosIfNotVisible(compDietSlider));

                __result = gizmos.AsEnumerable<Gizmo>();
            }

            return;
        }

        static List<Gizmo> AddGizmosIfNotVisible(ThingComp comp) 
        {
            List<Gizmo> gizmos = new List<Gizmo>();

            if (comp != null)
                foreach (var gizmo in comp.CompGetGizmosExtra())
                    if (!gizmo.Visible)
                        gizmos.Add(gizmo);

            return gizmos;
        }
    }
}
