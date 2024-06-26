﻿using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimRound.UI
{
    public class PersonalDynamicBodyGizmo : Command_Toggle
    {
        public PersonalDynamicBodyGizmo(PawnBodyType_ThingComp comp) 
        {
            this.comp = comp;
            defaultLabel = "Exempt Pawn - Dynamic Bodies Rule";
            defaultDesc = "Whether this pawn personally should obey global rules allowing dynamic bodies. When on, global rules are followed. When off, this pawn will never have a dynamic body regardless of the global rule.";
            icon = Utilities.Resources.FILLER_TEXTURE;
            isActive = () => comp.PersonallyExempt;
            toggleAction = () => { ToggleAction(); };
            Order = 401;
        }


        private void ToggleAction()
        {
            Utilities.Resources.gizmoClick.PlayOneShotOnCamera(null);

            if (comp.PersonallyExempt)
                comp.PersonallyExempt = false;
            else
                comp.PersonallyExempt = new ExemptionReason("RR_PersonallyExemptFromGizmo".Translate());
        }       


        PawnBodyType_ThingComp comp;
    }
}
