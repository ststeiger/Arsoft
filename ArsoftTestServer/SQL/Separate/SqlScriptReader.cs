
// Subtext.Scripting from Subtext\Subtext.Scripting\ScriptSplitter.cs
namespace ArsoftTestServer.SQL
{


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