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
using Newtonsoft.Json.Linq;
using bd.swseguridad.entidades.ObjectTranfer;
using bd.swseguridad.entidades.Constantes;

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

        
        [HttpGet]
        [Route("ListarAdscgrpDistinct")]
        public async Task<List<Adscgrp>> GetAdscgrpDistinct()
        {
            try
            {
                return await db.Adscgrp.GroupBy(x => x.AdgrBdd).Select(group => group.First()).ToListAsync();
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

        [HttpPost]
        [Route("MenusGrupo")]
        public async Task<List<Adscmenu>> MenusGrupo([FromBody] Adscgrp adscgrp)
        {

            try
            {
                var adscexe = await db.Adscexe.Where(x => x.AdexBdd == adscgrp.AdgrBdd && x.AdexGrupo == adscgrp.AdgrGrupo).ToListAsync();

                var listamenus = new List<Adscmenu>();
                foreach (var item in adscexe)
                {
                    var elementos = await db.Adscmenu.Where(x => x.AdmeSistema == item.AdexSistema && x.AdmeAplicacion == item.AdexAplicacion).FirstOrDefaultAsync();

                    listamenus.Add(elementos);
                }
                return listamenus;


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
                return new List<Adscmenu>();
            }
        }


        [HttpPost]
        [Route("MiembrosGrupo")]
        public async Task<List<Adscmiem>> MiembrosGrupo([FromBody] Adscgrp adscgrp)
        {

            try
            {
                return await db.Adscmiem.Where(x => x.AdmiBdd == adscgrp.AdgrBdd && x.AdmiGrupo==adscgrp.AdgrGrupo).ToListAsync();
                
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
        [Route("ListarBddPorGrupo")]
        public async Task<List<Adscbdd>> GetAdscbddPorGrp([FromBody] Adscgrp adscgrp)
        {
          
            try
            {
               var lista= await db.Adscgrp.Where(x=>x.AdgrGrupo==adscgrp.AdgrGrupo).Include(x=>x.AdgrBddNavigation).ToListAsync();
                var listaBdd = new List<Adscbdd>();
                foreach (var item in lista)
                {
                    listaBdd.Add(item.AdgrBddNavigation);
                }
                return listaBdd;
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
                return new List<Adscbdd>();
            }
        }


        [HttpPost]
        [Route("ListarGrupoPorBdd")]
        public async Task<List<Adscgrp>> GetGprPorBdd([FromBody] Adscgrp adscgrp)
        {

            try
            {
                var lista = await db.Adscgrp.Where(x => x.AdgrBdd == adscgrp.AdgrBdd).ToListAsync();
                return lista;
               
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
        [HttpPut]
        [Route("EditarAdscgrp")]
        public async Task<Response> PutAdscgrp( [FromBody] Adscgrp adscgrp)
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
        [Route("InsertarAdscgrp")]
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
                if (ex.InnerException.Message.Contains(Constantes.Referencia))
                {
                    return new Response
                    {
                        IsSuccess =false,
                        Message=Mensaje.BorradoNoSatisfactorio,
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