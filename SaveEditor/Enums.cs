using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor
{
    public enum Template : uint
    {
        [Description("None")] None = 0,
        [Description("Bit0")] Bit0 = 1 << 0,
        [Description("Bit1")] Bit1 = 1 << 1,
        [Description("Bit2")] Bit2 = 1 << 2,
        [Description("Bit3")] Bit3 = 1 << 3,
        [Description("Bit4")] Bit4 = 1 << 4,
        [Description("Bit5")] Bit5 = 1 << 5,
        [Description("Bit6")] Bit6 = 1 << 6,
        [Description("Bit7")] Bit7 = 1 << 7,
        [Description("Bit8")] Bit8 = 1 << 8,
        [Description("Bit9")] Bit9 = 1 << 9,
        [Description("Bit10")] Bit10 = 1 << 10,
        [Description("Bit11")] Bit11 = 1 << 11,
        [Description("Bit12")] Bit12 = 1 << 12,
        [Description("Bit13")] Bit13 = 1 << 13,
        [Description("Bit14")] Bit14 = 1 << 14,
        [Description("Bit15")] Bit15 = 1 << 15,
        [Description("Bit16")] Bit16 = 1 << 16,
        [Description("Bit17")] Bit17 = 1 << 17,
        [Description("Bit18")] Bit18 = 1 << 18,
        [Description("Bit19")] Bit19 = 1 << 19,
        [Description("Bit20")] Bit20 = 1 << 20,
        [Description("Bit21")] Bit21 = 1 << 21,
        [Description("Bit22")] Bit22 = 1 << 22,
        [Description("Bit23")] Bit23 = 1 << 23,
        [Description("Bit24")] Bit24 = 1 << 24,
        [Description("Bit25")] Bit25 = 1 << 25,
        [Description("Bit26")] Bit26 = 1 << 26,
        [Description("Bit27")] Bit27 = 1 << 27,
        [Description("Bit28")] Bit28 = 1 << 28,
        [Description("Bit29")] Bit29 = 1 << 29,
        [Description("Bit30")] Bit30 = 1 << 30,
        [Description("Bit31")] Bit31 = 1u << 31,
    }

    public enum enmCharacterFlags : uint
    {
        [Description("None")] None = 0,
        [Description("Is Available")] Bit0 = 1 << 0,
        [Description("Has Joined")] Bit1 = 1 << 1,
        [Description("Bit2")] Bit2 = 1 << 2,
        [Description("Is Dead")] Bit3 = 1 << 3,
        [Description("Bit4")] Bit4 = 1 << 4,
        [Description("Bit5")] Bit5 = 1 << 5,
        [Description("Bit6")] Bit6 = 1 << 6,
        [Description("Bit7")] Bit7 = 1 << 7,
        [Description("Bit8")] Bit8 = 1 << 8,
        [Description("Bit9")] Bit9 = 1 << 9,
        [Description("Bit10")] Bit10 = 1 << 10,
        [Description("Bit11")] Bit11 = 1 << 11,
        [Description("Bit12")] Bit12 = 1 << 12,
        [Description("Bit13")] Bit13 = 1 << 13,
        [Description("Bit14")] Bit14 = 1 << 14,
        [Description("Bit15")] Bit15 = 1 << 15,
        [Description("Bit16")] Bit16 = 1 << 16,
        [Description("Bit17")] Bit17 = 1 << 17,
        [Description("Is Deployed")] Bit18 = 1 << 18,
        [Description("Must take part in Battle")] Bit19 = 1 << 19,
        [Description("Is Adjutant")] Bit20 = 1 << 20,
        [Description("Bit21")] Bit21 = 1 << 21,
        [Description("Bit22")] Bit22 = 1 << 22,
        [Description("Bit23")] Bit23 = 1 << 23,
        [Description("Bit24")] Bit24 = 1 << 24,
        [Description("Bit25")] Bit25 = 1 << 25,
        [Description("Bit26")] Bit26 = 1 << 26,
        [Description("Bit27")] Bit27 = 1 << 27,
        [Description("Bit28")] Bit28 = 1 << 28,
        [Description("Bit29")] Bit29 = 1 << 29,
        [Description("Bit30")] Bit30 = 1 << 30,
        [Description("Bit31")] Bit31 = 1u << 31,
    }
    
    public enum enmCharacterClassFlags : byte
    {
        [Description("None")] None = 0,
        [Description("Bit0")] Bit0 = 1 << 0,
        [Description("Did Exam today")] Bit1 = 1 << 1,
        [Description("Was Praised today")] Bit2 = 1 << 2,
        [Description("Bit3")] Bit3 = 1 << 3,
        [Description("Bit4")] Bit4 = 1 << 4,
        [Description("Bit5")] Bit5 = 1 << 5,
        [Description("Bit6")] Bit6 = 1 << 6,
        [Description("Bit7")] Bit7 = 1 << 7,
    }

    public enum enmRank
    {
        [Description("E")] E = 0, 
        [Description("E+")] E2, 
        [Description("D")] D, 
        [Description("D+")] D2, 
        [Description("C")] C, 
        [Description("C+")] C2, 
        [Description("B")] B, 
        [Description("B+")] B2, 
        [Description("A")] A, 
        [Description("A+")] A2, 
        [Description("S")] S, 
        [Description("S+")] S2, 
    }

    public enum enmQuestState : byte
    {
        [Description("Unavailable")] Unavailable = 0, 
        [Description("Available")] Available, 
        [Description("Active")] Active, 
        [Description("Unk_0x03")] Unk_0x03, 
        [Description("Finished1")] Finished1,
        [Description("Unk_0x05")] Unk_0x05,
        [Description("Finished2")] Finished2,
    }

    public enum enmLanguage
    {
        [Description("Japanese")] jp = 0,
        [Description("English - USA")] en_u = 1, 
        [Description("English - EUR")] en_e = 2, 
        [Description("German")] de = 3,
        [Description("French - USA")] fr_u = 4,
        [Description("French - EUR")] fr_e = 5,
        [Description("Spanish - USA")] es_u = 6,
        [Description("Spanish - EUR")] es_e = 7,
        [Description("Italian")] it = 8,
        [Description("Korean")] kr = 9,
        [Description("Chinese")] chn = 10,
        [Description("Taiwanese")] twn = 11,
    }
    
    public enum enmItemTypes
    {
        Weapon = 0,
        Magic = 1,
        Object = 2,
        Special1 = 3,
        Special2 = 4,
        Accessory = 5,
        Consumeable = 6,

        MaxItemTypes,
    }

}
