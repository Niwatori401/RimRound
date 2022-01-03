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
    [HarmonyPatch(typeof(BodyPartDef))]
    [HarmonyPatch(nameof(BodyPartDef.GetMaxHealth))]
    class BodyPartDef_GetMaxHealth_AdjustMaxHPByWeight
    {
        public static void Postfix(ref float __result, BodyPartDef __instance, Pawn __0) 
        {
            foreach (var x in affectedBodyParts) 
            {
                if (x.label == __instance.label)
                {
                    int index = GetHediffIndex(__0);
                    if (weightStageToHealthMult.ContainsKey(index))
                        __result *= weightStageToHealthMult[index];
                    else
                        Log.Error($"Index {index} was not in the dictonary for pawn named {__0.Name}!");
                    
                    return;
                }
            }
        }


        static List<BodyPartDef> affectedBodyParts = new List<BodyPartDef>()
        {
            BodyPartDefOf.Arm,
            BodyPartDefOf.Leg,
            BodyPartDefOf.Torso
        };

        static int GetHediffIndex(Pawn p) 
        {
            Hediff weight = p.WeightHediff();

            if (weight is null)
                return 0;

            return weight.CurStageIndex;
        }

        static Dictionary<int, float> weightStageToHealthMult = new Dictionary<int, float>() 
        {
            { 0,  1f    },
            { 1,  1f    },
            { 2,  1f    },
            { 3,  1.13f },
            { 4,  1.23f },
            { 5,  1.33f },
            { 6,  1.50f },
            { 7,  1.70f },
            { 8,  1.93f },
            { 9,  2.23f },
            { 10, 2.43f },
            { 11, 2.76f },
            { 12, 3.23f },
            { 13, 3.76f },
            { 14, 4.46f },
            { 15, 5.30f },
            { 16, 6.23f },
            { 17, 7.33f },
            { 18, 8.63f },
            { 19, 10.0f }
        };

    }
}
