﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{
    class BackwardChaining
    {
        private string[] _hornKB;            // Holds the Horn clauses
        private string _query;               // Represents the goal state to be proven
        private string[] _propositionSymbol; // Contains the proposition symbols
        private List<string> _path;          // Stores the path taken to prove the goal state

        public BackwardChaining(string[] HornKB, string Query, string[] PropositionSymbol)
        {
            _hornKB = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
            _path = new List<string>();

            printResults(); // Calls the method to perform backward chaining and print the results
        }

        public void printResults()
        {
            bool result = backwardChainingAlg(_query); // Calls the backward chaining algorithm
            if (result)
            {
                Console.WriteLine("YES: " + string.Join(", ", _path)); // Prints the path taken to prove the goal state
            }
            else
            {
                Console.WriteLine("NO"); // Prints NO if the goal state cannot be proven
            }
        }

        private bool backwardChainingAlg(string query)
        {
            if (!_propositionSymbol.Contains(query))
            {
                Console.WriteLine("Invalid query!"); // Prints an error message if the query is not a valid proposition symbol
                return false;
            }

            if (_hornKB.Contains(query))
            {
                _path.Add(query); // Adds the goal state to the path
                return true;      // Returns true if the goal state is already in the Horn clauses
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
                        if (!backwardChainingAlg(symbol))
                        {
                            result = false; // Returns false if any premise symbol cannot be proven
                            break;
                        }
                    }

                    if (result)
                    {
                        _path.Add(query); // Inserts the goal state at the beginning of the path
                        return true;            // Returns true if the goal state can be proven based on the premises
                    }
                }
            }

            return false; // Returns false if the goal state cannot be proven
        }
    }
}