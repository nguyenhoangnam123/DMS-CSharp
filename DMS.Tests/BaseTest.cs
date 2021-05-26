using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using DMS.Models;
using Z.EntityFramework.Extensions;
using Thinktecture;
using DMS.Handlers;
using DMS.Common;
using DMS.Services.MImage;
using DMS.Repositories;
using DMS.Helpers;
using System.Threading.Tasks;
using DMS.Entities;

namespace DMS.Tests
{
    public class BaseTests
    {
        private readonly DbConnection _connection;
        public DataContext DataContext;
        public IUOW UOW;
        public ICurrentContext CurrentContext;
        public IRabbitManager RabbitManager;
        public ILogging Logging;
        public void Init()
        {
            LicenseManager.AddLicense("2456;100-FPT", "3f0586d1-0216-5005-8b7a-9080b0bedb5e");
            string licenseErrorMessage;
            if (!LicenseManager.ValidateLicense(out licenseErrorMessage))
            {
                throw new Exception(licenseErrorMessage);
            }


            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(CreateInMemoryDatabase(), sqlOptions =>
                {
                    sqlOptions.AddTempTableSupport();
                })
                .Options;
            DataContext = new DataContext(options);
            DataContext.Database.EnsureDeleted();
            DataContext.Database.EnsureCreated();
            EntityFrameworkManager.ContextFactory = DbContext => new DataContext(options);

            StatusDAO active = new StatusDAO
            {
                Id = 1,
                Code = "ACTIVE",
                Name = "Hoạt động"
            };
            DataContext.Status.Add(active);
            SexDAO sexDAO = new SexDAO
            {
                Id = 1,
                Code = "Male",
                Name = "Nam"
            };
            DataContext.Sex.Add(sexDAO);

            OrganizationDAO organizationDAO = new OrganizationDAO
            {
                Code = "ORG",
                Name = "ORG",
                Address = "ORG",
                CreatedAt = StaticParams.DateTimeNow,
                UpdatedAt = StaticParams.DateTimeNow,
                Id = 1,
                IsDisplay = true,
                Level = 1,
                Path = "1.",
                RowId = Guid.NewGuid(),
                StatusId = 1,
            };
            DataContext.Organization.Add(organizationDAO);

            AppUserDAO Admin = new AppUserDAO
            {
                Id = 1,
                Username = "Administrator",
                DisplayName = "Administrator",
                CreatedAt = StaticParams.DateTimeNow,
                UpdatedAt = StaticParams.DateTimeNow,
                GPSUpdatedAt = StaticParams.DateTimeNow,
                OrganizationId = 1,
                SexId = 1,
                StatusId = 1,
                Address = "123",
            };
            DataContext.AppUser.Add(Admin);

            ImageDAO ImageDAO = new ImageDAO
            {
                Id = 1,
                Name = "ABC",
                ThumbnailUrl = "PATH",
                Url = "PATH",
                CreatedAt = StaticParams.DateTimeNow,
                UpdatedAt = StaticParams.DateTimeNow,
            };
            DataContext.Image.Add(ImageDAO);

            DataContext.SaveChanges();

            CurrentContext = new CurrentContext
            {
                Latitude = 10,
                Longitude = 10,
                Language = "vi",
                TimeZone = 7,
                UserId = 1,
                UserName = "Administrator"
            };

            

            UOW = new UOW(DataContext);
            RabbitManager = new TestRabbitManager();
            Logging = new Logging(CurrentContext, RabbitManager);
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Data Source=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();
    }

    public class TestRabbitManager : IRabbitManager
    {
        public void PublishList<T>(List<T> message, GenericEnum routeKey) where T : DataEntity
        {
        }

        public void PublishSingle<T>(T message, GenericEnum routeKey) where T : DataEntity
        {
        }
    }

    public class TestImageService: IImageService
    {
        public TestImageService()
        {

        }


        public async Task<Image> Create(Image Image, string path, string thumbnailPath, int width, int height)
        {
            Image =  new Image
            {
                Id = 1,
                Name = "ABC",
                ThumbnailUrl = "PATH",
                Url = "PATH"
            };
            return Image;
        }

        public async Task<Image> Create(Image Image, string path)
        {
            return new Image
            {
                Id = 1,
                Name = "ABC",
                ThumbnailUrl = "PATH",
                Url = "PATH"
            };
        }

        public async Task<Image> Delete(Image Image)
        {
            return new Image
            {
                Id = 1,
                Name = "ABC",
                ThumbnailUrl = "PATH",
                Url = "PATH"
            };
        }

        public async Task<Image> Get(long Id)
        {
            return new Image
            {
                Id = 1,
                Name = "ABC",
                ThumbnailUrl = "PATH",
                Url = "PATH"
            };
        }

        public async Task<List<Image>> List(ImageFilter ImageFilter)
        {
            Image Image =new Image
            {
                Id = 1,
                Name = "ABC",
                ThumbnailUrl = "PATH",
                Url = "PATH"
            };

            return new List<Image> { Image };
        }
    }
}
