using System;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{
    class ForwardChaining
    {
        private string[] _hornKB;             // Holds the Horn clauses
        private string _query, errMsg;                // Represents the goal state to be proven
        private string[] _propositionSymbol;  // Contains the proposition symbols
        private List<string> _inferredSymbols; // Stores the path taken to prove the goal state

        public ForwardChaining(string[] HornKB, string Query, string[] PropositionSymbol)
        {
            _hornKB = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
            _inferredSymbols = new List<string>();

            PrintResults(); // Calls the method to perform forward chaining and print the results
        }

        public void PrintResults()
        {
            bool result = ForwardChainingAlg(_query); // Calls the forward chaining algorithm
            if (result)
            {
                Console.WriteLine("YES: " + string.Join(", ", _inferredSymbols)); // Prints the path taken to prove the goal state
            }
            else
            {
                Console.WriteLine("NO: " + errMsg); // Prints NO if the goal state cannot be proven
            }
        }

        private bool ForwardChainingAlg(string query)
        {
            if (!_propositionSymbol.Contains(query))
            {
                errMsg = "Invalid Query!"; // Prints an error message if the query is not a valid proposition symbol
                return false;
            }

            if (_hornKB.Contains(query))
            {
                _inferredSymbols.Add(query); // Adds the goal state to the path
                return true;      // Returns true if the goal state is already in the Horn clauses
            }

            foreach(string symbol in _propositionSymbol)
{
                foreach (string rule in _hornKB)
                {
                    string[] implication = rule.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                    string premise = implication[0];

                    if (implication.Length == 1) // atomic propositions
                    {
                        _inferredSymbols.Add(premise); // Adds the premise to the inferred symbols
                                                       // as if it is a Proposition Symbol without any conclusion, it is true
                    }
                    else if (implication.Length == 2)
                    {
                        string conclusion = implication[1];

                        if (premise.Contains("&"))
                        {
                            string[] premises = premise.Split('&');

                            bool allPremisesInferred = premises.All(p => _inferredSymbols.Contains(p.Trim()));
                            if (allPremisesInferred)
                            {
                                _inferredSymbols.Add(conclusion); // Adds the conclusion to the inferred symbols
                            }
                        }
                        else if (_inferredSymbols.Contains(premise))
                        {
                            _inferredSymbols.Add(conclusion); // Adds the conclusion to the inferred symbols
                        }


                        foreach (string inferredSymbol in _inferredSymbols)
                        {
                            if (inferredSymbol == _query)
                            {
                                return true;
                            }
                        }
                    }
                }
            }


            errMsg = "NO Soloution";
            return false; // Returns false if the goal state cannot be proven
        }
    }
}


