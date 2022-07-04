
namespace ArsoftTestServer.SQL
{

    class SeparatorLineReader 
        : ScriptReader
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


}
