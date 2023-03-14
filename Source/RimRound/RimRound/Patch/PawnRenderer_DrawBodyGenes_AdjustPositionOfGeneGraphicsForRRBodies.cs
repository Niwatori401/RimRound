using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;
using RimRound.Utilities;
using RimWorld;

using BodyAlignmentOffsets = System.Tuple<Verse.Pair<float, float>, Verse.Pair<float, float>, Verse.Pair<float, float>>;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(PawnRenderer))]
    [HarmonyPatch("DrawBodyGenes")]
    internal class PawnRenderer_DrawBodyGenes_AdjustPositionOfGeneGraphicsForRRBodies
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codeInstructions = new List<CodeInstruction>(instructions);
            List<CodeInstruction> newInstructions = new List<CodeInstruction>();

            bool attemptedPatch = false;

            MethodInfo drawOffsetAtMI = typeof(GeneGraphicData).GetMethod(nameof(GeneGraphicData.DrawOffsetAt), BindingFlags.Instance | BindingFlags.Public);
            MethodInfo replacementMI = typeof(PawnRenderer_DrawBodyGenes_AdjustPositionOfGeneGraphicsForRRBodies).GetMethod(nameof(ReplacementMethod), BindingFlags.Static | BindingFlags.NonPublic);

            if (drawOffsetAtMI is null)
            {
                Log.Error($"drawOffsetAtMI was null in {nameof(PawnRenderer_DrawBodyGenes_AdjustPositionOfGeneGraphicsForRRBodies.Transpiler)}");
                return codeInstructions;
            }

            for (int i = 0; i < codeInstructions.Count; ++i)
            {
                if (!(codeInstructions[i].operand is MethodInfo mi) || mi != drawOffsetAtMI)
                    continue;

                codeInstructions[i] = new CodeInstruction(OpCodes.Call, replacementMI);
                codeInstructions.Insert(i, new CodeInstruction(OpCodes.Ldarg_0)); // push pawn renderer instance onto stack

                attemptedPatch = true;
                break;
            }

            if (!attemptedPatch)
                Log.Error($"Failed to find suitable patch location in {nameof(PawnRenderer_DrawBodyGenes_AdjustPositionOfGeneGraphicsForRRBodies.Transpiler)}");


            return codeInstructions;
        }

        private static Vector3 ReplacementMethod(GeneGraphicData geneGraphic, Rot4 rot4, PawnRenderer pawnrenderer) 
        {
            Vector3 offset = geneGraphic.DrawOffsetAt(rot4);
            
            switch (rot4.AsInt) 
            {
                case 0: // North
                    offset += new Vector3(Values.debugPos, 0, Values.debugPos2);
                    break;
                case 1: // East
                    offset += new Vector3(Values.debugPos, 0, Values.debugPos2);
                    break;
                case 2: // South
                    offset += new Vector3(Values.debugPos, 0, Values.debugPos2);
                    break;
                case 3: // West
                    offset += new Vector3(-Values.debugPos, 0, Values.debugPos2);
                    break;
                default:
                    Log.Error("Ran default");
                    break;
            }

            return offset;
        }

        

        private Dictionary<BodyTypeDef, BodyAlignmentOffsets> bodyTypeToTailOffset = new Dictionary<BodyTypeDef, BodyAlignmentOffsets>()
        {
            { Defs.BodyTypeDefOf.F_005_Thick         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_006_Chonky        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_010_Chubby        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_020_Corpulent     , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_030_Fat           , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_040_Obese         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_050_MorbidlyObese , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_060_Lardy         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_070_Enormous      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_080_Gigantic      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_090_Titanic       , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_100_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_150_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_200_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_250_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_300_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_350_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_400_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_450_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_500_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_900_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_910_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_920_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_930_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_940_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_950_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_960_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_970_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_980_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_990_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_995_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },

            { Defs.BodyTypeDefOf.F_005a_Thick         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_006a_Chonky        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_010a_Chubby        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_020a_Corpulent     , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_030a_Fat           , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_040a_Obese         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_050a_MorbidlyObese , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_060a_Lardy         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_070a_Enormous      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_080a_Gigantic      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_090a_Titanic       , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_100a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_150a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_200a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_250a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_300a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_350a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_400a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_450a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_500a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_900a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_910a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_920a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_930a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_940a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_950a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_960a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_970a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_980a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_990a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_995a_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },

            { Defs.BodyTypeDefOf.M_005_Thick         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_006_Chonky        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_010_Chubby        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_020_Corpulent     , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_030_Fat           , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_040_Obese         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_050_MorbidlyObese , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_060_Lardy         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_070_Enormous      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_080_Gigantic      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_090_Titanic       , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_100_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_150_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_200_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_250_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_300_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_350_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_400_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_450_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_500_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_900_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_910_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_920_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_930_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_940_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_950_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_960_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_970_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_980_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_990_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_995_Gelatinous    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },


            { Defs.BodyTypeDefOf.F_005_Thick_090         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_006_Chonky_090        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_010_Chubby_090        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_020_Corpulent_090     , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_030_Fat_090           , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_040_Obese_090         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_050_MorbidlyObese_090 , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_060_Lardy_090         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_070_Enormous_090      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_080_Gigantic_090      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_090_Titanic_090       , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_100_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_150_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_200_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_250_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_300_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_350_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_400_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_450_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_500_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_900_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_910_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_920_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_930_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_940_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_950_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_960_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_970_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_980_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_990_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_995_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },

            { Defs.BodyTypeDefOf.F_010a_Chubby_090        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_020a_Corpulent_090     , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_030a_Fat_090           , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_040a_Obese_090         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_050a_MorbidlyObese_090 , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_060a_Lardy_090         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_070a_Enormous_090      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_080a_Gigantic_090      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_090a_Titanic_090       , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_100a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_150a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_200a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_250a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_300a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_350a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_400a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_450a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_500a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_900a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_910a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_920a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_930a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_940a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_950a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_960a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_970a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_980a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_990a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.F_995a_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },

            { Defs.BodyTypeDefOf.M_005_Thick_090         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_006_Chonky_090        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_010_Chubby_090        , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_020_Corpulent_090     , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_030_Fat_090           , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_040_Obese_090         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_050_MorbidlyObese_090 , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_060_Lardy_090         , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_070_Enormous_090      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_080_Gigantic_090      , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_090_Titanic_090       , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_100_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_150_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_200_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_250_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_300_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_350_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_400_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_450_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_500_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_900_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_910_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_920_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_930_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_940_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_950_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_960_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_970_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_980_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_990_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
            { Defs.BodyTypeDefOf.M_995_Gelatinous_090    , new BodyAlignmentOffsets(new Pair<float, float>(0, 0), new Pair<float, float>(0, 0), new Pair<float, float>(0, 0)) },
        };
    }
}
