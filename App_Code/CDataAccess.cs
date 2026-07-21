using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Data.SqlClient;
using System.Collections;
using System.IO;


namespace DBManager
{
    /// <summary>
    /// Summary description for CDataAccess
    /// </summary>
    /// 
    #region "CDataAccess Class"


    public class CDataAccess
    {

        #region "Fields and Enums"
        // Fields
        private SqlConnection connection;
        private string connectionString;
        private CDataAccess.DataAccessErrors errorCode;
        private string errorMessage;
        private bool isBeginTransaction;
        private bool keepConnectionOpened;
        private SqlDataAdapter daDataAdapter;
        private DataTable dtDataTable;
        private DataView dvDataView;
        private SqlCommandBuilder cbCommandBuilder;

        // Nested Types
        public enum DataAccessErrors
        {
            Successful = 0,
            Failed = 1,
            ConnectionOpenFailure = 2,
            ConnectionAlreadyOpened = 3,
            DataFetchFailure = 4,
            DataInsertFailure = 5,
            DataUpdateFailure = 6,
            ConcurrencyError = 7,
            AddNewFailure = 8,
            AllowNewFailure = 9,
            RecordMovementfailed = 10
            //public int value__;
        }

        #endregion


        #region "Properties"

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }

        public CDataAccess.DataAccessErrors ErrorCode
        {
            get
            {
                return this.errorCode;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this.errorMessage;
            }
        }

        #endregion


        #region "Constructors"

        //public CDataAccess()
        //{
        //    this.connectionString = "";
        //    this.errorCode = CDataAccess.DataAccessErrors.Successful;
        //    this.errorMessage = "";
        //    this.connection = null;
        //    this.keepConnectionOpened = false;
        //    this.isBeginTransaction = false;
        //}

        //public CDataAccess(SqlConnection connection)
        //{
        //    this.connectionString = "";
        //    this.errorCode = CDataAccess.DataAccessErrors.Successful;
        //    this.errorMessage = "";
        //    this.connection = null;
        //    this.keepConnectionOpened = false;
        //    this.isBeginTransaction = false;
        //    this.connection = connection;
        //    this.keepConnectionOpened = true;
        //}

        public CDataAccess(string connectionString)
        {
            this.connectionString = connectionString;
            this.errorCode = CDataAccess.DataAccessErrors.Successful;
            this.errorMessage = "";
            this.connection = null;
            this.keepConnectionOpened = false;
            this.isBeginTransaction = false;
            this.ConnectionString = connectionString;
        }

        #endregion



        #region "Connection Related"

        public SqlConnection GetConnection()
        {
            return new SqlConnection(this.connectionString);
        }


        public bool OpenConnection()
        {
            bool flag1;
            if ((this.connection != null) && (this.connection.State == ConnectionState.Open))
            {
                this.errorCode = CDataAccess.DataAccessErrors.ConnectionAlreadyOpened;
                return true;
            }
            try
            {
                this.connection = new SqlConnection(this.connectionString);
                this.connection.Open();
                this.errorCode = CDataAccess.DataAccessErrors.Successful;
                this.errorMessage = "";
                flag1 = true;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return flag1;
        }


        public void CloseConnection()
        {
            if (!this.keepConnectionOpened)
            {
                if ((this.connection != null) && (this.connection.State == ConnectionState.Open))
                {
                    this.connection.Close();
                }
                this.connection = null;
            }
        }

        #endregion


        #region "Parameter Related"

        private void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters != null) && (parameterValues != null))
            {
                bool flag1 = commandParameters[0].ParameterName == "@RETURN_VALUE";
                if (commandParameters.Length != parameterValues.Length)
                {
                    if (!flag1)
                    {
                        throw new ArgumentException("Parameter count does not match Parameter Value count.");
                    }
                    if ((commandParameters.Length - parameterValues.Length) != 1)
                    {
                        throw new ArgumentException("Parameter count does not match Parameter Value count.");
                    }
                }
                if (flag1)
                {
                    int num1 = 1;
                    int num2 = commandParameters.Length;
                    while (num1 < num2)
                    {
                        commandParameters[num1].Value = parameterValues[num1 - 1];
                        num1++;
                    }
                }
                else
                {
                    int num3 = 0;
                    int num4 = commandParameters.Length;
                    while (num3 < num4)
                    {
                        commandParameters[num3].Value = parameterValues[num3];
                        num3++;
                    }
                }
            }
        }


        private void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter parameter1 in commandParameters)
            {
                if ((parameter1.Direction == ParameterDirection.InputOutput) && (parameter1.Value == null))
                {
                    parameter1.Value = DBNull.Value;
                }
                command.Parameters.Add(parameter1);
            }
        }

        #endregion


        #region "Transaction Related"

        public bool BeginTransaction()
        {
            bool flag1;
            try
            {
                this.GetCommand("Start Transaction;").ExecuteNonQuery();
                this.isBeginTransaction = true;
                flag1 = true;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return flag1;
        }


        public bool CommitTransaction()
        {
            bool flag1;
            try
            {
                this.GetCommand("Commit;").ExecuteNonQuery();
                this.CloseConnection();
                this.isBeginTransaction = false;
                flag1 = true;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return flag1;
        }


        public bool RollbackTransaction()
        {
            bool flag1;
            try
            {
                this.GetCommand("Rollback;").ExecuteNonQuery();
                this.CloseConnection();
                this.isBeginTransaction = false;
                flag1 = true;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return flag1;
        }

        #endregion


        #region "Execute SQL and SP"

        public int Execute(string StrTSQL)
        {
            int num2;
            try
            {
                int num1 = this.GetCommand(StrTSQL).ExecuteNonQuery();
                num2 = num1;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return num2;
        }

        public DataSet ExecuteSP(string spName)
        {
            DataSet set2;
            try
            {
                this.OpenConnection();
                this.errorCode = CDataAccess.DataAccessErrors.Successful;
                DataSet set1 = this.ExecuteSP(spName, null);
                if (!this.isBeginTransaction)
                {
                    this.CloseConnection();
                }
                set2 = set1;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return set2;
        }

        public DataSet ExecuteSP(SqlTransaction transaction, string spName)
        {
            DataSet set2;
            try
            {
                DataSet set1 = this.ExecuteSP(transaction, spName, null);
                set2 = set1;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return set2;
        }

        public DataSet ExecuteSP(string spName, params SqlParameter[] commandParameters)
        {
            DataSet set2;
            try
            {
                this.OpenConnection();
                SqlCommand command1 = new SqlCommand();
                this.PrepareCommand(command1, null, spName, commandParameters);
                SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
                DataSet set1 = new DataSet();
                adapter1.Fill(set1);
                command1.Parameters.Clear();
                set2 = set1;
                this.CloseConnection();
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return set2;
        }

        public DataSet ExecuteSP(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            DataSet set2;
            try
            {
                this.connection = transaction.Connection;
                DataSet set1 = null;
                if ((parameterValues != null) && (parameterValues.Length > 0))
                {
                    SqlParameter[] parameterArray1 = CDataAccess.SqlParameterCache.GetSpParameterSet(transaction.Connection, spName, false);
                    this.AssignParameterValues(parameterArray1, parameterValues);
                    set1 = this.ExecuteSP(transaction, spName, parameterArray1);
                }
                else
                {
                    set1 = this.ExecuteSP(transaction, spName);
                }
                set2 = set1;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return set2;
        }

        public DataSet ExecuteSPWithDataSet(string spName, params object[] parameterValues)
        {
            DataSet set1 = null;
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                this.OpenConnection();
                this.errorCode = CDataAccess.DataAccessErrors.Successful;
                SqlParameter[] parameterArray1 = CDataAccess.SqlParameterCache.GetSpParameterSet(this.connection, spName, true);
                this.AssignParameterValues(parameterArray1, parameterValues);
                return this.ExecuteSP(spName, parameterArray1);
            }
            this.ExecuteSP(spName);
            if (!this.isBeginTransaction)
            {
                this.CloseConnection();
            }
            return set1;
        }

        public int ExecuteSPWithReturn(string spName, params object[] parameterValues)
        {
            int num1 = 0;
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                this.OpenConnection();
                this.errorCode = CDataAccess.DataAccessErrors.Successful;
                SqlParameter[] parameterArray1 = CDataAccess.SqlParameterCache.GetSpParameterSet(this.connection, spName, true);
                this.AssignParameterValues(parameterArray1, parameterValues);
                this.ExecuteSP(spName, parameterArray1);
                num1 = (int)parameterArray1[0].Value;
            }
            else
            {
                this.ExecuteSP(spName);
            }
            if (!this.isBeginTransaction)
            {
                this.CloseConnection();
            }
            return num1;
        }

        public DataTable ExecuteSPWithReturnDataTable(String spName, params SqlParameter[] CommandParameter)
        {
            DataTable tmpDataTable = new DataTable();
            //try
            //{
            this.OpenConnection();
            SqlCommand sqlCommand = new SqlCommand();
            this.PrepareCommand(sqlCommand, null, spName, CommandParameter);
            //this.PrepareCommand(sqlCommand, null, spName, CommandParameter);
            SqlDataAdapter SqlDataAdapter = new SqlDataAdapter(sqlCommand);
            SqlDataAdapter.Fill(tmpDataTable).ToString();
            this.CloseConnection();
            //}
            //catch (SqlException exception1)
            //{
            //    //(exception1.Message);
            //}

            //Catch exception2 As Exception
            //    Throw New Exception(exception2.Message)
            //End Try
            return tmpDataTable;
        }

        #endregion


        #region "Display Message"

        public void DisplayMessage(ref Panel pPanel, string sText)
        {

            Label lblText = new Label();
            lblText.Text = sText;
            lblText.CssClass = "Red";
            lblText.ForeColor = System.Drawing.Color.Red;
            lblText.Font.Size = 7;
            lblText.Font.Bold = true;
            pPanel.Controls.Add(lblText);

        }

        #endregion

        #region "Destructor"

        ~CDataAccess()
        {
            if (this.isBeginTransaction)
            {
                throw new Exception("Begin transaction without Rollback or Commit. Please check your code");
            }
        }

        #endregion


        #region "Command Related"

        public SqlCommand GetCommand(string StrTSQL)
        {
            SqlCommand command2;
            try
            {
                SqlCommand command1 = new SqlCommand();
                this.OpenConnection();
                command1.Connection = this.connection;
                command1.CommandText = StrTSQL;
                command2 = command1;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return command2;
        }


        public void ExecuteQuery(String strSql)
        {
            this.OpenConnection();
            SqlCommand tmpCommand = new SqlCommand(strSql, this.connection);
            tmpCommand.ExecuteNonQuery();
            this.CloseConnection();

        }

        private void PrepareCommand(SqlCommand command, SqlTransaction transaction, string commandText, SqlParameter[] commandParameters)
        {
            //this.PrepareCommand(command1, null, spName, commandParameters);
            command.Connection = this.connection;
            command.CommandText = commandText;
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandType = CommandType.StoredProcedure;
            if (commandParameters != null)
            {
                this.AttachParameters(command, commandParameters);
            }
        }

        #endregion


        #region "GetDataAdapter"

        public SqlDataAdapter GetDataAdapter(string StrSQL)
        {
            SqlDataAdapter adapter2;
            try
            {
                SqlDataAdapter adapter1 = new SqlDataAdapter(StrSQL, this.connectionString);
                adapter2 = adapter1;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return adapter2;
        }

        #endregion


        #region "GetDataReader"

        public SqlDataReader GetDataReader(string StrSQL)
        {
            SqlDataReader reader2;
            try
            {
                SqlDataReader reader1 = null;
                reader1 = this.GetCommand(StrSQL).ExecuteReader(CommandBehavior.Default);
                reader2 = reader1;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return reader2;
        }

        public SqlDataReader GetDataReaderSP(string spName, params SqlParameter[] commandParameters)
        {
            SqlCommand command1 = new SqlCommand();
            this.PrepareCommand(command1, null, spName, commandParameters);
            SqlDataAdapter adapter1 = new SqlDataAdapter(command1);
            SqlDataReader reader1 = command1.ExecuteReader(CommandBehavior.Default);
            command1.Parameters.Clear();
            return reader1;
        }

        public SqlDataReader GetDataReaderSP(string spName, params object[] parameterValues)
        {
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                this.errorCode = CDataAccess.DataAccessErrors.Successful;
                SqlParameter[] parameterArray1 = CDataAccess.SqlParameterCache.GetSpParameterSet(this.connection, spName, false);
                this.AssignParameterValues(parameterArray1, parameterValues);
                return this.GetDataReaderSP(spName, parameterArray1);
            }
            return this.GetDataReaderSP(spName, null);
        }

        #endregion


        #region "GetDataSet"

        public DataSet GetDataSet(string StrSQL, string DateSetName)
        {
            DataSet set2;
            try
            {
                DataSet set1 = new DataSet();
                this.OpenConnection();
                new SqlDataAdapter(StrSQL, this.connectionString).Fill(set1, DateSetName);
                this.CloseConnection();
                set2 = set1;
            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return set2;
        }


        public DataSet GetDataSet(string strSQL, string strTable, ref SqlDataAdapter m_DataAdapter, ref DataTable m_DataTable, ref DataView m_DataView)
        {
            DataSet dsDataSet2 = null;
            try
            {
                this.OpenConnection();
                DataSet Set1 = new DataSet();
                SqlCommand cmCommand;
                cmCommand = new SqlCommand(strSQL, connection);
                daDataAdapter = new SqlDataAdapter(cmCommand);
                SqlCommandBuilder cbCommandBuilder = new SqlCommandBuilder(daDataAdapter);

                //Build the Data Operation SQL if flag is True 
                cbCommandBuilder.GetUpdateCommand();

                DataSet dsDataSet = new DataSet();

                m_DataAdapter.Fill(dsDataSet, strTable);
                dsDataSet2.Merge(dsDataSet);
                dtDataTable = dsDataSet.Tables[0];
                dvDataView = dtDataTable.DefaultView;
                dvDataView.AllowNew = false;
                this.CloseConnection();

            }
            catch (SqlException exception1)
            {
                throw new Exception(exception1.Message);
            }
            catch (Exception exception2)
            {
                throw new Exception(exception2.Message);
            }
            return dsDataSet2;
        }


        public DataSet GetDataSet(string strSQL, string strTable, ref SqlDataAdapter m_DataAdapter, ref DataSet m_DataSet)
        {
            this.OpenConnection();
            DataSet tmpDataSet = new DataSet();
            SqlCommand m_Command;
            m_Command = new SqlCommand(strSQL, this.connection);
            m_DataAdapter = new SqlDataAdapter(m_Command);
            SqlCommandBuilder m_CommandBuilder = new SqlCommandBuilder(m_DataAdapter);
            cbCommandBuilder.GetUpdateCommand();
            m_DataAdapter.Fill(m_DataSet, strTable);
            m_DataSet.Merge(tmpDataSet);
            this.CloseConnection();
            return m_DataSet;
        }


        public DataSet GetDataSet(string strSQL, string strTable, ref DataSet m_DataSet)
        {
            this.OpenConnection();
            SqlDataAdapter m_SQL_DataAdapter;
            SqlCommand m_SQL_Command;
            DataSet tmpDataSet = new DataSet();
            m_SQL_Command = new SqlCommand(strSQL, this.connection);
            m_SQL_DataAdapter = new SqlDataAdapter(m_SQL_Command);
            m_SQL_DataAdapter.Fill(tmpDataSet, strTable);
            m_DataSet.Merge(tmpDataSet);
            this.CloseConnection();
            return m_DataSet;
        }


        public DataSet GetDataSet(string strSQL, string strTable, ref DataSet m_DataSet, int BuildCommand)
        {
            this.OpenConnection();
            DataSet tmpDataSet = new DataSet();
            SqlCommand cmCommand;
            cmCommand = new SqlCommand(strSQL, this.connection);
            daDataAdapter = new SqlDataAdapter(cmCommand);
            cbCommandBuilder = new SqlCommandBuilder(daDataAdapter);
            cbCommandBuilder.GetUpdateCommand();
            daDataAdapter.Fill(m_DataSet, strTable);
            m_DataSet.Merge(tmpDataSet);
            this.CloseConnection();
            return m_DataSet;
        }

        #endregion


        #region "GetDataTable"

        public DataTable GetDataTable(string strSQL)
        {

            this.OpenConnection();
            SqlCommand m_Command;
            m_Command = new SqlCommand(strSQL, this.connection);
            SqlDataAdapter tmpDataAdapter;
            tmpDataAdapter = new SqlDataAdapter(m_Command);
            DataTable tmpDataTable = new DataTable();
            tmpDataAdapter.Fill(tmpDataTable);
            this.CloseConnection();
            return tmpDataTable;

        }

        #endregion


        #region "FillCombo"

        public void FillCombo(ref DropDownList Combo, DataTable dtTable)
        {
                DataView DataView = dtTable.DefaultView;
                Combo.DataSource = DataView;
                Combo.DataTextField = dtTable.Columns[1].ColumnName;
                Combo.DataValueField = dtTable.Columns[0].ColumnName;
                Combo.DataBind();
  
        }

        public void FillCombo(ref DropDownList Combo, string strSQL, bool AddAllOption, bool None)
        {
            DataTable dtTable = GetDataTable(strSQL);
            if ((None))
            {
                DataRow drDataRow;
                drDataRow = dtTable.NewRow();
                drDataRow[0] = -1;
                drDataRow[1] = "";
                dtTable.Rows.Add(drDataRow);
            }
            if ((AddAllOption))
            {
                DataRow drDataRow;
                drDataRow = dtTable.NewRow();
                drDataRow[0] = 0;
                drDataRow[1] = "_ALL_";
                dtTable.Rows.Add(drDataRow);
            }
            {
                DataView DataView = dtTable.DefaultView;
                DataView.Sort = dtTable.Columns[1].ColumnName;
                Combo.DataSource = DataView;
                Combo.DataTextField = dtTable.Columns[1].ColumnName;
                Combo.DataValueField = dtTable.Columns[0].ColumnName;
                Combo.DataBind();
            }

        }

        public void FillCombo(ref DropDownList Combo, DataTable dtTable, bool AddAllOption, bool None)
        {
            //       DataTable dtTable = GetDataTable(strSQL);
            if ((None))
            {
                DataRow drDataRow;
                drDataRow = dtTable.NewRow();
                drDataRow[0] = -1;
                drDataRow[1] = "";
                dtTable.Rows.Add(drDataRow);
            }
            if ((AddAllOption))
            {
                DataRow drDataRow;
                drDataRow = dtTable.NewRow();
                drDataRow[0] = 0;
                drDataRow[1] = "_ALL_";
                dtTable.Rows.Add(drDataRow);
            }
            {
                DataView DataView = dtTable.DefaultView;
                DataView.Sort = dtTable.Columns[1].ColumnName;
                Combo.DataSource = DataView;
                Combo.DataTextField = dtTable.Columns[1].ColumnName;
                Combo.DataValueField = dtTable.Columns[0].ColumnName;
                Combo.DataBind();
            }

        }


        #endregion


        #region "SqlParameterCache Class"

        public sealed class SqlParameterCache
        {
            // Methods
            static SqlParameterCache()
            {
                CDataAccess.SqlParameterCache.paramCache = Hashtable.Synchronized(new Hashtable());
            }

            private SqlParameterCache()
            {
            }

            private static void CacheParameterSet(string connectionString, string spName, params SqlParameter[] commandParameters)
            {
                string text1 = connectionString + ":" + spName;
                CDataAccess.SqlParameterCache.paramCache[text1] = commandParameters;
            }

            private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
            {
                SqlParameter[] parameterArray1 = new SqlParameter[originalParameters.Length];
                int num1 = 0;
                int num2 = originalParameters.Length;
                while (num1 < num2)
                {
                    parameterArray1[num1] = (SqlParameter)((ICloneable)originalParameters[num1]).Clone();
                    num1++;
                }
                return parameterArray1;
            }

            private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
            {
                using (SqlCommand command1 = new SqlCommand(spName, connection))
                {
                    command1.CommandType = CommandType.StoredProcedure;
                    SqlCommandBuilder.DeriveParameters(command1);
                    if (!includeReturnValueParameter)
                    {
                        command1.Parameters.RemoveAt(0);
                    }
                    SqlParameter[] parameterArray1 = new SqlParameter[command1.Parameters.Count];
                    command1.Parameters.CopyTo(parameterArray1, 0);
                    return parameterArray1;
                }
            }

            private static SqlParameter[] GetCachedParameterSet(string connectionString, string spName)
            {
                string text1 = connectionString + ":" + spName;
                SqlParameter[] parameterArray1 = (SqlParameter[])CDataAccess.SqlParameterCache.paramCache[text1];
                if (parameterArray1 == null)
                {
                    return null;
                }
                return CDataAccess.SqlParameterCache.CloneParameters(parameterArray1);
            }

            public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
            {
                return CDataAccess.SqlParameterCache.GetSpParameterSet(connection, spName, false);
            }

            public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
            {
                string text1 = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter" : "");
                SqlParameter[] parameterArray1 = (SqlParameter[])CDataAccess.SqlParameterCache.paramCache[text1];
                if (parameterArray1 == null)
                {
                    object obj1;
                    CDataAccess.SqlParameterCache.paramCache[text1] = obj1 = CDataAccess.SqlParameterCache.DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                    parameterArray1 = (SqlParameter[])obj1;
                }
                return CDataAccess.SqlParameterCache.CloneParameters(parameterArray1);
            }

            // Fields
            private static Hashtable paramCache;
        }

        #endregion




        //Open file into a filestream and 
        //read data in a byte array.
        public byte[] ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open,
                                                    FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to 
            //supply number of bytes to read from file.
            //In this case we want to read entire file. 
            //So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);
            return data;
        }






    }

    #endregion

    public class prasun
    {
        string[] Fig = new string[28];
        string[] Words = new string[28];
        public prasun()
        {
            Fig[0] = "";

            Words[1] = "One";
            Words[2] = "Two";
            Words[3] = "Three";
            Words[4] = "Four";
            Words[5] = "Five";
            Words[6] = "Six";
            Words[7] = "Seven";
            Words[8] = "Eight";
            Words[9] = "Nine";
            Words[10] = "Ten";
            Words[11] = "Eleven";
            Words[12] = "Twelve";
            Words[13] = "Thirteen";
            Words[14] = "Forteen";
            Words[15] = "Fifteen";
            Words[16] = "Sixteen";
            Words[17] = "Seventeen";
            Words[18] = "Eighteen";
            Words[19] = "Nineteen";
            Words[20] = "Twenty";
            Words[21] = "Thirty";
            Words[22] = "Forty";
            Words[23] = "Fifty";
            Words[24] = "Sixty";
            Words[25] = "Seventy";
            Words[26] = "Eighty";
            Words[27] = "Ninety";
            for (int step = 1; step < 21; step++)
                Fig[step] = step.ToString();

            Fig[21] = "30";
            Fig[22] = "40";
            Fig[23] = "50";
            Fig[24] = "60";
            Fig[25] = "70";
            Fig[26] = "80";
            Fig[27] = "90";


            //
            // TODO: Add constructor logic here
            //

        }

        private string conv(int pval)
        {
            string SNAME = "";
            string ii1;
            string jj1;
            int ctr = 0;
            ii1 = pval.ToString();
            ctr = AScan(ii1);
            if (ctr != -1)
                SNAME = Words[ctr];
            else
            {
                jj1 = ii1.Substring(0, 1) + "0";
                ii1 = ii1.Substring(1, 1);
                ctr = AScan(jj1);
                if (ctr != -1) SNAME = Words[ctr];
                ctr = AScan(ii1);
                if (ctr != -1) SNAME = SNAME + " " + Words[ctr];
            }
            return SNAME;


        }

        private int AScan(string abc)
        {
            for (int ctr = 0; ctr < 28; ctr++)
            {
                if (Fig[ctr] == abc)
                    return ctr;
            }
            return -1;
        }




        public string num2word(double amnt)
        {
            if (amnt == 0) return "Zero";
            if (amnt > 9999999999999.99) return "";
            string cname = "";
            int theval = 0;
            string netv = "";
            string wrd = "";
            int step = 0;
            int r_time = 0;
            int t_time = 0;
            string p_nval = "";
            string p_fval = "";
            string p_rval = "";
            double netvalu = 0;
            string actval = "";
            p_nval = string.Format("{0:00.00}", amnt);
            p_nval = p_nval.PadLeft(20, '0');
            p_rval = p_nval.Substring(8, 12);
            p_fval = p_nval.Substring(0, 8);
            if (Convert.ToDouble(p_fval) == 0)
                t_time = 1;
            else
                t_time = 2;
            r_time = 1;


            while (r_time <= t_time)
            {
                if (r_time == 1)
                    netvalu = Convert.ToDouble(p_rval);
                else
                    netvalu = Convert.ToDouble(p_fval);

                step = 1;
                while (step <= 7)
                {
                    if (step == 1)
                        netv = string.Format("{0:00.00}", netvalu).PadLeft(12, '0');
                    if (step == 2)
                    {
                        theval = Convert.ToInt32(netv.Substring(0, 2));
                        cname = "";
                        if (theval != 0)
                            cname = conv(theval) + " Crore ";
                        actval = cname;
                    }
                    if (step == 3)
                    {
                        theval = Convert.ToInt32(netv.Substring(2, 2));
                        if (theval != 0)
                            actval += conv(theval) + " Lac ";
                    }
                    if (step == 4)
                    {
                        theval = Convert.ToInt32(netv.Substring(4, 2));
                        if (theval != 0)
                            actval += conv(theval) + " Thousand ";
                    }
                    if (step == 5)
                    {
                        theval = Convert.ToInt32(netv.Substring(6, 1));
                        if (theval != 0)
                            actval += conv(theval) + " Hundred ";
                    }
                    if (step == 6)
                    {
                        theval = Convert.ToInt32(netv.Substring(7, 2));
                        if (theval != 0)
                            actval += conv(theval);
                    }
                    if (step == 7)
                    {
                        theval = Convert.ToInt32(netv.Substring(10, 2));
                        if (theval != 0)
                            actval += " and Paise " + conv(theval);
                    }
                    step++;
                }
                if (cname == "" && r_time == 1 && Convert.ToDouble(p_fval) != 0)
                {
                    wrd = " Crore " + actval;
                    actval = "";
                }
                wrd = actval + wrd;
                r_time++;
            }
            return wrd;
            //wrd = wrd + " Only"
            //If Left(wrd, 3) = "and" Then wrd = Mid(wrd, 4)
            //Num2Word = wrd
            //End Function
        }
    }


}


