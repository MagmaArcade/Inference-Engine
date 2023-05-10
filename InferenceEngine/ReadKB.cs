using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InferenceEngine
{
    internal class ReadKB
    {
        private string _filename;
        private StreamReader _file;
        private List<string> _rawdata;

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

        public List<string> ParseHornKB()
        {
            for(int i = 0; i < _rawdata.Count; i++)
            {
                if (_rawdata[i] == "TELL")
                {
                    List<string> hornkb = 
                }
            }
        }


        public void PrintEnvironData() /* for development testing purposes, not actually called during final program execution */
        {
            _rawdata.ForEach(Console.WriteLine);
        }
    }
}
