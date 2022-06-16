using DotNetCore.CAP.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace DotNetCore.CAP.FurionDynamicApiControllerSupport
{
    /// <summary>
    /// Furion的DotNetCap扩展
    /// </summary>
    public static class DIExtension
    {
        /// <summary>
        /// 注入Furion动态控制器的Cap支持 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="filter"></param>
        public static void AddCapSupportOfDynamicController(this IServiceCollection services, string filter = "")
        {
            services.Add(new ServiceDescriptor(typeof(IConsumerServiceSelector),
                    x => new FurionDynamicControllerSelector(x, filter),
                    ServiceLifetime.Singleton));
        }
    }
}