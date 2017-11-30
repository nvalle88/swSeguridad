using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.log.guardar.Enumeradores;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using bd.swseguridad.datos;

namespace bd.swseguridad.web.Temporizador
{
    public class Temporizador
    {
        private static SwSeguridadDbContext db;
        public static Timer TimerToken { get; set; }
        public static double TiempoVidaToken { get; set; }

        public static void InicializarTemporizador(Timer timer, Action accion, TimeSpan tiempoEsperaFuncionCallBack, TimeSpan periodoEsperaFuncionCallBack)
        {
            db = db ?? CreateDbContext();
            TimerToken = new Timer((c) => {
                accion();
            }, accion, tiempoEsperaFuncionCallBack, periodoEsperaFuncionCallBack);
            
        }

        public static async Task ComprobarTokensExternos()
        {
            try
            {
                var tiempoReal = DateTime.Now;
                var tokens=   await db.Adsctoken.ToListAsync();
                
                foreach (var item in tokens)
                {
                   var tiempoToken=item.AdtoFecha;
                   TimeSpan duracion = tiempoReal - tiempoToken;
                    if (duracion.TotalSeconds>=TiempoVidaToken)
                    {
                        db.Adsctoken.Remove(item);
                        await db.SaveChangesAsync();
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }

        private static SwSeguridadDbContext CreateDbContext()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var builder = new DbContextOptionsBuilder<SwSeguridadDbContext>();

            var connectionString = configuration.GetConnectionString("SeguridadConnection");

            builder.UseSqlServer(connectionString);

            builder.ConfigureWarnings(w =>
                w.Throw(RelationalEventId.QueryClientEvaluationWarning));

            return new SwSeguridadDbContext(builder.Options);
        }

        public static void InicializarTemporizadorTokenExterno(TimeSpan inicioCiclo,TimeSpan duracionToken,TimeSpan intervaloCiclo)
        {
            TiempoVidaToken = duracionToken.TotalSeconds;
            InicializarTemporizador(TimerToken, async () => {
                await ComprobarTokensExternos();
            },  inicioCiclo, intervaloCiclo);
        }
    }
}
