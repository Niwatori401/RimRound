using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    /// <summary>
    /// Wipes all static caches on load.
    /// </summary>
    [HarmonyPatch(typeof(GameDataSaveLoader))]
    [HarmonyPatch(nameof(GameDataSaveLoader.LoadGame), new Type[] { typeof(string) })]
    public class GameDataSaveLoader_LoadGame_WipeCaches
    {
        public static void Postfix()
        {
            BodyTypeUtility.InvalidateCorpseCache();
            RacialBodyTypeInfoUtility.InvalidateCaches();
            PawnRenderer_GetBodyPos_HideBlankets.InvalidateCaches();
            Corpse_CurRotDrawMode_Get_ChangeDessicatedCorpseBodyTypeToNonRR.InvalidateCache();
            ApparelGraphicRecordGetter_TryGetGraphicApparel_UseTransparentImagesForBadTex.InvalidateCache();
        }
    }
}
