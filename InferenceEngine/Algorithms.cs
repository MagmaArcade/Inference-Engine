using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    class Algorithms
    {
        private string[] _hornkb; // array of strings that contain horn clauses
        private string _query; // query string (goal state)
        private string[] _propositionSymbol;


        // Default Constructor
        public Algorithms(string[] HornKB, string Query, string[] PropositionSymbol)
        {
            _hornkb = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
        }

        public void TruthTable()
        {
            TruthTable tt = new TruthTable(_hornkb, _query, _propositionSymbol);
        }

        public string ForwardChanneling()
        {
            return null;

        }
        public string BackwardChanneling()
        {
            return null;

        }

    }
}

