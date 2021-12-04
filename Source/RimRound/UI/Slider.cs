using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;

namespace RimRound.UI
{
    public class Slider
    {
		public Slider(float startingPercent = 0f, bool isOnlySlider = false) 
		{
			isOnlySliderMode = isOnlySlider;
			BarPercentage = startingPercent;
		}

		public void DrawSlider(Rect rect, Texture2D tex, float maxValue= 1f, float margin = -1f)
		{
			this.maxValue = maxValue;
			marginSize = margin > 0f ? margin : 1f;

			Rect rect1 = new Rect
			{
				width = rect.width - 2 * marginSize,
				height = rect.height,
				x = rect.x + marginSize,
				y = rect.y
			};

			float percentOffset = Mathf.Round((rect.width - 8f) * SliderCurrentPercentage);

			Rect sliderRect = new Rect
			{
				x = rect.x + 3f + percentOffset,
				y = rect.y,
				width = 2f,
				height = rect.height
			};
			float nubAdjustment = Widgets.AdjustCoordToUIScalingFloor(rect.x + 2f + percentOffset);
			float xMax = Widgets.AdjustCoordToUIScalingCeil(nubAdjustment + 4f);
			Rect topNub = new Rect
			{
				y = rect.y - 3f,
				height = 5f,
				xMin = nubAdjustment,
				xMax = xMax
			};

			Rect bottomNub = new Rect
			{
				y = rect.yMax - 2f,
				height = 5f,
				xMin = nubAdjustment,
				xMax = xMax
			};

			bool flag;

			if (isOnlySliderMode)
				flag = Mouse.IsOver(rect1);
			else
			{
				flag = Mouse.IsOver(sliderRect) || Mouse.IsOver(topNub) || Mouse.IsOver(bottomNub);
			}


			//Limits the value of the dragging bar between 0 and 1 as well as gives its smoothness.
			float num = Mathf.Clamp(
				Mathf.Round(
					(Event.current.mousePosition.x - (rect1.x + 3f)) / (rect1.width - 8f) * barDraggingPrecision)
					/ barDraggingPrecision,
				0f,
				1f);


			//Mouse input handling
			Event current = Event.current;

			//Upon first click on slider
			if (current.type == EventType.MouseDown && current.button == 0 && flag)
			{
				this.sliderHoveringPercentage = num;
				this.draggingBar = true;
				SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
				mouseDown = true;
				current.Use();
			}
			//Updates the value of hoveringStrengthTarget while it is being dragged.
			if (current.type == EventType.MouseDrag && current.button == 0 && this.draggingBar && mouseDown)
			{
				if (Mathf.Abs(num - this.sliderHoveringPercentage) > 1E-45f)
				{
					SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
				}
				this.sliderHoveringPercentage = num;
				current.Use();
			}
			//This is where the value of the slider is written.
			if (current.type == EventType.MouseUp && current.button == 0 && this.draggingBar)
			{
				if (this.sliderHoveringPercentage >= 0f)
				{
					BarPercentage = this.sliderHoveringPercentage;
				}
				this.sliderHoveringPercentage = -1f;
				this.draggingBar = false;
				mouseDown = false;
				current.Use();
			}

			MakeSlider(rect1, tex, SliderCurrentPercentage);
		}



		void MakeSlider(Rect rect, Texture2D tex, float percent)
		{
			float num = Mathf.Round((rect.width - 8f) * percent);
			GUI.DrawTexture(new Rect(rect.x + 3f + num, rect.y, 2f, rect.height), tex);

			float num2 = Widgets.AdjustCoordToUIScalingFloor(rect.x + 2f + num);
			float xMax = Widgets.AdjustCoordToUIScalingCeil(num2 + 4f);
			Rect rect2 = new Rect
			{
				y = rect.y - 3f,
				height = 5f,
				xMin = num2,
				xMax = xMax
			};

			//The little nubs
			GUI.DrawTexture(rect2, tex);
			Rect position = rect2;
			position.y = rect.yMax - 2f;
			GUI.DrawTexture(position, tex);
		}



		private float maxValue = 1f;
		private bool mouseDown = false;
		public bool isOnlySliderMode = false;

		private float marginSize;
		private float barDraggingPrecision = 100f;
		private float sliderHoveringPercentage = 0.3f;
		public bool draggingBar = false;
		public float barValue = 0.4f;

		public float BarPercentage
		{
			get
			{
				if (maxValue == 0)
					return 0;

				return barValue / maxValue;
			}
			set
			{
				barValue = value * maxValue;
			}
		}

		private float SliderCurrentPercentage
		{
			get
			{
				if (!this.draggingBar)
				{
					return BarPercentage;
				}

				return this.sliderHoveringPercentage;
			}
		}
	}
}
