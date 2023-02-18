using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    public class CompProperties_ConvertWeightOpinion : CompProperties_AbilityEffect
    {
        public CompProperties_ConvertWeightOpinion()
        {
            this.compClass = typeof(CompAbilityEffect_ConvertWeightOpinion);
        }

        [MustTranslate]
        public string majorIncrease;

        [MustTranslate]
        public string majorDecrease;


        [MustTranslate]
        public string minorIncrease;

        [MustTranslate]
        public string minorDecrease;

        public float convertPowerFactor = -1f;
    }
}
