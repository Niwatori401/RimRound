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
    public class Hediff_SuddenWeightGain : Hediff
    {
        public override void Tick()
        {
            base.Tick();

            if (!this.pawn.IsHashIntervalTick(60))
                return;

            var fndComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (fndComp is null)
                return;


            float kilosToAdd = GetStageWeightGainMultiplier();
            fndComp.activeWeightGainRequests.Enqueue(new WeightGainRequest(kilosToAdd, Find.TickManager.TicksGame + 10, 18000, true));

            fndComp.CumulativeSeverityKilosGained += kilosToAdd;

            AddOrMaxOutImmunityHediffIfOverMaxSeverity(fndComp);

            this.Severity -= (0.01f);
        }

        private void AddOrMaxOutImmunityHediffIfOverMaxSeverity(FullnessAndDietStats_ThingComp fndComp)
        {
            if (fndComp.CumulativeSeverityKilosGained > FullnessAndDietStats_ThingComp.severityUntilImmunity)
            {
                if (Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_SuddenWeightGainImmunity, this.pawn) is Hediff hediff)
                {
                    hediff.Severity = 1f;
                }
                else
                {                   
                    Utilities.HediffUtility.AddHediffSeverity(Defs.HediffDefOf.RimRound_SuddenWeightGainImmunity, pawn, 1.0f, true);
                }
            }
        }

        float GetStageWeightGainMultiplier()
        {
            switch (this.CurStageIndex)
            {
                case 0:
                    return 1;
                case 1:
                    return 3;
                case 2:
                    return 10;
                case 3:
                    return 20;
                case 4:
                    return 40;
                case 5:
                    return 100;
                default:
                    Log.Warning("Ran default case for sudden wg hediff curstage.");
                    break;
            }


            return 1;
        }

        public override float Severity 
        {
            get => base.Severity;
            set 
            {
                Hediff immunityHediff = null;
                if (value > 0)
                    immunityHediff = Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_SuddenWeightGainImmunity, this.pawn);

                
                //Increase the rate at which severity is lost if "Immune"
                
                if (!(immunityHediff is null) && value < 0)
                    value *= 10f;
                else if (!(immunityHediff is null) && value > 0)
                    value = 0;

                base.Severity = value;

            } 
        }
    }
}
