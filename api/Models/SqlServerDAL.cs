using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace api
{
    /// <summary>
    /// SqlServer 数据层
    /// </summary>
    public class SqlServerDAL
    {
        #region 字段、属性

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        private string _ConnectionString;
        
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString {
            get { return _ConnectionString; }
        }


        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数。默认获取配置文件web.config的GeneDB数据库连接字符串
        /// </summary>
        public SqlServerDAL() {
            //默认获取配置文件web.config的数据库连接字符串
            _ConnectionString = ConfigurationManager.ConnectionStrings["SqlToolDB"].ConnectionString;
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ConnectionString">数据库连接字符串</param>
        public SqlServerDAL(string ConnectionString)
        {
            _ConnectionString = ConnectionString;
        }

        #endregion

        /// <summary>
        /// 数据库连接
        /// </summary>
        public SqlConnection CreateSqlConnection()
        {
            SqlConnection conn = new SqlConnection(_ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 数据库命令
        /// </summary>
        private SqlCommand CreateSqlCommand()
        {
            SqlCommand _SqlCommand = new SqlCommand();
            _SqlCommand.Connection = CreateSqlConnection();
            _SqlCommand.CommandTimeout = 60;//以秒为单位
            return _SqlCommand;
        }

        
        #region 执行 SqlCommand 的ExecuteNonQuery、ExecuteReader、ExecuteScalar、ExecuteXmlReader方法

        /// <summary>
        /// 执行SQL语句或存储过程，返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="IsUseTransaction">是否使用事务</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql,SqlParameter[] parameters, CommandType cmdType, bool IsUseTransaction)
        {
            SqlCommand _SqlCommand = CreateSqlCommand();
            try
            {
                _SqlCommand.CommandText = sql;
                _SqlCommand.CommandType = cmdType;
                _SqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Length > 0)
                {
                    _SqlCommand.Parameters.AddRange(parameters);
                }

                if (IsUseTransaction)
                {
                    _SqlCommand.Transaction = _SqlCommand.Connection.BeginTransaction();
                    try
                    {
                        int r = _SqlCommand.ExecuteNonQuery();
                        _SqlCommand.Transaction.Commit();
                        return r;
                    }
                    catch (Exception ex)
                    {
                        _SqlCommand.Transaction.Rollback();
                        throw ex;
                    }

                }
                else
                {
                    return _SqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                _SqlCommand.Connection.Close();
                _SqlCommand.Dispose();
            }
        }

        /// <summary>
        /// 执行SQL语句或存储过程，返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cmdType">命令类型</param>
        public int ExecuteNonQuery(string sql, SqlParameter[] parameters, CommandType cmdType)
        {
            return ExecuteNonQuery(sql, parameters, cmdType, false);
        }
        /// <summary>
        /// 执行SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        public int ExecuteNonQuery(string sql, SqlParameter[] parameters = null)
        {
            return ExecuteNonQuery(sql, parameters, CommandType.Text, false);
        }

        /// <summary>
        /// 使用事务，批量执行SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="pars">执行参数对象</param>
        /// <returns></returns>
        public int ExecuteNonQuery(List<SqlParams> pars)
        {
            SqlConnection _Conn = CreateSqlConnection();

            try
            {
                SqlTransaction _Transaction = _Conn.BeginTransaction();
                int res = 0;
                try
                {
                    foreach (SqlParams par in pars)
                    {
                        SqlCommand cmd = new SqlCommand(par.Sql, _Conn, _Transaction);
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        if (par.Parameters != null && par.Parameters.Count > 0)
                        {
                            cmd.Parameters.AddRange(par.Parameters.ToArray());
                        }

                        //执行语句
                        res += cmd.ExecuteNonQuery();

                        cmd.Dispose();

                    }

                    //提交事务
                    _Transaction.Commit();

                }
                catch (Exception exp) {
                    //回滚事务
                    _Transaction.Rollback();
                    throw exp;
                }

                _Transaction.Dispose();

                return res;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _Conn.Close();
                _Conn.Dispose();
            }

        }
        
        /// <summary>
        /// 执行SQL语句或存储过程
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="IsUseTransaction">是否使用事务</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string sql, SqlParameter[] parameters, CommandType cmdType, bool IsUseTransaction)
        {
            SqlCommand _SqlCommand = CreateSqlCommand();
            try
            {
                _SqlCommand.CommandText = sql;
                _SqlCommand.CommandType = cmdType;
                _SqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Length > 0)
                {
                    _SqlCommand.Parameters.AddRange(parameters);
                }

                if (IsUseTransaction)
                {
                    _SqlCommand.Transaction = _SqlCommand.Connection.BeginTransaction();
                    try
                    {
                        SqlDataReader sdr = _SqlCommand.ExecuteReader();
                        _SqlCommand.Transaction.Commit();
                        return sdr;
                    }
                    catch (Exception ex)
                    {
                        _SqlCommand.Transaction.Rollback();
                        throw ex;
                    }

                }
                else
                {
                    return _SqlCommand.ExecuteReader();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                _SqlCommand.Connection.Close();
                _SqlCommand.Dispose();
            }
        }
        
        /// <summary>
        /// 执行SQL语句或存储过程
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数。类型为Dictionary字典（参数名带不带@都可以）或 SqlParameter[] </param>
        /// <param name="cmdType">命令类型</param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string sql, SqlParameter[] parameters, CommandType cmdType)
        {
            return ExecuteReader(sql, parameters, cmdType, false);
        }

        /// <summary>
        /// 执行SQL语句或存储过程，返回查询所返回的结果集中的第一行第一列
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="IsUseTransaction">是否使用事务</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, SqlParameter[] parameters, CommandType cmdType, bool IsUseTransaction)
        {
            SqlCommand _SqlCommand = CreateSqlCommand();
            try
            {
                _SqlCommand.CommandText = sql;
                _SqlCommand.CommandType = cmdType;
                _SqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Length > 0)
                {
                    _SqlCommand.Parameters.AddRange(parameters);
                }

                if (IsUseTransaction)
                {
                    _SqlCommand.Transaction = _SqlCommand.Connection.BeginTransaction();
                    try
                    {
                        object r = _SqlCommand.ExecuteScalar();
                        _SqlCommand.Transaction.Commit();
                        return r;
                    }
                    catch (Exception ex)
                    {
                        _SqlCommand.Transaction.Rollback();
                        throw ex;
                    }

                }
                else
                {
                    return _SqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally {
                _SqlCommand.Connection.Close();
                _SqlCommand.Dispose();
            }
        }
        
        /// <summary>
        /// 执行SQL语句或存储过程，返回查询所返回的结果集中的第一行第一列
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数。类型为Dictionary字典（参数名带不带@都可以）或 SqlParameter[] </param>
        /// <param name="cmdType">命令类型</param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, SqlParameter[] parameters, CommandType cmdType)
        {
            return ExecuteScalar(sql, parameters, cmdType, false);
        }


        #endregion
        
        #region 执行 ExecuteDataTable、ExecuteDataSet


        /// <summary>
        /// 执行SQL语句或存储过程
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cmdType">执行类型</param>
        /// <param name="IsUseTransaction">是否使用事务</param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string sql, SqlParameter[] parameters, CommandType cmdType, bool IsUseTransaction)
        {
            SqlCommand _SqlCommand = CreateSqlCommand();
            try
            {
                _SqlCommand.CommandText = sql;
                _SqlCommand.CommandType = cmdType;
                _SqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Length > 0)
                {
                    _SqlCommand.Parameters.AddRange(parameters);
                }

                using (DataTable dt = new DataTable())
                {
                    SqlDataAdapter sda = new SqlDataAdapter(_SqlCommand);

                    if (IsUseTransaction)
                    {
                        SqlTransaction tran = _SqlCommand.Connection.BeginTransaction();
                        sda.SelectCommand.Transaction = tran;

                        try
                        {
                            sda.Fill(dt);
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tran.Dispose();
                        }
                    }
                    else
                    {
                        sda.Fill(dt);
                    }
                    sda.Dispose();

                    return dt;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                _SqlCommand.Connection.Close();
                _SqlCommand.Dispose();
            }
        }
        
        /// <summary>
        /// 执行SQL语句或存储过程
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cmdType">执行类型</param>
        public DataTable ExecuteDataTable(string sql, SqlParameter[] parameters, CommandType cmdType)
        {
            return ExecuteDataTable(sql, parameters, cmdType, false);
        }

        /// <summary>
        /// 执行SQL语句或存储过程
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cmdType">执行类型</param>
        /// <param name="IsUseTransaction">是否使用事务</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sql, SqlParameter[] parameters, CommandType cmdType, bool IsUseTransaction)
        {
            SqlCommand _SqlCommand = CreateSqlCommand();
            try
            {
                _SqlCommand.CommandText = sql;
                _SqlCommand.CommandType = cmdType;
                _SqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Length > 0)
                {
                    _SqlCommand.Parameters.AddRange(parameters);
                }

                SqlDataAdapter sda = new SqlDataAdapter(_SqlCommand);
                using (DataSet ds = new DataSet())
                {
                    if (IsUseTransaction)
                    {
                        SqlTransaction tran = _SqlCommand.Connection.BeginTransaction();
                        sda.SelectCommand.Transaction = tran;
                        try
                        {
                            sda.Fill(ds);
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tran.Dispose();
                        }
                    }
                    else
                    {
                        sda.Fill(ds);
                    }
                    sda.Dispose();

                    return ds;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                _SqlCommand.Connection.Close();
                _SqlCommand.Dispose();
            }
        }

        /// <summary>
        /// 执行SQL语句或存储过程
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="cmdType">执行类型</param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sql, SqlParameter[] parameters, CommandType cmdType)
        {
            return ExecuteDataSet(sql, parameters, cmdType, false);
        }



        #endregion
        
        #region 分页
        
        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <param name="PageSize">每页大小</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="OrderBy">排序参数。比如：“bar_sn desc,CreateTime asc”、“bar_sn asc”、“bar_sn”。</param>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="parameters">参数（可空）</param>
        /// <param name="IsUseTransaction">是否使用事务</param>
        /// <returns></returns>
        public PagingData ExecutePaging(int PageSize,int pageNumber, string OrderBy,string sql, SqlParameter[] parameters, bool IsUseTransaction)
        {
            SqlCommand _SqlCommand = CreateSqlCommand();
            try
            {
                if (pageNumber < 1)
                {
                    pageNumber = 1;
                }
                if (string.IsNullOrEmpty(OrderBy))
                {
                    throw new Exception("排序参数OrderBy不能为空。");
                }

                int min = PageSize * (pageNumber - 1) + 1;
                int max = PageSize * pageNumber; // PageSize * (pageNumber - 1) + PageSize;

                string _sql = string.Format("select ROW_NUMBER() over ( order by {0} ) RowNumber,* from ( {1} ) {2} ", OrderBy, sql,"G_"+Guid.NewGuid().ToString("N"));

                _sql = string.Format("select count(*) from ({0}) {1}; ", _sql, "G_" + Guid.NewGuid().ToString("N")) /*总数据条数*/
                    + string.Format(@"select {3}.* from( {0} ) {3} where {3}.RowNumber between {1} and {2} ;", _sql, min, max, "G_" + Guid.NewGuid().ToString("N"));

                _SqlCommand.CommandText = _sql;
                _SqlCommand.CommandType = CommandType.Text;
                _SqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Length > 0)
                {
                    _SqlCommand.Parameters.AddRange(parameters);
                }

                using (DataSet ds = new DataSet())
                {
                    SqlDataAdapter sda = new SqlDataAdapter(_SqlCommand);

                    if (IsUseTransaction)
                    {
                        SqlTransaction tran = _SqlCommand.Connection.BeginTransaction();
                        sda.SelectCommand.Transaction = tran;

                        try
                        {
                            sda.Fill(ds);
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tran.Dispose();
                        }
                    }
                    else
                    {
                        sda.Fill(ds);
                    }
                    sda.Dispose();

                    PagingData pd = new PagingData();
                    pd.pagenumber = pageNumber;
                    pd.pagesize = PageSize;
                    pd.total = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                    pd.rows = ds.Tables[1];

                    return pd;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                _SqlCommand.Connection.Close();
                _SqlCommand.Dispose();
            }
        }

        /// <summary>
        /// 执行分页查询
        /// </summary>
        /// <param name="PageSize">每页大小</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="OrderBy">排序参数。比如：“bar_sn desc,CreateTime asc”、“bar_sn asc”、“bar_sn”。</param>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="parameters">参数（可空）</param>
        /// <returns></returns>
        public PagingData ExecutePaging(int PageSize, int pageNumber, string OrderBy, string sql, SqlParameter[] parameters = null) {
            return ExecutePaging(PageSize, pageNumber, OrderBy, sql, parameters, false);
        }

        /// <summary>
        /// 执行分页查询（DataTable分页）
        /// </summary>
        /// <param name="PageSize">每页大小</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="parameters">参数（可空）</param>
        /// <param name="IsUseTransaction">是否使用事务</param>
        /// <returns></returns>
        public PagingData ExecutePageByDataTable(int PageSize, int pageNumber, string sql, SqlParameter[] parameters, bool IsUseTransaction)
        {
            SqlCommand _SqlCommand = CreateSqlCommand();
            try
            {

                _SqlCommand.CommandText = sql;
                _SqlCommand.CommandType = CommandType.Text;
                _SqlCommand.Parameters.Clear();
                if (parameters != null && parameters.Length > 0)
                {
                    _SqlCommand.Parameters.AddRange(parameters);
                }

                using (DataTable dt = new DataTable())
                {
                    SqlDataAdapter sda = new SqlDataAdapter(_SqlCommand);

                    if (IsUseTransaction)
                    {
                        SqlTransaction tran = _SqlCommand.Connection.BeginTransaction();                       
                        sda.SelectCommand.Transaction = tran;

                        try
                        {
                            sda.Fill(dt);
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            tran.Dispose();
                        }
                    }
                    else
                    {
                        sda.Fill(dt);
                    }
                    sda.Dispose();

                    //---分页------------------------------------------------------------
                    PagingData pd = new PagingData();
                    pd.pagenumber = pageNumber;
                    pd.pagesize = PageSize;
                    pd.total = dt.Rows.Count;

                    if (pageNumber < 1)
                    {
                        pageNumber = 1;
                    }
                    /*
                    int minIndex = PageSize * (pageNumber - 1) + 1;
                    int maxIndex = PageSize * pageNumber; 
                    */

                    int minIndex = PageSize * (pageNumber - 1);//从零开始
                    int maxIndex = PageSize * pageNumber - 1;//1210
                    int rowMaxIndex = (pd.total - 1) >= maxIndex ? maxIndex : (pd.total - 1);

                    DataTable pageTable = dt.Clone();

                    for (int i = minIndex; i <= rowMaxIndex; i++)
                    {
                        DataRow pageRow = pageTable.NewRow();
                        DataRow dRow = dt.Rows[i];
                        foreach (DataColumn column in dt.Columns)
                        {
                            pageRow[column.ColumnName] = dRow[column.ColumnName];
                        }

                        //pageRow["RowIndex"] = i + 1;

                        pageTable.Rows.Add(pageRow);
                    }

                    pd.rows = pageTable;

                    return pd;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                _SqlCommand.Connection.Close();
                _SqlCommand.Dispose();
            }
        }


        /// <summary>
        /// 执行分页查询（DataTable分页）
        /// </summary>
        /// <param name="PageSize">每页大小</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="parameters">参数（可空）</param>
        /// <returns></returns>
        public PagingData ExecutePageByDataTable(int PageSize, int pageNumber, string sql, SqlParameter[] parameters = null) {
            return ExecutePageByDataTable(PageSize, pageNumber, sql, parameters, false);
        }


        #endregion

        #region Exists 判断表数据是否存在

        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="TableName">数据表名</param>
        /// <param name="PrimaryKey">主键字段名称</param>
        /// <param name="PrimaryKeyValue">主键字段值</param>
        /// <returns></returns>
        public bool Exists(string TableName,string PrimaryKey, object PrimaryKeyValue) {
            try
            {
                TableName = TableName.Trim();
                PrimaryKey = PrimaryKey.Trim();

                string sql = string.Format("select {1} from {0} where {1}=@{1}", TableName, PrimaryKey);
                DataTable dt = ExecuteDataTable(sql, new SqlParameter[] {
                    new SqlParameter("@"+PrimaryKey,PrimaryKeyValue)
                }, CommandType.Text);

                return dt.Rows.Count > 0;
            }
            catch(Exception exp) {
                throw new Exception("SqlServerDAL类中的Exists方法错误。" + exp.Message);
            }
        }

        /// <summary>
        /// 数据是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="whereStr"></param>
        /// <returns></returns>
        public bool Exists(string TableName, string whereStr)
        {
            try
            {
                TableName = TableName.Trim();
                whereStr = whereStr.Trim();

                string sql = string.Format("select * from {0} where {1}", TableName, whereStr);
                DataTable dt = ExecuteDataTable(sql,null, CommandType.Text);

                return dt.Rows.Count > 0;
            }
            catch (Exception exp)
            {
                throw new Exception("SqlServerDAL类中的Exists方法错误。" + exp.Message);
            }
        }

        #endregion

        #region 其它

        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public SqlParameter CreateSqlParameter(string parameterName, object value)
        {
            if (value == null|| (value is string && string.IsNullOrEmpty(value as string)))
            {
                value = (object)DBNull.Value;
            }
            
            return new SqlParameter(parameterName, value);
        }

        


        #endregion

    }


    #region 分页数据

    /// <summary>
    /// 分页数据
    /// </summary>
    [Serializable]
    public class PagingData
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int pagenumber;
        /// <summary>
        /// 每页大小
        /// </summary>
        public int pagesize;
        /// <summary>
        /// 总数据条数
        /// </summary>
        public int total;
        /// <summary>
        /// 数据
        /// </summary>
        public DataTable rows;

        public PagingData() { }

        /// <summary>
        /// 转换为字典列表
        /// </summary>
        /// <returns></returns>
        public PagingDictionaryList ToPagingDictionaryList() {
            PagingDictionaryList pdl = new PagingDictionaryList();
            pdl.pagenumber = pagenumber;
            pdl.pagesize = pagesize;
            pdl.total = total;

            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();
            foreach (DataRow dr in rows.Rows)
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                foreach (DataColumn dc in rows.Columns)
                {
                    row[dc.ColumnName] = dr[dc.ColumnName];
                }
                table.Add(row);
            }
            pdl.rows = table;

            return pdl;
            
        }


    }

    /// <summary>
    ///  分页字典数据
    /// </summary>
    [Serializable]
    public class PagingDictionaryList
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int pagenumber;
        /// <summary>
        /// 每页大小
        /// </summary>
        public int pagesize;
        /// <summary>
        /// 总数据条数
        /// </summary>
        public int total;
        /// <summary>
        /// 数据
        /// </summary>
        public List<Dictionary<string, object>> rows;


        public PagingDictionaryList() { }
        public PagingDictionaryList(int pagenumber, int pagesize, int total)
        {
            this.pagenumber = pagenumber;
            this.pagesize = pagesize;
            this.total = total;

        }
        public PagingDictionaryList(int pagenumber, int pagesize, int total, List<Dictionary<string, object>> rows)
        {
            this.pagenumber = pagenumber;
            this.pagesize = pagesize;
            this.total = total;
            this.rows = rows;

        }
    }


    #endregion

    /// <summary>
    /// 批量执行SQL语句之执行参数
    /// </summary>
    public class SqlParams {

        /// <summary>
        /// SQL语句
        /// </summary>
        public string Sql;
        /// <summary>
        /// 参数集合
        /// </summary>
        public List<SqlParameter> Parameters;

        /// <summary>
        /// 构造函数
        /// </summary>
        public SqlParams() {
            Sql = string.Empty;
            Parameters = new List<SqlParameter>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Sql">SQL语句</param>
        /// <param name="Parameters">参数</param>
        public SqlParams(string Sql, List<SqlParameter> Parameters)
        {
            this.Sql = Sql;
            this.Parameters = Parameters;
        }

    }


}
