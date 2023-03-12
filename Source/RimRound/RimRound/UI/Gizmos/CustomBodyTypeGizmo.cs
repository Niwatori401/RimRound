using RimRound.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Sound;
using Verse;
using RimRound.Defs;
using RimRound.FeedingTube;
using RimRound.Utilities;
using UnityEngine;

namespace RimRound.UI
{
    public class CustomBodyTypeGizmo : Command_Toggle
    {
        public CustomBodyTypeGizmo(PawnBodyType_ThingComp comp)
        {
            this.comp = comp;
            defaultLabel = "Custom body type set for pawn";
            defaultDesc = "Whether this pawn personally should use a custom body set. When on, you are to select the bodytype set that the pawn will use. This will override race/gender specific settings. Toggle again to reset.";
            icon = Utilities.Resources.FILLER_TEXTURE;
            isActive = () => { return comp.CustomBodyTypeDict != null; };
            toggleAction = () => { ToggleAction(); };
            Order = 404;
        }


        public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
        {
            base.DrawIcon(rect, buttonMat, parms);

            if (!isActive())
                CustomBodySelectDropdown.DrawResponseButton(rect, comp.parent.AsPawn(), false);
        }

        private void ToggleAction()
        {
            Utilities.Resources.gizmoClick.PlayOneShotOnCamera(null);
            if (comp.CustomBodyTypeDict != null)
            {
                comp.bodyTypeDictNameString = null;
            }
        }

        PawnBodyType_ThingComp comp;
        
    }
}
