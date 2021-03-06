using DotVVM.Framework.Configuration;
using DotVVM.Framework.ResourceManagement;
using DotVVM.Framework.Routing;
using GridViewGrouping.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace GridViewGrouping
{
    public class DotvvmStartup : IDotvvmStartup, IDotvvmServiceConfigurator
    {
        // For more information about this class, visit https://dotvvm.com/docs/tutorials/basics-project-structure
        public void Configure(DotvvmConfiguration config, string applicationPath)
        {

            ConfigureRoutes(config, applicationPath);
            ConfigureControls(config, applicationPath);
            ConfigureResources(config, applicationPath);
        }

        private void ConfigureRoutes(DotvvmConfiguration config, string applicationPath)
        {
            config.RouteTable.Add("Default", "", "Views/Default.dothtml");
            config.RouteTable.AutoDiscoverRoutes(new DefaultRouteStrategy(config));
        }

        private void ConfigureControls(DotvvmConfiguration config, string applicationPath)
        {
            config.Markup.AddCodeControls("cc", typeof(GroupingGridViewRowDecorator));
        }

        private void ConfigureResources(DotvvmConfiguration config, string applicationPath)
        {
            
        }


        public void ConfigureServices(IDotvvmServiceCollection options)
        {
            options.AddDefaultTempStorages("temp");
        }
    }
}
