using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using UnityEngine;
using RimRound.Comps;


namespace RimRound.UI
{

    [StaticConstructorOnStartup]
    public static class WeightGizmo_ModeButton
    {
        public static Texture2D GetIcon(this DietMode mode) 
        {
            switch (mode) 
            {
                case DietMode.Nutrition:
                    return NutritionModeIcon;
                case DietMode.Hybrid:
                    return HybridModeIcon;
                case DietMode.Fullness:
                    return FullnessModeIcon;
                case DietMode.Disabled:
                    return DisabledModeIcon;
                default:
                    return BaseContent.BadTex;
            }
        }

        public static DietMode GetNextResponse(Pawn pawn)
        {
            switch (pawn.TryGetComp<FullnessAndDietStats_ThingComp>().DietMode)
            {
                case DietMode.Nutrition:
                    return DietMode.Hybrid;
                case DietMode.Hybrid:
                    return DietMode.Fullness;
                case DietMode.Fullness:
                    return DietMode.Disabled;
                case DietMode.Disabled:
                    return DietMode.Nutrition;
                default:
                    return DietMode.Nutrition;
            }
        }

        public static string GetLabel(this DietMode response) 
        {
            return ("DietMode_" + response).Translate();
        }


        public static void DrawResponseButton(Rect rect, Pawn pawn, bool paintable)
        {
            Widgets.Dropdown<Pawn, DietMode>(
                rect, 
                pawn, 
                WeightGizmo_ModeButton.IconColor, 
                new Func<Pawn, DietMode>(WeightGizmo_ModeButton.DrawResponseButton_GetResponse), 
                new Func<Pawn, IEnumerable<Widgets.DropdownMenuElement<DietMode>>>(WeightGizmo_ModeButton.DrawResponseButton_GenerateMenu), 
                null,
                pawn.TryGetComp<FullnessAndDietStats_ThingComp>().DietMode.GetIcon(), 
                null, 
                null, 
                //CHANGE
                delegate ()
                    {
                        PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.HostilityResponse, KnowledgeAmount.SpecificInteraction);
                    }, 
                paintable);

            //Needs changed!
            //UIHighlighter.HighlightOpportunity(rect, "HostilityResponse");
            if (Mouse.IsOver(rect))
            {
                TooltipHandler.TipRegion(rect, "DietMode_Tip".Translate() + "\n\n" + "DietMode_CurrentMode".Translate() + ": " + pawn.TryGetComp<FullnessAndDietStats_ThingComp>().DietMode.GetLabel());
            }
        }

        private static DietMode DrawResponseButton_GetResponse(Pawn pawn)
        {
            return pawn.TryGetComp<FullnessAndDietStats_ThingComp>().DietMode;
        }


        private static IEnumerable<Widgets.DropdownMenuElement<DietMode>> DrawResponseButton_GenerateMenu(Pawn p)
        {

            IEnumerator enumerator = Enum.GetValues(typeof(DietMode)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                DietMode response = (DietMode)enumerator.Current;
                    yield return new Widgets.DropdownMenuElement<DietMode>
                    {
                        option = new FloatMenuOption(
                            response.GetLabel(), 
                            delegate ()
                                {
                                    p.TryGetComp<FullnessAndDietStats_ThingComp>().DietMode = response;
                                }, 
                            response.GetIcon(), 
                            Color.white, 
                            MenuOptionPriority.Default, 
                            null, 
                            null, 
                            0f, 
                            null, 
                            null, 
                            true, 
                            0),
                        payload = response
                    };
            }
            
            enumerator = null;
            yield break;
        }



        private static readonly Texture2D NutritionModeIcon = ContentFinder<Texture2D>.Get("UI/WeightGizmo/WeightGizmo_ModeButtons/WeightGizmo_Mode_Nutrition", true);
        private static readonly Texture2D FullnessModeIcon = ContentFinder<Texture2D>.Get("UI/WeightGizmo/WeightGizmo_ModeButtons/WeightGizmo_Mode_Fullness", true);
        private static readonly Texture2D HybridModeIcon = ContentFinder<Texture2D>.Get("UI/WeightGizmo/WeightGizmo_ModeButtons/WeightGizmo_Mode_Hybrid", true);
        private static readonly Texture2D DisabledModeIcon = ContentFinder<Texture2D>.Get("UI/Icons/HostilityResponse/Ignore", true);
        private static readonly Color IconColor = new Color(0.84f, 0.84f, 0.84f);
    }
}
