using Nop.Data.Mapping;
using Nop.Plugin.Widgets.Employees.Domain;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.Employees.Mapping
{
    public partial class TableNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(Department), "StatusDepartment" },
            { typeof(Employee), "StatusEmployee" }
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
    }
}
