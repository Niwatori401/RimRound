using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.UI
{
    public class MakeGeneratorGizmo : Command_Toggle
    {
        public MakeGeneratorGizmo(MakeBlobIntoBed_ThingComp comp, FullnessAndDietStats_ThingComp fndComp) 
        {
            this.comp = comp;
            this.fndComp = fndComp;
            defaultLabel = "Make Generator";
            defaultDesc = "Whether this pawn will serve basic power generator";
            icon = Resources.MAKE_GENERATOR_ICON;
            isActive = () => comp.IsPowerSpot;
            toggleAction = () => { ToggleAction(); };
            Order = 404;
        }
        

        private void ToggleAction() 
        {
            comp.IsPowerSpot = !comp.IsPowerSpot;
            if (comp.IsPowerSpot)
            {
                IntVec3 parentPos = new IntVec3(comp.parent.Position.x, comp.parent.Position.y, comp.parent.Position.z);
                parentPos.z += 1;

                ThingDef generatorToSpawn;

                switch (fndComp?.perkLevels?.PerkToLevels?["RR_PaunchPower_Title"] ?? 4)
                {
                    case 0:
                        generatorToSpawn = null;
                        break;
                    case 1:
                        generatorToSpawn = Defs.ThingDefOf.RR_PaunchGenerator_I;
                        break;
                    case 2:
                        generatorToSpawn = Defs.ThingDefOf.RR_PaunchGenerator_II;
                        break;
                    default:
                        generatorToSpawn = null;
                        Log.Error("Paunch Power level in Blob Bed gizmo was not handled!");
                        break;
                }

                comp.generatorSpot = GenSpawn.Spawn(generatorToSpawn, parentPos, comp.parent.Map);

                comp.generatorSpot.SetFaction(Faction.OfPlayer, null);
                Utilities.HediffUtility.AddHediffSeverity(Defs.HediffDefOf.RimRound_BlobBed_III, comp.parent.AsPawn(), 1.0f, true);
            }
            else
            {
                Reset();
            }

        }
        public void Reset()
        {
            comp.IsPowerSpot = false;
            Utilities.HediffUtility.RemoveHediffOfDefFrom(Defs.HediffDefOf.RimRound_BlobBed_III, comp.parent.AsPawn());
            if (GeneralUtility.IsNotNull(comp.generatorSpot))
            {
                comp.generatorSpot.DeSpawn();
                comp.generatorSpot = null;
            }

        }
        FullnessAndDietStats_ThingComp fndComp;
        MakeBlobIntoBed_ThingComp comp;
    }
}
