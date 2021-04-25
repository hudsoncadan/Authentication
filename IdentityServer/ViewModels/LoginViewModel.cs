using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.ViewModels
{
    public class LoginViewModel
    {
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public IEnumerable<AuthenticationScheme> ExternalProviders { get; set; }
    }
}