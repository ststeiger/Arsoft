
namespace ArsoftTestServer.Data 
{
    public delegate void DataReaderCallback_t(System.Data.Common.DbDataReader reader);



    /*
    public class DestinationSql : SQL
    {
        public override string GetConnectionString()
        {
            if (this.m_connectionString != null)
                return this.m_connectionString;

            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            // csb.DataSource = System.Environment.MachineName;
            csb.DataSource = "CORDB2012";
            csb.InitialCatalog = "COR_Basic_BKB";

            csb.IntegratedSecurity = true;

            if (!csb.IntegratedSecurity)
            {
                csb.UserID = "";
                csb.Password = "";
            } // End if (!csb.IntegratedSecurity) 

            csb.ConnectTimeout = 30;
            csb.MultipleActiveResultSets = false;
            csb.Pooling = true;
            csb.PersistSecurityInfo = false;
            csb.PacketSize = 4096;

            return csb.ConnectionString;
        }
    }


    public class SourceSql : SQL
    {
        public override string GetConnectionString()
        {
            if (this.m_connectionString != null)
                return this.m_connectionString;

            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            // csb.DataSource = System.Environment.MachineName;
            csb.DataSource = "CORDB2016";
            csb.InitialCatalog = "COR_Basic_BKB";

            csb.IntegratedSecurity = true;

            if (!csb.IntegratedSecurity)
            {
                csb.UserID = "";
                csb.Password = "";
            } // End if (!csb.IntegratedSecurity) 

            csb.ConnectTimeout = 30;
            csb.MultipleActiveResultSets = false;
            csb.Pooling = true;
            csb.PersistSecurityInfo = false;
            csb.PacketSize = 4096;

            return csb.ConnectionString;
        }
    }
    */

    public class MS_SQL
        : AnySQL
    {

        public MS_SQL()
            :base()
        { }


        public override string GetConnectionString()
        {
            if (this.m_connectionString != null)
                return this.m_connectionString;

            System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();
            csb.DataSource = System.Environment.MachineName + @"\SQLEXPRESS";
            csb.InitialCatalog = "DnsServer";
            
            csb.IntegratedSecurity = true;

            if (!csb.IntegratedSecurity)
            {
                csb.UserID = "";
                csb.Password = "";
            } // End if (!csb.IntegratedSecurity) 

            csb.ConnectTimeout = 5;
            csb.MultipleActiveResultSets = true;
            csb.Pooling = true;
            csb.PersistSecurityInfo = false;
            csb.PacketSize = 4096;

            this.m_connectionString = csb.ConnectionString;

            return this.m_connectionString;
        }


        public override System.Data.Common.DbProviderFactory GetFactory()
        {
            return System.Data.SqlClient.SqlClientFactory.Instance;
        }

    }


    public abstract class AnySQL
    {
        // http://www.sqlusa.com/bestpractices/datetimeconversion/
        const string DATEFORMAT = "{0:yyyyMMdd}"; // YYYYMMDD ISO date format works at any language setting - international standard
        const string DATETIMEFORMAT = "{0:yyyy'-'MM'-'dd'T'HH:mm:ss.fff}"; // ISO 8601 format: international standard - works with any language setting


        protected string m_connectionString;
        protected System.Data.Common.DbProviderFactory m_factory;

        public abstract string GetConnectionString();
        public abstract System.Data.Common.DbProviderFactory GetFactory();


        public System.Data.Common.DbConnection Connection
        {
            get
            {
                return GetConnection();
            }
        }


        public AnySQL()
        {
            this.m_factory = this.GetFactory();
        }


        public static AnySQL CreateInstance()
        {
            return new MS_SQL();
        }


        public virtual System.Data.Common.DbConnection GetConnection()
        {
            System.Data.Common.DbConnection conn = this.m_factory.CreateConnection();
            conn.ConnectionString = this.GetConnectionString();

            return conn;
        }
        

        public System.Data.Common.DbCommand CreateCommand(string sql)
        {
            System.Data.Common.DbCommand cmd = this.m_factory.CreateCommand();
            cmd.CommandText = sql;

            return cmd;
        }


        public System.Data.Common.DbCommand CreateCommand()
        {
            return CreateCommand(null);
        }



        protected virtual System.Data.DbType GetDbType(System.Type type)
        {
            // http://social.msdn.microsoft.com/Forums/en/winforms/thread/c6f3ab91-2198-402a-9a18-66ce442333a6
            string strTypeName = type.Name;
            System.Data.DbType dbType = System.Data.DbType.String; // default value

            try
            {
                if (object.ReferenceEquals(type, typeof(System.DBNull)))
                {
                    return dbType;
                }

                if (object.ReferenceEquals(type, typeof(System.Byte[])))
                {
                    return System.Data.DbType.Binary;
                }

                dbType = (System.Data.DbType)System.Enum.Parse(typeof(System.Data.DbType), strTypeName, true);

                // Es ist keine Zuordnung von DbType UInt64 zu einem bekannten SqlDbType vorhanden.
                // http://msdn.microsoft.com/en-us/library/bbw6zyha(v=vs.71).aspx
                if (dbType == System.Data.DbType.UInt64)
                    dbType = System.Data.DbType.Int64;
            }
            catch (System.Exception)
            {
                // add error handling to suit your taste
            }

            return dbType;
        } // End Function GetDbType



        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        protected virtual string SqlTypeFromDbType(System.Data.DbType type)
        {
            string strRetVal = null;

            // http://msdn.microsoft.com/en-us/library/cc716729.aspx
            switch (type)
            {
                case System.Data.DbType.Guid:
                    strRetVal = "uniqueidentifier";
                    break;
                case System.Data.DbType.Date:
                    strRetVal = "date";
                    break;
                case System.Data.DbType.Time:
                    strRetVal = "time(7)";
                    break;
                case System.Data.DbType.DateTime:
                    strRetVal = "datetime";
                    break;
                case System.Data.DbType.DateTime2:
                    strRetVal = "datetime2";
                    break;
                case System.Data.DbType.DateTimeOffset:
                    strRetVal = "datetimeoffset(7)";
                    break;

                case System.Data.DbType.StringFixedLength:
                    strRetVal = "nchar(MAX)";
                    break;
                case System.Data.DbType.String:
                    strRetVal = "nvarchar(MAX)";
                    break;

                case System.Data.DbType.AnsiStringFixedLength:
                    strRetVal = "char(MAX)";
                    break;
                case System.Data.DbType.AnsiString:
                    strRetVal = "varchar(MAX)";
                    break;

                case System.Data.DbType.Single:
                    strRetVal = "real";
                    break;
                case System.Data.DbType.Double:
                    strRetVal = "float";
                    break;
                case System.Data.DbType.Decimal:
                    strRetVal = "decimal(19, 5)";
                    break;
                case System.Data.DbType.VarNumeric:
                    strRetVal = "numeric(19, 5)";
                    break;

                case System.Data.DbType.Boolean:
                    strRetVal = "bit";
                    break;
                case System.Data.DbType.SByte:
                case System.Data.DbType.Byte:
                    strRetVal = "tinyint";
                    break;
                case System.Data.DbType.Int16:
                    strRetVal = "smallint";
                    break;
                case System.Data.DbType.Int32:
                    strRetVal = "int";
                    break;
                case System.Data.DbType.Int64:
                    strRetVal = "bigint";
                    break;
                case System.Data.DbType.Xml:
                    strRetVal = "xml";
                    break;
                case System.Data.DbType.Binary:
                    strRetVal = "varbinary(MAX)"; // or image
                    break;
                case System.Data.DbType.Currency:
                    strRetVal = "money";
                    break;
                case System.Data.DbType.Object:
                    strRetVal = "sql_variant";
                    break;

                case System.Data.DbType.UInt16:
                case System.Data.DbType.UInt32:
                case System.Data.DbType.UInt64:
                    throw new System.NotImplementedException("Uints not mapped - MySQL only");
            }

            return strRetVal;
        } // End Function SqlTypeFromDbType


        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void SetParameter(object param, object objValue)
        {
            SetParameter((System.Data.IDbDataParameter)param, objValue);
        }


        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void SetParameter(System.Data.IDbDataParameter param, object objValue)
        {
            if (objValue == null)
                param.Value = System.DBNull.Value;
            else
                param.Value = objValue;
        }


        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual T GetParameterValue<T>(System.Data.IDbCommand idbc, string strParameterName)
        {
            if (!strParameterName.StartsWith("@"))
            {
                strParameterName = "@" + strParameterName;
            }

            return (T)(((System.Data.IDbDataParameter)idbc.Parameters[strParameterName]).Value);
        } // End Function GetParameterValue<T>




        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual string GetParametrizedQueryText(System.Data.IDbCommand cmd)
        {
            string strReturnValue = "";

            try
            {
                System.Text.StringBuilder msg = new System.Text.StringBuilder();
                System.DateTime dtLogTime = System.DateTime.UtcNow;


                if (cmd == null || string.IsNullOrEmpty(cmd.CommandText))
                {
                    return strReturnValue;
                } // End if (cmd == null || string.IsNullOrEmpty(cmd.CommandText))


                if (cmd.Parameters != null && cmd.Parameters.Count > 0)
                {
                    msg.AppendLine("-- ***** Listing Parameters *****");

                    foreach (System.Data.IDataParameter idpThisParameter in cmd.Parameters)
                    {
                        // http://msdn.microsoft.com/en-us/library/cc716729.aspx
                        msg.AppendLine(string.Format("DECLARE {0} AS {1} -- DbType: {2}", idpThisParameter.ParameterName, SqlTypeFromDbType(idpThisParameter.DbType), idpThisParameter.DbType.ToString()));
                    } // Next idpThisParameter

                    msg.AppendLine(System.Environment.NewLine);
                    msg.AppendLine(System.Environment.NewLine);

                    foreach (System.Data.IDataParameter idpThisParameter in cmd.Parameters)
                    {
                        string strParameterValue = null;
                        if (object.ReferenceEquals(idpThisParameter.Value, System.DBNull.Value))
                        {
                            strParameterValue = "NULL";
                        }
                        else
                        {
                            if (idpThisParameter.DbType == System.Data.DbType.Date)
                                strParameterValue = System.String.Format(DATEFORMAT, idpThisParameter.Value);
                            else if (idpThisParameter.DbType == System.Data.DbType.DateTime || idpThisParameter.DbType == System.Data.DbType.DateTime2)
                                strParameterValue = System.String.Format(DATETIMEFORMAT, idpThisParameter.Value);
                            else
                                strParameterValue = idpThisParameter.Value.ToString();

                            strParameterValue = "'" + strParameterValue.Replace("'", "''") + "'";
                        }

                        msg.AppendLine(string.Format("SET {0} = {1}", idpThisParameter.ParameterName, strParameterValue));
                    } // Next idpThisParameter

                    msg.AppendLine("-- ***** End Parameter Listing *****");
                    msg.AppendLine(System.Environment.NewLine);
                } // End if cmd.Parameters != null || cmd.Parameters.Count > 0 



                msg.AppendLine(string.Format("-- ***** Start Query from {0} *****", dtLogTime.ToString(DATEFORMAT)));
                msg.AppendLine(cmd.CommandText);
                msg.AppendLine(string.Format("-- ***** End Query from {0} *****", dtLogTime.ToString(DATEFORMAT)));
                msg.AppendLine(System.Environment.NewLine);

                strReturnValue = msg.ToString();
                msg = null;
            } // End Try
            catch (System.Exception ex)
            {
                strReturnValue = "Error in Function cDAL.GetParametrizedQueryText";
                strReturnValue += System.Environment.NewLine;
                strReturnValue += ex.Message;
            } // End Catch

            return strReturnValue;
        } // End Function GetParametrizedQueryText


        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string JoinArray<T>(string separator, T[] inputTypeArray)
        {
            return JoinArray<T>(separator, inputTypeArray, object.ReferenceEquals(typeof(T), typeof(string)));
        }



        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string JoinArray<T>(string separator, T[] inputTypeArray, bool sqlEscapeString)
        {
            string strRetValue = null;
            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();

            for (int i = 0; i < inputTypeArray.Length; ++i)
            {
                string str = System.Convert.ToString(inputTypeArray[i], System.Globalization.CultureInfo.InvariantCulture);

                if (!string.IsNullOrEmpty(str))
                {
                    // SQL-Escape
                    if (sqlEscapeString)
                        str = str.Replace("'", "''");

                    ls.Add(str);
                } // End if (!string.IsNullOrEmpty(str))

            } // Next i 

            strRetValue = string.Join(separator, ls.ToArray());
            ls.Clear();
            ls = null;

            return strRetValue;
        } // End Function JoinArray


        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual void AddArrayParameter<T>(System.Data.IDbCommand command, string strParameterName, params T[] values)
        {
            if (values == null)
                return;

            if (!strParameterName.StartsWith("@"))
                strParameterName = "@" + strParameterName;

            string strSqlInStatement = JoinArray<T>(",", values);

            command.CommandText = command.CommandText.Replace(strParameterName, strSqlInStatement);
        } // End Function AddArrayParameter



        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual System.Data.IDbDataParameter AddParameter(System.Data.IDbCommand command, string strParameterName, object objValue)
        {
            return AddParameter(command, strParameterName, objValue, System.Data.ParameterDirection.Input);
        } // End Function AddParameter


        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual System.Data.IDbDataParameter AddParameter(System.Data.IDbCommand command, string strParameterName, object objValue, System.Data.ParameterDirection pad)
        {
            if (objValue == null)
            {
                //throw new ArgumentNullException("objValue");
                objValue = System.DBNull.Value;
            } // End if (objValue == null)

            System.Type tDataType = objValue.GetType();
            System.Data.DbType dbType = GetDbType(tDataType);

            return AddParameter(command, strParameterName, objValue, pad, dbType);
        } // End Function AddParameter


        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public virtual System.Data.IDbDataParameter AddParameter(System.Data.IDbCommand command, string strParameterName, object objValue, System.Data.ParameterDirection pad, System.Data.DbType dbType)
        {
            System.Data.IDbDataParameter parameter = command.CreateParameter();

            if (!strParameterName.StartsWith("@"))
            {
                strParameterName = "@" + strParameterName;
            } // End if (!strParameterName.StartsWith("@"))

            parameter.ParameterName = strParameterName;
            parameter.DbType = dbType;
            parameter.Direction = pad;

            // Es ist keine Zuordnung von DbType UInt64 zu einem bekannten SqlDbType vorhanden.
            // No association  DbType UInt64 to a known SqlDbType
            SetParameter(parameter, objValue);

            command.Parameters.Add(parameter);
            return parameter;
        } // End Function AddParameter
        

        public string QuoteName(string name)
        {
            return "\"" + name.Replace("\"", "\"\"") + "\"";
            // return "[" + name.Replace("]", "]]") + "]";
        }


        public string EscapeName(string name)
        {
            return name.Replace("'", "''");
            // return "[" + name.Replace("]", "]]") + "]";
        }



#if NETCORE_2_0


        public virtual System.Data.DataTable GetDataTable(System.Data.IDbCommand cmd)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            using (System.Data.IDbConnection idbc = this.Connection)
            {

                try
                {
                    cmd.Connection = idbc;

                    using (System.Data.Common.DbDataAdapter daQueryTable = this.m_factory.CreateDataAdapter())
                    {
                        daQueryTable.SelectCommand = (System.Data.Common.DbCommand)cmd;
                        daQueryTable.Fill(dt);
                    } // End Using daQueryTable

                } // End Try
                catch (System.Data.Common.DbException ex)
                {
                    // if (Log("cDAK.GetDataTable(System.Data.IDbCommand cmd)", ex, cmd.CommandText))
                    throw;
                }// End Catch
                finally
                {
                    if (idbc.State != System.Data.ConnectionState.Closed)
                        idbc.Close();
                } // End Finally

            } // End Using idbc

            return dt;
        } // End Function GetDataTable


        public virtual System.Data.DataTable GetDataTable(string strSQL)
        {
            System.Data.DataTable dt = null;

            using (System.Data.IDbCommand cmd = this.CreateCommand(strSQL))
            {
                dt = GetDataTable(cmd);
            } // End Using cmd

            return dt;
        } // End Function GetDataTable


        public virtual System.Data.DataSet GetDataSet(System.Data.IDbCommand cmd)
        {
            string datasetName = "NewDataSet";
            System.Data.DataSet ds = new System.Data.DataSet(datasetName);

            using (System.Data.IDbConnection idbc = this.Connection)
            {

                try
                {
                    cmd.Connection = idbc;

                    using (System.Data.Common.DbDataAdapter daQueryTable = this.m_factory.CreateDataAdapter())
                    {
                        daQueryTable.SelectCommand = (System.Data.Common.DbCommand)cmd;
                        daQueryTable.Fill(ds);
                    } // End Using daQueryTable

                } // End Try
                catch (System.Data.Common.DbException ex)
                {
                    //if (Log("cDAL.GetDataTable(System.Data.IDbCommand cmd)", ex, cmd.CommandText))
                    throw;
                }// End Catch
                finally
                {
                    if (idbc.State != System.Data.ConnectionState.Closed)
                        idbc.Close();
                } // End Finally

            } // End Using idbc

            return ds;
        } // End Function GetDataSet


        public virtual System.Data.DataSet GetDataSet(string strSQL)
        {
            System.Data.DataSet ds = null;

            using (System.Data.IDbCommand cmd = CreateCommand(strSQL))
            {
                ds = GetDataSet(cmd);
            } // End Using cmd 

            return ds;
        } // End Function GetDataSet




        public System.Data.Common.DbCommand CreateCommandFromFile(string fileName)
        {
            string dir = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
            dir = System.IO.Path.Combine(dir, "../../SQL");
            dir = System.IO.Path.Combine(dir, fileName);
            dir = System.IO.Path.GetFullPath(dir);

            string sql = System.IO.File.ReadAllText(dir, System.Text.Encoding.UTF8);
            return CreateCommand(sql);
        }
#endif

        public int ExecuteNonQuery(System.Data.IDbCommand cmd)
        {
            int iResult = -1;

            using (System.Data.Common.DbConnection con = this.Connection)
            {
                lock (con)
                {
                    cmd.Connection = con;

                    if (cmd.Connection.State != System.Data.ConnectionState.Open)
                        cmd.Connection.Open();

                    iResult = cmd.ExecuteNonQuery();


                    if (cmd.Connection.State != System.Data.ConnectionState.Closed)
                        cmd.Connection.Close();
                } // End lock (con) 

            } // End Using con 

            return iResult;
        }


        public int ExecuteNonQuery(string strSQL)
        {
            int iResult = -1;

            using (System.Data.IDbCommand cmd = CreateCommand(strSQL))
            {
                iResult = ExecuteNonQuery(cmd);
            } // End Using cmd

            return iResult;
        } // End Function ExecuteScalar(strSQL)


        public virtual void ExecuteReader(System.Data.IDbCommand cmd, DataReaderCallback_t callback)
        {
            using (System.Data.Common.DbConnection con = this.Connection)
            {
                cmd.Connection = con;

                if (con.State != System.Data.ConnectionState.Open)
                    con.Open();

                using (System.Data.Common.DbDataReader idr = (System.Data.Common.DbDataReader)cmd.ExecuteReader(
                        System.Data.CommandBehavior.SequentialAccess | System.Data.CommandBehavior.CloseConnection
                ))
                {
                    callback(idr);
                }

                if (con.State != System.Data.ConnectionState.Closed)
                    con.Close();
            } // End Using con 
        }


        public virtual void ExecuteReader(string sql, DataReaderCallback_t callback)
        {
            using (System.Data.Common.DbCommand cmd = CreateCommand(sql))
            {
                ExecuteReader(cmd, callback);
            } // End Using cmd 
        }



        public virtual System.Data.Common.DbDataReader ExecuteReader(
              System.Data.IDbConnection conn
            , System.Data.IDbCommand cmd
            , System.Data.CommandBehavior cmdBehaviour)
        {
            System.Data.Common.DbDataReader dataReader = null;

            try
            {
                cmd.Connection = conn;

                if (cmd.Connection.State != System.Data.ConnectionState.Open)
                    cmd.Connection.Open();

                // this line is Evil ;)
                dataReader = ((System.Data.Common.DbCommand)cmd).ExecuteReader(cmdBehaviour);
            }
            catch (System.Exception ex)
            {
                if (Log(ex, cmd))
                    throw;
            }

            return dataReader;
        }


        public virtual System.Data.Common.DbDataReader ExecuteReader(System.Data.IDbCommand cmd, System.Data.CommandBehavior cmdBehaviour)
        {
            System.Data.Common.DbDataReader idr = null;
            System.Data.IDbConnection idbc = null;

            try
            {
                idbc = this.Connection;
            }
            catch (System.Exception ex)
            {
                if (Log(ex, cmd))
                    throw;
            }

            idr = ExecuteReader(idbc, cmd, cmdBehaviour);

            return idr;
        } // End Function ExecuteReader


        public virtual System.Data.Common.DbDataReader ExecuteReader(System.Data.IDbCommand cmd)
        {
            return ExecuteReader(cmd, System.Data.CommandBehavior.CloseConnection);
        } // End Function ExecuteReader




        public virtual System.Data.Common.DbDataReader ExecuteReader(System.Data.IDbConnection conn, string strSQL, System.Data.CommandBehavior cmdBehaviour)
        {
            System.Data.Common.DbDataReader idr = null;

            using (System.Data.Common.DbCommand cmd = this.CreateCommand(strSQL))
            {
                idr = ExecuteReader(conn, cmd, cmdBehaviour);
            } // End Using cmd

            return idr;
        } // End Function ExecuteReader


        public virtual System.Data.Common.DbDataReader ExecuteReader(string strSQL, System.Data.CommandBehavior cmdBehaviour)
        {
            System.Data.Common.DbDataReader idr = null;

            using (System.Data.Common.DbCommand cmd = this.CreateCommand(strSQL))
            {
                idr = ExecuteReader(cmd, cmdBehaviour);
            } // End Using cmd

            return idr;
        } // End Function ExecuteReader


        public virtual System.Data.Common.DbDataReader ExecuteReader(string strSQL)
        {
            return ExecuteReader(strSQL, System.Data.CommandBehavior.CloseConnection);
        } // End Function ExecuteReader


        public virtual System.Data.Common.DbDataReader ExecuteReader(System.Data.IDbConnection conn, string strSQL)
        {
            System.Data.Common.DbDataReader idr = null;

            using (System.Data.Common.DbCommand cmd = this.CreateCommand(strSQL))
            {
                idr = ExecuteReader(conn, cmd, System.Data.CommandBehavior.CloseConnection);
            } // End Using cmd

            return idr;
        } // End Function ExecuteReader


        public virtual T ExecuteScalar<T>(System.Data.IDbCommand cmd, bool throwOnAssignNullToNonNullableType)
       {
           object objReturnValue = null;
            
           lock (cmd)
           {

               using (System.Data.IDbConnection idbc = this.Connection)
               {
                   cmd.Connection = idbc;

                   lock (cmd.Connection)
                   {

                       try
                       {
                           if (cmd.Connection.State != System.Data.ConnectionState.Open)
                               cmd.Connection.Open();

                           objReturnValue = cmd.ExecuteScalar();
                       } // End Try
                       catch (System.Data.Common.DbException ex)
                       {
                           if (Log(ex, cmd))
                               throw;
                       } // End Catch
                       finally
                       {
                           if (cmd.Connection.State != System.Data.ConnectionState.Closed)
                               cmd.Connection.Close();
                       } // End Finally

                   } // End lock (cmd.Connection)

               } // End using idbc

           } // End lock (cmd)

           return ConvertResult<T>(objReturnValue, throwOnAssignNullToNonNullableType);
       } // End Function ExecuteScalar(cmd)


       public virtual T ExecuteScalar<T>(System.Data.IDbCommand cmd)
       {
           return ExecuteScalar<T>(cmd, true);
       }


       public virtual T ExecuteScalar<T>(string strSQL, bool throwOnAssignNullToNonNullableType)
       {
           T tReturnValue = default(T);

           // pfff, mono C# compiler problem...
           //sqlCMD = new System.Data.SqlClient.SqlCommand(strSQL, m_SqlConnection);
           using (System.Data.IDbCommand cmd = CreateCommand(strSQL))
           {
               tReturnValue = ExecuteScalar<T>(cmd, throwOnAssignNullToNonNullableType);
           } // End Using cmd

           return tReturnValue;
       } // End Function ExecuteScalar(strSQL)


       public virtual T ExecuteScalar<T>(string strSQL)
       {
           return ExecuteScalar<T>(strSQL, true);
       } // End Function ExecuteScalar(strSQL)


   


        // Anything else than a struct or a class
       public virtual bool IsSimpleType(System.Type tThisType)
       {

           if (System.Reflection.IntrospectionExtensions.GetTypeInfo(tThisType).IsPrimitive)
           {
               return true;
           }

           if (object.ReferenceEquals(tThisType, typeof(System.String)))
           {
               return true;
           }

           if (object.ReferenceEquals(tThisType, typeof(System.DateTime)))
           {
               return true;
           }

           if (object.ReferenceEquals(tThisType, typeof(System.Guid)))
           {
               return true;
           }

           if (object.ReferenceEquals(tThisType, typeof(System.Decimal)))
           {
               return true;
           }

           if (object.ReferenceEquals(tThisType, typeof(System.Object)))
           {
               return true;
           }

           return false;
       } // End Function IsSimpleType


   
        // [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public T ConvertResult<T>(object objReturnValue, bool throwOnAssignNullToNonNullableType)
        {
            System.Type tReturnType = typeof(T);

            System.Reflection.TypeInfo ti = System.Reflection.IntrospectionExtensions.GetTypeInfo(tReturnType);
            bool typeIsNullable = (ti.IsGenericType && object.ReferenceEquals(tReturnType.GetGenericTypeDefinition(), typeof(System.Nullable<>)));
            bool typeCanBeAssignedNull = !ti.IsValueType || typeIsNullable;

            if (typeIsNullable)
            {
                tReturnType = System.Nullable.GetUnderlyingType(tReturnType);
            } // End if (typeIsNullable) 


            // Catch & Normalize NULL 
            if (objReturnValue == null || objReturnValue == System.DBNull.Value)
            {
                if (typeCanBeAssignedNull)
                    return (T)(object)null;

                if (throwOnAssignNullToNonNullableType)
                    throw new System.IO.InvalidDataException("ConvertResult cannot assign NULL to non-nullable type.");

                return default(T);
            } // End if (objReturnValue == null) 


            System.Type tDataTypeOfScalarResult = objReturnValue.GetType();
            if (object.ReferenceEquals(tReturnType, tDataTypeOfScalarResult))
                return (T)objReturnValue;

            string strReturnValue = System.Convert.ToString(objReturnValue, System.Globalization.CultureInfo.InvariantCulture);

            try
            {
                if (object.ReferenceEquals(tReturnType, typeof(object)))
                {
                    return (T)objReturnValue;
                }
                else if (object.ReferenceEquals(tReturnType, typeof(string)))
                {
                    return (T)(object)strReturnValue;
                } // End if string
                else if (object.ReferenceEquals(tReturnType, typeof(bool)))
                {
                    bool bReturnValue = false;

                    if (bool.TryParse(strReturnValue, out bReturnValue))
                        return (T)(object)bReturnValue;

                    if (strReturnValue == "0")
                        return (T)(object)false;

                    if (strReturnValue == "0.0")
                        return (T)(object)false;

                    return (T)(object)true;
                } // End if bool
                else if (object.ReferenceEquals(tReturnType, typeof(int)))
                {
                    int iReturnValue;
                    if (int.TryParse(strReturnValue, out iReturnValue))
                        return (T)(object)iReturnValue;

                    throw new System.IO.InvalidDataException("ConvertResult: Returned data \"" + strReturnValue + "\" is not a valid integer.");
                } // End if int
                else if (object.ReferenceEquals(tReturnType, typeof(uint)))
                {
                    uint uiReturnValue;
                    if (uint.TryParse(strReturnValue, out uiReturnValue))
                        return (T)(object)uiReturnValue;

                    throw new System.IO.InvalidDataException("ConvertResult: Returned data \"" + strReturnValue + "\" is not a valid unsigned integer.");
                } // End if uint
                else if (object.ReferenceEquals(tReturnType, typeof(long)))
                {
                    long lngReturnValue;
                    if (long.TryParse(strReturnValue, out lngReturnValue))
                        return (T)(object)lngReturnValue;

                    throw new System.IO.InvalidDataException("ConvertResult: Returned data \"" + strReturnValue + "\" is not a valid unsigned integer.");
                } // End if long
                else if (object.ReferenceEquals(tReturnType, typeof(ulong)))
                {
                    ulong ulngReturnValue;
                    if (ulong.TryParse(strReturnValue, out ulngReturnValue))
                        return (T)(object)ulngReturnValue;

                    throw new System.IO.InvalidDataException("ConvertResult: Returned data \"" + strReturnValue + "\" is not a valid unsigned long.");
                } // End if ulong
                else if (object.ReferenceEquals(tReturnType, typeof(float)))
                {
                    float fltReturnValue;
                    if (float.TryParse(strReturnValue, out fltReturnValue))
                        return (T)(object)fltReturnValue;

                    throw new System.IO.InvalidDataException("ConvertResult: Returned data \"" + strReturnValue + "\" is not a valid float.");
                }
                else if (object.ReferenceEquals(tReturnType, typeof(double)))
                {
                    double dblReturnValue;
                    if (double.TryParse(strReturnValue, out dblReturnValue))
                        return (T)(object)dblReturnValue;

                    throw new System.IO.InvalidDataException("ConvertResult: Returned data \"" + strReturnValue + "\" is not a valid double.");
                }
                else if (object.ReferenceEquals(tReturnType, typeof(System.Net.IPAddress)))
                {
                    System.Net.IPAddress ipAddress = null;

                    if (System.Net.IPAddress.TryParse(strReturnValue, out ipAddress))
                        return (T)(object)ipAddress;

                    throw new System.IO.InvalidDataException("ConvertResult: Returned data \"" + strReturnValue + "\" is not a valid IP address.");
                } // End if IPAddress
                else if (object.ReferenceEquals(tReturnType, typeof(System.Guid)))
                {
                    System.Guid retUID;

                    try
                    {
                        retUID = new System.Guid(strReturnValue);
                        return (T)(object)retUID;
                    }
                    catch (System.Exception ex)
                    { 
                    
                    }

                    throw new System.IO.InvalidDataException("ConvertResult: Returned data \"" + strReturnValue + "\" is not a valid GUID.");
                } // End if System.Guid
                else // No datatype matches
                {
                    throw new System.NotImplementedException("ConvertResult<T>: No implicit conversion operation defined for type \"" + tReturnType.FullName + "\".");
                } // End else of if tReturnType = datatype

            } // End Try
            catch (System.Exception ex)
            {
                if (Log(ex))
                    throw;
            } // End Catch

            return default(T);
        }


        public bool Log(System.Exception ex)
        { 
        return Log(ex, null);
        }

        public bool Log(System.Exception ex, System.Data.IDbCommand cmd)
        {
            System.Console.WriteLine(ex.Message);
            System.Console.WriteLine(ex.StackTrace);
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.NewLine);

            return true;
        }


        public virtual string RemoveSingleLineSqlComments(string strSQLwithComments)
        {
            if (string.IsNullOrEmpty(strSQLwithComments))
                return strSQLwithComments;

            strSQLwithComments = strSQLwithComments.Replace("\r", "\n");
            strSQLwithComments = strSQLwithComments.Replace("\n\n", "\n");

            string[] astrLines = strSQLwithComments.Split('\n');

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (string strThisLine in astrLines)
            {
                if (!string.IsNullOrEmpty(strThisLine))
                {
                    string strTrimmedLine = strThisLine.Trim();
                    if (!strTrimmedLine.StartsWith("--"))
                    {
                        if (!string.IsNullOrEmpty(strTrimmedLine))
                        {
                            sb.AppendLine(strTrimmedLine);
                        } // End if (!string.IsNullOrEmpty(strTrimmedLine))

                    } // End if (!strTrimmedLine.StartsWith("--"))

                } // End if (!string.IsNullOrEmpty(strThisLine))

            } // Next strThisLine

            string strReturnValue = sb.ToString();
            sb = null;

            return strReturnValue;
        } // End Function RemoveSingleLineSqlComments


        public virtual T GetClass<T>(System.Data.IDbCommand cmd)
        {
            T tThisClassInstance = default(T);

            System.Type t = typeof(T);

            using (System.Data.IDataReader idr = ExecuteReader(cmd))
            {
                while (idr.Read())
                {
                    tThisClassInstance = System.Activator.CreateInstance<T>();

                    int iFieldCount = idr.FieldCount;
                    Setter_t<T>[] mems = new Setter_t<T>[iFieldCount];

                    for (int i = 0; i < iFieldCount; ++i)
                    {
                        string strName = idr.GetName(i);
                        mems[i] = LinqHelper.GetSetter<T>(strName);
                    } // Next i


                    for (int i = 0; i < idr.FieldCount; ++i)
                    {

                        Setter_t<T> setter = mems[i];

                        if (setter != null)
                        {
                            object objVal = idr.GetValue(i);
                            setter(tThisClassInstance, objVal);
                        }

                    } // Next i

                    break;
                } // Whend

                idr.Close();

            } // End Using idr

            return tThisClassInstance;
        } // End Function GetClass
        
        public virtual T GetClass<T>(string strSQL)
        {
            T tReturnClassInstance = default(T);

            using (System.Data.IDbCommand cmd = CreateCommand(strSQL))
            {
                tReturnClassInstance = GetClass<T>(cmd);
            } // End Using cmd

            return tReturnClassInstance;
        } // End Function GetClass


        public virtual System.Collections.Generic.List<T> GetList<T>(System.Data.IDbCommand cmd)
        {
            System.Collections.Generic.List<T> lsReturnValue = new System.Collections.Generic.List<T>();
            T tThisValue = default(T);
            System.Type tThisType = typeof(T);

            lock (cmd)
            {
                using (System.Data.Common.DbDataReader rdr = ExecuteReader(cmd))
                {

                    lock (rdr)
                    {

                        if (IsSimpleType(tThisType))
                        {
                            while (rdr.Read())
                            {
                                object objVal = rdr.GetValue(0);

                                // tThisValue = System.Convert.ChangeType(objVal, T),
                                tThisValue = (T)ConvertResult<T>(objVal, true);

                                lsReturnValue.Add(tThisValue);
                            } // End while (idr.Read())

                        }
                        else
                        {
                            int iFieldCount = rdr.FieldCount;
                            Setter_t<T>[] mems = new Setter_t<T>[iFieldCount];

                            for (int i = 0; i < iFieldCount; ++i)
                            {
                                string strName = rdr.GetName(i);
                                mems[i] = LinqHelper.GetSetter<T>(strName);
                            } // Next i


                            while (rdr.Read())
                            {
                                tThisValue = System.Activator.CreateInstance<T>();

                                for (int i = 0; i < iFieldCount; ++i)
                                {
                                    Setter_t<T> setter = mems[i];

                                    if (setter != null)
                                    {
                                        object objVal = rdr.GetValue(i);
                                        setter(tThisValue, objVal);
                                    }

                                } // Next i

                                lsReturnValue.Add(tThisValue);
                            } // Whend

                        } // End if IsSimpleType(tThisType)

#if NETCORE_2_0
                        rdr.Close();
#endif
                    } // End Lock rdr

                } // End Using rdr

            } // End lock cmd

            return lsReturnValue;
        } // End Function GetList


        public System.Collections.Generic.List<T> GetList<T>(string sql)
        {
            System.Collections.Generic.List<T> ls = null;

            using(System.Data.Common.DbCommand cmd = CreateCommand(sql))
            {
                ls = this.GetList<T>(cmd);
            }

            return ls;
        } // End Function GetList
        

    }
}
