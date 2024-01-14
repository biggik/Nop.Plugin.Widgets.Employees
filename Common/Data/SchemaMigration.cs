using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.Employees.Domain;
using FluentMigrator.Infrastructure;

#if NOP_45
using Nop.Data.Extensions;
#endif

namespace Nop.Plugin.Widgets.Employees.Data
{
#if !NOP_45
    [SkipMigrationOnUpdate]
#endif
    [NopMigration("2020/06/02 09:30:17:6455422", "Employees Plugin base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
#if !NOP_45
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }
#endif

        public override void Up()
        {
#if NOP_45
            if (!_departmentExists)
            {
                Create.TableFor<Department>();
            }
            if (!_employeeExists)
            {
                Create.TableFor<Employee>();
            }
#else
            _migrationManager.BuildTable<Department>(Create);
            _migrationManager.BuildTable<Employee>(Create);
#endif
        }

#if NOP_45
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
#endif
    }
}
