using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    public class IsMountableUtility_IsMountable_IncludeMoblityChairs
    {
        static void Postfix(Pawn __0, ref bool __result)
        {
            if (__0.def.defName == "RR_HoverChair") 
            {
                __result = true;
            }
        }

        public static PatchCollection GetPatchCollection()
        {
            return new PatchCollection
            {
                postfix = typeof(IsMountableUtility_IsMountable_IncludeMoblityChairs).GetMethod(
                    nameof(IsMountableUtility_IsMountable_IncludeMoblityChairs.Postfix), ModCompatibilityUtility.majorFlags)
            };
        }
    }
}
