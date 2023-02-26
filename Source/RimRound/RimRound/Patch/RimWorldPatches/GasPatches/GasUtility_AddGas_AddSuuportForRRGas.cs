using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace RimRound.Patch
{
    /// <summary>
    /// Intercepts calls to generate gas on the map. For example, from a comp explosion. 
    /// The explosion type in comp explosion is checked against predefined values below so see if it is RR gas or not.
    /// If not, call original function.
    /// </summary>
    [HarmonyPatch(typeof(Verse.GasUtility))]
    [HarmonyPatch(nameof(Verse.GasUtility.AddGas), new Type[] { typeof(IntVec3), typeof(Map), typeof(GasType), typeof(int) })]
    public class GasUtility_AddGas_AddSuuportForRRGas
    {
        const GasType rimRoundFatteningGas = (GasType)32;
        const GasType rimRoundGasType1 = (GasType)64;
        const GasType rimRoundGasType2 = (GasType)128;
        const GasType rimRoundGasType3 = (GasType)256;

        public static bool Prefix(IntVec3 cell, Map map, GasType gasType, int amount) 
        {
            switch (gasType)
            {
                case rimRoundFatteningGas:
                    map.GetComponent<MapComp_RRGasGrid>().AddGas(cell, RRGasType.fatteningGas, amount, true);
                    return false;
                case rimRoundGasType1:
                    map.GetComponent<MapComp_RRGasGrid>().AddGas(cell, RRGasType.unused0, amount, true);
                    return false;
                case rimRoundGasType2:
                    map.GetComponent<MapComp_RRGasGrid>().AddGas(cell, RRGasType.unused1, amount, true);
                    return false;
                case rimRoundGasType3:
                    map.GetComponent<MapComp_RRGasGrid>().AddGas(cell, RRGasType.unused2, amount, true);
                    return false;
                default:
                    return true;
            }
        }
    }
}
