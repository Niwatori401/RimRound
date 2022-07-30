using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlienRace;
using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(AlienRace.HarmonyPatches))]
    [HarmonyPatch(nameof(AlienRace.HarmonyPatches.ResolveAllGraphicsPrefix))]
    public class HarmonyPatches_ResolveAllGraphicsPrefix_UpdatePawnBodySize
    {
        public static bool Prefix(PawnGraphicSet __0) 
        {
            if (__0?.pawn is null || !__0.pawn.RaceProps.Humanlike)
                return true;

            PawnBodyType_ThingComp PBTComp = __0.pawn.TryGetComp<PawnBodyType_ThingComp>();

            if (PBTComp != null)
                if (!PBTComp.usingCustomBodyMeshSize)
                    BodyTypeUtility.UpdatePawnDrawSize(__0.pawn, PBTComp.PersonallyExempt, PBTComp.CategoricallyExempt);

            return true;
        }
    }
}
