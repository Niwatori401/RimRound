using RimRound.Comps;
using RimRound.FeedingTube;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;
using static RimRound.Utilities.Perks;

namespace RimRound.Utilities
{
    public static class SoundUtility
    {
        public static bool PawnShouldPlaySound(Pawn pawn) 
        {
            return pawn.RaceProps.Humanlike &&
                pawn.TryGetComp<PawnBodyType_ThingComp>() is PawnBodyType_ThingComp comp &&
                comp != null &&
                !comp.CategoricallyExempt &&
                !comp.PersonallyExempt;
        }


        public static SoundDef GetSwallowSoundByWeightOpinionAndGender(Pawn pawn, AutoFeederMode mode) 
        {
            if (pawn == null || !PawnShouldPlaySound(pawn)) 
            {
                return null;
            }

            switch (mode)
            {
                case AutoFeederMode.off:
                    break;
                case AutoFeederMode.lose:
                    break;
                case AutoFeederMode.maintain:
                    return RimRound.Defs.SoundDefOf.RR_FeedingMachine_Swallow_Easy;
                case AutoFeederMode.gain:
                    return RimRound.Defs.SoundDefOf.RR_FeedingMachine_Swallow_Normal;
                case AutoFeederMode.maxgain:
                    if (pawn.gender == Gender.Male) 
                    {
                        if (WeightOpinionUtility.IsPawnBelowWeightOpinion(pawn, WeightOpinion.Fanatical)) 
                        {
                            return RimRound.Defs.SoundDefOf.RR_FeedingMachine_Swallow_Labored_Male;
                        }

                        return RimRound.Defs.SoundDefOf.RR_FeedingMachine_Swallow_Labored_Male_Pleasure;
                    }
                    else
                    {
                        if (WeightOpinionUtility.IsPawnBelowWeightOpinion(pawn, WeightOpinion.Fanatical))
                        {
                            return RimRound.Defs.SoundDefOf.RR_FeedingMachine_Swallow_Labored_Female;
                        }

                        return RimRound.Defs.SoundDefOf.RR_FeedingMachine_Swallow_Labored_Female_Pleasure;
                    }
                default:
                    return null;
            }

            return null;
        }


        public static SoundDef GetBreathingSoundByWeightOpinionAndGender(Pawn pawn) 
        {
            if (pawn == null || !PawnShouldPlaySound(pawn))
            {
                return null;
            }

            bool isMale = pawn.gender == Gender.Male;

            if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_090bs_Titanic))
            {
                if (WeightOpinionUtility.IsPawnBelowWeightOpinion(pawn, WeightOpinion.Fanatical))
                {
                    return isMale ? Defs.SoundDefOf.RR_M_BreathSuperHeavy : Defs.SoundDefOf.RR_F_BreathSuperHeavy;
                }
                else
                {
                    return isMale ? Defs.SoundDefOf.RR_M_PleasureBreathSuperHeavy : Defs.SoundDefOf.RR_F_PleasureBreathSuperHeavy;
                }
            }
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_060bs_Lardy))
            {
                if (WeightOpinionUtility.IsPawnBelowWeightOpinion(pawn, WeightOpinion.Love))
                {
                    return isMale ? Defs.SoundDefOf.RR_M_BreathHeavy : Defs.SoundDefOf.RR_F_BreathHeavy;
                }
                else
                {
                    return isMale ? Defs.SoundDefOf.RR_M_PleasureBreathHeavy : Defs.SoundDefOf.RR_F_PleasureBreathHeavy;
                }
            }
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_020bs_Corpulent))
            {
                if (WeightOpinionUtility.IsPawnBelowWeightOpinion(pawn, WeightOpinion.Like))
                {
                    return isMale ? Defs.SoundDefOf.RR_M_BreathMedium : Defs.SoundDefOf.RR_F_BreathMedium;
                }
                else
                {
                    return isMale ? Defs.SoundDefOf.RR_M_PleasureBreathMedium : Defs.SoundDefOf.RR_F_PleasureBreathMedium;
                }
            }

            else if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_020bs_Corpulent)) 
            {
                if (WeightOpinionUtility.IsPawnBelowWeightOpinion(pawn, WeightOpinion.NeutralPlus))
                {
                    return isMale ? Defs.SoundDefOf.RR_M_BreathLight : Defs.SoundDefOf.RR_F_BreathLight;
                }
                else
                {
                    return isMale ? Defs.SoundDefOf.RR_M_PleasureBreathLight : Defs.SoundDefOf.RR_F_PleasureBreathLight;
                }
            }

            return null;
        }


        public static SoundDef GetStomachStretchingSoundByFullness(Pawn pawn)
        {
            var fndComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            return GetStomachStretchingSoundByFullness(fndComp);
        }

        public static SoundDef GetStomachStretchingSoundByFullness(FullnessAndDietStats_ThingComp fndComp)
        {
            if (fndComp == null || 
                fndComp.parent.AsPawn() == null || 
                !PawnShouldPlaySound(fndComp.parent.AsPawn()) || 
                fndComp.fullnessbar.CurrentFullnessAsPercentOfSoftLimit < 1.0f)
            {
                return null;
            }

            if (fndComp.CurrentFullness >= 6)
            {
                return Defs.SoundDefOf.RR_Stretch_5;
            }
            else if (fndComp.CurrentFullness >= 5) { 
                return Defs.SoundDefOf.RR_Stretch_4;
            }
            else if (fndComp.CurrentFullness >= 4)
            {
                return Defs.SoundDefOf.RR_Stretch_3;
            }
            else if (fndComp.CurrentFullness >= 3)
            {
                return Defs.SoundDefOf.RR_Stretch_2;
            }
            else if (fndComp.CurrentFullness >= 2)
            {
                return Defs.SoundDefOf.RR_Stretch_1;
            }

            return null;
        }

        public static SoundDef GetStomachGurgleSoundsByWeight(Pawn pawn) // Digesting
        { 
            var fndComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            return GetStomachGurgleSoundsByWeight(fndComp);
        }

        public static SoundDef GetStomachGurgleSoundsByWeight(FullnessAndDietStats_ThingComp fndComp) // Digesting
        {
            if (fndComp == null ||
                fndComp.parent.AsPawn() == null ||
                !PawnShouldPlaySound(fndComp.parent.AsPawn()) ||
                fndComp.CurrentFullness <= 0.05f) // Arbitrarily low number above zero
            {
                return null;
            }

            if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_090bs_Titanic))
                return Defs.SoundDefOf.RR_StomachGurgles_SuperHeavy;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_060bs_Lardy))
                return Defs.SoundDefOf.RR_StomachGurgles_Heavy;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_020bs_Corpulent))
                return Defs.SoundDefOf.RR_StomachGurgles_Medium;

            return Defs.SoundDefOf.RR_StomachGurgles_Light;
        }


        public static SoundDef GetBurpSoundsByWeight(Pawn pawn) // Digesting
        {
            var fndComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            return GetBurpSoundsByWeight(fndComp);
        }

        public static SoundDef GetBurpSoundsByWeight(FullnessAndDietStats_ThingComp fndComp) // Digesting
        {
            if (fndComp == null ||
                fndComp.parent.AsPawn() == null ||
                !PawnShouldPlaySound(fndComp.parent.AsPawn()) ||
                fndComp.CurrentFullness <= 0.05f) // Arbitrarily low number above zero
            {
                return null;
            }

            if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_090bs_Titanic))
                return Defs.SoundDefOf.RR_StomachBurp_SuperHeavy;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_060bs_Lardy))
                return Defs.SoundDefOf.RR_StomachBurp_Heavy;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_020bs_Corpulent))
                return Defs.SoundDefOf.RR_StomachBurp_Medium;

            return Defs.SoundDefOf.RR_StomachBurp_Light;
        }


        // Call just before pawn changes body type. Does not check for if the sound should be playing, just which sound is appropriate for the current weight.
        public static SoundDef GetBwomfSoundByWeight(Pawn pawn)
        {
            if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_100bs_Gelatinous))
                return Defs.SoundDefOf.RR_Bwomf_6;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_080bs_Gigantic))
                return Defs.SoundDefOf.RR_Bwomf_5;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_060bs_Lardy))
                return Defs.SoundDefOf.RR_Bwomf_4;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_040bs_Obese))
                return Defs.SoundDefOf.RR_Bwomf_3;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(pawn, Defs.BodyTypeDefOf.F_006bs_Chonky))
                return Defs.SoundDefOf.RR_Bwomf_2;

            return Defs.SoundDefOf.RR_Bwomf_1;
        }


        public static SoundDef GetFootStepSoundsByWeightAndMovement(Pawn pawn)
        {
            var fndComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            return GetFootStepSoundsByWeightAndMovement(fndComp);
        }


        private static Dictionary<String, bool> playFootStepSound1 = new Dictionary<String, bool>();
        private static Dictionary<String, int> lastPlayedFootstepTick = new Dictionary<String, int>();
        public static SoundDef GetFootStepSoundsByWeightAndMovement(FullnessAndDietStats_ThingComp fndComp)
        {
            if (fndComp == null ||
                fndComp.parent.AsPawn() == null ||
                !PawnShouldPlaySound(fndComp.parent.AsPawn()))
            {
                return null;
            }

            BodyTypeDef minimumBodyTypeForBreathing = Defs.BodyTypeDefOf.F_020bs_Corpulent;

            if (fndComp.parent.AsPawn().Downed || 
                fndComp?.parent?.AsPawn()?.pather == null || 
                !fndComp.parent.AsPawn().pather.MovingNow || 
                !BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), minimumBodyTypeForBreathing))
                return null;

            string pawnID = fndComp.parent.ThingID;

            if (!playFootStepSound1.ContainsKey(pawnID)) { playFootStepSound1.Add(pawnID, true); }
            if (!lastPlayedFootstepTick.ContainsKey(pawnID)) { lastPlayedFootstepTick.Add(pawnID, -1); }

            const float TICKS_PER_SECOND = 60;
            float waitMultiplier = 1;
            float ticksBetweenStepsForThisPawn = fndComp.parent.AsPawn().TicksPerMoveCardinal * waitMultiplier;
            ticksBetweenStepsForThisPawn = Mathf.Clamp(ticksBetweenStepsForThisPawn, 1 * TICKS_PER_SECOND, 4 * TICKS_PER_SECOND);


            if (lastPlayedFootstepTick[pawnID] + ticksBetweenStepsForThisPawn > Find.TickManager.TicksAbs) 
                return null;

            bool playSound1 = playFootStepSound1[pawnID];

            SoundDef sd;
            if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_050bs_MorbidlyObese))
                sd = playSound1 ? Defs.SoundDefOf.RR_Footstep_Lardy_1 : Defs.SoundDefOf.RR_Footstep_Lardy_2;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_040bs_Obese))
                sd = playSound1 ? Defs.SoundDefOf.RR_Footstep_MorbidlyObese_1 : Defs.SoundDefOf.RR_Footstep_MorbidlyObese_2;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_030bs_Fat))
                sd = playSound1 ? Defs.SoundDefOf.RR_Footstep_Obese_1 : Defs.SoundDefOf.RR_Footstep_Obese_2;
            else
                sd = playSound1 ? Defs.SoundDefOf.RR_Footstep_Fat_1 : Defs.SoundDefOf.RR_Footstep_Fat_2;


            playFootStepSound1[pawnID] = !playSound1;
            lastPlayedFootstepTick[pawnID] = Find.TickManager.TicksAbs;

            return sd;
        }


        public static SoundDef GetStomachSloshByWeight(Pawn pawn)
        {
            var fndComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            return GetStomachSloshByWeight(fndComp);
        }


        // Returns sound based on weight, not whether the sound should play.
        public static SoundDef GetStomachSloshByWeight(FullnessAndDietStats_ThingComp fndComp) 
        {

            if (fndComp == null)
            {
                Log.Error($"fndComp was null in {nameof(GetStomachSloshByWeight)}");
                return null;
            }

            if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_090bs_Titanic))
                return Defs.SoundDefOf.RR_Slosh_SuperHeavy;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_060bs_Lardy))
                return Defs.SoundDefOf.RR_Slosh_Heavy;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_020bs_Corpulent))
                return Defs.SoundDefOf.RR_Slosh_Medium;

            return Defs.SoundDefOf.RR_Slosh_Light;
        }

        public static SoundDef GetEmptyStomachSoundsByWeight(Pawn pawn)
        {
            var fndComp = pawn.TryGetComp<FullnessAndDietStats_ThingComp>();
            return GetEmptyStomachSoundsByWeight(fndComp);
        }

        public static SoundDef GetEmptyStomachSoundsByWeight(FullnessAndDietStats_ThingComp fndComp)
        {
            if (fndComp == null ||
                fndComp.parent.AsPawn() == null ||
                !PawnShouldPlaySound(fndComp.parent.AsPawn()) ||
                fndComp.CurrentFullness > 0.05f)  // Arbitrarily low number above zero
            {
                return null;
            }


            if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_090bs_Titanic))
                return Defs.SoundDefOf.RR_StomachEmpty_SuperHeavy;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_060bs_Lardy))
                return Defs.SoundDefOf.RR_StomachEmpty_Heavy;
            else if (BodyTypeUtility.PawnIsOverWeightThreshold(fndComp.parent.AsPawn(), Defs.BodyTypeDefOf.F_020bs_Corpulent))
                return Defs.SoundDefOf.RR_StomachEmpty_Medium;

            return Defs.SoundDefOf.RR_StomachEmpty_Light;
        }


        // Pawn ThingID, then SoundDef name to get tick last played and delay in ticks until next. Delay is ticks after clip ends.
        private static Dictionary<String, Dictionary<String, float>> pawnSoundDefNextMinStartTick = new Dictionary<string, Dictionary<string, float>>();

        private static FieldInfo resolvedGrainsFI;
        
        public static void PlayOneShotForPawnIfNotWaiting(Pawn pawn, SoundDef sound, float secondsDelayBetweenPlays) 
        {
            // This often is the case if the sound is not supposed to play due to restrictions
            if (sound == null || !pawn.Spawned) 
            {
                return; 
            }


            if (resolvedGrainsFI == null) 
            { 
                resolvedGrainsFI = typeof(SubSoundDef).GetField("resolvedGrains", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            
            if (pawn == null) { Log.Error($"Pawn was null in {nameof(PlayOneShotForPawnIfNotWaiting)}!"); return; }

            string pawnID = pawn.ThingID;

            if (!pawnSoundDefNextMinStartTick.ContainsKey(pawnID))
            {
                pawnSoundDefNextMinStartTick.Add(pawnID, new Dictionary<String, float>());
            }
            
            if (!pawnSoundDefNextMinStartTick[pawnID].ContainsKey(sound.defName)) 
            {
                pawnSoundDefNextMinStartTick[pawnID].Add(sound.defName, 0);
            }

            float nextEligibleTickToPlay = pawnSoundDefNextMinStartTick[pawnID][sound.defName];

            if (Find.TickManager.TicksAbs >= nextEligibleTickToPlay)
            {
                const float TICKS_PER_SECOND = 60;
                
                float clipDurationSeconds = GetAverageGrainDuration(sound);

                sound.PlayOneShot(SoundInfo.InMap(new TargetInfo(pawn)));
                pawnSoundDefNextMinStartTick[pawnID][sound.defName] = Find.TickManager.TicksAbs + ((clipDurationSeconds + secondsDelayBetweenPlays) * TICKS_PER_SECOND);
            }
        }

        public static float GetAverageGrainDuration(SoundDef sound) 
        {
            var instance = sound.subSounds[0]; // Only chooses first subsound for this check. If using more than one subsound, make sure they are of similar lengths or the longest is first
            var resolvedGrains = (List<ResolvedGrain>)resolvedGrainsFI.GetValue(instance);

            if (resolvedGrains.Count > 1)
            {
                float maxDuration = 0;

                foreach (var resGrain in resolvedGrains)
                {
                    maxDuration = Math.Max(maxDuration, resGrain.duration);
                }

                return maxDuration;
            }
            else
            {
                return resolvedGrains[0].duration;
            }
        }
    }
}
