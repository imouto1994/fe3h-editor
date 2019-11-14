using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SaveEditor.Structs
{
    public class BinaryDB
    {
        public const string PATH_ASSEMBLY = "SaveEditor.Data.";
        public const string PATH_CharacterDB = PATH_ASSEMBLY + "CharacterDB";
        public const string PATH_ClassDB = PATH_ASSEMBLY + "ClassDB";
        public const string PATH_SupportTalkDB = PATH_ASSEMBLY + "SupportTalkDB";
        public const string PATH_ItemDB = PATH_ASSEMBLY + "ItemDB";

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
        public struct FETH_DATA_HEADER
        {
            public const int SIZE = 0x40;

            public uint Magic; //0x16121900
            public int Count;

            public int StructureSize;
            //remaining data is padding
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
        public struct STAT_ENTRY
        {
            public const int SIZE = 9;

            public sbyte Strength, Magic, Dexterity, Speed, Luck, Defense, Resistance, Movement, Charm;

            public override string ToString()
            {
                return $"{Strength};{Magic};{Dexterity};{Speed};{Luck};{Defense};{Resistance};{Movement};{Charm};";
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
        public struct CHARACTER_DATABASE_ENTRY
        {
            public const int SIZE = 0x4C;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] Scale;

            public short unk1, NameStringId, unk2, NameStringId2, NameStringId3;

            public byte unk3, BaseAge, BaseClass;
            public short unk4, unk5;
            public byte unk8, MaximumHP, unk7;
            public byte Affiliation, GrowthHP, Gender;
            public byte BaseHP, unk9;
            public byte Crest1, Crest2, unk10;
            public byte Height1, Height2, unk11, unk12;

            public STAT_ENTRY BaseStats, GrowthStats, MaximumStats;

            public byte padding;

            public string UnitName => Database.GetString(NameStringId + 1156);
            public string ExtraName => Database.GetString(NameStringId2 + 1730);
            public string SurName => Database.GetString(NameStringId3 + 2304);
            public string FullName => string.Join(" ", UnitName, ExtraName, SurName);

            public override string ToString()
            {
                return UnitName;
            }

            public string GenerateDebugOut()
            {
                string result = "";

                result += $"Scale: {Scale[0]}, {Scale[1]}, {Scale[2]}, {Scale[3]}\r\n";

                result += $"Unit Name: {UnitName}\r\n";
                result += $"Full Name: {FullName}\r\n";

                result += $"Base Age: {BaseAge}\r\n";
                result += $"Base Class: {Database.GetClassName(BaseClass, true)}\r\n";
                result += $"Gender: {Gender}\r\n";
                result += $"Affiliation: {Database.GetAffiliationName(Affiliation, true)}\r\n";
                result += $"Crests: {Database.GetCrestName(Crest1, true)}, {Database.GetCrestName(Crest2, true)}\r\n";

                result += $"Height1: {Height1} cm, Height2: {Height2} cm\r\n";

                result += $"Base Stats: {BaseHP};{BaseStats}\r\n";
                result += $"Growth Stats: {GrowthHP};{GrowthStats}\r\n";
                result += $"Maximum Stats: {MaximumHP};{MaximumStats}\r\n";
                
                result += $"unk1: {unk1}, unk2: {unk2}\r\n";
                result += $"unk3: {unk3}, unk4: {unk4}, unk5: {unk5}\r\n";
                result += $"unk7: {unk7}, unk8: {unk8}, unk9: {unk9}\r\n";
                result += $"unk10: {unk10}, unk11: {unk11}, unk12: {unk12}, padding: {padding}\r\n";

                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
        public struct CLASS_DATABASE_ENTRY
        {
            public const int SIZE = 0x6C;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public short[] Frame;

            public short MaleId, FemaleId, unk1, unk2;

            public byte ClassChangeHp;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] unk3;

            public sbyte GrowthHP;
            public STAT_ENTRY GrowthStats;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public byte[] unk4;
            
            public STAT_ENTRY ClassChangeStats;
            public STAT_ENTRY MountStats;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Database.MAX_SKILLS)]
            public byte[] SkillBonus;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] unk5;

            public byte ExamType;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] unk6;

            public byte Exp, unk7;
            public sbyte BaseHP;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] unk8;

            public byte Magic1, Magic2;

            public STAT_ENTRY BaseStats;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] CombatArts;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            public byte[] unk9;



            public string GenerateDebugOut()
            {
                string result = "";

                result += $"Frame: {Frame[0]},{Frame[1]},{Frame[2]},{Frame[3]}\r\n";

                result += $"MaleId: {MaleId}\r\n";
                result += $"FemaleId: {FemaleId}\r\n";

                result += $"Exp: {Exp}\r\n";
                result += $"Exam Type: {ExamType}\r\n";
                result += $"Magic: {Database.GetMagicSkillName(Magic1)}, {Database.GetMagicSkillName(Magic2)}\r\n";

                result += $"CombatArts: {Database.GetCombatArtName(CombatArts[0])}, {Database.GetCombatArtName(CombatArts[1])},";
                result += $"{Database.GetCombatArtName(CombatArts[2])}, {Database.GetCombatArtName(CombatArts[3])}\r\n";
                
                result += $"SkillBonus: {Util.Array2String(SkillBonus, " ")}\r\n";
                result += $"BaseStats: {BaseHP};{BaseStats}\r\n";
                result += $"GrowthStats: {GrowthHP};{GrowthStats}\r\n";
                result += $"ClassChangeStats: {ClassChangeHp};{ClassChangeStats}\r\n";
                result += $"MountStats: {0};{MountStats}\r\n";

                result += $"unk1: {unk1}\r\n";
                result += $"unk2: {unk2}\r\n";
                result += $"unk3: {Util.Array2String(unk3, " ")}\r\n";
                result += $"unk4: {Util.Array2String(unk4, " ")}\r\n";
                result += $"unk5: {Util.Array2String(unk5, " ")}\r\n";
                result += $"unk6: {Util.Array2String(unk6, " ")}\r\n";
                result += $"unk7: {unk7}\r\n";
                result += $"unk8: {Util.Array2String(unk8, " ")}\r\n";
                result += $"unk9: {Util.Array2String(unk9, " ")}\r\n";

                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
        public struct SUPPORT_TALK_DATABASE_ENTRY
        {
            public const int SIZE = 0x8;

            public short Id1, Id2;
            public ushort Flags;

            public sbyte Character1, Character2;
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
        public struct ITEM_DATABASE_ENTRY
        {
            public const int SIZE = 0x18;

            public ushort unk1;
            public byte BonusValue; 

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] unk2;

            public byte Rank, Range;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] unk3;

            public byte Durability;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] unk4;

            public byte Protect, HitRate, Crit, Weight, BonusType;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] unk5;

            public string GenerateDebugOut()
            {
                string result = "";
                
                result += $"Rank: {Rank}, Range: {Range}\r\n";
                result += $"Durability: {Durability}\r\n";
                result += $"Protect: {Protect}\r\n";
                result += $"HitRate: {HitRate}\r\n";
                result += $"Crit: {Crit}\r\n";
                result += $"Weight: {Weight}\r\n";
                result += $"BonusValue: {BonusValue}, BonusType: {BonusType}\r\n";

                result += $"unk1: {unk1}\r\n";
                result += $"unk2: {Util.Array2String(unk2, " ")}\r\n";
                result += $"unk3: {Util.Array2String(unk3, " ")}\r\n";
                result += $"unk4: {Util.Array2String(unk4, " ")}\r\n";
                result += $"unk5: {Util.Array2String(unk5, " ")}\r\n";

                return result;
            }
        }

        public List<CHARACTER_DATABASE_ENTRY> CharacterEntries;
        public List<CLASS_DATABASE_ENTRY> ClassEntries;
        public List<SUPPORT_TALK_DATABASE_ENTRY> SupportTalkEntries;
        public Dictionary<int, ITEM_DATABASE_ENTRY> ItemEntries;

        public BinaryDB()
        {
            CharacterEntries = new List<CHARACTER_DATABASE_ENTRY>();
            ClassEntries = new List<CLASS_DATABASE_ENTRY>();
            SupportTalkEntries = new List<SUPPORT_TALK_DATABASE_ENTRY>();
        }

        public void Init()
        {
            LoadCharacterDB();
            LoadClassDB();
            LoadSupportTalkDB();

            ItemEntries = new Dictionary<int, ITEM_DATABASE_ENTRY>();

            LoadItemDB(10);
            LoadItemDB(600);
            LoadItemDB(1000);
            LoadItemDB(4000);
            LoadItemDB(5000);
            LoadItemDB(6000);
            LoadItemDB(7000);
        }

        public void DumpToJson()
        {
            Util.DeleteFile(PATH_CharacterDB + ".json");
            Util.DeleteFile(PATH_ClassDB + ".json");

            var serializer = new JavaScriptSerializer();
            File.WriteAllText(PATH_CharacterDB + ".json", serializer.Serialize(CharacterEntries));
            File.WriteAllText(PATH_ClassDB + ".json", serializer.Serialize(ClassEntries));
        }

        public void LoadCharacterDB()
        {
            var h = new FETH_DATA_HEADER();
            CharacterEntries = new List<CHARACTER_DATABASE_ENTRY>();

            byte[] res = Util.GetRessourceFile(PATH_CharacterDB + ".dat");

            if (res != null)
            {
                using (var ms = new MemoryStream(res))
                using (var br = new BinaryReader(ms))
                {
                    h = Util.ReadStructure<FETH_DATA_HEADER>(br.ReadBytes(FETH_DATA_HEADER.SIZE));

                    for (int i = 0; i < h.Count; i++)
                    {
                        byte[] data = br.ReadBytes(h.StructureSize);
                        CharacterEntries.Add(Util.ReadStructure<CHARACTER_DATABASE_ENTRY>(data));
                    }
                }
            }
        }

        public void LoadClassDB()
        {
            var h = new FETH_DATA_HEADER();
            ClassEntries = new List<CLASS_DATABASE_ENTRY>();
                
            byte[] res = Util.GetRessourceFile(PATH_ClassDB + ".dat");

            if (res != null)
            {
                using (var ms = new MemoryStream(res))
                using (var br = new BinaryReader(ms))
                {
                    h = Util.ReadStructure<FETH_DATA_HEADER>(br.ReadBytes(FETH_DATA_HEADER.SIZE));

                    for (int i = 0; i < h.Count; i++)
                    {
                        byte[] data = br.ReadBytes(h.StructureSize);
                        ClassEntries.Add(Util.ReadStructure<CLASS_DATABASE_ENTRY>(data));
                    }
                }
            }
        }

        public void LoadSupportTalkDB()
        {
            var h = new FETH_DATA_HEADER();
            SupportTalkEntries = new List<SUPPORT_TALK_DATABASE_ENTRY>();
                
            byte[] res = Util.GetRessourceFile(PATH_SupportTalkDB + ".dat");

            if (res != null)
            {
                using (var ms = new MemoryStream(res))
                using (var br = new BinaryReader(ms))
                {
                    h = Util.ReadStructure<FETH_DATA_HEADER>(br.ReadBytes(FETH_DATA_HEADER.SIZE));

                    for (int i = 0; i < h.Count; i++)
                    {
                        byte[] data = br.ReadBytes(h.StructureSize);
                        SupportTalkEntries.Add(Util.ReadStructure<SUPPORT_TALK_DATABASE_ENTRY>(data));
                    }
                }
            }
        }

        public void LoadItemDB(int start_index)
        {
            var h = new FETH_DATA_HEADER();                
            byte[] res = Util.GetRessourceFile($"{PATH_ItemDB}_{start_index:D4}.dat");

            if (res != null)
            {
                using (var ms = new MemoryStream(res))
                using (var br = new BinaryReader(ms))
                {
                    h = Util.ReadStructure<FETH_DATA_HEADER>(br.ReadBytes(FETH_DATA_HEADER.SIZE));

                    for (int i = 0; i < h.Count; i++)
                    {
                        byte[] data = br.ReadBytes(h.StructureSize);
                        ItemEntries.Add(start_index + i, Util.ReadStructure<ITEM_DATABASE_ENTRY>(data));
                    }
                }
            }
        }

    }
}
