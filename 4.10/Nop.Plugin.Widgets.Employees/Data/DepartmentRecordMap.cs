using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Data.Mapping;
using Nop.Plugin.Widgets.Employees.Domain;

namespace Nop.Plugin.Widgets.Employees.Data
{
    public partial class DepartmentRecordMap : NopEntityTypeConfiguration<DepartmentRecord>
    {
        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<DepartmentRecord> builder)
        {
            builder.ToTable("StatusDepartment");
            builder.HasKey(x => x.Id);
            base.Configure(builder);
        }
    }
}