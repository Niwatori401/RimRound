using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestingRange
{
    public class PatchMaker
    {
        public static List<string> ReadFileIntoLines(string filepath) 
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

        public static List<string> Replace(List<string> strings, string wordToReplace, string replaceWith) 
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

                        if (j < stringParts.Length -1)
                            newString += replaceWith;
                    }

                    strings[i] = newString;
                }
            }

            return strings;
        }

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


    }
}
