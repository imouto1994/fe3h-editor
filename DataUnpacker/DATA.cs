using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataUnpacker
{
    public class DATA
    {
        public const int DATA0_ENTRY_SIZE = 0x20; 

        List<DATA0_ENTRY> Entries;

        public struct DATA0_ENTRY
        {
            public long Offset, USize, CSize, IsCompressed;

            public void Load(BinaryReader br)
            {
                Offset = br.ReadInt64();
                USize = br.ReadInt64();
                CSize = br.ReadInt64();
                IsCompressed = br.ReadInt64();
            }
        }

        public void Parse(string sPath)
        {
            string sWorkDir = Path.GetDirectoryName(sPath) + "\\";
            string sPathToDATA0 = sWorkDir + "DATA0.bin";
            string sPathToDATA1 = sWorkDir + "DATA1.bin";

            sWorkDir += "DATA\\";

            if (File.Exists(sPathToDATA0) && File.Exists(sPathToDATA1))
            {
                Util.CreateDir(sWorkDir);

                using (var bin_br = new BinaryReader(File.OpenRead(sPathToDATA1)))
                using (var idx_br = new BinaryReader(File.OpenRead(sPathToDATA0)))
                {
                    //INDEX
                    int count = (int)idx_br.BaseStream.Length / DATA0_ENTRY_SIZE;
                    Entries = new List<DATA0_ENTRY>();

                    for (int i = 0; i < count; i++)
                    {
                        var temp = new DATA0_ENTRY();
                        temp.Load(idx_br);
                        Entries.Add(temp);
                    }

                    //DATA
                    for (int i = 0; i < count; i++)
                    {
                        if(i == 50) break; //we only need the first few files

                        var genericName = $"FILE_{i:D5}.dat";

                        if (File.Exists(sWorkDir + genericName)) continue;

                        bin_br.SeekTo(Entries[i].Offset);

                        byte[] PayloadData = null;

                        if (Entries[i].IsCompressed == 1) //TODO: find a way to decompress data
                        {
                            if (Entries[i].CSize > 0)
                            {
                                PayloadData = bin_br.ReadBytes((int)Entries[i].CSize);
                            }
                        }
                        else
                        {
                            if (Entries[i].USize > 0)
                            {
                                PayloadData = bin_br.ReadBytes((int)Entries[i].USize);
                            }
                        }

                        if (PayloadData != null)
                        {
                            Console.Write($"Writing {genericName}... ");
                            File.WriteAllBytes(sWorkDir + genericName, PayloadData);
                        }

                    }

                    Console.WriteLine("Done!");
                }
            }
        }






    }
}
