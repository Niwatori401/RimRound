using HarmonyLib;
using RimRound.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace RimRound.Patch
{
    [HarmonyPatch(typeof(Root_Play))]
    [HarmonyPatch(nameof(Root_Play.Update))]
    public class RootPlay_Update_AddCountdownTick
    {
        public static void Postfix() 
        {
            if (GameOverUtility.timeLeft > 0f)
            {
                GameOverUtility.timeLeft -= Time.deltaTime;
                if (GameOverUtility.timeLeft <= 0f)
                {
                    GameOverUtility.AscensionGameEnding();
                }
            }
        }
    }
}
