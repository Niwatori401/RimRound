using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Need_Food))]
    [HarmonyPatch("MalnutritionSeverityPerInterval", MethodType.Getter)]
    class Need_Food_MalnutritionSeverityPerInterval_AdjustByPawnWeight
    {
        public static void Postfix(ref float __result, Pawn ___pawn) 
        {
            if (!___pawn.RaceProps.Humanlike)
                return;
            __result *= GetMalnutritionMultByWeight(___pawn);
            return;
        }

        static float GetMalnutritionMultByWeight(Pawn p) 
        {
            return 1 / (20 * Utilities.HediffUtility.KilosToSeverity(p.Weight()) + 1);
        }
    }
}
