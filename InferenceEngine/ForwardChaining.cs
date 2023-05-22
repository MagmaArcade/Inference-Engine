using System;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{
    class ForwardChaining
    {
        private string[] _hornKB;             // Holds the Horn clauses
        private string _query;                // Represents the goal state to be proven
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
                Console.WriteLine("NO"); // Prints NO if the goal state cannot be proven
            }
        }

        private bool ForwardChainingAlg(string query)
        {
            if (!_propositionSymbol.Contains(query))
            {
                Console.WriteLine("Invalid query!"); // Prints an error message if the query is not a valid proposition symbol
                return false;
            }

            if (_hornKB.Contains(query))
            {
                _inferredSymbols.Add(query); // Adds the goal state to the path
                return true;      // Returns true if the goal state is already in the Horn clauses
            }

            foreach (string symbol in _propositionSymbol)
            {
                foreach (string rule in _hornKB)
                {
                    string[] implication = rule.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                    string premise = implication[0].Trim();
                    string conclusion = "";

                    if (premise == query)
                    {
                        if (implication.Count() == 1)
                        {
                            _inferredSymbols.Add(premise); // Adds the premise to the inferred symbols
                        }
                        else if (implication.Count() == 2)
                        {
                            conclusion = implication[1].Trim();

                            if (_inferredSymbols.Contains(premise))
                            {
                                string[] conclusionsi = conclusion.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string s in conclusionsi)
                                {
                                    _inferredSymbols.Add(s); // Adds the conclusion to the inferred symbols
                                }
                            }
                            else
                            {
                                continue; // Skips to the next iteration if the premise is not found in the inferred symbols
                            }
                        }

                        bool result = true;
                        string[] conclusions = conclusion.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string s in conclusions)
                        {
                            if (!ForwardChainingAlg(s))
                            {
                                result = false; // Returns false if any conclusion symbol cannot be proven
                                break;
                            }
                        }

                        if (result)
                        {
                            _inferredSymbols.Insert(0, query); // Inserts the goal state at the beginning of the path
                            return true;            // Returns true if the goal state can be proven based on the premises
                        }
                    }
                }
            }

            return false; // Returns false if the goal state cannot be proven
        }
    }
}
