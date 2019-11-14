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
        public const int MAX_ITEM_TYPES = 7;
        public const int ITEM_ID_BASE_WEAPON = 10;
        public const int ITEM_ID_BASE_ACCESSORY = 600;
        public const int ITEM_ID_BASE_CONSUMEABLE = 1000;
        public const int ITEM_ID_BASE_MAGIC = 4000;
        public const int ITEM_ID_BASE_OBJECT = 5000;
        public const int ITEM_ID_BASE_SPECIAL1 = 6000;
        public const int ITEM_ID_BASE_SPECIAL2 = 7000;

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
            public const int SIZE = 0x50; //old: 0x4C

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public float[] Scale;

            public short unk1, NameStringId, unk2, NameStringId2, NameStringId3;

            public byte BaseClass, BaseAge, unk3;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] unk4;

            public byte MaximumHP;
            
            public byte unk5, Affiliation, unk6;
            public byte Gender, unk7, unk8;

            public byte GrowthHP, unk9;
            public byte BaseHP, Crest1, Crest2, unk10;
            public byte Height1, Height2, unk11, unk12;

            public STAT_ENTRY BaseStats, GrowthStats, MaximumStats;

            public short padding; //was "byte"

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
                result += $"unk3: {unk3}, unk4: {Util.Array2String(unk4, " ")}, unk5: {unk5}\r\n";
                result += $"unk6: {unk6}, unk7: {unk7}, unk8: {unk8}, unk9: {unk9}\r\n";
                result += $"unk10: {unk10}, unk11: {unk11}, unk12: {unk12}, padding: {padding}\r\n";

                return result;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
        public struct CLASS_DATABASE_ENTRY
        {
            public const int SIZE = 0x76; //old: 0x6C

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

            public byte unk5, ExamType, unk5_1;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] unk6;

            public byte unk5_2;

            public byte Exp;
            public sbyte BaseHP;
            public byte unk7;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] unk8;

            public byte Magic1, Magic2;

            public STAT_ENTRY BaseStats;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] CombatArts;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
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
                result += $"unk5: {unk5} {unk5_1} {unk5_2}\r\n";
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
            //old:
            //public const int SIZE = 0x8;
            //public short Id1, Id2;
            //public ushort Flags;
            
            public const int SIZE = 0xA;

            public short Id1, Character1, Character2, Id2;
            public ushort Flags;
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
        public int[] ItemCounts;

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
            ItemCounts = new int[MAX_ITEM_TYPES];

            LoadItemDB(enmItemTypes.Weapon, ITEM_ID_BASE_WEAPON);
            LoadItemDB(enmItemTypes.Accessory, ITEM_ID_BASE_ACCESSORY);
            LoadItemDB(enmItemTypes.Consumeable, ITEM_ID_BASE_CONSUMEABLE);
            LoadItemDB(enmItemTypes.Magic, ITEM_ID_BASE_MAGIC);
            LoadItemDB(enmItemTypes.Object, ITEM_ID_BASE_OBJECT);
            LoadItemDB(enmItemTypes.Special1, ITEM_ID_BASE_SPECIAL1);
            LoadItemDB(enmItemTypes.Special2, ITEM_ID_BASE_SPECIAL2);
        }

        public void DumpToJson()
        {
            CreateJson("person.json", CharacterEntries);
            CreateJson("class.json", ClassEntries);
            CreateJson("talk.json", SupportTalkEntries);
            CreateJson("items.json", ItemEntries);
        }

        private void CreateJson(string filename, object data)
        {
            Util.DeleteFile(filename);

            var serializer = new JavaScriptSerializer();
            File.WriteAllText(filename, serializer.Serialize(data));
        }

        public void LoadCharacterDB()
        {
            CharacterEntries = new List<CHARACTER_DATABASE_ENTRY>();

            MiniArchive person = new MiniArchive();
            person.Load(Util.DecompressGzip(Properties.Resources.fixed_persondata_bin));

            using (var ms = new MemoryStream(person.GetEntry(0)))
            using (var br = new BinaryReader(ms))
            {
                var h = Util.ReadStructure<FETH_DATA_HEADER>(br.ReadBytes(FETH_DATA_HEADER.SIZE));

                for (int i = 0; i < h.Count; i++)
                {
                    CharacterEntries.Add(Util.ReadStructure<CHARACTER_DATABASE_ENTRY>(br.ReadBytes(h.StructureSize)));
                }
            }
        }

        public void LoadClassDB()
        {
            ClassEntries = new List<CLASS_DATABASE_ENTRY>();
         
            MiniArchive classdata = new MiniArchive();
            classdata.Load(Util.DecompressGzip(Properties.Resources.fixed_classdata_bin));

            using (var ms = new MemoryStream(classdata.GetEntry(0)))
            using (var br = new BinaryReader(ms))
            {
                var h = Util.ReadStructure<FETH_DATA_HEADER>(br.ReadBytes(FETH_DATA_HEADER.SIZE));

                for (int i = 0; i < h.Count; i++)
                {
                    ClassEntries.Add(Util.ReadStructure<CLASS_DATABASE_ENTRY>(br.ReadBytes(h.StructureSize)));
                }
            }
        }

        public void LoadSupportTalkDB()
        {
            SupportTalkEntries = new List<SUPPORT_TALK_DATABASE_ENTRY>();
           
            MiniArchive person = new MiniArchive();
            person.Load(Util.DecompressGzip(Properties.Resources.fixed_persondata_bin));

            using (var ms = new MemoryStream(person.GetEntry(10)))
            using (var br = new BinaryReader(ms))
            {
                var h = Util.ReadStructure<FETH_DATA_HEADER>(br.ReadBytes(FETH_DATA_HEADER.SIZE));

                for (int i = 0; i < h.Count; i++)
                {
                    SupportTalkEntries.Add(Util.ReadStructure<SUPPORT_TALK_DATABASE_ENTRY>(br.ReadBytes(h.StructureSize)));
                }
            }
        }

        public void LoadItemDB(enmItemTypes type, int start_index)
        {
            MiniArchive data = new MiniArchive();
            data.Load(Util.DecompressGzip(Properties.Resources.fixed_data_bin));

            using (var ms = new MemoryStream(data.GetEntry((int)type)))
            using (var br = new BinaryReader(ms))
            {
                var h = Util.ReadStructure<FETH_DATA_HEADER>(br.ReadBytes(FETH_DATA_HEADER.SIZE));

                for (int i = 0; i < h.Count; i++)
                {
                    ItemEntries.Add(start_index + i, Util.ReadStructure<ITEM_DATABASE_ENTRY>(br.ReadBytes(h.StructureSize)));
                }

                ItemCounts[(int)type] = h.Count;
            }
        }

        public int GetMinItemIndex(enmItemTypes type)
        {
            switch (type)
            {
                case enmItemTypes.Weapon: return ITEM_ID_BASE_WEAPON;
                case enmItemTypes.Accessory: return ITEM_ID_BASE_ACCESSORY;
                case enmItemTypes.Consumeable: return ITEM_ID_BASE_CONSUMEABLE;
                case enmItemTypes.Magic: return ITEM_ID_BASE_MAGIC;
                case enmItemTypes.Object: return ITEM_ID_BASE_OBJECT;
                case enmItemTypes.Special1: return ITEM_ID_BASE_SPECIAL1;
                case enmItemTypes.Special2: return ITEM_ID_BASE_SPECIAL2;
                default:
                    return -1;
            }
        }

        public int GetMaxItemIndex(enmItemTypes type)
        {
            int min = GetMinItemIndex(type);
            var count = ItemCounts[(int) type];

            if (type >= enmItemTypes.MaxItemTypes)
                return -1;

            return min + count;
        }

        public bool IsItemInRange(enmItemTypes type, int id)
        {
            int min = GetMinItemIndex(type);
            int max = GetMaxItemIndex(type);

            return id >= min && id < max;
        }


    }
}
