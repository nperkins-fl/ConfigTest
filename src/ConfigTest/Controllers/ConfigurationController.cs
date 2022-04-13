using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ConfigTest.Controllers
{
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public List<KeyValuePair<string, string>> Get()
        {
            var values = _configuration.AsEnumerable().ToList();

            return values;
        }
    }
}
