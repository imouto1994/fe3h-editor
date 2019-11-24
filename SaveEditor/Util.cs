using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveEditor
{
    public static class Util
    {
        #region Marshal

        public static T ReadStructure<T>(byte[] bytes, bool IsLittleEndian = true) where T : struct
        {
            if (IsLittleEndian)
                return ByteArrayToStructure<T>(bytes);
            else
                return ByteArrayToStructureBE<T>(bytes);
        }

        public static T ReadStructure<T>(Stream fs, bool IsLittleEndian = true) where T : struct
        {
            if (IsLittleEndian)
                return ReadStreamToStructure<T>(fs);
            else
                return ReadStreamToStructureBE<T>(fs);
        }

        private static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                typeof(T));
            handle.Free();
            return stuff;
        }

        private static T ReadStreamToStructure<T>(Stream fs) where T : struct
        {
            byte[] array = new byte[Marshal.SizeOf(typeof(T))];
            fs.Read(array, 0, Marshal.SizeOf(typeof(T)));
            GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
            T result = (T) ((object) Marshal.PtrToStructure(gCHandle.AddrOfPinnedObject(), typeof(T)));
            gCHandle.Free();
            return result;
        }

        private static T ByteArrayToStructureBE<T>(byte[] bytes) where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T stuff = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            FieldInfo[] fieldInfo = stuff.GetType().GetFields();
            foreach (FieldInfo fi in fieldInfo)
            {
                byte[] bTemp, bTempRev;

                if (fi.FieldType == typeof(short))
                {
                    bTemp = BitConverter.GetBytes((short) fi.GetValue(stuff));
                    bTempRev = bTemp.Reverse<byte>().ToArray<byte>();
                    fi.SetValueDirect(__makeref(stuff), BitConverter.ToInt16(bTempRev, 0));
                }
                else if (fi.FieldType == typeof(int))
                {
                    bTemp = BitConverter.GetBytes((int) fi.GetValue(stuff));
                    bTempRev = bTemp.Reverse<byte>().ToArray<byte>();
                    fi.SetValueDirect(__makeref(stuff), BitConverter.ToInt32(bTempRev, 0));
                }
                else if (fi.FieldType == typeof(long))
                {
                    bTemp = BitConverter.GetBytes((long) fi.GetValue(stuff));
                    bTempRev = bTemp.Reverse<byte>().ToArray<byte>();
                    fi.SetValueDirect(__makeref(stuff), BitConverter.ToInt64(bTempRev, 0));
                }
                else if (fi.FieldType == typeof(ushort))
                {
                    bTemp = BitConverter.GetBytes((ushort) fi.GetValue(stuff));
                    bTempRev = bTemp.Reverse().ToArray();
                    fi.SetValueDirect(__makeref(stuff), BitConverter.ToUInt16(bTempRev, 0));
                }
                else if (fi.FieldType == typeof(uint))
                {
                    bTemp = BitConverter.GetBytes((uint) fi.GetValue(stuff));
                    bTempRev = bTemp.Reverse<byte>().ToArray<byte>();
                    fi.SetValueDirect(__makeref(stuff), BitConverter.ToUInt32(bTempRev, 0));
                }
                else if (fi.FieldType == typeof(ulong))
                {
                    bTemp = BitConverter.GetBytes((ulong) fi.GetValue(stuff));
                    bTempRev = bTemp.Reverse<byte>().ToArray<byte>();
                    fi.SetValueDirect(__makeref(stuff), BitConverter.ToUInt64(bTempRev, 0));
                }
            }

            return stuff;
        }

        private static T ReadStreamToStructureBE<T>(Stream fs) where T : struct
        {
            byte[] array = new byte[Marshal.SizeOf(typeof(T))];
            fs.Read(array, 0, Marshal.SizeOf(typeof(T)));
            return ByteArrayToStructureBE<T>(array);
        }

        public static byte[] StructureToByteArray(object obj)
        {
            int len = Marshal.SizeOf(obj);
            byte[] arr = new byte[len];
            IntPtr ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, arr, 0, len);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        #endregion

        #region Extension

        public static string GetDescription<T>(this T val) where T : IConvertible
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[]) val
                .GetType()
                .GetField(val.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static void SeekTo(this Stream stream, long Position)
        {
            stream.Seek(Position, SeekOrigin.Begin);
        }

        public static void SeekTo(this BinaryReader br, long Position)
        {
            br.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public static void SeekTo(this BinaryWriter bw, long Position)
        {
            bw.BaseStream.Seek(Position, SeekOrigin.Begin);
        }

        public static long Position(this Stream stream)
        {
            return stream.Position;
        }

        public static long Position(this BinaryReader br)
        {
            return br.BaseStream.Position;
        }

        public static long Position(this BinaryWriter bw)
        {
            return bw.BaseStream.Position;
        }

        #endregion

        #region Dictionary -> Combobox Utility

        public static void SetComboBoxDataSource(ComboBox cb, object src)
        {
            cb.DataSource = new BindingSource(src, null);
            cb.DisplayMember = "Value";
            cb.ValueMember = "Key";
        }

        public static void SetSortedIndex<T>(ComboBox cb, int idx)
        {
            var src = (BindingSource) cb.DataSource;
            var dict = (Dictionary<int, T>) src.DataSource;

            cb.SelectedItem = new KeyValuePair<int, T>(idx, dict[idx]);
        }

        public static int GetSortedKey<T>(ComboBox cb)
        {
            return ((KeyValuePair<int, T>) cb.SelectedItem).Key;
        }

        public static T GetSortedValue<T>(ComboBox cb)
        {
            return ((KeyValuePair<int, T>) cb.SelectedItem).Value;
        }

        #endregion

        #region File I/O

        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public static long GetFileSize(string sPath)
        {
            return new FileInfo(sPath).Length;
        }

        public static long GetFileSizeAlign(string sPath, uint alignment)
        {
            long size = GetFileSize(sPath);

            size = size + (size % alignment);

            return size;
        }

        public static bool CompareByteArrays(byte[] first, int firstIndex, byte[] second, int secondIndex, int length)
        {
            if (first.Length < length || second.Length < length) return false;

            for (int i = 0; i < length; i++)
                if (first[firstIndex + i] != second[secondIndex + i])
                    return false;

            return true;
        }

        public static bool CompareByteArrays(byte[] first, byte[] second)
        {
            if (first.Length != second.Length) return false;
            else
                for (int i = 0; i < first.Length; i++)
                    if (first[i] != second[i])
                        return false;

            return true;
        }

        public static bool CompareByteArrays(byte[] first, byte[] second, int startIndex)
        {
            if (first.Length != second.Length) return false;
            else
                for (int i = startIndex; i < first.Length; i++)
                    if (first[i] != second[i])
                        return false;

            return true;
        }

        public static bool IsArrayZero(byte[] data)
        {
            bool result = true;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != 0x0)
                {
                    result = false;
                }
            }

            return result;
        }

        public static string ReadCString(BinaryReader br, int MaxLength = -1, long lOffset = -1, Encoding enc = null)
        {
            string result = "";

            int Max = MaxLength == -1 ? 4096 : MaxLength;

            long fTemp = br.BaseStream.Position;
            int i = 0;

            if (lOffset > -1)
            {
                br.BaseStream.Seek(lOffset, SeekOrigin.Begin);
            }

            do
            {
                var bTemp = br.ReadByte();
                if (bTemp == 0)
                    break;
                i += 1;
            } while (i < Max);

            Max = MaxLength == -1 ? i + 1 : MaxLength;

            if (lOffset > -1) //jump to offset
            {
                br.BaseStream.Seek(lOffset, SeekOrigin.Begin);

                result = enc == null ? Encoding.ASCII.GetString(br.ReadBytes(i)) : enc.GetString(br.ReadBytes(i));

                br.BaseStream.Seek(fTemp, SeekOrigin.Begin);
            }
            else
            {
                br.BaseStream.Seek(fTemp, SeekOrigin.Begin);

                result = enc == null ? Encoding.ASCII.GetString(br.ReadBytes(i)) : enc.GetString(br.ReadBytes(i));

                br.BaseStream.Seek(fTemp + Max, SeekOrigin.Begin);
            }

            return result;
        }

        public static int Align(int value, int alignment, bool AllowZero = true)
        {
            if (value % alignment != 0 || AllowZero == false)
            {
                return value + alignment - (value % alignment);
            }
            else
                return value;
        }

        public static uint Align(uint value, int alignment, bool AllowZero = true)
        {
            if (value % alignment != 0 || AllowZero == false)
            {
                return (uint) (value + alignment - (value % alignment));
            }
            else
                return value;
        }

        public static long Align(long value, int alignment, bool AllowZero = true)
        {
            if (value % alignment != 0 || AllowZero == false)
            {
                return (long) (value + alignment - (value % alignment));
            }
            else
                return value;
        }

        public static void WritePadding(BinaryWriter bw, int count)
        {
            for (int i = 0; i < count; i++)
            {
                bw.Write((byte) 0);
            }
        }

        public static void WritePadding(BinaryWriter bw, long count)
        {
            for (int i = 0; i < count; i++)
            {
                bw.Write((byte) 0);
            }
        }

        #endregion

        #region Data I/O
        
        public static uint CalcChecksum32(byte[] data)
        {
            uint result = 0;

            for (int i = 0; i < data.Length; i++)
            {
                result += data[i];
            }

            return result;
        }
        
        public static string Array2String(sbyte[] data, string sep = null, int start = -1, int len = -1)
        {
            string result = "";
            var length = len > 0 ? len : data.Length;
            var pos = start > 0 ? start : 0;

            for (int i = 0; i < length; i++)
            {
                result += $"{data[pos + i]:X2}";
                if (sep != null)
                    result += sep;
            }

            return result;
        }

        public static string Array2String(byte[] data, string sep = null, int start = -1, int len = -1)
        {
            string result = "";
            var length = len > 0 ? len : data.Length;
            var pos = start > 0 ? start : 0;

            for (int i = 0; i < length; i++)
            {
                result += $"{data[pos + i]:X2}";
                if (sep != null)
                    result += sep;
            }

            return result;
        }
        
        public static string Array2String(short[] data, string sep = null, int start = -1, int len = -1)
        {
            string result = "";
            var length = len > 0 ? len : data.Length;
            var pos = start > 0 ? start : 0;

            for (int i = 0; i < length; i++)
            {
                result += $"{data[pos + i]:X4}";
                if (sep != null)
                    result += sep;
            }

            return result;
        }
              
        public static string Array2String(ushort[] data, string sep = null, int start = -1, int len = -1)
        {
            string result = "";
            var length = len > 0 ? len : data.Length;
            var pos = start > 0 ? start : 0;

            for (int i = 0; i < length; i++)
            {
                result += $"{data[pos + i]:X4}";
                if (sep != null)
                    result += sep;
            }

            return result;
        }

        public static byte[] String2Array(string data)
        {
            string trimmed = data.Trim();

            byte[] result = new byte[trimmed.Length / 2];

            for (int i = 0; i < (trimmed.Length / 2); i++)
            {
                result[i] = byte.Parse(trimmed.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return result;
        }

        public static int NumberCompare(int number1, int number2)  
        {  
            int result;  
            if (number1 > number2)  
            {  
                result = 1;  
            }  
            else if (number1 < number2)  
            {  
                result = -1;  
            }  
            else  
            {  
                result = 0;  
            }  
            return result;  
        }
        
        public static byte[] DecompressGzip(byte[] data)
        {
            const int BufferSize = 4096;

            byte[] buffer = new byte[BufferSize];

            using (var stream = new MemoryStream(data))
            using (var decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
            using (var memory = new MemoryStream())
            {
                int count = 0;
                do
                {
                    count = decompressionStream.Read(buffer, 0, BufferSize);
                    if (count > 0)
                    {
                        memory.Write(buffer, 0, count);
                    }
                }
                while (count > 0);
                buffer = memory.ToArray();
            }

            return buffer;
        }

        #endregion

        #region FlagTable

        public static byte GetFlagTableByte(CheckedListBox clb)
        {
            byte result = 0;

            for (int i = 0; i < clb.Items.Count; i++)
            {
                if (clb.GetItemChecked(i))
                {
                    result |= (byte)(1 << i);
                }
            }

            return result;
        }
        
        public static void SetFlagTableByte(CheckedListBox clb, byte flags)
        {
            for (int i = 0; i < clb.Items.Count; i++) 
                clb.SetItemChecked(i, (flags & (1 << i)) > 0);
        }
        
        public static uint GetFlagTableUint(CheckedListBox clb)
        {
            uint result = 0;

            for (int i = 0; i < clb.Items.Count; i++)
            {
                if (clb.GetItemChecked(i))
                {
                    result |= (1u << i);
                }
            }

            return result;
        }
         
        public static void SetFlagTableUint(CheckedListBox clb, uint flags)
        {
            for (int i = 0; i < clb.Items.Count; i++) 
                clb.SetItemChecked(i, (flags & (1u << i)) > 0);
        }


        public static byte[] GetFlagTableArray(CheckedListBox clb)
        {
            int count = clb.Items.Count / 8;

            byte[] result = new byte[count];

            for (int idx = 0; idx < count; idx++)
            {
                byte temp = 0;

                for (int i = 0; i < 8; i++)
                {
                    var index = (idx * 8) + i;

                    if (clb.GetItemChecked(index))
                    {
                        temp |= (byte)(1 << i);
                    }
                }

                result[idx] = temp;
            }

            return result;
        }
        
        public static sbyte[] GetFlagTableArrayS(CheckedListBox clb)
        {
            int count = clb.Items.Count / 8;

            sbyte[] result = new sbyte[count];

            for (int idx = 0; idx < count; idx++)
            {
                sbyte temp = 0;

                for (int i = 0; i < 8; i++)
                {
                    var index = (idx * 8) + i;

                    if (clb.GetItemChecked(index))
                    {
                        temp |= (sbyte)(1 << i);
                    }
                }

                result[idx] = temp;
            }

            return result;
        }

        public static void FillFlagTableArray(CheckedListBox clb, byte[] data)
        {
            for (int idx = 0; idx < data.Length; idx++)
            {
                for (int i = 0; i < 8; i++)
                {
                    var index = (idx * 8) + i;

                    clb.SetItemChecked(index, (data[idx] & (1 << (i))) > 0);
                }
            }
        }
        
        public static void FillFlagTableArray(CheckedListBox clb, sbyte[] data)
        {
            for (int idx = 0; idx < data.Length; idx++)
            {
                for (int i = 0; i < 8; i++)
                {
                    var index = (idx * 8) + i;

                    clb.SetItemChecked(index, (data[idx] & (1 << (i))) > 0);
                }
            }
        }

        #endregion


    }
}