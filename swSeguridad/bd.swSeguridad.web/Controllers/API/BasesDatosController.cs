using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using bd.swseguridad.datos;
using bd.swseguridad.entidades.Negocio;
using bd.swseguridad.entidades.Utils;
using bd.log.guardar.Servicios;
using bd.log.guardar.ObjectTranfer;
using bd.swseguridad.entidades.Enumeradores;
using bd.log.guardar.Enumeradores;
using bd.swseguridad.entidades.Constantes;

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/BasesDatos")]
    public class BasesDatosController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public BasesDatosController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        // GET: api/BasesDatos
        [HttpGet]
        [Route("ListarBasesDatos")]
        public async Task<List<Adscbdd>> GetAdscbdd()
        {
            try
            {
                return await db.Adscbdd.OrderBy(x => x.AdbdBdd).ToListAsync();
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = "Se ha producido una exepción",
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new List<Adscbdd>();
            }
        }

        public async Task<Adscbdd> SeleccionarAdscbdd(Adscbdd adscbdd)
        {
            var respuesta =await db.Adscbdd.Where(x => x.AdbdBdd == adscbdd.AdbdBdd).FirstOrDefaultAsync();
            return respuesta;
        }

        // GET: api/BasesDatos/5
        [HttpPost]
        [Route("SeleccionarAdscBdd")]
        public async Task<Response> GetAdscbdd([FromBody]  Adscbdd adscbdd)
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

                var respuesta = await db.Adscbdd.SingleOrDefaultAsync(m => m.AdbdBdd == adscbdd.AdbdBdd);

                if (adscbdd == null)
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
                    Message = "Ok",
                    Resultado = respuesta,
                };
            }
            catch (Exception ex)
            {
                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    Message = "Se ha producido una exepción",
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = "Error ",
                };
            }
        }

        // PUT: api/BasesDatos/5
        [HttpPut]
        [Route("EditarAdscbdd")]
        public async Task<Response> PutAdscbdd([FromBody] Adscbdd adscbdd)
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
                var RespuestaActualizar = await SeleccionarAdscbdd(adscbdd);

                if (RespuestaActualizar != null)
                {
                    try
                    {
                        RespuestaActualizar.AdbdBdd = adscbdd.AdbdBdd;
                        RespuestaActualizar.AdbdDescripcion = adscbdd.AdbdDescripcion;
                        RespuestaActualizar.AdbdServidor = adscbdd.AdbdServidor;
                        db.Adscbdd.Update(RespuestaActualizar);
                        await db.SaveChangesAsync();

                        return new Response
                        {
                            IsSuccess = true,
                            Message = Mensaje.Satisfactorio,
                            Resultado = RespuestaActualizar,
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

        // POST: api/BasesDatos
        [HttpPost]
        [Route("InsertarBaseDatos")]
        public async Task<Response> PostAdscbdd([FromBody] Adscbdd adscbdd)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess=false,
                        Message=Mensaje.ModeloInvalido,
                    };
                }
                var respuesta = Existe(adscbdd);
                if (!respuesta.IsSuccess)
                {
                    db.Adscbdd.Add(adscbdd);
                    await db.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = Mensaje.Satisfactorio,
                        Resultado=adscbdd,
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
                    Message = "Se ha producido una exepción",
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = "Error ",
                };
            }
        }

        // DELETE: api/BasesDatos/acccion/5
        [HttpDelete("{id}")]
        public async Task<Response> DeleteAdscbdd([FromRoute] string id)
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

                var respuesta = await db.Adscbdd.SingleOrDefaultAsync(m => m.AdbdBdd == id);
                if (respuesta == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }
                db.Adscbdd.Remove(respuesta);
                await db.SaveChangesAsync();

                 return new Response
                {
                    IsSuccess = true,
                   Resultado=respuesta,
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
                    ExceptionTrace=ex,
                    Message="Se ha producido una exepción",
                    LogCategoryParametre= Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName=Convert.ToString(LogLevelParameter.ERR),
                    UserName="",

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = Mensaje.Error,
                };
            }
        }       

        private Response Existe(Adscbdd adscbdd)
        {
            var bdd = adscbdd.AdbdBdd.ToUpper().TrimEnd().TrimStart();
            var loglevelrespuesta = db.Adscbdd.Where(p => p.AdbdBdd.ToUpper().TrimStart().TrimEnd() == bdd).FirstOrDefault();
            if (loglevelrespuesta != null)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Existe una base de datos de igual nombre",
                    Resultado = null,
                };

            }

            return new Response
            {
                IsSuccess = false,
                Resultado = loglevelrespuesta,
            };
        }
    }
}