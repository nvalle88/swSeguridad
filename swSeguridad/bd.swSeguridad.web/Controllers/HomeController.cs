using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using bd.swseguridad.entidades.Negocio;
using bd.log.guardar.Utiles;
using bd.swseguridad.datos;
using bd.swseguridad.entidades.Utils;
using Microsoft.EntityFrameworkCore;

namespace bd.log.web.Controllers
{
    public class HomeController : Controller
    {

        private readonly SwSeguridadDbContext db;

        public HomeController(SwSeguridadDbContext db)
        {
            this.db = db;
        }
        
        public IActionResult Index()

        {
        
            return View();
        }

       
        public IActionResult GetToken()
        {
            Adscpassw adscpassw = new Adscpassw();
            var queryStrings = Request.Query;
            var qsList = new List<string>();
            foreach (var key in queryStrings.Keys)
            {
                qsList.Add(queryStrings[key]);
            }
            adscpassw = GetAdscPassws(qsList[0], qsList[1]);

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }


    
        public Adscpassw GetAdscPassws(string miembro, string token)
        {
         
            var objectToken = db.Adscpassw.Where(p => p.AdpsLoginAdm == miembro && p.AdpsTokenTemp == token).FirstOrDefault();

            return objectToken;
        }

        [HttpPost]
        [Route("SeleccionarMiembroLogueado")]
        public Adscpassw GetAdscPassws([FromBody] Adscpassw adscpassw)
        {
            //try
            //{
            //    if (!ModelState.IsValid)
            //    {
            //        return new Response
            //        {
            //            IsSuccess = false,
            //            Message = Mensaje.ModeloInvalido,
            //        };
            //    }

                var adscgrpSeleccionado =  db.Adscpassw.Where(m => m.AdpsLoginAdm == adscpassw.AdpsLoginAdm && m.AdpsTokenTemp == adscpassw.AdpsTokenTemp).FirstOrDefault();

                return adscgrpSeleccionado;
            //    if (adscgrpSeleccionado == null)
            //    {
            //        return new Response
            //        {
            //            IsSuccess = false,
            //            Message = Mensaje.RegistroNoEncontrado,
            //        };
            //    }

            //    return new Response
            //    {
            //        IsSuccess = true,
            //        Message = Mensaje.Satisfactorio,
            //        Resultado = adscgrpSeleccionado,
            //    };
            //}
            //catch (Exception ex)
            //{
            //    await GuardarLogService.SaveLogEntry(new LogEntryTranfer
            //    {
            //        ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
            //        ExceptionTrace = ex,
            //        Message = Mensaje.Excepcion,
            //        LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
            //        LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
            //        UserName = "",

            //    });
            //    return new Response
            //    {
            //        IsSuccess = false,
            //        Message = Mensaje.Error,
            //    };
            //}
        }
    }
}
