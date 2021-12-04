using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

using RimRound.Enums;
using RimRound.Utilities;
using RimRound.Defs;
using RimWorld;
using Verse.AI;
using RimRound.UI;

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
                cachedSliderPos1 = ranges.First;
                cachedSliderPos2 = ranges.Second;
            }

            Scribe_Values.Look<float>(ref cachedSliderPos1, "cachedSliderPos1", -1);
            Scribe_Values.Look<float>(ref cachedSliderPos2, "cachedSliderPos2", -1);

            Scribe_Values.Look<DietMode>(ref dietMode, "dietMode", DietMode.Disabled);
            Scribe_Values.Look<float>(ref currentFullness, "currentFullness", 0);
            Scribe_Values.Look<float>(ref currentFullnessToNutritionRatio, "currentFullnessToNutritionRatio", Values.defaultFullnessToNutritionRatio);
            Scribe_Values.Look<float>(ref hardLimitAdditionalPercentage, "hardLimitAdditionalPercentage", Values.defaultHardLimitAdditionalPercentage);
            Scribe_Values.Look<float>(ref personalStomachElasticity, "personalStomachElasticity", Values.defaultPersonalStomachElasticity);
            Scribe_Values.Look<float>(ref digestionRateBonusMult, "digestionRateBonusMult", 1f);
            Scribe_Values.Look<float>(ref digestionRateBonusFlat, "digestionRateBonusFlat", 0f);
            Scribe_Values.Look<float>(ref personalDigestionRateMult, "personalDigestionRateMult", 1f);
            Scribe_Values.Look<float>(ref personalDigestionRateFlat, "personalDigestionRateFlat", 0f);
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
                if (nutritionbar == null)
                    this.nutritionbar = new WeightGizmo_NutritionBar(((Pawn)parent));

                if (fullnessbar == null)
                    this.fullnessbar = new WeightGizmo_FullnessBar(((Pawn)parent));

                if (weightGizmo == null)
                    this.weightGizmo = new WeightGizmo(this);

                Update();
                SetRanges(cachedSliderPos1, cachedSliderPos2);
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!parent.Spawned)
                return;

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

        public void PassiveWeightLossTick() 
        {
            Functions.AddHediffSeverity(
                Defs.HediffDefOf.RimRound_Weight, 
                ((Pawn)parent), 
                Functions.NutritionToSeverity(-1 * ((Pawn)parent).needs.food.FoodFallPerTick) * GlobalSettings.ticksPerHungerCheck.threshold);
        }

        public void ActiveWeightGainTick(float nutrition) 
        {
            Functions.AddHediffSeverity(
                Defs.HediffDefOf.RimRound_Weight,
                ((Pawn)parent),
                Functions.NutritionToSeverity(nutrition));
        }

        public void FullnessCheckTick() 
        {

            float severity = (CurrentFullness > 0 ? CurrentFullness / HardLimit : 0.01f);
            Functions.SetHediffSeverity(Defs.HediffDefOf.RimRound_Fullness, (Pawn)parent, severity);

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
                SoftLimit += GlobalSettings.stomachElasticityMultiplier.threshold * personalStomachElasticity * Values.baseStomachElasticity * GlobalSettings.ticksPerHungerCheck.threshold;
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
            //DamageInfo stomachRuptureDamageInfo = new DamageInfo(Defs.DamageDefOf.RR_StomachBurst, currentStomachHealth - afterRuptureStomachHealth, 0, -1, null, pawnStomach);
            //((Pawn)parent).TakeDamage(stomachRuptureDamageInfo);
        }

        private float currentFullnessToNutritionRatio = Values.defaultFullnessToNutritionRatio;

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

        public void UpdateRatio(float nutrition, float ratio = Values.defaultFullnessToNutritionRatio) 
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
                    (((Pawn)parent)?.needs?.food.CurCategory == RimWorld.HungerCategory.Starving ? 
                    ((Pawn)parent)?.needs?.food.FoodFallPerTickAssumingCategory(RimWorld.HungerCategory.Hungry, false) : 
                    ((Pawn)parent)?.needs?.food?.FoodFallPerTick) * 
                    GlobalSettings.digestionRateMultiplier.threshold *
                    personalDigestionRateMult * 
                    digestionRateBonusMult * 
                    Values.baseDigestionRate) + 
                    personalDigestionRateFlat + 
                    digestionRateBonusFlat ?? 999f;
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


        public BodyTypeDef defaultBodyType;




        private float personalDigestionRateMult = 1f;
        private float personalDigestionRateFlat = 0f;
        private float digestionRateBonusMult = 1f;
        private float digestionRateBonusFlat = 0f;




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
        private float softLimitPersonal = Values.defaultSoftLimit + Values.RandomFloat(Values.softLimitVariation.x, Values.softLimitVariation.y);
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
        public float hardLimitAdditionalPercentage = Values.defaultHardLimitAdditionalPercentage;
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
        public float personalStomachElasticity = Values.defaultPersonalStomachElasticity;


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

        public void SetRanges(float first, float second)
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

        public void Update()
        {
            if (nutritionbar != null)
                nutritionbar.UpdateBar(this.DietMode);
            if (fullnessbar != null)
                fullnessbar.UpdateBar(this.DietMode);
        }


        public WeightGizmo weightGizmo;
        public WeightGizmo_FullnessBar fullnessbar;
        public WeightGizmo_NutritionBar nutritionbar;

        float cachedSliderPos1 = 0.30f;
        float cachedSliderPos2 = 0.90f;
    }
}
