using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

using RimRound.Utilities;
using RimRound.Defs;
using RimWorld;
using Verse.AI;
using RimRound.UI;
using UnityEngine;
using System.Reflection;

namespace RimRound.Comps
{
    public class FullnessAndDietStats_ThingComp : ThingComp
    {
        public FullnessAndDietStats_ThingComp() 
        {
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                Pair<float, float> ranges = GetRanges();
                cachedSliderVal1 = ranges.First;
                cachedSliderVal2 = ranges.Second;
            }

            Scribe_Values.Look<float>(ref cachedSliderVal1, "cachedSliderPos1", -1);
            Scribe_Values.Look<float>(ref cachedSliderVal2, "cachedSliderPos2", -1);

            Scribe_Values.Look<DietMode>(ref dietMode,                     "dietMode",                        DietMode.Disabled);
            Scribe_Values.Look<float>(ref currentFullness,                 "currentFullness",                 0);
            Scribe_Values.Look<float>(ref softLimitPersonal,               "softLimit",                       defaultSoftLimit);
            Scribe_Values.Look<float>(ref currentFullnessToNutritionRatio, "currentFullnessToNutritionRatio", defaultFullnessToNutritionRatio);
            Scribe_Values.Look<float>(ref hardLimitAdditionalPercentage,   "hardLimitAdditionalPercentage",   defaultHardLimitAdditionalPercentage);
            Scribe_Values.Look<float>(ref personalStomachElasticity,       "personalStomachElasticity",       defaultPersonalStomachElasticity);
            Scribe_Values.Look<float>(ref globalDigestionRateBonusMult,    "digestionRateBonusMult",          1f);
            Scribe_Values.Look<float>(ref globalDigestionRateBonusFlat,    "digestionRateBonusFlat",          0f);
            Scribe_Values.Look<float>(ref personalDigestionRateMult,       "personalDigestionRateMult",       1f);
            Scribe_Values.Look<float>(ref personalDigestionRateFlat,       "personalDigestionRateFlat",       0f);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (GlobalSettings.showPawnDietManagementGizmo && ShouldShowWeightGizmo())
                yield return this.weightGizmo;

            if (Prefs.DevMode) 
            {
                yield return new Command_Action
                {
                    defaultLabel = "Fill stomach",
                    action = delegate ()
                    {
                        this.CurrentFullness = this.HardLimit - Values.MinRQ;
                    }
                };

                yield return new Command_Action
                {
                    defaultLabel = "Empty stomach",
                    action = delegate ()
                    {
                        this.CurrentFullness = 0;
                    }
                };
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (((Pawn)parent)?.RaceProps.Humanlike ?? false)
            {
                if (Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, parent.AsPawn()) is null)
                    Utilities.HediffUtility.AddHediffOfDefTo(Defs.HediffDefOf.RimRound_Weight, parent.AsPawn());

                if (Utilities.HediffUtility.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Fullness, parent.AsPawn()) is null)
                    Utilities.HediffUtility.AddHediffOfDefTo(Defs.HediffDefOf.RimRound_Fullness, parent.AsPawn());


                if (nutritionbar == null)
                    this.nutritionbar = new WeightGizmo_NutritionBar(((Pawn)parent));

                if (fullnessbar == null)
                    this.fullnessbar = new WeightGizmo_FullnessBar(((Pawn)parent));

                if (weightGizmo == null)
                    this.weightGizmo = new WeightGizmo(this);

                Update();
                SetRangesByValue(cachedSliderVal1, cachedSliderVal2);
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!parent.Spawned)
                return;

            ProcessWeightGainRequests(GlobalSettings.ticksBetweenWeightGainRequestProcess.threshold);

            if (parent?.IsHashIntervalTick(GlobalSettings.ticksPerHungerCheck.threshold) ?? false) 
            {
                float digestedAmt = DigestionTick() / CurrentFullnessToNutritionRatio;
                ((Pawn)parent).needs.food.CurLevel += digestedAmt;
                ActiveWeightGainTick(digestedAmt);
                PassiveWeightLossTick();
                FullnessCheckTick();
                StomachGrowthTick();

                if (GlobalSettings.burstingEnabled)
                    RuptureStomachCheckTick();
            }
        }

        static MethodInfo HungerRateIgnoringMalnutritionMI = typeof(Need_Food).GetProperty("HungerRateIgnoringMalnutrition", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);

        public void ProcessWeightGainRequests(int ticksBetweenChecks) 
        {
            if (!GeneralUtility.IsHashIntervalTick(ticksBetweenChecks))
                return;

            if (weightGainRequestDelayTracker > 0)
            {
                weightGainRequestDelayTracker -= ticksBetweenChecks;
                return;
            }

            weightGainRequestDelayTracker = 0;

            if (this.activeWeightGainRequests.Count > 0) 
            {
                WeightGainRequest gainRequest = this.activeWeightGainRequests.Dequeue();

                weightGainRequestDelayTracker += gainRequest.delayAmount;

                Utilities.HediffUtility.AddHediffSeverity(
                    Defs.HediffDefOf.RimRound_Weight, 
                    this.parent.AsPawn(), 
                    Utilities.HediffUtility.KilosToSeverityWithoutBaseWeight(gainRequest.amountToGain));

                var pbtThingComp = parent.TryGetComp<PawnBodyType_ThingComp>();
                if (pbtThingComp is null)
                    return;

                BodyTypeUtility.UpdatePawnSprite(parent.AsPawn(), pbtThingComp.PersonallyExempt, pbtThingComp.CategoricallyExempt);
            }

            return;
        }

        float weightGainRequestDelayTracker = 0;

        public void PassiveWeightLossTick() 
        {
            Utilities.HediffUtility.AddHediffSeverity(
                Defs.HediffDefOf.RimRound_Weight, 
                ((Pawn)parent),
                Utilities.HediffUtility.NutritionToSeverity(-1 * 2.6666667E-05f * (float)(HungerRateIgnoringMalnutritionMI.Invoke(parent.AsPawn().needs.food, null)) * GlobalSettings.ticksPerHungerCheck.threshold));
        }

        public void ActiveWeightGainTick(float nutrition) 
        {
            Utilities.HediffUtility.AddHediffSeverity(
                Defs.HediffDefOf.RimRound_Weight,
                ((Pawn)parent),
                Utilities.HediffUtility.NutritionToSeverity(nutrition));
        }

        public void FullnessCheckTick() 
        {

            float severity = (CurrentFullness > 0 ? CurrentFullness / HardLimit : 0.01f);
            Utilities.HediffUtility.SetHediffSeverity(Defs.HediffDefOf.RimRound_Fullness, (Pawn)parent, severity);

            return;
        }

        public void RuptureStomachCheckTick() 
        {
            float severity = (CurrentFullness > 0 ? CurrentFullness / HardLimit : 0.01f);

            if (severity > Defs.HediffDefOf.RimRound_Fullness.stages.Last().minSeverity)
            {
                float vomitChance = Values.RandomFloat(0, 1);
                if (vomitChance >= 0.50)
                    ((Pawn)parent).jobs.StartJob(
                        JobMaker.MakeJob(RimWorld.JobDefOf.Vomit),
                        JobCondition.InterruptForced,
                        null, true, true, null, null, false, false);

                RuptureStomach();

                CurrentFullness = SoftLimit * (1 - Values.RandomFloat(0.1f, 0.4f));
            }

            return;
        }

        public void StomachGrowthTick()
        {
            if (CurrentFullness > SoftLimit)
            {
                SoftLimit += GlobalSettings.stomachElasticityMultiplier.threshold * personalStomachElasticity * baseStomachElasticity * GlobalSettings.ticksPerHungerCheck.threshold;
                return;
            }
            return;
        }

        //Used in a CompTick(). Returns the amount of fullness lost.
        public float DigestionTick()
        {
            float amountToSubtract = DigestionRate * GlobalSettings.ticksPerHungerCheck.threshold;

            if (CurrentFullness > amountToSubtract)
            {
                CurrentFullness -= amountToSubtract;
                return amountToSubtract;
            }
            else
            {
                float tmp = CurrentFullness;
                CurrentFullness = 0;
                return tmp;
            }
        }

        public void RuptureStomach() 
        {
            BodyPartRecord pawnStomach = ((Pawn)parent).RaceProps.body.GetPartsWithDef(BodyPartDefOf.Stomach).First();
            float currentStomachHealth = ((Pawn)parent).health.hediffSet.GetPartHealth(pawnStomach);
            float afterRuptureStomachHealth = 2;

            if (currentStomachHealth <= afterRuptureStomachHealth)
                return;

            int numberOfWounds = Values.RandomInt(1, 5);
            float damageVariationPercent = 0.50f;
            float remainingDamage = (currentStomachHealth - afterRuptureStomachHealth);
            float damagePerWound = remainingDamage / numberOfWounds;

            while (remainingDamage > 0) 
            {
                float thisDmg = Values.RandomFloat(1 - damageVariationPercent, 1 + damageVariationPercent) * damagePerWound;

                if (thisDmg > remainingDamage)
                    thisDmg = remainingDamage;

                DamageInfo stomachRuptureDamageInfo = new DamageInfo(
                    Defs.DamageDefOf.RR_StomachBurst,
                    thisDmg, 0, -1, null, pawnStomach);

                remainingDamage -= thisDmg;

                ((Pawn)parent).TakeDamage(stomachRuptureDamageInfo);
            }
        }

        private float currentFullnessToNutritionRatio = defaultFullnessToNutritionRatio;

        public float CurrentFullnessToNutritionRatio 
        {
            get 
            {
                return currentFullnessToNutritionRatio;
            }
            set 
            {
                currentFullnessToNutritionRatio = value;
            }
        }

        public void UpdateRatio(float nutrition, float ratio = defaultFullnessToNutritionRatio) 
        {
            //Weighted average of current values and incoming values
            CurrentFullnessToNutritionRatio = 
                ((nutrition * ratio) * ratio + CurrentFullness * CurrentFullnessToNutritionRatio) /
                ((nutrition * ratio) + CurrentFullness);
        }


        //Digestion rate per tick
        public float DigestionRate
        {
            get
            {
                return (
                    (2.6666667E-05f * (float)(HungerRateIgnoringMalnutritionMI.Invoke(parent.AsPawn().needs.food, null))) * 
                    GlobalSettings.digestionRateMultiplier.threshold *
                    PersonalDigestionRateMult *
                    GlobalDigestionRateBonusMult * 
                    baseDigestionRate) +
                    PersonalDigestionRateFlat +
                    GlobalDigestionRateBonusFlat;
            }
        }


        public DietMode DietMode
        {
            get
            {
                return dietMode;
            }
            set
            {
                dietMode = value;
                this.Update();
            }

        }

        private DietMode dietMode = DietMode.Nutrition;

        public DietMode preCaravanDietMode;

        public BodyTypeDef defaultBodyType;



        public float PersonalDigestionRateMult 
        {
            get => personalDigestionRateMult * HungerDroneUtility.GetCurrentHungerMultiplierFromDrone(this.parent.AsPawn());
            set => personalDigestionRateMult = value;
        }
        private float personalDigestionRateMult = 1f;

        public float PersonalDigestionRateFlat 
        {
            get => personalDigestionRateFlat;
            set => personalDigestionRateFlat = value;
        }
        private float personalDigestionRateFlat = 0f;


        public float GlobalDigestionRateBonusMult 
        {
            get => globalDigestionRateBonusMult;
            set => globalDigestionRateBonusMult = value;
        }
        private static float globalDigestionRateBonusMult = 1f;

        public float GlobalDigestionRateBonusFlat
        {
            get => globalDigestionRateBonusFlat;
            set => globalDigestionRateBonusFlat = value;
        }
        private static float globalDigestionRateBonusFlat = 0f;




        public float RemainingFullnessUntil(float limit) 
        {
            return limit - CurrentFullness;
        }


        //Current actual fullness. Displayed as the bar
        private float currentFullness = 0.0f;

        public float CurrentFullness
        {
            get
            {
                return currentFullness;
            }
            set
            {
                currentFullness = value;
            }
        }


        //-------------------

        //In liters. represents threshold of stoach capacity. 
        private float softLimitPersonal = defaultSoftLimit + Values.RandomFloat(softLimitVariation.x, softLimitVariation.y);
        public float SoftLimit 
        {
            get 
            {
                return softLimitPersonal * GlobalSettings.softLimitMuliplier.threshold;
            }
            set 
            {
                softLimitPersonal = value;
            }
        }

        public bool SetAboveHardLimit 
        {
            get 
            {
                return fullnessbar.peaceForeverHeld;
            }
        }

        //How much more than the soft limit the Hard Limit is.
        public float hardLimitAdditionalPercentage = defaultHardLimitAdditionalPercentage;
        public float HardLimit
        {
            get
            {
                return SoftLimit * (1f + hardLimitAdditionalPercentage) * GlobalSettings.hardLimitMuliplier.threshold;
            }
            set 
            {
                if (value <= SoftLimit) 
                {
                    Log.Error("Hard Limit set below soft limit!");
                }

                hardLimitAdditionalPercentage = (value / SoftLimit) - 1f;
            }
        }


        //Multiplier for how quickly the stomach grows when over the soft limit.
        public float personalStomachElasticity = defaultPersonalStomachElasticity;


        bool ShouldShowWeightGizmo()
        {
            List<object> selectedPawns = new List<object>();
            foreach (object o in Find.Selector.SelectedObjects)
            {
                if (o.AsPawn() is null)
                {
                    continue;
                }
                else
                {
                    selectedPawns.Add(o);
                }
            }

            if (selectedPawns.Count > 1)
                return false;
            else
                return true;
        }

        public Pair<float, float> GetRanges()
        {
            if (nutritionbar is null || fullnessbar is null)
                return new Pair<float, float>(-1, -1);

            switch (this.DietMode)
            {
                case DietMode.Nutrition:
                    return nutritionbar.GetRanges();
                case DietMode.Hybrid:
                    return new Pair<float, float>(
                        nutritionbar.GetRanges().First,
                        fullnessbar.GetRanges().First);
                case DietMode.Fullness:
                    return fullnessbar.GetRanges();
                case DietMode.Disabled:
                    return new Pair<float, float>(-1, -1);
                default:
                    Log.Error("WeightGizmo_ThingComp GetRanges() ran default case!");
                    return new Pair<float, float>(-1, -1);
            }
        }


        public void SetRangesByValue(float first, float second) 
        {
            float maxNutrition = nutritionbar.needFood.MaxLevel;
            float maxDisplayFullness = fullnessbar.DisplayLimit;
            switch (this.DietMode)
            {
                case DietMode.Nutrition:
                    nutritionbar.SetRanges(first / maxNutrition, second / maxNutrition);
                    return;
                case DietMode.Hybrid:
                    nutritionbar.SetRanges(first / maxNutrition, 0);
                    fullnessbar.SetRanges(second / maxDisplayFullness, 0);
                    return;
                case DietMode.Fullness:
                    fullnessbar.SetRanges(first / maxDisplayFullness, second / maxDisplayFullness);
                    return;
                case DietMode.Disabled:
                    return;
                default:
                    Log.Error("WeightGizmo_ThingComp SetRanges() ran default case!");
                    return;
            }

        }
        public void SetRangesByPercent(float first, float second)
        {
            switch (this.DietMode)
            {
                case DietMode.Nutrition:
                    nutritionbar.SetRanges(first, second);
                    return;
                case DietMode.Hybrid:
                    nutritionbar.SetRanges(first, 0);
                    fullnessbar.SetRanges(second, 0);
                    return;
                case DietMode.Fullness:
                    fullnessbar.SetRanges(first, second);
                    return;
                case DietMode.Disabled:
                    return;
                default:
                    Log.Error("WeightGizmo_ThingComp SetRanges() ran default case!");
                    return;
            }
        }


        //100% for fullness is hardlimit
        public void SetRangesPercent(float first, float second) 
        {
            if (first > 1 || second > 1)
            {
                Log.Error("Inputs for SetRangesPercent must be less than or equal to 1!");
                return;
            }
                
            float maxNutrition = parent.AsPawn().needs.food.MaxLevel;
            float maxFullness = this.HardLimit;

            switch (this.DietMode)
            {
                case DietMode.Nutrition:
                    nutritionbar.SetRanges(first * maxNutrition, second * maxNutrition);
                    return;
                case DietMode.Hybrid:
                    nutritionbar.SetRanges(first * maxNutrition, 0);
                    fullnessbar.SetRanges(second * maxFullness, 0);
                    return;
                case DietMode.Fullness:
                    fullnessbar.SetRanges(first * maxFullness, second * maxFullness);
                    return;
                case DietMode.Disabled:
                    return;
                default:
                    Log.Error("WeightGizmo_ThingComp SetRanges() ran default case!");
                    return;
            }

        }

        public void Update()
        {
            if (nutritionbar != null)
                nutritionbar.UpdateBar(this.DietMode);
            if (fullnessbar != null)
                fullnessbar.UpdateBar(this.DietMode);
        }

        public Queue<WeightGainRequest> activeWeightGainRequests = new Queue<WeightGainRequest>();

        public WeightGizmo weightGizmo;
        public WeightGizmo_FullnessBar fullnessbar;
        public WeightGizmo_NutritionBar nutritionbar;

        float cachedSliderVal1 = 0.30f;
        float cachedSliderVal2 = 0.90f;

        public const float defaultSoftLimit = 0.9f;
        public static Vector2 softLimitVariation = new Vector2(-0.10f, 0.50f);
        public const float defaultHardLimitAdditionalPercentage = 0.3f;
        public const float defaultPersonalStomachElasticity = 1f;
        //How much (in liters) the stomach grows when over the softlimit per tick.
        public const float baseStomachElasticity = 0.00001f;
        public const float baseDigestionRate = 3.0f;
        public const float defaultFullnessToNutritionRatio = 1f; //i.e. 0.5 Fullness for 1 nutrition is 0.5f
    }


    public struct WeightGainRequest
    {
        public WeightGainRequest(float amountToGain, float delayAmount) 
        {
            this.amountToGain = amountToGain;
            this.delayAmount = delayAmount;
        }
        public float amountToGain;
        public float delayAmount;
    }


    public enum DietMode
    {
        Nutrition,
        Hybrid,
        Fullness,
        Disabled
    }
}
