using RimRound.Utilities;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    public class WorldComp_SaveValues : WorldComponent
    {
        public WorldComp_SaveValues(World world) : base(world) 
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Gizmo Display Settings
            Scribe_Values.Look<bool>(ref GlobalSettings.showPawnDietManagementGizmo, "_showPawnDietManagementGizmo");
            Scribe_Values.Look<bool>(ref GlobalSettings.showSleepPostureManagementGizmo, "_showSleepPostureManagementGizmo");
            Scribe_Values.Look<bool>(ref GlobalSettings.showBlanketManagementGizmo, "_showBlanketManagementGizmo");
            Scribe_Values.Look<bool>(ref GlobalSettings.showExemptionGizmo, "_showExemptionGizmo");
            Scribe_Values.Look<bool>(ref GlobalSettings.showBlobIntobedGizmo, "_showBlobIntobedGizmo");

            //Body Change Exemption Settings
            Scribe_Values.Look<bool>(ref GlobalSettings.bodyChangeMale, "_bodyChangeMale");
            Scribe_Values.Look<bool>(ref GlobalSettings.bodyChangeFemale, "_bodyChangeFemale");
            Scribe_Values.Look<bool>(ref GlobalSettings.bodyChangeHostileNPC, "_bodyChangeHostileNPC");
            Scribe_Values.Look<bool>(ref GlobalSettings.bodyChangeFriendlyNPC, "_bodyChangeFriendlyNPC");
            Scribe_Values.Look<bool>(ref GlobalSettings.bodyChangePrisoners, "_bodyChangePrisoners");
            Scribe_Values.Look<bool>(ref GlobalSettings.bodyChangeSlaves, "_bodyChangeSlaves");

            //General Settings
            Scribe_Values.Look<bool>(ref GlobalSettings.showBodyTatoosForCustomSprites, "_showBodyTatoosForCustomSprites");
            Scribe_Values.Look<bool>(ref GlobalSettings.burstingEnabled, "_burstingEnabled");
            Scribe_Values.Look<bool>(ref GlobalSettings.haveWeightHediffMale, "_haveWeightHediffMale");
            Scribe_Values.Look<bool>(ref GlobalSettings.haveWeightHediffFemale, "_haveWeightHediffFemale");

            //Value Settings
            Scribe_Values.Look<float>(ref GlobalSettings.digestionRateMultiplier.threshold, "_globalDigestionRateMultiplier");
            Scribe_Values.Look<float>(ref GlobalSettings.weightGainMultiplier.threshold, "_globalWeightGainMultiplier");
            Scribe_Values.Look<float>(ref GlobalSettings.weightLossMultiplier.threshold, "_globalWeightLossMultiplier");
            Scribe_Values.Look<float>(ref GlobalSettings.fullnessMultiplier.threshold, "_globalFullnessMultiplier");
            Scribe_Values.Look<float>(ref GlobalSettings.digestionRateMultiplier.threshold, "_globalStomachElasticityMultiplier");
            Scribe_Values.Look<int>(ref GlobalSettings.ticksPerHungerCheck.threshold, "_ticksPerHungerCheck.threshold");
            Scribe_Values.Look<int>(ref GlobalSettings.ticksPerBodyChangeCheck.threshold, "_ticksPerBodyChangeCheck");
            Scribe_Values.Look<float>(ref GlobalSettings.softLimitMuliplier.threshold, "_globalSoftLimitMuliplier");
            Scribe_Values.Look<float>(ref GlobalSettings.hardLimitMuliplier.threshold, "_globalHardLimitMuliplier");
            Scribe_Values.Look<int>(ref GlobalSettings.stomachElasticityMultiplier.threshold, "_globalStomachElasticityMultiplier");
            Scribe_Values.Look<int>(ref GlobalSettings.stomachElasticityMultiplier.threshold, "_globalStomachElasticityMultiplier");
            Scribe_Values.Look<float>(ref GlobalSettings.FLD.threshold, "_globalFLDThreshold");
            Scribe_Values.Look<float>(ref GlobalSettings.aFLD.threshold, "_globalAFLDThreshold");
            Scribe_Values.Look<float>(ref GlobalSettings.diabetes.threshold, "_globalDiabetesThreshold");
            Scribe_Values.Look<float>(ref GlobalSettings.hypertension.threshold, "_globalHypertensionThreshold");

        }
    }
}
