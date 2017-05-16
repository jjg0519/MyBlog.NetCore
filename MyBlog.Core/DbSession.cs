using System;
using System.Collections.Generic;
using System.Text;
using MySqlSugar;

namespace MyBlog.Core
{
    public abstract class DbSession : IDbSession
    {
        /// <summary>
        /// 数据库会话对象
        /// </summary>
        protected SqlSugarClient _client;

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        protected string _connStr;

        public DbSession(string connStr)
        {
            this._connStr = connStr;
        }

        public virtual SqlSugarClient GetSession()
        {
            if (null == this._client)
                this._client = new SqlSugarClient(this._connStr);
            return this._client;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (null != this._client)
                this._client.Dispose();
        }

    }
}
