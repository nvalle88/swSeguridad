using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace bd.swseguridad.entidades.LDAP
{
    public class LdapUserManager<TUser> : UserManager<TUser> where TUser : class
    {
        private readonly IAuthenticationService<TUser> _ldapAuthService;
        public LdapUserManager(IUserStore<TUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<TUser> passwordHasher, 
            IEnumerable<IUserValidator<TUser>> userValidators, 
            IEnumerable<IPasswordValidator<TUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<TUser>> logger, 
            IAuthenticationService<TUser> ldapAuthService) 
                : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _ldapAuthService = ldapAuthService;
        }
        public override Task<TUser> FindByNameAsync(string userName)
        {
            var ldapUser = _ldapAuthService.FindByNameAsync(userName);
            if (ldapUser != null)
                return Task.FromResult(ldapUser);
            return base.FindByNameAsync(userName);
        }
    }
}
