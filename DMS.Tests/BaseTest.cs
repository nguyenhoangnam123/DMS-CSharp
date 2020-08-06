using Common;
using DMS.Models;
using LightBDD.NUnit3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Threading.Tasks;
using Z.EntityFramework.Extensions;

namespace DMS.Tests
{
   
    public class BaseTest : FeatureFixture
    {
        public DataContext DataContext;
        protected ServiceProvider provider;
        protected ICurrentContext CurrentContext;

        public BaseTest()
        {
            var config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json")
             .Build();
            IHostEnvironment env = new HostingEnvironment();
            env.EnvironmentName = "Development";
            Startup startup = new Startup(env);
            IServiceCollection ServiceCollection = new ServiceCollection();
            startup.ConfigureServices(ServiceCollection);
            provider = ServiceCollection.BuildServiceProvider();
            DataContext = provider.GetService<DataContext>();
            CurrentContext = provider.GetService<ICurrentContext>();

        }
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

        }

       
        public async Task Clean()
        {

        }
    }
}