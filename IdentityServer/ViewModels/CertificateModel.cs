using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.ViewModels
{
    public class CertificateModel
    {
        public const string Certificate = "Certificate";
        
        public string Name { get; set; }

        public string Password { get; set; }
    }
}
