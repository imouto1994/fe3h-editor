using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveEditor.Structs
{
    public class StringDB
    {
        public struct Header
        {
            public uint unk1, unk2;

            public ushort Count;
            public ushort PtrSize;

            public uint Size, unk4;

            public void Read(BinaryReader br)
            {
                unk1 = br.ReadUInt32();
                unk2 = br.ReadUInt32();
                Count = br.ReadUInt16();
                PtrSize = br.ReadUInt16();
                Size = br.ReadUInt32();
                unk4 = br.ReadUInt32();
            }
        }

        private Header h;
        private List<uint> Offsets;
        public List<string> Strings;

        public StringDB()
        {
            h = new Header();
            Offsets = new List<uint>();
            Strings = new List<string>();
        }

        public void Load(enmLanguage language, int idx)
        {
            h = new Header();
            Offsets = new List<uint>();

            MiniArchive msg = new MiniArchive();
            msg.Load(Util.DecompressGzip(Properties.Resources.msgdata_bin));

            byte[] langdata = msg.GetEntry((int)language);

            MiniArchive lang = new MiniArchive();
            lang.Load(langdata);

            using (var ms = new MemoryStream(lang.GetEntry(idx)))
            using (var br = new BinaryReader(ms))
            {
                h.Read(br);

                for (int i = 0; i < h.Count; i++)
                {
                    Offsets.Add(br.ReadUInt32());
                }

                for (int i = 0; i < h.Count; i++)
                {
                    br.SeekTo(h.Size + Offsets[i]);

                    Strings.Add(Util.ReadCString(br, enc: Encoding.UTF8));
                }
            }
        }



    }
}
