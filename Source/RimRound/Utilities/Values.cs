using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using UnityEngine;

namespace RimRound.Utilities
{
    public static class Values
    {
        public static float debugPos = 0;
        public static float debugPos2 = 0;


        

        //Note: you must add "_{bodyType.defName}" to this to get the shirt graphic or it will not work.
        public static string defaultClothingSetGraphicPath = "Things/Pawn/Humanlike/Apparel/ShirtButton/ShirtButton";

        public static float severityPerKilo = 0.001f;
        public static float nutritionPerKilo = 1.0f;


        public static float[] validBodyMeshSizes = new float[] 
        {
            0.5f,
            0.5625f,
            0.625f,
            0.6875f,
            0.75f,
            0.8125f,
            0.875f,
            0.9375f,
            1.0f,
            1.125f,
            1.25f,
            1.375f,
            1.5f,
            1.625f,
            1.75f,
            1.875f,
            2.0f,
            2.125f,
            2.25f,
            2.375f,
            2.5f,
            2.625f,
            2.75f,
            2.875f,
            3.0f,
            3.125f,
            3.25f,
            3.375f,
            3.5f,
            3.625f,
            3.75f,
            3.875f,
            4.0f,
            4.25f,
            4.50f,
            4.75f,
            5.00f,
            5.25f,
            5.50f,
            5.75f,
            6.00f,
            6.25f,
            6.50f,
            6.75f,
            7.00f,
            7.25f,
            7.50f,
            7.75f,
            8.00f,
            8.5f,
            9.0f,
            10.0f,
            11.0f,
            12.0f,
            13.0f,
            15.0f,
            17.0f,
            20.0f,
        };


        public static bool RandomChanceAtOrBelow(float maxValue) 
        {
            if (maxValue >= 1)
                Log.Warning("maxValue for RandomChanceAtOrBelow() was set to 1 or higher! This will always result in a pass.");

            if (RandomFloat(0, 1) <= maxValue)
                return true;

            return false;
        }

        public static int RandomInt(int lower, int upper) 
        {
            return random.Next(lower, upper + 1);
        }

        public static float RandomFloat(float lower, float upper) 
        {
            return (float)random.NextDouble() * (upper - lower) + lower;
        }

        public const float MinRQ = 0.000001f;

        public static System.Random random = new System.Random();
    }
}


