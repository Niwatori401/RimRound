using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimRound.Comps;
using Mono.Cecil;
using RimRound.Utilities;

namespace RimRound.UI
{
    public class HideCoversGizmo : Command_Toggle
    {
        public HideCoversGizmo(HideCovers_ThingComp comp) 
        {
            this.comp = comp;
            defaultLabel = "Hide Covers";
            defaultDesc = "Whether you want to see a pawn's body when they sleep in a bed";
            icon = Resources.HIDE_COVERS_ICON;
            isActive = () => comp.HideCovers;
            toggleAction = () => { ToggleAction(); };
            Order = 401;
        }

        private void ToggleAction() 
        {
            comp.HideCovers = !comp.HideCovers;
        }


        HideCovers_ThingComp comp;
    }
}
