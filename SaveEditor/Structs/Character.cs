using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SaveEditor.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct CharacterPart1
    {
        public const int SIZE = 0x1B0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_CHARA_ITEMS)]
        public Item[] Items; //0x0

        public Battalion EquippedBattalion; //0x18

        public uint RNG_VALUE;  //0x20 - 82 90 6C 85
        public short Id; //0x24
        public short field_26; //0x26 - 16 00
        public short field_28; //0x28 - 0C 00
        public byte field_2A; //0x2A - FF
        public byte field_2B; //0x2B - FF

        public ushort Exp; //0x2C

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public short[] EquippedItem; //0x2E

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_SKILLS)]
        public ushort[] SkillExp; //0x32

        public ushort CurrentClassExp; //0x48
        public byte Level, Class, HP, field_4D; //0x4A - 0x4D
        public byte Strength, Magic, Dexterity, Speed, Luck, Defense, Resistance, Movement, Charm; //0x4E - 0x56

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] CombatArts; //0x57

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_ABILITIES)]
        public byte[] Abilities; //0x61
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] EquippedAbilities; //0x7F

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] EquippedCombatArts; //0x84

        public byte ItemCount; //0x87

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_SKILLS)]
        public byte[] SkillLevel; //0x88

        public byte CurrentClassLevel; //0x93
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_MAGIC)]
        public byte[] MagicDurability; //0x94

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_MAGIC)]
        public byte[] LearnedMagic; //0xA0

        public uint Flags; //0xAC

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] field_B0; //0xB0 - 00 00 00 00 05 FF FF FF 00 00 04 00 04 00 00 00 00 00 00 00

        public byte Motivation; //0xC4

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public byte[] field_C5; //00 00 00 00 00 00 00 00 00 00 00

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] field_D0; //00 08 0B

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_CLASS_FLAGS)]
        public byte[] ClassUnlockFlags; //0xD3

        public byte LearningFocus; //0xDB

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)] 
        public byte[] field_DC; //0xDC, FF 00 00

        public byte ClassFlags; //0xDF

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] field_E0; //00 00 00 00 00 00 00 00 00 00 

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] field_EA; //00 00 00 00 00 00 00 00 00 00 

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] field_F4; //00 00 00 00 00 00

        public short AdjutantId; //0xFA


        //the game uses a total of 101 slots for the exp combined, but we only need skills and the first 64 classes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_SKILLS)]
        public ushort[] SkillExp2; //0xFC
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_CLASSES)]
        public ushort[] ClassExp; //0x112

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        public ushort[] ClassExp2; //0x18A, ClassExp continued, but we can ignore it.

        
        public string GenerateDebugOut(bool DumpAllValues = false)
        {
            string result = "";

            result += "Part1:\r\n";

            if (DumpAllValues) result += $"Battalion: {EquippedBattalion}\r\n";
            if (DumpAllValues) result += $"RNG_VALUE: {RNG_VALUE:X8}\r\n";
            result += $"field_26: {field_26}, field_28: {field_28}, field_2A: {field_2A}, field_2B: {field_2B}\r\n";

            if (DumpAllValues) result += $"Exp: {Exp}\r\n";

            if (DumpAllValues) result += $"EquippedItems: {EquippedItem[0]}, {EquippedItem[1]}\r\n";

            if (DumpAllValues) result += $"SkillExp: {Util.Array2String(SkillExp, " ")}\r\n";
            if (DumpAllValues) result += $"CurrentClassExp: {CurrentClassExp}\r\n";
            if (DumpAllValues) result += $"Level: {Level}\r\n";
            if (DumpAllValues) result += $"HP: {HP}\r\n";

            result += $"field_4D: {field_4D}\r\n";

            if (DumpAllValues) result += $"Strength: {Strength}\r\n";
            if (DumpAllValues) result += $"Magic: {Magic}\r\n";
            if (DumpAllValues) result += $"Dexterity: {Dexterity}\r\n";
            if (DumpAllValues) result += $"Speed: {Speed}\r\n";
            if (DumpAllValues) result += $"Luck: {Luck}\r\n";
            if (DumpAllValues) result += $"Defense: {Defense}\r\n";
            if (DumpAllValues) result += $"Resistance: {Resistance}\r\n";
            if (DumpAllValues) result += $"Movement: {Movement}\r\n";
            if (DumpAllValues) result += $"Charm: {Charm}\r\n";
            if (DumpAllValues) result += $"BattleSkills: {Util.Array2String(CombatArts, " ")}\r\n";
            if (DumpAllValues) result += $"Perks: {Util.Array2String(Abilities, " ")}\r\n";
            if (DumpAllValues) result += $"EquippedPerks: {Util.Array2String(EquippedAbilities, " ")}\r\n";
            if (DumpAllValues) result += $"EquippedBattleSkills: {Util.Array2String(EquippedCombatArts, " ")}\r\n";
            if (DumpAllValues) result += $"ItemCount: {ItemCount}\r\n";
            if (DumpAllValues) result += $"SkillRanks: {Util.Array2String(SkillLevel, " ")}\r\n";
            if (DumpAllValues) result += $"CurrentClassLevel: {CurrentClassLevel}\r\n";
            if (DumpAllValues) result += $"MagicDurability: {Util.Array2String(MagicDurability, " ")}\r\n";
            if (DumpAllValues) result += $"LearnedMagic: {Util.Array2String(LearnedMagic, " ")}\r\n";
            if (DumpAllValues) result += $"Flags: 0x{Flags:X8}\r\n";

            result += $"field_B0: {Util.Array2String(field_B0, " ")}\r\n";

            if (DumpAllValues) result += $"Motivation: {Motivation}\r\n";

            result += $"field_C5: {Util.Array2String(field_C5, " ")}\r\n";
            result += $"field_D0: {Util.Array2String(field_D0, " ")}\r\n";

            if (DumpAllValues) result += $"ClassUnlockFlags: {Util.Array2String(ClassUnlockFlags, " ")}\r\n";
            if (DumpAllValues) result += $"LearningFocus: {LearningFocus}\r\n";

            result += $"field_DC: {Util.Array2String(field_DC, " ")}\r\n";

            if (DumpAllValues) result += $"ClassFlags: {ClassFlags}\r\n";
            
            result += $"field_E0: {Util.Array2String(field_E0, " ")}\r\n";
            result += $"field_EA: {Util.Array2String(field_EA, " ")}\r\n";
            result += $"field_F4: {Util.Array2String(field_F4, " ")}\r\n";
            
            if (DumpAllValues) result += $"AdjutantId: {AdjutantId}\r\n";
            if (DumpAllValues) result += $"SkillExp2: {Util.Array2String(SkillExp2, " ")}\r\n";
            if (DumpAllValues) result += $"ClassExp: {Util.Array2String(ClassExp, " ")}\r\n";
            if (DumpAllValues) result += $"ClassExp2: {Util.Array2String(ClassExp2, " ")}\r\n";

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct CharacterPart2
    {
        public const int SIZE = 0x80;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
        public ushort[] ClassExp3; //0x0, ClassExp continued, but we can ignore it.

        public byte DeployIndex; //0x16
        public byte DeploySlot; //0x17

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_SKILLS)]
        public byte[] SkillLevel2; //0x18

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_CLASSES)]
        public byte[] ClassLevel; //0x23
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public byte[] ClassLevel2; //0x5F, ClassLevel continued, but we can ignore it.
            
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] field_7D; //0x7D

        public string GenerateDebugOut(bool DumpAllValues = false)
        {
            string result = "";
            
            result += "Part2:\r\n";

            result += $"field_0: {Util.Array2String(ClassExp3, " ")}\r\n";
            result += $"DeployIndex: {DeployIndex}, DeploySlot: {DeploySlot}\r\n";
            if (DumpAllValues) result += $"SkillLevel2: {Util.Array2String(SkillLevel2, " ")}\r\n";
            if (DumpAllValues) result += $"ClassLevel: {Util.Array2String(ClassLevel, " ")}\r\n";
            if (DumpAllValues) result += $"ClassLevel2: {Util.Array2String(ClassLevel2, " ")}\r\n";
            result += $"field_7D: {Util.Array2String(field_7D, " ")}\r\n";

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct Character
    {
        public const int SIZE = 0x230;
        public const int SIZE_MEMORY = 0xC2;

        public CharacterPart1 p1;
        public CharacterPart2 p2;

        public short Id { get => p1.Id; set => p1.Id = value; }
        public byte Class { get => p1.Class; set => p1.Class = value; }


        public void Init()
        {
            p1.Items = new Item[Database.MAX_CHARA_ITEMS]; 
            for (int i = 0; i < Database.MAX_CHARA_ITEMS; i++) p1.Items[i].Init();

            p1.EquippedBattalion = new Battalion();
            p1.EquippedBattalion.Init();

            p1.RNG_VALUE = 0;
            p1.Id = -1;
            p1.field_26 = 0;
            p1.field_28 = 0;
            p1.field_2A = 0xFF;
            p1.field_2B = 0xFF;
            p1.Exp = 0;
            p1.EquippedItem = new short[] {-1, -1};
            p1.SkillExp = new ushort[Database.MAX_SKILLS]; 
            for (int i = 0; i < Database.MAX_SKILLS; i++) p1.SkillExp[i] = 0;
            p1.CurrentClassExp = 0;
            p1.Level = 0;
            p1.Class = 0;
            p1.HP = 0;
            p1.field_4D = 0;
            p1.Strength = 0;
            p1.Magic = 0;
            p1.Dexterity = 0;
            p1.Speed = 0;
            p1.Luck = 0;
            p1.Defense = 0;
            p1.Resistance = 0;
            p1.Movement = 0;
            p1.Charm = 0;
            p1.CombatArts = new byte[10]; for (int i = 0; i < 10; i++) p1.CombatArts[i] = 0;
            p1.Abilities = new byte[Database.MAX_ABILITIES]; 
            for (int i = 0; i < Database.MAX_ABILITIES; i++) p1.Abilities[i] = 0;
            p1.EquippedAbilities = new byte[Database.MAX_CHARA_ABILITIES];
            for (int i = 0; i < Database.MAX_CHARA_ABILITIES; i++) p1.EquippedAbilities[i] = Database.ABILITY_COUNT;
            p1.EquippedCombatArts = new byte[Database.MAX_CHARA_COMBAT_ARTS]; 
            for (int i = 0; i < Database.MAX_CHARA_COMBAT_ARTS; i++) p1.EquippedCombatArts[i] = Database.COMBAT_ARTS_COUNT;
            p1.ItemCount = 0;
            p1.SkillLevel = new byte[Database.MAX_SKILLS]; 
            for (int i = 0; i < Database.MAX_SKILLS; i++) p1.SkillLevel[i] = 0;
            p1.CurrentClassLevel = 0;
            p1.MagicDurability = new byte[Database.MAX_MAGIC]; 
            for (int i = 0; i < Database.MAX_MAGIC; i++) p1.MagicDurability[i] = 0;
            p1.LearnedMagic = new byte[Database.MAX_MAGIC]; 
            for (int i = 0; i < Database.MAX_MAGIC; i++) p1.LearnedMagic[i] = Database.MAGIC_COUNT;
            p1.Flags = 0xF0000000;
            p1.field_B0 = new byte[20]; 
            for (int i = 0; i < 20; i++) p1.field_B0[i] = 0;
            p1.Motivation = 50;
            p1.field_C5 = new byte[11]; 
            for (int i = 0; i < 11; i++) p1.field_C5[i] = 0;
            p1.field_D0 = new byte[3]; 
            for (int i = 0; i < 3; i++) p1.field_D0[i] = 0;
            p1.ClassUnlockFlags = new byte[Database.MAX_CLASS_FLAGS]; 
            for (int i = 0; i < Database.MAX_CLASS_FLAGS; i++) p1.ClassUnlockFlags[i] = 0;
            p1.field_DC = new byte[3]; 
            for (int i = 0; i < 3; i++) p1.field_DC[i] = 0;
            p1.LearningFocus = 0;
            p1.ClassFlags = 0;
            p1.field_E0 = new byte[10]; 
            for (int i = 0; i < 10; i++) p1.field_E0[i] = 0;
            p1.field_EA = new byte[10]; 
            for (int i = 0; i < 10; i++) p1.field_EA[i] = 0;
            p1.field_F4 = new byte[6]; 
            for (int i = 0; i < 6; i++) p1.field_F4[i] = 0;
            p1.AdjutantId = -1;
            p1.SkillExp2 = new ushort[Database.MAX_SKILLS]; 
            for (int i = 0; i < Database.MAX_SKILLS; i++) p1.SkillExp2[i] = 0;
            p1.ClassExp = new ushort[Database.MAX_CLASSES]; 
            for (int i = 0; i < Database.MAX_CLASSES; i++) p1.ClassExp[i] = 0;
            p1.ClassExp2 = new ushort[19]; 
            for (int i = 0; i < 19; i++) p1.ClassExp2[i] = 0;

            //part2:
            p2.ClassExp3 = new ushort[11]; 
            for (int i = 0; i < 11; i++) p2.ClassExp3[i] = 0;
            p2.DeployIndex = 0xFF;
            p2.DeploySlot = 0xFF;
            p2.SkillLevel2 = new byte[Database.MAX_SKILLS]; 
            for (int i = 0; i < Database.MAX_SKILLS; i++) p2.SkillLevel2[i] = 0;
            p2.ClassLevel = new byte[Database.MAX_CLASSES]; 
            for (int i = 0; i < Database.MAX_CLASSES; i++) p2.ClassLevel[i] = 0;
            p2.ClassLevel2 = new byte[30]; 
            for (int i = 0; i < 30; i++) p2.ClassLevel2[i] = 0;
            p2.field_7D = new byte[3]; 
            for (int i = 0; i < 3; i++) p2.field_7D[i] = 0;


        }

        public static Character LoadMemoryCharacter(byte[] data)
        {
            //when a character is send into a battle, the first 0xC2 byte are copied into a 105 slot battle character table

            Character temp = new Character();
            temp.Init();
            byte[] empty = Util.StructureToByteArray(temp);
            data.CopyTo(empty, 0);
            temp = Util.ReadStructure<Character>(empty);


            //generate some easy stuff
            for (int i = 0; i < Database.MAX_SKILLS; i++)
            {
                temp.p1.SkillExp2[i] = temp.p1.SkillExp[i];
                temp.p2.SkillLevel2[i] = temp.p1.SkillLevel[i];
            }

            temp.p2.ClassLevel[temp.Class] = temp.p1.CurrentClassLevel;
            temp.p1.ClassExp[temp.Class] = temp.p1.CurrentClassExp;

            return temp;
        }

        public override string ToString()
        {
            //return $"{Database.GetUnitName(p1.Id)} - Lv:{p1.Level}, Job:{Database.GetClassName(p1.Job)}";
            return $"{Database.GetUnitName(p1.Id)} - Lv:{p1.Level}";
        }

        public string GenerateDebugOut()
        {
            string result = "";

            return result;
        }

    }
}
