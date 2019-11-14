using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct SaveData
    {
        public const int SIZE = 0x25400;
        public const int ITEM_COUNT = 400;
        public const int CHARACTER_COUNT = 60;
        public const int Unknown0001_COUNT = 500;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ITEM_COUNT)]
        public Item[] Items; //0x0

        public uint ItemCount; //0x640

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = CHARACTER_COUNT)]
        public Character[] Characters; //0x644

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x28)]
        public byte[] PlayerName; //0x8984

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0xC)]
        public byte[] field_89AC; //0x89AC

        public uint SizeOfUnknown0001; //0x89B8 //19DF0

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Unknown0001_COUNT)]
        public Unknown0001[] Unknown0001s; //0x89BC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x7B0)]
        public byte[] field_21FFC; //0x21FFC

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30D)]
        public byte[] field_227AC; //0x227AC

        public Player Player; //0x22AB9
        
        public Activities Activities; //0x2498D
    }

    public class Save
    {
        public const int SIZE_SAVE_HEADER = 0xC;
        public const int SIZE_SAVE_DATA = SaveData.SIZE + SIZE_SAVE_HEADER;

        public uint Checksum, CalculatedChecksum;
        public uint SizeOfHeader;
        public uint SizeOfFile;
        public SaveData SaveData;
        public bool WarnChecksum;

        public Save()
        {
            Checksum = 0;
            SizeOfHeader = 0xC;
            SizeOfFile = SaveData.SIZE + SIZE_SAVE_HEADER;
            SaveData = new SaveData();
            WarnChecksum = false;
        }

        public void Read(string sPath)
        {
            using (var br = new BinaryReader(File.OpenRead(sPath)))
            {
                Checksum = br.ReadUInt32();
                SizeOfHeader = br.ReadUInt32();
                SizeOfFile = br.ReadUInt32();
                
                if(SizeOfFile != SIZE_SAVE_DATA)
                    throw new NotSupportedException("Current save is not supported!");

                byte[] data = br.ReadBytes(SaveData.SIZE);

                CalculatedChecksum = Util.CalcChecksum32(data);

                if (CalculatedChecksum != Checksum)
                {
                    WarnChecksum = true;
                }

                try
                {
                    SaveData = Util.ReadStructure<SaveData>(data);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                FixImpossibleValues();

            }
        }

        public void FixImpossibleValues()
        {
            //fix character class exp
            for (int i = 0; i < Database.CHARACTER_COUNT; i++)
            {
                var character = SaveData.Characters[i];

                for (int j = 0; j < Database.MAX_CLASSES; j++)
                {
                    if(character.p1.ClassExp[j] >= 0)
                    character.p1.ClassExp[j] = (ushort)Math.Min(character.p1.ClassExp[j], Database.GetMaxClassExp(j));
                }

                if(character.Class >= 0)
                    character.p1.CurrentClassExp = (ushort)Math.Min(character.p1.CurrentClassExp, Database.GetMaxClassExp(character.Class));

                SaveData.Characters[i] = character;
            }
        }

        public void Write(string sPath)
        {
            Util.DeleteFile(sPath);

            byte[] data = Util.StructureToByteArray(SaveData);

            using (var bw = new BinaryWriter(File.OpenWrite(sPath)))
            {
                bw.Write(Util.CalcChecksum32(data));
                bw.Write(SizeOfHeader);
                bw.Write(SizeOfFile);
                bw.Write(data);
            }
        }



    }
}
