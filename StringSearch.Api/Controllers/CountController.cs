using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StringSearch.Api.ViewModels;

namespace StringSearch.Api.Controllers
{
    [Route("api/Count")]
    public class CountController
    {
        public async Task<IActionResult> Index(VmCountRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
