using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class Program
{
    // Define paths
    static readonly string sourceDir = @"D:\Visual Studio - Projects\BewerbungMaster";
    static string appDir = default!;
    static string targetDir = default!;

    static void Main()
    {
        // Initialize directory paths
        appDir = Path.Combine(sourceDir, "BewerbungMasterApp");
        targetDir = Path.Combine(sourceDir, @"Extras\FilesForAI", "Files");

        // Check if Files folder exists, if so, delete it and create a new one
        if (Directory.Exists(targetDir))
        {
            Console.WriteLine($"Files folder already exists: {targetDir} Deleting and recreating...");
            try
            {
                Directory.Delete(targetDir, true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting directory: {e.Message}");
                return;
            }
        }

        try
        {
            Directory.CreateDirectory(targetDir);
            Console.WriteLine("Files folder created successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating directory: {e.Message}");
            return;
        }

        // Collect files to process
        var filesToProcess = new List<(string sourcePath, string targetPath, string originalPath)>();
        CollectFilesToProcess(filesToProcess);

        // Process files and build context content
        StringBuilder contextContent = new();
        ProcessFiles(filesToProcess, contextContent);

        // Write context file
        WriteContextFile(contextContent);

        Console.WriteLine("File copying and context creation completed.");
    }

    // Collect all files that need to be processed
    static void CollectFilesToProcess(List<(string, string, string)> filesToProcess)
    {
        string[] foldersToCopy = ["Components\\Pages", "Interfaces", "Models", "Services"];
        string[] filesToCopyFromApp = ["appsettings.Development.json", "appsettings.json", "Program.cs", "BewerbungMasterApp.csproj", "BewerbungMasterApp.csproj.user"];
        string[] filesToCopyFromRoot = ["BewerbungMaster.sln", "README.md"];

        // Add files from specified folders
        foreach (var folder in foldersToCopy)
        {
            AddFilesFromFolder(Path.Combine(appDir, folder), filesToProcess);
        }

        // Add individual files from the app directory
        foreach (var file in filesToCopyFromApp)
        {
            AddFileToProcess(Path.Combine(appDir, file), filesToProcess);
        }

        // Add individual files from the root directory
        foreach (var file in filesToCopyFromRoot)
        {
            AddFileToProcess(Path.Combine(sourceDir, file), filesToProcess);
        }
    }

    // Add all files from a specified folder to the processing list
    static void AddFilesFromFolder(string folderPath, List<(string, string, string)> filesToProcess)
    {
        if (Directory.Exists(folderPath))
        {
            foreach (var file in Directory.GetFiles(folderPath))
            {
                AddFileToProcess(file, filesToProcess);
            }
        }
        else
        {
            Console.WriteLine($"Warning: Folder not found - {folderPath}");
        }
    }

    // Add a single file to the processing list
    static void AddFileToProcess(string sourcePath, List<(string, string, string)> filesToProcess)
    {
        if (File.Exists(sourcePath))
        {
            string fileName = Path.GetFileName(sourcePath);
            string targetPath = Path.Combine(targetDir, fileName);
            string originalPath = sourcePath;
            filesToProcess.Add((sourcePath, targetPath, originalPath));
        }
        else
        {
            Console.WriteLine($"Warning: File not found - {sourcePath}");
        }
    }

    // Process all files: copy them and add to context
    static void ProcessFiles(List<(string sourcePath, string targetPath, string originalPath)> filesToProcess, StringBuilder contextContent)
    {
        foreach (var (sourcePath, targetPath, originalPath) in filesToProcess)
        {
            CopyFile(sourcePath, targetPath);
            AddToContext(contextContent, sourcePath, originalPath);
        }
    }

    // Copy a single file
    static void CopyFile(string sourcePath, string targetPath)
    {
        File.Copy(sourcePath, targetPath, true);
        Console.WriteLine($"Copied: {Path.GetFileName(sourcePath)}");
    }

    // Add file content to the context
    static void AddToContext(StringBuilder contextContent, string sourcePath, string originalPath)
    {
        contextContent.AppendLine($"<{originalPath}>");
        contextContent.AppendLine(File.ReadAllText(sourcePath));
        contextContent.AppendLine();
    }

    // Write the context content to a file
    static void WriteContextFile(StringBuilder contextContent)
    {
        string contextFilePath = Path.Combine(targetDir, "context.txt");
        File.WriteAllText(contextFilePath, contextContent.ToString());
        Console.WriteLine("context.txt created.");
    }
}