using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions; // needed for RegEx to remove whitespaces :)

namespace InferenceEngine
{
    class ReadKB
    {
        private string _filename;
        private StreamReader _file;
        private List<string> _rawdata;
        private string[] _hornkb; // array of strings that contain horn clauses
        private string _query; // query string (goal state)
        private string[] _propositionSymbol;


        // Read Only Properties 
        public string[] HornKB{
            get { return _hornkb; }}
        public string Query{
            get { return _query; }}
        public string[] PropositionSymbol{
            get { return _propositionSymbol; }}


        // read in the raw data
        public ReadKB (string testfile)
        {
            _filename = testfile;
            _rawdata = new List<string> { };
            _file = new StreamReader(_filename);
            ReadEnvironData();
        }
        public void ReadEnvironData()
        {
            string currentLine = _file.ReadLine();
            while (currentLine != null)
            {
                _rawdata.Add(currentLine);
                currentLine = _file.ReadLine();
            }
        }


        //parse the raw data
        public void ParseHornKB()
        {
            for (int i = 0; i < _rawdata.Count; i++)
            {
                if (_rawdata[i].Trim() == "TELL")
                {
                    _hornkb = _rawdata[i + 1].Split(';');

                    //gets all individual variables 
                    string[] delimiters = new string[] { "=>", "<=>", "&", "~", "||", ";", " "};
                    string[] _temp = _rawdata[i + 1].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    //removes duplicate variables
                    _propositionSymbol = _temp.Distinct().ToArray();
                }
 
                if (_rawdata[i].Trim() == "ASK")
                {
                    _query = _rawdata[i + 1];
                }

              /*  if (_rawdata[i].Trim() != "TELL" && _rawdata[i+2].Trim() != "ASK") // if Tell of ask is not in the file print error end loop
                {
                    Console.WriteLine("Issue With Reading File, Please Ensure TELL and ASK are correctly Implemented");

                } */
            }

            _hornkb = _hornkb.Take(_hornkb.Length - 1).ToArray(); // remove the last (blank) element from the array
            
            for (int i = 0; i < _hornkb.Count(); i++) // remove whitespace from every query (ease of use later)
            {
                string initial = _hornkb[i];
                initial = Regex.Replace(initial, @"\s", "");
                _hornkb[i] = initial;
            }

        }


        // for development testing purposes, not actually called during final program execution
        public void PrintEnvironData() 
        {
            Console.WriteLine("The Raw Input File:");
            _rawdata.ForEach(Console.WriteLine);
            Console.WriteLine();
            Console.WriteLine("The Horn Clauses:");
            for (int i = 0; i < _hornkb.Count(); i++)
            {
                Console.WriteLine(_hornkb[i]);
            }
            Console.WriteLine();
            Console.WriteLine("The Proposition Symbol:");
            for (int i = 0; i < _propositionSymbol.Count(); i++)
            {
                Console.WriteLine(_propositionSymbol[i]);
            }
            Console.WriteLine("---------------------------------");
        }
    }
}
