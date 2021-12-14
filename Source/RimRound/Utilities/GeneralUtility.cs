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
            return (T)nullableType;
        }

        public static bool IsHashIntervalTick(int interval) 
        {
            return Find.TickManager.TicksGame % interval == 0;
        }
    }
}
