using RimRound.Comps;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Hediffs
{
    public class Hediff_AIEncouragementChip : Hediff_AddedPart
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            StatChangeUtility.ChangeRimRoundStats(this.pawn, new RimRoundStatBonuses()
            {
                digestionRateMultiplier = 1f,
                weightGainMultiplier = 0.5f
            });
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            StatChangeUtility.ChangeRimRoundStats(this.pawn, new RimRoundStatBonuses()
            {
                digestionRateMultiplier = -1f,
                weightGainMultiplier = -0.5f
            });
        }

        public override string TipStringExtra
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(base.TipStringExtra);
                sb.AppendLine("Digestion rate +100%");
                sb.AppendLine("Weight gain rate +50%");
                sb.AppendLine("Conciousness -10%");
                sb.AppendLine("Mood +20");

                return sb.ToString();
            }
        }
    }
}
