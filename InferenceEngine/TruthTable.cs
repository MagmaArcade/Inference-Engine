using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    internal class TruthTable : Algorithms
    {
        private string[] _hornkb; // array of strings that contain horn clauses
        private string _query; // query string (goal state)
        private string[] _propositionSymbol;
        private int[,] _truthtable;


        public TruthTable(string[] HornKB, string Query, string[] PropositionSymbol) : base(HornKB, Query, PropositionSymbol)
        {
            _hornkb = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
            int numbits = _propositionSymbol.Length;

            int[] _binaryStrings = new int[ numbits ]; // new int array for processing each bit using recursion/backchannelling (temporary variable, does not need a field)
            _truthtable = new int[numbits, (int)Math.Pow(2, numbits)]; // the actual 2D int array which stores every combination of true/false for each symbol, will be written to & read off

             generateBinaryStrings(_propositionSymbol.Length, _binaryStrings, 0, 0);
        }

        public void generateBinaryStrings(int n, int[] bitarray, int arrpos, int rowcount) // credit for core backchannelling framework: https://www.geeksforgeeks.org/generate-all-the-binary-strings-of-n-bits/
        {
            if (arrpos == n)
            {
                addToTruthTable(n, bitarray, rowcount);
                return;
            }

            bitarray[arrpos] = 0;
            generateBinaryStrings(n, bitarray, arrpos + 1, rowcount);

            bitarray[arrpos] = 1;
            generateBinaryStrings(n, bitarray, arrpos + 1, rowcount);
        }

        public void addToTruthTable(int n, int[] bitarray, int rowcount)
        {
            for (int i = 0; i < n; i++)
            {
                _truthtable[i, rowcount] = bitarray[i];
                Console.Write(_truthtable[i, rowcount] + " "); // the two Console.Write statements were used in development to print out the truth table for each symbol
            }
            Console.WriteLine();

            rowcount++;

            return;
        }
    }
}
