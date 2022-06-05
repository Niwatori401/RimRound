using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TestingRange
{
    public class PatchMaker
    {
        public static void MakeSpecificPatch(List<string> linifiedTemplate, string filepathToWriteTo, string[] wordsToReplace, string[] wordsToReplaceWith) 
        {
            if (wordsToReplace.Length != wordsToReplaceWith.Length)
            {
                throw new Exception($"{wordsToReplace}'s length must be the same as {wordsToReplaceWith}'s");
            }

            for (int i = 0; i < wordsToReplace.Length; ++i) 
            {
                Replace(linifiedTemplate, wordsToReplace[i], wordsToReplaceWith[i]);
            }
            
            File.WriteAllLines(filepathToWriteTo, linifiedTemplate);
        }

        public static void MakePatchWithCSV(string templateFilePath, string destinationFilepath, string pathToCSV, int columnsForNaming = 1, NewlineMode newlineMode = NewlineMode.rn)
        {
            string stringParts = File.ReadAllText(pathToCSV);
            int numberOfArgs = GetNumberOfArgumentsPerLine(stringParts);

            List<List<string>> replacementValues = ParseIntoListOfLinesOfArgs(stringParts, numberOfArgs, newlineMode);

            int totalNumberOfRows = replacementValues[0].Count;

            string[] keywordsToReplace = GetKeywordsToReplace(replacementValues);

            for (int lineNumber = 1; lineNumber < totalNumberOfRows; ++lineNumber)
            {
                string[] keyWordsToReplaceWith = GetSpecificReplacementKeywords(numberOfArgs, replacementValues, lineNumber);

                string filenameFlair = "";
                for (int i = 0; i < columnsForNaming; ++i)
                {
                    if (i < keyWordsToReplaceWith.Length)
                    {
                        filenameFlair += $"_{keyWordsToReplaceWith[i]}";
                    }
                }

                filenameFlair = PatchMaker.RemoveCharacterFromString(
                    new char[] 
                    { 
                        ':', 
                        '<', 
                        '>', 
                        '\"', 
                        ',', 
                        '\\',
                        '/',
                        '|',
                        '?',
                        '*'
                    }, 
                    filenameFlair);

                string destinationFilepathWithIndividualName = destinationFilepath.Insert(destinationFilepath.LastIndexOf('.'), filenameFlair);

                List<string> linifiedTemaplate = ReadFileIntoLines(templateFilePath);
                PatchMaker.MakeSpecificPatch(linifiedTemaplate, destinationFilepathWithIndividualName, keywordsToReplace, keyWordsToReplaceWith);
                
            }
        }

        async public static Task MakePatchWithCSVAsync(string templateFilePath, string destinationFilepath, string pathToCSV, int columnsForNaming = 1) 
        {
            string stringParts = await File.ReadAllTextAsync(pathToCSV);
            int numberOfArgs = GetNumberOfArgumentsPerLine(stringParts);

            List<List<string>> replacementValues = ParseIntoListOfLinesOfArgs(stringParts, numberOfArgs);

            int totalNumberOfRows = replacementValues[0].Count;

            string[] keywordsToReplace = GetKeywordsToReplace(replacementValues);


            List<Task> patchTasks = new List<Task>();

            for (int lineNumber = 1; lineNumber < totalNumberOfRows; ++lineNumber)
            {
                string[] keyWordsToReplaceWith = GetSpecificReplacementKeywords(numberOfArgs, replacementValues, lineNumber);

                string filenameFlair = "";
                for (int i = 0; i < columnsForNaming; ++i)
                {
                    if (i < keyWordsToReplaceWith.Length)
                    {
                        filenameFlair += $"_{keyWordsToReplaceWith[i]}";
                    }
                }

                filenameFlair = PatchMaker.RemoveCharacterFromString(
                    new char[]
                    {
                        ':',
                        '<',
                        '>',
                        '\"',
                        ',',
                        '\\',
                        '/',
                        '|',
                        '?',
                        '*'
                    },
                    filenameFlair);

                string destinationFilepathWithIndividualName = destinationFilepath.Insert(destinationFilepath.LastIndexOf('.'), filenameFlair);
                patchTasks.Add(Task.Run(() => PatchMaker.MakeSpecificPatchAsync(templateFilePath, destinationFilepathWithIndividualName, keywordsToReplace, keyWordsToReplaceWith)));

            }

            await Task.WhenAll(patchTasks);
            return;
        }

        async public static Task MakeSpecificPatchAsync(string filepathForTemplate, string filepathToWriteTo, string[] wordsToReplace, string[] wordsToReplaceWith)
        {
            List<string> linifiedtemplate = ReadFileIntoLines(filepathForTemplate);

            if (wordsToReplace.Length != wordsToReplaceWith.Length)
            {
                throw new Exception($"{wordsToReplace}'s length must be the same as {wordsToReplaceWith}'s");
            }

            for (int i = 0; i < wordsToReplace.Length; ++i)
            {
                Replace(linifiedtemplate, wordsToReplace[i], wordsToReplaceWith[i]);
            }

            await File.WriteAllLinesAsync(filepathToWriteTo, linifiedtemplate);
        }

        private static string[] GetKeywordsToReplace(List<List<string>> replacementValues)
        {
            string[] keyWordsToReplace = new string[replacementValues.Count];

            for (int i = 0; i < keyWordsToReplace.Length; ++i)
            {
                keyWordsToReplace[i] = replacementValues[i][0];
            }

            return keyWordsToReplace;
        }

        private static string[] GetSpecificReplacementKeywords(int numberOfArgs, List<List<string>> replacementValues, int lineNumber)
        {
            string[] keyWordsToReplaceWith = new string[numberOfArgs];
            for (int argumentIndex = 0; argumentIndex < replacementValues.Count; ++argumentIndex)
            {
                keyWordsToReplaceWith[argumentIndex] = replacementValues[argumentIndex][lineNumber];
            }

            return keyWordsToReplaceWith;
        }


        public enum NewlineMode 
        {
            n,
            rn,
        };

        private static List<List<string>> ParseIntoListOfLinesOfArgs(string stringParts, int argumentsNumber, NewlineMode newlineMode = NewlineMode.rn)
        {
            string newlineChars;
            if (newlineMode == NewlineMode.rn)
                newlineChars = "\r\n";
            else
                newlineChars = "\n";

            string[] arrayOfArgs = stringParts.Split(new string[] { ",", newlineChars }, StringSplitOptions.RemoveEmptyEntries);

            List<List<string>> replacementValues = new List<List<string>>();

            int numberOfLists = argumentsNumber;
            for (int i = 0; i < numberOfLists; ++i)
            {
                replacementValues.Add(new List<string>());
            }

            for (int i = 0; i < arrayOfArgs.Length; ++i)
            {
                replacementValues[i % argumentsNumber].Add(arrayOfArgs[i]);
            }

            return replacementValues;
        }

        private static int GetNumberOfArgumentsPerLine(string stringParts)
        {
            int commaNumber = 0;
            foreach (char c in stringParts)
            {
                if (c == ',')
                    ++commaNumber;
                if (c == '\n' || c == '\r')
                    break;
            }

            return ++commaNumber;
        }

        static List<string> ReadFileIntoLines(string filepath)
        {
            List<string> listOfStrings = new List<string>();

            if (File.Exists(filepath))
            {
                foreach (string s in File.ReadAllLines(filepath))
                {
                    listOfStrings.Add(s);
                }
            }

            return listOfStrings;
        }

        static List<string> Replace(List<string> strings, string wordToReplace, string replaceWith)
        {
            for (int i = 0; i < strings.Count; ++i)
            {
                string currentLine = strings[i];
                if (currentLine.Contains(wordToReplace))
                {
                    string[] stringParts = currentLine.Split(wordToReplace, StringSplitOptions.RemoveEmptyEntries);

                    string newString = "";
                    for (int j = 0; j < stringParts.Length; ++j)
                    {
                        newString += stringParts[j];

                        if (j < stringParts.Length - 1)
                            newString += replaceWith;
                    }

                    strings[i] = newString;
                }
            }

            return strings;
        }

        static string RemoveCharacterFromString(char[] c, string s) 
        {
            foreach (char character in c) 
            {
                while (s.IndexOf(character) is int index && index != -1)
                {
                    s = s.Remove(index, 1);
                }
            }
            return s;
        }
    }
}
