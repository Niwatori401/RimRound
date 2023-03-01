using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
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

            ShowCredits(MakeEndCredits("RimRound_Credits_Ascension_Intro".Translate(), "RimRound_Credits_Ascension_Body".Translate(), launchedPawns.ToString(), "", listOfPawns), SongDefOf.EndCreditsSong, true, 3f);

        }


        private static string MakeEndCredits(string intro, string ending, string escapees, string colonistsEscapeeTKey, IList<Pawn> escaped = null) 
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(intro);
            if (!escapees.NullOrEmpty())
            {
                stringBuilder.Append(" ");
                stringBuilder.Append(colonistsEscapeeTKey.Translate(escapees));
            }
            stringBuilder.AppendLine();
            string text = GameVictoryUtility.PawnsLeftBehind(escaped);
            if (!text.NullOrEmpty())
            {
                stringBuilder.AppendLine("GameOverColonistsLeft".Translate(text));
            }
            stringBuilder.AppendLine(ending);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(GameVictoryUtility.InMemoryOfSection());
            return stringBuilder.ToString();
        }

        private static void ShowCredits(string victoryText, SongDef endCreditsSong = null, bool exitToMainMenu = false, float songStartDelay = 5f)
        {
            Screen_Credits screen_Credits = new Screen_Credits(victoryText);
            FieldInfo credsFI = typeof(Screen_Credits).GetField("creds", BindingFlags.Instance | BindingFlags.NonPublic);
            List<CreditsEntry> credits = (List<CreditsEntry>)credsFI.GetValue(screen_Credits);
            credits.Add(new CreditRecord_Space(200f));
            credits.Add(new CreditRecord_Title("RimRound_Credits_Title".Translate()));
            credits.Add(new CreditRecord_Text("RimRound_Credits_Description".Translate(), TextAnchor.UpperLeft));

            credsFI.SetValue(screen_Credits, credits);

            screen_Credits.wonGame = true;
            screen_Credits.endCreditsSong = endCreditsSong;
            screen_Credits.exitToMainMenu = exitToMainMenu;
            screen_Credits.songStartDelay = songStartDelay;
            Find.WindowStack.Add(screen_Credits);
            Find.MusicManagerPlay.ForceSilenceFor(999f);
            ScreenFader.StartFade(Color.clear, 3f);
        }

        public static void InitiateCountdown()
        {
            timeLeft = 7.2f;
            ScreenFader.StartFade(Color.white, 7.2f);
        }

        public static float timeLeft = -1;
    }
}
