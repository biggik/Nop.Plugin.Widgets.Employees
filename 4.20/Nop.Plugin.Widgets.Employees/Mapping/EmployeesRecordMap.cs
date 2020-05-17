using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Mapping
{
    public partial class EmployeesRecordMap : NopEntityTypeConfiguration<Employee>
    {
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("StatusEmployee");
            builder.HasKey(x => x.Id);
            base.Configure(builder);
        }
    }
}