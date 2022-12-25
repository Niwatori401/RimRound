using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using RimWorld;
using Verse;
using UnityEngine;
using RimRound.Comps;


namespace RimRound.UI
{
    public static class WeightGizmo_DeathWarning
    {
        public static void DisplayWarning(Pawn pawn, List<Slider> sliderList) 
        {
            DiaNode diaNode = new DiaNode("FullnessDeathWarningExplanation".Translate(pawn.Name.ToStringShort));

            DiaOption diaOption1 = new DiaOption("FullnessDeathWarningKill".Translate());
            diaOption1.resolveTree = true;
            diaOption1.action = delegate () 
            {
                //"Do nothing" - Magic Conch Shell
                WeightGizmo_FullnessBar bar = pawn.TryGetComp<FullnessAndDietStats_ThingComp>().fullnessbar;
                bar.deathDialogOpen = false;
                bar.peaceForeverHeld = true;
            };
            
            diaNode.options.Add(diaOption1);




            DiaOption diaOption2 = new DiaOption("FullnessDeathWarningCancel".Translate());
            diaOption2.resolveTree = true;
            diaOption2.action = delegate ()
            {
                foreach (Slider s in sliderList)
                {
                    s.BarPercentage = 0;
                }
                pawn.TryGetComp<FullnessAndDietStats_ThingComp>().fullnessbar.deathDialogOpen = false;
            };

            diaNode.options.Add(diaOption2);



            Dialog_NodeTree dialog_NodeTree = new Dialog_NodeTree(diaNode, true, false, "FullnessDeathWarningTitle".Translate());
            dialog_NodeTree.forcePause = true;
            dialog_NodeTree.screenFillColor = Color.clear;

            Find.WindowStack.Add(dialog_NodeTree);

        }




        
    }
}
