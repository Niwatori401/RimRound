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
            BodyTypeInfo? bodyTypeInfo = RacialBodyTypeInfoUtility.GetRacialBodyTypeInfo(__0);
            if (bodyTypeInfo is null)
                return true;


            if (!GlobalSettings.useZoomPortraitStyle || RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(__0.story.bodyType).defName == Defs.BodyTypeDefOf.F_100_Gelatinous.defName || RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(__0.story.bodyType).defName == Defs.BodyTypeDefOf.F_100a_Gelatinous.defName) 
            {
                __3.z = bodyTypeInfo.AsNonNullable().portraitOffsetZoomMethod;
                __4 = bodyTypeInfo.AsNonNullable().portraitZoom;
                //__3.z = Values.debugPos2;
                //__4 = Values.debugPos;


                return true;
            }
            else 
            {
                __3.z = bodyTypeInfo.AsNonNullable().portraitOffsetPanMethod;
                return true;
            }
            
        }
    }
}
