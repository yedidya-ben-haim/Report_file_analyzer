namespace ReportFileAnalyzer
{

    enum ReportTypes { Collect, Analyze, Recon, Intel };
    class Project
    {
        static string[]? LoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string[] alllines = File.ReadAllLines(filePath);
                    int numOfLine = alllines.Length;
                    if (numOfLine == 0)
                    {
                        Console.WriteLine("Error: File is empty");
                        return null;
                    }
                    else
                    {
                        Console.WriteLine($"File loaded: {numOfLine} lines found");
                        return alllines;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file: {ex}");
                    return null;
                }

            }
            else
            {
                Console.WriteLine($"Error: File {Path.GetFileName(filePath)} not found");
                return null;
            }
        }


        static int ProcessReports(string[] fileLines)
        {
            foreach (string line in fileLines)
            {
                string[] linesplit = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                // Checking that there are 5 values
                if (linesplit.Length < 5)
                {
                    continue;
                }
                else
                {
                    int validLine = 0;

                    // 1. Check ReportType
                    if (ReportTypes.TryParse(linesplit[1], true, out ReportTypes ReportType))
                    {
                        Console.WriteLine($"Type: {ReportType}");
                        validLine++;
                    }
                    else
                    {
                        Console.WriteLine("Invalid record: Unknown ReportType type");
                    }

                    // 2. Check Priority
                    if (int.TryParse(linesplit[2], out int))
                    {
                        Console.WriteLine($"Type: {ReportType}");
                        validLine++;
                    }
                    else
                    {
                        Console.WriteLine("Invalid record: Unknown ReportType type");
                    }
                    

                }



                //foreach (string linesplits in linesplit)
                //{
                //    Console.WriteLine(linesplits);
                //}

            }
            return 1;

        }


















        static void Main()
        {
            string?[] alllines = LoadFile("reports.txt");
            int m = ProcessReports(alllines);


            //Console.WriteLine(alllines)

            //foreach (string line in alllines)
            //{
            //    Console.WriteLine(line);
            //}
        }
    }
}
