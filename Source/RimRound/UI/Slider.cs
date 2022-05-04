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
        public Slider(float startingPercent = 0f, bool isOnlySlider = false, float maxValue = 1) 
		{
			isOnlySliderMode = isOnlySlider;
			BarPercentage = startingPercent;
            this.maxValue = maxValue;
		}

		public void DrawSlider(Rect rect, Texture2D tex, float maxValue= 1f, float margin = -1f)
        {
            this.maxValue = maxValue;
            marginSize = margin > 0f ? margin : 1f;

            Rect containingRect = new Rect
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
            Rect topNubHitbox = new Rect
            {
                y = rect.y - 3f,
                height = 5f,
                xMin = nubAdjustment,
                xMax = xMax
            };

            Rect bottomNubHitbox = new Rect
            {
                y = rect.yMax - 2f,
                height = 5f,
                xMin = nubAdjustment,
                xMax = xMax
            };

            bool isMouseOverSlider;

            if (isOnlySliderMode)
                isMouseOverSlider = Mouse.IsOver(containingRect);
            else
            {
                isMouseOverSlider = Mouse.IsOver(sliderRect) || Mouse.IsOver(topNubHitbox) || Mouse.IsOver(bottomNubHitbox);
            }


            //Limits the value of the dragging bar between 0 and 1 as well as gives its smoothness.
            float currentPositionAsPercent = Mathf.Clamp(
                Mathf.Round(
                    (Event.current.mousePosition.x - (containingRect.x + 3f)) / (containingRect.width - 8f) * barDraggingPrecision)
                    / barDraggingPrecision,
                0f,
                1f);


            //Mouse input handling
            Event current = Event.current;

            HandleInitialClickOnSlider(isMouseOverSlider, currentPositionAsPercent, current);

            HandleSlidingOfSlider(currentPositionAsPercent, current);

            HandleReleaseOfMouseOnSlider(current);

            MakeSlider(containingRect, tex, SliderCurrentPercentage);
        }

        private void HandleInitialClickOnSlider(bool isMouseOverSlider, float currentPositionAsPercent, Event current)
        {
            if (current.type == EventType.MouseDown && current.button == 0 && isMouseOverSlider)
            {
                this.sliderHoveringPercentage = currentPositionAsPercent;
                this.draggingBar = true;
                SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                mouseDown = true;
                current.Use();
            }
        }

        private void HandleSlidingOfSlider(float currentPositionAsPercent, Event current)
        {
            if (current.type == EventType.MouseDrag && current.button == 0 && this.draggingBar && mouseDown)
            {
                if (Mathf.Abs(currentPositionAsPercent - this.sliderHoveringPercentage) > 1E-45f)
                {
                    SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                }
                this.sliderHoveringPercentage = currentPositionAsPercent;
                current.Use();
            }
        }

        private void HandleReleaseOfMouseOnSlider(Event current)
        {
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
