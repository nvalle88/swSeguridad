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
using bd.log.guardar.Enumeradores;
using bd.swseguridad.entidades.Utils;
using bd.swseguridad.entidades.Constantes;

namespace bd.swseguridad.web.Controllers.API
{
    [Produces("application/json")]
    [Route("api/Adscsists")]
    public class AdscsistsController : Controller
    {
        private readonly SwSeguridadDbContext db;

        public AdscsistsController(SwSeguridadDbContext db)
        {
            this.db = db;
        }

        // GET: api/Adscsists
        [HttpGet]
        [Route("ListarAdscSistema")]
        public async Task<List<Adscsist>> GetAdscsist()
        {
            try
            {
                return await db.Adscsist.OrderBy(x => x.AdstSistema).ToListAsync();
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
                return new List<Adscsist>();
            }
        }


        [HttpPost]
        [Route("ListarAdscSistemaMiembro")]
        public async Task<List<Adscsist>> GetAdscsistMiembro([FromBody] string miembro)
         {
            try
            {           
                var ListadoSistemas = await (from s in db.Adscsist
                                       join m in db.Adscmenu
                                       on s.AdstSistema equals m.AdmeSistema
                                       join e in db.Adscexe
                                       on new { colA = m.AdmeSistema, colB = m.AdmeAplicacion } equals new { colA = e.AdexSistema, colB = e.AdexAplicacion }
                                       join g in db.Adscgrp
                                       on new { colC = e.AdexGrupo, colD = e.AdexBdd } equals new { colC = g.AdgrGrupo, colD = g.AdgrBdd }
                                       join b in db.Adscmiem
                                       on new { colE = g.AdgrGrupo, colF = g.AdgrBdd } equals new { colE = b.AdmiGrupo, colF = b.AdmiBdd }
                                       where b.AdmiEmpleado == miembro
                                       group s by s.AdstDescripcion into pg
                                       select new Adscsist
                                       {
                                           AdstSistema = pg.FirstOrDefault().AdstSistema,
                                           AdstDescripcion = pg.FirstOrDefault().AdstDescripcion,
                                           AdstHost = pg.FirstOrDefault().AdstHost
                                       }).ToListAsync();

                var listaSalida = new List<Adscsist>();
                foreach (var item in ListadoSistemas)
                {
                    listaSalida.Add(new Adscsist
                    {
                        AdstSistema = item.AdstSistema,
                        AdstDescripcion = item.AdstDescripcion,
                        AdstHost = item.AdstHost
                    });
                }

                return listaSalida;
            }
            catch (Exception ex)
            {
                //await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                //{
                //    ApplicationName = Convert.ToString(Aplicacion.SwTH),
                //    ExceptionTrace = ex,
                //    Message = Mensaje.Excepcion,
                //    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                //    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                //    UserName = "",

                //});
                return new List<Adscsist>();
            }
        }

        // GET: api/Adscsists/5
        [HttpGet("{id}")]
        public async Task<Response> GetAdscsist([FromRoute] string id)
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

                var adscbdd = await db.Adscsist.SingleOrDefaultAsync(m => m.AdstSistema == id);

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
                    Message = Mensaje.Satisfactorio,
                    Resultado = adscbdd,
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

        // PUT: api/Adscsists/5
        [HttpPut("{id}")]
        public async Task<Response> PutAdscsist([FromRoute] string id, [FromBody] Adscsist adscsist)
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

                var adscsistActualizar = await db.Adscsist.Where(x => x.AdstSistema == id).FirstOrDefaultAsync();
                if (adscsistActualizar != null)
                {
                    try
                    {
                        adscsistActualizar.AdstBdd = adscsist.AdstBdd;
                        adscsistActualizar.AdstTipo = adscsist.AdstTipo;
                        adscsistActualizar.AdstHost = adscsist.AdstHost;
                        adscsistActualizar.AdstDescripcion = adscsist.AdstDescripcion;
                        db.Adscsist.Update(adscsistActualizar);
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

        // POST: api/Adscsists
        [HttpPost]
        [Route("InsertarAdscSist")]
        public async Task<Response> PostAdscsist([FromBody] Adscsist adscsist)
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

                var respuesta = Existe(adscsist);
                if (!respuesta.IsSuccess)
                {
                    db.Adscsist.Add(adscsist);
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

        // DELETE: api/Adscsists/5
        [HttpDelete("{id}")]
        public async Task<Response> DeleteAdscsist([FromRoute] string id)
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

                var respuesta = await db.Adscsist.SingleOrDefaultAsync(m => m.AdstSistema == id);
                if (respuesta == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }
                db.Adscsist.Remove(respuesta);
                await db.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.Satisfactorio,
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

        public Response Existe(Adscsist adscsist)
        {
            var bdd = adscsist.AdstSistema.ToUpper().TrimEnd().TrimStart();
            var loglevelrespuesta = db.Adscsist.Where(p => p.AdstSistema.ToUpper().TrimStart().TrimEnd() == bdd).FirstOrDefault();
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