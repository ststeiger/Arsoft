
namespace ArsoftTestServer
{
    

    public class Program
    {


        public static void TestScriptSplitting()
        {
            string sql = @"
 
SELECT 
     CAST(SERVERPROPERTY('ServerName') AS nvarchar(MAX)) + ' > ' + DB_NAME() AS db 
    ,SUSER_SNAME() + N' [' + @@LANGUAGE + N']' AS usr 
    ,SUBSTRING(@@VERSION, 1, CHARINDEX(' (', @@version)) AS SqlServer 
    ,SERVERPROPERTY('ProductVersion') AS ProductVersion 
    ,SERVERPROPERTY('ProductLevel') AS ProductLevel 
    ,SERVERPROPERTY('Edition') AS Edition 
    ,SERVERPROPERTY('BuildClrVersion') AS BuildClrVersion 
    ,CASE WHEN EXISTS( 
        SELECT * FROM sys.server_permissions AS what 
        INNER JOIN sys.server_principals AS who 
            ON who.principal_id = what.grantee_principal_id 
        WHERE(1 = 1) 
        AND what.permission_name = 'ALTER TRACE' 
        AND who.name NOT LIKE '##MS%##' 
        AND who.type_desc <> 'SERVER_ROLE' 
        -- AND who.name IN('CORCOR_DB', 'RZCOR_PowerUser') 
        AND who.name IN(SELECT name FROM sys.login_token WHERE principal_id > 0) 
    ) 
        OR EXISTS(SELECT name FROM sys.login_token WHERE principal_id > 0 AND name = 'sysadmin') 
        THEN 1 
        ELSE 0 
    END AS can_trace 

GO


SELECT 
     db.name 
    ,db.create_date 
    ,db.state_desc 
    ,db.user_access_desc 
    ,db.snapshot_isolation_state_desc 
    ,db.recovery_model_desc 
FROM sys.databases AS db 
WHERE db.owner_sid <> 0x01 
ORDER BY name 

GO


SELECT 
     ist.TABLE_SCHEMA AS table_schema 
    ,ist.TABLE_NAME AS table_name 
    ,ist.go 
FROM INFORMATION_SCHEMA.TABLES AS ist 
WHERE ist.TABLE_TYPE = 'BASE TABLE' 
ORDER BY table_schema, table_name 


";


            sql = "SELECT 1 AS a\r\nGO\r\nSELECT 2 AS b ";
            sql = "SELECT GO\n FROM dbo.mytest ";


            Data.ScriptSplitter allScripts = new Data.ScriptSplitter(sql);
            foreach (string s in allScripts)
            {
                System.Console.WriteLine(s);
            }
        }


        public static void Main(string[] args)
        {
            // TestScriptSplitting();
            // System.Console.WriteLine("argh");

            // https://tools.ietf.org/html/rfc3110

            // https://tools.ietf.org/html/rfc2539
            // https://tools.ietf.org/html/rfc3110
            // https://tools.ietf.org/html/rfc3757
            // https://www.rfc-editor.org/rfc/rfc4034.txt
            // https://tools.ietf.org/html/rfc4034#appendix-A.1


            // https://metebalci.com/blog/a-minimum-complete-tutorial-of-dnssec/
            // https://www.icann.org/news/blog/changing-the-keys-to-the-domain-name-system-dns-root-zone
            // https://blog.cloudflare.com/dnssec-an-introduction/
            // https://bytesarena.com/dns/dnssec/2019/02/19/dnssec-keys-and-signing-explained.html
            // https://www.nic.ch/faqs/dnssec/details/
            // https://tools.ietf.org/html/rfc2535




            string foo = @"AQPSKmynfzW4kyBv015MUG2DeIQ3
            Cbl+BBZH4b/0PY1kxkmvHjcZc8no
            kfzj31GajIQKY+5CptLr3buXA10h
                WqTkF7H6RfoRqXQeogmMHfpftf6z
            Mv1LyBUgia7za6ZEzOJBOztyvhjL
            742iU/TpPSEDhm2SNKLijfUppn1U
            aNvv4w== ";


            foo = System.Text.RegularExpressions.Regex.Replace(foo, @"\s+", "");
            byte[] b64 = System.Convert.FromBase64String(foo);

            foo = System.Text.Encoding.UTF8.GetString(b64);
            System.Console.WriteLine(foo);






            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = new System.Security.Cryptography.X509Certificates.X509Certificate2(byte[] rawData);
            // System.Security.Cryptography.X509Certificates.X509Certificate2 cert2 = DotNetUtilities.CreateX509Cert2("mycert");
            // SecurityKey secKey = new X509SecurityKey(cert2);


            ArsoftTestServer.SpecificForwaringServer.Test();


            // ArsoftTestServer.Resolvers.Test4();
            // ArsoftTestServer.SimpleServer.Test();

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(" --- Press any key to continue --- ");
            System.Console.ReadKey();
        }


    }


}
