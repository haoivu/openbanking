using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace openbanking.Models
{
    public class AccessTokenModel
    {
        public string AccessToken { get; set; }
        public long ExpiresIn { get; set; }
        public string TokenType { get; set; }
    }
}