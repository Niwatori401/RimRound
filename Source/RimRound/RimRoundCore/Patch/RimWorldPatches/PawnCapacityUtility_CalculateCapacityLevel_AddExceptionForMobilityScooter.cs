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
    [HarmonyPatch(typeof(PawnCapacityUtility))]
    [HarmonyPatch(nameof(PawnCapacityUtility.CalculateCapacityLevel))]
    class PawnCapacityUtility_CalculateCapacityLevel_AddExceptionForMobilityScooter
    {
        static bool Prefix(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors)
        {
            bool shouldSkipParentFunc = AlterMovementIfWearingScooter(ref __result, diffSet, capacity, impactors);
            return shouldSkipParentFunc;
        }

        private static bool AlterMovementIfWearingScooter(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors)
        {
            float scooterSpeed = 0.5f;

            if (capacity == PawnCapacityDefOf.Moving)
            {
                Pawn pawn = diffSet.pawn;

                if (!pawn.RaceProps.Humanlike || !PawnCapacityUtility.BodyCanEverDoCapacity(pawn.RaceProps.body, PawnCapacityDefOf.Manipulation))
                    return true;

                if (PawnCapacityUtility.CalculateCapacityLevel(diffSet, PawnCapacityDefOf.Manipulation, impactors) < .10)
                {
                    __result = 0;
                    return false;
                }

                if (capacity.zeroIfCannotBeAwake && !diffSet.pawn.health.capacities.CanBeAwake)
                {
                    return true;
                }

                if (IsAHumanlikePawn(pawn) && MobilityChairUtility.IsWearingAMobilityScooter(pawn))
                {
                    __result = scooterSpeed;
                    return false;
                }
            }

            return true;
        }

        static bool IsAHumanlikePawn(Pawn p) 
        {
            return p != null && p.RaceProps.Humanlike;
        }
    }
}
