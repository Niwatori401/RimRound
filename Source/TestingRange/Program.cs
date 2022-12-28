using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


using strig = System.String;


namespace TestingRange
{
    class Program
    {
        static void Main1(string[] args) 
        {

        }
        async static Task Main(string[] args)
        {
            string pathToRR = "";
            pathToRR = DoPathInputLoop();
            PatchMode patchMode;
            do
            {
                patchMode = PatchMode.None;
                patchMode = DoPatchModeInputLoop(patchMode);

                string[] directoriesToWipe = new string[] {
                    @$"{pathToRR}Patches\AlienRacePatches\",
                    @$"{pathToRR}Patches\Food\VCE\",
                };

                CheckIfPathsValid(directoriesToWipe);

                List<PatchData> patchSets = new List<PatchData>()
                {
                    new PatchData("RimRound_AdjustAlignWithHeadTag",                    "alignWithHeadPatches",                     3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_AddComps",                                  "compsPatches",                             2, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_AlignBodyPart",                             "bodyPartAlignmentPatches",                 3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveBodySpecificOffsets",                 "removeBodySpecificOffsetsPatches",         3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_BodyTypeSpecificAlignmentPatch",            "bodyTypeSpecificAlignmentPatch",           4, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveGeneralOffset",                       "removeGeneralOffsetPatch",                 3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_SetBodyAddonDrawsize",                      "setBodyAddonDrawsize",                     3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveScaleWithBodyDrawsizeTag",            "removeScaleWithBodyDrawsizeTag",           3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveBodySpecificPortraitOffset",          "removeBodySpecificPortraitOffsetsPatches", 3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveGeneralHeadOffset",                   "removeGeneralHeadOffset",                  2, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveSpecificGraphicPathHeadOffset",       "removeSpecificGraphicPathHeadOffset",      3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveDefaultOffset",                       "removeDefaultOffset",                      3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_AlignBodyPartAbstractDef",                  "bodyPartAlignmentPatchesAbstract",         3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_BodyTypeSpecificAlignmentPatchAbstractDef", "bodyTypeSpecificAlignmentPatchAbstract",   4, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_RemoveScaleWithBodyDrawsizeTagAbstractDef", "removeScaleWithBodyDrawsizeTagAbstract",   3, @$"{pathToRR}Patches\AlienRacePatches\"),
                    new PatchData("RimRound_AddNutritionDensity",                       "setNutritionDensity",                      2, @$"{pathToRR}Patches\Food\VCE\"),
                    new PatchData("RimRound_AlterMaxToIngestAtOnceTag",                 "alterMaxToIngestTag",                      2, @$"{pathToRR}Patches\Food\VCE\"),
                    new PatchData("RimRound_AddOrModifyLayerOffsetOnCardinals",         "addOrAlterLayerOffsetOnCardinals",         3, @$"{pathToRR}Patches\AlienRacePatches\"),
                };

                CheckIfPathsValid(patchSets);
                CheckIfCSVFilePathsValid(patchSets, @$"{pathToRR}Patches\PatchMaker\");
                CheckIfTemplateFilePathsValid(patchSets, @$"{pathToRR}Patches\PatchMaker\");
                
                DeleteFiles(patchMode, directoriesToWipe);

                var watch = System.Diagnostics.Stopwatch.StartNew();

                List<Task> tasks = new List<Task>();

                foreach (var x in patchSets)
                {
                    string templateFilePath = @$"{pathToRR}Patches\PatchMaker\{x.templateName}.xml~";
                    string destinationFilePath = @$"{x.saveToDirectory}{x.templateName}.xml";
                    string csvFilePath = @$"{pathToRR}Patches\PatchMaker\{x.csvName}.csv";
                    int flairArgs = x.flairArgNumber;

                    if ((patchMode & PatchMode.Refresh) > PatchMode.None)
                        if ((patchMode & PatchMode.Async) > PatchMode.None)
                            tasks.Add(PatchMaker.MakePatchWithCSVAsync(templateFilePath, destinationFilePath, csvFilePath, flairArgs));
                        else
                            PatchMaker.MakePatchWithCSV(templateFilePath, destinationFilePath, csvFilePath, flairArgs, (patchMode & PatchMode.NewlineAlt) > PatchMode.None ? PatchMaker.NewlineMode.n : PatchMaker.NewlineMode.rn);                            
                }

                await Task.WhenAll(tasks);

                watch.Stop();

                var elapsedMs = watch.ElapsedMilliseconds;
                Console.Write($"Patch Maker finished in {elapsedMs}ms\n");

            } while (((patchMode & PatchMode.GoAgain) > PatchMode.None));

            
        }

        private static string DoPathInputLoop() 
        {
            do
            {
                Console.WriteLine("Please input the RimRound install directory. For example: \nC:\\Program Files (x86)\\Steam\\steamapps\\common\\RimWorld\\Mods\\RimRound---Alpha\\\n\nMake sure to use BACKSLASH and include the last backslash at the end.");
                string userInput = Console.ReadLine();
                Console.WriteLine($"Got: {userInput}\nChecking if valid...");
                if (!Directory.Exists(userInput))
                    Console.WriteLine($"Could not find directory at {userInput}. Please try again.");
                else
                {
                    Console.WriteLine("Directory valid!");
                    return userInput;
                }
                    
            } while (true);
        }

        private static void CheckIfPathsValid(string[] directoriesToWipe)
        {
            foreach (string path in directoriesToWipe)
            {
                if (!Directory.Exists(path))
                    throw new Exception($"Invalid directory at {path}");
            }
        }

        private static void CheckIfTemplateFilePathsValid(List<PatchData> patchDatas, string templatePath) 
        {
            foreach (PatchData patch in patchDatas)
            {
                string filepath = templatePath + patch.templateName + ".xml~";
                if (!File.Exists(filepath))
                    throw new Exception($"Nonexistent file at {filepath}");
            }
        }

        private static void CheckIfCSVFilePathsValid(List<PatchData> patchDatas, string csvPath)
        {
            foreach (PatchData patch in patchDatas)
            {
                string filepath = csvPath + patch.csvName + ".csv";
                if (!File.Exists(filepath))
                    throw new Exception($"Nonexistent file at {filepath}");
            }
        }
        private static void CheckIfPathsValid(List<PatchData> patchDatas)
        {
            foreach (PatchData patch in patchDatas)
            {

                if (!Directory.Exists(patch.saveToDirectory))
                    throw new Exception($"Invalid directory at {patch.saveToDirectory}");
            }
        }
         
        private static PatchMode DoPatchModeInputLoop(PatchMode patchMode)
        {
            while (patchMode == PatchMode.None)
            {
                Console.WriteLine("Select patch mode: \n" +
                    "1: Wipe and regenerate\n" +
                    "2: Only regenerate\n" +
                    "3: Only wipe\n" +
                    "\nOptional Args: \n A - async patch \n R - repeat patch maker process \n N - use \\n instead of \\r\\n");

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
                if (response.Contains('n') || response.Contains('N'))
                    patchMode = patchMode | PatchMode.NewlineAlt;

            }

            return patchMode;
        }

        private static void DeleteFiles(PatchMode patchMode, string[] directoriesToWipe)
        {
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
            NewlineAlt = 16,
            WipeAndRefresh = 3,
        }
    }
}
