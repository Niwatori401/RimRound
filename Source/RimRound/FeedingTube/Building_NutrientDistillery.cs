using RimRound.Comps;
using RimRound.FeedingTube.Comps;
using RimRound.FeedingTube.Utilities;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimRound.FeedingTube
{
    public class Building_NutrientDistillery : Building
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            this.trader = base.GetComp<FoodNetTrader_ThingComp>();
        }

        public override void Tick()
        {
            base.Tick();
            ProcessFood();
        }

        public override string GetInspectString()
        {
            string description = base.GetInspectString();

            description += $"\nTarget nutrition density: {1/targetRatio}";

            return description;
        }

        public override IEnumerable<Gizmo> GetGizmos() 
        {
            foreach (Gizmo g in base.GetGizmos())
                yield return g;

            yield return new Command_Action
            {
                defaultLabel = $"Increase target nutrition density",
                icon = RimRound.Utilities.Resources.PLUS_TEXTURE,
                action = delegate ()
                {
                    if (targetRatio <= minAllowedRatio)
                    {
                        MoteMaker.ThrowText(this.Position.ToVector3(), this.Map, $"maximum density");
                        return;
                    }

                    float increaseAmount = 0.1f;

                    RimRound.Utilities.Resources.gizmoClick.PlayOneShotOnCamera(null);
                    float cachedRatio = targetRatio;

                    float amountToAdd = (1 / ((1 / targetRatio) + increaseAmount) - targetRatio);

                    targetRatio = Mathf.Clamp(targetRatio + amountToAdd, minAllowedRatio, maxAllowedRatio);

                    MoteMaker.ThrowText(this.Position.ToVector3(), this.Map, $"+ {1/targetRatio - 1/cachedRatio:f1}");
                }
            };

            yield return new Command_Action
            {
                defaultLabel = $"Decrease target nutrition density",
                icon = RimRound.Utilities.Resources.MINUS_TEXTURE,
                action = delegate ()
                {
                    if (targetRatio >= maxAllowedRatio)
                    {
                        MoteMaker.ThrowText(this.Position.ToVector3(), this.Map, $"minimum density");
                        return;
                    }

                    float increaseAmount = 0.1f;

                    RimRound.Utilities.Resources.gizmoClick.PlayOneShotOnCamera(null);
                    float cachedRatio = targetRatio;

                    float amountToAdd = (1 / ((1 / targetRatio) + increaseAmount) - targetRatio);
                    targetRatio = Mathf.Clamp(targetRatio - amountToAdd, minAllowedRatio, maxAllowedRatio);

                    MoteMaker.ThrowText(this.Position.ToVector3(), this.Map, $"+ {1/targetRatio - 1/cachedRatio:f1}");
                }
            };
        }

        private bool CanProcessNow 
        {
            get
            {
                Pair<FoodNet, FoodNet> foodNets = GetFoodNets();
                bool foodNetsAreNull = foodNets.First == null || foodNets.Second == null;

                if (foodNetsAreNull)
                    return false;

                FoodNet inputNet = foodNets.First;
                FoodNet outputNet = foodNets.Second;

                bool nutritionDensityOnOutputNotMaxed = outputNet.FullnessToNutritionRatio - maxAllowedRatio <= 0 + FeedingTubeUtility.MinRQ;
                bool nutritionDensityOnOutputNotBelowMinimum = outputNet.FullnessToNutritionRatio - minAllowedRatio >= 0 - FeedingTubeUtility.MinRQ || outputNet.Stored <= FeedingTubeUtility.MinRQ;

                bool isEnoughFoodToProcess = inputNet.Stored * (inputNet.NutritionToFullnessRatio) >= amountOfNutritionToProcessPerOperation;
                
                bool isRoomForOutput = _IsRoomForOutput(inputNet, outputNet);


                return trader.IsOn &&
                trader.CanBeOn &&
                trader.TransmitsFoodNow &&
                nutritionDensityOnOutputNotMaxed &&
                nutritionDensityOnOutputNotBelowMinimum &&
                isEnoughFoodToProcess &&
                isRoomForOutput;
            }
        }

        private bool _IsRoomForOutput(FoodNet inputNet, FoodNet outputNet)
        {
            float volumeFromSource = amountOfNutritionToProcessPerOperation * inputNet.FullnessToNutritionRatio;
            float volumeAtDestination = volumeFromSource *  (1 / targetRatio);

            bool isRoomForOutput;
            if (inputNet.id == outputNet.id)
            {
                // Source is less dense than target
                if (inputNet.FullnessToNutritionRatio > targetRatio)
                    isRoomForOutput = true;
                else
                    isRoomForOutput = (inputNet.StorageCapacity - inputNet.Stored - volumeFromSource) > volumeAtDestination;
            }
            else
            {
                isRoomForOutput = volumeAtDestination <= outputNet.StorageCapacity - outputNet.Stored;
            }

            return isRoomForOutput;
        }

        private Pair<FoodNet, FoodNet> GetFoodNets() 
        {
            IntVec3 inputLoc = base.Position + IntVec3.West + IntVec3.South;
            IntVec3 outputLoc = base.Position + 2 * IntVec3.East + IntVec3.South;
            return new Pair<FoodNet, FoodNet>(FoodTransmitter_NetManager.For(this.Map)?.FoodNetAt(inputLoc), FoodTransmitter_NetManager.For(this.Map)?.FoodNetAt(outputLoc));
        }

        private bool ShouldProcessFood 
        {
            get 
            {
                return CanProcessNow;
            }
        }

        private void ProcessFood() 
        {
            if (Utilities.FeedingTubeUtility.IsHashIntervalTick((int)processingFrequency) && ShouldProcessFood) 
            {
                Pair<FoodNet, FoodNet> foodNets = GetFoodNets();
                FoodNet inputNet = foodNets.First;
                FoodNet outputNet = foodNets.Second;

                float amountOfNutritionToAttemptExtract = amountOfNutritionToProcessPerOperation * inputNet.FullnessToNutritionRatio;
                float volumeOfFoodToBeProcessed = amountOfNutritionToAttemptExtract - inputNet.Drain(amountOfNutritionToAttemptExtract);

                float nutritionSuccessfullyExtractedFromInput = volumeOfFoodToBeProcessed * (inputNet.NutritionToFullnessRatio);
                float volumeToBeOutput = nutritionSuccessfullyExtractedFromInput * targetRatio;

                outputNet.Fill(volumeToBeOutput, targetRatio);
            }
        }


        FoodNetTrader_ThingComp trader;

        //amount of ticks between processing
        float processingFrequency = 60;

        float targetRatio = 1f;
        float minAllowedRatio = 0.1f;
        float maxAllowedRatio = 10f;
        float amountOfNutritionToProcessPerOperation = 0.1f;
    }
}
