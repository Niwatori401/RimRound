using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
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
	/// <summary>
	/// This patch prevents an exception from being thrown when a pawn tries to wear clothing that does not have a sprite. 
	/// </summary>
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter))]
    [HarmonyPatch(nameof(ApparelGraphicRecordGetter.TryGetGraphicApparel))]
    public class ApparelGraphicRecordGetter_TryGetGraphicApparel_UseTransparentImagesForBadTex
    {
		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) 
		{
			List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
			List<CodeInstruction> newInstructions = new List<CodeInstruction>();
			Label label = generator.DefineLabel();

			int startJndex = -1;

			MethodInfo graphicDatabase_Get_MethodInfo = typeof(GraphicDatabase).GetMethod(
				nameof(GraphicDatabase.Get),
				BindingFlags.Public | BindingFlags.Static,
				Type.DefaultBinder,
				new Type[]
				{
					typeof(string),
					typeof(Shader),
					typeof(Vector2),
					typeof(Color)
				},
				null).MakeGenericMethod(new Type[] { typeof(Graphic_Multi) });

			MethodInfo genText_NullOrEmpty_MethodInfo = typeof(GenText).GetMethod(
				nameof(GenText.NullOrEmpty),
				BindingFlags.Static | BindingFlags.Public);

			for (int jndex = 0; jndex < codeInstructions.Count; ++jndex) 
			{
				if (codeInstructions[jndex].Calls(genText_NullOrEmpty_MethodInfo)) 
				{
					startJndex = jndex + 1;
					break;
				}
			}

			for (int jndex = startJndex; jndex < codeInstructions.Count; ++jndex)
			{
				if (codeInstructions[jndex].opcode == OpCodes.Ret)
				{
					codeInstructions[jndex - 6].labels.Add(label);
					break;
				}
			}

			newInstructions.Add(new CodeInstruction(OpCodes.Brtrue_S, label));
			newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
			newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_1));
			newInstructions.Add(CodeInstruction.Call(
				typeof(ApparelGraphicRecordGetter_TryGetGraphicApparel_UseTransparentImagesForBadTex),
				nameof(AddGraphicPathToIsNullDictionary)));

			if (startJndex != -1)
			{
				codeInstructions.InsertRange(startJndex, newInstructions);
			}
			
			
			for (int jndex = 0; jndex < codeInstructions.Count; ++jndex) 
			{
				if (codeInstructions[jndex].Calls(graphicDatabase_Get_MethodInfo))
				{
					codeInstructions[jndex].operand =
						typeof(ApparelGraphicRecordGetter_TryGetGraphicApparel_UseTransparentImagesForBadTex).GetMethod(
							nameof(BugmaOmega),
							BindingFlags.Public | BindingFlags.Static);
					
					codeInstructions.InsertRange(jndex,
						new List<CodeInstruction>()
						{
							new CodeInstruction(OpCodes.Ldarg_0),
							new CodeInstruction(OpCodes.Ldarg_1)
						});
				}
			}
			
			return codeInstructions;
		}

		public static Graphic BugmaOmega(string str, Shader shader, Vector2 vector, Color color, Apparel apparel, BodyTypeDef bodyType) 
		{

			bool dictionaryContainsEntry = false;
			if (!graphicPathResultIsNull.ContainsKey(str)) 
			{
				dictionaryContainsEntry = true;
			}
			if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || PawnRenderer.RenderAsPack(apparel) || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath)
			{
				str = apparel.WornGraphicPath;
			} 
			else if (GlobalSettings.preferDefaultOutfitOverNaked && (dictionaryContainsEntry || graphicPathResultIsNull[str]))//) 
			{
				str = Values.defaultClothingSetGraphicPath + "_" + bodyType.defName;
			}

			return GraphicDatabase.Get<Graphic_Multi>(str, shader, vector, color);
		}

		public static bool AddGraphicPathToIsNullDictionary(Apparel apparel, BodyTypeDef bodyType)
		{
			string path = apparel.WornGraphicPath + "_" + bodyType.defName;

			if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || PawnRenderer.RenderAsPack(apparel) || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath)
			{
				path = apparel.WornGraphicPath;
			}


			bool dustSettled = false;
			while (!dustSettled) 
			{
				if (!graphicPathResultIsNull.ContainsKey(path))
				{
					bool result =
						ContentFinder<Texture2D>.Get(path + "_north", false).NullOrBad() &&
						ContentFinder<Texture2D>.Get(path + "_east", false).NullOrBad() &&
						ContentFinder<Texture2D>.Get(path + "_south", false).NullOrBad() &&
						ContentFinder<Texture2D>.Get(path + "_west", false).NullOrBad();

					graphicPathResultIsNull.Add(path, result);
				}
				else
				{
					if (GlobalSettings.preferDefaultOutfitOverNaked && graphicPathResultIsNull[path])
					{
						path = Values.defaultClothingSetGraphicPath + "_" + bodyType.defName;
					}

					dustSettled = graphicPathResultIsNull.ContainsKey(path) ? true : false;
				}
			}
			return graphicPathResultIsNull[path];
		}
		//False means the graphic IS valid. 
		static Dictionary<string, bool> graphicPathResultIsNull = new Dictionary<string, bool>();
	}
}
