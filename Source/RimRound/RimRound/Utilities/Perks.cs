using RimRound.Comps;
using RimRound.Defs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Utilities
{
    [StaticConstructorOnStartup]
    public static class Perks
    {
        public static readonly Texture2D blockedTexture =              ContentFinder<Texture2D>.Get("UI/PerkIcons/BLOCKED", true);

        public static readonly Texture2D ampleAppetiteIcon =           ContentFinder<Texture2D>.Get("UI/PerkIcons/AMPLE_APPETITE", true);
        public static readonly Texture2D apexAbsorptionIcon =          ContentFinder<Texture2D>.Get("UI/PerkIcons/APEX_ABSORPTION", true);
        public static readonly Texture2D ascensionIcon =               ContentFinder<Texture2D>.Get("UI/PerkIcons/ASCENSION", true);
        public static readonly Texture2D beautifulBulkIcon =           ContentFinder<Texture2D>.Get("UI/PerkIcons/BEAUTIFUL_BULK", true);
        public static readonly Texture2D breakneckBuffetIcon =         ContentFinder<Texture2D>.Get("UI/PerkIcons/BREAKNECK_BUFFET", true);
        public static readonly Texture2D bulletProofBlobIcon =         ContentFinder<Texture2D>.Get("UI/PerkIcons/BULLETPROOF_BLOB", true);
        public static readonly Texture2D comfortableCorpulenceIcon =   ContentFinder<Texture2D>.Get("UI/PerkIcons/COMFORTABLE_CORPULENCE", true);
        public static readonly Texture2D culinaryConnisseurIcon =      ContentFinder<Texture2D>.Get("UI/PerkIcons/CULINARY_CONNISSEUR", true);
        public static readonly Texture2D demonicDevourmentIcon =       ContentFinder<Texture2D>.Get("UI/PerkIcons/DEMONIC_DEVOURMENT", true);
        public static readonly Texture2D dietPlanIcon =                ContentFinder<Texture2D>.Get("UI/PerkIcons/DIET_PLAN", true);
        public static readonly Texture2D digestionBeyondQuestionIcon = ContentFinder<Texture2D>.Get("UI/PerkIcons/DIGESTION_BEYOND_QUESTION", true);
        public static readonly Texture2D endlessIndulgenceIcon =       ContentFinder<Texture2D>.Get("UI/PerkIcons/ENDLESS_INDULGENCE", true);
        public static readonly Texture2D evenFurtherBeyondIcon =       ContentFinder<Texture2D>.Get("UI/PerkIcons/EVEN_FURTHER_BEYOND", true);
        public static readonly Texture2D fatFurnaceIcon =              ContentFinder<Texture2D>.Get("UI/PerkIcons/FAT_FURNACE", true);
        public static readonly Texture2D foldsOfHeavenIcon =           ContentFinder<Texture2D>.Get("UI/PerkIcons/FOLDS_OF_HEAVEN", true);
        public static readonly Texture2D gigaGurglingIcon =            ContentFinder<Texture2D>.Get("UI/PerkIcons/GIGA_GURGLING", true);
        public static readonly Texture2D gluttonyIncarnateIcon =       ContentFinder<Texture2D>.Get("UI/PerkIcons/GLUTTONY_INCARNATE", true);
        public static readonly Texture2D heavyRevianIcon =             ContentFinder<Texture2D>.Get("UI/PerkIcons/HEAVY_REVIAN", true);
        public static readonly Texture2D itsComingThisWayIcon =        ContentFinder<Texture2D>.Get("UI/PerkIcons/ITS_COMING_THIS_WAY", true);
        public static readonly Texture2D limitBreakIcon =              ContentFinder<Texture2D>.Get("UI/PerkIcons/LIMIT_BREAK", true);
        public static readonly Texture2D makesAllTheRulesIcon =        ContentFinder<Texture2D>.Get("UI/PerkIcons/MAKES_ALL_OF_THE_RULES", true);
        public static readonly Texture2D noPainStillGainIcon =         ContentFinder<Texture2D>.Get("UI/PerkIcons/NO_PAIN_STILL_GAIN", true);
        public static readonly Texture2D paunchPowerIcon =             ContentFinder<Texture2D>.Get("UI/PerkIcons/PAUNCH_POWER", true);
        public static readonly Texture2D practicalProblemsIcon =       ContentFinder<Texture2D>.Get("UI/PerkIcons/PRACTICAL_PROBLEMS", true);
        public static readonly Texture2D rotundRegenerationIcon =      ContentFinder<Texture2D>.Get("UI/PerkIcons/ROTUND_REGENERATION", true);
        public static readonly Texture2D squareOneIcon =               ContentFinder<Texture2D>.Get("UI/PerkIcons/SQUARE_ONE", true);
        public static readonly Texture2D titanicTankIcon =             ContentFinder<Texture2D>.Get("UI/PerkIcons/TITANIC_TANK", true);
        public static readonly Texture2D voraciousIcon =               ContentFinder<Texture2D>.Get("UI/PerkIcons/VORACIOUS", true);
        public static readonly Texture2D weLikeToPartyIcon =           ContentFinder<Texture2D>.Get("UI/PerkIcons/WE_LIKE_TO_PARTY", true);
        public static readonly Texture2D weightGain4000Icon =          ContentFinder<Texture2D>.Get("UI/PerkIcons/WEIGHT_GAIN_4000", true);
        public static readonly Texture2D wellInsulatedIcon =           ContentFinder<Texture2D>.Get("UI/PerkIcons/WELL_INSULATED", true);

        public static readonly Texture2D atomicAnomalyIcon =           ContentFinder<Texture2D>.Get("UI/PerkIcons/ATOMIC_ANOMALY", true);
        public static readonly Texture2D cropNotchIcon =               ContentFinder<Texture2D>.Get("UI/PerkIcons/CROP_NOTCH", true);
        public static readonly Texture2D packWhaleIcon =               ContentFinder<Texture2D>.Get("UI/PerkIcons/PACK_WHALE", true);
        public static readonly Texture2D suddenExpansionIcon =         ContentFinder<Texture2D>.Get("UI/PerkIcons/SUDDEN_EXPANSION", true);
        public static readonly Texture2D thatIcon =                    ContentFinder<Texture2D>.Get("UI/PerkIcons/THAT", true);
        public static readonly Texture2D theresTheBeefIcon =           ContentFinder<Texture2D>.Get("UI/PerkIcons/THERES_THE_BEEF", true);


        public static List<Perk> basicPerks = new List<Perk>
        {
            new Perk("RR_Ample_Appetite_Title", "RR_Ample_Appetite_Desc", 2, 3, ampleAppetiteIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Apex_Absorption_Title", "RR_Apex_Absorption_Desc", 1, 10, apexAbsorptionIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Comfortable_Corpulence_Title", "RR_Comfortable_Corpulence_Desc", 1, 10, comfortableCorpulenceIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Culinary_Connisseur_Title", "RR_Culinary_Connisseur_Desc", 2, 5, culinaryConnisseurIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Demonic_Devourment_Title", "RR_Demonic_Devourment_Desc", 1, 5, demonicDevourmentIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                    p.parent.AsPawn().health.capacities.Notify_CapacityLevelsDirty();
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Diet_Plan_Title", "RR_Diet_Plan_Desc", 2, 5, dietPlanIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Digestion_Beyond_Question_Title", "RR_Digestion_Beyond_Question_Desc", 2, 10, digestionBeyondQuestionIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Endless_Indulgence_Title", "RR_Endless_Indulgence_Desc", 5, 4, endlessIndulgenceIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_LimitBreak_Title", "RR_LimitBreak_Desc", 3, 3, limitBreakIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_NoPainStillGain_Title", "RR_NoPainStillGain_Desc", 1, 7, noPainStillGainIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_PracticalProblems_Title", "RR_PracticalProblems_Desc", 10, 1, practicalProblemsIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_SquareOne_Title", "RR_SquareOne_Desc", 5, 1, squareOneIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.availablePoints -= perk.cost;

                    p.parent.AsPawn().WeightHediff().Severity = 0;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_TitanicTank_Title", "RR_TitanicTank_Desc", 2, 5, titanicTankIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_WellInsulated_Title", "RR_WellInsulated_Desc", 3, 5, wellInsulatedIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_SuddenExpansion_Title", "RR_SuddenExpansion_Desc", 1, 1, suddenExpansionIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    //p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;


                    Pawn pawn = p.parent.AsPawn();

                    Utilities.HediffUtility.AddHediffSeverity(HediffDefOf.RimRound_Weight, pawn, Utilities.HediffUtility.KilosToSeverityWithoutBaseWeight(5));
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);


                return new SuccessReport("", true);
            }),
        };

        public static List<Perk> advancedPerks = new List<Perk> 
        {
            new Perk("RR_Voracious_Title", "RR_Voracious_Desc", 4, 4, voraciousIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
               if (GetInsufficientPerkLevelSuccessReport("RR_Ample_Appetite_Title", 3, p) is SuccessReport s && !s)
                    return s;

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_WeightGain4000_Title", "RR_WeightGain4000_Desc", 3, 5, weightGain4000Icon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
               if (GetInsufficientPerkLevelSuccessReport("RR_Apex_Absorption_Title", 10, p) is SuccessReport s && !s)
                    return s;

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_ItsComingThisWay_Title", "RR_ItsComingThisWay_Desc", 15, 1, itsComingThisWayIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
               if (GetInsufficientPerkLevelSuccessReport("RR_Comfortable_Corpulence_Title", 10, p) is SuccessReport s && !s)
                    return s;

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Bulletproof_Blob_Title", "RR_Bulletproof_Blob_Desc", 3, 5, bulletProofBlobIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (!BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_090_Titanic))
                    return new SuccessReport("Must be at least Gelatinous I to purchase", false);


                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_GigaGurgling_Title", "RR_GigaGurgling_Desc", 5, 5, gigaGurglingIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
               if (GetInsufficientPerkLevelSuccessReport("RR_Digestion_Beyond_Question_Title", 10, p) is SuccessReport s && !s)
                    return s;

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Breakneck_Buffet_Title", "RR_Breakneck_Buffet_Desc", 2, 4, breakneckBuffetIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                    p.parent.AsPawn().health.capacities.Notify_CapacityLevelsDirty();
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
               if (GetInsufficientPerkLevelSuccessReport("RR_Demonic_Devourment_Title", 5, p) is SuccessReport s && !s)
                    return s;


                float lardyIISeverity = 0.350f * RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(p.parent.AsPawn());
                if (p.parent.AsPawn().WeightHediff().Severity < lardyIISeverity)
                    return new SuccessReport("Must be at least Lardy II to purchase", false);


                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_FoldsOfHeaven_Title", "RR_FoldsOfHeaven_Desc", 5, 3, foldsOfHeavenIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Beautiful_Bulk_Title", "RR_Beautiful_Bulk_Desc", 3, 3, beautifulBulkIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                float lardyIISeverity = 0.350f * RacialBodyTypeInfoUtility.GetBodyTypeWeightRequirementMultiplier(p.parent.AsPawn());
                if (p.parent.AsPawn().WeightHediff().Severity < lardyIISeverity)
                    return new SuccessReport("Must be at least Lardy II to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Fat_Furnace_Title", "RR_Fat_Furnace_Desc", 10, 2, fatFurnaceIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (!BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_090_Titanic))
                    return new SuccessReport("Must be at least Gelatinous I to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_RotundRegeneration_Title", "RR_RotundRegeneration_Desc", 10, 2, rotundRegenerationIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (!Utilities.BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_080_Gigantic))
                    return new SuccessReport("Must be at least Titanic I to purchase!", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_WeLikeToParty_Title", "RR_WeLikeToParty_Desc", 5, 1, weLikeToPartyIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (!(Utilities.BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_060_Lardy)))
                    return new SuccessReport("Must be Enormous I to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),
        };

        public static List<Perk> elitePerks = new List<Perk>
        {
            new Perk("RR_GluttonyIncarnate_Title", "RR_GluttonyIncarnate_Desc", 20, 1, gluttonyIncarnateIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (GetInsufficientPerkLevelSuccessReport("RR_Voracious_Title", 4, p) is SuccessReport s && !s)
                    return s;

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_PaunchPower_Title", "RR_PaunchPower_Desc", 20, 2, paunchPowerIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (!(Utilities.BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_350_Gelatinous)))
                    return new SuccessReport("Must be Gelatinous VII to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_PackWhale_Title", "RR_PackWhale_Desc", 6, 1, packWhaleIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (!(Utilities.BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_040_Obese)))
                    return new SuccessReport("Must be Morbidly Obese I to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),


            new Perk("RR_MakesAllTheRules_Title", "RR_MakesAllTheRules_Desc", 10, 1, makesAllTheRulesIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (!(p.parent.AsPawn().def is AlienRace.ThingDef_AlienRace race) || !(race.defName == "Ratkin_Su" || race.defName == "Ratkin"))
                    return new SuccessReport("Only Ratkin can purchase", false);

                if (!BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_500_Gelatinous))
                    return new SuccessReport("Must be Gelatinous X to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_HeavyRevian_Title", "RR_HeavyRevian_Desc", 15, 1, heavyRevianIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (!(p.parent.AsPawn().def is AlienRace.ThingDef_AlienRace race) || !(race.defName == "ReviaRaceAlien"))
                    return new SuccessReport("Only Revia can purchase", false);

                if (!BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_500_Gelatinous))
                    return new SuccessReport("Must be Gelatinous X to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),
        };

        public static List<Perk> ultimatePerks = new List<Perk>
        {
            new Perk("RR_Even_Further_Beyond_Title", "RR_Even_Further_Beyond_Desc", 25, 1, evenFurtherBeyondIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {

                // Perk requirements
                StringBuilder failureDescription = new StringBuilder();

                foreach (var bperk in Perks.basicPerks)
                {
                    if (bperk.perkName == "RR_SquareOne_Title" ||
                        bperk.perkName == "RR_Diet_Plan_Title" ||
                        bperk.perkName == "RR_SuddenExpansion_Title")
                        continue;

                    if (p.perkLevels.PerkToLevels[bperk.perkName] < bperk.numberOfLevels)
                        failureDescription.AppendLine($"Requires {bperk.perkName.Translate()} level {bperk.numberOfLevels}");
                }

                foreach (var aperk in Perks.advancedPerks)
                {
                    if (p.perkLevels.PerkToLevels[aperk.perkName] < aperk.numberOfLevels)
                        failureDescription.AppendLine($"Requires {aperk.perkName.Translate()} level {aperk.numberOfLevels}");
                }

                foreach (var eperk in Perks.elitePerks)
                {
                    if (eperk.perkName == "RR_HeavyRevian_Title" ||
                        eperk.perkName == "RR_MakesAllTheRules_Title")
                        continue;


                    if (p.perkLevels.PerkToLevels[eperk.perkName] < eperk.numberOfLevels)
                        failureDescription.AppendLine($"Requires {eperk.perkName.Translate()} level {eperk.numberOfLevels}");
                }

                if (failureDescription.Length != 0)
                {
                    if (failureDescription.Length > 10)
                        return new SuccessReport("Requires every perk purchased (Excluding racial perks / square one / diet plan)", false);
                    else
                        return new SuccessReport(failureDescription.ToString(), false);
                }


                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);


                return new SuccessReport("", true);
            }),

            new Perk("RR_Atomic_Anomaly_Title", "RR_Atomic_Anomaly_Desc", 25, 1, atomicAnomalyIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    //p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;

                    Pawn pawn = p.parent.AsPawn();
                    Utilities.HediffUtility.AddHediffSeverity(HediffDefOf.RimRound_Weight, pawn, Utilities.HediffUtility.KilosToSeverityWithoutBaseWeight(500));

                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (GetInsufficientPerkLevelSuccessReport("RR_Even_Further_Beyond_Title", 1, p) is SuccessReport s && !s)
                    return s;

                if (!BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_500_Gelatinous))
                    return new SuccessReport("Must be Gelatinous 10 to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

            new Perk("RR_Ascension_Title", "RR_Ascension_Desc", 1, 1, ascensionIcon,
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (p.perkLevels.availablePoints >= perk.cost)
                {
                    p.perkLevels.PerkToLevels[perk.perkName] += 1;
                    p.perkLevels.availablePoints -= perk.cost;
                    GameOverUtility.InitiateCountdown();
                }
            },
            (FullnessAndDietStats_ThingComp p, Perks.Perk perk) =>
            {
                if (GetInsufficientPerkLevelSuccessReport("RR_Even_Further_Beyond_Title", 1, p) is SuccessReport s && !s)
                    return s;

                if (!BodyTypeUtility.PawnIsOverWeightThreshold(p.parent.AsPawn(), Defs.BodyTypeDefOf.F_990_Gelatinous))
                    return new SuccessReport("Must be Gelatinous 20 to purchase", false);

                if (p.perkLevels.PerkToLevels[perk.perkName] >= perk.numberOfLevels)
                    return new SuccessReport("Max level", false);

                if (perk.cost > p.perkLevels.availablePoints)
                    return new SuccessReport("Not enough points", false);

                return new SuccessReport("", true);
            }),

        };

        public static List<Perk> abilities = new List<Perk>
        {

        };

        private static SuccessReport GetInsufficientPerkLevelSuccessReport(string perkName, int neededLevel, FullnessAndDietStats_ThingComp comp)
        {
            if (comp.perkLevels.PerkToLevels[perkName] < neededLevel)
                return new SuccessReport($"Requires {perkName.Translate()} level {neededLevel}", false);
            else
                return new SuccessReport("", true);
        }

        public delegate SuccessReport EligibilityValidator(FullnessAndDietStats_ThingComp p, Perks.Perk perk);
        public delegate void OnClickAction(FullnessAndDietStats_ThingComp p, Perks.Perk perk);

        public struct Perk
        {
            public Perk(String perkTranslateString, string description, int cost, int numberOfLevels, Texture2D perkIcon, OnClickAction onClick, EligibilityValidator eligibilityValidator)
            {
                this.perkName = perkTranslateString;
                this.onClickEvent = onClick;
                this.perkIcon = perkIcon;
                this.cost = cost;
                this.description = description;
                this.numberOfLevels = numberOfLevels;
                this.eligibilityValidator = eligibilityValidator;
            }


            public readonly string description;
            public readonly int cost;
            public readonly int numberOfLevels;
            public readonly String perkName;
            public readonly Texture2D perkIcon;
            public readonly OnClickAction onClickEvent;
            public readonly EligibilityValidator eligibilityValidator;
        }


        public class SuccessReport 
        {
            public static implicit operator bool(SuccessReport successReport) => successReport.successValue;

            public SuccessReport(string reason, bool successValue) 
            {
                this.reason = reason;
                this.successValue = successValue;
            }
            public string reason = "";
            bool successValue = false;
        }

    }
}
