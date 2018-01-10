using bd.swseguridad.entidades.Interfaces;
using bd.swseguridad.entidades.LDAP;
using bd.swseguridad.entidades.Utils;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;

namespace bd.swseguridad.entidades.Servicios
{
    /// <summary>
    /// Servicio para la conexión al ldap 
    /// para más información visitar:https://www.novell.com/documentation/developer/jldap/jldapenu/api/com/novell/ldap/LDAPConnection.html
    /// </summary>
    public class LdapAuthenticationService: IAuthenticationService
    {
        /// <summary>
        /// Constantes para poder realizar la consulta al ldap
        /// </summary>
        private const string MemberOfAttribute = "memberOf";
        private const string DisplayNameAttribute = "displayName";
        private const string SAMAccountNameAttribute = "sAMAccountName";

        /// <summary>
        /// Variable donde está contenida la información de la configuración del ldap
        /// </summary>
        private readonly LdapConfig _config;

        /// <summary>
        /// clase para poder realizar acciones sobre el ldap
        /// </summary>
        private readonly LdapConnection _connection;

        /// <summary>
        /// constructor de la clase donde se configuran la conexión del ldap 
        /// y se lee la configuración del ldap
        /// </summary>
        /// <param name="config"></param>
        public LdapAuthenticationService(IOptions<LdapConfig> config)
        {
            /// <summary>
            /// Se lee la configuración del ldap del json y se añade por inyección de dependencia
            /// </summary>
            _config = config.Value;
            /// <summary>
            /// se inicializa el servicio del que manipula el ldap
            /// </summary>
            _connection = new LdapConnection
            {
                
                SecureSocketLayer = false
            };
        }

        public Response Login(string username, string password)
        {

            try
            {
                /// <summary>
                /// se conecta al ldap con lo que se lee del servicio _config
                /// </summary>
                _connection.Connect(_config.Url, _config.Port);

                /// <summary>
                /// se añaden la autenticación de un ususario con permisos para posteriormente 
                /// poder relizar consultas 
                /// </summary>
                _connection.Bind(_config.BindDn, _config.BindCredentials);

                /// <summary>
                /// Se busca si el usuario (username) existe en el ldap
                /// </summary>
                var searchFilter = string.Format(_config.SearchFilter, username);
                var result = _connection.Search(
                    _config.SearchBase,
                    LdapConnection.SCOPE_SUB,
                    searchFilter,
                    new[] { MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute },
                    false
                );

                /// <summary>
                /// se obtiene el usuario que se ha buscado en caso de que el usuario no exista se 
                /// retorna null , si existe se verifica la contraseña del ususario contra el ldap
                /// 
                /// </summary>
                var user = result.next();
                if (user != null)
                {
                    /// <summary>
                    /// se verifica la contraseña del usuario contra el ldap
                    /// </summary>
                    _connection.Bind(user.DN, password);
                    if (_connection.Bound)
                    {

                        return new Response
                        {
                            IsSuccess =true,Resultado=new UsuarioLDAP
                                {
                                    DisplayName = user.getAttribute(DisplayNameAttribute).StringValue,
                                    Username = user.getAttribute(SAMAccountNameAttribute).StringValue,
                                }
                        };
                       
                        
                    }
                }
            }
            catch(Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                };
            }
            _connection.Disconnect();
            return null;
        }

    }
}
