using HarmonyLib;
using RimRound.Utilities;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(CaravanMaker))]
    [HarmonyPatch(nameof(CaravanMaker.MakeCaravan))]
    public class CaravanMaker_MakeCaravan_ChangeDietMode
    {
        public static void Postfix(IEnumerable<Pawn> __0, Caravan __result)
        {
            int id = __result.ID; //CaravanPatchUtility.GetUniqueID(__result);

            if (CaravanPatchUtility.activeCaravans.ContainsKey(id))
            {
                CaravanPatchUtility.activeCaravans[id].AddRange(__0);
            }
            else
            {

                CaravanPatchUtility.activeCaravans.Add(id, new List<Pawn>(__0));
            }

            foreach (Pawn p in __0)
                CaravanPatchUtility.SetDietModeToDisabled(p);
        }

       

    }


    //[HarmonyPatch(typeof(Caravan))]
    //[HarmonyPatch(nameof(Caravan.AddPawn))]
    //public class Caravan_AddPawn_ChangeDietMode
    //{
    //    public static void Postfix(Pawn __0, int ___uniqueId) 
    //    {
    //        if (CaravanPatchUtility.activeCaravans.Find(x => x.First == ___uniqueId) is Pair<int, List<Pawn>> caravan && caravan.First != 0)
    //        {
    //            caravan.Second.Add(__0);
    //        }
    //        else 
    //        {
    //            CaravanPatchUtility.activeCaravans.Add(new Pair<int, List<Pawn>>(___uniqueId, new List<Pawn>() { __0 }));
    //        }

    //        CaravanPatchUtility.SetDietModeToDisabled(__0);
    //    }
    //}
}
