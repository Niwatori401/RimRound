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
            
            IfGraphicPathIsNullReturnNullApparelGraphicRecord(generator, codeInstructions);
            ReplaceVanillaGraphicDatabaseGetMethodWithMine(codeInstructions);

            return codeInstructions;
        }

        private static void IfGraphicPathIsNullReturnNullApparelGraphicRecord(ILGenerator generator, List<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();
            Label label = generator.DefineLabel();

            int startJndex = -1;

            for (int jndex = 0; jndex < codeInstructions.Count; ++jndex)
            {
                if (codeInstructions[jndex].Calls(genTextNullOrEmptyMI))
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
                nameof(IsGraphicPathResultNullForApparel)));

            if (startJndex != -1)
            {
                codeInstructions.InsertRange(startJndex, newInstructions);
            }
        }

        private static void ReplaceVanillaGraphicDatabaseGetMethodWithMine(List<CodeInstruction> codeInstructions)
        {
            for (int jndex = 1; jndex < codeInstructions.Count; jndex++)
            {
                int currentIndex = codeInstructions.Count - jndex;
                if (codeInstructions[currentIndex].opcode == OpCodes.Call) // Replace the final Call (which should be the GrahpicDatabase.Get)
                {
                    codeInstructions[currentIndex].operand =
                        typeof(ApparelGraphicRecordGetter_TryGetGraphicApparel_UseTransparentImagesForBadTex).GetMethod(
                            nameof(GetApparelGraphic),
                            BindingFlags.Public | BindingFlags.Static);

                    /*
                    codeInstructions.InsertRange(currentIndex,
                        new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Ldarg_1)
                        });
                    */
                    break;
                }
            }
        }

        public static Graphic GetApparelGraphic(string graphicPath, Shader shader, Vector2 vector, Color color, Apparel apparel, BodyTypeDef bodyType) 
		{
            if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || PawnRenderer.RenderAsPack(apparel) || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath || apparel.def.apparel.LastLayer.defName == "OnHead" || apparel.def.apparel.LastLayer.defName == "StrappedHead")
            {
                graphicPath = apparel.WornGraphicPath;
            } 
            else 
            {
                graphicPath = apparel.WornGraphicPath + "_" + BodyTypeUtility.ConvertBodyTypeDefDefnameAccordingToSettings(RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(bodyType).defName);
            }

            if (!graphicPathResultIsNull.ContainsKey(graphicPath))
            {
                Log.Error($"Graphic path was not in null dict and should have been! Graphic path: {graphicPath}");
            }
			else if (GlobalSettings.preferDefaultOutfitOverNaked && graphicPathResultIsNull[graphicPath]) 
			{
				graphicPath = Values.defaultClothingSetGraphicPath + "_" + BodyTypeUtility.ConvertBodyTypeDefDefnameAccordingToSettings(RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(bodyType).defName);
            }

            return GraphicDatabase.Get<Graphic_Multi>(graphicPath, shader, vector, color);
		}

		public static bool IsGraphicPathResultNullForApparel(Apparel apparel, BodyTypeDef bodyType)
        {
            string apparelGraphicPath = apparel.WornGraphicPath + "_" + BodyTypeUtility.ConvertBodyTypeDefDefnameAccordingToSettings(RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(bodyType).defName);

            apparelGraphicPath = AlterGraphicPathIfLastLayerIsOuter(apparel, apparelGraphicPath);
            AddGraphicPathToDictionaryIfNecessary(apparelGraphicPath);
            apparelGraphicPath = AlterApparelGraphicPathIfPathIsNullAndPreferDefaultOverNaked(bodyType, apparelGraphicPath);

            return graphicPathResultIsNull[apparelGraphicPath];
        }

        private static string AlterGraphicPathIfLastLayerIsOuter(Apparel apparel, string apparelGraphicPath)
        {
            if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover || PawnRenderer.RenderAsPack(apparel) || apparel.WornGraphicPath == BaseContent.PlaceholderImagePath || apparel.WornGraphicPath == BaseContent.PlaceholderGearImagePath || apparel.def.apparel.LastLayer.defName == "OnHead" || apparel.def.apparel.LastLayer.defName == "StrappedHead")
            {
                apparelGraphicPath = apparel.WornGraphicPath;
            }

            return apparelGraphicPath;
        }

        private static void AddGraphicPathToDictionaryIfNecessary(string apparelGraphicPath)
        {
            if (!graphicPathResultIsNull.ContainsKey(apparelGraphicPath))
            {
                bool result =
                    ContentFinder<Texture2D>.Get(apparelGraphicPath + "_north", false).NullOrBad() &&
                    ContentFinder<Texture2D>.Get(apparelGraphicPath + "_east", false).NullOrBad() &&
                    ContentFinder<Texture2D>.Get(apparelGraphicPath + "_south", false).NullOrBad() &&
                    ContentFinder<Texture2D>.Get(apparelGraphicPath + "_west", false).NullOrBad();

                graphicPathResultIsNull.Add(apparelGraphicPath, result);
            }
        }

        private static string AlterApparelGraphicPathIfPathIsNullAndPreferDefaultOverNaked(BodyTypeDef bodyType, string apparelGraphicPath)
        {
            if (GlobalSettings.preferDefaultOutfitOverNaked && graphicPathResultIsNull[apparelGraphicPath])
            {
                apparelGraphicPath = Values.defaultClothingSetGraphicPath + "_" + BodyTypeUtility.ConvertBodyTypeDefDefnameAccordingToSettings(RacialBodyTypeInfoUtility.GetEquivalentBodyTypeDef(bodyType).defName);
                AddGraphicPathToDictionaryIfNecessary(apparelGraphicPath);
            }

            return apparelGraphicPath;
        }

        //False means the graphic IS valid. 
        static Dictionary<string, bool> graphicPathResultIsNull = new Dictionary<string, bool>();


		static MethodInfo genTextNullOrEmptyMI = typeof(GenText).GetMethod(
			nameof(GenText.NullOrEmpty),
			BindingFlags.Static | BindingFlags.Public);
	}
}
