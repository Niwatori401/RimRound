using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    public class ThingComp_PawnAttitude : ThingComp
    {
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();

            Scribe_Values.Look<WeightOpinion>(ref weightOpinion, "weightOpinion");
            Scribe_Values.Look<float?>(ref gainingResistance, "gainingResistance");
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "Increase Weight Opinion",
                    action = delegate ()
                    {
                        IncreaseOpinion();
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "Decrease Weight Opinion",
                    action = delegate ()
                    {
                        LowerOpinion();
                    }
                };

            }
        }

        public WeightOpinion IncreaseOpinion() 
        {
            List<WeightOpinion> weightOpinions = new List<WeightOpinion>();

            foreach (WeightOpinion x in Enum.GetValues(typeof(WeightOpinion)))
            {
                weightOpinions.Add(x);
            }

            int currentIndex = -1;

            for (int i = 0; i < weightOpinions.Count; ++i)
            {
                if (weightOpinions[i] == this.weightOpinion)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex == -1)
                Log.Warning("Index not found in ThingComp_PawnAttitude!");

            if (currentIndex != weightOpinions.Count - 1)
                this.weightOpinion = weightOpinions[currentIndex + 1];

            if (Prefs.DevMode)
                Log.Message($"Current opinion is now: {weightOpinion}");

            if (!((Pawn)parent).story.traits.HasTrait(WeightOpinionUtility.GetTraitByWeightOpinion(this.weightOpinion)))
            {
                WeightOpinionUtility.RemoveWeightOpinionTraits((Pawn)parent);
                ((Pawn)parent).story.traits.GainTrait(new Trait(WeightOpinionUtility.GetTraitByWeightOpinion(this.weightOpinion)));
            }


            return weightOpinion;
        }

        public WeightOpinion LowerOpinion() 
        {
            List<WeightOpinion> weightOpinions = new List<WeightOpinion>();

            foreach (WeightOpinion x in Enum.GetValues(typeof(WeightOpinion)))
            {
                weightOpinions.Add(x);
            }

            int currentIndex = -1;

            for (int i = 0; i < weightOpinions.Count; ++i)
            {
                if (weightOpinions[i] == this.weightOpinion)
                {
                    currentIndex = i;
                    break;
                }
            }

            if (currentIndex == -1)
                Log.Warning("Index not found in ThingComp_PawnAttitude!");

            if (currentIndex != 0) // weightOpinions.Last() will cause it to wrap around.
                this.weightOpinion = weightOpinions[currentIndex - 1];

            if (Prefs.DevMode)
                Log.Message($"Current opinion is now: {weightOpinion}");

            if (!((Pawn)parent).story.traits.HasTrait(WeightOpinionUtility.GetTraitByWeightOpinion(this.weightOpinion)))
            {
                WeightOpinionUtility.RemoveWeightOpinionTraits((Pawn)parent);
                ((Pawn)parent).story.traits.GainTrait(new Trait(WeightOpinionUtility.GetTraitByWeightOpinion(this.weightOpinion)));
            }

            return weightOpinion;
        }

        public WeightOpinion weightOpinion = WeightOpinion.None;

        //----------------

        private float? gainingResistance = null;

        public float GainingResistance 
        {
            get 
            {
                if (gainingResistance is null)
                {
                    gainingResistance = GetAlteredResistanceBasedOnWeightOpinion(this.parent.AsPawn()?.guest?.resistance ?? 999);
                }

                return gainingResistance.Value;
            }
            set 
            {
                gainingResistance = value;
            }
        }

        private float GetAlteredResistanceBasedOnWeightOpinion(float gainingResistance) 
        {
            switch (weightOpinion) 
            {
                case WeightOpinion.None:
                    return gainingResistance;
                case WeightOpinion.Hate:
                    return gainingResistance * 3.5f;
                case WeightOpinion.Dislike:
                    return gainingResistance * 2.25f;
                case WeightOpinion.NeutralMinus:
                    return gainingResistance * 1.5f;
                case WeightOpinion.Neutral:
                    return gainingResistance * 1.1f;
                case WeightOpinion.NeutralPlus:
                    return gainingResistance * 0.7f;
                case WeightOpinion.Like:
                    return gainingResistance * 0.4f;
                case WeightOpinion.Love:
                    return gainingResistance * 0.1f;
                case WeightOpinion.Fanatical:
                    return gainingResistance * 0.0f;
                default:
                    return gainingResistance;
            }
        }
        
    }
}
