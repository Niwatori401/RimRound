using RimRound.Comps;
using RimRound.Hediffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    //All fields in this class are saved in WorldComp_SaveValues.cs. Do not add reference type fields without adding exceptions to saving code
    public static class GlobalSettings
    {
        #region Hediff Settings

        public static NumericFieldData<float> weightHediffManipulationPenaltyMult = new NumericFieldData<float>(1, 0, 100);
        public static NumericFieldData<float> weightHediffMovementPenaltyMult = new NumericFieldData<float>(1, 0, 100);
        public static NumericFieldData<float> weightHediffHungerRateMult = new NumericFieldData<float>(1, 0, 100);

        public static NumericFieldData<float> weightHediffRestRateMult = new NumericFieldData<float>(1, 0, 100);

        public static NumericFieldData<float> fullnessHediffPainMult = new NumericFieldData<float>(1, 0, 100);
        public static NumericFieldData<float> fullnessHediffMovementPenaltyMult = new NumericFieldData<float>(1, 0, 100);
        public static NumericFieldData<float> fullnessHediffEatingPenaltyMult = new NumericFieldData<float>(1, 0, 100);



        #endregion



        #region Gizmo Display Settings

        public static bool showSpecialDebugSettings = false;
        public static bool showPawnDietManagementGizmo = true;
        public static bool showSleepPostureManagementGizmo = true;
        public static bool showBlanketManagementGizmo = true;
        public static bool showExemptionGizmo = false;
        public static bool showBlobIntobedGizmo = true;
        public static bool largeDietGizmo = false;

        #endregion

        #region Body Change Exemption Settings

        public static bool bodyChangeMale = false;
        public static bool bodyChangeFemale = true;
        public static bool bodyChangeHostileNPC = false;
        public static bool bodyChangeFriendlyNPC = true;
        public static bool bodyChangePrisoners = true;
        public static bool bodyChangeSlaves = true;


        #endregion

        #region General Settings 

        public static bool showBodyTatoosForCustomSprites = false;
        public static bool burstingEnabled = false;
        public static bool haveWeightHediffMale = true;
        public static bool haveWeightHediffFemale = true;
        public static bool preferDefaultOutfitOverNaked = true;
        public static bool alternateNorthHeadPositionForRRBodytypes = false;
        public static bool moodletsForWeightOpinions = true;
        public static bool varyMinWeightForBodyTypeByBodySize = true;
        public static bool useOldLardySprite = false;
        public static bool useZoomPortraitStyle = false;
        public static bool onlyUseStandardBodyType = false;
        public static bool hidePacksForCustomBodies = true;
        public static bool usePoundsWherePossible = false;

        #endregion

        #region Value Settings

        public static NumericFieldData<int> minimumAgeForCustomBody = new NumericFieldData<int>(20, 0, 1000);
        public static NumericFieldData<float> minForCapableMovement = new NumericFieldData<float>(0.01f, 0, 1);
        public static NumericFieldData<float> diabetes = new NumericFieldData<float>(1, 0, 1);
        public static NumericFieldData<float> aFLD = new NumericFieldData<float>(1, 0, 1);
        public static NumericFieldData<float> FLD = new NumericFieldData<float>(1, 0, 1);
        public static NumericFieldData<int> weightToAdjustWiggleAngle = new NumericFieldData<int>(130, 0, int.MaxValue);
        public static NumericFieldData<int> weightToBeBed = new NumericFieldData<int>(700, 0, int.MaxValue);
        public static NumericFieldData<int> ticksPerHungerCheck = new NumericFieldData<int>(150, 30, 1000);
        public static NumericFieldData<int> ticksPerBodyChangeCheck = new NumericFieldData<int>(150, 30, 1000);
        public static NumericFieldData<float> hardLimitMuliplier = new NumericFieldData<float>(1, 0.5f, 10);
        public static NumericFieldData<float> softLimitMuliplier = new NumericFieldData<float>(1, 0.5f, 10);
        public static NumericFieldData<int> stomachElasticityMultiplier = new NumericFieldData<int>(1, 0, 10);
        public static NumericFieldData<float> fullnessMultiplier = new NumericFieldData<float>(1, 0, 10);
        public static NumericFieldData<float> weightLossMultiplier = new NumericFieldData<float>(1, 0, 1000);
        public static NumericFieldData<float> weightLossMultiplierMale = new NumericFieldData<float>(1, 0, 1000);
        public static NumericFieldData<float> weightLossMultiplierFemale = new NumericFieldData<float>(1, 0, 1000);
        public static NumericFieldData<float> weightGainMultiplier = new NumericFieldData<float>(1, 0, 1000);
        public static NumericFieldData<float> weightGainMultiplierMale = new NumericFieldData<float>(1, 0, 1000);
        public static NumericFieldData<float> weightGainMultiplierFemale = new NumericFieldData<float>(1, 0, 1000);
        public static NumericFieldData<float> digestionRateMultiplier = new NumericFieldData<float>(1, 0, 100);
        public static NumericFieldData<float> hypertension = new NumericFieldData<float>(1, 0, 1);
        public static NumericFieldData<int> maxWeight = new NumericFieldData<int>(999999999, Hediff_Weight.ModExtension.baseWeight, int.MaxValue - 1);
        public static NumericFieldData<int> minWeight = new NumericFieldData<int>((int)Hediff_Weight.ModExtension.baseWeight, Hediff_Weight.ModExtension.baseWeight, int.MaxValue - 1);
        public static NumericFieldData<int> ticksBetweenWeightGainRequestProcess = new NumericFieldData<int>(15, 5, 600);
        public static NumericFieldData<float> meatMultiplierForWeight = new NumericFieldData<float>(1, 0, 10);
        public static NumericFieldData<int> maxVisualSizeGelLevel = new NumericFieldData<int>(20, 1, 20);
        public static NumericFieldData<int> nutritionPerPerkLevel = new NumericFieldData<int>(15, 5, 30);
        public static NumericFieldData<int> levelsGainedPerLevel = new NumericFieldData<int>(1, 1, 10);
        public static NumericFieldData<float> minWeightChangeForNumberText = new NumericFieldData<float>(0.5f, 0.01f, float.MaxValue);
        
        
        
        //Mod specific multipliers
        public static NumericFieldData<float> milkMultiplierForWeight = new NumericFieldData<float>(1, 0, 100);

        #endregion
    }

    public struct NumericFieldData<T> 
    {
        public NumericFieldData(T initialValue, float min, float max) 
        {
            threshold = initialValue;
            this.min = min;
            this.max = max;
            stringBuffer = null;
        }

        public T threshold;
        public readonly float max;
        public readonly float min;
        public string stringBuffer;
    }

    public struct NutritionTable
    {
        public float Undefined;
        public float NeverForNutrition;
        public float DesperateOnly;
        public float DesperateOnlyForHumanlikes;
        public float RawBad;
        public float RawTasty;
        public float MealAwful;
        public float MealSimple;
        public float MealFine;
        public float MealLavish;
    }
}
