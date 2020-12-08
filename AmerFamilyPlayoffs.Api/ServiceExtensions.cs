namespace AmerFamilyPlayoffs.Api
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public static class ServiceExtensions
    {
        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
#if DEBUG
            var connectionString = config["ConnectionStrings:DevDBConnectionString"];
#else
            var connectionString = config["ConnectionStrings:TestDBConnectionString"];
#endif
            services.AddDbContext<AmerFamilyPlayoffContext>(o => o.UseLazyLoadingProxies().UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        }
    }
}
