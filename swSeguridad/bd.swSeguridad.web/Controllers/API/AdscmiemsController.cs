using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bd.swseguridad.datos;
using bd.swseguridad.entidades.Negocio;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.swseguridad.entidades.Enumeradores;
using bd.swseguridad.entidades.Utils;
using bd.log.guardar.Enumeradores;
using bd.swseguridad.entidades.Constantes;

namespace bd.swseguridad.web.Controllers.API
{
    /// <summary>
    /// Controladores API de los servicios web estos son los que trabajan directamente con la base de datos
    /// Tiene algunos aspectos que explicar como:
    /// [Produces("application/json")]: filtro especifica los formatos de 
    /// respuesta para una acción específica (o controlador). 
    /// Al igual que la mayoría de los filtros , 
    /// esto se puede aplicar en la acción, el controlador o el alcance global.
    /// para más información visitar:https://docs.microsoft.com/en-us/aspnet/core/mvc/models/formatting
    /// [Route("api/Codificar")]:es la ruta del recurso del controlador en general
    /// public class NombreController : Controller
    /// [Route("Nombre")] en los métodos es la ruta del recurso.
    /// para acceder a estos recursos es: Host + ruta concatenados 
    /// , ruta del controlador + ruta del método
    /// [FromBody] es para capturar el objeto que se envia en el body 
    /// para más información visitar:https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api
    /// </summary>
    [Produces("application/json")]
    [Route("api/Adscmiems")]
    public class AdscmiemsController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public AdscmiemsController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        // GET: api/Adscmiems
        [HttpGet]
        [Route("ListarAdscmiem")]
        public async Task<List<Adscmiem>>  GetAdscmiem()
        {
            try
            {
                return await db.Adscmiem.OrderBy(x => x.AdmiGrupo).ThenBy(x => x.AdmiEmpleado).ToListAsync();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new List<Adscmiem>();
            }
        }


        [HttpPost]
        [Route("SeleccionarAdscmiem")]
        public async Task<Response> GetAdscmiem([FromBody] Adscmiem adscmiem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.ModeloInvalido,
                    };
                }

                var adscmienmSeleccionado = await db.Adscmiem.SingleOrDefaultAsync(m => m.AdmiGrupo == adscmiem.AdmiGrupo 
                                                                                   && m.AdmiEmpleado == adscmiem.AdmiEmpleado
                                                                                   && m.AdmiBdd == adscmiem.AdmiBdd);

                if (adscmienmSeleccionado == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.Satisfactorio,
                    Resultado = adscmienmSeleccionado,
                };
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Error,
                };
            }
        }


        [HttpPut]
        [Route("EditarAdscmiem")]
        public async Task<Response> PutAdscmiem([FromBody] Adscmiem adscmiem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.ModeloInvalido
                    };
                }

                var adscmienmSeleccionado = await db.Adscmiem.SingleOrDefaultAsync(m => m.AdmiGrupo == adscmiem.AdmiGrupo
                                                                                 && m.AdmiEmpleado == adscmiem.AdmiEmpleado
                                                                                 && m.AdmiBdd == adscmiem.AdmiBdd);
                if (adscmienmSeleccionado != null)
                {
                    try
                    {
                        adscmienmSeleccionado.AdmiTotal = adscmiem.AdmiTotal;
                        adscmienmSeleccionado.AdmiCodigoEmpleado = adscmiem.AdmiCodigoEmpleado;
                        db.Adscmiem.Update(adscmienmSeleccionado);
                        await db.SaveChangesAsync();

                        return new Response
                        {
                            IsSuccess = true,
                            Message = Mensaje.Satisfactorio,
                            Resultado=adscmienmSeleccionado,
                        };

                    }
                    catch (Exception ex)
                    {
                        await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                        {
                            ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                            ExceptionTrace = ex,
                            Message = Mensaje.Excepcion,
                            LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                            LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                            UserName = "",

                        });
                        return new Response
                        {
                            IsSuccess = false,
                            Message = Mensaje.Error,
                        };
                    }
                }

                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.ExisteRegistro
                };
            }
            catch (Exception)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Excepcion
                };
            }
        }


        [HttpPost]
        [Route("InsertarAdscmiem")]
        public async Task<Response> PostAdscmiem([FromBody] Adscmiem adscmiem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.ModeloInvalido
                    };
                }

                var respuesta = Existe(adscmiem);
                if (!respuesta.IsSuccess)
                {
                    db.Adscmiem.Add(adscmiem);
                    await db.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = Mensaje.Satisfactorio,
                        Resultado=adscmiem,
                    };
                }

                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.ExisteRegistro
                };

            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Error,
                };
            }
        }


        [HttpPost]
        [Route("EliminarAdscmiem")]
        public async Task<Response> DeleteAdscmiem([FromBody]Adscmiem adscmiem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.ModeloInvalido,
                    };
                }

                var adscmienmSeleccionado = await db.Adscmiem.SingleOrDefaultAsync(m => m.AdmiGrupo == adscmiem.AdmiGrupo
                                                                                  && m.AdmiEmpleado == adscmiem.AdmiEmpleado
                                                                                  && m.AdmiBdd == adscmiem.AdmiBdd);
                if (adscmienmSeleccionado == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }
                db.Adscmiem.Remove(adscmienmSeleccionado);
                await db.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.Satisfactorio,
                    Resultado=adscmiem,
                };
            }
            catch (Exception ex)
            {

                if (ex.InnerException.Message.Contains(Constantes.Referencia))
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.BorradoNoSatisfactorio,
                    };

                }
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Error,
                };
            }
        }

        public Response Existe(Adscmiem adscmiem)
        {
            var grupo = adscmiem.AdmiGrupo.ToUpper().TrimEnd().TrimStart();
            var bdd = adscmiem.AdmiBdd.ToUpper().TrimEnd().TrimStart();
            var empleado = adscmiem.AdmiEmpleado.ToUpper().TrimEnd().TrimStart();
            var loglevelrespuesta = db.Adscmiem.Where(p => p.AdmiBdd.ToUpper().TrimStart().TrimEnd() == bdd 
                                                        && p.AdmiGrupo.ToUpper().TrimStart().TrimEnd() == grupo
                                                        && p.AdmiEmpleado.ToUpper().TrimStart().TrimEnd()==empleado).FirstOrDefault();
            if (loglevelrespuesta != null)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.ExisteRegistro,
                    Resultado = null,
                };

            }

            return new Response
            {
                IsSuccess = false,
                Resultado = loglevelrespuesta,
            };
        }


        // DELETE: api/Adscmiems/5
      
    }
}