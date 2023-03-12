using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InstallationIntegrityChecker
{
    public class Program
    {
        static void Main(string[] args)
        {
            string mode = args[0]; // CREATE or CHECK
            string basePath = args[1]; // Has ending backslash
            string outputFileLocation = mode == "CREATE" ? $@"{basePath}output.md5" : $@"{basePath}verify_output.md5";
            string readFileLocation = mode == "CREATE" ? $@"{basePath}SHA_MEASURE\" : $@"{basePath}";
            Console.WriteLine($"Output file: {outputFileLocation}");
            Console.WriteLine($"Read file location: {readFileLocation}");
            
            List<string> files = GetFilesInDir(readFileLocation);
            Console.WriteLine($"Number of files {files.Count}");
            string result = CalculateMD5(files);
            File.WriteAllText(outputFileLocation, result);
        }

        static List<string> backlistFilePaths = new List<string>() 
        {
            ".md5",
            ".git",
            "INTEGRITY_HASH",
            "RESULT_HASH",
            ".bat",
            ".exe",
        };

        static List<string> GetFilesInDir(string sDir)
        {
            List<string> files = new List<string>();

            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        bool badFile = false;

                        foreach (var blitem in backlistFilePaths)
                            if (f.Contains(blitem))
                                badFile = true;

                        if (!badFile)
                            files.Add(f);     
                    }
                    files.AddRange(GetFilesInDir(d));
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }

            return files;
        }

        static string CalculateMD5(List<string> filenames)
        {
            string result = "";
            using (var md5 = MD5.Create())
            {
                foreach (string filename in filenames)
                {
                    try
                    {
                        using (var stream = File.OpenRead(filename))
                        {
                            var hash = md5.ComputeHash(stream);
                            result += BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant()+ '\n';
                        }
                    }
                    catch (Exception e) 
                    {
                        Console.Write(e.Message);
                    }
                }
            }

            return result;
        }
    }
}
