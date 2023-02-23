using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    public static class StatChangeUtility
    {
        public static void ChangeRimRoundStats(Pawn pawn, RimRoundStatBonuses bonus)
        {
            var fndComp = pawn?.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (fndComp is null)
                return;

            HandleBaseRimRoundStats(fndComp, bonus);
        }

        private static void HandleBaseRimRoundStats(FullnessAndDietStats_ThingComp fndComp, RimRoundStatBonuses bonus) 
        {
            fndComp.statBonuses.digestionRateFlatBonus += bonus.digestionRateFlatBonus;
            fndComp.statBonuses.digestionRateMultiplier += bonus.digestionRateMultiplier;

            fndComp.statBonuses.hardLimitAdditionalPercentageMultBonus += bonus.hardLimitAdditionalPercentageMultBonus;
            fndComp.statBonuses.hardLimitAdditionalPercentageMultiplier += bonus.hardLimitAdditionalPercentageMultiplier;

            fndComp.statBonuses.softLimitFlatBonus += bonus.softLimitFlatBonus;
            fndComp.statBonuses.softLimitMultiplier += bonus.softLimitMultiplier;
            fndComp.statBonuses.weightGainMultBonus += bonus.weightGainMultBonus;
            fndComp.statBonuses.weightGainMultiplier += bonus.weightGainMultiplier;

            fndComp.statBonuses.weightLossMultBonus += bonus.weightLossMultBonus;
            fndComp.statBonuses.weightLossMultiplier += bonus.weightLossMultiplier;

            fndComp.statBonuses.stomachElasticityMultiplier += bonus.stomachElasticityMultiplier;

            fndComp.statBonuses.stomachElasticityFlatBonus += bonus.stomachElasticityFlatBonus;
            fndComp.statBonuses.fullnessGainedMultBonus += bonus.fullnessGainedMultBonus;
            fndComp.statBonuses.movementFlatBonus += bonus.movementFlatBonus;
            fndComp.statBonuses.manipulationFlatBonus += bonus.manipulationFlatBonus;
            fndComp.statBonuses.eatingFlatBonus += bonus.eatingFlatBonus;
            fndComp.statBonuses.movementPenaltyMitigationMultBonus_Weight += bonus.movementPenaltyMitigationMultBonus_Weight;
            fndComp.statBonuses.movementPenaltyMitigationMultBonus_Fullness += bonus.movementPenaltyMitigationMultBonus_Fullness;
            fndComp.statBonuses.eatingSpeedReductionMitigationMultBonus_Fullness += bonus.eatingSpeedReductionMitigationMultBonus_Fullness;
            fndComp.statBonuses.manipulationPenaltyMitigationMultBonus_Weight += bonus.manipulationPenaltyMitigationMultBonus_Weight;
            fndComp.statBonuses.painMitigationMultBonus_Fullness += bonus.painMitigationMultBonus_Fullness;
        }

    }

    /// <summary>
    /// Values are calculated as follows: Result = Clamp(baseMultiplier * (1 + statBonuses.STATMULT) + STATBONUS, min, max)
    /// Values for each stat can be negative, though they will be clamped if the result is below the minimum.
    /// Ultimately these stats are affected by global multipliers.
    /// </summary>
    public struct RimRoundStatBonuses
    {
        // Final value clamped to [0, inf)
        public float weightGainMultiplier;
        public float weightLossMultiplier;
        public float digestionRateMultiplier;
        public float softLimitMultiplier;
        public float stomachElasticityMultiplier;
        // Final value clamped [0.3, 10]
        public float hardLimitAdditionalPercentageMultiplier;


        // Final value clamped to [0, inf)
        public float weightGainMultBonus;
        public float weightLossMultBonus;

        //public float digestionRateMultBonus;
        //public float softLimitMultBonus;

        public float digestionRateFlatBonus;
        public float softLimitFlatBonus;


        //public float stomachElasticityMultBonus;
        public float stomachElasticityFlatBonus;
        // Final value clamped [0.3, 2]
        public float hardLimitAdditionalPercentageMultBonus;

        //Should be in range [0, inf)
        public float fullnessGainedMultBonus;

        public float movementFlatBonus;
        public float manipulationFlatBonus;
        public float eatingFlatBonus;

        //All below should be in range [0, 1]. 0 means full mitigation, 1 means none.
        public float movementPenaltyMitigationMultBonus_Weight;
        public float movementPenaltyMitigationMultBonus_Fullness;
        public float eatingSpeedReductionMitigationMultBonus_Fullness;
        public float manipulationPenaltyMitigationMultBonus_Weight;

        public float painMitigationMultBonus_Fullness;
    }


}
