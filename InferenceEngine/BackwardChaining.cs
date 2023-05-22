using System;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{
    class BackwardChaining
    {
        private string[] _hornKB;
        private string _query;
        private string[] _propositionSymbol;
        private List<string> _path;

        public BackwardChaining(string[] HornKB, string Query, string[] PropositionSymbol)
        {
            _hornKB = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
            _path = new List<string>();

            printResults();
        }

        public void printResults()
        {
            bool result = PL_BC_Entails(_query);
            if (result)
            {
                _path.Reverse();
                Console.WriteLine("YES: " + string.Join(", ", _path));
            }
            else
            {
                Console.WriteLine("NO");
            }
        }

        private bool PL_BC_Entails(string query)
        {
            if (!_propositionSymbol.Contains(query))
            {
                Console.WriteLine("Invalid query!");
                return false;
            }

            if (_hornKB.Contains(query))
            {
                _path.Add(query);
                return true;
            }

            foreach (string rule in _hornKB)
            {
                string[] implication = rule.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                string premise = implication[0].Trim();
                string conclusion = implication[1].Trim();

                if (conclusion == query)
                {
                    bool result = true;
                    string[] premises = premise.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string symbol in premises)
                    {
                        if (!PL_BC_Entails(symbol))
                        {
                            result = false;
                            break;
                        }
                    }

                    if (result)
                    {
                        _path.Insert(0, query);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}