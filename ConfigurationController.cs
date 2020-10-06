using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace configuration_lab
{
    [Route("config")]
    [ApiController]
    public class ConfigurationController : Controller {
        private IConfiguration _configuration;

        public ConfigurationController(IConfiguration configuration) {
            _configuration = configuration;
        }

        [HttpGet]
        [HttpGet("connection-string")]
        public ActionResult<string> GetConnectionString()
        {
            string connection = _configuration.GetConnectionString("MyDbConnection");
            foreach (KeyValuePair<string, string> key in _configuration.AsEnumerable().ToList())
            {
                Console.WriteLine(key.Key + "  " + key.Value);
            }
            Console.WriteLine(connection);
            if (connection == null || connection.Length == 0) connection = "No connnection defined";
            return Ok(connection);
        }

    }

}