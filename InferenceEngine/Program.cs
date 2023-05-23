using System;
using System.IO;
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
            // Code to ensure two arguments are passed in
            try
            {
                string test = args[0] + args[1];
            }
            catch
            {
                Console.WriteLine("Missing one or more arguments. Exiting program...");
                return;
            }

            string searchmethod = args[0].ToString();
            string filelocation = args[1].ToString();

            // Some code to catch rogue filenames and exit the program before passing it on and executing
            try
            {
                using (StreamReader reader = new StreamReader(filelocation))
                {
                    reader.ReadToEnd();
                }
            }
            catch
            {
                Console.WriteLine("File does not exist. Exiting program...");
                return;
            }

            ReadKB fileread = new ReadKB(filelocation);
            bool validFileRead = fileread.ParseHornKB();     // this is used to parse the raw data 

            // DEBUGGING
            //fileread.PrintEnvironData();   // this is used to test the input data and data parse

            if (validFileRead == true)
            {
                switch (args[0].ToLower())
                {
                    case "tt":
                        TruthTable tt = new TruthTable(fileread.HornKB, fileread.Query, fileread.PropositionSymbol);
                        break;

                    case "fc":
                        ForwardChaining fc = new ForwardChaining(fileread.HornKB, fileread.Query, fileread.PropositionSymbol);
                        break;

                    case "bc":
                        BackwardChaining bc = new BackwardChaining(fileread.HornKB, fileread.Query, fileread.PropositionSymbol);
                        break;

                    default:
                        Console.WriteLine("No search method called " + args[0] + ". Exiting program...");
                        break;
                }
            }
            else
            {
                Console.WriteLine("File is not correctly formatted. Try again.");
            }
        }
    }
}
