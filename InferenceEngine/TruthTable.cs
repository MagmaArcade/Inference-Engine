using System;
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
        private int[] _symbolTT;

        public TruthTable(string[] HornKB, string Query, string[] PropositionSymbol) : base(HornKB, Query, PropositionSymbol)
        {
            _hornkb = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
            _symbolTT = new int[] { };
            generateAllBinaryStrings(_propositionSymbol.Count(), _symbolTT, 0);
        }

        static void generateAllBinaryStrings(int n, int[] arr, int i)
        {
            if (i == n)
            {
                for (int c = 0; c < n; c++)
                {
                    Console.Write(arr[c] + " ");
                }
                Console.WriteLine();
            }

            // First assign "0" at ith position
            // and try for all other permutations
            // for remaining positions
            arr[i] = 0;
            generateAllBinaryStrings(n, arr, i + 1);

            // And then assign "1" at ith position
            // and try for all other permutations
            // for remaining positions
            arr[i] = 1;
            generateAllBinaryStrings(n, arr, i + 1);
        }
    }
}
