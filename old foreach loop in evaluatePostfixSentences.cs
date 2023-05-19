/*foreach (string[] postfix in postfixSentences)
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
                            string b = symbolstack.Pop();
                            string a = symbolstack.Pop();
                            symbolstack.Push(evaluateConjunction(truthtable, a, b, modelcount, propSymbols));
                            // Console.WriteLine(symbolstack.Peek());
                        }
                        else
                        {
                            symbolstack.Push(postfix[i]); // propositional symbols
                            // Console.WriteLine($"pushed {postfix[i]} onto the stack");
                        }
                    }*/
