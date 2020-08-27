using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Ajax返回值格式
/// </summary>
[Serializable]
public class AjaxResult
{
    /// <summary>
    /// 请求状态码  200（成功）500（服务器内部错误）
    /// </summary>
    public int statusCode = 0;
    /// <summary>
    /// 消息
    /// </summary>
    public string message;
    /// <summary>
    /// 数据
    /// </summary>
    public object data;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AjaxResult()
    {
        statusCode = 0;
        message = string.Empty;
        data = null;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="success">请求状态码  200（成功）500（服务器内部错误）</param>
    /// <param name="message">消息</param>
    /// <param name="data">数据</param>
    public AjaxResult(int statusCode, string message = "", object data = null)
    {
        this.statusCode = statusCode;
        this.message = message;
        this.data = data;
    }
}
