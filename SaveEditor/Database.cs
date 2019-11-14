using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SaveEditor.Structs;

namespace SaveEditor
{
    public static class Database
    {
        public const string STR_UNKNOWN = "???";
        public const string STR_NONE = "----";

        public const int MAX_PLAYTIME = 3599999;
        public const int MAX_MONEY = 9999999;
        public const int MAX_REPUTATION = 999999;
        public const int MAX_INSTRUCT_EXP = 44500;

        public const int MAX_CHARA_ITEMS = 6;
        public const int MAX_CHARA_ABILITIES = 5;
        public const int MAX_CHARA_COMBAT_ARTS = 3;
        public const int MAX_SKILLS = 11;
        public const int MAX_MAGIC = 12;
        public const int MAX_CLASSES = 64; //90, but to match the flags and make everything easier, we just use the first 64
        public const int MAX_CLASS_FLAGS = 8;
        public const int MAX_ABILITIES = 30; // bytes
        public const int MAX_CLASS_LEVEL = 1;

        public const int DIFFICULTY_COUNT = 4;
        public const int GAMESTYLE_COUNT = 2;
        public const int CHARACTER_COUNT = 60;
        public const int CHARACTER_USEABLE_COUNT = 35;
        public const int ABILITY_COUNT = MAX_ABILITIES * 8; // bits
        public const int CLASS_FLAGS_COUNT = MAX_CLASS_FLAGS * 8; // bits
        public const int MAGIC_COUNT = 38; // of 300
        public const int BATTALION_COUNT = 200;
        public const int COMBAT_ARTS_COUNT = 80;
        public const int BATTALION_SKILL_COUNT = 80;
        public const int CREST_COUNT = 86;


        public static int[] SkillLevelupRank = { 40, 60, 80, 120, 160, 220, 280, 360, 440, 760, 1080, 65535 };
        public static int[] TeacherLevelupRank = { 0, 0, 0, 0, 0, 6400, 10900, 16300, 24000, 32800, MAX_INSTRUCT_EXP, 99999999 };

        public static List<short> EssentialItems = new List<short>
        {
            //Weapons:
            65, //Sword of Seiros
            66, //Sword of Begalta
            67, //Sword of Moralta
            90, //Athame
            91, //Ridill
            92, //Aymr
            93, //Dark Creator Sword
            98, //Mercurius
            99, //Gradivus
            100, //Heuteclere
            101, //Parthia
            131, //Brave Sword+
            132, //Killing Edge+
            137, //Brave Lance+
            138, //Killer Lance+
            143, //Brave Axe+
            144, //Killer Axe+
            149, //Brave Bow+
            150, //Killer Bow+
            153, //Steel Gauntlets+
            154, //Silver Gauntlets+
            168, //Rapier+
            175, //Wo Dao+
            180, //Arrow of Indra+
            183, //Dragon Claws+
            185, //Venin Edge+
            186, //Venin Lance+
            187, //Venin Axe+
            188, //Venin Bow+
            189, //Killer Knuckles+
            190, //Aura Knuckles+

            //Relic Weapons
            192, 193, 194, 195, 196, 197, 198, 199,

            //Accessories
            608, //Seiros Shield
            622, //Thyrsus
            623, //Rafail Gem
            624, //Experience Gem
            625, //Knowlegde Gem

            //Items:
            1002, //Elixir
            1015, //Master Key

            //Seals
            1003, 1004, 1005, 1006, 1157, 1158,

            //Stat Items
            1016, 1017, 1018, 1019, 1020, 1021, 1022,
            1023, 1024, 1025, 1148, 1148, 1149, 1150,
            1151, 1152, 1153, 1154, 1155, 1156
        };


        private static StringDB Text1, Text2, Text3;
        public static BinaryDB BinaryDatabase;

        public static Dictionary<int, string> ItemList;
        public static Dictionary<int, string> UnitList, CharacterList;
        
        public static Dictionary<int, string> ClassList;
        public static Dictionary<int, string> BattalionList;
        public static Dictionary<int, string> AbilityList;
        public static Dictionary<int, string> CombatArtList;
        public static Dictionary<int, string> MagicList;
        public static Dictionary<int, string> BattalionSkillList;
    
        public static void Init(enmLanguage lang = enmLanguage.en, bool DumpToFile = false)
        {
            try
            {
                Text1 = LoadDatabaseFile(lang.ToString(), 0, DumpToFile);
                Text2 = LoadDatabaseFile(lang.ToString(), 1, DumpToFile);
                Text3 = LoadDatabaseFile(lang.ToString(), 2, DumpToFile);

                BinaryDatabase = new BinaryDB();
                BinaryDatabase.Init();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new InvalidDataException("Could not load one or more database files, check the data folder!");
            }
            
            InitLists(DumpToFile);

            if (DumpToFile)
            {
                BinaryDatabase.DumpToJson();
            }
        }

        private static void LoadCSV(out Dictionary<int, string> tbl, string fileName, string lang)
        {
            string sPath = $"Data\\{fileName}.csv";
            
            tbl = new Dictionary<int, string>();

            if (!File.Exists(sPath)) return;

            using (var sr = new StreamReader(sPath))
            {
                var header = sr.ReadLine().Split(';');

                int idx = -1;

                for (int i = 0; i < header.Length; i++)
                {
                    if (header[i] == lang)
                        idx = i;
                }

                if(idx == -1) // error
                    return;


                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();
                    if(string.IsNullOrEmpty(line)) continue;

                    var data = line.Split(';');

                    if(string.IsNullOrEmpty(data[0])) continue;
                    int id = int.Parse(data[0]);

                    if(string.IsNullOrEmpty(data[idx])) continue;
                    string str = data[idx];

                    tbl.Add(id, str);
                }
            }
        }
        
        private static void LoadCSV(out Dictionary<int, int> tbl, string fileName, string id_name, string str_name)
        {
            string sPath = $"Data\\{fileName}.csv";
            
            tbl = new Dictionary<int, int>();

            if (!File.Exists(sPath)) return;

            using (var sr = new StreamReader(sPath))
            {
                var header = sr.ReadLine().Split(';');

                int idx1 = -1, idx2 = -1;

                for (int i = 0; i < header.Length; i++) if (header[i] == id_name) idx1 = i;
                for (int i = 0; i < header.Length; i++) if (header[i] == str_name) idx2 = i;

                if(idx1 == -1 || idx2 == -1) // error
                    return;

                while (sr.Peek() != -1)
                {
                    var line = sr.ReadLine();
                    if(string.IsNullOrEmpty(line)) continue;

                    var data = line.Split(';');

                    if(string.IsNullOrEmpty(data[idx1])) continue;
                    var id = int.Parse(data[idx1]);

                    var id2 = string.IsNullOrEmpty(data[idx2]) ? -1 : int.Parse(data[idx2]);

                    tbl.Add(id, id2);
                }
            }
        }

        private static StringDB LoadDatabaseFile(string lang, int idx, bool DumpToFile = false)
        {
            StringDB sdb = new StringDB();
            sdb.Load($"SaveEditor.Data.{lang}.FILE_{idx:D3}.dat.gz");

            if (DumpToFile)
            {
                using (var sw = new StreamWriter(File.OpenWrite($"Data\\{lang}\\FILE_{idx:D3}.csv"), Encoding.UTF8))
                {
                    for (int i = 0; i < sdb.Strings.Count; i++)
                    {
                        sw.Write(i.ToString());
                        sw.Write(";");
                        sw.Write(sdb.Strings[i].Replace("\r", "\\r").Replace("\n", "\\n"));
                        sw.WriteLine();
                    }
                }
            }

            return sdb;
        }

        public static string GetString(int id, int idx = 2)
        {
            string result = "";

            if (id == -1) return STR_UNKNOWN;

            switch (idx)
            {
                case 0:
                    result = Text1.Strings[id];
                    break;
                case 1:
                    result = Text2.Strings[id];
                    break;
                case 2:
                    result = Text3.Strings[id];
                    break;
            }

            //return result == string.Empty ? STR_UNKNOWN : result;
            return result;
        }

        public static void InitLists(bool DumpToCsv = false)
        {
            ItemList = new Dictionary<int, string> { { -1, STR_NONE } };
            UnitList = new Dictionary<int, string> { { -1, STR_NONE } };
            CharacterList = new Dictionary<int, string> { { -1, STR_NONE } };
            ClassList = new Dictionary<int, string> { { -1, STR_NONE } };
            BattalionList = new Dictionary<int, string> {{ -1, STR_NONE }};

            AbilityList = new Dictionary<int, string> { { ABILITY_COUNT, STR_NONE } };
            CombatArtList = new Dictionary<int, string> { { COMBAT_ARTS_COUNT, STR_NONE } };
            MagicList = new Dictionary<int, string> { { MAGIC_COUNT, STR_NONE } };
            BattalionSkillList = new Dictionary<int, string>{ { BATTALION_SKILL_COUNT, STR_NONE } };

            foreach (var item in BinaryDatabase.ItemEntries)
            {
                string str = GetItemName(item.Key, true);
                if(str.EndsWith(STR_UNKNOWN)) continue;
                ItemList.Add(item.Key, str);
            }

            for (int i = 0; i < BinaryDatabase.CharacterEntries.Count; i++)
            {
                string str = GetUnitName(i);
                if(str.EndsWith(STR_UNKNOWN)) continue;
                UnitList.Add(i, str);

                if(i < CHARACTER_USEABLE_COUNT) CharacterList.Add(i, str);
            }

            //for (int i = 0; i < 100; i++) UnitList.Add(i, GetUnitName(i));
            //for (int i = 100; i < 200; i++) UnitList.Add(i, GetUnitName(i));

            for (int i = 0; i < 90; i++) ClassList.Add(i, GetClassName(i));

            for (int i = 0; i < BATTALION_COUNT; i++) BattalionList.Add(i, GetBattalionName(i));

            for (int i = 0; i < ABILITY_COUNT; i++) AbilityList.Add(i, GetAbilityName(i));
            for (int i = 0; i < COMBAT_ARTS_COUNT; i++) CombatArtList.Add(i, GetCombatArtName(i));
            for (int i = 0; i < MAGIC_COUNT; i++) MagicList.Add(i, GetMagicSkillName(i));
            for (int i = 0; i < BATTALION_SKILL_COUNT; i++) BattalionSkillList.Add(i, GetBattalionSkillName(i));

            if (DumpToCsv)
            {
                using (var sw = new StreamWriter("items.txt"))
                {
                    foreach (var item in ItemList)
                    {
                        sw.WriteLine($@"{item.Key};{item.Value}");
                    }
                }
            }
        }

        public static string GetItemName(int id, bool debug = false)
        {
            if (id == -1) return STR_NONE;

            string result = STR_UNKNOWN;

            if (id >= 10 && id < 510) result = GetString(3722 + (id - 10)); // weapon and other
            if (id >= 600 && id < 650) result = GetString(4522 + (id - 600)); // accessory
            if (id >= 1000 && id < 1200) result = GetString(4622 + (id - 1000)); // consumable / dish / other 
            if (id >= 4000 && id < 4038) result = GetString(7802 + (id - 4000)); // magic
            
            switch (id) // stationary weapons
            {
                case 5000: result = GetString(5535); break;
                case 5001: result = GetString(5528); break;
                case 5002: result = GetString(5529); break;
            }

            if (id >= 6000 && id < 6080) result = GetString(6580 + (id - 6000)); // special attacks 1
            if (id >= 7000 && id < 7060) result = GetString(8402 + (id - 7000)); // special attacks 2

            if(debug) return $"{id:D4} - {result}";
            return result;
        }

        public static int GetItemDurability(int id)
        {
            if (BinaryDatabase.ItemEntries.ContainsKey(id))
            {
                return BinaryDatabase.ItemEntries[id].Durability;
            }

            return 0;
        }

        public static string GetUnitName(int id, bool debug = false, bool ShowGender = false, bool NoGender = false)
        {
            if (id == -1) return STR_NONE;
            
            string result = STR_UNKNOWN;

            var unit = BinaryDatabase.CharacterEntries[id];

            if ((id > 0 && unit.NameStringId == 0)) //empty entry
            {
                result = STR_NONE;
            }
            else
            {
                result = GetString(unit.NameStringId + 1156 );

                if (!NoGender && (id == 0 || id == 1 || ShowGender))
                {
                    result += unit.Gender == 0 ? "♂" : "♀";
                }
            }

            if(debug) return $"{id:D4} - {result}";
            return result;
        }
    
        public static string GetMiscItemName(int id, bool debug = false)
        {
            if (id == -1) return STR_NONE;

            string result = GetString(5022 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
                            
        public static string GetGiftItemName(int id, bool debug = false)
        {
            if (id == -1) return STR_NONE;

            string result = GetString(10108 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }

        public static string GetBattalionName(int id, bool debug = false)
        {
            if (id == -1 || id == BATTALION_COUNT) return STR_NONE;
            if (id > BATTALION_COUNT) return STR_UNKNOWN;

            string result = GetString(9062 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
        
        public static string GetBattalionSkillName(int id, bool debug = false)
        {
            if (id == -1 || id == BATTALION_SKILL_COUNT) return STR_NONE;
            if (id > BATTALION_SKILL_COUNT) return STR_UNKNOWN;

            string result = GetString(6580 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }

        public static string GetClassName(int id, bool debug = false)
        {
            if (id == -1) return STR_NONE;

            string result = GetString(3452 + id);

            var @class = BinaryDatabase.ClassEntries[id];

            if (@class.ExamType == 6) //special class
            {
                result += "★";
            }

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }

        public static string GetCombatArtName(int id, bool debug = false)
        {
            if (id == -1 || id == COMBAT_ARTS_COUNT) return STR_NONE;
            if (id > COMBAT_ARTS_COUNT) return STR_UNKNOWN;

            string result = GetString(5980 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
        
        public static string GetAbilityName(int id, bool debug = true)
        {
            if (id == -1 || id == ABILITY_COUNT) return STR_NONE;
            if (id > ABILITY_COUNT) return STR_UNKNOWN;

            string result = GetString(7202 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
             
        public static string GetMagicSkillName(int id, bool debug = false)
        {
            if (id == -1 || id == MAGIC_COUNT) return STR_NONE;
            if (id > MAGIC_COUNT) return STR_UNKNOWN;

            string result = GetString(7802 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
                     
        public static string GetMapName(int id, bool debug = false)
        {
            if (id == -1) return STR_NONE;

            string result = GetString(5108 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
                       
        public static string GetAffiliationName(int id, bool debug = false)
        {
            if (id == -1) return STR_NONE;

            string result = GetString(9464 + id); // Home: 9464 - 45

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
                                     
        public static string GetCrestName(int id, bool debug = false)
        {
            if (id == -1 || id == CREST_COUNT) return STR_NONE;

            string result = GetString(9556 + id);

            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
                                             
        public static string GetQuestName(int id, bool debug = false)
        {
            if (id == -1) return STR_NONE;

            string result = GetString(12227 + id);

            if(result == "iron_untranslated" || result == "Unused")
                return STR_NONE;


            if(debug) return $"[{id:D3} - {result}]";
            return result;
        }
            
        public static string GetSupportTalkName(int index)
        {
            var talk = BinaryDatabase.SupportTalkEntries[index];

            if(talk.Character1 == -1 || talk.Character2 == -1)
            {
                return STR_NONE;
            }

            return $"{GetUnitName(talk.Character1, NoGender: true)} 🔗 {GetUnitName(talk.Character2, NoGender: true)}";
        }

        public static int GetMaxHP(int UnitId, int ClassId)
        {
            if (UnitId == -1) return 0;

            var unit = BinaryDatabase.CharacterEntries[UnitId];
            var @class = BinaryDatabase.ClassEntries[ClassId];

            return unit.MaximumHP + @class.BaseHP;
        }

        public static int GetMinStat(int UnitId, int ClassId, int statType)
        {
            if (UnitId == -1) return 0;

            var unit = BinaryDatabase.CharacterEntries[UnitId];
            var @class = BinaryDatabase.ClassEntries[ClassId];
            int result = 0;
            
            switch (statType)
            {
                case 0: result = Math.Max(unit.BaseStats.Strength, @class.BaseStats.Strength); break;
                case 1: result = Math.Max(unit.BaseStats.Magic, @class.BaseStats.Magic); break;
                case 2: result = Math.Max(unit.BaseStats.Dexterity, @class.BaseStats.Dexterity); break;
                case 3: result = Math.Max(unit.BaseStats.Speed, @class.BaseStats.Speed); break;
                case 4: result = Math.Max(unit.BaseStats.Luck, @class.BaseStats.Luck); break;
                case 5: result = Math.Max(unit.BaseStats.Defense, @class.BaseStats.Defense); break;
                case 6: result = Math.Max(unit.BaseStats.Resistance, @class.BaseStats.Resistance); break;
                case 7: result = Math.Max(unit.BaseStats.Movement, @class.BaseStats.Movement); break;
                case 8: result = Math.Max(unit.BaseStats.Charm, @class.BaseStats.Charm); break;
            }

            return result;
        }

        public static int GetMaxStat(int UnitId, int ClassId, int statType)
        {
            if (UnitId == -1) return 0;

            var unit = BinaryDatabase.CharacterEntries[UnitId];
            var @class = BinaryDatabase.ClassEntries[ClassId];
            int result = 0;

            switch (statType)
            {
                case 0: result = unit.MaximumStats.Strength; break;
                case 1: result = unit.MaximumStats.Magic; break;
                case 2: result = unit.MaximumStats.Dexterity; break;
                case 3: result = unit.MaximumStats.Speed; break;
                case 4: result = unit.MaximumStats.Luck; break;
                case 5: result = unit.MaximumStats.Defense; break;
                case 6: result = unit.MaximumStats.Resistance; break;
                case 7: result = unit.MaximumStats.Movement; break;
                case 8: result = unit.MaximumStats.Charm; break;
            }

            return result;
        }

        public static int GetMaxClassExp(int ClassId)
        {
            return BinaryDatabase.ClassEntries[ClassId].Exp;
        }
    




    }
}
