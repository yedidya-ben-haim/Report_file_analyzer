using System.Diagnostics.CodeAnalysis;

namespace ReportFileAnalyzer
{

    enum ReportTypes { Collect, Analyze, Recon, Intel };
    enum Statuses { Pending, Approved, Rejected };
    
    class Program
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
                        File.AppendAllText("output.txt" ,$"\n\nFile loaded: {numOfLine} lines found");
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

            File.AppendAllText("output.txt", "\n\nProcessing complete.");
            File.AppendAllText("output.txt", $"\nValid records: {correctLines}");
            File.AppendAllText("output.txt", $"\nInvalid records: {fileLines.Length - correctLines}");
            File.AppendAllText("output.txt", $"\nStored {correctLines} valid records analysis.");
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


        static double FindMinScore(double[] score, int validRecord)
        {
            double minScore = score[0];

            if (score.Length > 0)
            {
                for (int i = 1; i < validRecord; i++)
                {
                    if (score[i] < minScore)
                    {
                        minScore = score[i];
                    }
                }
            }
            return minScore;
        }


        static int CountByStatus(Statuses[] statusArry, Statuses statusSelected, int validRecords)
        {
            int count = 0;

            for (int i = 0; i < validRecords; i++)
            {
                if (statusArry[i] == statusSelected)
                {
                    count++;
                }

            }
            return count;
        }


        static int CountByType(ReportTypes[] reportTypes, ReportTypes typeSelected, int validRecords)
        {
            int count = 0;

            for (int i = 0; i < validRecords; i++)
            {
                if (reportTypes[i] == typeSelected)
                {
                    count++;
                }

            }
            return count;
        }


        static void DisplayBasicStatistics(double[] score, int validRecord)
        {
            File.AppendAllText("output.txt", "\n\n===Report Statistics ===");
            File.AppendAllText("output.txt", $"\nTotal Reports: {validRecord}");

            double averageScore = CalculateAverage(score, validRecord);
            File.AppendAllText("output.txt", $"\nAverage Score: {averageScore:F2}");

            double maxScore = FindMaxScore(score);
            File.AppendAllText("output.txt", $"\nHighest Score: {maxScore}");

            double minScore = FindMinScore(score, validRecord);
            File.AppendAllText("output.txt", $"\nLowest Score: {minScore}");
        }


        static void DisplayStatusCounts(Statuses[] statusArry, int validRecords)
        {
            File.AppendAllText("output.txt", "\n\n===Reports by Status ===");

            int numOfRejected = CountByStatus(statusArry, Statuses.Rejected, validRecords);
            int numOfApproved = CountByStatus(statusArry, Statuses.Approved, validRecords);
            int numOfPending = CountByStatus(statusArry, Statuses.Pending, validRecords);

            File.AppendAllText("output.txt", $"\nRejected: {numOfRejected}");
            File.AppendAllText("output.txt", $"\nApproved: {numOfApproved}");
            File.AppendAllText("output.txt", $"\nPending: {numOfPending}");
        }


        static void DisplayTypeCounts(ReportTypes[] reportTypes, int validRecords)
        {
            File.AppendAllText("output.txt", "\n\n===Reports by Type ===");

            int numOfCollect = CountByType(reportTypes, ReportTypes.Collect, validRecords);
            int numOfAnalyze = CountByType(reportTypes, ReportTypes.Analyze, validRecords);
            int numOfRecon = CountByType(reportTypes, ReportTypes.Recon, validRecords);
            int numOfIntel = CountByType(reportTypes, ReportTypes.Intel, validRecords);

            File.AppendAllText("output.txt", $"\nCollect: {numOfCollect}");
            File.AppendAllText("output.txt", $"\nAnalyze: {numOfAnalyze}");
            File.AppendAllText("output.txt", $"\nRecon: {numOfRecon}");
            File.AppendAllText("output.txt", $"\nIntel: {numOfIntel}");
        }


        static void DisplayHighestPriorityApproved(string[] unitNameArry, ReportTypes[] reportTypeArry,
            int[] priorityArry, double[] scoreArry, Statuses[] statusAryy, int validRecord)
        {
            int maxPriority = 0;

            for (int i = 0; i < validRecord; i++)
            {
                if (statusAryy[i] == Statuses.Approved)
                {
                    if (priorityArry[i] > maxPriority)
                    {
                        maxPriority = priorityArry[i];
                    }
                }
            }
            File.AppendAllText("output.txt", "\n\n===Highest Priority Approved Report ===");
            for (int i = 0; i < validRecord; i++)
            {
                

                if (statusAryy[i] == Statuses.Approved)
                {
                    if (priorityArry[i] == maxPriority)
                    {
                        File.AppendAllText("output.txt", $"\n\nUnit: {unitNameArry[i]}");
                        File.AppendAllText("output.txt", $"\nType: {reportTypeArry[i]}");
                        File.AppendAllText("output.txt", $"\nPriority: {priorityArry[i]}");
                        File.AppendAllText("output.txt", $"\nScore: {scoreArry[i]:F2}");

                    }
                }
            }


        }


        static void DisplayAverageByPriority(int[] priorityArry, double[] scoreArry, int validRecord)
        {

            double[] sumScoreArry = new double[6];
            int[] numOfScoreArry = new int[6];

            for (int i = 0; i < validRecord; i++)
            {

                sumScoreArry[priorityArry[i]] += scoreArry[i];
                numOfScoreArry[priorityArry[i]]++;

            }
            File.AppendAllText("output.txt", "\n\n===Average Score by Priority ===");

            for (int i = 1; i< sumScoreArry.Length; i++)
            {
                if (sumScoreArry[i] == 0)
                {
                    File.AppendAllText("output.txt", $"\nPriority {i}: No reports");
                }
                else
                {
                    double average = sumScoreArry[i] / numOfScoreArry[i];
                    File.AppendAllText("output.txt", $"\nPriority {i}: {average:F2}");

                }
            }  



        }









        static void Main()
        {
            string[] unitName = new string[100];
            ReportTypes[] reportType = new ReportTypes[100];
            int[] priority = new int[100];
            double[] score = new double[100];
            Statuses[] status = new Statuses[100];



            File.WriteAllText("output.txt", "===ReportFileAnalyzer===");
            
            string[]? alllines = LoadFile("reports.txt");
            if (alllines != null)
            {
                int validRecords = ProcessReports(alllines, unitName, reportType, priority, score, status);

                // Basic Statistics
                DisplayBasicStatistics(score, validRecords);

                // Count By Status
                DisplayStatusCounts(status, validRecords);

                // Count By Type
                DisplayTypeCounts(reportType, validRecords);

                // Display Highest Priority Approved
                DisplayHighestPriorityApproved(unitName, reportType, priority, score, status, validRecords);

                // Display Average By Priority
                DisplayAverageByPriority(priority, score, validRecords);


            }



        }
    }
}
