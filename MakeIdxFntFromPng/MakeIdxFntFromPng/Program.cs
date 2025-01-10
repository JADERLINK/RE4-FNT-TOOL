using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using SharedCode;

namespace MakeIdxFntFromPng
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("MakeIdxFntFromPng");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("Version 1.2 (2025-01-10)");
            Console.WriteLine("");

            try
            {
                Shared.Continue(args, Continue2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }  
            
            Console.WriteLine("Finished!!!");
            Console.WriteLine("Press any key to close the console.");
            Console.ReadKey();
        }

        static void Continue2(Bitmap bitmap, Config config, FileInfo pngFileInfo) 
        {
            Shared.Part2Info(bitmap, config);
            Console.WriteLine("Processing data!");
            var FontSpacing = Part3New(bitmap, config);
            Part4Fix(ref FontSpacing, config);
            Part5NewFile(FontSpacing, config.DefaultHeader, pngFileInfo);
            PngCheckFile(FontSpacing, bitmap, config, pngFileInfo);
        }

        private static Dictionary<int, (sbyte StartPoint, sbyte EndPoint)> Part3New(Bitmap bitmap, Config config)
        {
            int TotalCharCount = (config.HorizontalCharCount * config.VerticalCharCount);

            Dictionary<int, (sbyte StartPoint, sbyte EndPoint)> FontSpacing = new Dictionary<int, (sbyte StartPoint, sbyte EndPoint)>();

            int CharX = 0;
            int CharY = 0;

            for (int i = 0; i < TotalCharCount; i++)
            {
                var calc = Calc(bitmap, config, CharX, CharY);
                FontSpacing.Add(i + 0x80, calc);

                CharX++;
                if (CharX >= config.HorizontalCharCount)
                {
                    CharX = 0;
                    CharY++;
                }
            }

            FontSpacing.Add(TotalCharCount + 0x80, (0, (sbyte)config.BaseHorizontalCharLength));
            return FontSpacing;
        }

        private static (sbyte StartPoint, sbyte EndPoint) Calc(Bitmap bitmap, Config config, int CharX, int CharY) 
        {
            int ImageHorizontalCharLength = (int)(bitmap.Width / config.HorizontalCharCount);
            int ImageVerticalCharLength = (int)(bitmap.Height / config.VerticalCharCount);

            int ImageStartPosX = CharX * ImageHorizontalCharLength;
            int ImageStartPosY = CharY * ImageVerticalCharLength;

            //start //left
            int leftMargin = 0;
            bool _break = false;
            bool isEmptyChar = false;
            for (int x = 0; x < ImageHorizontalCharLength; x++)
            {
                for (int y = 0; y < ImageVerticalCharLength; y++)
                {
                    Color color = bitmap.GetPixel(ImageStartPosX + x, ImageStartPosY + y);

                    if (color.A > config.CheckAlpha)
                    {
                        _break = true;
                        break;
                    }

                }
                if (_break)
                {
                    break;
                }

                leftMargin++;
            }

            if (!_break)
            {
                leftMargin++;
                isEmptyChar = true;
            }

            leftMargin -= config.ExtraLeftMargin;
            if (leftMargin < 0)
            {
                leftMargin = 0;
            }

            //right
            int rightMargin = 0;
            _break = false;

            for (int x = ImageHorizontalCharLength -1; x >= 0; x--)
            {
                for (int y = ImageVerticalCharLength -1; y >= 0 ; y--)
                {
                    Color color = bitmap.GetPixel(ImageStartPosX + x, ImageStartPosY + y);

                    if (color.A > config.CheckAlpha)
                    {
                        _break = true;
                        break;
                    }

                }
                if (_break)
                {
                    break;
                }

                rightMargin++;
            }

            rightMargin -= config.ExtraRightMargin;
            if (rightMargin < 0)
            {
                rightMargin = 0;
            }

            // endPoint fild
            int _length = ImageHorizontalCharLength - rightMargin - leftMargin;
            int endPoint = ImageHorizontalCharLength - rightMargin;
            if (endPoint < 0)
            {
                endPoint = 0;
            }
            if (isEmptyChar)
            {
                endPoint = leftMargin;
                leftMargin = 0;
            }
            else if (_length < config.MinimumCharLength)
            {
                rightMargin -= (config.MinimumCharLength - _length) / 2;
                leftMargin -= (config.MinimumCharLength - _length) / 2;
                if (leftMargin < 0)
                {
                    rightMargin += leftMargin;
                    leftMargin = 0;
                }
                if (rightMargin < 0)
                {
                    rightMargin = 0;
                }
                endPoint = ImageHorizontalCharLength - rightMargin;
            }

            // proporção
            float proportion = config.BaseHorizontalCharLength / (float)ImageHorizontalCharLength;
            endPoint = (int)Math.Round(endPoint * proportion, 0);
            leftMargin = (int)Math.Round(leftMargin * proportion, 0);

            return ((sbyte)leftMargin, (sbyte)endPoint);
        }

        private static void Part4Fix(ref Dictionary<int, (sbyte StartPoint, sbyte EndPoint)> FontSpacing, Config config) 
        {
            foreach (var item in config.DefaultFontSpacing)
            {
                if (FontSpacing.ContainsKey(item.Key))
                {
                    FontSpacing[item.Key] = item.Value;
                }
            }
        }

        private static void Part5NewFile(Dictionary<int, (sbyte StartPoint, sbyte EndPoint)> FontSpacing,Dictionary<int, uint> DefaultHeader, FileInfo pngFileInfo)
        {
            var idxfnt = Path.ChangeExtension(pngFileInfo.FullName, "idxfnt");

            var idx = new FileInfo(idxfnt).CreateText();

            idx.WriteLine("# MakeIdxFntFromPng");
            idx.WriteLine("# By: JADERLINK");
            idx.WriteLine("# youtube.com/@JADERLINK");
            idx.WriteLine();
            idx.WriteLine();

            if (DefaultHeader.Count != 0)
            {
                foreach (var item in DefaultHeader)
                {
                    idx.WriteLine($"Header{item.Key}:" + item.Value.ToString("X8"));
                }
                idx.WriteLine();
                idx.WriteLine();
            }

            foreach (var item in FontSpacing)
            {
                idx.WriteLine("FontSpacing_" + (item.Key).ToString("X4") + "_StartPoint:" + item.Value.StartPoint.ToString("D1"));
                idx.WriteLine("FontSpacing_" + (item.Key).ToString("X4") + "_!End!Point:" + item.Value.EndPoint.ToString("D1"));
                idx.WriteLine();
            }

            idx.Close();
        }

        private static void PngCheckFile(Dictionary<int, (sbyte StartPoint, sbyte EndPoint)> FontSpacing, Bitmap bitmap, Config config, FileInfo pngFileInfo) 
        {
            int TotalCharCount = (config.HorizontalCharCount * config.VerticalCharCount);
            int ImageHorizontalCharLength = (int)(bitmap.Width / config.HorizontalCharCount);
            int ImageVerticalCharLength = (int)(bitmap.Height / config.VerticalCharCount);
            float proportion = (float)ImageHorizontalCharLength / config.BaseHorizontalCharLength;

            Bitmap check = new Bitmap(bitmap.Width, bitmap.Height);
            Graphics graphics = Graphics.FromImage(check);

            int CharX = 0;
            int CharY = 0;
            bool alt = false;

            for (int i = 0; i < TotalCharCount; i++)
            {
                Draw(graphics, FontSpacing[i + 0x80], CharX, CharY, ImageHorizontalCharLength, ImageVerticalCharLength, proportion, alt);
                alt = !alt;

                CharX++;
                if (CharX >= config.HorizontalCharCount)
                {
                    CharX = 0;
                    CharY++;
                    alt = !alt;
                }
            }

            var pngcheck = Path.ChangeExtension(pngFileInfo.FullName, "check.png");
            check.Save(pngcheck, System.Drawing.Imaging.ImageFormat.Png);

        }

        private static void Draw(Graphics graphics, (sbyte StartPoint, sbyte EndPoint) Spacing, int CharX, int CharY,
          int ImageHorizontalCharLength, int ImageVerticalCharLength, float proportion, bool alt)
        {
            Brush background = Brushes.PowderBlue;
            Brush front = Brushes.Salmon;
            if (alt)
            {
                 background = Brushes.LightSkyBlue;
                 front = Brushes.SandyBrown;
            }

            int ImageStartPosX = CharX * ImageHorizontalCharLength;
            int ImageStartPosY = CharY * ImageVerticalCharLength;

            graphics.FillRectangle(background, new Rectangle(ImageStartPosX, ImageStartPosY, ImageHorizontalCharLength, ImageVerticalCharLength));
            
            int StartPoint = (int)Math.Round(Spacing.StartPoint * proportion, 0); 
            int Length = (int)Math.Round(Spacing.EndPoint * proportion, 0) - StartPoint;
            
            graphics.FillRectangle(front, new Rectangle(ImageStartPosX + StartPoint, ImageStartPosY, Length, ImageVerticalCharLength));
        }

    }
}
