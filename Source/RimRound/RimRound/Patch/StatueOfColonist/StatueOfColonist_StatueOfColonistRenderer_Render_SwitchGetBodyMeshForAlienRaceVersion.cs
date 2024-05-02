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
using AlienRace;
using Verse;
using RimWorld;

namespace RimRound.Patch
{
 //   public class StatueOfColonist_StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion
 //   {
	//	static MethodInfo getBodyMeshMI = ModCompatibilityUtility.GetMethodInfo("Statue of Colonist", "StatueOfColonistRenderer", "GetBodyMesh");
	//	static MethodInfo newGetBodyMeshMI = typeof(StatueOfColonist_StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion).GetMethod(nameof(StatueOfColonist_StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion.NewGetBodyMesh), BindingFlags.Static | BindingFlags.NonPublic);

	//	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
	//	{
	//		List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
 //           bool foundInsertionPoint = false;

 //           for (int jndex = 0; jndex < codeInstructions.Count; jndex++)
 //           {
	//			if (codeInstructions[jndex].Calls(getBodyMeshMI))
	//			{
 //                   foundInsertionPoint = true;

 //                   codeInstructions[jndex].operand = newGetBodyMeshMI;
	//				codeInstructions[jndex].opcode = OpCodes.Call;
	//				codeInstructions.Insert(jndex, new CodeInstruction(OpCodes.Ldarg, 13));
	//			}
					
 //           }

 //           if (!foundInsertionPoint)
 //               Log.Error($"Failed to find insertion point in {nameof(StatueOfColonist_StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion)}.");


 //           return codeInstructions;
	//	}

	//	public static PatchCollection GetPatchCollection()
	//	{
	//		return new PatchCollection
	//		{
	//			transpiler = typeof(StatueOfColonist_StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion).GetMethod(
	//				nameof(StatueOfColonist_StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion.Transpiler), ModCompatibilityUtility.majorFlags)
	//		};
	//	}

	//	// The first parameter takes the instance off of the stack
	//	static Mesh NewGetBodyMesh(dynamic instanceDoNotUse, bool portrait, ThingDef raceDef, Rot4 bodyFacing, LifeStageDef lifeStageDef, BodyTypeDef bodyTypeDef) 
	//	{
	//		if (BodyTypeUtility.IsRRBody(bodyTypeDef))
	//		{
	//			return MeshPool.GetMeshSetForWidth(GetMeshSize(bodyTypeDef) * 1.5f).MeshAt(bodyFacing);
 //           }
	//		else 
	//		{
 //               if (lifeStageDef.bodyWidth != null)
 //               {
 //                   return MeshPool.GetMeshSetForWidth(lifeStageDef.bodyWidth.Value).MeshAt(bodyFacing);
 //               }
 //               return MeshPool.humanlikeBodySet.MeshAt(bodyFacing);
 //           }
	//	}

	//	static float GetMeshSize(BodyTypeDef bodyTypeDef) 
	//	{
	//		if (RacialBodyTypeInfoUtility.defaultFemaleSet.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.defaultFemaleSet[bodyTypeDef].meshSize;

 //           if (RacialBodyTypeInfoUtility.defaultMaleSet.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.defaultMaleSet[bodyTypeDef].meshSize;

	//		if (RacialBodyTypeInfoUtility.appleFemaleSet.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.appleFemaleSet[bodyTypeDef].meshSize;

	//		if (RacialBodyTypeInfoUtility.set070Male.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.set070Male[bodyTypeDef].meshSize;

	//		if (RacialBodyTypeInfoUtility.set070Female.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.set070Female[bodyTypeDef].meshSize;

	//		if (RacialBodyTypeInfoUtility.set090Female.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.set090Female[bodyTypeDef].meshSize;

	//		if (RacialBodyTypeInfoUtility.set090FemaleApple.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.set090FemaleApple[bodyTypeDef].meshSize;

	//		if (RacialBodyTypeInfoUtility.set090FemaleAppleNoFemaleSprite.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.set090FemaleAppleNoFemaleSprite[bodyTypeDef].meshSize;

	//		if (RacialBodyTypeInfoUtility.set090FemaleNoFemaleSprite.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.set090FemaleNoFemaleSprite[bodyTypeDef].meshSize;

	//		if (RacialBodyTypeInfoUtility.set090Male.ContainsKey(bodyTypeDef))
	//			return RacialBodyTypeInfoUtility.set090Male[bodyTypeDef].meshSize;

	//		return 1f;
	//	}
	//}
}
