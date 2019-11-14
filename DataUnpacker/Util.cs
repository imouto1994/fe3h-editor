using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataUnpacker
{
    public static class MyExtensions
    {
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

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }

    }

    public static class Util
    {
        
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        public static void CreateDir(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        
        public static void DeleteDir(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);
        }
        
        public static byte[] GetArray(string infile, long position, int length)
        {
            byte[] result = null;
            using (var br = new BinaryReader(File.OpenRead(infile)))
            {
                br.BaseStream.Seek(position, SeekOrigin.Begin);
                result = br.ReadBytes(length);
            }
            return result;
        }

        public static byte[] GetArray(byte[] data, long position, int length)
        {
            byte[] result = new byte[length];

            if (position > data.Length || position + length > data.Length)
                throw new ArgumentOutOfRangeException();

            Array.Copy(data, position, result, 0, length);

            return result;
        }
        
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
        
        public static void WritePadding(BinaryWriter bw, int count)
        {
            for (int i = 0; i < count; i++)
            {
                bw.Write((byte)0);
            }
        }

        public static void WritePadding(BinaryWriter bw, long count)
        {
            for (int i = 0; i < count; i++)
            {
                bw.Write((byte)0);
            }
        }

        public static bool IsAligned(long value, int alignmentSize)
        {
            return (value & (long)(alignmentSize - 1)) == 0L;
        }

        public static long AlignUp(long value, int alignmentSize)
        {
            return value + (long)(alignmentSize - 1) & (long)(~(long)(alignmentSize - 1));
        }

        public static long AlignDown(long value, int alignmentSize)
        {
            return value & (long)(~(long)(alignmentSize - 1));
        }

    }
}
