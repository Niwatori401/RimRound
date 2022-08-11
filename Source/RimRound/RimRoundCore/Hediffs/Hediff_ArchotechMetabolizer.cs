using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Hediffs
{
    public class Hediff_ArchotechMetabolizer : Hediff_AddedPart
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            var comp = this.pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp != null)
            {
                comp.PersonalDigestionRateMult += 1f;
                comp.PersonalWeightGainModifier += 0.5f;
                comp.personalStomachElasticity += 1f;
            }
            Hediff_Fullness fullnessHediff = Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Fullness, this.pawn) as Hediff_Fullness;

            if (fullnessHediff != null)
            {
                fullnessHediff.PersonalFullnessPainMult = fullnessHediff.PersonalFullnessPainMult - 0.50f;
                fullnessHediff.PersonalFullnessEatingSpeedMult = fullnessHediff.PersonalFullnessEatingSpeedMult - 0.40f;
                fullnessHediff.PersonalFullnessMovementMult = fullnessHediff.PersonalFullnessMovementMult - 0.40f;
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            var comp = this.pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp != null)
            {
                comp.PersonalDigestionRateMult -= 1f;
                comp.PersonalWeightGainModifier -= 0.5f;
                comp.personalStomachElasticity -= 1f;
            }

            Hediff_Fullness fullnessHediff = Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Fullness, this.pawn) as Hediff_Fullness;
            
            if (fullnessHediff != null)
            {
                fullnessHediff.PersonalFullnessPainMult = fullnessHediff.PersonalFullnessPainMult + 0.50f;
                fullnessHediff.PersonalFullnessEatingSpeedMult = fullnessHediff.PersonalFullnessEatingSpeedMult + 0.40f;
                fullnessHediff.PersonalFullnessMovementMult = fullnessHediff.PersonalFullnessMovementMult + 0.40f;
            }
        }

        public override string TipStringExtra
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(base.TipStringExtra);
                sb.AppendLine("Digestion rate +100%");
                sb.AppendLine("Weight gain rate +50%");
                sb.AppendLine("Personal stomach elasticity +100%");
                sb.AppendLine("Pain from fullness -50%");
                sb.AppendLine("Movement penalty from fullness -40%");
                sb.AppendLine("Eating speed penalty from fullness -40%");
                return sb.ToString();
            }
        }
    }
}
