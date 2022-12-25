using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    public static class CaravanPatchUtility
    {

        public static void SetDietModeToDisabled(Pawn p) 
        {
            if (p.TryGetComp<Comps.FullnessAndDietStats_ThingComp>() is Comps.FullnessAndDietStats_ThingComp comp)
            {
                comp.preCaravanDietMode = comp.DietMode;
                comp.DietMode = Comps.DietMode.Disabled;
            }
        }
        public static void RestoreDietMode(Pawn p)
        {
            if (p.TryGetComp<Comps.FullnessAndDietStats_ThingComp>() is Comps.FullnessAndDietStats_ThingComp comp)
            {
                comp.DietMode = comp.preCaravanDietMode;
            }
        }

        public static Dictionary<int, List<Pawn>> activeCaravans = new Dictionary<int, List<Pawn>>();

        static FieldInfo uniqueIDFI = typeof(Caravan).GetField("uniqueId", BindingFlags.NonPublic | BindingFlags.Instance);

        public static int GetUniqueID(Caravan caravan) 
        {
            return (int)uniqueIDFI.GetValue(caravan);
        }
    }
}
