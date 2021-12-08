using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(ExpansionDef))]
    [HarmonyPatch(nameof(ExpansionDef.ConfigErrors))]
    public class ExpansionDef_ConfigErrors_HideConfigErrorsForRimRound
    {
        public static void Postfix(ref IEnumerable<string> __result) 
        {
            List<string> listOfErrors = new List<string>(__result);
            List<string> newListOfErrors = new List<string>();
            foreach (var s in listOfErrors) 
            {
                if (!s.Contains("RimRound")) 
                {
                    newListOfErrors.Add(s);
                }
            }


            __result = newListOfErrors;
        }
    }
}
