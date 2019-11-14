using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveEditor
{
    public static class MemoryHelper
    {
        private const ulong MainAddress = 0x7616C04000;
        private const ulong SaveDataPtr_v100 = 0x19CF6E0;
        private const ulong SaveDataPtr_v101 = 0x19D76F0;
        private const ulong BattlePtr_v100 = 0x0;
        private const ulong BattlePtr_v101 = 0x1988EF0;

        public static void GetMemoryAddresses()
        {
            string result = "";

            result += $"SavePointer: {MainAddress + SaveDataPtr_v101:X}\r\n";
            result += $"BattlePointer: {MainAddress + BattlePtr_v101:X}\r\n";
            result += $"TeachingMotivationCost: {MainAddress + 0x18EDAE0:X}\r\n";
            result += $"TeachingActivityPointCost: {MainAddress + 0x18ED8C8:X}\r\n";
            result += $"InitMotivation: {MainAddress + 0x18EDAEC:X}\r\n";

            result += $"Exp Address: {MainAddress + 0x3BC418:X}\r\n";
            result += $"Skill Exp Address: {MainAddress + 0x3BC63C:X},{MainAddress + 0x38D434:X}\r\n";
            result += $"Stats Address1: {MainAddress + 0x38CE08:X},{MainAddress + 0x38CF94:X}\r\n";
            result += $"Stats Address1: {MainAddress + 0x38C1F0:X},{MainAddress + 0x38C3F0:X}\r\n";

            result += GetCharacterAddresses(MainAddress + 0x19D76F0 + 0x10);

            Clipboard.SetText(result);
        }

        private static string GetCharacterAddresses(ulong MainPointer)
        {
            ulong baseAddress = MainPointer + 0x640;

            string result = "";

            for (int i = 0; i < 35; i++)
            {
                result += $"{baseAddress + (uint) (i * 0x230):X} - {Database.GetUnitName(i + 1)}";
                result += $", EXP: {baseAddress + (uint) (i * 0x230) + 0x30:X}";
                result += $", Motivation: {baseAddress + (uint) (i * 0x230) + 0xC8:X}\r\n";
            }

            return result;
        }


    }
}
