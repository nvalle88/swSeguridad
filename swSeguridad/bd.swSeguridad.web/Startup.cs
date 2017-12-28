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
using bd.swseguridad.entidades.LDAP;
using bd.swseguridad.entidades.Interfaces;
using bd.swseguridad.entidades.Servicios;

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

            /// <summary>
            /// Se adiciona una configuración para eliminar los posibles lasos del Json. 
            /// </summary>
            services.AddMvc().AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            /// <summary>
            /// Se obtiene la configuración del ldap del appsetting.json 
            /// y se adiciona el servicio para ser utilizado en la inyección de dependencia...
            /// </summary>
            services.Configure<LdapConfig>(Configuration.GetSection("ldap"));
            services.AddScoped<IAuthenticationService, LdapAuthenticationService>();

            /// <summary>
            /// se adiciona el contexto de datos y se lee la configuracion de la base de datos del appsetting.json
            /// </summary>
            services.AddDbContext<SwSeguridadDbContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("SeguridadConnection")));

            /// <summary>
            /// Se lee el fichero appsetting.json según las etiquetas expuestas en este.
            /// Ejemplo:TiempoVidaTokenHoras Horas que tendra de vida la token externo.
            /// TiempoVidaTokenMinutos Minutos que tendra de vida la token externo
            ///  TiempoVidaTokenSegundos Segundos que tendra de vida la token externo.
            ///  Con estas tres variables mencionadas se conforma el tiempo de vida del Token externo 
            ///   que serán consumidos por terceros
            /// </summary>
            var tiempoVidaTokenHoras = Configuration.GetSection("TiempoVidaTokenHoras").Value;
            var tiempoVidaTokenMinutos = Configuration.GetSection("TiempoVidaTokenMinutos").Value;
            var tiempoVidaTokenSegundos = Configuration.GetSection("TiempoVidaTokenSegundos").Value;


            /// <summary>
            /// Se lee el fichero appsetting.json según las etiquetas expuestas en este.
            /// Ejemplo:IntervaloTemporizadorHoras Horas que tendra de vida la token.
            /// IntervaloTemporizadorMinutos Minutos que tendra de vida la token
            ///  IntervaloTemporizadorSegundos Segundos que tendra de vida la token.
            ///  Con estas tres variables mencionadas se conforma cada que tiempo se realizará 
            ///  el ciclo para invalidar los Token externos que serán consumidos por terceros
            /// </summary>
            var IntervaloCicloHoras = Configuration.GetSection("IntervaloTemporizadorHoras").Value;
            var IntervaloCicloMinutos = Configuration.GetSection("IntervaloTemporizadorMinutos").Value;
            var IntervaloCicloSegundos = Configuration.GetSection("IntervaloTemporizadorSegundos").Value;

            /// <summary>
            /// Se lee el fichero appsetting.json según las etiquetas expuestas en este.
            /// Ejemplo:inicioCicloHoras Horas que comensará a ejecutarse una vez iniciada la aplicación.
            /// inicioCicloMinutos Minutos que comensará a ejecutarse una vez iniciada la aplicación.
            ///  inicioCicloSegundos Segundos que comensará a ejecutarse una vez iniciada la aplicación.
            ///  Con estas tres variables mencionadas se conforma el tiempo que se comenzará a ejecutar 
            ///  el ciclo para  invalidar los Token externos que serán consumidos por terceros
            /// </summary>
            var inicioCicloHoras = Configuration.GetSection("inicioCicloHoras").Value;
            var inicioCicloMinutos = Configuration.GetSection("inicioCicloMinutos").Value;
            var inicioCicloSegundos = Configuration.GetSection("inicioCicloSegundos").Value;


            /// <summary>
            /// Se inicializa el temporizador invalidar los Token externos que serán consumidos por terceros
            /// </summary>
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
