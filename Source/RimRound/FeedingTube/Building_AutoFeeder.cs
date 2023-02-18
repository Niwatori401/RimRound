using RimRound.Comps;
using RimRound.FeedingTube.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimRound.FeedingTube
{
    [StaticConstructorOnStartup]
    public class Building_AutoFeeder : Building
    {
        int tickCheckInterval = 200;
        AutoFeederMode currentMode = AutoFeederMode.off;
        Pawn _currentPawn;
        public Pawn CurrentPawn 
        {
            get 
            {
                if (_currentPawn is null)
                    _currentPawn = forcedTarget.Pawn;

                return _currentPawn;
            }
            set 
            {
                _currentPawn = value;
            }
        }
        
        FullnessAndDietStats_ThingComp _cachedFNDComp = null;
        FullnessAndDietStats_ThingComp CachedFNDComp 
        {
            get 
            {
                if (_cachedFNDComp is null)
                    _cachedFNDComp = forcedTarget.Pawn.TryGetComp<FullnessAndDietStats_ThingComp>();

                return _cachedFNDComp;
            }
            set 
            { 
                _cachedFNDComp = value; 
            }
        }
        
        LocalTargetInfo forcedTarget = LocalTargetInfo.Invalid;

        FoodNetTrader_ThingComp _foodTraderComp = null;
        FoodNetTrader_ThingComp FoodNetTrader 
        {
            get 
            {
                if (_foodTraderComp is null) 
                {
                    _foodTraderComp = this.TryGetComp<FoodNetTrader_ThingComp>();
                }

                return _foodTraderComp;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_TargetInfo.Look(ref forcedTarget, "autofeederForcedTarget");
            Scribe_Values.Look<AutoFeederMode>(ref currentMode, "CurrentAutoFeederMode", AutoFeederMode.off);
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

        }

        public override void Tick()
        {
            base.Tick();
            if (GeneralUtility.IsHashIntervalTick(tickCheckInterval) && CurrentPawn != null)
            {
                float currentNutritionPercent = CurrentPawn.needs.food.CurLevelPercentage;
                switch (currentMode)
                {
                    case AutoFeederMode.off:
                        break;
                    case AutoFeederMode.lose:
                        if (currentNutritionPercent <= CurrentPawn.needs.food.PercentageThreshUrgentlyHungry)
                            FillPawnNutritionTo(CachedFNDComp, CurrentPawn.needs.food.PercentageThreshHungry);
                        break;
                    case AutoFeederMode.maintain:
                        if (currentNutritionPercent <= CurrentPawn.needs.food.PercentageThreshHungry)
                            FillPawnNutritionTo(CachedFNDComp, 0.8f);
                        break;
                    case AutoFeederMode.gain:
                        FillPawnStomachTo(CachedFNDComp, 0.9f);
                        break;
                    case AutoFeederMode.maxgain:
                        FillPawnStomachToHardLimit(CachedFNDComp);
                        break;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (forcedTarget != LocalTargetInfo.Invalid)
            {
                DrawFeedingTubeFromDeviceToPawn();
            }
        }


        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            float range = 9;
            GenDraw.DrawRadiusRing(base.Position, range);

            if (forcedTarget != LocalTargetInfo.Invalid) 
            {
                Vector3 targetLocation = forcedTarget.CenterVector3;

                targetLocation = forcedTarget.Thing.TrueCenter();

                Vector3 autoFeederCenter = this.TrueCenter();
                targetLocation.y = AltitudeLayer.MetaOverlays.AltitudeFor();
                autoFeederCenter.y = targetLocation.y;

                GenDraw.DrawLineBetween(autoFeederCenter, targetLocation, Building_TurretGun.ForcedTargetLineMat, 0.2f);
            }
        }


        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            { 
                yield return gizmo;
            }

            if (forcedTarget == LocalTargetInfo.Invalid)
                yield return GetStartActionGizmo();
            else
                yield return GetStopActionGizmo();

            yield return GetModeSwitchGizmo();
        }

        private void DrawFeedingTubeFromDeviceToPawn()
        {
            Vector3 autoFeederHoseAttachmentPoint = GetAutoFeederLocationVector();
            Vector3 mouthPosition = GetInBedTargetMouthPosition();

            GenDraw.DrawLineBetween(autoFeederHoseAttachmentPoint, mouthPosition, feedingTubeMat, 0.2f);
        }

        private Vector3 GetInBedTargetMouthPosition()
        {
            Vector3 targetLocation = forcedTarget.CenterVector3;

            targetLocation = forcedTarget.Pawn.TrueCenter();

            targetLocation.y = AltitudeLayer.MetaOverlays.AltitudeFor();

            float headOffsetInBed = -0.2f;
            targetLocation.z = targetLocation.z + headOffsetInBed;

            return targetLocation;
        }

        private Vector3 GetAutoFeederLocationVector()
        {
            Vector3 autoFeederHoseAttachPoint = this.TrueCenter();

            autoFeederHoseAttachPoint.y = AltitudeLayer.MetaOverlays.AltitudeFor();

            autoFeederHoseAttachPoint.x = autoFeederHoseAttachPoint.x + -0.11f;
            autoFeederHoseAttachPoint.z = autoFeederHoseAttachPoint.z + 0.36f;

            return autoFeederHoseAttachPoint;
        }


        private void FillPawnStomachTo(FullnessAndDietStats_ThingComp pawnFNDComp, float percentToFillTo) 
        {
            float currentFullness = pawnFNDComp.CurrentFullness;
            float targetFullnessVolume = pawnFNDComp.SoftLimit * percentToFillTo;

            if (pawnFNDComp.CurrentFullness >=  targetFullnessVolume)
            {
                return;
            }

            float fullnessToAdd = targetFullnessVolume - currentFullness;
            float netNutritionDensity = FoodNetTrader.FoodNet.FullnessToNutritionRatio;

            if (FoodNetTrader.FoodNet.Stored > fullnessToAdd)
            {
                pawnFNDComp.UpdateRatio(fullnessToAdd * (1 / netNutritionDensity), netNutritionDensity);
                pawnFNDComp.CurrentFullness = targetFullnessVolume;

                FoodNetTrader.FoodNet.Drain(fullnessToAdd);
            }
        }

        private void FillPawnStomachToHardLimit(FullnessAndDietStats_ThingComp pawnFNDComp)
        {
            float hardlimit = pawnFNDComp.HardLimit;
            float softlimit = pawnFNDComp.SoftLimit;

            FillPawnStomachTo(pawnFNDComp, (hardlimit / softlimit) - Values.MinRQ);
        }

        private void FillPawnNutritionTo(FullnessAndDietStats_ThingComp pawnFNDComp, float percentToFillTo) 
        {
            Pawn pawn = pawnFNDComp.parent.AsPawn();
            float nutritionCurrentPercentage = pawn.needs.food.CurLevelPercentage;

            if (percentToFillTo <= nutritionCurrentPercentage)
                return;

            float currentNutritionBeingDigested = pawnFNDComp.CurrentFullness / pawnFNDComp.CurrentFullnessToNutritionRatio;
            float currentNutritionBeingDigestedAsPercentOfNutrition = currentNutritionBeingDigested / pawn.needs.food.MaxLevel;


            float percentageToAdd = percentToFillTo - (nutritionCurrentPercentage + currentNutritionBeingDigestedAsPercentOfNutrition);

            if (percentageToAdd < 0)
                return;

            float rawNutritionToAdd = percentageToAdd * pawn.needs.food.MaxLevel;
            float currentFoodNetDensity = FoodNetTrader.FoodNet.FullnessToNutritionRatio;

            float fullnessToAdd = rawNutritionToAdd * currentFoodNetDensity;
            float currentFullness = pawnFNDComp.CurrentFullness;

            float fullnessPercent = (currentFullness + fullnessToAdd) / pawnFNDComp.SoftLimit;

            if (fullnessPercent > 1)
                fullnessPercent = 1;

            FillPawnStomachTo(pawnFNDComp, fullnessPercent);
        }



        Command_Action GetModeSwitchGizmo() 
        {
            Command_Action command_Action = new Command_Action();

            switch (currentMode) 
            {
                case AutoFeederMode.off:
                    command_Action.defaultLabel = "FeedingTubeModeLabel_off".Translate();
                    command_Action.defaultDesc = "FeedingTubeModeLabel_offDesc".Translate();
                    command_Action.icon = offIcon;
                    command_Action.action = delegate ()
                    {
                        currentMode = AutoFeederMode.lose;
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    };

                    break;
                case AutoFeederMode.lose:

                    command_Action.defaultLabel = "FeedingTubeModeLabel_lose".Translate();
                    command_Action.defaultDesc = "FeedingTubeModeLabel_loseDesc".Translate();
                    command_Action.icon = loseIcon;
                    command_Action.action = delegate ()
                    {
                        currentMode = AutoFeederMode.maintain;
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    };

                    break;
                case AutoFeederMode.maintain:
                    command_Action.defaultLabel = "FeedingTubeModeLabel_maintain".Translate();
                    command_Action.defaultDesc = "FeedingTubeModeLabel_maintainDesc".Translate();
                    command_Action.icon = maintainIcon;
                    command_Action.action = delegate ()
                    {
                        currentMode = AutoFeederMode.gain;
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    };

                    break;
                case AutoFeederMode.gain:
                    command_Action.defaultLabel = "FeedingTubeModeLabel_gain".Translate();
                    command_Action.defaultDesc = "FeedingTubeModeLabel_gainDesc".Translate();
                    command_Action.icon = gainIcon;
                    command_Action.action = delegate ()
                    {
                        currentMode = AutoFeederMode.maxgain;
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    };

                    break;
                case AutoFeederMode.maxgain:
                    command_Action.defaultLabel = "FeedingTubeModeLabel_maxgain".Translate();
                    command_Action.defaultDesc = "FeedingTubeModeLabel_maxgainDesc".Translate();
                    command_Action.icon = forceGainIcon;
                    command_Action.action = delegate ()
                    {
                        currentMode = AutoFeederMode.off;
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    };

                    break;
            }

            return command_Action;
        }

        Command_Action GetStartActionGizmo() 
        {
            Command_Action command_Action = new Command_Action();
            command_Action.defaultLabel = "FeedingTube_TargetPawn".Translate();
            command_Action.defaultDesc = "FeedingTube_TargetPawnDesc".Translate();
            command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true);
            command_Action.action = delegate ()
            {
                this.BeginTargeting();
                SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
            };

            //command_VerbTarget.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());

            return command_Action;
        }

        Command_Action GetStopActionGizmo() 
        {
            Command_Action command_Action = new Command_Action();
            command_Action.defaultLabel = "FeedingTube_StopFeedingLabel".Translate();
            command_Action.defaultDesc = "FeedingTube_StopFeedingDesc".Translate();
            command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt", true);
            command_Action.action = delegate ()
            {
                ResetForcedTarget();
                currentMode = AutoFeederMode.off;
                CachedFNDComp = null;
                SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
            };

            return command_Action;
        }

        private bool ValidateTarget(LocalTargetInfo target)
        {
            if (target.Pawn is null)
            {
                return false;
            }
            return true;
        }

        private void BeginTargeting()
        {
            Find.Targeter.BeginTargeting(this.TargetingParams,
                delegate (LocalTargetInfo t) //action
                    {
                        if (!this.ValidateTarget(t))
                        {
                            this.BeginTargeting();
                            return;
                        }
                        this.CurrentPawn = t.Pawn;
                        this.CachedFNDComp = t.Pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
                        Log.Message($"Targeted Pawn! {this.CurrentPawn.Name}");
                        this.CurrentPawn.health.AddHediff(Defs.HediffDefOf.RimRound_UsingFeedingTube);
                        forcedTarget = t;
                        SoundDefOf.Tick_High.PlayOneShotOnCamera(null);

                        return;
                    },
                delegate (LocalTargetInfo t) //highlight action
                    {
                        if (t.Pawn != null)
                        {
                            GenDraw.DrawTargetHighlight(t);
                            return;
                        }
                    },
                delegate (LocalTargetInfo t) //validator
                    {
                        if (t.Pawn is null || !t.Pawn.RaceProps.Humanlike || !t.Pawn.InBed())
                            return false;

                        return true;
                    }, 
                null, //Caster
                null, // Action when finished
                Defs.ThingDefOf.RR_FeedingTubeFluid.uiIcon, //Icon
                false); //play sound on action
        }

        private TargetingParameters TargetingParams
        {
            get
            {
                return new TargetingParameters
                {
                    canTargetPawns = true,
                    canTargetLocations = false
                };
            }
        }

        private void ResetForcedTarget()
        {
            this.forcedTarget = LocalTargetInfo.Invalid;
            this.CurrentPawn.health.RemoveHediff(
                (from h
                in this.CurrentPawn.health.hediffSet.hediffs
                where h.def.defName == Defs.HediffDefOf.RimRound_UsingFeedingTube.defName
                select h).FirstOrDefault()
                );
            this.CurrentPawn = null;
        }


        private static readonly Material feedingTubeMat = MaterialPool.MatFrom("Things/Building/Production/FeedingTubePipeThickMat", ShaderDatabase.Transparent, Color.white);
        private static readonly Texture2D offIcon = ContentFinder<Texture2D>.Get("UI/AutoFeeder/Modes/Off", true);
        private static readonly Texture2D loseIcon = ContentFinder<Texture2D>.Get("UI/AutoFeeder/Modes/Lose", true);
        private static readonly Texture2D maintainIcon = ContentFinder<Texture2D>.Get("UI/AutoFeeder/Modes/Maintain", true);
        private static readonly Texture2D gainIcon = ContentFinder<Texture2D>.Get("UI/AutoFeeder/Modes/Gain", true);
        private static readonly Texture2D forceGainIcon = ContentFinder<Texture2D>.Get("UI/AutoFeeder/Modes/ForceGain", true);

    }



    public enum AutoFeederMode 
    {
        off,
        lose,
        maintain,
        gain,
        maxgain,
    }
}
