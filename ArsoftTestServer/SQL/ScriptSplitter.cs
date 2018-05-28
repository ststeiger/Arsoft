
#region Disclaimer/Info

///////////////////////////////////////////////////////////////////////////////////////////////////
// Subtext WebLog
// 
// Subtext is an open source weblog system that is a fork of the .TEXT
// weblog system.
//
// For updated news and information please visit http://subtextproject.com/
// Subtext is hosted at Google Code at http://code.google.com/p/subtext/
// The development mailing list is at subtext@googlegroups.com 
//
// This project is licensed under the BSD license.  See the License.txt file for more information.
///////////////////////////////////////////////////////////////////////////////////////////////////

#endregion


// Subtext.Scripting from Subtext\Subtext.Scripting\ScriptSplitter.cs
namespace ArsoftTestServer.Data
{

    public class SqlParseException : System.Exception
    {
        
        public SqlParseException()
        { }

        public SqlParseException(string message) : base(message)
        { }

        public SqlParseException(string message, System.Exception exception)
            : base(message, exception)
        { }

        // protected SqlParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        // {}

        // [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        // public override void GetObjectData(SerializationInfo info, StreamingContext context)
        // {base.GetObjectData(info, context); }
    }



    public class ScriptSplitter : System.Collections.Generic.IEnumerable<string>
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

    abstract class ScriptReader
    {
        protected readonly ScriptSplitter Splitter;

        protected ScriptReader(ScriptSplitter splitter)
        {
            Splitter = splitter;
        }

        /// <summary>
        /// This acts as a template method. Specific Reader instances 
        /// override the component methods.
        /// </summary>
        public bool ReadNextSection()
        {
            if (IsQuote)
            {
                ReadQuotedString();
                return false;
            }

            if (BeginDashDashComment)
            {
                return ReadDashDashComment();
            }

            if (BeginSlashStarComment)
            {
                ReadSlashStarComment();
                return false;
            }

            return ReadNext();
        }

        protected virtual bool ReadDashDashComment()
        {
            Splitter.Append(Current);
            while (Splitter.Next())
            {
                Splitter.Append(Current);
                if (EndOfLine)
                {
                    break;
                }
            }
            //We should be EndOfLine or EndOfScript here.
            Splitter.SetParser(new SeparatorLineReader(Splitter));
            return false;
        }

        protected virtual void ReadSlashStarComment()
        {
            if (ReadSlashStarCommentWithResult())
            {
                Splitter.SetParser(new SeparatorLineReader(Splitter));
                return;
            }
        }

        private bool ReadSlashStarCommentWithResult()
        {
            Splitter.Append(Current);
            while (Splitter.Next())
            {
                if (BeginSlashStarComment)
                {
                    ReadSlashStarCommentWithResult();
                    continue;
                }
                Splitter.Append(Current);

                if (EndSlashStarComment)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void ReadQuotedString()
        {
            Splitter.Append(Current);
            while (Splitter.Next())
            {
                Splitter.Append(Current);
                if (IsQuote)
                {
                    return;
                }
            }
        }

        protected abstract bool ReadNext();

        #region Helper methods and properties

        protected bool HasNext
        {
            get { return Splitter.HasNext; }
        }

        protected bool WhiteSpace
        {
            get { return char.IsWhiteSpace(Splitter.Current); }
        }

        protected bool EndOfLine
        {
            get { return '\n' == Splitter.Current; }
        }

        protected bool IsQuote
        {
            get { return '\'' == Splitter.Current; }
        }

        protected char Current
        {
            get { return Splitter.Current; }
        }

        protected char LastChar
        {
            get { return Splitter.LastChar; }
        }

        bool BeginDashDashComment
        {
            get { return Current == '-' && Peek() == '-'; }
        }

        bool BeginSlashStarComment
        {
            get { return Current == '/' && Peek() == '*'; }
        }

        bool EndSlashStarComment
        {
            get { return LastChar == '*' && Current == '/'; }
        }

        protected static bool CharEquals(char expected, char actual)
        {
            return System.Char.ToLowerInvariant(expected) == System.Char.ToLowerInvariant(actual);
        }

        protected bool CharEquals(char compare)
        {
            return CharEquals(Current, compare);
        }

        protected char Peek()
        {
            if (!HasNext)
            {
                return char.MinValue;
            }
            return (char)Splitter.Peek();
        }

        #endregion
    }

    class SeparatorLineReader : ScriptReader
    {
        private System.Text.StringBuilder _builder = new System.Text.StringBuilder();
        private bool _foundGo;
        private bool _gFound;

        public SeparatorLineReader(ScriptSplitter splitter)
            : base(splitter)
        {
        }

        void Reset()
        {
            _foundGo = false;
            _gFound = false;
            _builder = new System.Text.StringBuilder();
        }

        protected override bool ReadDashDashComment()
        {
            if (!_foundGo)
            {
                base.ReadDashDashComment();
                return false;
            }
            base.ReadDashDashComment();
            return true;
        }

        protected override void ReadSlashStarComment()
        {
            if (_foundGo)
            {
                throw new SqlParseException("Incorrect Syntax near GO");
            }
            base.ReadSlashStarComment();
        }

        protected override bool ReadNext()
        {
            if (EndOfLine) //End of line or script
            {
                if (!_foundGo)
                {
                    _builder.Append(Current);
                    Splitter.Append(_builder.ToString());
                    Splitter.SetParser(new SeparatorLineReader(Splitter));
                    return false;
                }
                Reset();
                return true;
            }

            if (WhiteSpace)
            {
                _builder.Append(Current);
                return false;
            }

            if (!CharEquals('g') && !CharEquals('o'))
            {
                FoundNonEmptyCharacter(Current);
                return false;
            }

            if (CharEquals('o'))
            {
                if (CharEquals('g', LastChar) && !_foundGo)
                {
                    _foundGo = true;
                }
                else
                {
                    FoundNonEmptyCharacter(Current);
                }
            }

            if (CharEquals('g', Current))
            {
                if (_gFound || (!System.Char.IsWhiteSpace(LastChar) && LastChar != char.MinValue))
                {
                    FoundNonEmptyCharacter(Current);
                    return false;
                }

                _gFound = true;
            }

            if (!HasNext && _foundGo)
            {
                Reset();
                return true;
            }

            _builder.Append(Current);
            return false;
        }

        void FoundNonEmptyCharacter(char c)
        {
            _builder.Append(c);
            Splitter.Append(_builder.ToString());
            Splitter.SetParser(new SqlScriptReader(Splitter));
        }
    }

    class SqlScriptReader : ScriptReader
    {
        public SqlScriptReader(ScriptSplitter splitter)
            : base(splitter)
        {
        }

        protected override bool ReadNext()
        {
            if (EndOfLine) //end of line
            {
                Splitter.Append(Current);
                Splitter.SetParser(new SeparatorLineReader(Splitter));
                return false;
            }

            Splitter.Append(Current);
            return false;
        }
    }
}