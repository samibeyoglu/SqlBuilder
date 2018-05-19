using System;

namespace Atlas.Data.SqlBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }
    }
}
