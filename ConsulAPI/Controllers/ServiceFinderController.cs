using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace ConsulAPI.Controllers
{
    [ApiController]
    public class ServiceFinderController : ControllerBase
    {
        private IConfiguration Configuration;
        public ServiceFinderController(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        /// <summary>
        /// 返回服务发现相关信息
        /// </summary>
        /// <returns></returns>
        [Route("api/[controller]/Get"), HttpGet]
        public string Get()
        {
            var service = ConsulHelper.GetService(consulAddress: Configuration["ConsulOption:ConsulAddress"], serviceName: Configuration["ConsulOption:ServiceName"]);
            return $@"CatalogService:{JsonConvert.SerializeObject(service)}";
        }


        [Route("api/[controller]/GetName"), HttpGet]
        public string GetName(string name)
        {
            return name;
        }

        /// <summary>
        /// 通过服务发现，获取服务的ip+端口号，进行服务请求的模拟
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Route("api/[controller]/GetNameByService"), HttpGet]
        public string GetNameByService(string name)
        {
            var service = ConsulHelper.GetService(consulAddress: Configuration["ConsulOption:ConsulAddress"], serviceName: Configuration["ConsulOption:ServiceName"]);

            using (var client = new HttpClient())
            {
                Task<string> result = client.GetStringAsync($"http://{service.ServiceAddress}:{service.ServicePort}/api/ServiceFinder/GetName?name={name}");

                return $"{service.ServiceAddress}:{service.ServicePort} -> result :{result.Result}";
            }
        }

        
    }
}