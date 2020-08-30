using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;

namespace api.Controllers
{
    public class SqlController : ApiController
    {
        /// <summary>
        /// 获取数据库名称
        /// </summary>
        /// <returns></returns>
        [HttpGet, JsonDateTimeFormat]
        public AjaxResult getDatabase(string uid = "", string pwd = "", string ds = "")
        {
            string _connectionString = string.Format(@"Data Source={0};Initial Catalog=master;Persist Security Info=True;User ID={1};Password={2};Pooling=true;Min Pool Size=1;Max Pool Size=500;MultipleActiveResultSets=true"
                    , string.IsNullOrEmpty(ds) ? "" : ds
                    , string.IsNullOrEmpty(uid) ? "" : uid
                    , string.IsNullOrEmpty(pwd) ? "" : pwd
                );
            SqlServerDAL dal = string.IsNullOrEmpty(ds) ? new SqlServerDAL(): new SqlServerDAL(_connectionString);

            return new AjaxResult
            {
                statusCode = 200,
                data = dal.ExecuteDataTable("SELECT name,filename FROM SYSDATABASES ORDER BY name", null,CommandType.Text)
            };
        }
        /// <summary>
        /// 获取数据库表名称
        /// </summary>
        /// <returns></returns>
        [HttpGet, JsonDateTimeFormat]
        public AjaxResult getTable(string db="",string uid = "", string pwd = "", string ds = "")
        {
            string _connectionString = string.Format(@"Data Source={0};Initial Catalog={3};Persist Security Info=True;User ID={1};Password={2};Pooling=true;Min Pool Size=1;Max Pool Size=500;MultipleActiveResultSets=true"
                    , string.IsNullOrEmpty(ds) ? "" : ds
                    , string.IsNullOrEmpty(uid) ? "" : uid
                    , string.IsNullOrEmpty(pwd) ? "" : pwd
                    , string.IsNullOrEmpty(db) ? "" : db
                );
            SqlServerDAL dal = string.IsNullOrEmpty(ds) ? new SqlServerDAL() : new SqlServerDAL(_connectionString);

            return new AjaxResult
            {
                statusCode = 200,
                data = dal.ExecuteDataTable("select name,crdate from dbo.sysobjects where xtype='U ' AND [status] >= 0", null, CommandType.Text)
            };
        }
        /// <summary>
        /// 获取数据库表字段名称
        /// </summary>
        /// <returns></returns>
        [HttpGet, JsonDateTimeFormat]
        public AjaxResult getColumns(string table, string db = "", string uid = "", string pwd = "", string ds = "")
        {
            string _connectionString = string.Format(@"Data Source={0};Initial Catalog={3};Persist Security Info=True;User ID={1};Password={2};Pooling=true;Min Pool Size=1;Max Pool Size=500;MultipleActiveResultSets=true"
                    , string.IsNullOrEmpty(ds) ? "" : ds
                    , string.IsNullOrEmpty(uid) ? "" : uid
                    , string.IsNullOrEmpty(pwd) ? "" : pwd
                    , string.IsNullOrEmpty(db) ? "" : db
                );
            SqlServerDAL dal = string.IsNullOrEmpty(ds) ? new SqlServerDAL() : new SqlServerDAL(_connectionString);
            string sql =string.Format( @"
select obj.name table_name,col.name column_name
,t.name data_type,col.length data_length,col.isnullable,ep.[value] column_description
,isnull(pkIndex.is_primary_key,cast(0 as bit)) is_primary_key
from syscolumns col
inner join sysobjects obj on col.id = obj.id AND obj.xtype = 'U' AND obj.status >= 0  
left join systypes t on col.xusertype = t.xusertype 
left join sys.extended_properties ep on  col.id = ep.major_id AND col.colid = ep.minor_id AND ep.name = 'MS_Description' 
left join sys.index_columns colIndex on colIndex.object_id=col.id and colIndex.column_id=col.colid
left join sys.indexes pkIndex on pkIndex.object_id = colIndex.object_id and pkIndex.index_id = colIndex.index_id
where obj.name='{0}'
", string.IsNullOrEmpty(table) ? "" : table);

            return new AjaxResult
            {
                statusCode = 200,
                data = dal.ExecuteDataTable(sql, null, CommandType.Text)
            };
        }












        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet,JsonDateTimeFormat]
        public AjaxResult test(int i = 10) {
            return new AjaxResult
            {
                statusCode = 200,
                data = 2 *1.0 / i
            };
        }

        [HttpGet, JsonDateTimeFormat]
        public AjaxResult test2()
        {
            AuthInfo authInfo = new AuthInfo
            {
                IsAdmin = true,
                Roles = new List<string> { "admin", "owner" },
                UserName = "admin"
            };

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            string encryptToken = encoder.Encode(authInfo, "123456");


            
            IJwtDecoder decoder = new JwtDecoder(serializer, urlEncoder);
            string result = decoder.Decode(encryptToken);


            string token = JwtHelper.CreateJWT(authInfo, "123456");

            AuthInfo tokenAuthInfo = JwtHelper.DecodeJWT<AuthInfo>(token, "123456");





            return new AjaxResult
            {
                statusCode = 200,
                data = new {
                    authInfo= authInfo,
                    encryptToken= encryptToken,
                    result= JsonHelper.Deserialize<AuthInfo>(result),
                    token= token,
                    tokenAuthInfo= tokenAuthInfo
                }
            };
        }





        /// <summary>
        /// 表示jwt的payload
        /// </summary>
        public class AuthInfo
        {
            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName;
            /// <summary>
            /// 角色列表，可以用于记录该用户的角色,相当于claims的概念(如不清楚什么事claim，请google一下"基于声明的权限控制")
            /// </summary>
            public List<string> Roles { get; set; }
            /// <summary>
            /// 是否是管理员
            /// </summary>
            public bool IsAdmin { get; set; }


        }


























    }
}
