using HarmonyLib;
using RimRound.Comps;
using RimRound.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Page_ConfigureStartingPawns))]
    [HarmonyPatch(nameof(Page_ConfigureStartingPawns.PostOpen))]
    public class Page_ConfigureStartingPawns_PostOpen_WarnForCertainMods
    {
        public static void Postfix() 
        {
            List<Pair<string, string>> warningsToDisplay = new List<Pair<string, string>>();

            foreach (var modWarningPair in warningsAndMods) 
            {
                if (ModLister.HasActiveModWithName(modWarningPair.First)) 
                {
                    warningsToDisplay.Add(modWarningPair);
                }
            }

            DisplayWarning(warningsToDisplay);
        }


        public static void DisplayWarning(List<Pair<string, string>> warningsAndMods)
        {
            if (warningsAndMods.Count == 0)
                return;

            string errorString = "";

            foreach (var warning in warningsAndMods) 
            {
                errorString += warning.First;
                errorString += " - ";
                errorString += warning.Second;
                errorString += '\n';
            }

            DiaNode diaNode = new DiaNode(errorString);

            DiaOption diaOption1 = new DiaOption("Ok");
            diaOption1.resolveTree = true;
            diaOption1.action = delegate ()
            {
            };

            diaNode.options.Add(diaOption1);



            Dialog_NodeTree dialog_NodeTree = new Dialog_NodeTree(diaNode, true, false)
            {
                forcePause = true,
                screenFillColor = Color.clear
            };

            Find.WindowStack.Add(dialog_NodeTree);
        }


        private static List<Pair<string, string>> warningsAndMods = new List<Pair<string, string>>() 
        {
            new Pair<string, string>(
                "Character Editor", 
                "The mod 'Character Editor' has been detected as active. " +
                "RimRound does not support changing pawns in this menu. " +
                "You can still use it to customize your pawn, but if you wish to change anything related to RimRound, such as bodytype, weight opinion or body weight, you cannot do so here successfully. " +
                "Instead, please customize them with dev mode once you start. " +
                "This can be activated in the options menu. You can then click a pawn and change the stats there."
                ),
            new Pair<string, string>(
                "EdB Prepare Carefully",
                "The mod 'EdB Prepare Carefully' has been detected as active. " +
                "RimRound does not support changing pawns in this menu. " +
                "You can still use it to customize your pawn, but if you wish to change anything related to RimRound, such as bodytype, weight opinion or body weight, you cannot do so here successfully. " +
                "Instead, please customize them with dev mode once you start. " +
                "This can be activated in the options menu. You can then click a pawn and change the stats there."
                ),
        };
    }
}
