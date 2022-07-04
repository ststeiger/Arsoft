
namespace ArsoftTestServer.SQL
{


    public class SqlParseException 
        : System.Exception
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

}