using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataUnpacker
{
    public class MiniArchive
    {
        List<ENTRY> Entries;

        public struct ENTRY
        {
            public long Offset, Size;

            public void Load(BinaryReader br)
            {
                Offset = br.ReadInt32();
                Size = br.ReadInt32();
            }
        }

        public void Unpack(string sPath)
        {
            string sWorkDir = Path.GetDirectoryName(sPath) + "\\" + Path.GetFileNameWithoutExtension(sPath) + "\\";

            if (File.Exists(sPath))
            {
                Util.CreateDir(sWorkDir);

                using (var br = new BinaryReader(File.OpenRead(sPath)))
                {
                    int count = br.ReadInt32();
                    Entries = new List<ENTRY>();

                    for (int i = 0; i < count; i++)
                    {
                        var temp = new ENTRY();
                        temp.Load(br);
                        Entries.Add(temp);
                    }

                    for (int i = 0; i < count; i++)
                    {
                        var genericName = $"FILE_{i:D3}.dat";

                        br.SeekTo(Entries[i].Offset);

                        byte[] PayloadData = br.ReadBytes((int)Entries[i].Size);

                        if (PayloadData != null)
                        {
                            Console.Write($"Writing {genericName}... ");
                            File.WriteAllBytes(sWorkDir + genericName, PayloadData);
                        }
                    }
                }
            }
         

        }
    }
}
