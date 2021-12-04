
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimRound.Comps;

namespace RimRound.UI
{
    [StaticConstructorOnStartup]
	public class WeightGizmo : Gizmo
    {
		public WeightGizmo(FullnessAndDietStats_ThingComp comp) 
		{
			WGThingComp = comp;
			this.order = -69.0f;
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
			Widgets.Label(rect2, "Dietary Management");

			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.Label(
				new Rect
				{
					x = rect2.x,
					y = rect2.y + 3f,
					width = rect2.width,
					height = rect2.height
				},
				"\nTest Phrase");


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
		
	}
}