﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace InferenceEngine
{
    class BackwardChaining
    {
        private string[] _hornKB;            // Holds the Horn clauses
        private string _query, _errMsg;               // Represents the goal state to be proven
        private string[] _propositionSymbol; // Contains the proposition symbols
        private List<string> _inferredSymbols;          // Stores the path taken to prove the goal state
        private int _loopCounter; // put in place to avoid a stack overflow that comes from faulty propositional logic

        public BackwardChaining(string[] HornKB, string Query, string[] PropositionSymbol)
        {
            _hornKB = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;
            _inferredSymbols = new List<string>();
            _loopCounter = 0;

            printResults(); // Calls the method to perform backward chaining and print the results
        }

        public void printResults()
        {
            bool result = backwardChainingAlg(_query); // Calls the backward chaining algorithm
            if (result)
            {
                Console.WriteLine("YES: " + string.Join(", ", _inferredSymbols)); // Prints the path taken to prove the goal state
            }
            else
            {
                Console.WriteLine("NO"); // Prints NO if the goal state cannot be proven
                // Console.WriteLine(_errMsg);  Gives more information on the error, had to be removed for batch testing requirements!
            }
        }

        private bool backwardChainingAlg(string query)
        {
                if (_loopCounter > 5999) // prevents a buffer overflow exception from being thrown because of a logic loop in the sentences - number not exact, found through trial and error
                {
                    _errMsg = "Infinite Loop!";
                    return false;
                }
                _loopCounter++;

            if (!_propositionSymbol.Contains(query))
            {
                _errMsg = "Invalid query!"; // Prints an error message if the query is not a valid proposition symbol
                return false;
            }

            if (_hornKB.Contains(query))
            {
                _inferredSymbols.Add(query); // Adds the goal state to the path
                return true;      // Returns true if the goal state is already in the Horn clauses
            }

            foreach (string rule in _hornKB)
            {
                string[] implication = rule.Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries);
                string premise = implication[0];
                string conclusion = "";

                if (implication.Length > 1)
                {
                    conclusion = implication[1];
                }

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

                        _inferredSymbols.Add(query); // Inserts the goal state at the beginning of the path
                        _inferredSymbols = _inferredSymbols.Distinct().ToList(); // removes any duplicates
                        return true;            // Returns true if the goal state can be proven based on the premises
                        
                    }
                }
            }

            _errMsg = "No solution!";
            return false; // Returns false if the goal state cannot be proven
        }
    }
}