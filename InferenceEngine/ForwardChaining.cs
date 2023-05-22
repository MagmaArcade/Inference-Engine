using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    class ForwardChaining
    {
        private string[] _hornKB;
        private string _query;
        private string[] _propositionSymbol;
        private List<string> _path;

        public ForwardChaining(string[] HornKB, string Query, string[] PropositionSymbol)
        {
            _hornKB = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
            _path = new List<string>();

            PrintResults();
        }

        private void PrintResults()
        {
            bool result = ForwardChainingAlg(_query); // Calls the backward chaining algorithm
            if (result)
            {
                Console.WriteLine("YES: " + string.Join(", ", _path)); // Prints the path taken to prove the goal state
            }
            else
            {
                Console.WriteLine("NO"); // Prints NO if the goal state cannot be proven
            }
        }

        private bool ForwardChainingAlg(string query)
        { 

        }
    }
}
