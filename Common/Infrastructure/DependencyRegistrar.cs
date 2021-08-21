#if !NOP_ASYNC
using Autofac;
#else
using Microsoft.Extensions.DependencyInjection;
#endif
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Widgets.Employees.Services;
#if NOP_PRE_4_3
using Autofac.Core;
using Nop.Core.Data;
using Nop.Data;
using Nop.Plugin.Widgets.Employees.Data;
using Nop.Plugin.Widgets.Employees.Domain;
using Nop.Web.Framework.Infrastructure.Extensions;
#endif

namespace Nop.Plugin.Widgets.Employees.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
#if !NOP_ASYNC
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<EmployeesService>().As<IEmployeesService>().InstancePerLifetimeScope();

#if NOP_PRE_4_3
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
#endif
        }
#else
        public void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            services.AddScoped<EmployeesService>();
        }
#endif

        public int Order => 1;
    }
}
