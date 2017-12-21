using bd.swseguridad.entidades.Interfaces;
using bd.swseguridad.entidades.LDAP;
using bd.swseguridad.entidades.Utils;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;

namespace bd.swseguridad.entidades.Servicios
{
    public class LdapAuthenticationService: IAuthenticationService
    {
        private const string MemberOfAttribute = "memberOf";
        private const string DisplayNameAttribute = "displayName";
        private const string SAMAccountNameAttribute = "sAMAccountName";

        private readonly LdapConfig _config;
        private readonly LdapConnection _connection;

        public LdapAuthenticationService(IOptions<LdapConfig> config)
        {
            _config = config.Value;
            _connection = new LdapConnection
            {
                SecureSocketLayer = true
            };
        }

        public Response Login(string username, string password)
        {

            try
            {
                _connection.Connect(_config.Url, LdapConnection.DEFAULT_SSL_PORT);
                _connection.Bind(_config.BindDn, _config.BindCredentials);

                var searchFilter = string.Format(_config.SearchFilter, username);
                var result = _connection.Search(
                    _config.SearchBase,
                    LdapConnection.SCOPE_SUB,
                    searchFilter,
                    new[] { MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute },
                    false
                );

                var user = result.next();
                if (user != null)
                {
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
            catch
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
