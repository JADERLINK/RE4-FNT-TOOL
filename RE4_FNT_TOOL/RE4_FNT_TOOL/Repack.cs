using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleEndianBinaryIO;

namespace RE4_FNT_TOOL
{
    internal static class Repack
    {
        public static void Repack_All(FileInfo fileInfo, Endianness endianness) 
        {
            uint[] header = new uint[8];
            Dictionary<int, (sbyte StartPoint, sbyte EndPoint)> FontSpacing = new Dictionary<int, (sbyte StartPoint, sbyte EndPoint)>();

            //read idx
            var idx = fileInfo.OpenText();
            while (!idx.EndOfStream)
            {
                string line = idx.ReadLine().Trim().ToLowerInvariant();
                if (line.StartsWith("#") || line.StartsWith(":") || line.StartsWith("\\") || line.StartsWith("/"))
                {
                    continue;
                }

                if (line.StartsWith("header"))
                {
                    try
                    {
                        var split = line.Split(':');
                        var sId = split[0].Replace("header", "").Trim();
                        int id = int.Parse(sId, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                        var sValue = split[1].Trim();
                        uint value = uint.Parse(sValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                        header[id] = value;
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (line.StartsWith("fontspacing"))
                {
                    var split = line.Split(':');
                    var sprop = split[0].Trim();
                    var subsplit = sprop.Split('_');
                    var sId = subsplit[1].Trim();
                    int id = int.Parse(sId, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    var sValue = split[1].Trim();
                    sbyte value = sbyte.Parse(sValue, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

                    if (!FontSpacing.ContainsKey(id))
                    {
                        FontSpacing.Add(id, (0,0));
                    }

                    if (sprop.EndsWith("startpoint"))
                    {
                        var v = FontSpacing[id];
                        v.StartPoint = value;
                        FontSpacing[id] = v;
                    }
                    else if (sprop.EndsWith("!end!point"))
                    {
                        var v = FontSpacing[id];
                        v.EndPoint = value;
                        FontSpacing[id] = v;
                    }
                }
            }
            idx.Close();

            //add missing codes
            int lastCode = FontSpacing.Keys.OrderBy(x =>x).LastOrDefault();
            for (int i = 0x80; i < lastCode; i++)
            {
                if (!FontSpacing.ContainsKey(i))
                {
                    FontSpacing.Add(i,(0,0));
                }
            }

            //arr
            int index = 0;
            int arrLenght = (lastCode + 1 - 0x80) * 2;
            byte[] arr = new byte[arrLenght];
            for (int i = 0x80; i <= lastCode; i++)
            {
                arr[index] = (byte)FontSpacing[i].StartPoint;
                arr[index +1] = (byte)FontSpacing[i].EndPoint;
                index += 2;
            }

            //load tpl

            string directory = Path.GetDirectoryName(fileInfo.FullName);
            string fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);

            //tpl
            byte[] TPL = File.ReadAllBytes(Path.Combine(directory, fileName + ".TPL"));

            //create the file
            header[0] = 0x20;

            //alinhamento
            int val = 0x20 + TPL.Length;
            int div = val / 16;
            int rest = val % 16;
            div += rest != 0 ? 1 : 0;
            val = div * 16;
            header[1] = (uint)val;

            var bw = new EndianBinaryWriter(new FileInfo(Path.Combine(directory, fileName + ".FNT")).Create(), endianness);
            for (int i = 0; i < header.Length; i++)
            {
                bw.Write(header[i]);
            }
            bw.Write(TPL);

            bw.BaseStream.Position = val;
            bw.Write(arr);
            bw.Close();
        }
    }
}
