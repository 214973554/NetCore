using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Utils;

namespace ConsulAPI.Controllers
{

    [ApiController]
    public class KVController : ControllerBase
    {
        private IConfiguration Configuration;
        private string consulAddress;

        public KVController(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.consulAddress = Configuration["ConsulOption:ConsulAddress"];
        }

        [Route("api/[controller]/SetKV/{key}/{value}")]
        public bool SetKV(string key, string value)
        {
            return ConsulHelper.SetKeyValue(consulAddress, key, value);
        }

        [Route("api/[controller]/GetValue/{key}")]
        public string GetValue(string key)
        {
            return ConsulHelper.GetValue(consulAddress, key);
        }
    }
}