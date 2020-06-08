using FluentMigrator;
using Nop.Data.Migrations;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Data
{
    [SkipMigrationOnUpdate]
    [NopMigration("2020/06/02 09:30:17:6455422", "Employees Plugin base schema")]
    public class SchemaMigration : AutoReversingMigration
    {
        protected IMigrationManager _migrationManager;

        public SchemaMigration(IMigrationManager migrationManager)
        {
            _migrationManager = migrationManager;
        }

        public override void Up()
        {
            _migrationManager.BuildTable<Department>(Create);
            _migrationManager.BuildTable<Employee>(Create);
        }
    }
}
