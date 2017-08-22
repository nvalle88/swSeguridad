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
using bd.swseguridad.entidades.ObjectTranfer;
using bd.log.guardar.Servicios;
using bd.swseguridad.entidades.Enumeradores;
using bd.log.guardar.ObjectTranfer;
using bd.log.guardar.Enumeradores;

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Adscpassws")]
    public class AdscpasswsController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public AdscpasswsController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        // GET: api/Adscpassws
        [HttpGet]
        [Route("ListarAdscPassw")]
        public async Task<List<Adscpassw>> GetAdscpassw()
        {
            try
            {
                return await db.Adscpassw.OrderBy(x => x.AdpsLogin).ToListAsync();
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
                return new List<Adscpassw>();
            }
        }

        [Route("Login")]
        public async Task<Response> Login([FromBody] Login login)
        {

            try
            {


                var usuario = db.Adscpassw.Where(x => x.AdpsLogin == login.Usuario).FirstOrDefault();

                if (usuario == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No existe"
                    };
                }

                if (usuario.AdpsFechaVencimiento<DateTime.Now)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Usuario Caducado"
                    };
                }

                if (usuario.AdpsIntentos>=3)
                {
                    return new Response
                    {
                        IsSuccess=false,
                        Message="Usuario Bloqueado contacte con el administrador"
                    }; 
                }

               
                
                var existeLogin = db.Adscpassw.Where(x => x.AdpsLogin == login.Usuario && x.AdpsPassword == login.Contrasena).FirstOrDefault();
                if (existeLogin == null)
                {
                    usuario.AdpsIntentos = usuario.AdpsIntentos +1;
                    db.Entry(usuario).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No existe",
                        Resultado = "",
                    };
                }
                return new Response
                {
                    IsSuccess=true,
                    Message="Ok",
                    Resultado=existeLogin,
                };
            }
            catch (Exception ex)
            {

                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName =Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace=ex,
                    LogCategoryParametre=Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName=Convert.ToString(LogLevelParameter.ERR),
                    Message=ex.Message,
                    UserName=login.Usuario,
                    
                });
                return new Response
                {
                    IsSuccess=false,
                    Message=ex.Message,

                };
                throw;
            }


        }

        // GET: api/Adscpassws/5
        [HttpGet("{id}")]
        public async Task<Response>GetAdscpassw([FromRoute] string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Módelo no válido",
                    };
                }

                var adscbdd = await db.Adscpassw.SingleOrDefaultAsync(m => m.AdpsLogin == id);

                if (adscbdd == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No encontrado",
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Resultado = adscbdd,
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

        // PUT: api/Adscpassws/5
        [HttpPut("{id}")]
        public async Task<Response> PutAdscpassw([FromRoute] string id, [FromBody] Adscpassw adscpassw)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Módelo inválido"
                    };
                }

                var adscPsswActualizar = await db.Adscpassw.Where(x => x.AdpsLogin == id).FirstOrDefaultAsync();
                if (adscPsswActualizar != null)
                {
                    try
                    {

                        adscPsswActualizar.AdpsPreguntaRecuperacion = adscpassw.AdpsPreguntaRecuperacion;
                        adscPsswActualizar.AdpsRespuestaRecuperacion = adscpassw.AdpsRespuestaRecuperacion;
                        db.Adscpassw.Update(adscPsswActualizar);
                        await db.SaveChangesAsync();

                        return new Response
                        {
                            IsSuccess = true,
                            Message = "Ok",
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

                return new Response
                {
                    IsSuccess = false,
                    Message = "Existe"
                };
            }
            catch (Exception)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Excepción"
                };
            }
        }

        // POST: api/Adscpassws
        [HttpPost]
        [Route("InsertarAdscPassw")]
        public async Task<Response> PostAdscpassw([FromBody] Adscpassw adscpassw)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Módelo inválido"
                    };
                }

                var respuesta = Existe(adscpassw);
                if (!respuesta.IsSuccess)
                {
                    db.Adscpassw.Add(adscpassw);
                    await db.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = "OK"
                    };
                }

                return new Response
                {
                    IsSuccess = false,
                    Message = "OK"
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

        // DELETE: api/Adscpassws/5
        [HttpDelete("{id}")]
        public async Task<Response> DeleteAdscpassw([FromRoute] string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Módelo no válido ",
                    };
                }

                var respuesta = await db.Adscpassw.SingleOrDefaultAsync(m => m.AdpsLogin == id);
                if (respuesta == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "No existe ",
                    };
                }
                db.Adscpassw.Remove(respuesta);
                await db.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = "Eliminado ",
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

        private bool AdscpasswExists(string id)
        {
            return db.Adscpassw.Any(e => e.AdpsLogin == id);
        }

        public Response Existe(Adscpassw adscpassw)
        {
            var bdd = adscpassw.AdpsLogin.ToUpper().TrimEnd().TrimStart();
            var loglevelrespuesta = db.Adscpassw.Where(p => p.AdpsLogin.ToUpper().TrimStart().TrimEnd() == bdd).FirstOrDefault();
            if (loglevelrespuesta != null)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = "Existe Un  usuario de igual nombre",
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