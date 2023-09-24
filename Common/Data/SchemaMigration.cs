using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.Employees.Domain;
#if NOP_45
using Nop.Data.Extensions;
#endif

namespace Nop.Plugin.Widgets.Employees.Data
{
#if !NOP_45
    [SkipMigrationOnUpdate]
#endif
    [NopMigration("2020/06/02 09:30:17:6455422", "Employees Plugin base schema")]
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
            Create.TableFor<Department>();
            Create.TableFor<Employee>();
#else
            _migrationManager.BuildTable<Department>(Create);
            _migrationManager.BuildTable<Employee>(Create);
#endif
        }
    }
}
