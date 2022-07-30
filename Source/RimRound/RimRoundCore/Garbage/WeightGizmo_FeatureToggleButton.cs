using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.UI
{
	[StaticConstructorOnStartup]
    public class WeightGizmo_FeatureToggleButton
    {
		public bool enabled = true;


		public void DrawButton(Rect rect)
		{
			Rect position = new Rect
			{
				x = rect.x + 1,
				y = rect.y + 1,
				width = 3f,
				height = 3f
			};


			GUI.DrawTexture(position, BaseContent.BlackTex);


			Widgets.Checkbox(rect.position, ref enabled, 24, false, false, onTex, BaseContent.BlackTex);

		}

		private static readonly Texture2D onTex = ContentFinder<Texture2D>.Get("UI/WeightGizmo/CheckboxOn", true);
	}
}
