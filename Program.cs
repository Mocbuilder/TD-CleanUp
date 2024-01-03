using System;
using System.ComponentModel.Design;
using System.IO;

class Program
{
    static void Main()
    {
        Logo();
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Print("INFO[]  SELECT AN OPTION:");
            Print("INFO[]  1. DELETE TEMPORARY FILES");
            Print("INFO[]  2. DELETE DUPLICATE FILES");
            Print("INFO[]  3. EXIT");

            string userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    TempClean();
                    break;
                case "2":
                    DupClean();
                    break;
                case "3":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Print("SYSTEM[]  SHUTTING DOWN...");
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Print("WARNING[]  INVALID OPTION. TRY AGAIN...");
                    break;
            }
        }
    }

    static void TempClean()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Print("SYSTEM[]  LOADING SYSTEMS...");
        string tempFolderPath = Path.GetTempPath();
        Console.ForegroundColor = ConsoleColor.White;
        Print("INFO[]  Temp folder found at: " + tempFolderPath);
        Console.ForegroundColor = ConsoleColor.Red;
        Print("WARNING[]  THIS WILL DELETE ALL FILES IN THE TEMP FOLDER: ENTER y TO CONFIRM:\n");
        string userOK = Console.ReadLine();

        if (userOK != "y")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Print("WARNING[]  SYSTEMS SHUTTING DOWN...");
            Environment.Exit(0);
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Print("INFO[]  CLEAN UP CONFIRMED. PROCEEDING...");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Print("SYSTEM[]  STARTING CLEANUP...");
        try
        {
            DeleteFilesInFolder(tempFolderPath);

            Console.ForegroundColor= ConsoleColor.White;
            Print("INFO[]  CLEANUP SUCCESFULL: RETURNING TO MENU...");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor= ConsoleColor.Red;
            Print($"WARNING[]  ERROR OCCURED:\n{ex.Message}");
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Print("SYSTEM[]  LOADING MENU...");
    }

    static void DeleteFilesInFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            Print($"WARNING[]  FOLDER DOES NOT EXIST: {folderPath}");
            Main();
        }

        string[] files = Directory.GetFiles(folderPath);

        foreach (string file in files)
        {
            try
            {
                File.Delete(file);
                Console.ForegroundColor = ConsoleColor.Green;
                Print($"PROGRESS[]  DELETED FILE: {file}");
            }
            catch (IOException)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Print($"PROGRESS[]  FILE SKIPPED (IN USE): {file}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Print($"WARNING[] ERROR WITH FILE: {file}\nWARNING[]  ERROR MESSAGE: {ex.Message}");
            }
        }
    }

    static void DupClean()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Print("SYSTEM[]  LOADING SYSTEMS...");
        Console.ForegroundColor = ConsoleColor.White;
        Print("INFO[]  ENTER THE ROOT FOLDER PATH TO SEARCH FOR DUPLICATE FILES:");
        string rootFolderPath = Console.ReadLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Print("SYSTEM[]  SCANNING FOR DUPLICATE FILES...");

        try
        {
            Dictionary<string, List<string>> fileNames = GetFileNames(rootFolderPath);
            DeleteDuplicateFiles(fileNames);

            Console.ForegroundColor = ConsoleColor.White;
            Print("INFO[]  DUPLICATE FILE CLEANUP SUCCESSFUL: RETURNING TO MENU...");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Print($"WARNING[]  ERROR OCCURRED:\n{ex.Message}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Print("SYSTEM[]  LOADING MENU...");
            Main();
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Print("SYSTEM[]  LOADING MENU...");
    }

    static Dictionary<string, List<string>> GetFileNames(string rootFolderPath)
    {
        Dictionary<string, List<string>> fileNames = new Dictionary<string, List<string>>();

        string[] allFiles = Directory.GetFiles(rootFolderPath, "*", SearchOption.AllDirectories);

        foreach (string filePath in allFiles)
        {
            try
            {
                string fileName = Path.GetFileName(filePath);

                if (fileNames.ContainsKey(fileName))
                {
                    fileNames[fileName].Add(filePath);
                }
                else
                {
                    fileNames[fileName] = new List<string> { filePath };
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Print($"WARNING[] ERROR WITH FILE: {filePath}\nWARNING[] ERROR MESSAGE: {ex.Message}");
            }
        }

        return fileNames;
    }

    static void DeleteDuplicateFiles(Dictionary<string, List<string>> fileNames)
    {
        foreach (var nameEntry in fileNames)
        {
            if (nameEntry.Value.Count < 1)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Print("PROGRESS[]  NO DUPLICATE FOUND..");
            }

            for (int i = 1; i < nameEntry.Value.Count; i++)
            {
                try
                {
                    File.Delete(nameEntry.Value[i]);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Print($"PROGRESS[] DELETED DUPLICATE FILE: {nameEntry.Value[i]}");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Print($"WARNING[] ERROR WITH FILE: {nameEntry.Value[i]}\nWARNING[] ERROR MESSAGE: {ex.Message}");
                }
            }
        }
    }

    static void Logo()
    {
        Console.ForegroundColor= ConsoleColor.Blue;
        Console.WriteLine(@"
              _______ _____          _____ _      ______          _   _ _    _ _____  
             |__   __|  __ \        / ____| |    |  ____|   /\   | \ | | |  | |  __ \ 
                | |  | |  | |______| |    | |    | |__     /  \  |  \| | |  | | |__) |
                | |  | |  | |______| |    | |    |  __|   / /\ \ | . ` | |  | |  ___/ 
                | |  | |__| |      | |____| |____| |____ / ____ \| |\  | |__| | |     
                |_|  |_____/        \_____|______|______/_/    \_\_| \_|\____/|_|     
                                                                          
                                                                          
        ");
    }

    static void Print(string text)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(50);
        }
        Console.WriteLine();
    }
}
