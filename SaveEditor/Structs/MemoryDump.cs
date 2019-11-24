using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    public class MemoryDump
    {
        private const int MemoryDumpHeaderSize = 0x32;
        private const int SaveStart_v101 = MemoryDumpHeaderSize + 0x129700;

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
        public struct MemorySave
        {
            public const int SIZE = 0x25427;
            public const int ITEM_COUNT = 400;
            public const int CHARACTER_COUNT = 60;
            public const int Unknown0001_COUNT = 500;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_COUNT)]
            public Item[] Items; //0x0

            public uint ItemCount; //0x640

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = CHARACTER_COUNT)]
            public CharacterV1000[] Characters; //0x644

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x28)]
            public byte[] PlayerName; //0x8984
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xC)]
            public byte[] field_89AC; //0x89AC

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xC)] public byte[] padding001;

            public uint SizeOfUnknown0001; //0x89B8 //19DF0
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x8)] public byte[] padding002;
      
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Unknown0001_COUNT)]
            public Unknown0001[] Unknown0001s; //0x89BC
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x7B0)]
            public byte[] field_21FFC; //0x21FFC

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)] public byte[] padding003;
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30D)]
            public byte[] field_227AC; //0x227AC
            
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3)] public byte[] padding004;

            public Player1000 Player; //0x22AB9
                    
            public Activities Activities; //0x249A8
        }

        public void Load(ref Save save, string sPath)
        {
            using (var br = new BinaryReader(File.OpenRead(sPath)))
            {
                br.SeekTo(SaveStart_v101);

                byte[] data = br.ReadBytes(MemorySave.SIZE);
                byte[] temp;

                var SaveData = Util.ReadStructure<MemorySave>(data);

                save = new Save();
                /*
                save.SaveData1000.Items = new Item[SaveData.Items.Length];
                SaveData.Items.CopyTo(save.SaveData1000.Items, 0);

                save.SaveData1000.ItemCount = SaveData.ItemCount;

                save.SaveData1000.Characters = new CharacterV1000[SaveData.Characters.Length];
                SaveData.Characters.CopyTo(save.SaveData1000.Characters, 0);

                save.SaveData1000.PlayerName = new byte[SaveData.PlayerName.Length];
                SaveData.PlayerName.CopyTo(save.SaveData1000.PlayerName, 0);
                
                save.SaveData1000.field_89AC = new byte[SaveData.field_89AC.Length];
                SaveData.field_89AC.CopyTo(save.SaveData1000.field_89AC, 0);

                save.SaveData1000.SizeOfUnknown0001 = SaveData.SizeOfUnknown0001;
                
                save.SaveData1000.Unknown0001s = new Unknown0001[SaveData.Unknown0001s.Length];
                SaveData.Unknown0001s.CopyTo(save.SaveData1000.Unknown0001s, 0);
                
                save.SaveData1000.field_21FFC = new byte[SaveData.field_21FFC.Length];
                SaveData.field_21FFC.CopyTo(save.SaveData1000.field_21FFC, 0);
                   
                save.SaveData1000.field_227AC = new byte[SaveData.field_227AC.Length];
                SaveData.field_227AC.CopyTo(save.SaveData1000.field_227AC, 0);

                temp = Util.StructureToByteArray(SaveData.Player);
                save.SaveData1000.Player = Util.ReadStructure<Player1000>(temp);

                temp = Util.StructureToByteArray(SaveData.Activities);
                save.SaveData1000.Activities = Util.ReadStructure<Activities>(temp);
                */
            }
        }
    }
}
