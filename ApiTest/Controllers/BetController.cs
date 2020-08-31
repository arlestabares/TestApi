using System;
using Microsoft.AspNetCore.Mvc;

namespace ApiTest.Controllers
{
    [ApiController]
    [Route("api")]
    public class BetController: ControllerBase
    {
        public string CreateRoulette()
        {
            return "Created succesfully";
        }
      
    }
}
