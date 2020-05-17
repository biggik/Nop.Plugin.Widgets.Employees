using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Data
{
    public partial class EmployeesRecordMap : NopEntityTypeConfiguration<EmployeesRecord>
    {
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<EmployeesRecord> builder)
        {
            builder.ToTable("StatusEmployee");
            builder.HasKey(x => x.Id);
            base.Configure(builder);
        }
    }
}