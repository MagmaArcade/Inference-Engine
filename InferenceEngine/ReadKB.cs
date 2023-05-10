using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    class ReadKB
    {
        private string _filename;
        private StreamReader _file;
        private List<string> _rawdata;
        private string[] _hornkb; // array of strings that contain horn clauses
        private string _query; // query string
        private string[] _propositionSymbol;


        // Read Only Properties 
        public string[] HornKB{
            get { return _hornkb; }}
        public string Query{
            get { return _query; }}


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
                if (_rawdata[i] == "TELL")
                {
                    _hornkb = _rawdata[i + 1].Split(';');

                    //gets all individual variables 
                    string[] delimiters = new string[] { "=>", "<=>", "&", "~", "||", ";", " "};
                    string[] _temp = _rawdata[i + 1].Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    //removes duplicate variables
                    _propositionSymbol = _temp.Distinct().ToArray();
                }
 
                if (_rawdata[i] == "ASK")
                {
                    _query = _rawdata[i + 1];
                }
            }

            _hornkb = _hornkb.Take(_hornkb.Length - 1).ToArray(); // remove the last (blank) element from the array
            for (int i = 0; i < _hornkb.Count(); i++) // 
            {
                _hornkb[i] = _hornkb[i].Trim(' ');
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
        }
    }
}
