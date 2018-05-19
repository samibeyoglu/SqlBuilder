using System;

namespace Atlas.Data.SqlBuilder.Tests.Entities
{
    [Table(Schema = "ec",Name = "Customers",Alias = "c")]
    public class CustomerAttr
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
