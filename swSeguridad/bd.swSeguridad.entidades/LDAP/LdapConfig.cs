using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bd.swseguridad.entidades.LDAP
{
    public class LdapConfig
    {
        public string Url { get; set; }
        public string BindDn { get; set; }
        public string BindCredentials { get; set; }
        public string SearchBase { get; set; }
        public string SearchFilter { get; set; }
    }
}
