using RimRound.AI;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;
using static RimRound.Utilities.HungerDroneUtility;

namespace RimRound.Comps
{
    class CompCauseGameCondition_HungerDrone : CompCauseGameCondition
    {
        private Gender gender;
        private HungerDroneLevel droneLevel;
        private int ticksToIncreaseDroneLevel;

        public new CompProperties_CausesGameCondition_HungerDrone Props
        {
            get
            {
                return (CompProperties_CausesGameCondition_HungerDrone)this.props;
            }
        }

        public HungerDroneLevel Level 
        {
            get 
            {
                return Props.droneLevel;
            }
        }

        private bool DroneLevelIncreases
        {
            get
            {
                return this.Props.droneLevelIncreaseInterval != int.MinValue;
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.gender = (Rand.Bool ? Gender.Male : Gender.Female);
            this.droneLevel = this.Props.droneLevel;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (!respawningAfterLoad && this.DroneLevelIncreases)
            {
                this.ticksToIncreaseDroneLevel = this.Props.droneLevelIncreaseInterval;
                SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(this.parent.Map);
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!this.parent.Spawned || !this.DroneLevelIncreases || !base.Active)
            {
                return;
            }
            this.ticksToIncreaseDroneLevel--;
            if (this.ticksToIncreaseDroneLevel <= 0)
            {
                this.IncreaseDroneLevel();
                this.ticksToIncreaseDroneLevel = this.Props.droneLevelIncreaseInterval;
            }
        }

        private void IncreaseDroneLevel()
        {
            if (this.droneLevel == HungerDroneLevel.Severe)
            {
                return;
            }
            this.droneLevel += 1;
            //Change these!
            TaggedString text = "LetterPsychicDroneLevelIncreased".Translate(this.gender.GetLabel(false));
            Find.LetterStack.ReceiveLetter("LetterLabelPsychicDroneLevelIncreased".Translate(), text, LetterDefOf.NegativeEvent, null);
            //****************
            SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(this.parent.Map);
            base.ReSetupAllConditions();
        }

        protected override void SetupCondition(GameCondition condition, Map map)
        {
            base.SetupCondition(condition, map);
            GameCondition_PsychicHungerIncrease gameCondition_PsychicHungerIncrease = (GameCondition_PsychicHungerIncrease)condition;
            gameCondition_PsychicHungerIncrease.gender = this.gender;
            gameCondition_PsychicHungerIncrease.level = this.Level;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<Gender>(ref this.gender, "gender", Gender.None, false);
            Scribe_Values.Look<int>(ref this.ticksToIncreaseDroneLevel, "ticksToIncreaseDroneLevel", 0, false);
            Scribe_Values.Look<HungerDroneLevel>(ref this.droneLevel, "droneLevel", HungerDroneLevel.None, false);
        }

        public override string CompInspectStringExtra()
        {
            //Add stuff in here!
            return base.CompInspectStringExtra();
        }

        public override void RandomizeSettings(Site site)
        {
            this.gender = (Rand.Bool ? Gender.Male : Gender.Female);
            if (site.ActualThreatPoints < 400f)
            {
                this.droneLevel = HungerDroneLevel.Low;
                return;
            }
            if (site.ActualThreatPoints < 1000f)
            {
                this.droneLevel = HungerDroneLevel.Medium;
                return;
            }
            this.droneLevel = HungerDroneLevel.Severe;
        }
    }


}
