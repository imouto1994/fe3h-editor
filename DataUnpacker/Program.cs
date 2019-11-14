using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataUnpacker
{
    class Program
    {
        static void ShowVersion()
        {
            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine("Fire Emblem - Three Houses - DATA Unpacker Tool v" + assemblyVersion + "\n");
        }

        static void Init()
        {
            //ShowVersion();
        }

        static void Exit(bool showFinished = false)
        {
            if (showFinished)
            {
                Console.WriteLine();
                Console.WriteLine("Finished, please press any key to exit...");
                Console.ReadKey();
            }

            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            Init();
            DATA data;

            //debug
            //args = new[] {@"F:\Switch\Games\!Nintendo\Fire Emblem Three Houses\romfs\DATA0.bin"};

            //extract DATA
            if (args.Length == 1 && File.Exists(args[0]) && 
            (args[0].EndsWith("DATA0.bin") || args[0].EndsWith("DATA1.bin")))
            {
                data = new DATA();
                data.Parse(args[0]);
                Exit();
            }

            //this is used to extract the sub-archives inside of the main files
            if (args.Length == 1 && File.Exists(args[0]))
            {
                MiniArchive data2 = new MiniArchive();
                data2.Unpack(args[0]);
                Exit();
            }

            Exit(true);
        }
    }
}
