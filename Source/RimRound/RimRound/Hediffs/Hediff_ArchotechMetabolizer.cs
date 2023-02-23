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
    public class Hediff_ArchotechMetabolizer : Hediff_AddedPart
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            StatChangeUtility.ChangeRimRoundStats(this.pawn, new RimRoundStatBonuses()
            {
                stomachElasticityMultiplier = 1f,
                digestionRateMultiplier = 1f,
                weightGainMultiplier = 0.5f,
                painMitigationMultBonus_Fullness = 0.5f,
                eatingSpeedReductionMitigationMultBonus_Fullness = 0.4f,
                movementPenaltyMitigationMultBonus_Fullness = 0.4f
            });
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            StatChangeUtility.ChangeRimRoundStats(this.pawn, new RimRoundStatBonuses()
            {
                stomachElasticityMultiplier = -1f,
                digestionRateMultiplier = -1f,
                weightGainMultiplier = -0.5f,
                painMitigationMultBonus_Fullness = -0.5f,
                eatingSpeedReductionMitigationMultBonus_Fullness = -0.4f,
                movementPenaltyMitigationMultBonus_Fullness = -0.40f
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
                sb.AppendLine("Personal stomach elasticity +100%");
                sb.AppendLine("Pain from fullness -50%");
                sb.AppendLine("Movement penalty from fullness -40%");
                sb.AppendLine("Eating speed penalty from fullness -40%");
                return sb.ToString();
            }
        }
    }
}
