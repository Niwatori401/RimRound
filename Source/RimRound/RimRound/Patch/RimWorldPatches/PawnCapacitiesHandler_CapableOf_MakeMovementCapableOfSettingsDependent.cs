using HarmonyLib;
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
    [HarmonyPatch(typeof(PawnCapacitiesHandler))]
    [HarmonyPatch(nameof(PawnCapacitiesHandler.CapableOf))]
    public class PawnCapacitiesHandler_CapableOf_MakeMovementCapableOfSettingsDependent
    {
        public static bool Prefix(PawnCapacitiesHandler __instance, ref bool __result, PawnCapacityDef __0)
        {
            if (__0.defName != PawnCapacityDefOf.Moving.defName)
                return true;

            __result = __instance.GetLevel(__0) >= GlobalSettings.minForCapableMovement.threshold;

            return false;
        }
    }
}
