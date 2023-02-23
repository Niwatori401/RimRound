using RimRound.Comps;
using RimRound.Utilities;
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

            StatChangeUtility.ChangeRimRoundStats(this.pawn, new RimRoundStatBonuses()
            {
                digestionRateMultiplier = 0.5f,
                weightGainMultiplier = 0.2f
            });
        }

        public override void PostRemoved()
        {
            base.PostRemoved();

            StatChangeUtility.ChangeRimRoundStats(this.pawn, new RimRoundStatBonuses()
            {
                digestionRateMultiplier = -0.5f,
                weightGainMultiplier = -0.2f
            });
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
