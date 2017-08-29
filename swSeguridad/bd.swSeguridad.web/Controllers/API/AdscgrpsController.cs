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

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Adscgrps")]
    public class AdscgrpsController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public AdscgrpsController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        // GET: api/Adscgrps
        [HttpGet]
        [Route("ListarAdscgrp")]
        public async Task<List<Adscgrp>> GetAdscgrp()
        {
            try
            {
                return await db.Adscgrp.OrderBy(x => x.AdgrGrupo).ThenBy(x=>x.AdgrNombre).ToListAsync();
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
                return new List<Adscgrp>();
            }
        }

        // GET: api/Adscgrps/5
        [HttpPost]
        [Route("SeleccionarAdscgrp")]
        public async Task<Response> GetAdscgrp([FromBody] Adscgrp adscgrp)
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

                var adscgrpSeleccionado= await db.Adscgrp.SingleOrDefaultAsync(m => m.AdgrGrupo == adscgrp.AdgrGrupo && m.AdgrBdd==adscgrp.AdgrBdd);

                if (adscgrpSeleccionado == null)
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
                    Resultado = adscgrpSeleccionado,
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

        // PUT: api/Adscgrps/5
        [HttpPut("{id}")]
        public async Task<Response> PutAdscgrp([FromRoute] string id, [FromBody] Adscgrp adscgrp)
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

                var adscgrpSeleccionado = await db.Adscgrp.SingleOrDefaultAsync(m => m.AdgrGrupo == adscgrp.AdgrGrupo && m.AdgrBdd == adscgrp.AdgrBdd);
                if (adscgrpSeleccionado != null)
                {
                    try
                    {
                        adscgrpSeleccionado.AdgrNombre = adscgrp.AdgrNombre;
                        adscgrpSeleccionado.AdgrDescripcion = adscgrp.AdgrDescripcion;
                        db.Adscgrp.Update(adscgrpSeleccionado);
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

        // POST: api/Adscgrps
        [HttpPost]
        public async Task<Response> PostAdscgrp([FromBody] Adscgrp adscgrp)
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

                var respuesta = Existe(adscgrp);
                if (!respuesta.IsSuccess)
                {
                    db.Adscgrp.Add(adscgrp);
                    await db.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = Mensaje.Satisfactorio
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

        // DELETE: api/Adscgrps/5
        [HttpPost]
        [Route("EliminarAdscgrp")]
        public async Task<Response> DeleteAdscgrp([FromBody]Adscgrp adscgrp)
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

                var adscgrpSeleccionado = await db.Adscgrp.SingleOrDefaultAsync(m => m.AdgrGrupo == adscgrp.AdgrGrupo && m.AdgrBdd == adscgrp.AdgrBdd);
                if (adscgrpSeleccionado == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }
                db.Adscgrp.Remove(adscgrpSeleccionado);
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

        public Response Existe(Adscgrp adscgrp)
        {
            var grupo = adscgrp.AdgrGrupo.ToUpper().TrimEnd().TrimStart();
            var bdd = adscgrp.AdgrBdd.ToUpper().TrimEnd().TrimStart();
            var loglevelrespuesta = db.Adscgrp.Where(p => p.AdgrBdd.ToUpper().TrimStart().TrimEnd() == bdd && p.AdgrGrupo.ToUpper().TrimStart().TrimEnd()==grupo).FirstOrDefault();
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
    }
}