using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnCapacityUtility))]
    [HarmonyPatch(nameof(PawnCapacityUtility.CalculateCapacityLevel))]
    class PawnCapacityUtility_CalculateCapacityLevel_AlterForPerksAndSooter
    {
        static bool Prefix(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors)
        {
            bool shouldSkipParentFunc = AlterMovementIfWearingScooter(ref __result, diffSet, capacity, impactors);
            
            return shouldSkipParentFunc;
        }

        static void Postfix(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors) 
        {
            AlterConciousnessForPerks(ref __result, diffSet, capacity, impactors);
            AlterManipulationForPerks(ref __result, diffSet, capacity, impactors);
            AlterMovementForPerks(ref __result, diffSet, capacity, impactors);

            return;
        }

        private static void AlterManipulationForPerks(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors) 
        {
            if (capacity == PawnCapacityDefOf.Manipulation)
            {
                Pawn pawn = diffSet.pawn;

                if (!pawn.RaceProps.Humanlike)
                    return;

                int heavyRevianLevel = pawn.TryGetComp<FullnessAndDietStats_ThingComp>()?.perkLevels.PerkToLevels?["RR_HeavyRevian_Title"] ?? 0;
                int itsComingThisWayLevel = pawn.TryGetComp<FullnessAndDietStats_ThingComp>()?.perkLevels.PerkToLevels?["RR_ItsComingThisWay_Title"] ?? 0;

                if (heavyRevianLevel > 0)
                {
                    __result += 0.60f;
                }

                if (itsComingThisWayLevel > 0)
                {
                    float gelatinous11SeverityThreshold = 21.85f * RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn);

                    if (pawn.WeightHediff().Severity > gelatinous11SeverityThreshold)
                        __result = 0;
                    else
                        __result = Mathf.Max(0.05f, __result);
                }
            }

            return;
        }


        private static void AlterMovementForPerks(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors)
        {
            if (capacity == PawnCapacityDefOf.Moving)
            {
                Pawn pawn = diffSet.pawn;


                if (!pawn.RaceProps.Humanlike)
                    return;

                int heavyRevianLevel = pawn.TryGetComp<FullnessAndDietStats_ThingComp>()?.perkLevels.PerkToLevels?["RR_HeavyRevian_Title"] ?? 0;
                int itsComingThisWayLevel = pawn.TryGetComp<FullnessAndDietStats_ThingComp>()?.perkLevels.PerkToLevels?["RR_ItsComingThisWay_Title"] ?? 0;
                //int comfortableCorpulenceLevel = pawn.TryGetComp<FullnessAndDietStats_ThingComp>()?.perkLevels.PerkToLevels?["RR_Comfortable_Corpulence_Title"] ?? 0;


                if (heavyRevianLevel > 0)
                {
                    __result += 0.60f;
                }
                /*
                if (comfortableCorpulenceLevel > 0)
                {
                    __result += comfortableCorpulenceLevel * 0.03f;                
                }
                */
                if (itsComingThisWayLevel > 0)
                {
                    float gelatinous11SeverityThreshold = 21.85f * RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn);

                    if (pawn.WeightHediff().Severity > gelatinous11SeverityThreshold)
                        __result = 0;
                    else
                        __result = Mathf.Max(0.05f, __result);
                }
            }

            return;

        }

        private static void AlterConciousnessForPerks(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors)
        {
            //Heavy revian
            if (capacity == PawnCapacityDefOf.Consciousness)
            {
                Pawn pawn = diffSet.pawn;

                if (pawn is null)
                    return;

                int heavyRevianLevel = pawn.TryGetComp<FullnessAndDietStats_ThingComp>()?.perkLevels?.PerkToLevels?["RR_HeavyRevian_Title"] ?? 0;

                if (!pawn.RaceProps.Humanlike)
                    return;

                __result += heavyRevianLevel * 0.2f;
            }

            return;
        }

        private static bool AlterMovementIfWearingScooter(ref float __result, HediffSet diffSet, PawnCapacityDef capacity, List<PawnCapacityUtility.CapacityImpactor> impactors)
        {

            if (capacity == PawnCapacityDefOf.Moving)
            {
                Pawn pawn = diffSet.pawn;

                float scooterSpeed = 0.5f + 0.25f * pawn.TryGetComp<FullnessAndDietStats_ThingComp>()?.perkLevels.PerkToLevels?["RR_PracticalProblems_Title"] ?? 1;

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
