using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.Employees.Domain;
using FluentMigrator.Infrastructure;
using Nop.Data.Extensions;

namespace Nop.Plugin.Widgets.Employees.Data
{
    [NopMigration("2020/06/02 09:30:17:6455422", "Employees Plugin base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            if (!_departmentExists)
            {
                Create.TableFor<Department>();
            }
            if (!_employeeExists)
            {
                Create.TableFor<Employee>();
            }
        }

        private bool _departmentExists;
        private bool _employeeExists;
        public override void GetUpExpressions(IMigrationContext context)
        {
            // Schema migration is always attempting to create the tables, so override 
            // here to get out of this situation
            _departmentExists = context.QuerySchema.TableExists("dbo", "StatusDepartment");
            _employeeExists = context.QuerySchema.TableExists("dbo", "StatusEmployee");

            base.GetUpExpressions(context);
        }
    }
}
