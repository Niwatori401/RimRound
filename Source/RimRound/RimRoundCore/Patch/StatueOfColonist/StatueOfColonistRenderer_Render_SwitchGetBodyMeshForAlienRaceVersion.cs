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
    internal class StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion
    {
		static MethodInfo getBodyMeshMI = ModCompatibilityUtility.GetMethodInfo("Statue of Colonist", "StatueOfColonistRenderer", "GetBodyMesh");
		static MethodInfo newGetBodyMeshMI = typeof(StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion).GetMethod(nameof(StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion.NewGetBodyMesh), BindingFlags.Static | BindingFlags.NonPublic);

		//ModCompatibilityUtility.GetMethodInfo("Statue of Colonist", "StatueOfColonistRenderer", "Render");
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
		{
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);

            for (int jndex = 0; jndex < codeInstructions.Count; jndex++)
            {
				if (codeInstructions[jndex].Calls(getBodyMeshMI))
				{
					codeInstructions[jndex].operand = newGetBodyMeshMI;
					codeInstructions[jndex].opcode = OpCodes.Call;
					codeInstructions.Insert(jndex, new CodeInstruction(OpCodes.Ldarg, 13));
					codeInstructions.Insert(jndex - 3, new CodeInstruction(OpCodes.Pop));
				}
					
            }

			return codeInstructions;
		}

		public static PatchCollection GetPatchCollection()
		{
			return new PatchCollection
			{
				transpiler = typeof(StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion).GetMethod(
					nameof(StatueOfColonistRenderer_Render_SwitchGetBodyMeshForAlienRaceVersion.Transpiler), ModCompatibilityUtility.majorFlags)
			};
		}

		static Mesh NewGetBodyMesh(bool portrait, ThingDef raceDef, Rot4 bodyFacing, BodyTypeDef bodyTypeDef) 
		{
			FieldInfo meshPool = typeof(AlienPartGenerator).GetField("meshPools", BindingFlags.NonPublic | BindingFlags.Static);
			Dictionary<Vector2, AlienPartGenerator.AlienGraphicMeshSet> meshPoolDict = (Dictionary<Vector2, AlienPartGenerator.AlienGraphicMeshSet>)meshPool.GetValue(null);

			if (meshPoolDict is null)
				Log.Error("meshPoolDict was null!");

			float meshSize = GetMeshSize(bodyTypeDef);

			return meshPoolDict[new Vector2(meshSize, meshSize)].bodySet.MeshAt(bodyFacing);
		}

		static float GetMeshSize(BodyTypeDef bodyTypeDef) 
		{
			if (RacialBodyTypeInfoUtility.defaultFemaleSet.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.defaultFemaleSet[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.defaultMaleSet.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.defaultMaleSet[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.appleFemaleSet.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.appleFemaleSet[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.set070Male.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.set070Male[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.set070Female.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.set070Female[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.set090Female.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.set090Female[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.set090FemaleApple.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.set090FemaleApple[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.set090FemaleAppleNoFemaleSprite.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.set090FemaleAppleNoFemaleSprite[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.set090FemaleNoFemaleSprite.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.set090FemaleNoFemaleSprite[bodyTypeDef].meshSize;

			if (RacialBodyTypeInfoUtility.set090Male.ContainsKey(bodyTypeDef))
				return RacialBodyTypeInfoUtility.set090Male[bodyTypeDef].meshSize;

			return 1f;
		}
	}
}
