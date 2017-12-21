using bd.swseguridad.entidades.LDAP;
using bd.swseguridad.entidades.Utils;

namespace bd.swseguridad.entidades.Interfaces
{
    public interface IAuthenticationService
    {
        Response Login(string username, string password);
    }
}
