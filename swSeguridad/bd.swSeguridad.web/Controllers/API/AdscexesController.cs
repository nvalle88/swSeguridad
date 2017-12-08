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
    [Produces("application/json")]
    [Route("api/Adscexes")]
    public class AdscexesController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public AdscexesController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        // GET: api/Adscexes
        [HttpGet]
        [Route("ListarAdscexe")]
        public async Task<List<Adscexe>> GetAdscexe()
        {
            try
            {
                return await db.Adscexe.OrderBy(x => x.AdexGrupo).ThenBy(x => x.AdexAplicacion).ToListAsync();
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
                return new List<Adscexe>();
            }
        }

        [HttpPost]
        [Route("SeleccionarAdscexe")]
        public async Task<Response> GetAdscMenu([FromBody] Adscexe adscexe)
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

                var adscexeSeleccionado = SeleccionarAdscExe(adscexe);
                if (adscexeSeleccionado == null)
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
                    Resultado = adscexeSeleccionado,
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
        [Route("EditarAdscexe")]
        public async Task<Response> PutAdscmenu([FromBody] Adscexe adscexe)
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
               var adscexeSeleccionado= SeleccionarAdscExe(adscexe);

                if (adscexeSeleccionado != null)
                {
                    try
                    {
                        adscexeSeleccionado.AdexBdd = adscexe.AdexBdd;
                        adscexeSeleccionado.AdexAplicacion = adscexe.AdexAplicacion;
                        adscexeSeleccionado.AdexGrupo = adscexe.AdexGrupo;
                        adscexeSeleccionado.AdexSistema = adscexe.AdexSistema;
                        adscexeSeleccionado.AdexSql = adscexe.AdexSql;
                        adscexeSeleccionado.Del = adscexe.Del;
                        adscexeSeleccionado.Ins = adscexe.Ins;
                        adscexeSeleccionado.Sel = adscexe.Sel;
                        adscexeSeleccionado.Upd = adscexe.Upd;
                        db.Adscexe.Update(adscexeSeleccionado);
                        await db.SaveChangesAsync();

                        return new Response
                        {
                            IsSuccess = true,
                            Message = Mensaje.Satisfactorio,
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

        // POST: api/Adscmenus
        [HttpPost]
        [Route("InsertarAdscexe")]
        public async Task<Response> PostAdscgrp([FromBody] Adscexe adscexe)
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

                var respuesta = Existe(adscexe);
                if (!respuesta.IsSuccess)
                {
                    db.Adscexe.Add(adscexe);
                    await db.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = Mensaje.Satisfactorio,
                        Resultado=adscexe,
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

        // DELETE: api/Adscmenus/5
        [HttpPost]
        [Route("EliminarAdscexe")]
        public async Task<Response> DeleteAdscgrp([FromBody]Adscexe adscexe)
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
                var respuesta = SeleccionarAdscExe(adscexe);

                if (respuesta == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }
                db.Adscexe.Remove(respuesta);
                await db.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.Satisfactorio,
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

        public Adscexe SeleccionarAdscExe(Adscexe adscexe)
        {
            var sistema = adscexe.AdexSistema.ToUpper().TrimEnd().TrimStart();
            var baseDatos = adscexe.AdexBdd.ToUpper().TrimEnd().TrimStart();
            var grupo = adscexe.AdexGrupo.ToUpper().TrimEnd().TrimStart();
            var aplicacion = adscexe.AdexAplicacion.ToUpper().TrimEnd().TrimStart();
            var respuesta = db.Adscexe.Where(p => p.AdexSistema.ToUpper().TrimStart().TrimEnd() == sistema &&
                                             p.AdexAplicacion.ToUpper().TrimStart().TrimEnd() == aplicacion &&
                                             p.AdexBdd == baseDatos && p.AdexGrupo == grupo)
                                             .FirstOrDefault();
            return respuesta;
        }

        public Response Existe(Adscexe adscexe)
        {
            var respuesta = SeleccionarAdscExe(adscexe);

            if (respuesta != null)
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
                Resultado = respuesta,
            };
        }
    }
}