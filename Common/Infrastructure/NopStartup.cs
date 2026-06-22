using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.Employees.Services;

namespace Nop.Plugin.Widgets.Employees.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmployeesService, EmployeesService>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 1;
    }
}
