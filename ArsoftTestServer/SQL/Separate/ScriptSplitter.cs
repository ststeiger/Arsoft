
namespace ArsoftTestServer.SQL
{


    public class ScriptSplitter 
        : System.Collections.Generic.IEnumerable<string>
    {
        private readonly System.IO.TextReader _reader;
        private System.Text.StringBuilder _builder = new System.Text.StringBuilder();
        private char _current;
        private char _lastChar;
        private ScriptReader _scriptReader;

        public ScriptSplitter(string script)
        {
            _reader = new System.IO.StringReader(script);
            _scriptReader = new SeparatorLineReader(this);
        }


        public static System.Collections.Generic.List<string> SplitScript(string strSQL)
        {
            System.Collections.Generic.List<string> ls =
                new System.Collections.Generic.List<string>();

            System.Collections.Generic.IEnumerable<string> scp = new ScriptSplitter(strSQL);
            ls.AddRange(scp);

            return ls;
        }


        // http://stackoverflow.com/questions/40814/how-do-i-execute-a-large-sql-script-with-go-commands-from-c
        // WARNING: Ignores GOs that are not on a separate line
        // Arbitrary numbers of whitespaces/tabs are no problem
        // Warning: splits GO in multline SQL strings
        protected static System.Collections.Generic.List<string> FastAndTooSimpleSplit(string sql)
        {
            System.Collections.Generic.List<string> lsBatchJobs = new System.Collections.Generic.List<string>();
            System.Text.StringBuilder sbCurrentBatch = new System.Text.StringBuilder();
            string strNewLineCharacter = "\n";

            char[] chrWhiteSpaces = new char[] { ' ', '\t' };
            try
            {
                string[] astrLines = sql.Split(new string[2] { "\n", "\r" }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (string strThisLine in astrLines)
                {
                    // Note: This could be inside a multiline SQL string, or a comment... ==> BUG
                    // Therefore use the ScriptSplitter class
                    if (strThisLine.ToUpperInvariant().Trim(chrWhiteSpaces) == "GO")
                    {
                        lsBatchJobs.Add(sbCurrentBatch.ToString());
                        sbCurrentBatch = null;
                        sbCurrentBatch = new System.Text.StringBuilder();
                    }
                    else
                    {
                        sbCurrentBatch.Append(strThisLine + strNewLineCharacter);
                    }
                } // Next strThisLine

                if (sbCurrentBatch.Length != 0)
                {
                    lsBatchJobs.Add(sbCurrentBatch.ToString());
                    sbCurrentBatch = null;
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
            }

            sbCurrentBatch = null;

            return lsBatchJobs;
        } // End Function GetBatches


        internal bool HasNext
        {
            get { return _reader.Peek() != -1; }
        }

        internal char Current
        {
            get { return _current; }
        }

        internal char LastChar
        {
            get { return _lastChar; }
        }

        #region IEnumerable<string> Members

        public System.Collections.Generic.IEnumerator<string> GetEnumerator()
        {

            while (Next())
            {
                if (Split())
                {
                    string script = _builder.ToString().Trim();
                    if (script.Length > 0)
                    {
                        yield return (script);
                    }
                    Reset();
                }
            }
            if (_builder.Length > 0)
            {
                string scriptRemains = _builder.ToString().Trim();
                if (scriptRemains.Length > 0)
                {
                    yield return (scriptRemains);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        internal bool Next()
        {
            if (!HasNext)
            {
                return false;
            }

            _lastChar = _current;
            _current = (char)_reader.Read();
            return true;
        }

        internal int Peek()
        {
            return _reader.Peek();
        }

        private bool Split()
        {
            return _scriptReader.ReadNextSection();
        }

        internal void SetParser(ScriptReader newReader)
        {
            _scriptReader = newReader;
        }

        internal void Append(string text)
        {
            _builder.Append(text);
        }

        internal void Append(char c)
        {
            _builder.Append(c);
        }

        void Reset()
        {
            _current = _lastChar = char.MinValue;
            _builder = new System.Text.StringBuilder();
        }
    }


}
