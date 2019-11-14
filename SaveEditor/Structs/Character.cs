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
    public struct CharacterDataPart1
    {
        public const int SIZE = 0x1B0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct CharacterData1000
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
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_CLASS)]
        public ushort[] ClassExp; //0x112

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public ushort[] ClassExp2; //0x18A, ClassExp continued, but we can ignore it.

        public byte DeployIndex; //0x1A4
        public byte DeploySlot; //0x1A5

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_SKILLS)]
        public byte[] SkillLevel2; //0x18

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_CLASS)]
        public byte[] ClassLevel; //0x23
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
        public byte[] ClassLevel2; //0x5F, ClassLevel continued, but we can ignore it.
            
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] field_7D; //0x7D

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
                        
            result += "Part2:\r\n";

            result += $"DeployIndex: {DeployIndex}, DeploySlot: {DeploySlot}\r\n";
            if (DumpAllValues) result += $"SkillLevel2: {Util.Array2String(SkillLevel2, " ")}\r\n";
            if (DumpAllValues) result += $"ClassLevel: {Util.Array2String(ClassLevel, " ")}\r\n";
            if (DumpAllValues) result += $"ClassLevel2: {Util.Array2String(ClassLevel2, " ")}\r\n";
            result += $"field_7D: {Util.Array2String(field_7D, " ")}\r\n";

            return result;
        }
    }
        
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct CharacterData
    {
        public const int SIZE = 0x24C;

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
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_CLASS)]
        public ushort[] ClassExp; //0x112

        public byte DeployIndex; //0x1A4
        public byte DeploySlot; //0x1A5

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_SKILLS)]
        public byte[] SkillLevel2; //0x18

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_CLASS)]
        public byte[] ClassLevel; //0x23

        public string GenerateDebugOut(bool DumpAllValues = false)
        {
            string result = "";

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
     
            result += $"DeployIndex: {DeployIndex}, DeploySlot: {DeploySlot}\r\n";
            if (DumpAllValues) result += $"SkillLevel2: {Util.Array2String(SkillLevel2, " ")}\r\n";
            if (DumpAllValues) result += $"ClassLevel: {Util.Array2String(ClassLevel, " ")}\r\n";

            return result;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct CharacterV1000
    {
        public const int SIZE = 0x230; //old: 0x230
        public const int SIZE_MEMORY = 0xC2;

        public CharacterData1000 data;

        public short Id { get => data.Id; set => data.Id = value; }
        public byte Class { get => data.Class; set => data.Class = value; }

        public void Init()
        {
            data.Items = new Item[Database.MAX_CHARA_ITEMS]; 
            for (int i = 0; i < Database.MAX_CHARA_ITEMS; i++) data.Items[i].Init();

            data.EquippedBattalion = new Battalion();
            data.EquippedBattalion.Init();

            data.RNG_VALUE = 0;
            data.Id = -1;
            data.field_26 = 0;
            data.field_28 = 0;
            data.field_2A = 0xFF;
            data.field_2B = 0xFF;
            data.Exp = 0;
            data.EquippedItem = new short[] {-1, -1};
            data.SkillExp = new ushort[Database.MAX_SKILLS]; 
            for (int i = 0; i < Database.MAX_SKILLS; i++) data.SkillExp[i] = 0;
            data.CurrentClassExp = 0;
            data.Level = 0;
            data.Class = 0;
            data.HP = 0;
            data.field_4D = 0;
            data.Strength = 0;
            data.Magic = 0;
            data.Dexterity = 0;
            data.Speed = 0;
            data.Luck = 0;
            data.Defense = 0;
            data.Resistance = 0;
            data.Movement = 0;
            data.Charm = 0;
            data.CombatArts = new byte[10]; for (int i = 0; i < 10; i++) data.CombatArts[i] = 0;
            data.Abilities = new byte[Database.MAX_ABILITIES]; 
            for (int i = 0; i < Database.MAX_ABILITIES; i++) data.Abilities[i] = 0;
            data.EquippedAbilities = new byte[Database.MAX_CHARA_ABILITIES];
            for (int i = 0; i < Database.MAX_CHARA_ABILITIES; i++) data.EquippedAbilities[i] = 240;
            data.EquippedCombatArts = new byte[Database.MAX_CHARA_COMBAT_ARTS]; 
            for (int i = 0; i < Database.MAX_CHARA_COMBAT_ARTS; i++) data.EquippedCombatArts[i] = Database.COMBAT_ARTS_COUNT;
            data.ItemCount = 0;
            data.SkillLevel = new byte[Database.MAX_SKILLS]; 
            for (int i = 0; i < Database.MAX_SKILLS; i++) data.SkillLevel[i] = 0;
            data.CurrentClassLevel = 0;
            data.MagicDurability = new byte[Database.MAX_MAGIC]; 
            for (int i = 0; i < Database.MAX_MAGIC; i++) data.MagicDurability[i] = 0;
            data.LearnedMagic = new byte[Database.MAX_MAGIC]; 
            for (int i = 0; i < Database.MAX_MAGIC; i++) data.LearnedMagic[i] = Database.MAGIC_COUNT;
            data.Flags = 0xF0000000;
            data.field_B0 = new byte[20]; 
            for (int i = 0; i < 20; i++) data.field_B0[i] = 0;
            data.Motivation = 50;
            data.field_C5 = new byte[11]; 
            for (int i = 0; i < 11; i++) data.field_C5[i] = 0;
            data.field_D0 = new byte[3]; 
            for (int i = 0; i < 3; i++) data.field_D0[i] = 0;
            data.ClassUnlockFlags = new byte[Database.MAX_CLASS_FLAGS]; 
            for (int i = 0; i < Database.MAX_CLASS_FLAGS; i++) data.ClassUnlockFlags[i] = 0;
            data.field_DC = new byte[3]; 
            for (int i = 0; i < 3; i++) data.field_DC[i] = 0;
            data.LearningFocus = 0;
            data.ClassFlags = 0;
            data.field_E0 = new byte[10]; 
            for (int i = 0; i < 10; i++) data.field_E0[i] = 0;
            data.field_EA = new byte[10]; 
            for (int i = 0; i < 10; i++) data.field_EA[i] = 0;
            data.field_F4 = new byte[6]; 
            for (int i = 0; i < 6; i++) data.field_F4[i] = 0;
            data.AdjutantId = -1;
            data.SkillExp2 = new ushort[Database.MAX_SKILLS]; 
            for (int i = 0; i < Database.MAX_SKILLS; i++) data.SkillExp2[i] = 0;
            data.ClassExp = new ushort[Database.MAX_CLASS]; 
            for (int i = 0; i < Database.MAX_CLASS; i++) data.ClassExp[i] = 0;
            data.ClassExp2 = new ushort[26]; 
            for (int i = 0; i < 26; i++) data.ClassExp2[i] = 0;

            //part2:
            data.DeployIndex = 0xFF;
            data.DeploySlot = 0xFF;
            data.SkillLevel2 = new byte[Database.MAX_SKILLS]; 
            for (int i = 0; i < Database.MAX_SKILLS; i++) data.SkillLevel2[i] = 0;
            data.ClassLevel = new byte[Database.MAX_CLASS]; 
            for (int i = 0; i < Database.MAX_CLASS; i++) data.ClassLevel[i] = 0;
            data.ClassLevel2 = new byte[30]; 
            for (int i = 0; i < 30; i++) data.ClassLevel2[i] = 0;
            data.field_7D = new byte[3]; 
            for (int i = 0; i < 3; i++) data.field_7D[i] = 0;


        }

        public static CharacterV1000 LoadMemoryCharacter(byte[] data)
        {
            //when a character is send into a battle, the first 0xC2 byte are copied into a 105 slot battle character table

            CharacterV1000 temp = new CharacterV1000();
            temp.Init();
            byte[] empty = Util.StructureToByteArray(temp);
            data.CopyTo(empty, 0);
            temp = Util.ReadStructure<CharacterV1000>(empty);


            //generate some easy stuff
            for (int i = 0; i < Database.MAX_SKILLS; i++)
            {
                temp.data.SkillExp2[i] = temp.data.SkillExp[i];
                temp.data.SkillLevel2[i] = temp.data.SkillLevel[i];
            }

            temp.data.ClassLevel[temp.Class] = temp.data.CurrentClassLevel;
            temp.data.ClassExp[temp.Class] = temp.data.CurrentClassExp;

            return temp;
        }

        public override string ToString()
        {
            //return $"{Database.GetUnitName(p1.Id)} - Lv:{p1.Level}, Job:{Database.GetClassName(p1.Job)}";
            return $"{Database.GetUnitName(data.Id)} - Lv:{data.Level}";
        }

        public string GenerateDebugOut()
        {
            string result = "";

            return result;
        }

    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct Character
    {
        public const int SIZE = 0x24C; //old: 0x230
        public const int SIZE_MEMORY = 0xC2;

        public CharacterData data;

        public short Id { get => data.Id; set => data.Id = value; }
        public byte Class { get => data.Class; set => data.Class = value; }

        public override string ToString()
        {
            //return $"{Database.GetUnitName(data.Id)} - Lv:{data.Level}, Job:{Database.GetClassName(p1.Job)}";
            return $"{Database.GetUnitName(data.Id)} - Lv:{data.Level}";
        }

        public string GenerateDebugOut()
        {
            string result = "";

            return result;
        }

    }


}
