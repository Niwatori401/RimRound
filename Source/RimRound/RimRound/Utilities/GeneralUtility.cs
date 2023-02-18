using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;


using RimWorld;
using UnityEngine;
using HarmonyLib;
using AlienRace;
using RimRound.Comps;
using Verse.AI;
using System.Reflection;
using RimRound.Hediffs;
using Verse.Sound;

namespace RimRound.Utilities
{
    public static class GeneralUtility
    {
        public static Pawn AsPawn(this ThingWithComps t) 
        {
            return t as Pawn;
        }

        public static Pawn AsPawn(this object t)
        {
            return t as Pawn;
        }

        public static bool IsNotNull(object o) 
        {
            return !(o is null);
        }

        public static T AsNonNullable<T>(this ref T? nullableType) where T : struct
        {
            if (nullableType is null)
            { 
                throw new ArgumentNullException(nameof(nullableType));
            }

            return (T)nullableType;
        }

        public static List<Pawn> GetAllGlobalHumanlikePawns() 
        {
            List<Pawn> list = new List<Pawn>();

            foreach (Map map in Find.Maps) 
            {
                if (GetAllLivingHumanlikesOnMap(map) is List<Pawn> listp)
                    list.AddRange(listp);
            }

            return list;
        }

        public static List<Pawn> GetAllLivingHumanlikesOnMap(Map map) 
        {
            List<Pawn> humanlikePawns = GetAllHumanlikesOnMap(map);
            List<Pawn> livingPawns = new List<Pawn>();
            for (int i = 0; i < humanlikePawns.Count; i++)
            {
                if (!humanlikePawns[i].Dead) 
                {
                    livingPawns.Add(humanlikePawns[i]);
                }
            }

            return livingPawns;
        }

        public static List<Pawn> GetAllHumanlikesOnMap(Map map) 
        {
            List<Pawn> allPawns = map.mapPawns.AllPawns;
            List<Pawn> humanlikePawns = new List<Pawn>();
            foreach (Pawn pawn in allPawns) 
            {
                if (pawn.RaceProps.Humanlike) 
                {
                    humanlikePawns.Add(pawn);   
                }
            }

            return humanlikePawns;
        }

        public static bool IsHashIntervalTick(int interval) 
        {
            return Find.TickManager.TicksGame % interval == 0;
        }

        public static string GetRaceName(Pawn p) 
        {
            if (p.def is AlienRace.ThingDef_AlienRace race)
            {
                return race.defName;
            }

            Log.ErrorOnce($"GetRaceName failed to get race name for pawn {p.Name}!", p.thingIDNumber);
            return "null race";
        }
    }
}
