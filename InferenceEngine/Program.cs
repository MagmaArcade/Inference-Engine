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
            ReadKB fileread = new ReadKB(args[1]);

            fileread.ParseHornKB();     // this is used to parse the raw data 

            fileread.PrintEnvironData();   // this is used to test the input data and data parse


            switch (args[0].ToLower())
            {
                case "tt":
                    TruthTable tt = new TruthTable(fileread.HornKB, fileread.Query, fileread.PropositionSymbol);
                    break;

                case "fc":
                    ForwardChannelling fc = new ForwardChannelling(fileread.HornKB, fileread.Query, fileread.PropositionSymbol);
                    break;

                case "bc":
                    BackwardChannelling bc = new BackwardChannelling(fileread.HornKB, fileread.Query, fileread.PropositionSymbol);
                    break;

                default:
                    Console.WriteLine("No search method called " + args[0]);
                    break;

            }

            Console.ReadLine();

        }
    }
}
