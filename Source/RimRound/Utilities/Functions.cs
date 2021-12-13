using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;


using RimWorld;
using UnityEngine;
using HarmonyLib;
using RimRound.Enums;
using AlienRace;
using RimRound.Comps;
using Verse.AI;
using System.Reflection;
using RimRound.Hediffs;
using Verse.Sound;

namespace RimRound.Utilities
{
    public static class Functions
    {
        #region conversions

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

        #endregion

        #region Render Functions

        public static void UpdatePawnSprite(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false, bool forceUpdate = false, bool BodyCheck = true) 
        {

            if (BodyCheck && GetBodyTypeBasedOnWeightSeverity(pawn, personallyExempt, categoricallyExempt) is BodyTypeDef b && b != pawn.story.bodyType)
            {
                pawn.story.bodyType = b;
                RedrawPawn(pawn);
                return;
            } 
            else if (forceUpdate)
            {
                RedrawPawn(pawn);
                return;
            }


        }

        public static void RedrawPawn(Pawn pawn)
        {
            PortraitsCache.SetDirty(pawn);
            GlobalTextureAtlasManager.TryMarkPawnFrameSetDirty(pawn);
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
        }

        internal static void AssignBodyTypeCategoricalExemptions(bool updatePawnSprite = false)
        {
            foreach (Map m in Find.Maps)
            {
                foreach (Pawn p in m.mapPawns.AllPawns)
                {
                    if (p is null || (!p?.RaceProps.Humanlike ?? false))
                        continue;

                    PawnBodyType_ThingComp comp = p.TryGetComp<PawnBodyType_ThingComp>();

                    if (comp is null)
                        continue;

                    if ((GlobalSettings.bodyChangeFemale is false && p.gender is Gender.Female) ||
                        (GlobalSettings.bodyChangeMale is false && p.gender is Gender.Male) ||
                        (GlobalSettings.bodyChangeFriendlyNPC is false && p.Faction != Faction.OfPlayer && p.Faction.AllyOrNeutralTo(Faction.OfPlayer)) ||
                        (GlobalSettings.bodyChangeHostileNPC is false && p.Faction.HostileTo(Faction.OfPlayer) && !p.IsPrisoner) ||
                        (GlobalSettings.bodyChangePrisoners is false && p.IsPrisoner) ||
                        (GlobalSettings.bodyChangeSlaves is false && p.IsSlaveOfColony)
                        )
                    {
                        comp.CategoricallyExempt = true;
                    }
                    else
                    {
                        comp.CategoricallyExempt = false;
                    }

                    if (updatePawnSprite)
                        UpdatePawnSprite(p, comp.PersonallyExempt, comp.CategoricallyExempt, true, true);
                }
            }

            return;
        }

        internal static void UpdatePawnDrawSize(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false)
        {
          
            if (pawn.def is ThingDef_AlienRace alienProps)
            {
                GraphicPaths gp = alienProps.alienRace.graphicPaths.GetCurrentGraphicPath(pawn.ageTracker.CurLifeStage);
                //alienProps.alienRace.generalSettings.alienPartGenerator.bodyAddons.First().offsets.north.bodyTypes.First().

                float drawSize = Values.bodyTypeDrawSizes[pawn.story.bodyType];
                gp.customDrawSize = new UnityEngine.Vector2(drawSize, drawSize);
            }
        }

        #endregion

        #region Hediff Functions

        public static void AddHediffOfDefTo(HediffDef def, Pawn pawn) 
        {
            if (pawn is null)
                return;

            Hediff pawnHediff = Functions.GetHediffOfDefFrom(def, pawn);

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
            Hediff h =  pawn?.health?.hediffSet?.hediffs?.Find(x => x.def == def) ?? null;

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

            return pawn?.health?.hediffSet?.hediffs?.Find(x => x.def == def) ?? null;
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



            if (h.def == Defs.HediffDefOf.RimRound_Weight)
            {
                if (amount > 0)
                {
                    float additionalSeverity = GlobalSettings.weightGainMultiplier.threshold * amount;
                    if (Functions.SeverityToKilosWithBaseWeight(h.Severity + additionalSeverity) > GlobalSettings.maxWeight.threshold)
                        h.Severity = Functions.KilosToSeverity(GlobalSettings.maxWeight.threshold);
                    else
                        h.Severity += additionalSeverity;
                }
                else
                {
                    h.Severity += GlobalSettings.weightLossMultiplier.threshold * amount;
                }




                ThrowValueText(pawn, amount, 1, 0.5f);


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

        static Dictionary<BodyTypeDef, float> GetProperSeverityDictionary(Pawn pawn)
        {
            if (pawn.def is ThingDef_AlienRace race)
            {
                foreach (var x in Values.raceToBodytypeDictionary) 
                {
                    if (pawn.def.defName == x.Key) 
                    {
                        return x.Value;
                    }
                }
            }
            else 
            {
                return Values.raceToBodytypeDictionary.First().Value;
            }
            return Values.raceToBodytypeDictionary.First().Value;

        }

        public static BodyTypeDef GetBodyTypeBasedOnWeightSeverity(Pawn pawn, bool personallyExempt = false, bool categoricallyExempt = false) 
        {
            Dictionary<BodyTypeDef, float> bTD = GetProperSeverityDictionary(pawn);

            if (personallyExempt || categoricallyExempt)
            {
                return pawn.TryGetComp<FullnessAndDietStats_ThingComp>().defaultBodyType ??
                    RimWorld.BodyTypeDefOf.Thin;
            }


            float weightSeverity = Functions.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn)?.Severity ?? 0;


            //Edge cases
            if (weightSeverity == 0)
                return bTD.First().Key;

            if (weightSeverity >= bTD.Last().Value)
                return bTD.Last().Key;


            foreach (var x in bTD) 
            {
                if (weightSeverity <= x.Value)
                    return x.Key;
            }

            return bTD.First().Key;
        }

        #endregion

        #region Food Functions

        public static bool FoodValidator(Thing t, Pawn getter, Pawn eater, bool allowForbidden)
        {
            if (t is Building_FoodFaucet building_FoodFaucet)
            {
                //!eater.WillEat(ThingDefOf.MealNutrientPaste, getter, true) ||
                //Defs.ThingDefOf.RR_FeedingTubeFluid.ingestible.preferability > FoodPreferability.MealLavish ||
                //Defs.ThingDefOf.RR_FeedingTubeFluid.ingestible.preferability < FoodPreferability.MealAwful ||
                if (
                getter != eater ||
                !(getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)) ||
                (t.Faction != getter.Faction && t.Faction != getter.HostFaction) ||
                (!allowForbidden && t.IsForbidden(getter)) ||
                (!building_FoodFaucet.foodNetTrader.CanBeOn) ||
                !t.InteractionCell.Standable(t.Map) ||
                !getter.Map.reachability.CanReachNonLocal(
                    getter.Position,
                    new TargetInfo(t.InteractionCell, t.Map, false),
                    PathEndMode.OnCell,
                    TraverseParms.For(getter, Danger.Some, TraverseMode.ByPawn, false, false, false)) ||
                ((Building_FoodFaucet)t).foodNetTrader.FoodNet.Stored <= ((Building_FoodFaucet)t).litersPerDispense)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        #endregion

        #region Extension Methods

        public static float? Weight(this Pawn pawn)
        {
            if (Functions.GetHediffOfDefFrom(Defs.HediffDefOf.RimRound_Weight, pawn) is Hediff weight)
            {
                return (float)Functions.SeverityToKilosWithoutBaseWeight(weight.Severity) + Hediffs.Hediff_Weight.ModExtension.baseWeight;
            }
            else
            {
                return null;
            }
        }

        internal static NutritionTable TotalHumanEdibleNutritionOfType(this ResourceCounter rc) 
        {
            NutritionTable nutritionTable = new NutritionTable();

            float num;

            Dictionary<ThingDef, int> dict = (Dictionary<ThingDef, int>)Traverse.Create(rc).Field("countedAmounts").GetValue();

            foreach (KeyValuePair<ThingDef, int> keyValuePair in dict)
            {
                if (keyValuePair.Key.IsNutritionGivingIngestible && keyValuePair.Key.ingestible.HumanEdible)
                {
                    num = keyValuePair.Key.GetStatValueAbstract(StatDefOf.Nutrition, null) * (float)keyValuePair.Value;

                    switch (keyValuePair.Key.ingestible.preferability) 
                    {
                        case (FoodPreferability.Undefined):
                            nutritionTable.Undefined += num;
                            break;
                        case FoodPreferability.NeverForNutrition:
                            nutritionTable.NeverForNutrition += num;
                            break;
                        case FoodPreferability.DesperateOnly:
                            nutritionTable.DesperateOnly += num;
                            break;
                        case FoodPreferability.DesperateOnlyForHumanlikes:
                            nutritionTable.DesperateOnlyForHumanlikes += num;
                            break;
                        case FoodPreferability.RawBad:
                            nutritionTable.RawBad += num;
                            break;
                        case FoodPreferability.RawTasty:
                            nutritionTable.RawTasty += num;
                            break;
                        case FoodPreferability.MealAwful:
                            nutritionTable.MealAwful += num;
                            break;
                        case FoodPreferability.MealSimple:
                            nutritionTable.MealSimple += num;
                            break;
                        case FoodPreferability.MealFine:
                            nutritionTable.MealFine += num;
                            break;
                        case FoodPreferability.MealLavish:
                            nutritionTable.MealLavish += num;
                            break;
                        default:
                            Log.Warning($"{keyValuePair.Key.label} had unexpected food preferability!");
                            nutritionTable.Undefined += num;
                            break;
                    }
                }
            }

            return nutritionTable;
        }

        public static Pawn AsPawn(this ThingWithComps t) 
        {
            return t as Pawn;
        }

        public static Pawn AsPawn(this object t)
        {
            return t as Pawn;
        }

        public static bool IsNotNull(object o) 
        {
            return !(o is null);
        }

        #endregion

        #region Misc Functions

        public static void DebugLogMessage(string message) 
        {
            if (Prefs.DevMode)
                Log.Message(message);
        }


        static MethodInfo checkboxDrawMI = typeof(Widgets).GetMethod("CheckboxDraw", BindingFlags.NonPublic | BindingFlags.Static);
        public delegate void SwitchActionCallback();
        public static void CheckboxLabeled(Rect rect, string label, ref bool checkOn, bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false, SwitchActionCallback action = null) 
        {
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            if (placeCheckboxNearText)
            {
                rect.width = Mathf.Min(rect.width, Text.CalcSize(label).x + 24f + 10f);
            }
            Widgets.Label(rect, label);
            if (!disabled && Widgets.ButtonInvisible(rect, true))
            {
                checkOn = !checkOn;
                if (checkOn)
                {
                    SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
                }
                else
                {
                    SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
                }
                if (IsNotNull(action))
                    action();
            }
            checkboxDrawMI.Invoke(null, new object[] { rect.x + rect.width - 24f, rect.y, checkOn, disabled, 24f, null, null });
            Text.Anchor = anchor;
        }


        public static void CheckCanBeBlobBed() 
        {
        
        }

        public static bool IsHashIntervalTick(int interval) 
        {
            return Find.TickManager.TicksGame % interval == 0;
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


        //Places a mote at Pawn with the value specified below. 
        public static void ThrowValueText(Pawn pawn, float value, int precision = 1, float minimumDisplayValue = 0.1f)
        {
            Vector3 position = pawn.Position.ToVector3();
            Map map = pawn.MapHeld;

            if (value < minimumDisplayValue)
                return;

            MoteMaker.ThrowText(position, map, RoundedDisplayValueAsString(value, precision));
        }


        public static string RoundedDisplayValueAsString(float floatToRound, int precision)
        {
            List<int> x = RoundDisplayValue(floatToRound, precision);
            string y = x[1] == -1 ? $"{x[0]}" : $"{x[0]}.{x[1]}";
            return y;
        }

        //Takes in a float, returns a list of two ints of its rounded parts. E.g. RoundDisplayValue(12.34567, 2) yields {12, 34}
        //For precision, 0 is ones place, 1 is tenths, -1 is tens place etc)
        private static List<int> RoundDisplayValue(float floatToRound, int precision)
        {
            int wholePart = 0;
            int fracPart = 0;

            if (precision < 0)
            {
                floatToRound *= (float)Math.Pow(10, precision);
                wholePart = (int)floatToRound / (int)Math.Pow(10, precision);
                fracPart = -1;
            }
            else if (precision == 0)
            {
                wholePart = (int)Math.Truncate(floatToRound);
                fracPart = -1;
            }
            else 
            {
                wholePart = (int)Math.Truncate(floatToRound);
                fracPart = (int)Math.Truncate(floatToRound * Math.Pow(10, precision)) - wholePart * (int)Math.Pow(10, precision);
            }
            return new List<int> { wholePart, fracPart };
        }


        public static bool HasAnyWeightOpinion(Pawn pawn) 
        {
            foreach (var x in Values.traitAndCommonalityPair) 
            {
                if (pawn.story.traits.HasTrait(x.Key)) 
                {
                    return true;
                }
            }
            return false;
        }

        public static TraitDef GetWeightedRandWeightOpinionTrait(Dictionary<TraitDef, float> traitCommonalityPairs) 
        {
            float sumOfKeys = 0;

            foreach (var x in traitCommonalityPairs) 
            {
                sumOfKeys += x.Value;
            }

            float randomPortion = (float)Values.random.NextDouble() * sumOfKeys;

            foreach (var x in traitCommonalityPairs) 
            {
                randomPortion -= x.Value;
                if (randomPortion <= 0) 
                {
                    return x.Key;        
                }
            }

            return null;
        }

        public static TraitDef AssignInitialWeightOpinionTraits(Pawn p) 
        {
            if (HasAnyWeightOpinion(p))
                return null;

            TraitDef t = Functions.GetWeightedRandWeightOpinionTrait(Values.traitAndCommonalityPair);

            p.story.traits.GainTrait(new Trait(t, 0, true));

            return t;
        }

        public static WeightOpinion TraitDefToWeightOpinion(TraitDef t) 
        {
            if (t is null)
                return WeightOpinion.None;

            if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Hate_Trait)
                return WeightOpinion.Hate;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Dislike_Trait)
                return WeightOpinion.Dislike;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_NeutralMinus_Trait)
                return WeightOpinion.NeutralMinus;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Neutral_Trait)
                return WeightOpinion.Neutral;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_NeutralPlus_Trait)
                return WeightOpinion.NeutralPlus;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Like_Trait)
                return WeightOpinion.Like;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Love_Trait)
                return WeightOpinion.Love;
            else if (t == RimRound.Defs.TraitDefOf.RR_WeightOpinion_Fanatical_Trait)
                return WeightOpinion.Fanatical;
            else
                return WeightOpinion.None;
        }

        public static TraitDef GetTraitByWeightOpinion(WeightOpinion w) 
        {
            return Values.weightOpinionToTraitDef[w];
        }

        public static void RemoveWeightOpinionTraits(Pawn p) 
        {
            List<Trait> shitList = new List<Trait>();
            foreach (var x in p.story.traits.allTraits) 
            {
                if (x.def.exclusionTags.Contains("RR_Trait_WeightOpinion"))
                    shitList.Add(x);
            }

            foreach (var y in shitList)
                p.story.traits.RemoveTrait(y);
        }

        public static bool HasCustomBody(Pawn p) 
        {
            if (p?.story?.bodyType is null)
                return false;

            foreach (BodyTypeDef bodyTypeDef in Values.rimRoundBodyTypeDefs) 
            {
                if (p.story.bodyType.defName == bodyTypeDef.defName) 
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
