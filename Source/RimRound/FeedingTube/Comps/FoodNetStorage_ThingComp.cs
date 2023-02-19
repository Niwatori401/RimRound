using RimRound.FeedingTube.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.FeedingTube.Comps
{
    //Battery bar drawing happens here?
    public class FoodNetStorage_ThingComp : FoodTransmitter_ThingComp
    {
        public float Stored { get => this.storedFood; set => this.storedFood = value; }

        public float Capacity { get => this.Props.capacity; }

        public float Remaining 
        { 
            get 
            { 
                return Capacity - Stored; 
            } 
        }


        float _fullnessToNutritionRatio = 1;
        public float FullnessToNutritionRatio
        {
            get
            {
                return _fullnessToNutritionRatio;
            }
            set
            {
                if (value <= 0)
                    Log.Warning("Set ftnRatio to illegal value");

                _fullnessToNutritionRatio = value;
            }
        }


        public new FoodNetStorage_CompProperties Props => (FoodNetStorage_CompProperties)this.props;

        public override void ReceiveCompSignal(string signal)
        {
            base.ReceiveCompSignal(signal);
            if (signal == "breakdown") 
            {
                this.Stored = 0;
            }
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(base.CompInspectStringExtra());

            sb.Append("RR_FT_RemainingFoodStored".Translate() + ": ");
            sb.Append(Stored.ToString("F1") + "L" + " / " + Capacity.ToString("F1") + "L\n");
            sb.AppendLine("RR_NutritionDensity".Translate() + $": {(FoodNet.NutritionToFullnessRatio):F1}" );
            return sb.ToString().Trim();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }

            yield return new Command_Action
            {
                defaultLabel = "Purge Contents",
                action = delegate ()
                {
                    this.Stored = 0;
                    
                },
                defaultDesc = "If the mixture becomes too tainted, you can purge the contents of this container to start over.",
                icon = Widgets.GetIconFor(RimWorld.ThingDefOf.Filth_Water),
                Order = 401
            };

            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Fill",
                    action = delegate ()
                    {
                        this.Stored = this.Capacity;
                    }
                };


                yield return new Command_Action
                {
                    defaultLabel = "DEBUG: Empty",
                    action = delegate ()
                    {
                        this.Stored = 0;
                    }
                };

            }
            yield break;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<float>(ref storedFood, "FoodStored", 0f);
        }


        /// <param name="amount">A positive number that represents the amount of fluid to subtract</param>
        /// <returns>A positive number that returns amount successfully drawn</returns>
        public float Subtract(float amount) 
        {
            if (amount < 0)
            {
                Log.Warning("Tried to use Subtract() to subtract a negative amount of food. Use Add() instead.");
                return 0;
            }

            if (amount >= Stored) 
            {
                float amtStored = Stored;
                Stored = 0;
                return amtStored;
            }

            Stored -= amount;
            return amount;
        }

        /// <param name="volumeToAdd">A positive number that represents the amount of fluid to add</param>
        /// <returns>A positive number that returns amount successfully addded</returns>
        public float Add(float volumeToAdd, float ftnRatio) 
        {
            if (volumeToAdd < 0)
            {
                Log.Warning("Tried to use Add() to add a negative amount of food. Use Subtract() instead.");
                return 0;
            }

            if (volumeToAdd >= Remaining) 
            {
                float amtRemaining = Remaining;
                Stored = Capacity;
                return amtRemaining;
            }
            UpdateRatio(volumeToAdd, ftnRatio);
            Stored += volumeToAdd;
            return volumeToAdd;
        }

        private void UpdateRatio(float volume, float ftnRatio) 
        {
            if (Stored <= FeedingTubeUtility.MinRQ)
            {
                this.FullnessToNutritionRatio = ftnRatio;
                return;
            }

            this.FullnessToNutritionRatio = (Stored * FullnessToNutritionRatio + volume * ftnRatio) / (Stored + volume);
        }

        float storedFood;
    }
}
