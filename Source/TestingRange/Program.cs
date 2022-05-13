using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Verse;
using RimRound;

using strig = System.String;
using RimRound.Utilities;

namespace TestingRange
{
    class Program
    {
        public enum CardQuality
        {
            BadlyDamaged,
            Poor,
            Good,
            Excellent,
            Mint
        }

        public class Card
        {
            public Card(CardQuality cardQuality) 
            {
                this.cardQuality = cardQuality;
            }

            public float ticketCost = 0;

            public CardQuality cardQuality;

            public bool TryUpgrade()
            {
                float priceOfMint = (1f / 6f) + 0.5f;
                float priceOfExcellent = (1f / 12f) + 0.25f;
                float priceOfGood = (1f / 100f) + (2f / 40f);
                float priceOfPoor = (1f / 100f) + (1f / 40f);

                float percentChance = Values.RandomFloat(0, 1);
                switch (cardQuality)
                {
                    case CardQuality.BadlyDamaged:
                        ticketCost += (priceOfPoor * 5);
                        if (percentChance < 0.80f)
                        {
                            cardQuality = CardQuality.Poor;
                            return true;
                        }  
                        break;
                    case CardQuality.Poor:
                        ticketCost += (priceOfGood * 5);
                        if (percentChance < 0.70f)
                        {
                            cardQuality = CardQuality.Good;
                            return true;
                        }  
                        break;
                    case CardQuality.Good:
                        ticketCost += (priceOfExcellent * 5);
                        if (percentChance < 0.60f)
                        {
                            cardQuality = CardQuality.Excellent;
                            return true;
                        } 
                        break;
                    case CardQuality.Excellent:
                        ticketCost += (priceOfMint * 5);
                        if (percentChance < 0.50f)
                        {
                            cardQuality = CardQuality.Mint;
                            return true;
                        } 
                        break;
                    case CardQuality.Mint:
                        break;
                    default:
                        break;


                }

                return false;
            }
        }


        static void Main(string[] args)
        {
            float runs = 10000;
            Console.WriteLine("\n -----------------------------");

            CalculateAvgCostToUpgradeToMint(CardQuality.BadlyDamaged, CardQuality.Mint, runs);
            CalculateAvgCostToUpgradeToMint(CardQuality.Poor, CardQuality.Mint, runs);
            CalculateAvgCostToUpgradeToMint(CardQuality.Good, CardQuality.Mint, runs);
            CalculateAvgCostToUpgradeToMint(CardQuality.Excellent, CardQuality.Mint, runs);

            Console.WriteLine("\n -----------------------------");

            CalculateAvgCostToUpgradeToMint(CardQuality.BadlyDamaged, CardQuality.Excellent, runs);
            CalculateAvgCostToUpgradeToMint(CardQuality.Poor, CardQuality.Excellent, runs);
            CalculateAvgCostToUpgradeToMint(CardQuality.Good, CardQuality.Excellent, runs);

            Console.WriteLine("\n -----------------------------");

        }

        private static void CalculateAvgCostToUpgradeToMint(CardQuality startingQuality, CardQuality endQuality, float runs)
        {
            List<Card> goodCards = new List<Card>();
            float totalCostOfUpgrades = 0;
            for (int i = 0; i < runs; i++)
            {
                goodCards.Add(new Card(startingQuality));
            }

            for (int i = 0; i < runs; i++)
            {
                while (goodCards[i].cardQuality != endQuality)
                    goodCards[i].TryUpgrade();

                totalCostOfUpgrades += goodCards[i].ticketCost;
            }

            Console.WriteLine($"This is the cost of upgrading from {startingQuality} to {endQuality} on avg {totalCostOfUpgrades / runs:F1}");
        }


        async static Task Ma1in(string[] args)
        {

            PatchMode patchMode;
            do
            {
                patchMode = PatchMode.None;
                while (patchMode == PatchMode.None)
                {
                    Console.WriteLine("Select patch mode: \n" +
                        "1: Wipe and regenerate\n" +
                        "2: Only regenerate\n" +
                        "3: Only wipe\n" +
                        "\nOptional Args: \n A - async patch \n R - repeat patch maker process");

                    string response = Console.ReadLine();

                    if (response != null && response.Length < 1)
                    {
                        Console.WriteLine("Please choose a valid patch mode.\n");
                        continue;
                    }


                    if (response[0] == '1')
                    {
                        patchMode = PatchMode.WipeAndRefresh;
                    }
                    else if (response[0] == '2')
                    {
                        patchMode = PatchMode.Refresh;
                    }
                    else if (response[0] == '3')
                    {
                        patchMode = PatchMode.Wipe;
                    }
                    else
                    {
                        Console.WriteLine("Please choose a valid patch mode.\n");
                    }

                    if ((patchMode & PatchMode.WipeAndRefresh) > PatchMode.None)
                        if (response.Contains('r') || response.Contains('R'))
                            patchMode = patchMode | PatchMode.GoAgain;
                        if (response.Contains('a') || response.Contains('A'))
                            patchMode = patchMode | PatchMode.Async;

                }

                

                string[] directoriesToWipe = new string[] {
                    $@"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\",
                    @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\Food\VCE\",
                };

                List<PatchData> patchSets = new List<PatchData>()
                {
                    new PatchData("RimRound_AdjustAlignWithHeadTag",                    "alignWithHeadPatches",                     3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_AddComps",                                  "compsPatches",                             2, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_AlignBodyPart",                             "bodyPartAlignmentPatches",                 3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveBodySpecificOffsets",                 "removeBodySpecificOffsetsPatches",         3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_BodyTypeSpecificAlignmentPatch",            "bodyTypeSpecificAlignmentPatch",           4, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveGeneralOffset",                       "removeGeneralOffsetPatch",                 3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_SetBodyAddonDrawsize",                      "setBodyAddonDrawsize",                     3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveScaleWithBodyDrawsizeTag",            "removeScaleWithBodyDrawsizeTag",           3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveBodySpecificPortraitOffset",          "removeBodySpecificPortraitOffsetsPatches", 3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveGeneralHeadOffset",                   "removeGeneralHeadOffset",                  2, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveSpecificGraphicPathHeadOffset",       "removeSpecificGraphicPathHeadOffset",      3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveDefaultOffset",                       "removeDefaultOffset",                      3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_AlignBodyPartAbstractDef",                  "bodyPartAlignmentPatchesAbstract",         3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_BodyTypeSpecificAlignmentPatchAbstractDef", "bodyTypeSpecificAlignmentPatchAbstract",   4, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveScaleWithBodyDrawsizeTagAbstractDef", "removeScaleWithBodyDrawsizeTagAbstract",   3, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\AlienRacePatches\"),
                    new PatchData("RimRound_AddNutritionDensity",                       "setNutritionDensity",                      2, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\Food\VCE\"),
                    new PatchData("RimRound_AlterMaxToIngestAtOnceTag",                 "alterMaxToIngestTag",                      2, @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\Food\VCE\"),
                };


                if ((patchMode & PatchMode.Wipe) > PatchMode.None)
                {
                    List<string> files = new List<strig>();
                    foreach (string directory in directoriesToWipe)
                    {
                        files.AddRange(Directory.GetFiles(directory, "*.xml"));
                    }

                    foreach (string file in files)
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Threw new exception at Delete: {e.Message}");
                        }
                }

                var watch = System.Diagnostics.Stopwatch.StartNew();

                List<Task> tasks = new List<Task>();

                foreach (var x in patchSets)
                {
                    string templateFilePath = @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\PatchMaker\{x.templateName}.xml~";
                    string destinationFilePath = @$"{x.saveToDirectory}{x.templateName}.xml";
                    string csvFilePath = @$"O:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\RimRound---Alpha\Patches\PatchMaker\{x.csvName}.csv";
                    int flairArgs = x.flairArgNumber;

                    if ((patchMode & PatchMode.Refresh) > PatchMode.None)
                        if ((patchMode & PatchMode.Async) > PatchMode.None)
                            tasks.Add(PatchMaker.MakePatchWithCSVAsync(templateFilePath, destinationFilePath, csvFilePath, flairArgs));
                        else
                            tasks.Add(Task.Run(() => PatchMaker.MakePatchWithCSV(templateFilePath, destinationFilePath, csvFilePath, flairArgs)));
                }

                await Task.WhenAll(tasks);
                
                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds;
                Console.Write($"Patch Maker finished in {elapsedMs}ms\n");

            } while (((patchMode & PatchMode.GoAgain) > PatchMode.None));

            
        }

        public struct PatchData
        {
            public PatchData(string templateName, string csvName, int flairArgNumber, strig saveToDirectory) 
            {
                this.templateName = templateName;
                this.csvName = csvName;
                this.flairArgNumber = flairArgNumber;
                this.saveToDirectory = saveToDirectory;
            }

            public string templateName;
            public string csvName;
            public string saveToDirectory;
            public int flairArgNumber;
        }

        [Flags]
        public enum PatchMode
        {
            None = 0, //0
            Wipe = 1, //1
            Refresh = 2, //10
            GoAgain = 4, //100
            Async = 8,
            WipeAndRefresh = 3,
        }
    }
}
