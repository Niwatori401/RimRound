using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

using RimRound.UI;
using RimRound.Utilities;

namespace RimRound.Comps
{
    public class MakeBlobIntoBed_ThingComp : ThingComp
    {
        public MakeBlobIntoBed_ThingComp() 
        {
            gizmo = new MakeBlobIntoBedGizmo(this);
        }

        bool CheckBlobBedElligibility() 
        {
            if (Find.TickManager.TicksGame % ticksBetweenChecks == 0)
            {
                if (Functions.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, (Pawn)parent) is Hediff h &&
                    h?.Severity is float s &&
                    s >= Functions.KilosToSeverity(GlobalSettings.weightToBeBed.threshold))
                {
                    canBeBed = true;
                }
                else 
                {
                    canBeBed = false;
                    isBed = false;
                }
            }
            return canBeBed;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent.AsPawn().Downed)
                gizmo.Disable($"They must not be downed in order to be a bed!");
            else
                gizmo.disabled = false;

            if (GlobalSettings.showBlobIntobedGizmo && CheckBlobBedElligibility())
                yield return gizmo;
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref isBed, "blobIsBed", false);
        }

        bool isBed = false;
        bool canBeBed = false;
        public bool IsBed 
        {
            get 
            {
                return isBed;
            }
            set 
            {
                isBed = value;
            }
        }



        const int ticksBetweenChecks = 15;

        public MakeBlobIntoBedGizmo gizmo;

        public Thing blobBed;
    }
}
