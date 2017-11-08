namespace bd.swseguridad.entidades.LDAP
{
    public interface IAuthenticationService<T>
    {
        T Login(string username, string password);
        T FindByNameAsync(string userName);
    }
}
