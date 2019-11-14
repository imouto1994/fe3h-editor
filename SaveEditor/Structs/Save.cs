using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    public enum SaveFileVersion
    {
        Error = -1,
        V1000 = 0,
        V1001 = 1,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct SaveData_1000
    {
        public const int SIZE = 0x25400;
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

        public uint SizeOfUnknown0001; //0x89B8 //19DF0

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Unknown0001_COUNT)]
        public Unknown0001[] Unknown0001s; //0x89BC
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x7B0)]
        public byte[] field_21FFC; //0x21FFC

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x30D)]
        public byte[] field_227AC; //0x227AC

        public Player1000 Player; //0x22AB9
        
        public Activities Activities; //0x2498D
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct SaveData
    {
        public const int SIZE = 0x25B20;
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

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x39D)] //changed from 0x30D to 0x39D
        public byte[] field_227AC; //0x227AC

        public Player Player; //0x22AB9
        
        public Activities Activities; //0x2498D
    }

    public class Save
    {
        public const int SIZE_SAVE_HEADER = 0xC;
        public const int SIZE_VERSION_1000 = SaveData_1000.SIZE + SIZE_SAVE_HEADER;
        public const int SIZE_VERSION_1001 = SaveData.SIZE + SIZE_SAVE_HEADER;
 
        public uint Checksum, CalculatedChecksum;
        public uint SaveVersion;
        public uint SizeOfFile;
        //public SaveData_1000 SaveData1000;
        public SaveData SaveData;
        public bool WarnChecksum;
        public SaveFileVersion Version;

        public Save()
        {
            Checksum = 0;
            SaveVersion = 0xC;
            SizeOfFile = SaveData_1000.SIZE + SIZE_SAVE_HEADER;
            SaveData = new SaveData();
            WarnChecksum = false;
        }

        public void Read(string sPath)
        {
            using (var br = new BinaryReader(File.OpenRead(sPath)))
            {
                Checksum = br.ReadUInt32();
                SaveVersion = br.ReadUInt32();
                SizeOfFile = br.ReadUInt32();

                byte[] data;

                switch (SizeOfFile)
                {
                    case SIZE_VERSION_1000:
                    case SIZE_VERSION_1001:
                        break;

                    default:
                        throw new NotSupportedException($"Current save filesize '{SizeOfFile}' is not supported!");
                }

                switch (SaveVersion)
                {
                    case 12:
                    case 13:
                        Version = SaveFileVersion.V1000;
                        data = br.ReadBytes(SaveData_1000.SIZE);
                        throw new NotSupportedException($"Current save version '{SaveVersion}' is not supported!");

                    case 23: 
                        Version = SaveFileVersion.V1001;
                        data = br.ReadBytes(SaveData.SIZE);
                        break;

                    default:
                        throw new NotSupportedException($"Current save version '{SaveVersion}' is not supported!");
                }

                CalculatedChecksum = Util.CalcChecksum32(data);

                if (CalculatedChecksum != Checksum)
                {
                    WarnChecksum = true;
                }

                try
                {
                    SaveData = Util.ReadStructure<SaveData>(data);
                    FixImpossibleValues();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                

            }
        }

        public void FixImpossibleValues()
        {
            //fix character class exp
            for (int i = 0; i < Database.CHARACTER_COUNT; i++)
            {
                var character = SaveData.Characters[i];

                for (int j = 0; j < Database.MAX_CLASS; j++)
                {
                    character.data.ClassExp[j] = (ushort)Math.Min(character.data.ClassExp[j], Database.GetMaxClassExp(j));
                }

                character.data.CurrentClassExp = (ushort)Math.Min(character.data.CurrentClassExp, Database.GetMaxClassExp(character.Class));
                SaveData.Characters[i] = character;
            }
        }

        public void Write(string sPath)
        {
            Util.DeleteFile(sPath);

            byte[] data;

            switch (Version)
            {
                case SaveFileVersion.V1000:
                    //data = Util.StructureToByteArray(SaveData1000);
                    //break;
                    return;
                case SaveFileVersion.V1001:
                    data = Util.StructureToByteArray(SaveData);
                    break;
                default:
                    return;
            }

            using (var bw = new BinaryWriter(File.OpenWrite(sPath)))
            {
                bw.Write(Util.CalcChecksum32(data));
                bw.Write(SaveVersion);
                bw.Write(SizeOfFile);
                bw.Write(data);
            }
        }

    }
}
