using Consul;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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
            var service = GetService(consulAddress: Configuration["ConsulOption:ConsulAddress"], serviceName: Configuration["ConsulOption:ServiceName"]);
            return $@"CatalogService:{JsonConvert.SerializeObject(service)}";
        }

        /// <summary>
        /// 通过consul服务发现，随机返回服务信息
        /// </summary>
        /// <param name="consulAddress"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private CatalogService GetService(string consulAddress, string serviceName)
        {
            using (ConsulClient client = new ConsulClient((c) => { c.Address = new Uri(consulAddress); }))
            {
                var services = client.Catalog.Service(serviceName).Result.Response;
                int seed = Guid.NewGuid().GetHashCode();
                int serviceIndex = new Random(seed).Next(services.Length);
                var service = services.ElementAt(serviceIndex);
                return service;
            }
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
            var service = GetService(consulAddress: Configuration["ConsulOption:ConsulAddress"], serviceName: Configuration["ConsulOption:ServiceName"]);

            using (var client = new HttpClient())
            {
                Task<string> result = client.GetStringAsync($"http://{service.ServiceAddress}:{service.ServicePort}/api/ServiceFinder/GetName?name={name}");

                return $"{service.ServiceAddress}:{service.ServicePort} -> result :{result.Result}";
            }
        }
    }
}