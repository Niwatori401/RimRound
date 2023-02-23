using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using UnityEngine;
using Verse;


namespace RimRound.Hediffs
{

    public class Hediff_Weight : Hediff
    {
        public static HediffDef_Weight ModExtension 
        {
            get 
            {
                return Defs.HediffDefOf.RimRound_Weight.GetModExtension<HediffDef_Weight>();
            }
        }

        public override string SeverityLabel 
        {
            get 
            {
                if (GlobalSettings.usePoundsWherePossible)
                    return $"{Utilities.HediffUtility.SeverityToKilosWithBaseWeight(this.Severity) * 2.20462f:F1}Lbs";
                else
                    return $"{Utilities.HediffUtility.SeverityToKilosWithBaseWeight(this.Severity):F1}Kgs";
            }
        } 
        public override int CurStageIndex
        {
            get 
            {
                if (this.def.stages == null)
                {
                    return 0;
                }
                List<HediffStage> stages = this.def.stages;
                float severity = this.Severity;
                float weightRequirementMultiplier = GlobalSettings.varyMinWeightForBodyTypeByBodySize ? RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(this.pawn) : 1;
                for (int i = stages.Count - 1; i >= 0; i--)
                {
                    if (severity >= (stages[i].minSeverity * weightRequirementMultiplier))
                    {
                        return i;
                    }
                }
                return 0;
            }
        }

        /// <summary>
        /// 1 is no mitigation, 0 is full mitigation
        /// </summary>
        public float MovementPenaltyMitigationFactor 
        {
            get
            {
                var comp = this?.pawn?.TryGetComp<FullnessAndDietStats_ThingComp>();
                if (comp is null)
                {
                    Log.Error("Failed to get comp in PersonalFullnessPainMult!");
                    return 1f;
                }
                return Mathf.Clamp01(1 - comp.statBonuses.movementPenaltyMitigationMultBonus_Weight);
            }      
        }

        /// <summary>
        /// 1 is no mitigation, 0 is full mitigation
        /// </summary>
        public float ManipulationPenaltyMitigationFactor
        {
            get
            {
                var comp = this?.pawn?.TryGetComp<FullnessAndDietStats_ThingComp>();
                if (comp is null)
                {
                    Log.Error("Failed to get comp in PersonalFullnessPainMult!");
                    return 1f;
                }
                return Mathf.Clamp01(1 - comp.statBonuses.manipulationPenaltyMitigationMultBonus_Weight);
            }
        }
    }
}

