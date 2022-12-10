using AlienRace;
using RimRound.Utilities;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Comps
{
    public class WorldComp_SaveValues : WorldComponent
    {
        public WorldComp_SaveValues(World world) : base(world) 
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();
            SaveAllSettings();
            //SaveCaravanPatchUtility();

        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();

            //BodyTypeUtility.RefreshBodyTypeGraphicLocations();

            if (ModCompatibilityUtility.CheckModInstalled("Vanilla Expanded Framework"))
            { 
                Log.Message("[RimRound] Disabling texure caching...");


                FieldInfo disableCachingField = ModCompatibilityUtility.GetFieldInfo("Vanilla Expanded Framework", "VFEGlobalSettings", "disableCaching");
                FieldInfo vfeGlobalSettingsFI = ModCompatibilityUtility.GetFieldInfo("Vanilla Expanded Framework", "VFEGlobal", "settings");

                if (disableCachingField != null && vfeGlobalSettingsFI != null)
                {
                    disableCachingField.SetValue(vfeGlobalSettingsFI.GetValue(null), true);
                }
                else 
                {
                    Log.Error("[RimRound] Vanilla Expanded Framework was installed but RimRound failed to change settings!");
                }
            }
            else 
            {
                Log.Warning("[RimRound] Failed to find Vanilla Expanded Framework. Please make sure it is installed or sprites will not render correctly!");
            }
        }

        public void SaveCaravanPatchUtility() 
        {
            List<int> list1 = new List<int>();
            List<List<Pawn>> list2 = new List<List<Pawn>>();

            Scribe_Collections.Look(ref CaravanPatchUtility.activeCaravans, "activeCaravans", LookMode.Value, LookMode.Reference, ref list1, ref list2);

            if (CaravanPatchUtility.activeCaravans is null)
                CaravanPatchUtility.activeCaravans = new Dictionary<int, List<Pawn>>();
        }

        public void SaveAllSettings()
        {
            IEnumerable<FieldInfo> fieldInfos = typeof(GlobalSettings).GetRuntimeFields();

            foreach (FieldInfo f in fieldInfos)
            {
                if (f.FieldType == typeof(bool))
                {
                    ExposeField<bool>(f);
                }
                else if (f.FieldType == typeof(NumericFieldData<float>))
                {
                    ExposeNumericFieldData<float>(f);
                }
                else if (f.FieldType == typeof(NumericFieldData<int>))
                {
                    ExposeNumericFieldData<int>(f);
                }
            }
        }

        private static void ExposeNumericFieldData<T>(FieldInfo f)
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                T currentVal = (T)((NumericFieldData<T>)f.GetValue(null)).threshold;

                Scribe_Values.Look<T>(ref currentVal, f.Name, default, true);
                return;
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                NumericFieldData<T> dummyNumericFieldData = (NumericFieldData<T>)f.GetValue(null);
                T dummyVal = default;
                Scribe_Values.Look<T>(ref dummyVal, f.Name, dummyNumericFieldData.threshold);

                dummyNumericFieldData.threshold = dummyVal;
                dummyNumericFieldData.stringBuffer = null;

                f.SetValue(null, dummyNumericFieldData);
                return;
            }
        }

        private static void ExposeField<T>(FieldInfo f) where T : struct
        {
            if (typeof(T) == typeof(NumericFieldData<float>) || typeof(T) == typeof(NumericFieldData<int>))
            {
                Log.Error($"Cannot use {nameof(ExposeField)} with {nameof(T)}");
                return;
            }

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                T currentVal = (T)f.GetValue(null);
                Scribe_Values.Look<T>(ref currentVal, f.Name, default, true);
                return;
            }
            else if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                T dummyVal = default;
                Scribe_Values.Look<T>(ref dummyVal, f.Name, dummyVal);
                f.SetValue(null, dummyVal);
                return;
            }
        }

    }
}
