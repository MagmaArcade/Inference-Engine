using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ReadKB fileread = new ReadKB("test_HornKB.txt");
            fileread.ParseHornKB();
            fileread.PrintEnvironData();
            Console.ReadLine();

        }
    }
}
