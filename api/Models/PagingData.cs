using System;
using System.Collections.Generic;
using System.Data;


/// <summary>
/// 
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
    public PagingDictionaryList ToPagingDictionaryList()
    {
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