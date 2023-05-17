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
        private List<int> _symbolTT;

        public TruthTable(string[] HornKB, string Query, string[] PropositionSymbol) : base(HornKB, Query, PropositionSymbol)
        {
            _hornkb = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
            _symbolTT = new List<int> { };
             generateAllTruthTable(_propositionSymbol.Count(), _symbolTT, Math.Pow(2, _propositionSymbol.Length));
        }

        static void generateAllTruthTable(int n, List<int> ttarray, double count)
        {
            if (count == 0)
            {
                return;
            }
            
            ttarray[n - 1] = 0;
            generateAllTruthTable(n - 1, ttarray, count - 1);
            ttarray[n - 1] = 1;
            generateAllTruthTable(n - 1, ttarray, count - 1);
        }
    }
}
