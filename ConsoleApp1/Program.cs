using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        // Ask the user to choose an option
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;
        Console.WriteLine("Please select an option:");
        Console.WriteLine("[1] Tối Ưu");
        Console.WriteLine("[2] Tương Thích");
        string option = Console.ReadLine();

        // Ask the user to input the directory path
        Console.WriteLine("Please enter the directory path where re4.exe is located:");
        string directoryPath = Console.ReadLine();

        // Check if the directory exists
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("The directory does not exist. Please check the path and try again.");
            return;
        }

        // Define the file names
        string file1 = "re4.exe";
        string file2 = "modmanager.exe";

        // Define the path to the Patch folder
        string patchFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Patch");

        // Define the path to the FluffyManager.zip file
        string zipFilePath = Path.Combine(patchFolderPath, "FluffyManager.zip");

        // Define the path to the .7z file within the Patch folder
        string sourceFilePath = Path.Combine(patchFolderPath, "RE4R-VI_HOTA4_V1.5.7z");
        string modsDirectory = Path.Combine(directoryPath, "Games", "RE4R", "Mods");
        string destinationPath = Path.Combine(modsDirectory, "RE4R-VI_HOTA4_V1.5.7z");

        try
        {
            switch (option)
            {
                case "1":
                    // Option 1: Normal operations
                    bool file1Exists = File.Exists(Path.Combine(directoryPath, file1));
                    bool file2Exists = File.Exists(Path.Combine(directoryPath, file2));

                    // Output the result
                    Console.WriteLine($"{file1} exists: {file1Exists}");
                    Console.WriteLine($"{file2} exists: {file2Exists}");

                    if (file1Exists && (!file2Exists))
                    {
                        // Extract FluffyManager.zip to the directory where re4.exe is located
                        if (File.Exists(zipFilePath))
                        {
                            ZipFile.ExtractToDirectory(zipFilePath, directoryPath, true); // true to overwrite existing files
                            Console.WriteLine("FluffyManager.zip has been successfully extracted to the directory where re4.exe is located.");

                            // Copy the .7z file to the Mods directory
                            if (File.Exists(sourceFilePath))
                            {
                                if (!Directory.Exists(modsDirectory))
                                {
                                    Directory.CreateDirectory(modsDirectory);
                                }
                                File.Copy(sourceFilePath, destinationPath, true);
                                Console.WriteLine("The 7z file has been successfully copied to the Mods folder.");
                            }
                            else
                            {
                                Console.WriteLine($"The file {sourceFilePath} does not exist.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"The file {zipFilePath} does not exist.");
                        }
                    }
                    else if (file1Exists && file2Exists)
                    {
                        // Copy the .7z file to the Mods directory
                        if (File.Exists(sourceFilePath))
                        {
                            if (!Directory.Exists(modsDirectory))
                            {
                                Directory.CreateDirectory(modsDirectory);
                            }
                            File.Copy(sourceFilePath, destinationPath, true);
                            Console.WriteLine("The 7z file has been successfully copied to the Mods folder.");
                        }
                        else
                        {
                            Console.WriteLine($"The file {sourceFilePath} does not exist.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("The required files do not meet the criteria for extraction.");
                    }
                    break;

                case "2":
                    // Option 2: Handle patch file naming and copying
                    if (File.Exists(Path.Combine(directoryPath, file1)))
                    {
                        // Find all patch files in the directory
                        var patchFiles = Directory.GetFiles(directoryPath, "re_chunk_000.pak.patch_*.pak")
                                                   .Select(Path.GetFileName)
                                                   .OrderByDescending(f => f)
                                                   .ToList();

                        if (patchFiles.Count > 0)
                        {
                            // Determine the last patch file and increment the number
                            string lastPatchFile = patchFiles.First();
                            int lastNumber = int.Parse(lastPatchFile.Split('_').Last().Split('.').First());
                            int newNumber = lastNumber + 1;

                            // Define the new patch file name
                            string newPatchFileName = $"re_chunk_000.pak.patch_{newNumber:D3}.pak";
                            string newPatchFilePath = Path.Combine(directoryPath, newPatchFileName);

                            // Define the path to the placeholder file in the Patch folder
                            string placeholderFilePath = Path.Combine(patchFolderPath, "re_chunk_000.pak.patch_00x.pak");

                            // Replace placeholder filename with the new patch filename
                            if (File.Exists(placeholderFilePath))
                            {
                                string newFileNameInPatch = Path.Combine(patchFolderPath, newPatchFileName);
                                File.Copy(placeholderFilePath, newFileNameInPatch, true);
                                Console.WriteLine($"Replaced placeholder filename with {newPatchFileName} in the Patch folder.");

                                // Copy the new patch file to the directory where re4.exe is placed
                                string destinationFilePath = Path.Combine(directoryPath, newPatchFileName);
                                File.Copy(newFileNameInPatch, destinationFilePath, true);
                                Console.WriteLine($"Copied {newPatchFileName} to the directory where re4.exe is located.");
                            }
                            else
                            {
                                Console.WriteLine($"The placeholder file re_chunk_000.pak.patch_00x.pak does not exist in the Patch folder.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No patch files found in the directory.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{file1} does not exist in the directory.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid option selected.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
