using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;

namespace ShowAssemblyVersion
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.WithAssemblyVersionOnRoot();
                endpoints.MapControllers();
            });
        }
    }

    public static class AssemblyVersionExtension
    {
        public static void WithAssemblyVersionOnRoot(this Microsoft.AspNetCore.Routing.IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/", async context =>
            {
                dynamic result = new ExpandoObject();
                Assembly assembly = Assembly.GetEntryAssembly();
                AssemblyInformationalVersionAttribute versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                string assemblyVersion = versionAttribute.InformationalVersion;
                result.version = assemblyVersion;
                string versionAsText = JsonSerializer.Serialize(result);
                await context.Response.WriteAsync(versionAsText);
            });
        }
    }
}