using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestingRange
{
    public class PatchMaker
    {
        public static void MakeSpecificPatch(string filepathForTemplate, string filepathToWriteTo, string[] wordsToReplace, string[] wordsToReplaceWith) 
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

            File.WriteAllLines(filepathToWriteTo, linifiedtemplate);
        }

        public static void MakePatchWithCSV(string templateFilePath, string destinationFilepath, string pathToCSV, int columnsForNaming = 1)
        {
            string stringParts = File.ReadAllText(pathToCSV);

            int numberOfArgs = GetNumberOfArgumentsPerLine(stringParts);

            List<List<string>> replacementValues = ParseIntoListOfLinesOfArgs(stringParts, numberOfArgs);

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
                PatchMaker.MakeSpecificPatch(templateFilePath, destinationFilepathWithIndividualName, keywordsToReplace, keyWordsToReplaceWith);
            }
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

        private static List<List<string>> ParseIntoListOfLinesOfArgs(string stringParts, int argumentsNumber)
        {
            string[] arrayOfArgs = stringParts.Split(new string[] { ",", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

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
