using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;

namespace bd.swseguridad.entidades.LDAP
{
    public class LdapAuthenticationService<T> : IAuthenticationService<T> where T : class
    {
        private const string MemberOfAttribute = "memberOf";
        private const string DisplayNameAttribute = "displayName";
        private const string SAMAccountNameAttribute = "sAMAccountName";
        private const string EmailAttribute = "mail";

        private readonly LdapConfig _config;
        private readonly LdapConnection _connection;

        private readonly ILogger _logger;

        public LdapAuthenticationService(IOptions<LdapConfig> config,
            ILoggerFactory loggerFactory)
        {
            _config = config.Value;
            _connection = new LdapConnection
            {
                //SecureSocketLayer = true
            };
            _logger = loggerFactory.CreateLogger<LdapAuthenticationService<T>>();
        }

        public T Login(string username, string password)
        {
            try
            {
                _connection.Connect(_config.Url, LdapConnection.DEFAULT_PORT); //also LdapConnection.DEFAULT_SSL_PORT can be used
            }
            catch (Exception ex)
            {
                _logger.LogError(10, ex, "Cannot connect to LDAP Server. Verify server or port.");
                return default(T);
            }
            try
            {
                _connection.Bind(_config.BindDn, _config.BindCredentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(10, ex, "Cannot Bind with provided Authentication to LDAP Server. Verify credentials.");
                return default(T);
            }

            var searchFilter = string.Format(_config.SearchFilter, username);
            var result = _connection.Search(
                _config.SearchBase,
                LdapConnection.SCOPE_SUB,
                searchFilter,
                new[] { MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute, EmailAttribute },
                false
            );

            try
            {
                var user = result.next();
                if (user != null)
                {
                    _connection.Bind(user.DN, password);
                    if (_connection.Bound)
                    {
                        var userFirstName = user.getAttribute(DisplayNameAttribute)?.StringValue ?? username;
                        var email = user.getAttribute(EmailAttribute)?.StringValue;
                        var roles = CommaSeparatedRolesForUser(user.getAttribute(MemberOfAttribute)?.StringValueArray);
                        return Activator.CreateInstance(typeof(T), new object[] { username, email, userFirstName, roles }) as T;
                    }
                }
            }
            catch
            {
                return default(T);
            }
            _connection.Disconnect();
            return default(T);
        }

        private string CommaSeparatedRolesForUser(string[] stringValueArray)
        {
            if (stringValueArray == null)
                return null;

            var roles = new List<string>(stringValueArray.Length);

            for (int i = 0; i < stringValueArray.Length; i++)
            {
                var searchBase = _config.SearchBase.ToLowerInvariant();
                if (stringValueArray[i].ToLowerInvariant().Contains(searchBase))
                {
                    //candidate Role found
                    var CNs = stringValueArray[i].Split(',');
                    if(CNs.Length > 0)
                    {
                        roles.Add(CNs[0].Substring(3));
                    }
                }
            }

            return string.Join(",", roles);
        }

        public T FindByNameAsync(string userName)
        {
            try
            {
                _connection.Connect(_config.Url, LdapConnection.DEFAULT_PORT); //also LdapConnection.DEFAULT_SSL_PORT can be used
            }
            catch (Exception ex)
            {
                _logger.LogError(10, ex, "Cannot connect to LDAP Server. Verify server or port.");
                return default(T);
            }
            try
            {
                _connection.Bind(_config.BindDn, _config.BindCredentials);
            }
            catch (Exception ex)
            {
                _logger.LogError(10, ex, "Cannot Bind with provided Authentication to LDAP Server. Verify credentials.");
                return default(T);
            }
            var searchFilter = string.Format(_config.SearchFilter, userName);
            var result = _connection.Search(
                _config.SearchBase,
                LdapConnection.SCOPE_SUB,
                searchFilter,
                new[] { MemberOfAttribute, DisplayNameAttribute, SAMAccountNameAttribute, EmailAttribute },
                false
            );
            try
            {
                var user = result.next();
                if (user != null)
                {
                    var userFirstName = user.getAttribute(DisplayNameAttribute)?.StringValue ?? userName;
                    var email = user.getAttribute(EmailAttribute)?.StringValue;
                    return Activator.CreateInstance(typeof(T), new object[] { userName, email, userFirstName, null }) as T;
                }
            }
            catch
            {
                return default(T);
            }
            _connection.Disconnect();
            return default(T);
        }
    }
}
