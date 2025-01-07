using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace SharedCode
{
    internal static class Shared
    {
        public static void Continue(string[] args, Action<Bitmap, Config, FileInfo> Continue2)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("You must pass as parameters a PNG file and a CONFIG file");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("The first file does not exist.");
                return;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine("The second file does not exist.");
                return;
            }

            FileInfo pngFileInfo = null;
            FileInfo configFileInfo = null;

            if (Path.GetExtension(args[0].ToLowerInvariant()).Contains("png"))
            {
                pngFileInfo = new FileInfo(args[0]);
            }
            else if (Path.GetExtension(args[1].ToLowerInvariant()).Contains("png"))
            {
                pngFileInfo = new FileInfo(args[1]);
            }

            if (Path.GetExtension(args[0].ToLowerInvariant()).Contains("config"))
            {
                configFileInfo = new FileInfo(args[0]);
            }
            else if (Path.GetExtension(args[1].ToLowerInvariant()).Contains("config"))
            {
                configFileInfo = new FileInfo(args[1]);
            }

            if (pngFileInfo == null)
            {
                Console.WriteLine("The png file path not found.");
                return;
            }

            if (configFileInfo == null)
            {
                Console.WriteLine("The png config path not found.");
                return;
            }

            Bitmap bitmap = null;
            Config config = null;

            try
            {
                bitmap = new Bitmap(pngFileInfo.FullName);
            }
            catch (Exception)
            {
                Console.WriteLine("Error loading png file.");
                return;
            }

            try
            {
                config = Config.GetConfig(configFileInfo);
            }
            catch (Exception)
            {
                Console.WriteLine("Error loading config file.");
                return;
            }

            Continue2(bitmap, config, pngFileInfo);
        }

        public static void Part2Info(Bitmap bitmap, Config config)
        {
            int BitmapWidth = bitmap.Width;
            int BitmapHeight = bitmap.Height;

            Console.WriteLine("PNG: " + BitmapWidth + "x" + BitmapHeight);
            Console.WriteLine("HorizontalCharCount: " + config.HorizontalCharCount);
            Console.WriteLine("VerticalCharCount: " + config.VerticalCharCount);
            Console.WriteLine("BaseHorizontalCharLength: " + config.BaseHorizontalCharLength);
            Console.WriteLine("ExtraLeftMargin: " + config.ExtraLeftMargin);
            Console.WriteLine("ExtraRightMargin: " + config.ExtraRightMargin);
            Console.WriteLine("MinimumCharLength: " + config.MinimumCharLength);
            Console.WriteLine("CheckAlpha: " + config.CheckAlpha);
            Console.WriteLine("ImageHorizontalCharLength: " + (int)(bitmap.Width / config.HorizontalCharCount));
            Console.WriteLine("ImageVerticalCharLength: " + (int)(bitmap.Height / config.VerticalCharCount));
            Console.WriteLine("TotalCharCount: " + (config.HorizontalCharCount * config.VerticalCharCount));

            Console.WriteLine("DefaultHeader:");
            foreach (var item in config.DefaultHeader)
            {
                Console.WriteLine($"Header{item.Key}: " + item.Value.ToString("X8"));
            }
            Console.WriteLine("DefaultFontSpacing:");
            foreach (var item in config.DefaultFontSpacing)
            {
                Console.WriteLine("FontSpacing_" + (item.Key).ToString("X4") + "_StartPoint:" + item.Value.StartPoint.ToString("D1"));
                Console.WriteLine("FontSpacing_" + (item.Key).ToString("X4") + "_!End!Point:" + item.Value.EndPoint.ToString("D1"));
            }
        }

    }
}
