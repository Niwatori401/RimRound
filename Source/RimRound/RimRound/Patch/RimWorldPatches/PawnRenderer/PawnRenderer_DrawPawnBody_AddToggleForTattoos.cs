using HarmonyLib;
using RimRound.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    //[HarmonyPatch(typeof(PawnRenderer))]
    //[HarmonyPatch("DrawPawnBody")]
    //class PawnRenderer_DrawPawnBody_AddToggleForTattoos
    //{
    //    static bool Bigma(Pawn p) 
    //    {
    //        return (GlobalSettings.showBodyTatoosForCustomSprites || !BodyTypeUtility.HasCustomBody(p));
    //    }

    //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il) 
    //    {
    //        List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
    //        List<CodeInstruction> instructionsToAdd = new List<CodeInstruction>();

    //        Label jumpPoint = il.DefineLabel();
    //        bool foundInsertionPoint = false;
    //        bool foundInsertionPoint2 = false;

    //        int insertionJndex = -1;
    //        int endJndex = -1;

    //        MethodInfo ideologyActiveGetterInfo = typeof(Verse.ModsConfig).GetProperty("IdeologyActive").GetGetMethod();
    //        //MethodInfo drawMeshNowOrLaterInfo = AccessTools.Method(Verse.GenDraw:DrawMeshNowOrLater, new Type[] { typeof(Mesh), typeof(Vector3), typeof(Quaternion), typeof(Material), typeof(bool) });
    //        MethodInfo drawMeshNowOrLaterInfo = typeof(Verse.GenDraw).GetMethod(
    //            "DrawMeshNowOrLater", 
    //            BindingFlags.Static | BindingFlags.Public, 
    //            Type.DefaultBinder, 
    //            new Type[] 
    //            { 
    //                typeof(Mesh), 
    //                typeof(Vector3), 
    //                typeof(Quaternion), 
    //                typeof(Material), 
    //                typeof(bool) 
    //            }, 
    //            null);
            
    //        MethodInfo bigmaInfo = typeof(PawnRenderer_DrawPawnBody_AddToggleForTattoos).GetMethod(nameof(Bigma), BindingFlags.Static | BindingFlags.NonPublic);

    //        for (int jndex = 0; jndex < codeInstructions.Count; ++jndex)
    //        {
    //            if (codeInstructions[jndex].Calls(ideologyActiveGetterInfo))
    //            {
    //                foundInsertionPoint = true;

    //                insertionJndex = jndex;
    //                break;
    //            }
    //        }

    //        for (int jndex2 = insertionJndex; jndex2 < codeInstructions.Count; ++jndex2)
    //        {
    //            if (codeInstructions[jndex2].Calls(drawMeshNowOrLaterInfo))
    //            {
    //                foundInsertionPoint2 = true;

    //                endJndex = jndex2 + 1;
    //                codeInstructions[endJndex].labels.Add(jumpPoint);
    //                break;
    //            }
    //        }

    //        FieldInfo pawnRendererPawnFieldInfo = typeof(PawnRenderer).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
    //        instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldarg_0));
    //        instructionsToAdd.Add(new CodeInstruction(OpCodes.Ldfld, pawnRendererPawnFieldInfo));
    //        instructionsToAdd.Add(new CodeInstruction(OpCodes.Call, bigmaInfo));
    //        instructionsToAdd.Add(new CodeInstruction(OpCodes.Brfalse_S, jumpPoint));

    //        codeInstructions.InsertRange(insertionJndex, instructionsToAdd);

    //        if (!foundInsertionPoint || !foundInsertionPoint2)
    //            Log.Error($"Failed to find insertion point in {nameof(PawnRenderer_DrawPawnBody_AddToggleForTattoos)}.");


    //        return codeInstructions.AsEnumerable();
    //    }
    //}
}
