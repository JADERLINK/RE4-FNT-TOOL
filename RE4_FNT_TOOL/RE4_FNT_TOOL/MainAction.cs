using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_FNT_TOOL
{
    internal static class MainAction
    {
        public const string Version = "Version 1.1 (2025-01-03)";

        public static void Continue(string[] args, Endianness endianness) 
        {
            bool usingBatFile = false;
            int start = 0;

            if (args.Length != 0 && args[0].ToLowerInvariant() == "-bat") 
            {
                usingBatFile = true;
                start = 1;
            }

            for (int i = start; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        Action(args[i], endianness);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: ");
                        Console.WriteLine(ex);
                    }
                }
            }

            Console.WriteLine("Finished!!!");
            if (!usingBatFile)
            {
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
        }

        static void Action(string file, Endianness endianness)
        {
            var fileInfo = new FileInfo(file);
            Console.WriteLine("File: " + fileInfo.Name);
            var Extension = Path.GetExtension(fileInfo.Name).ToUpperInvariant();

            if (Extension == ".FNT")
            {
                Extract.Extract_All(fileInfo, endianness);
            }
            else if (Extension == ".IDXFNT")
            {
                Repack.Repack_All(fileInfo, endianness);
            }
            else
            {
                Console.WriteLine("The extension is not valid: " + Extension);
            }
        }
    }
}
