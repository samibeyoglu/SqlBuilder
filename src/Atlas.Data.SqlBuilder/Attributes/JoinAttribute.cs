using System;
using Atlas.Data.SqlBuilder.Enums;

namespace Atlas.Data.SqlBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JoinAttribute : Attribute
    {
        public JoinType JoinType { get; set; }
        public string ForeignSchema { get; set; }
        public string ForeignTableName { get; set; }
        public string Alias { get; set; }
        public string ColumnName { get; set; }
        public string ForeignColumnName { get; set; }
    }
}
