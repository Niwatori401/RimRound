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
    [HarmonyPatch(typeof(Pawn_GuestTracker))]
    [HarmonyPatch(nameof(Pawn_GuestTracker.SetGuestStatus))]
    public class Pawn_GuestTracker_SetGuestStatus_UpdateCategoricalExemptions
    {
        public static void Postfix(Pawn ___pawn)
        {
            BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true);

            ResetDietSettings(___pawn);
        }

        private static void ResetDietSettings(Pawn ___pawn)
        {
            if (___pawn is null)
                return;

            if (___pawn.TryGetComp<FullnessAndDietStats_ThingComp>() is FullnessAndDietStats_ThingComp comp)
            {
                comp.DietMode = DietMode.Nutrition;
                comp.SetRangesByPercent(0.3f, 0.9f);
            }
        }
    }
}
