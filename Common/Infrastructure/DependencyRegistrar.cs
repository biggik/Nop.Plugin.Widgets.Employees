using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.Employees.Data;
using Nop.Plugin.Widgets.Employees.Domain;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Web.Framework.Infrastructure.Extensions;

namespace Nop.Plugin.Widgets.Employees.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<EmployeesService>().As<IEmployeesService>().InstancePerLifetimeScope();

            //data context
            builder.RegisterPluginDataContext<EmployeesObjectContext>("nop_object_context_employees_zip");
            
            //override required repository with our custom context
            builder.RegisterType<EfRepository<Employee>>()
                .As<IRepository<Employee>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_employees_zip"))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<Department>>()
                .As<IRepository<Department>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_employees_zip"))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 1; }
        }
    }
}
