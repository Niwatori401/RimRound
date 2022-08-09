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
    public class Hediff_BionicMetabolizer : Hediff_AddedPart
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            var comp = this.pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp != null)
            {
                comp.PersonalDigestionRateMult += 0.5f;
                comp.PersonalWeightGainModifier += 0.2f;
            }
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            var comp = this.pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (comp != null)
            {
                comp.PersonalDigestionRateMult -= 0.5f;
                comp.PersonalWeightGainModifier -= 0.2f;
            }

        }

        public override string TipStringExtra
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(base.TipStringExtra);
                sb.AppendLine("Digestion rate +50%");
                sb.AppendLine("Weight Gain rate +20%");

                return sb.ToString();
            }
        }
    }
}
