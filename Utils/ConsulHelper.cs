using Consul;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class ConsulHelper
    {
        /// <summary>
        /// 通过consul服务发现，随机返回服务信息
        /// </summary>
        /// <param name="consulAddress">consul client 地址</param>
        /// <param name="serviceName">服务名称</param>
        /// <returns></returns>
        public static CatalogService GetService(string consulAddress, string serviceName)
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

        /// <summary>
        /// 异步设置kv
        /// </summary>
        /// <param name="consulAddress">consul client 地址</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<WriteResult<bool>> SetKeyValueAsync(string consulAddress, string key, string value)
        {
            using (var client = new ConsulClient((x) => { x.Address = new Uri(consulAddress); }))
            {
                var kv = new KVPair(key) { Value = Encoding.UTF8.GetBytes(value) };
                return await client.KV.Put(kv);
            }
        }

        /// <summary>
        /// 设置kv
        /// </summary>
        /// <param name="consulAddress">consul client 地址</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetKeyValue(string consulAddress, string key, string value)
        {
            Task<WriteResult<bool>> result = SetKeyValueAsync(consulAddress, key, value);

            return result.GetAwaiter().GetResult().Response;
        }

        /// <summary>
        /// 异步获取key对应的value
        /// </summary>
        /// <param name="consulAddress"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<QueryResult<KVPair>> GetValueAsync(string consulAddress, string key)
        {
            using (var client = new ConsulClient((x) => { x.Address = new Uri(consulAddress); }))
            {
                return await client.KV.Get(key);
            }
        }

        /// <summary>
        /// 获取key对应的value
        /// </summary>
        /// <param name="consulAddress"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(string consulAddress, string key)
        {
            Task<QueryResult<KVPair>> result = GetValueAsync(consulAddress, key);
            byte[] bytes = result.GetAwaiter().GetResult().Response.Value;

            return Encoding.UTF8.GetString(bytes);
        }
    }
}
