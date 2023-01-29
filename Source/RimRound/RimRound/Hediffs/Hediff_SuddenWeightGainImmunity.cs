using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Hediffs
{
    public class Hediff_SuddenWeightGainImmunity : Hediff
    {
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);

            var comp = this.pawn.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (comp is null)
            {
                Log.Error("FnDComp was null in Hediff_SuddenWeightGainImmunity PostAdd()");
                return;
            }

            comp.CumulativeSeverityKilosGained = 0;
        }

        // -1 severity after 1 quadrum
        public override void Tick()
        {
            base.Tick();
            if (!this.pawn.IsHashIntervalTick(5000))
                return;

            
            this.Severity -= 0.00555555555f;
        }
    }
}
