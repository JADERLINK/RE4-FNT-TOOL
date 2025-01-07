using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_FNT_TOOL
{
    internal static class Extract
    {
        public static void Extract_All(FileInfo fileInfo, Endianness endianness) 
        {
            var br = new EndianBinaryReader(fileInfo.OpenRead(), endianness);

            uint Sprite_Sheet_Offset = br.ReadUInt32();
            uint Spacing_Offset = br.ReadUInt32();
            uint header2 = br.ReadUInt32();
            uint header3 = br.ReadUInt32();
            uint header4 = br.ReadUInt32();
            uint header5 = br.ReadUInt32();
            uint header6 = br.ReadUInt32();
            uint header7 = br.ReadUInt32();

            if (Sprite_Sheet_Offset != 0x20)
            {
                br.Close();
                throw new NotSupportedException("The magic must be 0x20");
            }

            br.BaseStream.Position = Sprite_Sheet_Offset;
            int TplLength = (int)(Spacing_Offset - Sprite_Sheet_Offset);
            byte[] TPL = br.ReadBytes(TplLength);

            List<(sbyte StartPoint, sbyte EndPoint)> FontSpacing = new List<(sbyte StartPoint, sbyte EndPoint)>();

            br.BaseStream.Position = Spacing_Offset;

            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                sbyte StartPoint = br.ReadSByte();
                sbyte EndPoint = br.ReadSByte();
                FontSpacing.Add((StartPoint, EndPoint));
            }
            br.Close();

            string directory = Path.GetDirectoryName(fileInfo.FullName);
            string fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);

            //tpl
            File.WriteAllBytes(Path.Combine(directory, fileName + ".TPL"), TPL);

            //idxfnt
            var idx = new FileInfo(Path.Combine(directory, fileName + ".idxfnt")).CreateText();

            idx.WriteLine("# RE4_FNT_TOOL");
            idx.WriteLine("# By: JADERLINK");
            idx.WriteLine("# youtube.com/@JADERLINK");
            idx.WriteLine("# " + MainAction.Version);
            idx.WriteLine();
            idx.WriteLine();
            idx.WriteLine("Header2:" + header2.ToString("X8"));
            idx.WriteLine("Header3:" + header3.ToString("X8"));
            idx.WriteLine("Header4:" + header4.ToString("X8"));
            idx.WriteLine("Header5:" + header5.ToString("X8"));
            idx.WriteLine("Header6:" + header6.ToString("X8"));
            idx.WriteLine("Header7:" + header7.ToString("X8"));
            idx.WriteLine();
            idx.WriteLine();

            for (int i = 0; i < FontSpacing.Count; i++)
            {
                idx.WriteLine("FontSpacing_" + (i + 0x80).ToString("X4") + "_StartPoint:" + FontSpacing[i].StartPoint.ToString("D1"));
                idx.WriteLine("FontSpacing_" + (i + 0x80).ToString("X4") + "_!End!Point:" + FontSpacing[i].EndPoint.ToString("D1"));
                idx.WriteLine();
            }
            idx.Close();
        }
    }
}
