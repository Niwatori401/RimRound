using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimRound.Comps;
using RimRound.Utilities;
using RimWorld;
using Verse;

namespace SwellGlow
{
    public class SwellGlow : Mod
    {
        public SwellGlow(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("Galactase.SwellGlow");
            harmony.PatchAll();
        }
    }
    public class SGTraitStatExtension : DefModExtension
    {
        public float weightGainMultiplierMultBonus;
        public float weightLossMultiplierMultBonus;
        public float digestionSpeedMultBonus;
        public float stomachElasticityMultBonus;
        public float fullnessGainedMultiplierBonus;
        public float eatingSpeedReductionMitigationMultBonus_Fullness;
    }

    [HarmonyPatch(typeof(TraitSet))]
    public class SGTraits
    {

        [HarmonyPostfix]
        [HarmonyPatch(nameof(TraitSet.GainTrait))]
        public static void GainTrait_Postfix(TraitSet __instance, Trait trait)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            SGTraitStatExtension geneStats = trait.def.GetModExtension<SGTraitStatExtension>();
            if (geneStats != null)
            {
                StatChangeUtility.ChangeRimRoundStats(pawn, new RimRoundStatBonuses()
                {
                    weightGainMultBonus = geneStats.weightGainMultiplierMultBonus,
                    weightLossMultBonus = geneStats.weightLossMultiplierMultBonus,
                    digestionRateMultiplier = geneStats.digestionSpeedMultBonus,
                    stomachElasticityMultiplier = geneStats.stomachElasticityMultBonus,
                    fullnessGainedMultBonus = geneStats.fullnessGainedMultiplierBonus,
                    eatingSpeedReductionMitigationMultBonus_Fullness = geneStats.eatingSpeedReductionMitigationMultBonus_Fullness
                });
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(nameof(TraitSet.RemoveTrait))]
        public static void RemoveTrait_Postfix(TraitSet __instance, Trait trait)
        {
            Pawn pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();

            SGTraitStatExtension geneStats = trait.def.GetModExtension<SGTraitStatExtension>();
            if (geneStats != null)
            {
                StatChangeUtility.ChangeRimRoundStats(pawn, new RimRoundStatBonuses()
                {
                    weightLossMultBonus = geneStats.weightLossMultiplierMultBonus * -1,
                    weightGainMultBonus = geneStats.weightGainMultiplierMultBonus * -1,
                    digestionRateMultiplier = geneStats.digestionSpeedMultBonus * -1,
                    stomachElasticityMultiplier = geneStats.stomachElasticityMultBonus * -1,
                    fullnessGainedMultBonus = geneStats.fullnessGainedMultiplierBonus * -1,
                    eatingSpeedReductionMitigationMultBonus_Fullness = geneStats.eatingSpeedReductionMitigationMultBonus_Fullness * -1
                });
            }
        }
    }
}
