using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

using RimRound.Comps;
using RimWorld;
using RimRound.Utilities;

namespace RimRound.UI
{
    public class MakeBlobIntoBedGizmo : Command_Toggle
    {
        public MakeBlobIntoBedGizmo(MakeBlobIntoBed_ThingComp comp) 
        {
            this.comp = comp;
            defaultLabel = "Blob bed";
            defaultDesc = "Whether this pawn will serve as a resting place for other colonists";
            icon = Widgets.GetIconFor(RimWorld.ThingDefOf.SleepingSpot);
            isActive = () => comp.IsBed;
            toggleAction = () => { ToggleAction(); };
            order = 401;
        }

        private void ToggleAction()
        {
            comp.IsBed = !comp.IsBed;
            if (comp.IsBed)
            {
                IntVec3 parentPos = new IntVec3(comp.parent.Position.x, comp.parent.Position.y, comp.parent.Position.z);
                parentPos.z -= 1;
                comp.blobBed = GenSpawn.Spawn(RimRound.Defs.ThingDefOf.BlobBed, parentPos, comp.parent.Map);
                
                comp.blobBed.SetFaction(Faction.OfPlayer, null);
                Utilities.HediffUtility.AddHediffSeverity(Defs.HediffDefOf.RimRound_BlobBed, comp.parent.AsPawn(), 1.0f, true);
            }
            else 
            {
                Reset();
            }
        }


        public void Reset() 
        {
            comp.IsBed = false;
            Utilities.HediffUtility.RemoveHediffOfDefFrom(Defs.HediffDefOf.RimRound_BlobBed, comp.parent.AsPawn());
            if (GeneralUtility.IsNotNull(comp.blobBed))
            {
                comp.blobBed.DeSpawn();
                comp.blobBed = null;
            }
                
        }

        MakeBlobIntoBed_ThingComp comp;
    }
}
