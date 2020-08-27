using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 
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