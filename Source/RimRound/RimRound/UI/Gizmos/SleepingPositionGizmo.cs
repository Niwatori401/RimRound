using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using RimWorld;
using UnityEngine;
using Verse;
using RimRound.Comps;
using RimRound.Utilities;
using Resources = RimRound.Utilities.Resources;
using Verse.Sound;

namespace RimRound.UI
{
	[StaticConstructorOnStartup]
    public class SleepingPositionGizmo : Command
    {
        public SleepingPositionGizmo(SleepingPosition_ThingComp comp) 
        {
			this.comp = comp;
			defaultLabel = "Sleeping Position";
			defaultDesc = "Changes which direction you want this pawn to face when they sleep.";
			icon = Resources.SLEEPING_POSITION_ICON;
			isActive = () => fakebool;
			toggleAction = () => { ToggleAction(); };
			Order = 402;
		}

		void ToggleAction() 
		{
            Resources.gizmoClick.PlayOneShotOnCamera(null);
            comp.sleepIndex++;
			comp.sleepIndex %= 4;
			fakebool = !fakebool;
			comp.parent.AsPawn().GetComp<MakeBlobIntoBed_ThingComp>().gizmo.Reset();
		}
		
		public override GizmoResult GizmoOnGUI(Vector2 loc, float maxWidth, GizmoRenderParms parms)
		{
			GizmoResult result = base.GizmoOnGUI(loc, maxWidth, parms);

			Rect rect = new Rect(loc.x, loc.y, this.GetWidth(maxWidth), 75f);
			Rect position = new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f);
			Texture2D image = this.isActive() ? blankTex : blankTex;
			GUI.DrawTexture(position, image);
			return result;
		}


        SleepingPosition_ThingComp comp;

		bool fakebool = false;

		static readonly Texture2D blankTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);


		public override SoundDef CurActivateSound
		{
			get
			{
				return this.turnOnSound;
			}
		}

		public override void ProcessInput(Event ev)
		{
			base.ProcessInput(ev);
			this.toggleAction();
		}

		public override bool InheritInteractionsFrom(Gizmo other)
		{
			Command_Toggle command_Toggle = other as Command_Toggle;
			return command_Toggle != null && command_Toggle.isActive() == this.isActive();
		}

		public Func<bool> isActive;

		public Action toggleAction;

		public SoundDef turnOnSound = SoundDefOf.Checkbox_TurnedOn;

		public SoundDef turnOffSound = SoundDefOf.Checkbox_TurnedOff;

		public bool activateIfAmbiguous = true;
	}
}
