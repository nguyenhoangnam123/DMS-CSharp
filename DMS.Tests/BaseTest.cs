using Common;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Z.EntityFramework.Extensions;

namespace DMS.Tests
{
    public class BaseTest
    {
        public DataContext DataContext;
        protected ServiceProvider provider;
        protected ICurrentContext currentContext;

        public BaseTest()
        {
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();
            IHostEnvironment env = new HostingEnvironment();
            env.EnvironmentName = "Development";
            Startup startup = new Startup(env);
            IServiceCollection serviceCollection = new ServiceCollection();
            startup.ConfigureServices(serviceCollection);
            provider = serviceCollection.BuildServiceProvider();
            DataContext = provider.GetService<DataContext>();
            currentContext = provider.GetService<ICurrentContext>();

        }
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }
    }
}