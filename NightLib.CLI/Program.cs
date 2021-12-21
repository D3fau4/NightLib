using System;
using System.Collections.Generic;
using System.Linq;
using NightLib.Files.BND;

namespace NightLib.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            switch (args[0])
            {
                case "--extract":
                    BND _bnd = new BND(args[1]);
                    Unpacker unpacker = new Unpacker(_bnd);
                    unpacker.Unpack();
                    break;
                case "--build":
                    Repacker repacker = new Repacker(args[1], args[2], args[3], args[4]);
                    repacker.Build();
                    break;
            }
        }
    }
}