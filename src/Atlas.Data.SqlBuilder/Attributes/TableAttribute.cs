using System;

namespace Atlas.Data.SqlBuilder
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public string Schema { get; set; } = "dbo";

        public string Alias { get; set; }
    }
}
