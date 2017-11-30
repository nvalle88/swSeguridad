using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using bd.swseguridad.datos;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace bd.swseguridad.web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.

            services.AddMvc();
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.AddDbContext<SwSeguridadDbContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("SeguridadConnection")));

            var tiempoVidaTokenHoras = Configuration.GetSection("TiempoVidaTokenHoras").Value;
            var tiempoVidaTokenMinutos = Configuration.GetSection("TiempoVidaTokenMinutos").Value;
            var tiempoVidaTokenSegundos = Configuration.GetSection("TiempoVidaTokenSegundos").Value;


            var IntervaloCicloHoras = Configuration.GetSection("IntervaloTemporizadorHoras").Value;
            var IntervaloCicloMinutos = Configuration.GetSection("IntervaloTemporizadorMinutos").Value;
            var IntervaloCicloSegundos = Configuration.GetSection("IntervaloTemporizadorSegundos").Value;

            var inicioCicloHoras = Configuration.GetSection("inicioCicloHoras").Value;
            var inicioCicloMinutos = Configuration.GetSection("inicioCicloMinutos").Value;
            var inicioCicloSegundos = Configuration.GetSection("inicioCicloSegundos").Value;


            Temporizador.Temporizador.InicializarTemporizadorTokenExterno
                (new TimeSpan(Convert.ToInt32(inicioCicloHoras), Convert.ToInt32(inicioCicloMinutos), Convert.ToInt32(inicioCicloSegundos))
                , new TimeSpan(Convert.ToInt32(tiempoVidaTokenHoras), Convert.ToInt32(tiempoVidaTokenMinutos), Convert.ToInt32(tiempoVidaTokenSegundos))
                , new TimeSpan(Convert.ToInt32(IntervaloCicloHoras), Convert.ToInt32(IntervaloCicloMinutos), Convert.ToInt32(IntervaloCicloSegundos)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();




            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();

                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
                {
                    //serviceScope.ServiceProvider.GetService<SwSeguridadDbContext>()
                    //         .Database.Migrate();

                    //serviceScope.ServiceProvider.GetService<SwCompartidoDbContext>().EnsureSeedData();
                }

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
