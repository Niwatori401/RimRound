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

        public static float KilosToSeverity(float weightIncludingBaseWeight)
        {
            return (weightIncludingBaseWeight - Hediff_Weight.ModExtension.baseWeight) * Values.severityPerKilo;
        }


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

        public static void AddHediffOfDefTo(HediffDef def, Pawn pawn)
        {
            if (pawn is null)
                return;

            Hediff pawnHediff = GetHediffOfDefFrom(def, pawn);

            if (pawnHediff is null)
            {
                Hediff hediff = HediffMaker.MakeHediff(def, pawn);
                hediff.Severity = 0.01f;
                pawn.health.AddHediff(hediff);
            }

            return;
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

        public static void AddHediffSeverity(HediffDef def, Pawn pawn, float amount, bool addIfHediffNull = false)
        {
            Hediff h = GetHediffOfDefFrom(def, pawn);

            if (h is null)
            {
                if (addIfHediffNull)
                {
                    AddHediffOfDefTo(def, pawn);
                    h = GetHediffOfDefFrom(def, pawn);
                }
                else
                {
                    return;
                }
            }


            float cachedSeverity = h.Severity;



            if (h.def.defName == Defs.HediffDefOf.RimRound_Weight.defName)
            {
                if (amount > 0)
                {
                    float additionalSeverity = GlobalSettings.weightGainMultiplier.threshold * amount;

                    if (SeverityToKilosWithBaseWeight(h.Severity + additionalSeverity) > GlobalSettings.maxWeight.threshold)
                        h.Severity = KilosToSeverity(GlobalSettings.maxWeight.threshold);
                    else
                        h.Severity += additionalSeverity;
                }
                else
                {
                    float additionalSeverity = GlobalSettings.weightLossMultiplier.threshold * amount;

                    if (SeverityToKilosWithBaseWeight(h.Severity + additionalSeverity) < GlobalSettings.minWeight.threshold)
                        h.Severity = KilosToSeverity(GlobalSettings.minWeight.threshold);
                    else
                        h.Severity += additionalSeverity;
                }


                ThrowValueText(pawn, amount, 1, 0.0005f);


                if (GetWeightChangedMessage(pawn, cachedSeverity, h.Severity) is Message m)
                    Messages.Message(m);
            }
            else
            {
                h.Severity += amount;
            }
        }

        //This will bypass checks normally made in AddHediffSeverity for weight. Use wisely.
        public static void SetHediffSeverity(HediffDef def, Pawn pawn, float amount)
        {
            Hediff h = GetHediffOfDefFrom(def, pawn);

            if (h is null)
                return;

            h.Severity = amount;
        }

        public static void ThrowValueText(Pawn pawn, float value, int precision = 1, float minimumDisplayMagnitude = 0.01f)
        {
            Vector3 position = pawn.Position.ToVector3();
            Map map = pawn.MapHeld;

            if (Mathf.Abs(value) < minimumDisplayMagnitude)
                return;

            MoteMaker.ThrowText(position, map, (value < 0 ? "" : "+") + SeverityToKilosWithoutBaseWeight(value).ToString($"F{(precision <= 9 && precision >= 0 ? precision : 0)}") + "Kgs");
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

    }
}
