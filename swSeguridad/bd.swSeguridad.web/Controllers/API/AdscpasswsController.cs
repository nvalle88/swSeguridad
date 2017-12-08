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
using System.Security.Cryptography;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

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
                    Message = Mensaje.Excepcion,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    UserName = "",

                });
                return new List<Adscpassw>();
            }
        }

        [HttpPost]
        [Route("TienePermiso")]
        public async Task<Response> TienePermiso([FromBody]PermisoUsuario permiso)
        {
            try
            {
                var path =NormalizarPathContexto(permiso.Contexto);

                var token =await db.Adscpassw.Where(x => x.AdpsToken == permiso.Token && x.AdpsLogin==permiso.Usuario).FirstOrDefaultAsync();

                if (token!=null)
                {
                    var grupos = await db.Adscmiem.Where(m => m.AdmiEmpleado == permiso.Usuario).ToListAsync();
                    foreach (var item in grupos)
                    {
                        var a = await db.Adscexe.Where(x => x.AdexGrupo == item.AdmiGrupo).ToListAsync();
                        foreach (var s in a)
                        {
                            var ds = await db.Adscmenu.Where(x => x.AdmeSistema == s.AdexSistema && x.AdmeAplicacion == s.AdexAplicacion).FirstOrDefaultAsync();
                            if (path.ToUpper()==ds.AdmeControlador.ToUpper())
                            {
                                return new Response { IsSuccess = true };
                            }

                        }
                    } 
                }

                return new Response { IsSuccess = false };

            }
            catch (Exception ex)
            {

                throw;
            }

          
        }

        [HttpPost]
        [Route("TienePermisoTemp")]
        public async Task<Response> TienePermisoTemp([FromBody]PermisoUsuario permiso)
        {
            try
            {
          

                var token = await db.Adscpassw.Where(x => x.AdpsToken == permiso.Token && x.AdpsLogin == permiso.Usuario).FirstOrDefaultAsync();
                return new Response { IsSuccess = true };

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        [HttpPost]
        [Route("TieneAcceso")]
        public async Task<Response> TieneAcceso([FromBody] PermisoUsuarioSwExternos permiso)
        {
            try
            {
              
                    var servicio = await db.Adscswepwd.Where(x => x.AdpsLogin == permiso.Usuario && x.AdseSw == permiso.NombreServicio).FirstOrDefaultAsync();
                    if (servicio != null)
                    {
                        return new Response { IsSuccess = true };
                    }
                
                return new Response { IsSuccess = false };

            }
            catch (Exception ex)
            {
                return new Response { IsSuccess = false };
            }

        }

        [HttpPost]
        [Route("TienePermisoSwExterno")]
        public async Task<Response> TienePermisoSwExterno([FromBody] PermisoUsuarioSwExternos permiso)
        {
            try
            {
                var token = await db.Adsctoken.Where(x => x.AdtoToken == permiso.Token && x.AdtoId==permiso.Id && x.AdpsLogin == permiso.Usuario && x.AdtoNombreServicio==permiso.NombreServicio).FirstOrDefaultAsync();
                if (token != null)
                { 
                    var servicio = await db.Adscswepwd.Where(x => x.AdpsLogin == permiso.Usuario && x.AdseSw==permiso.NombreServicio).FirstOrDefaultAsync();
                    if (servicio != null)
                    {   
                        return new Response {IsSuccess=true};
                    }
                }
                return new Response { IsSuccess = false };

            }
            catch (Exception ex)
            {
                return new Response { IsSuccess = false };
            }
            
        }


        private async Task<bool> EliminarToken(PermisoUsuarioSwExternos objeto)
        {
            try
            {
                var token = await db.Adsctoken.Where(x => x.AdtoToken == objeto.Token && x.AdpsLogin == objeto.Usuario).FirstOrDefaultAsync();
                db.Entry(token).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpPost]
        [Route("ConsumirSwExterno")]
        public async Task<JsonResult> ConsumirSWExterno([FromBody] PermisoUsuarioSwExternos objeto)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    if (objeto!=null)
                    {
                        var uriServicio = db.Adscswext.Where(x => x.AdseSw == objeto.NombreServicio).FirstOrDefaultAsync().Result.AdseUri;
                        var eliminado = false;
                        if (objeto.parametros == null)
                        {
                            var respuestaGet = await client.GetAsync(new Uri(uriServicio));
                            var resultadoGet = await respuestaGet.Content.ReadAsStringAsync();
                            var respuesta = JsonConvert.DeserializeObject(resultadoGet);
                            eliminado = await EliminarToken(objeto);
                            if (!eliminado)
                            {
                                return Json(false);
                            }
                            return Json(respuesta);
                        }

                        var request = JsonConvert.SerializeObject(objeto.parametros);
                        var content = new StringContent(request, Encoding.UTF8, "application/json");
                        var respuestaPost = await client.PostAsync(new Uri(uriServicio), content);
                        var resultadoPost = await respuestaPost.Content.ReadAsStringAsync();
                        var resultadoPostD= JsonConvert.DeserializeObject(resultadoPost);

                        ///Eliminar Token de la base de datos
                        ///
                        eliminado=await EliminarToken(objeto);
                        if (!eliminado)
                        {
                            return Json(false);
                        }

                        return Json(resultadoPostD); 


                    }
                    
                }
                
            }
            catch (Exception ex)
            {
                Json(false);
            }
          return  Json(false);
        }

        private string NormalizarPathContexto(string cadena)
        {
            var contador = 0;
            var ultimoencontrado = 0;
            for (int i = 0; i < cadena.Length; i++)
            {
                if (cadena[i].ToString()==("/"))
                {
                    contador = contador + 1;
                    ultimoencontrado = i;
                }
            }
            if (contador>=3)
            {
                var recortar = cadena.Length -ultimoencontrado ;
                cadena= cadena.Remove(ultimoencontrado, recortar);
            }
            return cadena;
        }


        [HttpPost]
        [Route("SalvarTokenSwExternos")]
        public async Task<Response> SalvarTokenSwExternos([FromBody]  PermisoUsuarioSwExternos permisoUsuario)
        {
            try
            {
                var usuario =await db.Adscpassw.Where(x => x.AdpsLogin == permisoUsuario.Usuario).FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado
                    };
                }


               
                var adsctokenRequest = await db.Adsctoken.Where(x => x.AdpsLogin == permisoUsuario.Usuario && x.AdtoNombreServicio == permisoUsuario.NombreServicio).FirstOrDefaultAsync();

                if (adsctokenRequest == null)
                {
                    var adsctoken = new Adsctoken
                    {
                        AdpsLogin = permisoUsuario.Usuario,
                        AdtoNombreServicio = permisoUsuario.NombreServicio,
                        AdtoToken = permisoUsuario.Token,
                        AdtoFecha = DateTime.Now,
                    };
                    db.Adsctoken.Add(adsctoken);
                    await db.SaveChangesAsync();

                    return new Response
                    {
                        IsSuccess = true,
                        Resultado=adsctoken,
                    };
                }


                var SeleccionarToken =await db.Adsctoken.Where(x=>x.AdtoId==adsctokenRequest.AdtoId).FirstOrDefaultAsync();

                db.Adsctoken.Remove(SeleccionarToken);
                await db.SaveChangesAsync();
                var Token = SeleccionarToken;
                Token.AdtoId = 0;
                Token.AdtoToken = permisoUsuario.Token;
                Token.AdtoFecha = DateTime.Now;
                db.Adsctoken.Add(SeleccionarToken);
                await db.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                    Resultado = SeleccionarToken,
                };
            }
            catch (Exception ex)
            {

                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    Message = ex.Message,
                    UserName = permisoUsuario.Usuario,

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,

                };
                throw;
            }
        }



        [HttpPost]
        [Route("SalvarToken")]
        public async Task<Response> SalvarToken([FromBody]  PermisoUsuario permisoUsuario)
        {
            try
            {
                var usuario = db.Adscpassw.Where(x => x.AdpsLogin == permisoUsuario.Usuario).FirstOrDefault();

                if (usuario == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado
                    };
                }

                usuario.AdpsToken = permisoUsuario.Token;
                db.Adscpassw.Update(usuario);
                await db.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                };

               
            }
            catch (Exception ex)
            {

                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    Message = ex.Message,
                    UserName = permisoUsuario.Usuario,

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,

                };
                throw;
            }
        }


        [HttpPost]
        [Route("SalvarTokenTemp")]
        public async Task<Response> SalvarTokenTemp([FromBody]  PermisoUsuario permisoUsuario)
        {
            try
            {
                var usuario = db.Adscpassw.Where(x => x.AdpsLogin == permisoUsuario.Usuario).FirstOrDefault();

                if (usuario == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado
                    };
                }

                usuario.AdpsTokenTemp = permisoUsuario.Token;
                db.Adscpassw.Update(usuario);
                await db.SaveChangesAsync();

                return new Response
                {
                    IsSuccess = true,
                };


            }
            catch (Exception ex)
            {

                await GuardarLogService.SaveLogEntry(new LogEntryTranfer
                {
                    ApplicationName = Convert.ToString(Aplicacion.SwSeguridad),
                    ExceptionTrace = ex,
                    LogCategoryParametre = Convert.ToString(LogCategoryParameter.Critical),
                    LogLevelShortName = Convert.ToString(LogLevelParameter.ERR),
                    Message = ex.Message,
                    UserName = permisoUsuario.Usuario,

                });
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,

                };
                throw;
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<Response> Login([FromBody] Login login)
        {
            try
            {
                //Verificar tipo de usuario Interno u otro...
                //Interno LDAP externo u otros contra la BDD
                

                var usuario = db.Adscpassw.Where(x => x.AdpsLogin == login.Usuario).FirstOrDefault();

                if (usuario == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado
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

               var salida= CodificarHelper.SHA512(new Codificar { Entrada = login.Contrasena }).Salida;

                var existeLogin = db.Adscpassw.Where(x => x.AdpsLogin == login.Usuario && x.AdpsPasswPoint == salida).FirstOrDefault();
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
                        Message = Mensaje.ModeloInvalido,
                    };
                }

                var adscbdd = await db.Adscpassw.SingleOrDefaultAsync(m => m.AdpsLogin == id);

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

        [HttpPost]
        [Route("SeleccionarMiembroLogueado")]
        public async Task<Response> GetAdscPassws([FromBody] Adscpassw adscpassw)
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

            var adscgrpSeleccionado =await db.Adscpassw.Where(m => m.AdpsLoginAdm == adscpassw.AdpsLoginAdm && m.AdpsTokenTemp == adscpassw.AdpsTokenTemp).FirstOrDefaultAsync();

            return new Response {IsSuccess=true,Resultado=adscgrpSeleccionado };
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
                        Message = Mensaje.ModeloInvalido
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

        // PUT: api/Adscpassws/5
        [HttpPost]
        [Route("EliminarTokenTemp")]
        public async Task<Response> EliminarTokenTemp([FromBody] Adscpassw adscpassw)
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

                var adscPsswActualizar = await db.Adscpassw.Where(x => x.AdpsLogin == adscpassw.AdpsLogin).FirstOrDefaultAsync();
                if (adscPsswActualizar != null)
                {
                    try
                    {

                        adscPsswActualizar.AdpsTokenTemp = null;
                        db.Adscpassw.Update(adscPsswActualizar);
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

        [HttpPost]
        [Route("EliminarToken")]
        public async Task<Response> EliminarToken([FromBody] Adscpassw adscpassw)
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

                var adscPsswActualizar = await db.Adscpassw.Where(x => x.AdpsLogin == adscpassw.AdpsLogin).FirstOrDefaultAsync();
                if (adscPsswActualizar != null)
                {
                    try
                    {

                        adscPsswActualizar.AdpsToken = null;
                        db.Adscpassw.Update(adscPsswActualizar);
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
                        Message = Mensaje.ModeloInvalido
                    };
                }

                var respuesta = Existe(adscpassw);
                if (!respuesta.IsSuccess)
                {
                    adscpassw.AdpsFechaCambio = DateTime.Now;
                    adscpassw.AdpsFechaVencimiento = DateTime.Now.AddMonths(3);
                    adscpassw.AdpsIntentos = 0;
                    adscpassw.AdpsPasswCg = adscpassw.AdpsLogin;
                    adscpassw.AdpsPreguntaRecuperacion = Mensaje.UsuarioSinConfirmar;
                    adscpassw.AdpsRespuestaRecuperacion = Mensaje.UsuarioSinConfirmar;

                   
                    
                    adscpassw.AdpsPasswPoint = CodificarHelper.SHA512(new Codificar {Entrada=adscpassw.AdpsLogin }).Salida; 
                    db.Adscpassw.Add(adscpassw);
                    await db.SaveChangesAsync();
                    return new Response
                    {
                        IsSuccess = true,
                        Message = Mensaje.Satisfactorio,
                        Resultado=adscpassw,
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
                        Message = Mensaje.ModeloInvalido,
                    };
                }

                var respuesta = await db.Adscpassw.SingleOrDefaultAsync(m => m.AdpsLogin == id);
                if (respuesta == null)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = Mensaje.RegistroNoEncontrado,
                    };
                }
                db.Adscpassw.Remove(respuesta);
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

        public Response Existe(Adscpassw adscpassw)
        {
            var bdd = adscpassw.AdpsLogin.ToUpper().TrimEnd().TrimStart();
            var loglevelrespuesta = db.Adscpassw.Where(p => p.AdpsLogin.ToUpper().TrimStart().TrimEnd() == bdd).FirstOrDefault();
            if (loglevelrespuesta != null)
            {
                return new Response
                {
                    IsSuccess = true,
                    Message = Mensaje.Excepcion,
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