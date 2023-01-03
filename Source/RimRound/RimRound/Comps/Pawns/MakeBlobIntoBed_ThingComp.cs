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
                    _isBed = false;
                }
            }
            return canBeBed;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            fndComp = parent.AsPawn().TryGetComp<FullnessAndDietStats_ThingComp>();
            if (fndComp is null)
            {
                Log.Error("Comp was null in MakeBlobIntoBed ctor");
                return;
            }

            gizmo = new MakeBlobIntoBedGizmo(this, fndComp);
            gizmoRec = new MakeRecreationSpotGizmo(this);
            generatorGizmo = new MakeGeneratorGizmo(this, fndComp);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (gizmo is null || gizmoRec is null || generatorGizmo is null || fndComp is null)
                yield break;

            if (!parent.AsPawn().InBed())
            {
                gizmo.Disable($"They must be in a bed to be one!");
                gizmoRec.Disable("They must be in a bed to be a rec spot!");
                generatorGizmo.Disable("They must be in a bed to be a generator!");
            }
            else
            {
                gizmo.disabled = false;
                gizmoRec.disabled = false;
                generatorGizmo.disabled = false;
            }

            if (GlobalSettings.showBlobIntobedGizmo && CheckBlobBedElligibility())
            {
                yield return gizmo;

                if ((fndComp?.perkLevels?.PerkToLevels?["RR_WeLikeToParty_Title"] ?? 0) > 0)
                    yield return gizmoRec;

                if ((fndComp?.perkLevels?.PerkToLevels?["RR_PaunchPower_Title"] ?? 0) > 0)
                    yield return generatorGizmo;
            }
                
        }


        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref _isBed, "blobIsBed", false);
            Scribe_Values.Look<bool>(ref _isRecSpot, "isRecSpot", false);
            Scribe_Values.Look<bool>(ref _isPowerSpot, "isPowerSpot", false);

            Scribe_References.Look<Thing>(ref recSpot, "recSpot");
            Scribe_References.Look<Thing>(ref generatorSpot, "generatorSpot");
            Scribe_References.Look<Thing>(ref blobBed, "blobBed");
        }

        bool _isBed = false;
        bool canBeBed = false;
        public bool IsBed 
        {
            get 
            {
                return _isBed;
            }
            set 
            {
                _isBed = value;
            }
        }

        bool _isRecSpot = false;
        public bool IsRecSpot 
        {
            get { return _isRecSpot; }
            set { _isRecSpot = value; }
        }

        bool _isPowerSpot = false;
        public bool IsPowerSpot
        {
            get { return _isPowerSpot; }
            set { _isPowerSpot = value; }
        }


        const int ticksBetweenChecks = 15;

        public MakeBlobIntoBedGizmo gizmo;
        public MakeRecreationSpotGizmo gizmoRec;
        public MakeGeneratorGizmo generatorGizmo;


        private FullnessAndDietStats_ThingComp fndComp;
        public Thing recSpot;
        public Thing generatorSpot;
        public Thing blobBed;
    }
}
