using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SharedCode
{
    internal class Config
    {
        public int HorizontalCharCount;
        public int VerticalCharCount;
        public int BaseHorizontalCharLength;
        public int ExtraLeftMargin;
        public int ExtraRightMargin;
        public int MinimumCharLength;
        public int CheckAlpha;

        public Dictionary<int, (sbyte StartPoint, sbyte EndPoint)> DefaultFontSpacing { get; }
        public Dictionary<int, uint> DefaultHeader { get; }

        public Config()
        {
            DefaultFontSpacing = new Dictionary<int, (sbyte StartPoint, sbyte EndPoint)>();
            DefaultHeader = new Dictionary<int, uint>();
        }

        public static Config GetConfig(FileInfo configFileInfo)
        {
            Config config = new Config();

            var idx = configFileInfo.OpenText();
            while (!idx.EndOfStream)
            {
                string line = idx.ReadLine().Trim().ToLowerInvariant();
                if ((line.Length == 0
                        || line.StartsWith("#")
                        || line.StartsWith("\\")
                        || line.StartsWith("/")
                        || line.StartsWith(":")
                        ))
                {
                    continue;
                }

                var split = line.Split(':');
                if (split[0] == "horizontalcharcount")
                {
                    config.HorizontalCharCount = int.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (split[0] == "verticalcharcount")
                {
                    config.VerticalCharCount = int.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (split[0] == "basehorizontalcharlength")
                {
                    config.BaseHorizontalCharLength = int.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (split[0] == "extraleftmargin")
                {
                    config.ExtraLeftMargin = int.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (split[0] == "extrarightmargin")
                {
                    config.ExtraRightMargin = int.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (split[0] == "minimumcharlength")
                {
                    config.MinimumCharLength = int.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (split[0] == "checkalpha")
                {
                    config.CheckAlpha = int.Parse(split[1], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (line.StartsWith("header"))
                {
                    try
                    {
                        var sId = split[0].Replace("header", "").Trim();
                        int id = int.Parse(sId, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                        var sValue = split[1].Trim();
                        uint value = uint.Parse(sValue, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                        if (!config.DefaultHeader.ContainsKey(id))
                        {
                            config.DefaultHeader.Add(id, value);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                else if (line.StartsWith("fontspacing"))
                {
                    var sprop = split[0].Trim();
                    var subsplit = sprop.Split('_');
                    var sId = subsplit[1].Trim();
                    int id = int.Parse(sId, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    var sValue = split[1].Trim();
                    sbyte value = sbyte.Parse(sValue, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);

                    if (!config.DefaultFontSpacing.ContainsKey(id))
                    {
                        config.DefaultFontSpacing.Add(id, (0, 0));
                    }

                    if (sprop.EndsWith("startpoint"))
                    {
                        var v = config.DefaultFontSpacing[id];
                        v.StartPoint = value;
                        config.DefaultFontSpacing[id] = v;
                    }
                    else if (sprop.EndsWith("!end!point"))
                    {
                        var v = config.DefaultFontSpacing[id];
                        v.EndPoint = value;
                        config.DefaultFontSpacing[id] = v;
                    }
                }
            }
            idx.Close();

            return config;
        }

    }
}
