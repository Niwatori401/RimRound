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
    [HarmonyPatch(typeof(Corpse))]
    [HarmonyPatch(nameof(Corpse.InnerPawn), MethodType.Getter)]
    public class Corpse_InnerPawn_Get_UpdateBodyType
    {
        public static void Postfix(ref Pawn __result, Corpse __instance) 
        {
            if (__result is null)
                return;

            if (__instance.GetRotStage() == RotStage.Dessicated) // Don't update dessicated corpses so we can change their render size elsewhere
                return;

            PawnBodyType_ThingComp comp = __result.TryGetComp<PawnBodyType_ThingComp>();

            if (comp is null)
            {
                Log.Error($"Comp was null in {nameof(Corpse_InnerPawn_Get_UpdateBodyType.Postfix)} ");
                return;
            }

            //This prevents an infinite loop when the UpdatePawnSprite method ultimately calls Corpse.InnerPawn
            if (comp.ticksSinceLastBodyChange < comp.numberOfTicksCooldownPerChange)
                return;

            BodyTypeUtility.UpdatePawnSprite(__result, comp.PersonallyExempt, comp.CategoricallyExempt, true, true);
        }
    }
}
