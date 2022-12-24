using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RimRound.Utilities
{
    internal class GameOverUtility
    {
        public static void AscensionGameEnding() 
        {
            StringBuilder launchedPawns = new StringBuilder();
            List<Pawn> listOfPawns = (from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
                               where p.RaceProps.Humanlike
                               select p).ToList<Pawn>();
            foreach (Pawn pawn in listOfPawns)
            {
                if (!pawn.Dead && !pawn.IsQuestLodger())
                {
                    launchedPawns.AppendLine("   " + pawn.LabelCap);
                    Find.StoryWatcher.statsRecord.colonistsLaunched++;
                }
            }

            //GameVictoryUtility.ShowCredits(GameVictoryUtility.MakeEndCredits("GameOverArchotechInvokedIntro".Translate(), "GameOverArchotechInvokedEnding".Translate(), launchedPawns.ToString(), "GameOverColonistsTranscended", list), SongDefOf.ArchonexusVictorySong, true, 2.5f);

        }
    }
}
