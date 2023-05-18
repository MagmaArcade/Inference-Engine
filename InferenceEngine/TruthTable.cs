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
        private int[,] _truthtable;
        private List<string[]> _postfixSentences; // list of arrays that contain kb sentences expressed in a postfix manner
        private int[,] _evaluatedPostfixSentences; // a 2D array of True/False values for each postfixed sentence


        public TruthTable(string[] HornKB, string Query, string[] PropositionSymbol) : base(HornKB, Query, PropositionSymbol)
        {
            _hornkb = HornKB;
            _query = Query;
            _propositionSymbol = PropositionSymbol;

                int numbits = _propositionSymbol.Length;
                int[] _binaryStrings = new int[ numbits ]; // new int array for processing each bit using recursion/backchannelling (temporary variable, does not need a field)
                _truthtable = new int[numbits, (int)Math.Pow(2, numbits)]; // the actual 2D int array which stores every combination of true/false for each symbol, will be written to & read off

            generateBinaryStrings(_propositionSymbol.Length, _binaryStrings, 0, 0); // assigns actual values to _truthtable - cannot return int[,] because it is a recursively called method, it is acts as void and assigns to a class field
            
            _postfixSentences = generatePostfixArrays(_hornkb);

            _evaluatedPostfixSentences = evaluatePostfixSentences(_postfixSentences, _truthtable, _propositionSymbol);

        }

        public int[,] evaluatePostfixSentences(List<string[]> postfixSentences, int[,] truthtable, string[] propSymbols) // Great overview of postfix expressions and how to evaluate them using a stack: https://www2.cs.sfu.ca/CourseCentral/125/tjd/postfix.html
        {
            int[,] evaluations = new int[postfixSentences.Count(), truthtable.GetLength(1)]; ; // will create a new array with columns = sentences & # rows = the # of models shown in the TT

            for (int modelcount = 2047; modelcount < truthtable.GetLength(1); modelcount++) // a for loop to go through every single model in the truth table
                //
                // ATTENTION NICHOLAS - THIS HAS BEEN CHANGED TO 2047 FOR DEBUGGING - WHEN ITS NOT WORKING CHANGE IT BACK TO 0
                //
            {
                foreach (string[] postfix in postfixSentences)
                {
                    Stack<string> symbolstack = new Stack<string>(); // the stack used to evaluate each postfix array

                    for (int i = 0; i < postfix.Length; i++)
                    {
                        if (postfix[i] == "=>") // implication symbol
                        {
                            string b = symbolstack.Pop(); // order of the letters here is important - b is popped first but it has to be treated as the second symbol in the array
                            string a = symbolstack.Pop();
                            symbolstack.Push(evaluateImplication(truthtable, a, b, modelcount, propSymbols));
                            // Console.WriteLine(symbolstack.Peek());
                        }
                        else if (postfix[i] == "&") // conjunction symbol
                        {
                            string b = symbolstack.Pop(); // order of the letters here is important - b is popped first but it has to be treated as the second symbol in the array
                            string a = symbolstack.Pop();
                            symbolstack.Push(evaluateConjunction(truthtable, a, b, modelcount, propSymbols));
                            // Console.WriteLine(symbolstack.Peek());
                        }
                        else
                        {
                            symbolstack.Push(postfix[i]); // propositional symbols
                            // Console.WriteLine($"pushed {postfix[i]} onto the stack");
                        }
                    }

                    // is here where we lift the top number off the stack and assign it accordingly to it's proper sentence and proper model in the evaluations 2D array
                    // WILL HAVE TO CONVERT STRING TO INT
                    // it is for every implication sentence in the list of postfixed sentences, but outside of the model increment (so still part of a loop)

                }
                modelcount++;
        }

            return evaluations;
        }

        public string evaluateImplication(int[,] truthtable, string a, string b, int count, string[] propSymbolsList)
        {
            int indexA = 0;
            int indexB = 0;

            for(int i = 0; i < propSymbolsList.Length; i++) // gets the column value for the two symbols (used to look them up in the TT)
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

            // Console.WriteLine($"implication: {a} is at position {indexA}, {b} is at position {indexB}");


            return $"implication of {a} & {b}";
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

            // Console.WriteLine($"conjunction: {a} is at position {indexA}, {b} is at position {indexB}");

            return $"(conjunction of {a} & {b})";
        }

        public void generateBinaryStrings(int n, int[] bitarray, int arrpos, int rowcount) // credit for core backchannelling framework: https://www.geeksforgeeks.org/generate-all-the-binary-strings-of-n-bits/
        {
            if (arrpos == n)
            {
                addToTruthTable(n, bitarray, rowcount);
                return;
            }

            bitarray[arrpos] = 0;
            generateBinaryStrings(n, bitarray, arrpos + 1, rowcount);

            bitarray[arrpos] = 1;
            generateBinaryStrings(n, bitarray, arrpos + 1, rowcount);
        }

        public void addToTruthTable(int n, int[] bitarray, int rowcount)
        {
            for (int i = 0; i < n; i++)
            {
                _truthtable[i, rowcount] = bitarray[i];
                // Console.Write(_truthtable[i, rowcount] + " "); // the two Console.Write statements were used in development to print out the truth table for each symbol
            }
            // Console.WriteLine();

            rowcount++;

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

            printPostfixedSentences(postfixes); // function call to print to console for dev purposes

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

        // the following code printed out postfixed versions of each sentence for development purposes
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
