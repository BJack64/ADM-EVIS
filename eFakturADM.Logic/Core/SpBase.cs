using System;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections;
using System.Xml;

namespace eFakturADM.Logic.Core
{
    /// <summary>
    /// Represents request type enum.
    /// </summary>
    public enum DbBaseRequestType
    {
        /// <summary>
        /// Parameterized SQL statement.
        /// </summary>
        ParameterizedSQLText = 1,
        /// <summary>
        /// Strored procedure.
        /// </summary>
        StoredProcedure = 2
    }

    /// <summary>
    /// Provides a wrapper for using SQL Stored Procedures and Commands.
    /// </summary>
    public class SpBase
    {				
        /// <summary>
        /// Throw an exception if something wrong happend inside SQL executor class. Will be removed in next release.
        /// </summary>
        protected static bool ThrowExceptionIsOn = true;

        /// <summary>
        /// DbBase object.
        /// </summary>
        protected DbBaseRequestType RequestType = DbBaseRequestType.ParameterizedSQLText;


        #region Member variables

        /// <summary>
        /// Parameterized SQL statement or strored procedure name.
        /// </summary>
        private String m_ProcedureName;

        /// <summary>
        /// SQL connection object.
        /// </summary>
        private IDbConnection m_SqlConnection;

        /// <summary>
        /// SQL command object.
        /// </summary>
        private IDbCommand m_SqlCommand;

        /// <summary>
        /// Parameters name value hash table.
        /// </summary>
        private Hashtable m_HashParameters = new Hashtable();

        /// <summary>
        /// SQL adapter object.
        /// </summary>
        private IDataAdapter m_SqlDataAdapter;

        /// <summary>
        /// SQL reader object.
        /// </summary>
        private IDataReader m_SqlDataReader;

        /// <summary>
        /// Connetion string value.
        /// </summary>
        private String m_strConnection;

        /// <summary>
        /// DataSet object
        /// </summary>
        private DataSet m_Dataset;

        /// <summary>
        /// DataView object.
        /// </summary>
        private DataView m_DataView;

        /// <summary>
        /// True if an error happened.
        /// </summary>
        private bool blnError = false;

        /// <summary>
        /// Error message string an error happened or emty string.
        /// </summary>
        private String strErrorMessage;

        #endregion

        #region Constructors

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public SpBase()
        {
        }

        /// <summary>
        /// Constructor. Needs a procedure name
        /// </summary>
        /// <param name="strConnection">Connection string.</param>
        /// <param name="ProcedureName">Stored procedure name or parameterized SQL statement.</param>
        public SpBase(String strConnection, String ProcedureName)
        {
            m_ProcedureName = ProcedureName;
            m_strConnection = strConnection;
        }

        /// <summary>
        /// Constructor. Needs a procedure name.
        /// </summary>
        /// <param name="ProcedureName">Stored procedure name or parameterized SQL statement.</param>
        public SpBase(String ProcedureName)
        {
            m_ProcedureName = ProcedureName;
        }

        public SpBase(String ProcedureName, DbBaseRequestType ReqType)
        {
            m_ProcedureName = ProcedureName;
            RequestType = ReqType;
        }

        /// <summary>
        /// Constructor. Needs a procedure name and a hashtable containing the parameters
        /// </summary>
        /// <param name="strConnection">Connection string.</param>
        /// <param name="ProcedureName">Stored procedure name or parameterized SQL statement.</param>
        /// <param name="HashParameters">Parameters hash table.</param>
        public SpBase(String strConnection, String ProcedureName, Hashtable HashParameters)
        {
            m_ProcedureName = ProcedureName;
            m_HashParameters = HashParameters;
            m_strConnection = strConnection;
        }

        /// <summary>
        /// Constructor. Needs a procedure name and a hashtable containing the parameters
        /// </summary>
        /// <param name="ProcedureName">Stored procedure name or parameterized SQL statement.</param>
        /// <param name="HashParameters">Parameters hash table.</param>
        public SpBase(String ProcedureName, Hashtable HashParameters)
        {
            m_ProcedureName = ProcedureName;
            m_HashParameters = HashParameters;
        }

        /// <summary>
        /// Constructor. Needs a procedure name and key/value set as Parameters
        /// </summary>
        /// <param name="strConnection">Connection string.</param>
        /// <param name="ProcedureName">Stored procedure name or parameterized SQL statement.</param>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="ParameterValue">Parameter value.</param>
        public SpBase(String strConnection, String ProcedureName, string ParameterName, Object ParameterValue)
        {
            m_ProcedureName = ProcedureName;
            m_strConnection = strConnection;
            AddParameter(ParameterName, ParameterValue);
        }

        /// <summary>
        /// Constructor. Needs a procedure name and key/value set as Parameters.
        /// </summary>
        /// <param name="ProcedureName">Stored procedure name or parameterized SQL statement.</param>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="ParameterValue">Parameter value.</param>
        public SpBase(String ProcedureName, string ParameterName, Object ParameterValue)
        {
            m_ProcedureName = ProcedureName;
            AddParameter(ParameterName, ParameterValue);
        }

        #endregion

        #region Public methods
        /// <summary>
        /// Creates a SqlParameter, no properties set
        /// </summary>
        /// <returns></returns>
        private IDataParameter AddParameter()
        {
            try
            {
                return new SqlParameter();
            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;

                if (ThrowExceptionIsOn)
                    throw e;

                return (SqlParameter)null;
            }
        }

        /// <summary>
        /// Add a parameter (parameter name, parameter value)
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="ParameterValue">Parameter value.</param>
        /// <returns></returns>
        public IDataParameter AddParameter(string ParameterName, Object ParameterValue)
        {
            return AddParameter(ParameterName, ParameterValue, ParameterDirection.Input);
        }

        /// <summary>
        /// Gets parameter object by name.
        /// </summary>
        /// <param name="ParameterName"></param>
        /// <returns></returns>
        public object GetParameter(string ParameterName)
        {
            var paramDats = ((IDbDataParameter) Parameters[ParameterName]);
            if (paramDats == null) return null;
            return paramDats.Value;
        }

        /// <summary>
        /// Add a parameter (parameter name, parameter value, parameter direction)
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="ParameterValue">Parameter value.</param>
        /// <param name="paramDirection">Direction: Input, Output, InputOutput.</param>
        /// <returns></returns>
        public IDataParameter AddParameter(string ParameterName, Object ParameterValue, ParameterDirection paramDirection)
        {
            String strCleanKey = CleanKey(ParameterName);

            IDataParameter paramSql = AddParameter();
            paramSql.ParameterName = "@" + strCleanKey;
            paramSql.Value = ParameterValue;
            paramSql.Direction = paramDirection;

            m_HashParameters.Add(strCleanKey, paramSql);
            return paramSql;
        }


        /// <summary>
        /// Add a parameter (parameter name, parameter value, parameter direction, parameter sqldbtype)
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="ParameterValue">Parameter value.</param>
        /// <param name="paramDirection">Direction: Input, Output, InputOutput.</param>
        /// <param name="paramSqlDbType">Parameter SQL type.</param>
        /// <returns></returns>
        public IDataParameter AddParameter(string ParameterName, Object ParameterValue, ParameterDirection paramDirection, DbType paramSqlDbType)
        {
            IDataParameter param = AddParameter(ParameterName, ParameterValue, paramDirection);
            param.DbType = paramSqlDbType;

            AddDefaultPrecisionAndScale(param);

            return param;
        }

        /// <summary>
        /// Add a parameter (parameter name, parameter value, parameter direction, parameter sqldbtype, int paramSize)
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="ParameterValue">Parameter value.</param>
        /// <param name="paramDirection">Direction: Input, Output, InputOutput.</param>
        /// <param name="paramSqlDbType">Parameter SQL type.</param>
        /// <param name="intSize">Parameter size.</param>
        /// <returns></returns>
        public IDataParameter AddParameter(string ParameterName, Object ParameterValue, ParameterDirection paramDirection, DbType paramSqlDbType, int intSize)
        {
            IDataParameter param = AddParameter(ParameterName, ParameterValue, paramDirection);
            param.DbType = paramSqlDbType;

            ((SqlParameter)param).Size = intSize;

            AddDefaultPrecisionAndScale(param);

            return param;
        }


        /// <summary>
        /// Add a parameter (ParameterName, paramDirection, paramSqlDbType)
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="paramDirection">Direction: Input, Output, InputOutput.</param>
        /// <param name="paramSqlDbType">Parameter SQL type.</param>
        /// <returns></returns>
        public IDataParameter AddParameter(string ParameterName, ParameterDirection paramDirection, DbType paramSqlDbType)
        {
            String strCleanKey = CleanKey(ParameterName);

            IDataParameter paramSql = AddParameter();
            paramSql.ParameterName = "@" + strCleanKey;
            paramSql.DbType = paramSqlDbType;
            paramSql.Direction = paramDirection;

            AddDefaultPrecisionAndScale(paramSql);

            m_HashParameters.Add(strCleanKey, paramSql);
            return paramSql;
        }

        /// <summary>
        /// Add a parameter (ParameterName, paramDirection, paramSqlDbType, intSize)
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="paramDirection">Direction: Input, Output, InputOutput.</param>
        /// <param name="paramSqlDbType">Parameter SQL type.</param>
        /// <param name="intSize">Parameter size.</param>
        /// <returns></returns>
        public IDataParameter AddParameter(string ParameterName, ParameterDirection paramDirection, DbType paramSqlDbType, int intSize)
        {
            IDataParameter param = AddParameter(ParameterName, paramDirection, paramSqlDbType);

            ((SqlParameter)param).Size = intSize;

            AddDefaultPrecisionAndScale(param);
            return param;
        }

        /// <summary>
        /// Add a parameter ( ParameterName, paramDirection, paramSqlDbType, intSize, intPrecision, intScale)
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="paramDirection">Direction: Input, Output, InputOutput.</param>
        /// <param name="paramSqlDbType">Parameter SQL type.</param>
        /// <param name="intSize">Parameter size.</param>
        /// <param name="intPrecision"></param>
        /// <param name="intScale"></param>
        /// <returns></returns>
        public IDataParameter AddParameter(string ParameterName, ParameterDirection paramDirection, DbType paramSqlDbType, int intSize, int intPrecision, int intScale)
        {
            IDataParameter param = AddParameter(ParameterName, paramDirection, paramSqlDbType, intSize);

            ((SqlParameter)param).Precision = (byte)intPrecision;
            ((SqlParameter)param).Scale = (byte)intScale;

            return param;
        }

        /// <summary>
        /// Add a parameter ( ParameterName, paramDirection, paramSqlDbType, intPrecision, intScale).
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <param name="paramDirection">Direction: Input, Output, InputOutput.</param>
        /// <param name="paramSqlDbType">Parameter SQL type.</param>
        /// <param name="intPrecision"></param>
        /// <param name="intScale"></param>
        /// <returns></returns>
        public IDataParameter AddParameter(string ParameterName, ParameterDirection paramDirection, DbType paramSqlDbType, int intPrecision, int intScale)
        {
            IDataParameter param = AddParameter(ParameterName, paramDirection, paramSqlDbType);

            ((SqlParameter)param).Precision = (byte)intPrecision;
            ((SqlParameter)param).Scale = (byte)intScale;

            return param;
        }

        private void AddDefaultPrecisionAndScale(IDataParameter param)
        {
            // if direction is inputoutput or output and type is decimal: add default values for scale and precision
            if (param.DbType == DbType.Decimal && (param.Direction == ParameterDirection.InputOutput || param.Direction == ParameterDirection.InputOutput))
            {
                ((SqlParameter)param).Precision = 18;
                ((SqlParameter)param).Scale = 5;
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            if (m_SqlConnection != null && m_SqlConnection.State != ConnectionState.Closed && m_SqlConnection.State != ConnectionState.Broken)
                m_SqlConnection.Close();
        }

        /// <summary>
        /// Executes a command in non-query mode.
        /// </summary>
        public int ExecuteNonQuery()
        {
            Init();

            try
            {
                m_SqlCommand.ExecuteNonQuery();
                m_SqlConnection.Close();
                return (int)ParameterValue("RETURN_VALUE");
            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;

                if (ThrowExceptionIsOn)
                    throw e;

                return -1;
            }
        }

        /// <summary>
        /// Executes a command returning a SqlDataAdapter. The connection has to be closed!
        /// </summary>
        public IDataAdapter ExecuteDataAdapter()
        {
            Init();

            try
            {
                m_SqlDataAdapter = new SqlDataAdapter((SqlCommand)m_SqlCommand);

                return m_SqlDataAdapter;

            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;

                if (ThrowExceptionIsOn)
                    throw e;

                return (SqlDataAdapter)null;
            }
        }

        /// <summary>
        /// Executes a command in scalar mode.
        /// </summary>
        public object ExecuteScalar()
        {
            Init();

            try
            {
                object value = m_SqlCommand.ExecuteScalar();
                m_SqlConnection.Close();
                return value;
            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;

                if (ThrowExceptionIsOn)
                    throw e;

                return (object)null;
            }
        }

        /// <summary>
        /// Executes a command returning a XML Reader.
        /// </summary>
        public XmlReader ExecuteXMLReader()
        {
            Init();

            try
            {
                return ((SqlCommand)m_SqlCommand).ExecuteXmlReader();
            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;


                if (ThrowExceptionIsOn)
                    throw e;

                return (XmlReader)null;
            }
        }

        /// <summary>
        /// Executes a command returning a DataSet.
        /// </summary>
        /// <param name="strTablename">Table name.</param>
        /// <returns>Result DataSte object.</returns>
        public DataSet ExecuteDataSet(String strTablename)
        {
            ExecuteDataAdapter();

            m_Dataset = new DataSet();

            try
            {
                // Fill DataSet
                m_Dataset = new System.Data.DataSet(strTablename);
                m_SqlDataAdapter.Fill(m_Dataset);
                m_SqlConnection.Close();
            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;

                if (ThrowExceptionIsOn)
                    throw e;
            }

            return m_Dataset;
        }

        /// <summary>
        /// Executes a command returning a DataView.
        /// </summary>
        /// <param name="strTablename">Tabel name.</param>
        /// <returns>Result DataView object.</returns>
        public DataView ExecuteDataView(String strTablename)
        {
            ExecuteDataAdapter();

            m_DataView = new DataView();

            try
            {
                DataSet dt = new DataSet(strTablename);
                m_SqlDataAdapter.Fill(dt);

                // Fill DataView
                m_DataView = new DataView(dt.Tables[0]);
                m_SqlConnection.Close();
            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;

                if (ThrowExceptionIsOn)
                    throw e;
            }

            return m_DataView;
        }

        /// <summary>
        /// Executes a command returning a SqlDataReader. The SqlDataReader has to be closed!
        /// </summary>
        /// <returns>SqlDataReader object.</returns>
        public IDataReader ExecuteReader()
        {
            Init();

            try
            {
                m_SqlDataReader = m_SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                return m_SqlDataReader;
            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;

                if (ThrowExceptionIsOn)
                    throw e;

                return (SqlDataReader)null;
            }
        }

        /// <summary>
        /// Gets  the value of a parameter.
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <returns>Object value.</returns>
        public Object ParameterValue(String ParameterName)
        {
            try
            {
                return ((SqlParameter)m_HashParameters[CleanKey(ParameterName)]).Value;
            }
            catch (Exception e)
            {
                this.blnError = true;
                this.strErrorMessage = e.Message;

                if (ThrowExceptionIsOn)
                    throw e;

                return (SqlParameter)null;
            }
        }

        #endregion

        #region Properties


        /// <summary>
        /// Gets  the Sql connection
        /// </summary>
        public IDbConnection SqlConnection
        {
            get
            {
                return m_SqlConnection;
            }
        }

        /// <summary>
        /// Gets  / sets the stored procedure name
        /// </summary>
        public String Procedurename
        {
            get
            {
                return m_ProcedureName;
            }
            set
            {
                m_ProcedureName = value;
            }
        }

        /// <summary>
        /// Gets  the SqlCommand
        /// </summary>
        public IDbCommand SqlCommand
        {
            get
            {
                return m_SqlCommand;
            }
        }

        /// <summary>
        /// Gets  the SqlDataAdapter
        /// </summary>
        public IDataAdapter SqlDataAdapter
        {
            get
            {
                return m_SqlDataAdapter;
            }
        }

        /// <summary>
        /// Gets  the SqlDataReader
        /// </summary>
        public IDataReader SqlDataReader
        {
            get
            {
                return m_SqlDataReader;
            }
        }

        /// <summary>
        /// Gets  the dataset
        /// </summary>
        public DataSet DataSet
        {
            get
            {
                return m_Dataset;
            }
        }

        /// <summary>
        /// Gets  the dataset
        /// </summary>
        public DataView DataView
        {
            get
            {
                return m_DataView;
            }
        }

        /// <summary>
        /// Gets  / sets the parameters hashtable
        /// </summary>
        public Hashtable Parameters
        {
            get
            {
                return m_HashParameters;
            }
            set
            {
                m_HashParameters = value;
            }
        }

        /// <summary>
        /// Gets  / sets the parameters hashtable
        /// </summary>
        public String ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["eFakturADM.Connection.String"];
            }
        }

        /// <summary>
        /// Timeout (in seconds) for sql command.
        /// </summary>
        public int CommandTimeOut
        {
            get
            {
                var tSet = System.Configuration.ConfigurationManager.AppSettings["eFakturADM.Connection.CommandTimeOut"];
                return Convert.ToInt32(tSet);
            }
        }

        /// <summary>
        /// Gets the error setting (true is there was an error)
        /// </summary>
        public bool IsError
        {
            get { return blnError; }
        }

        /// <summary>
        /// Gets the last error message, if any.
        /// </summary>
        public string ErrorMessage
        {
            get { return strErrorMessage; }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Does the basic job of initialising the db stuff (connection, command) and adding the parameters
        /// </summary>
        private void Init()
        {
            Init(RequestType);
        }

        /// <summary>
        /// Does the basic job of initialising the db stuff (connection, command) and adding the parameters
        /// </summary>
        /// <param name="RequestType">Parameterized SQL text or stored procedure.</param>
        /// <param name="DatabaseProvider">Database server provider</param>
        private void Init(DbBaseRequestType RequestType)
        {
            m_strConnection = ConnectionString;

            try
            {
                m_SqlConnection = new SqlConnection(m_strConnection);

                m_SqlConnection.Open();
            }
            catch (Exception e)
            {
                if (ThrowExceptionIsOn)
                    throw e;

                return;
            }

            // Creates a SqlCommand and sets type
            m_SqlCommand = new SqlCommand(m_ProcedureName, (SqlConnection)m_SqlConnection);
            
            if (RequestType == DbBaseRequestType.ParameterizedSQLText)
                m_SqlCommand.CommandType = CommandType.Text;
            else if (RequestType == DbBaseRequestType.StoredProcedure)
                m_SqlCommand.CommandType = CommandType.StoredProcedure;

            if (this.CommandTimeOut > 0)
                m_SqlCommand.CommandTimeout = this.CommandTimeOut;

            // Add a return value parameter by default
            AddParameter("RETURN_VALUE", DbType.Int16, ParameterDirection.ReturnValue);

            // Adds the necessary parameters to the command
            foreach (IDbDataParameter p in m_HashParameters.Values)
                m_SqlCommand.Parameters.Add(p);
        }

        /// <summary>
        /// Gets a key string, without preceding '@'.
        /// </summary>
        /// <param name="ParameterName">Parameter name.</param>
        /// <returns></returns>
        private String CleanKey(String ParameterName)
        {
            String strCleanKey = ParameterName;

            if (strCleanKey.Substring(0, 1).Equals("@"))
                strCleanKey = strCleanKey.Substring(1);

            return strCleanKey;
        }

        #endregion
    }

}
