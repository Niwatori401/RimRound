using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
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
            Rect titleRect = new Rect(0, 0, basePawnCardSize.x, basePawnCardSize.y).ContractedBy(10f);
            Rect descriptionRect = new Rect(titleRect);
            descriptionRect.yMin += 35f;

            Text.Font = GameFont.Medium;
            Widgets.Label(titleRect, "RR_PerkTab_Title".Translate());
            Text.Font = GameFont.Small;

            Widgets.DrawLineHorizontal(0f, titleRect.yMax + 2, titleRect.width);

            Rect containingRectOfPerkIcons = CalculateSizeOfPerksContainingRect(
                descriptionRect,
                Perks.basicPerks.Count + Perks.advancedPerks.Count + Perks.elitePerks.Count + Perks.ultimatePerks.Count + Perks.abilities.Count + 30, 
                new Vector2(100, 150));

            descriptionRect.y -= Values.debugPos;

            Widgets.BeginScrollView(descriptionRect, ref scrollbarPos, containingRectOfPerkIcons, true);

            //GUI.BeginGroup(containingRectOfPerkIcons);

            DrawPerks(containingRectOfPerkIcons);

            //GUI.EndGroup();



            Widgets.EndScrollView();
            
        }


        private void DrawPerks(Rect drawRect) 
        {
            float verticalSpaceBetweenIcons = 100;
            int perkIndex = 0;
            int titleOffset = -72;

            Text.Font = GameFont.Medium;
            Rect basictitleRect = new Rect(drawRect.x, drawRect.y + titleOffset + Mathf.FloorToInt(perkIndex/3) * verticalSpaceBetweenIcons, drawRect.width, Text.CalcSize("RR_Basic_Perks_Title".Translate().Truncate(drawRect.width)).y);
            Text.Font = GameFont.Small;

            

            

            Text.Font = GameFont.Medium;
            Widgets.Label(basictitleRect, "RR_Basic_Perks_Title".Translate().Truncate(basictitleRect.width)); 
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0f, basictitleRect.yMax + 2, basictitleRect.width);

            foreach (Perk perk in Perks.basicPerks)
                DrawSinglePerk(perk, perkIndex++, drawRect);

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
                DrawSinglePerk(perk, perkIndex++, drawRect);

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
                DrawSinglePerk(perk, perkIndex++, drawRect);

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
                DrawSinglePerk(perk, perkIndex++, drawRect);

            int ultimatePerkRemainders = Perks.ultimatePerks.Count % 3;
            perkIndex += (6 - ultimatePerkRemainders);

            Text.Font = GameFont.Medium;
            Rect abilitiesTitleRect = new Rect(drawRect.x, drawRect.y + titleOffset + Mathf.FloorToInt(perkIndex/3) * verticalSpaceBetweenIcons, drawRect.width, Text.CalcSize("RR_Basic_Perks_Title".Translate().Truncate(drawRect.width)).y);
            Text.Font = GameFont.Small;

            Text.Font = GameFont.Medium;
            Widgets.Label(abilitiesTitleRect, "RR_Abilities_Title".Translate());
            Text.Font = GameFont.Small;
            Widgets.DrawLineHorizontal(0f, abilitiesTitleRect.yMax + 2, abilitiesTitleRect.width);
        }

        private void DrawSinglePerk(Perk perk, int entryIndex, Rect drawInRect) 
        {
            float rightwardOffset = 28;
            float totalWidthForEntry = 145;
            float totalHeightForEntry = 100;
            float textOffset = 20f;
            float imageWidth = 75;
            float imageheight = 75;

            int possibleEntriesPerRow = Mathf.FloorToInt(drawInRect.width / totalWidthForEntry);

            string textTitle = perk.translationString.Translate().Truncate(totalWidthForEntry);

            Rect drawRect = new Rect(
                drawInRect.xMin + entryIndex % possibleEntriesPerRow * totalWidthForEntry + rightwardOffset, 
                drawInRect.yMin + (Mathf.FloorToInt(entryIndex / possibleEntriesPerRow) * totalHeightForEntry), 
                imageWidth, 
                imageheight);

            Rect labelRect = new Rect(drawRect.x, drawRect.y - textOffset, totalWidthForEntry, Text.CalcSize(textTitle).y);
            Widgets.Label(labelRect , textTitle);

            if (Widgets.ButtonImage(drawRect, perk.perkIcon))
            {
                perk.onClickEvent(PawnToShowInfoAbout);
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
