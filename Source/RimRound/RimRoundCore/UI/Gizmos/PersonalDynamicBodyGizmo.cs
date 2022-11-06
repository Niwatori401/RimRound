using RimRound.Comps;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.UI
{
    public class PersonalDynamicBodyGizmo : Command_Toggle
    {
        public PersonalDynamicBodyGizmo(PawnBodyType_ThingComp comp) 
        {
            this.comp = comp;
            defaultLabel = "Exempt Pawn - Dynamic Bodies Rule";
            defaultDesc = "Whether this pawn personally should obey global rules allowing dynamic bodies. When on, global rules are followed. When off, this pawn will never have a dynamic body regardless of the global rule.";
            icon = Widgets.GetIconFor(ThingDefOf.Fence);
            isActive = () => comp.PersonallyExempt;
            toggleAction = () => { ToggleAction(); };
            Order = 401;
        }


        private void ToggleAction()
        {
            comp.PersonallyExempt = !comp.PersonallyExempt;
        }


        PawnBodyType_ThingComp comp;
    }
}
