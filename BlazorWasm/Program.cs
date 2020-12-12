namespace AmerFamilyPlayoffs
{
    using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

#if DEBUG
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("https://localhost:44325/") });
#else
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri("https://stevencodeswright.com/") });
#endif

            await builder.Build().RunAsync();
        }
    }
}
