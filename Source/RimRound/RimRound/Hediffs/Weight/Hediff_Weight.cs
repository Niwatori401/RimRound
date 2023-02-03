using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RimRound.Utilities;
using RimWorld;
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
    }
}

