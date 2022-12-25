using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

using RimRound.Comps;

namespace RimRound.UI
{
	[StaticConstructorOnStartup]
    public class WeightGizmo_NutritionBar
	{

		public WeightGizmo_NutritionBar(Pawn pawn)
		{
			needFood = pawn.needs.food;
		}


		public void DrawOnGUI(Rect inRect, float yPos, float customMargin = -1f, bool drawArrows = false, bool doTooltip = true, Rect? rectForTooltip = null)
		{
			List<float> threshPercentCopy = Traverse.Create(needFood).Field("threshPercent").GetValue() as List<float>;

			if (threshPercentCopy == null)
			{
				threshPercentCopy = new List<float>();
			}
			threshPercentCopy.Clear();
		 	threshPercentCopy.Add(needFood.PercentageThreshHungry);
			threshPercentCopy.Add(needFood.PercentageThreshUrgentlyHungry);


			Rect containingRect = rectForTooltip ?? inRect;

			if (Mouse.IsOver(containingRect))
			{
				Widgets.DrawHighlight(containingRect);
			}

			/*
			if (doTooltip && Mouse.IsOver(containingRect))
			{
				TooltipHandler.TipRegion(containingRect, new TipSignal(() => needFood.GetTipString(), containingRect.GetHashCode()));
			}
			*/






			float maxFoodValue = 1f;
			if (needFood.def.scaleBar && needFood.MaxLevel < 1f)
			{
				maxFoodValue = needFood.MaxLevel;
			}


			float marginAmount = (customMargin >= 0f) ? customMargin : 1f;

			Rect backgroundBar = new Rect
			{
				width = inRect.width - 2 * marginAmount,
				height = barHeight,
				x = inRect.x + marginAmount,
				y = yPos
			};

			yPosition = backgroundBar.y;

			Rect barRect;
			
			if (enabled)
				barRect = Widgets.FillableBar(backgroundBar, needFood.CurLevelPercentage);
			else
				barRect = Widgets.FillableBar(backgroundBar, needFood.CurLevelPercentage, DisabledTex);


			if (drawArrows)
			{
				Widgets.FillableBarChangeArrows(backgroundBar, needFood.GUIChangeArrow);
			}

			//Adds the little dashes every threshPercentCopy[i]
			if (threshPercentCopy != null)
			{
				for (int i = 0; i < threshPercentCopy.Count; i++)
				{
					DrawBarThreshold(barRect, threshPercentCopy[i] * maxFoodValue);
				}
			}

			//If the food maxvalue is more than num5 (in this case 1) draw an extra divider.
			if (needFood.def.scaleBar)
			{
				int num5 = 1;
				while ((float)num5 < needFood.MaxLevel)
				{
					DrawBarDivision(barRect, (float)num5 / needFood.MaxLevel * maxFoodValue);

					num5++;
				}
			}



			Text.Font = GameFont.Small;


			if (thresholdSlider != null)
				thresholdSlider.DrawSlider(barRect, SliderTex, needFood.MaxLevel, 1);
			if (maxSlider != null)
				maxSlider.DrawSlider(barRect, SliderTex, needFood.MaxLevel, 1);
		}


		public void UpdateBar(DietMode dietMode) 
		{
			if (dietMode == DietMode.Disabled)
				enabled = false;
			else
				enabled = true;


			UpdateSliders(dietMode);
		}


		void UpdateSliders(DietMode dietMode) 
		{
			if (thresholdSlider?.BarPercentage is float percent1)
				thresholdSliderLastValue = percent1;

			if (maxSlider?.BarPercentage is float percent2)
				maxSliderLastValue = percent2;


			thresholdSlider = null;
			maxSlider = null;

			switch (dietMode)
			{
				case DietMode.Nutrition:
					thresholdSlider = new Slider(thresholdSliderLastValue, false, needFood.MaxLevel);
					maxSlider = new Slider(maxSliderLastValue, false, needFood.MaxLevel);
					return;
				case DietMode.Hybrid:
					thresholdSlider = new Slider(thresholdSliderLastValue, true, needFood.MaxLevel);
					return;
				case DietMode.Fullness:
					return;
				case DietMode.Disabled:
					return;
				default:
					return;
			}
		}






		protected void DrawBarDivision(Rect barRect, float threshPct) 
		{
			float num = 5f;
			Rect rect = new Rect(barRect.x + barRect.width * threshPct - (num - 1f), barRect.y, num, barRect.height);
			if (threshPct < needFood.CurLevelPercentage)
			{
				GUI.color = new Color(0f, 0f, 0f, 0.9f);
			}
			else
			{
				GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
			}
			Rect position = rect;
			position.yMax = position.yMin + 4f;
			GUI.DrawTextureWithTexCoords(position, NeedUnitDividerTex, new Rect(0f, 0.5f, 1f, 0.5f));
			Rect position2 = rect;
			position2.yMin = position2.yMax - 4f;
			GUI.DrawTextureWithTexCoords(position2, NeedUnitDividerTex, new Rect(0f, 0f, 1f, 0.5f));
			Rect position3 = rect;
			position3.yMin = position.yMax;
			position3.yMax = position2.yMin;
			if (position3.height > 0f)
			{
				GUI.DrawTextureWithTexCoords(position3, NeedUnitDividerTex, new Rect(0f, 0.4f, 1f, 0.2f));
			}
			GUI.color = Color.white;

		}

		protected void DrawBarThreshold(Rect barRect, float threshPct)
		{
			float num = (float)((barRect.width > 60f) ? 2 : 1);
			Rect position = new Rect(barRect.x + barRect.width * threshPct - (num - 1f), barRect.y + barRect.height / 2f, num, barRect.height / 2f);

			if (threshPct < needFood.CurLevelPercentage)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.9f);
				GUI.DrawTexture(position, blackTex);
			}
			else
			{
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				GUI.DrawTexture(position, greyTex);
			}
			
			GUI.color = Color.white;
		}

		public void ResetDietSettings()
		{
			SetRanges(lowerDefaultValue, upperDefaultValue);
		}

		private float lowerDefaultValue = 0.3f;
		private float upperDefaultValue = 0.8f;

		//First and second are percents
		public void SetRanges(float first, float second) 
		{

			//If either slider is disabled
			if (thresholdSlider == null || maxSlider == null)
			{
				//If the bar is disabled
				if (thresholdSlider == null && maxSlider == null)
				{
					return;
				}

				//If the max slider is null
				else if (maxSlider == null)
				{
					thresholdSlider.BarPercentage = first;
				}

				//If the max isn't null but the threshold is
				else if (thresholdSlider == null)
				{
					maxSlider.BarPercentage = first;
				}
				else
				{
					return;
				}
			}
			//If neither are disabled
			else
			{
				thresholdSlider.BarPercentage = first;
				maxSlider.BarPercentage = second;
			}
		}


		public Pair<float, float> GetRanges() 
		{
			//If either slider is disabled
			if (thresholdSlider == null || maxSlider == null)
			{
				//If the bar is disabled
				if (thresholdSlider == null && maxSlider == null)
				{
					return new Pair<float, float>(-1, -1);
				}

				//If the max slider is null
				else if (maxSlider == null)
				{
					return new Pair<float, float>(thresholdSlider.barValue, -1);
				}

				//If the max isn't null but the threshold is
				else if (thresholdSlider == null)
				{
					return new Pair<float, float>(maxSlider.barValue, -1);
				}
				else
				{
					return new Pair<float, float>(-1, -1);
				}
			}
			//If neither are disabled
			else
			{
				return thresholdSlider.barValue <= maxSlider.barValue ?
					new Pair<float, float>(thresholdSlider.barValue, maxSlider.barValue) :
					new Pair<float, float>(maxSlider.barValue, thresholdSlider.barValue);
			}
		}



		public Need_Food needFood;



		public float yPosition 
		{
			get
			{
				return nutritionBarYPos;
			}
			private set 
			{
				nutritionBarYPos = value;
			}
		}

		public float BarHeight 
		{
			get 
			{
				return barHeight;
			}
		}


		float nutritionBarYPos;


		private static readonly Texture2D NeedUnitDividerTex = ContentFinder<Texture2D>.Get("UI/Misc/NeedUnitDivider", true);
		private static readonly Texture2D blackTex = BaseContent.BlackTex;
		private static readonly Texture2D greyTex = BaseContent.GreyTex;
		private static readonly Texture2D SliderTex = SolidColorMaterials.NewSolidColorTexture(ColorLibrary.BlueGreen);
		private static readonly Texture2D DisabledTex = SolidColorMaterials.NewSolidColorTexture(ColorLibrary.Grey);

		Slider thresholdSlider;
		Slider maxSlider;





		float thresholdSliderLastValue;
		float maxSliderLastValue;


		public static float barHeight = 20;
        private bool enabled;
    }
}
