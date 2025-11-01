using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.Employees.Services;
using Nop.Services.Events;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Widgets.Employees.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmployeesService, EmployeesService>();
#if !NOP_47
            services.AddScoped<IConsumer<AdminMenuCreatedEvent>, AdminMenuEventHandler>();
#endif
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 1;
    }
}
