using RimRound.Comps;
using RimRound.Hediffs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Utilities
{
    public static class HediffUtility
    {
        public static float NutritionToSeverity(float nutrition)
        {
            return (nutrition / Values.nutritionPerKilo) * Values.severityPerKilo;
        }

        public static float SeverityToKilosWithoutBaseWeight(float severity)
        {
            return severity / Values.severityPerKilo;
        }

        public static float SeverityToKilosWithBaseWeight(float severity)
        {
            return (severity / Values.severityPerKilo) + Hediff_Weight.ModExtension.baseWeight;
        }

        public static float KilosToSeverityWithBaseWeight(float weightIncludingBaseWeight)
        {
            return (weightIncludingBaseWeight - Hediff_Weight.ModExtension.baseWeight) * Values.severityPerKilo;
        }

        public static float KilosToSeverityWithoutBaseWeight(float weightInKilograms)
        {
            return weightInKilograms * Values.severityPerKilo;
        }

        public static Hediff WeightHediff(this Pawn pawn) 
        {
            if (GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn) is Hediff weight)
            {
                return weight;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns>Pawn weight including base weight in kilograms</returns>
        public static float Weight(this Pawn pawn)
        {
            if (GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn) is Hediff weight)
            {
                return (float)SeverityToKilosWithBaseWeight(weight.Severity);
            }
            else
            {
                return 0;
            }
        }

        public static Hediff AddHediffOfDefTo(HediffDef def, Pawn pawn)
        {
            if (pawn is null)
                return null;

            Hediff pawnHediff = GetHediffOfDefFrom(def, pawn);

            Hediff hediff = null;
            if (pawnHediff is null)
            {
                hediff = HediffMaker.MakeHediff(def, pawn);
                hediff.Severity = 0.01f;
                pawn.health.AddHediff(hediff);
            }

            return hediff;
        }

        public static bool RemoveHediffOfDefFrom(HediffDef def, Pawn pawn)
        {
            Hediff h = pawn?.health?.hediffSet?.hediffs?.Find(x => x.def == def) ?? null;

            if (h is null)
            {
                return false;
            }

            return pawn.health.hediffSet.hediffs.Remove(h);
        }

        public static Hediff GetHediffOfDefFrom(HediffDef def, Pawn pawn)
        {
            if (pawn is null || def is null)
                return null;

            return pawn?.health?.hediffSet?.hediffs?.Find(x => x.def.defName == def.defName) ?? null;
        }

        public static void AddHediffSeverity(Hediff hediff, Pawn pawn, float amount, bool triggerMessages = true) 
        {
            if (hediff.def.defName == Defs.HediffDefOf.RimRound_Weight.defName)
            {
                _HandleWeightHediff(hediff, pawn, amount, triggerMessages);
            }
            else
            {
                hediff.Severity += amount;
            }
        }

        private static void _HandleWeightHediff(Hediff hediff, Pawn pawn, float amount, bool triggerMessages)
        {
            float cachedSeverity = hediff.Severity;
            float realSeverityApplied;
            if (amount > 0)
            {
                realSeverityApplied = _AddWeight(hediff, pawn, amount);
            }
            else
            {
                realSeverityApplied = _LoseWeight(hediff, pawn, amount);
            }


            ThrowValueText(pawn, realSeverityApplied, 1, GlobalSettings.minWeightChangeForNumberText.threshold / 1000f);


            if (triggerMessages && GetWeightChangedMessage(pawn, cachedSeverity, hediff.Severity) is Message m)
                Messages.Message(m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hediff"></param>
        /// <param name="pawn"></param>
        /// <param name="amount"></param>
        /// <returns>Weight attempted to be lost</returns>
        private static float _LoseWeight(Hediff hediff, Pawn pawn, float amount)
        {
            float personalWeightLossModifier = pawn.TryGetComp<FullnessAndDietStats_ThingComp>()?.WeightLossMultiplier is float p ? p : 1f;

            float additionalSeverity =
                personalWeightLossModifier *
                amount;

            if (SeverityToKilosWithBaseWeight(hediff.Severity + additionalSeverity) < GlobalSettings.minWeight.threshold)
                hediff.Severity = KilosToSeverityWithBaseWeight(GlobalSettings.minWeight.threshold);
            else
                hediff.Severity += additionalSeverity;

            return additionalSeverity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hediff"></param>
        /// <param name="pawn"></param>
        /// <param name="amount"></param>
        /// <returns>Weight attempted to be gained</returns>
        private static float _AddWeight(Hediff hediff, Pawn pawn, float amount)
        {
            var comp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();

            float personalWeightGainModifier = comp?.WeightGainMultiplier is float p ? p : 1f;

            float additionalSeverity =
                personalWeightGainModifier *
                amount;

            _AddWeightIfNotAtLimit(hediff, pawn, comp, additionalSeverity);

            return additionalSeverity;
        }

        private static void _AddWeightIfNotAtLimit(Hediff hediff, Pawn pawn, FullnessAndDietStats_ThingComp comp, float additionalSeverity)
        {
            if (SeverityToKilosWithBaseWeight(hediff.Severity + additionalSeverity) > GlobalSettings.maxWeight.threshold)
                hediff.Severity = KilosToSeverityWithBaseWeight(GlobalSettings.maxWeight.threshold);
            else if ((hediff.Severity + additionalSeverity) >= (Utilities.RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn) * 21.85f))//gel 11
            {
                // don't add weight if missing perk!
                if (Prefs.DevMode || comp.perkLevels.PerkToLevels["RR_Even_Further_Beyond_Title"] >= 1)
                {
                    hediff.Severity += additionalSeverity;
                }
            }
            else
                hediff.Severity += additionalSeverity;
        }

        public static void AddHediffSeverity(HediffDef def, Pawn pawn, float amount, bool addIfHediffNull = false, bool triggerMessages = true)
        {
            Hediff h = GetHediffOfDefFrom(def, pawn);

            if (h is null)
            {
                if (addIfHediffNull)
                {
                    h = AddHediffOfDefTo(def, pawn);
                }
                else
                {
                    return;
                }
            }

            AddHediffSeverity(h, pawn, amount, triggerMessages);
        }

        //This will bypass checks normally made in AddHediffSeverity for weight. Use wisely.
        public static void SetHediffSeverity(HediffDef def, Pawn pawn, float amount)
        {
            Hediff h = GetHediffOfDefFrom(def, pawn);

            if (h is null || amount < 0)
                return;

            h.Severity = amount;
        }

        public static void ThrowValueText(Pawn pawn, float value, int precision = 1, float minimumDisplayMagnitude = 0.01f)
        {
            Vector3 position = pawn.Position.ToVector3();
            Map map = pawn.MapHeld;

            if (Mathf.Abs(value) < minimumDisplayMagnitude)
                return;

            MoteMaker.ThrowText(
                position, 
                map, 
                (value < 0 ? "" : "+") + (GlobalSettings.usePoundsWherePossible ? (SeverityToKilosWithoutBaseWeight(value) * 2.20462f).ToString($"F{(precision <= 9 && precision >= 0 ? precision : 0)}") + "Lbs" : SeverityToKilosWithoutBaseWeight(value).ToString($"F{(precision <= 9 && precision >= 0 ? precision : 0)}") + "Kgs"));
        }

        private static Message GetWeightChangedMessage(Pawn pawn, float cachedSeverity, float currentSeverity)
        {
            int cachedSeverityInt = (int)(cachedSeverity * 1000);
            int currentSeverityInt = (int)(currentSeverity * 1000);

            int baseWeight = (int)Hediffs.Hediff_Weight.ModExtension.baseWeight;

            Dictionary<int, string> weightMessagesAndSeverityGain = new Dictionary<int, string>()
            {
                {   90 - baseWeight, "WGNotification_Stage0010G".Translate(pawn.Named("PAWN"))},
                {  140 - baseWeight, "WGNotification_Stage0020G".Translate(pawn.Named("PAWN"))},
                {  190 - baseWeight, "WGNotification_Stage0030G".Translate(pawn.Named("PAWN"))},
                {  330 - baseWeight, "WGNotification_Stage0040G".Translate(pawn.Named("PAWN"))},
                {  700 - baseWeight, "WGNotification_Stage0050G".Translate(pawn.Named("PAWN"))},
                { 1450 - baseWeight, "WGNotification_Stage0060G".Translate(pawn.Named("PAWN"))}
            };

            Dictionary<int, string> weightMessagesAndSeverityLoss = new Dictionary<int, string>()
            {
                {  90 - baseWeight, "WGNotification_Stage0010L".Translate(pawn.Named("PAWN"))},
                { 140 - baseWeight, "WGNotification_Stage0020L".Translate(pawn.Named("PAWN"))},
                { 190 - baseWeight, "WGNotification_Stage0030L".Translate(pawn.Named("PAWN"))},
                { 330 - baseWeight, "WGNotification_Stage0040L".Translate(pawn.Named("PAWN"))},
                { 700 - baseWeight, "WGNotification_Stage0050L".Translate(pawn.Named("PAWN"))},
            };

            if (cachedSeverityInt == currentSeverityInt)
                return null;

            //weight loss
            if (cachedSeverityInt > currentSeverityInt)
            {
                foreach (var x in weightMessagesAndSeverityLoss)
                {
                    if (cachedSeverityInt > x.Key && currentSeverityInt < x.Key)
                    {
                        return new Message(x.Value, MessageTypeDefOf.NeutralEvent, new LookTargets(pawn));
                    }
                }
            }

            //weight gain
            if (cachedSeverityInt < currentSeverityInt)
            {
                foreach (var x in weightMessagesAndSeverityGain)
                {
                    if (cachedSeverityInt < x.Key && currentSeverityInt > x.Key)
                    {
                        return new Message(x.Value, MessageTypeDefOf.NeutralEvent, new LookTargets(pawn));
                    }
                }
            }

            return null;
        }

        public static float ProgressToNextWeightStage(Pawn pawn) 
        {
            Hediff weight = GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn);
            if (weight == null) 
            {
                return 0;
            }

            int currentStageIndex = weight.CurStageIndex;
            if (currentStageIndex < weight.def.stages.Count - 1)
            {
                float minimumSeverityForCurrentStage  = weight.CurStage.minSeverity * (GlobalSettings.varyMinWeightForBodyTypeByBodySize? RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn) : 1) ;
                float nextStageMinimumSeverity = weight.def.stages[currentStageIndex + 1].minSeverity * (GlobalSettings.varyMinWeightForBodyTypeByBodySize ? RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(pawn) : 1);
                float currentSeverity = weight.Severity;

                return (currentSeverity - minimumSeverityForCurrentStage) / (nextStageMinimumSeverity - minimumSeverityForCurrentStage);
            }
            else 
            {
                return 1;
            }
        }

        public static void AlterCapacityAccordingToSettings(List<PawnCapacityModifier> pcmList, PawnCapacityDef defToAlter, NumericFieldData<float> appropriateSetting, float personalMultiplier = 1f)
        {
            if (appropriateSetting.threshold == 1)
                return;

            int pcmIndex = pcmList.FindIndex(x => x.capacity == defToAlter);
            if (pcmIndex != -1)
            {
                pcmList[pcmIndex].offset *= appropriateSetting.threshold;
                pcmList[pcmIndex].offset *= personalMultiplier;
            }
        }

        public static PawnCapacityModifier Clone(this PawnCapacityModifier pawnCapacityModifier) 
        {
            return new PawnCapacityModifier 
            {
                capacity = pawnCapacityModifier.capacity,
                offset = pawnCapacityModifier.offset,
                setMax = pawnCapacityModifier.setMax,
                postFactor = pawnCapacityModifier.postFactor,
                setMaxCurveOverride = pawnCapacityModifier.setMaxCurveOverride,
                setMaxCurveEvaluateStat = pawnCapacityModifier.setMaxCurveEvaluateStat,
            };
        }
    }
}
