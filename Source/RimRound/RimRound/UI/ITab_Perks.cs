using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using static RimRound.Utilities.Perks;

namespace RimRound.UI
{
    [StaticConstructorOnStartup]
    public class ITab_Perks : ITab
    {
        
        public ITab_Perks() 
        {
            this.labelKey = "RR_PerkTabLabel";
        }

        public override bool IsVisible
        {
            get
            {
                return GlobalSettings.weightToAdjustWiggleAngle.threshold == 401 && PawnToShowInfoAbout.Faction == Faction.OfPlayer && PawnToShowInfoAbout.RaceProps.Humanlike;
            }
        }

        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = null;
                if (base.SelPawn != null)
                {
                    pawn = base.SelPawn;
                }
                else
                {
                    Corpse corpse = base.SelThing as Corpse;
                    if (corpse != null)
                    {
                        pawn = corpse.InnerPawn;
                    }
                }
                if (pawn == null)
                {
                    Log.Error("CharacterDesc tab found no selected pawn to display.");
                    return null;
                }
                return pawn;
            }
        }

        protected override void UpdateSize()
        {
            base.UpdateSize();
            this.size = basePawnCardSize;
        }

        protected override void FillTab()
        {
            float heightOfTitleRect = 100;

            Rect titleRect = DrawUpperSection(new Rect(0, 0, basePawnCardSize.x, heightOfTitleRect).ContractedBy(10f));
            Rect descriptionRect = new Rect(titleRect.x, titleRect.yMax, basePawnCardSize.x, basePawnCardSize.y - heightOfTitleRect);


            Rect containingRectOfPerkIcons = CalculateSizeOfPerksContainingRect(
                descriptionRect,
                Perks.basicPerks.Count + Perks.advancedPerks.Count + Perks.elitePerks.Count + Perks.ultimatePerks.Count + Perks.abilities.Count + 30, 
                new Vector2(100, 150));

            Widgets.BeginScrollView(descriptionRect, ref scrollbarPos, containingRectOfPerkIcons, true);
            DrawPerks(containingRectOfPerkIcons);
            Widgets.EndScrollView();
            
        }


        private Rect DrawUpperSection(Rect titleRect) 
        {
            titleRect.y -= 7;

            Vector2 barOffset = new Vector2(0, 35);
            Vector2 barSize = new Vector2(230, 20);
            Rect fillableBarRect = new Rect(titleRect.x + barOffset.x, titleRect.y + barOffset.y, barSize.x, barSize.y);

            var fdsComp = PawnToShowInfoAbout.TryGetComp<FullnessAndDietStats_ThingComp>();
            if (fdsComp is null)
                return titleRect;

            float heightOffsets = 23;

            Text.Font = GameFont.Small;
            Widgets.FillableBar(fillableBarRect, (fdsComp.ConsumedNutrition % GlobalSettings.nutritionPerPerkLevel.threshold) / GlobalSettings.nutritionPerPerkLevel.threshold);
            Rect barLabelRect = new Rect(fillableBarRect);
            barLabelRect.y += -heightOffsets;
            Widgets.Label(barLabelRect, $"Points Available: {fdsComp.perkLevels.availablePoints}");
            Widgets.DrawLineHorizontal(0f, titleRect.yMax - 8, titleRect.width);

            Rect currentLevelRect = new Rect(barLabelRect);
            currentLevelRect.x += 250;
            Widgets.Label(currentLevelRect, $"Current Level: {fdsComp.perkLevels.currentLevel}");

            Rect currentNutritionRect = new Rect(currentLevelRect);
            currentNutritionRect.y += heightOffsets;
            Widgets.Label(currentNutritionRect, $"Nutrition Consumed: {fdsComp.ConsumedNutrition:f1}");


            return titleRect;
        }


        private void DrawPerks(Rect drawRect) 
        {
            float verticalSpaceBetweenIcons = 100;
            int perkIndex = 0;
            int titleOffset = -72 + (int)Values.debugPos;

            perkIndex += 3;

            Text.Font = GameFont.Medium;
            Rect basictitleRect = new Rect(drawRect.x, drawRect.y + titleOffset + Mathf.FloorToInt(perkIndex/3) * verticalSpaceBetweenIcons, drawRect.width, Text.CalcSize("RR_Basic_Perks_Title".Translate().Truncate(drawRect.width)).y);
            Text.Font = GameFont.Small;


            FullnessAndDietStats_ThingComp fdsComp = PawnToShowInfoAbout.TryGetComp<FullnessAndDietStats_ThingComp>();

            if (fdsComp is null) 
            {
                Log.Error("FullnessAndDietStats_ThingComp was null in DrawPerks()");
                return;
            }


            Text.Font = GameFont.Medium;
            Widgets.Label(basictitleRect, "RR_Basic_Perks_Title".Translate().Truncate(basictitleRect.width)); 
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0f, basictitleRect.yMax + 2, basictitleRect.width);

            foreach (Perk perk in Perks.basicPerks)
                if (perk.showPerkInMenu is null || perk.showPerkInMenu(fdsComp) || GlobalSettings.showAllPerks)
                    DrawSinglePerk(perk, perkIndex++, drawRect, fdsComp);

            //Adds a line of space
            int basicPerkRemainders = Perks.basicPerks.Count % 3;
            perkIndex += (6 - basicPerkRemainders);

            Text.Font = GameFont.Medium;
            Rect advancedTitleRect = new Rect(drawRect.x, drawRect.y + titleOffset + Mathf.FloorToInt(perkIndex/3) * verticalSpaceBetweenIcons, drawRect.width, Text.CalcSize("RR_Basic_Perks_Title".Translate().Truncate(drawRect.width)).y);
            Text.Font = GameFont.Small;

            Text.Font = GameFont.Medium;
            Widgets.Label(advancedTitleRect, "RR_Advanced_Perks_Title".Translate());
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0f, advancedTitleRect.yMax + 2, advancedTitleRect.width);
            
            foreach (Perk perk in Perks.advancedPerks)
                if (perk.showPerkInMenu is null || perk.showPerkInMenu(fdsComp) || GlobalSettings.showAllPerks)
                    DrawSinglePerk(perk, perkIndex++, drawRect, fdsComp);

            int advancedPerkRemainders = Perks.advancedPerks.Count % 3;
            perkIndex += (6 - advancedPerkRemainders);

            Text.Font = GameFont.Medium;
            Rect eliteTitleRect = new Rect(drawRect.x, drawRect.y + titleOffset + Mathf.FloorToInt(perkIndex/3) * verticalSpaceBetweenIcons, drawRect.width, Text.CalcSize("RR_Basic_Perks_Title".Translate().Truncate(drawRect.width)).y);
            Text.Font = GameFont.Small;

            Text.Font = GameFont.Medium;
            Widgets.Label(eliteTitleRect, "RR_Elite_Perks_Title".Translate());
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0f, eliteTitleRect.yMax + 2, eliteTitleRect.width);

            foreach (Perk perk in Perks.elitePerks)
                if (perk.showPerkInMenu is null || perk.showPerkInMenu(fdsComp) || GlobalSettings.showAllPerks)
                    DrawSinglePerk(perk, perkIndex++, drawRect, fdsComp);

            int elitePerkRemainders = Perks.elitePerks.Count % 3;
            perkIndex += (6 - elitePerkRemainders);


            Text.Font = GameFont.Medium;
            Rect ultraTitleRect = new Rect(drawRect.x, drawRect.y + titleOffset + Mathf.FloorToInt(perkIndex/3) * verticalSpaceBetweenIcons, drawRect.width, Text.CalcSize("RR_Basic_Perks_Title".Translate().Truncate(drawRect.width)).y);
            Text.Font = GameFont.Small;

            Text.Font = GameFont.Medium;
            Widgets.Label(ultraTitleRect, "RR_Ultimate_Perks_Title".Translate());
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0f, ultraTitleRect.yMax + 2, ultraTitleRect.width);

            foreach (Perk perk in Perks.ultimatePerks)
                if (perk.showPerkInMenu is null || perk.showPerkInMenu(fdsComp) || GlobalSettings.showAllPerks)
                    DrawSinglePerk(perk, perkIndex++, drawRect, fdsComp);

            int ultimatePerkRemainders = Perks.ultimatePerks.Count % 3;
            perkIndex += (6 - ultimatePerkRemainders);

            Text.Font = GameFont.Medium;
            Rect abilitiesTitleRect = new Rect(drawRect.x, drawRect.y + titleOffset + Mathf.FloorToInt(perkIndex/3) * verticalSpaceBetweenIcons, drawRect.width, Text.CalcSize("RR_Basic_Perks_Title".Translate().Truncate(drawRect.width)).y);
            Text.Font = GameFont.Small;

            Text.Font = GameFont.Medium;
            //Widgets.Label(abilitiesTitleRect, "RR_Abilities_Title".Translate());
            //Text.Font = GameFont.Small;
            //Widgets.DrawLineHorizontal(0f, abilitiesTitleRect.yMax + 2, abilitiesTitleRect.width);
        }

        private void DrawSinglePerk(Perk perk, int entryIndex, Rect drawInRect, FullnessAndDietStats_ThingComp comp) 
        {
            float rightwardOffset = 32;
            float totalWidthForEntry = 145;
            float totalHeightForEntry = 100;
            float textOffset = 20f;
            float imageWidth = 75;
            float imageheight = 75;

            int possibleEntriesPerRow = Mathf.FloorToInt(drawInRect.width / totalWidthForEntry);

            string textTitle = perk.perkName.Translate().Truncate(totalWidthForEntry);
            float textLength = Text.CalcSize(textTitle).x;


            Rect drawRect = new Rect(
                drawInRect.xMin + entryIndex % possibleEntriesPerRow * totalWidthForEntry + rightwardOffset, 
                drawInRect.yMin + (Mathf.FloorToInt(entryIndex / possibleEntriesPerRow) * totalHeightForEntry), 
                imageWidth, 
                imageheight);

            Rect labelRect = new Rect(drawRect.x - (textLength - imageWidth) / 2, drawRect.y - textOffset, totalWidthForEntry, Text.CalcSize(textTitle).y);
            Widgets.Label(labelRect , textTitle);

            Perks.SuccessReport shouldBeAvailable = perk.eligibilityValidator(comp, perk);
            if (!shouldBeAvailable)
            {
                //To make it click first
                if (Widgets.ButtonImage(drawRect, blockedTexture)) 
                {
                    SoundDefOf.Designate_Failed.PlayOneShotOnCamera(null);
                    //Put anything that needs to happen on click of prohibited button here
                }
            }

            TooltipHandler.TipRegion(drawRect, () => 
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (!shouldBeAvailable)
                {
                    stringBuilder.AppendLine(shouldBeAvailable.reason.ToUpper());
                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendLine(perk.description.Translate(PawnToShowInfoAbout));
                stringBuilder.AppendLine();
                stringBuilder.AppendLine($"Point cost per level: {perk.cost}");
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Current Level: " + comp.perkLevels.PerkToLevels[perk.perkName] + "/" + perk.numberOfLevels);

                return  stringBuilder.ToString();
            }, 
            426911630 + entryIndex);

            if (Widgets.ButtonImage(drawRect, perk.perkIcon))
            {
                SoundDefOf.Click.PlayOneShotOnCamera(null);
                perk.onClickEvent(comp, perk);
            }

            if (!shouldBeAvailable)
            {
                //Put no behavior here
                Widgets.ButtonImage(drawRect, blockedTexture);
            }
        }

        private Rect CalculateSizeOfPerksContainingRect(Rect containingRect, int numberOfPerkIcons, Vector2 sizeOfEachIconArea) 
        {
            int numberOfIconsPerRow = (int)(containingRect.width / sizeOfEachIconArea.x);
            
            int numberOfRows = (int)(Mathf.CeilToInt(numberOfPerkIcons / numberOfIconsPerRow));

            Rect resultRect = new Rect(containingRect);
            resultRect.height = Mathf.Max(resultRect.height, numberOfRows * sizeOfEachIconArea.y);

            return resultRect;
        }


        private Vector2 scrollbarPos = new Vector2();

        public static Vector2 basePawnCardSize = new Vector2(480f, 455f);


    }




}
