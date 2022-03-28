using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimRound.UI
{
    public class MainTabWindow_RRGlobalSettings : MainTabWindow
    {
        const float windowWidth = 1200f;
        const float windowHeight = 900f;

        const float spaceBetweenCheckBoxes = 26;
        const float spaceBetweenNumberFields = 26;

        const float marginBetweenNumberFields = 8;

        const float bufferForCheckmarks = 40;
        const float bufferForNumberFields = 80;
        const float numberFieldRightOffset = 120;

        const int numberOfRowsPerColumnAlienRaceSettings = 24;


        readonly int numberOfGizmoSettingCheckboxes = 5;
        readonly int numberOfExemptionSettingsCheckboxes = 6;

        static float _metaFloat;
        static string _metaStrBuffer;
        static float _metaFloat2;
        static string _metaStrBuffer2;
        static float _metaFloat3;
        static string _metaStrBuffer3;

        List<TabRecord> tabs = new List<TabRecord>();

        enum TabKind
        {
            mainSettings,
            alienBodySettings,
        };

        TabKind curTab = TabKind.mainSettings;

        public override void PreOpen()
        {
            base.PreOpen();
            tabs.Clear();
            this.tabs.Add(new TabRecord(
                "Main Settings",
                delegate () { this.curTab = TabKind.mainSettings; },
                () => this.curTab == TabKind.mainSettings));

            this.tabs.Add(new TabRecord(
                "Alien Body Settings",
                delegate () { this.curTab = TabKind.alienBodySettings; },
                () => this.curTab == TabKind.alienBodySettings));

        }


        public override Vector2 RequestedTabSize
        {
            get
            {
                return new Vector2(windowWidth, windowHeight);
            }
        }



        public override void DoWindowContents(Rect inRect)
        {
            Rect tabRect = inRect;
            tabRect.yMin += 45f;
            TabDrawer.DrawTabs<TabRecord>(tabRect, this.tabs);

            switch (curTab)
            {
                case TabKind.mainSettings:
                    DoGeneralSettingsWindow(inRect);
                    return;
                case TabKind.alienBodySettings:
                    DoAlienBodySettingsWindow(inRect);
                    return;
                default:
                    return;
            }
        }

        private struct GenderRaceCombo
        {
            public Gender gender;
            public String race;
        };

        private void DoAlienBodySettingsWindow(Rect inRect)
        {
            MakeAllDropdownsForAllRaces();

        }

        private static void MakeAllDropdownsForAllRaces()
        {
            int positionIndex = 0;

            foreach (var raceEntry in RacialBodyTypeInfoUtility.raceToProperDictDictionary)
            {
                if (DefDatabase<AlienRace.ThingDef_AlienRace>.AllDefsListForReading.Any(x => x.defName == raceEntry.Key)) 
                {
                    string raceName = raceEntry.Key;

                    MakeAllDropdownsForEachGenderForRace(raceName, ref positionIndex);
                }

            }
        }
        
        private static void MakeAllDropdownsForEachGenderForRace(string raceName, ref int positionIndex) 
        {
            Widgets.Label(new Rect { x = (float)(20 + 200 * Math.Floor(positionIndex/(float)numberOfRowsPerColumnAlienRaceSettings)), y = 50 + 27 * (positionIndex % numberOfRowsPerColumnAlienRaceSettings), width = 180, height = 25 }, raceName + "'s body settings");
            ++positionIndex;
            MakeDropdownsForGender(new GenderRaceCombo { race = raceName, gender = Gender.Male }, positionIndex++);
            MakeDropdownsForGender(new GenderRaceCombo { race = raceName, gender = Gender.Female }, positionIndex++);
            MakeDropdownsForGender(new GenderRaceCombo { race = raceName, gender = Gender.None }, positionIndex++);
        }

        private static void MakeDropdownsForGender(GenderRaceCombo genderRaceCombo, int positionIndex) 
        {
            string buttonLabel = genderRaceCombo.gender.ToString() + " bodytype";
            Rect dropdownMenuRect = new Rect()
            {
                x = (float)(20 + 200 * Math.Floor(positionIndex / (float)numberOfRowsPerColumnAlienRaceSettings)),
                y = 50 + 27 * (positionIndex % numberOfRowsPerColumnAlienRaceSettings),
                width = 180,
                height = 25,
            };

            Widgets.Dropdown<
                GenderRaceCombo,
                Dictionary<BodyTypeDef, BodyTypeInfo>>
                (
                    dropdownMenuRect, genderRaceCombo, null, new Func<GenderRaceCombo, IEnumerable<Widgets.DropdownMenuElement<Dictionary<BodyTypeDef, BodyTypeInfo>>>>(BodyTypeSetDropdownMenuGenerator), buttonLabel
                );
        }

        private static IEnumerable<Widgets.DropdownMenuElement<Dictionary<BodyTypeDef, BodyTypeInfo>>> BodyTypeSetDropdownMenuGenerator(GenderRaceCombo genderRaceCombo)
        {
            using (var enumerator = RacialBodyTypeInfoUtility.genderedSets.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var currentEntry = enumerator.Current;

                    string label = currentEntry.Key;
                    var dicitonaryPayload = currentEntry.Value;

                    yield return new Widgets.DropdownMenuElement<Dictionary<BodyTypeDef, BodyTypeInfo>>
                    {
                        option = new FloatMenuOption(label, delegate ()
                        {
                            RacialBodyTypeInfoUtility.raceToProperDictDictionary[genderRaceCombo.race][genderRaceCombo.gender] = dicitonaryPayload;

                            BodyTypeUtility.UpdateAllPawnSprites();
                        }),
                        payload = dicitonaryPayload
                    };
                }
            }
        }

        private void DoGeneralSettingsWindow(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(inRect.x, inRect.y + 45f, inRect.width, 2 * Text.LineHeight);
            DoMainSettingsTitleGroup(titleRect);

            Rect nutritionSettingsGroup = new Rect(inRect.x, titleRect.yMax, windowWidth / 3, 200);
            DoNutritionSettingsGroup(nutritionSettingsGroup);

            Rect globalMultipliersSettingsGroup = new Rect(nutritionSettingsGroup.x, nutritionSettingsGroup.yMax, nutritionSettingsGroup.width, windowHeight);
            DoGlobalMultpliersSettingsGroup(globalMultipliersSettingsGroup);

            Rect exemptionSettingsGroup = new Rect(0.33333f * inRect.width, titleRect.yMax, 0.33333f * inRect.width, 200);
            DoExemptionSettingsGroup(exemptionSettingsGroup);

            Rect generalSettingsRect = new Rect(exemptionSettingsGroup.x, exemptionSettingsGroup.yMax, exemptionSettingsGroup.width, exemptionSettingsGroup.height);
            DoGeneralSettingsGroup(generalSettingsRect);

            Rect gizmoSettingsGroupRect = new Rect(0.66666f * inRect.width, titleRect.yMax, 0.33333f * inRect.width, inRect.height - titleRect.height);
            DoGizmoSettingsGroup(gizmoSettingsGroupRect);
        }

        private void DoGizmoSettingsGroup(Rect gizmoSettingsGroup)
        {
            GUI.BeginGroup(gizmoSettingsGroup);

            Text.Font = GameFont.Medium;
            Rect gizmoSettingsTitleRect = new Rect(0, 0, gizmoSettingsGroup.width, Text.LineHeight);
            Widgets.Label(gizmoSettingsTitleRect, "RR_Mtw_GizmoSettingsTitle".Translate());


            Rect gizmoSettingsCheckBoxesRect = new Rect(0, gizmoSettingsTitleRect.yMax, gizmoSettingsGroup.width, numberOfGizmoSettingCheckboxes * spaceBetweenCheckBoxes);

            Text.Font = GameFont.Small;


            Widgets.CheckboxLabeled(new Rect(
                gizmoSettingsCheckBoxesRect.x,
                gizmoSettingsCheckBoxesRect.y,
                gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                spaceBetweenCheckBoxes),
                "RR_Mtw_GizmoSettings_PawnDietManagementGizmo".Translate(), ref GlobalSettings.showPawnDietManagementGizmo);

            Widgets.CheckboxLabeled(new Rect
            {
                x = gizmoSettingsCheckBoxesRect.x,
                y = 1 * spaceBetweenCheckBoxes + gizmoSettingsCheckBoxesRect.y,
                width = gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_GizmoSettings_SleepPostureManagementGizmo".Translate(), ref GlobalSettings.showSleepPostureManagementGizmo);
            Widgets.CheckboxLabeled(new Rect
            {
                x = gizmoSettingsCheckBoxesRect.x,
                y = 2 * spaceBetweenCheckBoxes + gizmoSettingsCheckBoxesRect.y,
                width = gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_GizmoSettings_BlanketMangementGizmo".Translate(), ref GlobalSettings.showBlanketManagementGizmo);
            Widgets.CheckboxLabeled(new Rect
            {
                x = gizmoSettingsCheckBoxesRect.x,
                y = 3 * spaceBetweenCheckBoxes + gizmoSettingsCheckBoxesRect.y,
                width = gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_GizmoSettings_ExemptionGizmo".Translate(), ref GlobalSettings.showExemptionGizmo);
            Widgets.CheckboxLabeled(new Rect
            {
                x = gizmoSettingsCheckBoxesRect.x,
                y = 4 * spaceBetweenCheckBoxes + gizmoSettingsCheckBoxesRect.y,
                width = gizmoSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_GizmoSettings_BlobIntoBedGizmo".Translate(), ref GlobalSettings.showBlobIntobedGizmo);


            GUI.EndGroup();
        }

        private void DoGeneralSettingsGroup(Rect generalSettingsRect)
        {
            GUI.BeginGroup(generalSettingsRect);

            Text.Font = GameFont.Medium;
            Rect generalSettingsTitleRect = new Rect(0, 0, generalSettingsRect.width, Text.LineHeight);
            Widgets.Label(generalSettingsTitleRect, "RR_Mtw_GeneralSettings_Title".Translate());

            Text.Font = GameFont.Small;

            Rect generalSettingsCheckboxesRect = new Rect(0, generalSettingsTitleRect.yMax, generalSettingsRect.width, 200);

            CheckboxLabeled(new Rect
            {
                x = 0,
                y = generalSettingsTitleRect.yMax,
                width = generalSettingsRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
            "RR_Mtw_GeneralSettings_BurstingEnabled".Translate(), ref GlobalSettings.burstingEnabled);

            CheckboxLabeled(new Rect
            {
                x = 0,
                y = generalSettingsTitleRect.yMax + spaceBetweenCheckBoxes * 1,
                width = generalSettingsRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
            "RR_Mtw_GeneralSettings_ShowTattoosForCustomBodies".Translate(), ref GlobalSettings.showBodyTatoosForCustomSprites, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });

            CheckboxLabeled(new Rect
            {
                x = 0,
                y = generalSettingsTitleRect.yMax + spaceBetweenCheckBoxes * 2,
                width = generalSettingsRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
            "RR_Mtw_GeneralSettings_PreferDefaultOverNaked".Translate(), ref GlobalSettings.preferDefaultOutfitOverNaked, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });

            CheckboxLabeled(new Rect
            {
                x = 0,
                y = generalSettingsTitleRect.yMax + spaceBetweenCheckBoxes * 3,
                width = generalSettingsRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
           "RR_Mtw_GeneralSettings_AlternateNorthHeadDepthForRRBodies".Translate(), ref GlobalSettings.alternateNorthHeadPositionForRRBodytypes, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });

            CheckboxLabeled(new Rect
            {
                x = 0,
                y = generalSettingsTitleRect.yMax + spaceBetweenCheckBoxes * 4,
                width = generalSettingsRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
            "RR_Mtw_GeneralSettings_UseZoomPortraitStyle".Translate(), ref GlobalSettings.useZoomPortraitStyle, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });


            GUI.EndGroup();
        }

        private void DoExemptionSettingsGroup(Rect exemptionSettingsGroup)
        {
            GUI.BeginGroup(exemptionSettingsGroup);

            Text.Font = GameFont.Medium;
            Rect exemptionSettingsTitleRect = new Rect(0, 0, exemptionSettingsGroup.width, Text.LineHeight);
            Widgets.Label(exemptionSettingsTitleRect, "RR_Mtw_BodyChangeExemptionSettingsTitle".Translate());

            Text.Font = GameFont.Small;

            Rect exemptionSettingsCheckBoxesRect = new Rect(0, exemptionSettingsTitleRect.yMax, exemptionSettingsGroup.width, numberOfExemptionSettingsCheckboxes * spaceBetweenCheckBoxes);

            CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_Male".Translate(), ref GlobalSettings.bodyChangeMale, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });
            CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 1 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_Female".Translate(), ref GlobalSettings.bodyChangeFemale, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });
            CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 2 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_HostileNPC".Translate(), ref GlobalSettings.bodyChangeHostileNPC, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });
            CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 3 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_FriendlyNPC".Translate(), ref GlobalSettings.bodyChangeFriendlyNPC, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });
            CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 4 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_Prisoner".Translate(), ref GlobalSettings.bodyChangePrisoners, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });
            CheckboxLabeled(new Rect
            {
                x = 0,
                y = exemptionSettingsCheckBoxesRect.y + 5 * spaceBetweenCheckBoxes,
                width = exemptionSettingsCheckBoxesRect.width - bufferForCheckmarks,
                height = spaceBetweenCheckBoxes
            },
                "RR_Mtw_BodyChangeExemptionSettings_Slave".Translate(), ref GlobalSettings.bodyChangeSlaves, false, null, null, false, () => { BodyTypeUtility.AssignBodyTypeCategoricalExemptions(true); });

            GUI.EndGroup();
        }

        private void DoGlobalMultpliersSettingsGroup(Rect globalMultipliersSettingsGroup)
        {
            GUI.BeginGroup(globalMultipliersSettingsGroup);

            //Category Title
            Text.Font = GameFont.Medium;
            Rect globalMultipliersSettingsTitleRect = new Rect(0, 0, globalMultipliersSettingsGroup.width, Text.LineHeight);
            Widgets.Label(globalMultipliersSettingsTitleRect, "RR_Mtw_GlobalMultipliersSettingsTitle".Translate());

            Rect globalMultipliersSettingsFieldRect = new Rect(0, globalMultipliersSettingsTitleRect.yMax, globalMultipliersSettingsTitleRect.width, 200);
            //globalMultipliersSettingsFieldRect.y += _metaFloat3;
            Text.Font = GameFont.Small;

            int numericFieldCount = 0;


            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.weightGainMultiplier, numericFieldCount++, "RR_Mtw_GlobalWeightGainMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.weightLossMultiplier, numericFieldCount++, "RR_Mtw_GlobalWeightLossMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.fullnessMultiplier, numericFieldCount++, "RR_Mtw_GlobalFullnessMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.digestionRateMultiplier, numericFieldCount++, "RR_Mtw_GlobalDigestionRateMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.stomachElasticityMultiplier, numericFieldCount++, "RR_Mtw_GlobalStomachElasticityMultiplierTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.ticksPerHungerCheck, numericFieldCount++, "RR_Mtw_TicksPerHungerCheckTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.ticksPerBodyChangeCheck, numericFieldCount++, "RR_Mtw_TicksPerBodyChangeCheckTitle");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.softLimitMuliplier, numericFieldCount++, "RR_Mtw_GlobalSoftLimitMultiplier");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.hardLimitMuliplier, numericFieldCount++, "RR_Mtw_GlobalHardLimitMultiplier");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.minWeight, numericFieldCount++, "RR_Mtw_MinWeight", GameFont.Small, () => { CheckMaxMinThresholds(); });
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.maxWeight, numericFieldCount++, "RR_Mtw_MaxWeight", GameFont.Small, () => { CheckMaxMinThresholds(); });
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.weightToBeBed, numericFieldCount++, "RR_Mtw_GlobalBlobIntoBedThreshold");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.weightToAdjustWiggleAngle, numericFieldCount++, "RR_Mtw_GlobalWeightToAdjustWiggleAngleThreshold");
            NumberFieldLabeledWithRect(globalMultipliersSettingsFieldRect, ref GlobalSettings.ticksBetweenWeightGainRequestProcess, numericFieldCount++, "RR_Mtw_GlobalTicksBetweenWeightGainRequestProcess");


            /**/
            globalMultipliersSettingsFieldRect.height = numericFieldCount * spaceBetweenNumberFields;

            GUI.EndGroup();
        }

        private void DoMainSettingsTitleGroup(Rect titleRect)
        {
            if (Prefs.DevMode)
            {
                Rect metaRect = new Rect(titleRect.x + (titleRect.width / 4), titleRect.y, titleRect.width / 5, Text.LineHeight);
                Widgets.TextFieldNumericLabeled<float>(metaRect, "Meta field 1 ", ref _metaFloat, ref _metaStrBuffer);

                Rect metaRect2 = new Rect(titleRect.x + (titleRect.width / 2), titleRect.y, titleRect.width / 5, Text.LineHeight);
                Widgets.TextFieldNumericLabeled<float>(metaRect2, "Meta field 2 ", ref _metaFloat2, ref _metaStrBuffer2);

                Rect metaRect3 = new Rect(titleRect.x + (0.75f * titleRect.width), titleRect.y, titleRect.width / 5, Text.LineHeight);
                Widgets.TextFieldNumericLabeled<float>(metaRect3, "Meta field 3 ", ref _metaFloat3, ref _metaStrBuffer3);
            }

            Widgets.Label(titleRect, "RR_Mtw_Title".Translate());
        }

        private void DoNutritionSettingsGroup(Rect nutritionSettingsGroup)
        {
            GUI.BeginGroup(nutritionSettingsGroup);

            //Category Title
            Text.Font = GameFont.Medium;
            Rect mapNutritionTitleRect = new Rect(0, 0, nutritionSettingsGroup.width, Text.LineHeight);
            Widgets.Label(mapNutritionTitleRect, "RR_Mtw_MapNutritionStatsTitle".Translate());

            float mapNutrition = Find.CurrentMap?.resourceCounter?.TotalHumanEdibleNutrition ?? 0;


            //Map Nutrition Section
            Text.Font = GameFont.Tiny;
            Rect rectMapNutContent = new Rect(0, mapNutritionTitleRect.yMax, nutritionSettingsGroup.width, 6 * Text.LineHeight);
            NutritionTable nutTable = TotalHumanEdibleNutritionOfType(Find.CurrentMap.resourceCounter);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_TotalNutrition".Translate() + ": " + mapNutrition.ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_SimpleMealNutrition".Translate() + ": " + nutTable.MealSimple.ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_FineMealNutrition".Translate() + ": " + nutTable.MealFine.ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_LavishMealNutrition".Translate() + ": " + nutTable.MealLavish.ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_UndesireableNutrition".Translate() + ": " + (nutTable.MealAwful + nutTable.RawBad + nutTable.DesperateOnly + nutTable.DesperateOnlyForHumanlikes).ToString("F1"));
            stringBuilder.AppendLine("RR_Mtw_NutritionOverview_OtherNutrition".Translate() + ": " +
                (mapNutrition -
                (nutTable.MealSimple +
                nutTable.MealFine +
                nutTable.MealLavish +
                nutTable.Undefined +
                nutTable.MealAwful +
                nutTable.RawBad +
                nutTable.DesperateOnly +
                nutTable.DesperateOnlyForHumanlikes)).ToString("F1"));

            Widgets.Label(rectMapNutContent, stringBuilder.ToString().Trim());

            Rect nutritionPerPawnLabel = new Rect(0, rectMapNutContent.yMax, nutritionSettingsGroup.width, Text.LineHeight);
            Widgets.Label(nutritionPerPawnLabel, "RR_Mtw_NutritionPerPawnLabel".Translate() + ": " + (mapNutrition / (Find.CurrentMap.mapPawns.ColonistCount + Find.CurrentMap.mapPawns.SlavesOfColonySpawned.Count)).ToString("F1"));

            GUI.EndGroup();
        }

        internal static NutritionTable TotalHumanEdibleNutritionOfType(ResourceCounter rc)
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

        static MethodInfo checkboxDrawMI = typeof(Widgets).GetMethod("CheckboxDraw", BindingFlags.NonPublic | BindingFlags.Static);
        delegate void SwitchActionCallback();
        static void CheckboxLabeled(Rect rect, string label, ref bool checkOn, bool disabled = false, Texture2D texChecked = null, Texture2D texUnchecked = null, bool placeCheckboxNearText = false, SwitchActionCallback action = null)
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
                if (GeneralUtility.IsNotNull(action))
                    action();
            }
            checkboxDrawMI.Invoke(null, new object[] { rect.x + rect.width - 24f, rect.y, checkOn, disabled, 24f, null, null });
            Text.Anchor = anchor;
        }

        static void CheckMaxMinThresholds() 
        {
            if (GlobalSettings.minWeight.threshold >= GlobalSettings.maxWeight.threshold)
            {
                GlobalSettings.minWeight.threshold = (int)GlobalSettings.minWeight.min;
                GlobalSettings.minWeight.stringBuffer = null;
                GlobalSettings.maxWeight.threshold = (int)99999;
                GlobalSettings.maxWeight.stringBuffer = null;
            }
        }

        static void NumberFieldLabeledWithRect<T>(
            Rect categoryRect, ref NumericFieldData<T> numericFieldData, int positionNumberInList, string translationLabel , GameFont font = GameFont.Small, SwitchActionCallback action = null) where T : struct
        {
            Text.Font = font;
            Rect boundingRect = new Rect(0, categoryRect.y + positionNumberInList * spaceBetweenNumberFields, categoryRect.width - numberFieldRightOffset, Text.LineHeight);
            Widgets.Label(boundingRect, translationLabel.Translate() + ": ");
            Widgets.TextFieldNumeric<T>(
                new Rect(boundingRect.xMax - bufferForNumberFields, boundingRect.y, bufferForNumberFields, Text.LineHeight),
                ref numericFieldData.threshold,
                ref numericFieldData.stringBuffer,
                numericFieldData.min,
                numericFieldData.max);

            if (action != null)
                action();
        }
    }
}
