using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimRound.Comps;
using RimRound.Utilities;

namespace RimRound.Patch
{
    public class Thing_Ingested_StomachBurstCheck
    {
        public static void Postfix(FullnessAndDietStats_ThingComp comp) 
        {
            if (comp is null)
                return;

            if (GlobalSettings.burstingEnabled)
                comp.RuptureStomachCheckTick();
        }
    }
}
