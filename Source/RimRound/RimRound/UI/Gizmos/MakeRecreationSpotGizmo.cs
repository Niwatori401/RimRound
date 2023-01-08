using RimRound.Comps;
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
using Resources = RimRound.Utilities.Resources;

namespace RimRound.UI
{
    public class MakeRecreationSpotGizmo : Command_Toggle
    {
        public MakeRecreationSpotGizmo(MakeBlobIntoBed_ThingComp comp) 
        {
            this.comp = comp;
            defaultLabel = "Recreation Spot";
            defaultDesc = "Whether this pawn will serve as a recreation spot";
            icon = Resources.RECREATION_SPOT_ICON;
            isActive = () => comp.IsRecSpot;
            toggleAction = () => { ToggleAction(); };
            Order = 402;
        }

        private void ToggleAction() 
        {
            Resources.gizmoClick.PlayOneShotOnCamera(null);
            comp.IsRecSpot = !comp.IsRecSpot;
            if (comp.IsRecSpot)
            {
                IntVec3 parentPos = new IntVec3(comp.parent.Position.x, comp.parent.Position.y, comp.parent.Position.z);
                parentPos.z -= 2;

                ThingDef recSpotToSpawn = Defs.ThingDefOf.WLTP_Building;

                comp.recSpot = GenSpawn.Spawn(recSpotToSpawn, parentPos, comp.parent.Map);

                comp.recSpot.SetFaction(Faction.OfPlayer, null);
                Utilities.HediffUtility.AddHediffSeverity(Defs.HediffDefOf.RimRound_BlobBed_II, comp.parent.AsPawn(), 1.0f, true);
            }
            else
            {
                Reset();
            }
        }

        public void Reset()
        {
            comp.IsRecSpot = false;
            Utilities.HediffUtility.RemoveHediffOfDefFrom(Defs.HediffDefOf.RimRound_BlobBed_II, comp.parent.AsPawn());
            if (GeneralUtility.IsNotNull(comp.recSpot))
            {
                comp.recSpot.DeSpawn();
                comp.recSpot = null;
            }

        }

        MakeBlobIntoBed_ThingComp comp;
    }
}
