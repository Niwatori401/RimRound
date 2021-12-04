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
    }
}

