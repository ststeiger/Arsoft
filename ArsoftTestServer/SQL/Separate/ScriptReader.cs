
namespace ArsoftTestServer.SQL
{

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


}
