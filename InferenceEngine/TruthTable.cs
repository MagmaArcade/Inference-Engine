﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    internal class TruthTable
    {
        private int[,] _truthtable;
        private int _rowcount; // making rowcount a field because backtracking/recursion keeps wiping my values
        private List<string[]> _postfixSentences; // list of arrays that contain kb sentences expressed in a postfix manner
        private int[,] _evaluatedPostfixSentences; // a 2D array of True/False values for each postfixed sentence
        private int[,] _evaluatedKnowledgeBase; // a 2D array storing conjoined sentence values + the value of the query for that model
        private string _errMsg;

        public TruthTable(string[] hornKB, string query, string[] propositionSymbol)
        {
            int numbits = propositionSymbol.Length;
            int[] _binaryStrings = new int[ numbits ]; // new int array for processing each bit using recursion/backchannelling (temporary variable, does not need a field)
            _truthtable = new int[numbits, (int)Math.Pow(2, numbits)]; // the actual 2D int array which stores every combination of true/false for each symbol, will be written to & read off

            _rowcount = 0;
            generateBinaryStrings(propositionSymbol.Length, _binaryStrings, 0); // assigns actual values to _truthtable - cannot return int[,] because it is a recursively called method, it is acts as void and assigns to a class field
            
            _postfixSentences = generatePostfixArrays(hornKB);

            _evaluatedPostfixSentences = evaluatePostfixSentences(_postfixSentences, _truthtable, propositionSymbol);

            _evaluatedKnowledgeBase = evaluateKnowledgeBase(_truthtable, _evaluatedPostfixSentences, query, propositionSymbol);

            printResults(_evaluatedKnowledgeBase);
        }

        public int[,] evaluateKnowledgeBase(int[,] truthtable, int[,] sentencevalues, string query, string[] propSymbolsList)
        {
            int[,] conjoinedKB = new int[2, sentencevalues.GetLength(1)]; // creates an array with two columns - one to carry the conjoined sentences and one to carry the query in that model

            // Grab the index for the query
            int queryIndex = -1;

            for (int i = 0; i < propSymbolsList.Length; i++) // gets the column value for the two symbols (used to look them up in the TT)
            {
                if (propSymbolsList[i] == query)
                {
                    queryIndex = i;
                }
            }

            // Begin iterating through the KB models
            for (int i = 0; i < sentencevalues.GetLength(1); i++)
            {
                int modelvalue = sentencevalues[0, i];

                for (int j = 1; j < sentencevalues.GetLength(0); j++)
                {
                    modelvalue = modelvalue & sentencevalues[j, i];
                }

                conjoinedKB[0, i] = modelvalue;
                
                // Console.Write(conjoinedKB[0, i].ToString());
                // Console.Write(" ");
                
                    // Small block of code to catch edge case where a query is asked that doesn't exist in the list
                    try
                    {
                        int testQuery = truthtable[queryIndex, i];
                    }
                    catch
                    {
                        _errMsg = ("Invalid query!");
                        break;
                    }

                conjoinedKB[1, i] = truthtable[queryIndex, i];
                
                // Console.Write(conjoinedKB[1, i].ToString());
                // Console.WriteLine();
            }
            return conjoinedKB;
        }

        public void printResults(int[,] kb)
        {
            List<int> doesEntail = new List<int>();

            for (int i = 0; i < kb.GetLength(1); i++) // if every value whre the KB is true, query is also true, then add 1 to the list
            {
                if (kb[0, i] == 1 && kb[1, i] == 1)
                {
                    doesEntail.Add(i);
                }
            }

            for (int i = 0; i < kb.GetLength(1); i++) // after this, if there is a value where KB is true but alpha is false, automatic no (wipe the list)
            {
                if (kb[0, i] == 1 && kb[1, i] == 0)
                {
                    doesEntail.Clear();
                    _errMsg = "No solution!";
                }
            }

            if (doesEntail.Count > 0)
            {
                Console.WriteLine("YES: {0}", doesEntail.Count());
            }
            else
            {
                Console.WriteLine("NO");
                // Console.WriteLine(_errMsg);  Gives more information on the error, had to be removed for batch testing requirements!
            }
        }

        public int[,] evaluatePostfixSentences(List<string[]> postfixSentences, int[,] truthtable, string[] propSymbols) // Great overview of postfix expressions and how to evaluate them using a stack: https://www2.cs.sfu.ca/CourseCentral/125/tjd/postfix.html
        {
            int[,] evaluations = new int[postfixSentences.Count(), truthtable.GetLength(1)]; ; // will create a new array with columns = sentences & # rows = the # of models shown in the TT

            for (int modelcount = 0; modelcount < truthtable.GetLength(1); modelcount++) // a for loop to go through every single model in the truth table
            {
                for (int sentencecount = 0; sentencecount < postfixSentences.Count(); sentencecount++)
                {
                    Stack<string> symbolstack = new Stack<string>(); // the stack used to evaluate each postfix array

                    if (postfixSentences[sentencecount].Length == 1)    // introduce the edge case for if the 'sentence' is simply a symbol
                    {
                        string symbol = postfixSentences[sentencecount][0];
                        int symbolIndex = 0; 

                        for (int i = 0; i < propSymbols.Length; i++) // Need to first find the symbols position in the TT array
                        {
                            if (propSymbols[i] == symbol)
                            {
                                symbolIndex = i;
                            }
                        }

                        evaluations[sentencecount, modelcount] = truthtable[symbolIndex, modelcount]; // associate that symbols T/F in the model with corresponding value
                        // Console.Write($"1 ");

                    }
                    else
                    {
                        for (int i = 0; i < postfixSentences[sentencecount].Length; i++)
                        {
                            if (postfixSentences[sentencecount][i] == "=>") // implication symbol
                            {
                                string b = symbolstack.Pop(); // order of the letters here is important - b is popped first but it has to be treated as the second symbol in the array
                                string a = symbolstack.Pop();
                                symbolstack.Push(evaluateImplication(truthtable, a, b, modelcount, propSymbols));
                            }
                            else if (postfixSentences[sentencecount][i] == "&") // conjunction symbol
                            {
                                string b = symbolstack.Pop();
                                string a = symbolstack.Pop();
                                symbolstack.Push(evaluateConjunction(truthtable, a, b, modelcount, propSymbols));
                            }
                            else
                            {
                                symbolstack.Push(postfixSentences[sentencecount][i]); // propositional symbols
                            }
                        }

                        string stacktop = symbolstack.Pop();
                        int evaluationresult = Int32.Parse(stacktop);
                        evaluations[sentencecount, modelcount] = evaluationresult;

                        // Console.Write($"{evaluationresult} ");
                    }
                }
                // Console.WriteLine();

            }

            return evaluations;
        }

        public string evaluateConjunction(int[,] truthtable, string a, string b, int count, string[] propSymbolsList)
        {
            int indexA = 0;
            int indexB = 0;

            for (int i = 0; i < propSymbolsList.Length; i++) // gets the column value for the two symbols (used to look them up in the TT)
            {
                if (propSymbolsList[i] == a)
                {
                    indexA = i;
                }

                if (propSymbolsList[i] == b)
                {
                    indexB = i;
                }
            }

            int result = truthtable[indexA, count] & truthtable[indexB, count];

                // Writing values to console for dev purposes
                /*Console.WriteLine($"the two symbols: {a}, {b}");
                Console.WriteLine($"the two positions: {indexA}, {indexB}");
                Console.WriteLine($"the values in model {count}");
                Console.WriteLine($"the resulting conjunction: {result}");
                Console.WriteLine("--------------------------------------");*/

            return result.ToString();
        }

        public string evaluateImplication(int[,] truthtable, string a, string b, int count, string[] propSymbolsList)
        {
            if (a == "0" | a == "1") // identifies if a conjunction has already been evaluated, as this needs to be processed differently
            {
                return evaluateImpAfterConj(truthtable, a, b, count, propSymbolsList);
            }
            else
            {
                int indexA = 0;
                int indexB = 0;

                for (int i = 0; i < propSymbolsList.Length; i++) // gets the column value for the two symbols (used to look them up in the TT)
                {
                    if (propSymbolsList[i] == a)
                    {
                        indexA = i;
                    }

                    if (propSymbolsList[i] == b)
                    {
                        indexB = i;
                    }
                }

                // Begin implication elimination
                int modelA = truthtable[indexA, count];

                // Negation of alpha
                if (modelA == 0)
                {
                    modelA = 1;
                }
                else
                {
                    modelA = 0;
                }

                int result = modelA | truthtable[indexB, count];

                return result.ToString();
            }
        }

        public string evaluateImpAfterConj(int[,] truthtable, string a, string b, int count, string[] propSymbolsList)
        {
            int indexB = 0; // only need a value for B, we already have A as True/False

            for (int i = 0; i < propSymbolsList.Length; i++) // gets the column value for the two symbols (used to look them up in the TT)
            {
                if (propSymbolsList[i] == b)
                {
                    indexB = i;
                }
            }

            // Begin implication elimination
            int modelA = Int32.Parse(a); // takes the string result of the evaluated conjunction and runs an impliccation through it

            // Negation of alpha
            if (modelA == 0)
            {
                modelA = 1;
            }
            else
            {
                modelA = 0;
            }

            int result = modelA | truthtable[indexB, count];

            return result.ToString();
        }

        public void generateBinaryStrings(int n, int[] bitarray, int arrpos) // credit for core backchannelling framework: https://www.geeksforgeeks.org/generate-all-the-binary-strings-of-n-bits/
        {

            if (arrpos == n)
            {
                addToTruthTable(n, bitarray);
                return;
            }

            bitarray[arrpos] = 0;
            generateBinaryStrings(n, bitarray, arrpos + 1);

            bitarray[arrpos] = 1;
            generateBinaryStrings(n, bitarray, arrpos + 1);
        }

        public void addToTruthTable(int n, int[] bitarray)
        {
            for (int i = 0; i < n; i++)
            {
                _truthtable[i, _rowcount] = bitarray[i];
                // Console.Write(_truthtable[i, _rowcount] + " "); // the two Console.Write statements were used in development to print out the truth table for each symbol
            }
            // Console.WriteLine();

            _rowcount = _rowcount + 1;

            return;
        }

        public List<string[]> generatePostfixArrays(string[] hornkb)
        {
            List<string[]> postfixes = new List<string[]> { };

            foreach (string sentence in hornkb)
            {
                if (sentence.Contains("=>") && sentence.Contains("&")) // contains both implication and conjunction
                {
                    string[] splitimp = splitAtImplication(sentence);
                    string [] splitconj = splitAtConjunction(splitimp);
                    postfixes.Add(splitconj);
                }
                else if (sentence.Contains("=>") && !sentence.Contains("&")) // contains just an implication
                {
                    postfixes.Add(splitAtImplication(sentence));
                }
                else
                {
                    string[] tempsymbol = new string[] { sentence };
                    postfixes.Add(tempsymbol); // just adds a single symbol in the form of an array to the list
                }
            }

            // printPostfixedSentences(postfixes); // function call to print to console for dev purposes

            return postfixes;
        }

        public string[] splitAtImplication(string target)
        {
            //gets all individual variables 
            string[] delimiters = new string[] {"=>", " "};
            string[] result = target.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            result = result.Append("=>").ToArray(); // ensures the symbol makes it into the postfix array

            return result;
        }

        public string[] splitAtConjunction(string[] impsplit)
        {   
            string[] delimiters = new string[] { "&", " " };
            string[] result = impsplit[0].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            result = result.Append("&").ToArray();

            List<string> arrayAsList = impsplit.ToList(); // explicit conversion into list to allow for the remove/insert functions
            arrayAsList.RemoveAt(0); // removes the old "symbol&symbol" split in the last function, in order to make room for the new, postfix version

            for (int i = 2; i >= 0; i--)
            {
                arrayAsList.Insert(0, result[i]); // will add each element of the postfixed conjunction sentence to the front of the list (in reverse order as it always adds to the front)
            }

            string[] returnArray = arrayAsList.ToArray(); // explicit cast back into string[] to suit return value

            return returnArray;
        }

        // the following code printed out postfixed versions of each
        // sentence for development purposes
        public void printPostfixedSentences(List<string[]> postfixSentences)
        {
            Console.WriteLine("The KB sentences after being placed into postfix arrays:");
            foreach (string[] i in postfixSentences)
            {
                foreach (string n in i)
                {
                    Console.Write(n);
                    Console.Write(" ");
                }

                Console.WriteLine();
            }
            Console.WriteLine("---------------------------------");
        }
    }
}
