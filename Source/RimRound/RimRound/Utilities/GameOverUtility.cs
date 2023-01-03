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

            ShowCredits(MakeEndCredits(AscensionIntro, AscensionEnding, launchedPawns.ToString(), "", listOfPawns), SongDefOf.EndCreditsSong, true, 3f);

        }

        static string AscensionIntro = "You've done it. Your ceaseless quest for gluttony has allowed you to expand to a size so colossal, that an alien ship was able to spot you from orbit. Intrigued, they came to investigate, and were fascinated with the...results. They decided to assist you, not in leaving the planet, but in making you a test subject.\n";

        static string AscensionEnding = "Their technologies and culinary preparations were far beyond comprehension, and the subject had little choice in this new undertaking. In due time, their current size was dwarfed, and their exponential growth continued unimpeded. The world beneath them shrank, until they took its place, becoming the land itself. With only the vast expanse of space to challenge their scale, their conquest did not end there, on the contrary, it was merely beginning.\n\nFattened beyond even mortal understanding, the subject became so immense that even many archotech societies took notice, and, eventually, cultivated a galactic empire using their eternally expanding form. Some say they are there to this very day, in a state of perpetual bliss, their inescapable fate quite literally written in the stars.\n\nYou have chosen your destiny.";

        static string RR_CreditsSectionTitle = "RimRound Credits";
        static string RR_CreditsSection = "\n\nProgramming: Niwatori401\nArt - BambooAle";

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
            credits.Add(new CreditRecord_Text(RR_CreditsSectionTitle, TextAnchor.MiddleCenter));
            credits.Add(new CreditRecord_Text(RR_CreditsSection, TextAnchor.UpperLeft));

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
