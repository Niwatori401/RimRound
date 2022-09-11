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


        public static List<Perk> basicPerks = new List<Perk>
        {
            new Perk("RR_Ample_Appetite_Title", ampleAppetiteIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Apex_Absorption_Title", apexAbsorptionIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Comfortable_Corpulence_Title", comfortableCorpulenceIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Culinary_Connisseur_Title", culinaryConnisseurIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Demonic_Devourment_Title", demonicDevourmentIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Diet_Plan_Title", dietPlanIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Digestion_Beyond_Question_Title", digestionBeyondQuestionIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Endless_Indulgence_Title", endlessIndulgenceIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_LimitBreak_Title", limitBreakIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_NoPainStillGain_Title", noPainStillGainIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),

            new Perk("RR_PracticalProblems_Title", practicalProblemsIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),

            new Perk("RR_SquareOne_Title", squareOneIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_TitanicTank_Title", titanicTankIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_WellInsulated_Title", wellInsulatedIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
        };

        public static List<Perk> advancedPerks = new List<Perk> 
        {
            new Perk("RR_Voracious_Title", voraciousIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_WeightGain4000_Title", weightGain4000Icon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_ItsComingThisWay_Title", itsComingThisWayIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Bulletproof_Blob_Title", bulletProofBlobIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_GigaGurgling_Title", gigaGurglingIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Breakneck_Buffet_Title", breakneckBuffetIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_FoldsOfHeaven_Title", foldsOfHeavenIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Beautiful_Bulk_Title", beautifulBulkIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Fat_Furnace_Title", fatFurnaceIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_RotundRegeneration_Title", rotundRegenerationIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_WeLikeToParty_Title", weLikeToPartyIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
        };

        public static List<Perk> elitePerks = new List<Perk>
        {
            new Perk("RR_GluttonyIncarnate_Title", gluttonyIncarnateIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_PaunchPower_Title", paunchPowerIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_MakesAllTheRules_Title", makesAllTheRulesIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_HeavyRevian_Title", heavyRevianIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
        };

        public static List<Perk> ultimatePerks = new List<Perk>
        {
            new Perk("RR_Even_Further_Beyond_Title", evenFurtherBeyondIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),
            new Perk("RR_Ascension_Icon_Title", ascensionIcon, (Pawn p) =>
            {
                Log.Message($"Clicked button on {p.Name.ToStringShort}!");
                return 0;
            }),

        };

        public static List<Perk> abilities = new List<Perk>
        {

        };

        public struct Perk
        {
            public Perk(String perkTranslateString, Texture2D perkIcon, Func<Pawn, int> onClick)
            {
                this.translationString = perkTranslateString;
                this.onClickEvent = onClick;
                this.perkIcon = perkIcon;
            }

            public readonly String translationString;
            public readonly Texture2D perkIcon;
            public readonly Func<Pawn, int> onClickEvent;
        }
    }
}
