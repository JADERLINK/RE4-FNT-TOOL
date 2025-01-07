using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_FNT_TOOL_LITTLE_ENDIAN
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine("RE4_FNT_TOOL_LITTLE_ENDIAN");
            Console.WriteLine("by: JADERLINK");
            Console.WriteLine("youtube.com/@JADERLINK");
            Console.WriteLine(RE4_FNT_TOOL.MainAction.Version);
            Console.WriteLine("");

            RE4_FNT_TOOL.MainAction.Continue(args, SimpleEndianBinaryIO.Endianness.LittleEndian);
        }
    }
}
