using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;

namespace svh_host
{
    class Typer
    {
        string text;
        int start_position;
        int words_count;

        string current_word;
        int current_word_int;
        string current_sym;

        int _current_sym_int = 0;
        public int current_sym_int
        {
            get {
                return _current_sym_int;
            }
            set {
                _current_sym_int = value;
                if (CurrIndexRaise != null)
                {
                    CurrIndexRaise(this, null);
                }
            }
        }

        public event EventHandler CurrIndexRaise;

        int maxSymPause = 400;
        int minSymPause=50;

        int maxWordPause = 2000;
        int minWordPause = 500;

        ManualResetEvent are = new ManualResetEvent(true);

        private Typer()
        {
        }

        public static Typer NewTyper(string text)
        {
            Typer typer = new Typer();
            typer.text = text;
            return typer;
        }

        public void Start()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                current_word = "";
                current_word_int = 0;
                current_sym = "";
                current_sym_int = 0;

                Type(current_sym_int);

            });

        }

        public void Type(int position = 0)
        {
            string textt="";
            if (position > 0)
            {
                textt = text.Substring(position);
            }
            else
            {
                textt = text;
            }
            var lines = SplitByLines(textt);
            foreach (var line in lines)
            {
                TypeLine(line);
                TypeReturn();
            }
        }

        private void TypeLine(string line)
        {
            List<string> words = SplitByWords(line);
            foreach (var word in words)
            {
                current_word = word;
                current_word_int++;

                TypeWord(word);
                WordPause();
            }
        }

        private void TypeWord(string word)
        {
            List<string> simbs = SplitBySymbols(word);
            foreach (var sim in simbs)
            {
                TypeSym(sim);
                SymPause();
            }
        }

        private List<string> SplitByLines(string text)
        {
            Regex reg = new Regex(@"(?-s).+");
            return SplitByReg(text,reg);
        }

        private List<string> SplitByReg(string st,Regex reg)
        {
            
            List<string> lines=new List<string>();
            foreach (Match m in reg.Matches(st))
            {
                lines.Add(m.Value);
            }

            return lines;
        }

        private List<string> SplitByWords(string line)
        {
            Regex reg = new Regex(@".+?\s");
            return SplitByReg(line, reg);
        }

        private List<string> SplitBySymbols(string word)
        {
            Regex reg = new Regex(@".{1}");
            return SplitByReg(word, reg);
        }

        private void WordPause()
        {
            IsPause();
            int pause = rnd.Next(maxWordPause - minWordPause);
            Thread.Sleep(minWordPause+pause);
        }

        Random rnd = new Random();
        private void SymPause()
        {
            IsPause();
            int pause=rnd.Next(maxSymPause - minSymPause);
            Thread.Sleep(minSymPause+pause);
        }

        private void IsPause()
        {
            are.WaitOne();
        }

        public void Pause()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                ((Typer)o).are.Reset();
            }, this);
            
        }

        public void Resume()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                ((Typer)o).are.Set();
            }, this);
        }

        private void TypeSym(string sim)
        {
            current_sym_int++;
            current_sym = sim;
            InputSimulator.SimulateTextEntry(sim);
        }
        private void TypeReturn()
        {
            InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
        }
    }
}
