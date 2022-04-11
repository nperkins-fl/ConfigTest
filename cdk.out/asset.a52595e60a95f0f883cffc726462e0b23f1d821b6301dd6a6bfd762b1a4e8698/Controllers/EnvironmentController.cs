using System;
using System.Collections;
using Microsoft.AspNetCore.Mvc;

namespace ConfigTest.Controllers
{
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase
    {
        [HttpGet]
        public IDictionary Get()
        {
            var variables = Environment.GetEnvironmentVariables();

            return variables;
        }
    }
}
