--快速查看表结构
--获取表名
select name,crdate from dbo.sysobjects where xtype='U ' AND [status] >= 0  
--获取表列名
select obj.name table_name,col.name column_name
,t.name data_type,col.length data_length,col.isnullable,ep.[value] column_description
,isnull(pkIndex.is_primary_key,cast(0 as bit)) is_primary_key

from syscolumns col
inner join sysobjects obj on col.id = obj.id AND obj.xtype = 'U' AND obj.status >= 0  
left join systypes t on col.xusertype = t.xusertype 
left join sys.extended_properties ep on  col.id = ep.major_id AND col.colid = ep.minor_id AND ep.name = 'MS_Description' 
left join sys.index_columns colIndex on colIndex.object_id=col.id and colIndex.column_id=col.colid
left join sys.indexes pkIndex on pkIndex.object_id = colIndex.object_id and pkIndex.index_id = colIndex.index_id

--where obj.name='Files'
