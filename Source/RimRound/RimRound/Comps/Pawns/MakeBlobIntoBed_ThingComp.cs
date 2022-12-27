using RimWorld;
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

        }

        bool CheckBlobBedElligibility() 
        {
            if (Find.TickManager.TicksGame % ticksBetweenChecks == 0)
            {
                if (Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, (Pawn)parent) is Hediff h &&
                    h?.Severity is float s &&
                    s >= Utilities.HediffUtility.KilosToSeverityWithBaseWeight(GlobalSettings.weightToBeBed.threshold))
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

        public override void PostPostMake()
        {
            base.PostPostMake();
            fndComp = parent.AsPawn().TryGetComp<FullnessAndDietStats_ThingComp>();
            if (fndComp is null)
            {
                Log.Error("Comp was null in MakeBlobIntoBed ctor");
                return;
            }
            gizmo = new MakeBlobIntoBedGizmo(this, fndComp);
            gizmoRec = new MakeRecreationSpotGizmo(this);

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (gizmo is null || gizmoRec is null || fndComp is null)
                yield break;

            if (!parent.AsPawn().InBed())
            {
                gizmo.Disable($"They must be in a bed to be one!");
                gizmoRec.Disable("They must be in a bed to be a rec spot!");
            }
            else
            {
                gizmo.disabled = false;
                gizmoRec.disabled = false;
            }

            if (GlobalSettings.showBlobIntobedGizmo && CheckBlobBedElligibility())
            {
                yield return gizmo;

                if ((fndComp?.perkLevels?.PerkToLevels?["RR_WeLikeToParty_Title"] ?? 0) > 0)
                    yield return gizmoRec;
            }
                
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref isBed, "blobIsBed", false);
            Scribe_Values.Look<bool>(ref _isRecSpot, "isRecSpot", false);
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

        bool _isRecSpot = false;
        public bool IsRecSpot 
        {
            get { return _isRecSpot; }
            set { _isRecSpot = value; }
        }



        const int ticksBetweenChecks = 15;

        public MakeBlobIntoBedGizmo gizmo;
        public MakeRecreationSpotGizmo gizmoRec;
        private FullnessAndDietStats_ThingComp fndComp;
        public Thing recSpot;
        public Thing blobBed;
    }
}
