using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace openbanking.Controllers
{
    [Route("api/[controller]")]
    public class APIController : Controller
    {
        [HttpGet, Authorize]
        public IEnumerable<API> Get()
        {
            var currentUser = HttpContext.User;
            var resultAPIList = new API[] {
                new API { Author = "FUCK THIS", Title = "Hola Señor" },
                new API { Author = "FUCK YOU", Title = "Bonjour Señor" },
                new API { Author = "FUCK ME", Title = "Hello Señor" },
                new API { Author = "FUCK US", Title = "Hallo Señor" },
                new API { Author = "FUCK HER", Title = "Gutan Tag Señor" },
            };
            return resultAPIList;
        }
    }
    public class API
    {
        public string Author { get; set; }
        public string Title { get; set; }
        public bool AgeRestriction { get; set; }
    }
}