using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimRound.Defs;
using RimRound.Utilities;
using RimWorld;

namespace RimRound.Hediffs
{
    public class Hediff_Fullness : Hediff
    {
        public override float PainOffset
        {
            get
            {
                return (base.PainOffset * GlobalSettings.fullnessHediffPainMult.threshold);
            }
        }
    }
}
