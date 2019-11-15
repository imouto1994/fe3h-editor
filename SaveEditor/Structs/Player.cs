using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct Player1000
    {
        public const int SIZE = 0x1EC8;
        public const int COUNT_SUPPORT = 256;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
        public byte[] field_0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public CharacterData[] OnlineCharacter; //0x18

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] field_888; //0x888
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] field_8B8; //0x8B8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] field_8E8; //0x8E8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] field_918; //0x918
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 232)]
        public byte[] field_948; //0x948

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public Battalion[] Battalions; //0xA30

        public uint Playtime, Money, field_1078, Chapter;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = COUNT_SUPPORT)]
        public ushort[] CharacterSupportValues;

        public byte Difficulty, Gamestyle, Route, field_1283, MapID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 272)]
        public byte[] field_1285;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 223)]
        public byte[] MiscItems; //0x1395

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 225)]
        public byte[] GiftItems; //0x1474

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] field_1555;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public short[] field_155A;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public short[] field_175A;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1860)]
        public byte[] field_1782;

        public short field_1EC6;

        public string GetPlaytime()
        {
            return $"{Playtime / 3600}:{Playtime % 3600 / 60}";
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = SIZE)]
    public struct Player
    {
        public const int SIZE = 0x1EC8;
        public const int COUNT_SUPPORT = 270;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x18)]
        public byte[] field_0;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public CharacterDataPart1[] OnlineCharacter; //0x18

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] field_888; //0x888
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] field_8B8; //0x8B8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] field_8E8; //0x8E8

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public byte[] field_918; //0x918
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 232)]
        public byte[] field_948; //0x948

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public Battalion[] Battalions; //0xA30

        public uint Playtime, Money, field_1078, Chapter;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = COUNT_SUPPORT)]
        public ushort[] CharacterSupportValues;

        public byte Difficulty, Gamestyle, Route, field_1283, MapID;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 272)]
        public byte[] field_1285;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 223)]
        public byte[] MiscItems; //0x1395

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 225)]
        public byte[] GiftItems; //0x1474

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2391)]
        public byte[] new_game_plus_related;

        /*
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] field_1555;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public short[] field_155A;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public short[] field_175A;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1860)]
        public byte[] field_1782;

        public short field_1EC6;
        */

        public string GetPlaytime()
        {
            return $"{Playtime / 3600}:{Playtime % 3600 / 60}";
        }
    }


}
