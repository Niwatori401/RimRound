using HarmonyLib;
using RimRound.Utilities;
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
    [HarmonyPatch(typeof(PortraitsCache))]
    [HarmonyPatch(nameof(PortraitsCache.Get))]
    public class PortraitsCache_Get_MakePortraitsScaleForBodySize
    {
        public static bool Prefix(Pawn __0, ref Vector3 __3, ref float __4) 
        {
            //3 moves the portrait around
            //1 is the resolution?
            //4 is the zoom of everything
            BodyTypeDef pawnBodytype = __0?.story?.bodyType;
            if (pawnBodytype is null)
                return true;

            if (!GlobalSettings.useZoomPortraitStyle || pawnBodytype.defName == Defs.BodyTypeDefOf.F_100_Gelatinous.defName)
            {
                if (Values.portraitOffsetByBodyTypeZoomMethod.ContainsKey(pawnBodytype) && Values.portraitZoomByBodyType.ContainsKey(pawnBodytype))
                {
                    __3.z = Values.portraitOffsetByBodyTypeZoomMethod[pawnBodytype];
                    __4 = Values.portraitZoomByBodyType[pawnBodytype];
                    return true;
                }
            }
            else 
            {
                if (Values.portraitOffsetByBodyTypeOffsetMethod.ContainsKey(pawnBodytype)) 
                {
                    __3.z = Values.portraitOffsetByBodyTypeOffsetMethod[pawnBodytype];
                    return true;
                }
            }
            
            return true;
        }
    }
}
