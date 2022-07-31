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
        int tickCheckInterval = 50;
        AutoFeederMode currentMode = AutoFeederMode.off;
        Pawn currentPawn;
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

        public override void Tick()
        {
            base.Tick();
            if (GeneralUtility.IsHashIntervalTick(tickCheckInterval))
            {
                switch (currentMode)
                {
                    case AutoFeederMode.off:
                        break;
                    case AutoFeederMode.lose:

                        break;
                    case AutoFeederMode.maintain:

                        break;
                    case AutoFeederMode.gain:

                        break;
                    case AutoFeederMode.maxgain:

                        break;
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            if (forcedTarget != LocalTargetInfo.Invalid)
            {
                Vector3 targetLocation = forcedTarget.CenterVector3;

                targetLocation = forcedTarget.Pawn.TrueCenter();

                Vector3 autoFeederCenter = this.TrueCenter();
                targetLocation.y = AltitudeLayer.MetaOverlays.AltitudeFor();
                autoFeederCenter.y = targetLocation.y;



                float headOffsetInBed = -0.2f;
                targetLocation.z = targetLocation.z + headOffsetInBed;


                GenDraw.DrawLineBetween(autoFeederCenter, targetLocation, feedingTubeMat, 0.2f);
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
                        this.currentPawn = t.Pawn;
                        Log.Message($"Targeted Pawn! {this.currentPawn.Name}");
                        this.currentPawn.health.AddHediff(Defs.HediffDefOf.RimRound_UsingFeedingTube);
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
            this.currentPawn.health.RemoveHediff(
                (from h
                in this.currentPawn.health.hediffSet.hediffs
                where h.def.defName == Defs.HediffDefOf.RimRound_UsingFeedingTube.defName
                select h).FirstOrDefault()
                );
            this.currentPawn = null;
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
