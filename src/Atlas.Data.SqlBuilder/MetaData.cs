using Atlas.Data.SqlBuilder.Enums;

namespace Atlas.Data.SqlBuilder
{
    public class TableMetaData
    {
        public string TableName { get; set; }
        public string Schema { get; set; }
        public string Alias { get; set; }
    }

    public class NavigationColumnMetaData : ColumnMetaData
    {
        public string Alias { get; set; }
        public string ForeignSchema { get; set; }
        public string ForeignTableName { get; set; }
        public string ForeignColumnName { get; set; }
        public JoinType JoinType { get; set; }
    }

    public class ColumnMetaData
    {
        public string ColumnName { get; set; }
    }
}
