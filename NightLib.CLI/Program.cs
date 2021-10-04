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
            BND _bnd = new BND(args[1]);
            switch (args[0])
            {
                case "--extract":
                    Unpacker unpacker = new Unpacker(_bnd);
                    unpacker.Unpack();
                    break;
            }
        }
    }
}