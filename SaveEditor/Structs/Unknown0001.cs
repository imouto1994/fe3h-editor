using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct Unknown0001
    {
        public const int SIZE = 0xD0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xD0)]
        public byte[] Data;
             
        public string GenerateDebugOut()
        {
            string result = "";

            result += $"Unknown0001:\r\n{Util.Array2String(Data, " ")}\r\n";

            return result;
        }

    }
}
