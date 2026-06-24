namespace ReportFileAnalyzer
{

    enum ReportTypes { Collect, Analyze, Recon, Intel };
    enum Statuses { Pending, Approved, Rejected };
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


        static int ProcessReports(string[] fileLines, string[] unitName, ReportTypes[] reportType,
            int[] priority, double[] score, Statuses[] status)
        {
            int correctLines = 0;

            foreach (string line in fileLines)
            {
                string[] linesplit = line.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                // Checking that there are 5 values
                if (linesplit.Length != 5)
                {
                    Console.WriteLine("Invalid record: Missing or excess information");
                    continue;
                }
                else
                {
                    // 1. Check ReportType
                    if (!ReportTypes.TryParse(linesplit[1], true, out ReportTypes reportTypeVaild))
                    {
                        Console.WriteLine("Invalid record: Unknown ReportType type");
                        continue;
                    }

                    // 2. Check Priority
                    if (int.TryParse(linesplit[2], out int priorityVaild))
                    {
                        if (1 > priorityVaild || priorityVaild > 5)
                        {
                            Console.WriteLine("Invalid record: Priority should be in the range of 1-5");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid record: Priority must be a number");
                        continue;
                    }

                    // 3. Check Score
                    if (double.TryParse(linesplit[3], out double scoreVaild))
                    {
                        if (0 > scoreVaild || scoreVaild > 100)
                        {
                            Console.WriteLine("Invalid record: score should be in the range of 0 - 100");
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid record: Score must be a number");
                        continue;
                    }

                    // 4. Check Status
                    if (!Statuses.TryParse(linesplit[4], true, out Statuses statusVaild))
                    {
                        Console.WriteLine("Invalid record: Unknown Status type");
                        continue;
                    }


                    // Record storage
                    unitName[correctLines] = linesplit[0];
                    reportType[correctLines] = reportTypeVaild;
                    priority[correctLines] = priorityVaild;
                    score[correctLines] = scoreVaild;
                    status[correctLines] = statusVaild;
                    correctLines++;
                }
            }
            Console.WriteLine($"Stored {correctLines} valid records analysis.");
            return correctLines;

        }


        static double CalculateAverage(double[] score, int validRecord)
        {
            double totalSum = 0.0;

            if (validRecord <= 0)
            {
                return totalSum;
            }
            foreach (double num in score)
            {
                totalSum += num;
            }
            double average = totalSum / validRecord;
            return average;

        }


        static double FindMaxScore(double[] score)
        {
            double maxScore = score[0];

            if (score.Length > 0)
            {
                foreach (double s in score)
                {
                    if (s > maxScore)
                    {
                        maxScore = s;
                    }
                }
            }
            return maxScore;
        }






        static void Main()
        {
            string[] unitName = new string[100];
            ReportTypes[] reportType = new ReportTypes[100];
            int[] priority = new int[100];
            double[] score = new double[100];
            Statuses[] status = new Statuses[100];




            string[]? alllines = LoadFile("reports.txt");
            if (alllines != null)
            {
                int validRecords = ProcessReports(alllines, unitName, reportType, priority, score, status);
                Console.WriteLine("===Report Statistics ===");
                Console.WriteLine($"Total Reports: {validRecords}");

                double averageScore = CalculateAverage(score, validRecords);
                Console.WriteLine($"Average Score: {averageScore:F2}");

                double maxScore = FindMaxScore(score);
                Console.WriteLine($"Max Score: {maxScore}");

            }



        }
    }
}
