using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    public class MiniArchive
    {
        public struct ENTRY
        {
            public long Offset, Size;

            public void Load(BinaryReader br)
            {
                Offset = br.ReadInt32();
                Size = br.ReadInt32();
            }
        }

        private byte[] ArchiveData;
        public List<ENTRY> Entries;

        public MiniArchive()
        {
            ArchiveData = null;
            Entries = new List<ENTRY>();
        }

        public void Load(byte[] data)
        {
            ArchiveData = new byte[data.Length];
            data.CopyTo(ArchiveData, 0);

            using (var ms = new MemoryStream(ArchiveData))
            using (var br = new BinaryReader(ms))
            {
                int count = br.ReadInt32();
                Entries = new List<ENTRY>();

                for (int i = 0; i < count; i++)
                {
                    var temp = new ENTRY();
                    temp.Load(br);
                    Entries.Add(temp);
                }
            }
        }

        public byte[] GetEntry(int idx)
        {
            byte[] result;

            using (var ms = new MemoryStream(ArchiveData))
            using (var br = new BinaryReader(ms))
            {
                br.SeekTo(Entries[idx].Offset);

                result = br.ReadBytes((int)Entries[idx].Size);
            }

            return result;
        }

    }
}
