using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginFlowInMauiBlazorApp.Models
{
    public class AuthenticateRequestAndResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
