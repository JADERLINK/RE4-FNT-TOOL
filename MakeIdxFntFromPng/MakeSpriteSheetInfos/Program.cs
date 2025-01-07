using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using SharedCode;

namespace MakeSpriteSheetInfos
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("MakeSpriteSheetInfos");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine("Version 1.1 (2025-01-07)");
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
            MakeImageIDs(bitmap, config, pngFileInfo);
            MakeImageIDsDouble(bitmap, config, pngFileInfo);
        }

        static void MakeImageIDs(Bitmap bitmap, Config config, FileInfo pngFileInfo) 
        {
            int TotalCharCount = (config.HorizontalCharCount * config.VerticalCharCount);
            int ImageHorizontalCharLength = (int)(bitmap.Width / config.HorizontalCharCount);
            int ImageVerticalCharLength = (int)(bitmap.Height / config.VerticalCharCount);
            float AltProportion = (float)ImageHorizontalCharLength / 64;

            Bitmap part1 = new Bitmap(bitmap.Width, bitmap.Height);
            Graphics graphics1 = Graphics.FromImage(part1);
            Bitmap part2 = new Bitmap(bitmap.Width, bitmap.Height);
            Graphics graphics2 = Graphics.FromImage(part2);


            int CharX = 0;
            int CharY = 0;
            bool alt = false;

            for (int i = 0; i < TotalCharCount; i++)
            {
                Draw(graphics1, i + 0x80, CharX, CharY, ImageHorizontalCharLength, ImageVerticalCharLength, AltProportion, alt);
                Draw(graphics2, i + 0x80 + TotalCharCount, CharX, CharY, ImageHorizontalCharLength, ImageVerticalCharLength, AltProportion, alt);
                alt = !alt;

                CharX++;
                if (CharX >= config.HorizontalCharCount)
                {
                    CharX = 0;
                    CharY++;
                    alt = !alt;
                }
            }

            var png1 = Path.ChangeExtension(pngFileInfo.FullName, "IDs_Part1.png");
            var png2 = Path.ChangeExtension(pngFileInfo.FullName, "IDs_Part2.png");
            part1.Save(png1, System.Drawing.Imaging.ImageFormat.Png);
            part2.Save(png2, System.Drawing.Imaging.ImageFormat.Png);
        }

        private static void Draw(Graphics graphics, int ID, int CharX, int CharY,
         int ImageHorizontalCharLength, int ImageVerticalCharLength, float proportion, bool alt)
        {
            Brush background = Brushes.PowderBlue;
            if (alt)
            {
                background = Brushes.LightSkyBlue;
            }

            int ImageStartPosX = CharX * ImageHorizontalCharLength;
            int ImageStartPosY = CharY * ImageVerticalCharLength;

            graphics.FillRectangle(background, new Rectangle(ImageStartPosX, ImageStartPosY, ImageHorizontalCharLength, ImageVerticalCharLength));

            graphics.DrawString($"{ID.ToString("X3")}", new Font("Consolas", 20f * proportion), Brushes.Black, ImageStartPosX + (5 * proportion), ImageStartPosY + (15* proportion));
        }


        static void MakeImageIDsDouble(Bitmap bitmap, Config config, FileInfo pngFileInfo)
        {
            int TotalCharCount = (config.HorizontalCharCount * config.VerticalCharCount);
            int ImageHorizontalCharLength = (int)(bitmap.Width / config.HorizontalCharCount);
            int ImageVerticalCharLength = (int)(bitmap.Height / config.VerticalCharCount);
            float AltProportion = (float)ImageHorizontalCharLength / 64;

            Bitmap part1 = new Bitmap(bitmap.Width, bitmap.Height);
            Graphics graphics1 = Graphics.FromImage(part1);

            int CharX = 0;
            int CharY = 0;
            bool alt = false;

            for (int i = 0; i < TotalCharCount; i++)
            {
                DrawDouble(graphics1, i + 0x80, i + 0x80 + TotalCharCount, CharX, CharY, ImageHorizontalCharLength, ImageVerticalCharLength, AltProportion, alt);
                CharX++;
                if (CharX >= config.HorizontalCharCount)
                {
                    CharX = 0;
                    CharY++;
                    alt = !alt;
                }
            }

            var png1 = Path.ChangeExtension(pngFileInfo.FullName, "IDs_Double.png");
            part1.Save(png1, System.Drawing.Imaging.ImageFormat.Png);
        }

        private static void DrawDouble(Graphics graphics, int ID1, int ID2, int CharX, int CharY,
       int ImageHorizontalCharLength, int ImageVerticalCharLength, float proportion, bool alt)
        {
            Brush background = Brushes.PowderBlue;
            Brush background2 = Brushes.LightSkyBlue;
            if (alt)
            {
                background = Brushes.LightSkyBlue;
                background2 = Brushes.PowderBlue;
            }

            int ImageStartPosX = CharX * ImageHorizontalCharLength;
            int ImageStartPosX2 = CharX * ImageHorizontalCharLength + ImageHorizontalCharLength / 2;
            int ImageStartPosY = CharY * ImageVerticalCharLength;

            graphics.FillRectangle(background, new Rectangle(ImageStartPosX, ImageStartPosY, ImageHorizontalCharLength, ImageVerticalCharLength));
            graphics.FillRectangle(background2, new Rectangle(ImageStartPosX2, ImageStartPosY, ImageHorizontalCharLength / 2, ImageVerticalCharLength));

            string sID1 = string.Join("\n", ID1.ToString("X3").ToArray());
            string sID2 = string.Join("\n", ID2.ToString("X3").ToArray());

            graphics.DrawString($"{sID1}", new Font("Consolas", 13f * proportion), Brushes.Black, ImageStartPosX + (8 * proportion), ImageStartPosY);
            graphics.DrawString($"{sID2}", new Font("Consolas", 13f * proportion), Brushes.Black, ImageStartPosX2 + (8 * proportion), ImageStartPosY);
        }
    }
}
