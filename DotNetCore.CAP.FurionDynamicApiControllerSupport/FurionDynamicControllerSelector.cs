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
    /// DotnetCoreCap的Furion的动态控制器选择器
    /// </summary>
    public class FurionDynamicControllerSelector : ConsumerServiceSelector
    {
        private readonly string _filter = "";
        public FurionDynamicControllerSelector(IServiceProvider serviceProvider, string filter = "") : base(serviceProvider)
        {
            _filter = filter;
        }
        private void GetReferanceAssemblies(Assembly assembly, List<Assembly> list)
        {
            var asses = _filter == String.Empty ? assembly.GetReferencedAssemblies().ToList() : assembly.GetReferencedAssemblies().Where(x => x.FullName.Contains(_filter)).ToList();
            asses.ForEach(item =>
            {
                try
                {
                    var ass = Assembly.Load(item);
                    if (!list.Contains(ass))
                    {
                        list.Add(ass);
                        GetReferanceAssemblies(ass, list);
                    }
                }
                catch { }
            });
        }
        protected override IEnumerable<ConsumerExecutorDescriptor> FindConsumersFromControllerTypes()
        {
            try
            {
                var executorDescriptorList = new List<ConsumerExecutorDescriptor>();

                var assemblyList = _filter == string.Empty ? AppDomain.CurrentDomain.GetAssemblies().ToList() : AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains(_filter)).ToList();

                var finalAssemblyList = new List<Assembly>();

                finalAssemblyList.AddRange(assemblyList);

                assemblyList.ForEach(assembly => GetReferanceAssemblies(assembly, finalAssemblyList));

                var ExportTypesList = finalAssemblyList
                    .Where(x => x.IsDynamic == false)
                    .SelectMany(x => x.ExportedTypes)
                    .ToList();

                return ExportTypesList
                    .Where(x => x.GetTypeInfo() != null && x.GetTypeInfo().ImplementedInterfaces != null && x.GetTypeInfo().ImplementedInterfaces.Any(it => it.Name.Contains("IDynamicApiController")))
                    .SelectMany(x => GetTopicAttributesDescription(x.GetTypeInfo()))
                    .ToList();
            }
            catch
            {
                return Enumerable.Empty<ConsumerExecutorDescriptor>();
            }
        }
    }
}