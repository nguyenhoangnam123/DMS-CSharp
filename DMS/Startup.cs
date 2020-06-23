using Common;
using DMS.Handlers;
using DMS.Helpers;
using DMS.Models;
using DMS.Rpc;
using DMS.Services;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Winton.Extensions.Configuration.Consul;
using Z.EntityFramework.Extensions;

namespace DMS
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

            if (env.EnvironmentName == "Production")
            {
                builder.AddConsul(
                $"{env.ApplicationName}/appsettings.Production.json",
                options =>
                {
                    options.ConsulConfigurationOptions =
                        cco => { cco.Address = new Uri("http://localhost:8500"); };
                    options.Optional = true;
                    options.ReloadOnChange = true;
                    options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                });
            }
            Configuration = builder.Build();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = DataEntity.ErrorResource;
            services.AddControllers().AddNewtonsoftJson();
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();
            services.AddSingleton<IRabbitManager, RabbitManager>();
            services.AddHostedService<ConsumeRabbitMQHostedService>();

            services.AddDbContext<DataContext>(options =>
              options.UseSqlServer(Configuration.GetConnectionString("DataContext")));
            EntityFrameworkManager.ContextFactory = context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
                DataContext DataContext = new DataContext(optionsBuilder.Options);
                return DataContext;
            };

            services.Scan(scan => scan
             .FromAssemblyOf<IServiceScoped>()
                 .AddClasses(classes => classes.AssignableTo<IServiceScoped>())
                     .AsImplementedInterfaces()
                     .WithScopedLifetime());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["Token"];
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKeyResolver = (token, secutiryToken, kid, validationParameters) =>
                    {
                        var secretKey = Configuration["Config:SecretKey"];
                        var key = Encoding.ASCII.GetBytes(secretKey);
                        SecurityKey issuerSigningKey = new SymmetricSecurityKey(key);
                        return new List<SecurityKey>() { issuerSigningKey };
                    }
                };
            });

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permission", policy =>
                    policy.Requirements.Add(new PermissionRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, SimpleHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Simple", policy =>
                    policy.Requirements.Add(new SimpleRequirement()));
            });

            Action onChange = () =>
            {
                InternalServices.UTILS = Configuration["InternalServices:UTILS"];
                InternalServices.ES = Configuration["InternalServices:ES"];
                RecurringJob.AddOrUpdate<MaintenanceService>("CleanHangfire", x => x.CleanHangfire(), Cron.Daily);
                RecurringJob.AddOrUpdate<MaintenanceService>("CleanEventMessage", x => x.CleanEventMessage(), Cron.Daily);
            };
            onChange();
            ChangeToken.OnChange(() => Configuration.GetReloadToken(), onChange);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "rpc/dms/swagger/{documentname}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/rpc/dms/swagger/v1/swagger.json", "dms API");
                c.RoutePrefix = "rpc/dms/swagger";
            });
            app.UseDeveloperExceptionPage();

        }
    }
}
