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
            BND _bnd = new BND(args[0]);

            foreach (var i in _bnd._f)
            {
                Console.WriteLine(i.Name);
            }
        }
    }
}