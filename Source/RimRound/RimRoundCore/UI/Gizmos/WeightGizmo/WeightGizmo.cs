
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimRound.Comps;
using RimRound.Utilities;

namespace RimRound.UI
{
    [StaticConstructorOnStartup]
	public class WeightGizmo : Gizmo
    {
		public WeightGizmo(FullnessAndDietStats_ThingComp comp) 
		{
			WGThingComp = comp;
			Order = -69.0f;
		}


		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
			//Large Rectangle that contains the whole gizmo
			Rect rect = new Rect(topLeft.x, topLeft.y - this.extraHeight, this.GetWidth(maxWidth), this.OverrideHeight);
			Widgets.DrawWindowBackground(rect);

			//The highlighted rectangle if you hover over the main gizmo.
			Rect rect2 = rect.ContractedBy(6f);


			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
			Rect titleRect = new Rect(rect2);
			titleRect.height = Text.LineHeight;
			Widgets.Label(rect2, "Dietary Management");


			if (WGThingComp.DietMode != DietMode.Disabled) 
			{
				Text.Font = GameFont.Tiny;
				Text.Anchor = TextAnchor.UpperLeft;
				Widgets.Label(
					new Rect
					{
						x = titleRect.x,
						y = titleRect.yMax + 1f,
						width = titleRect.width,
						height = titleRect.height
					},
					$"Fullness: {(WGThingComp.fullnessbar.CurrentFullnessAsPercentOfSoftLimit * 100).ToString("F0")}% ({WGThingComp.CurrentFullness.ToString("F1")}/{WGThingComp.SoftLimit.ToString("F1")}L)");
			}


			float gapBetweenBars = 2f;
			WGThingComp.fullnessbar.DrawOnGUI(rect2, rect2.yMax - WeightGizmo_FullnessBar.barHeight - gapBetweenBars);

			WGThingComp.nutritionbar.DrawOnGUI(rect2, WGThingComp.fullnessbar.yPosition - WeightGizmo_NutritionBar.barHeight - gapBetweenBars);

			float modeButtonSize = 30f;
			Rect modeButtonContainer = new Rect
			{
				x = rect2.x + rect2.width - modeButtonSize,
				y = rect2.y,
				width = modeButtonSize,
				height = modeButtonSize
			};

			GUI.DrawTexture(modeButtonContainer, modeHighlight);

			WeightGizmo_ModeButton.DrawResponseButton(
				modeButtonContainer,
                (Pawn)WGThingComp.parent, 
				false);


			float resetButtonWidth = 15;
			Rect resetButtonRect = new Rect
			{
				x = modeButtonContainer.x - resetButtonWidth,
				y = modeButtonContainer.y,
				width = resetButtonWidth,
				height = resetButtonWidth
			};

			if (Widgets.ButtonImageFitted(resetButtonRect, resetButtonIcon))
			{
				WGThingComp.fullnessbar.ResetDietSettings();
				WGThingComp.nutritionbar.ResetDietSettings();
			}

			return new GizmoResult(GizmoState.Clear);
		}



		//For sizing the main box
		private readonly float extraHeight = Text.LineHeight * 1.5f;

		public override float GetWidth(float maxWidth)
		{
			return 212f;
		}

		private float OverrideHeight
		{
			get
			{
				return 75f + this.extraHeight;
			}
		}


		FullnessAndDietStats_ThingComp WGThingComp;

		private static readonly Texture2D modeHighlight = SolidColorMaterials.NewSolidColorTexture(new Color(0.121f, 0.133f, 0.145f));
		private static readonly Texture2D resetButtonIcon = ContentFinder<Texture2D>.Get("UI/WeightGizmo/resetSymbol", true);

	}
}