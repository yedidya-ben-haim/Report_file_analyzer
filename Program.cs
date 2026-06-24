using System;
using System.IO;

namespace ReportFileAnalyzer
{
    class Project
    {

        
        static string[] LoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string[] alllines = File.ReadAllLines(filePath);
                    return alllines;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file: {ex}");
                    return new string[0];
                }

            }
            else
            {
                Console.WriteLine("File not found");
                return new string[0];
            }
        }






        static void Main()
        {
            string [] alllines = LoadFile("reports.txt");
            foreach (string line in alllines)
            {
                Console.WriteLine(line);
            }
        }
    }
}
