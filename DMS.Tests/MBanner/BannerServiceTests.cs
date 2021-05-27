using DMS.Common;
using DMS.Entities;
using DMS.Handlers;
using DMS.Helpers;
using DMS.Models;
using DMS.Repositories;
using DMS.Services.MBanner;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Tests.MBanner
{
    public class BannerServiceTests : BaseTests
    {
        private IBannerService BannerService;
        [SetUp]
        public void Setup()
        {
            Init();
           
            IBannerValidator BannerValidator = new BannerValidator(UOW, CurrentContext);
            TestImageService TestImageService = new TestImageService();
            BannerService = new BannerService(UOW, Logging, CurrentContext, BannerValidator, TestImageService);
        }

        [Test]
        public async Task Create_Without_Error()
        {
            // Arrange
            // Update 1 cái ảnh lên trước rồi tạo 1 cái banner theo cái ảnh được up
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

            // Act
            Banner Banner = new Banner
            {
                Content = "ABC",
                ImageId = 1,
                OrganizationId = 1,
                StatusId = 1,
                Priority = 1,
                Title = "ABC",
            };
            await BannerService.Create(Banner);
            // Assert
            BannerDAO bannerDAO = DataContext.Banner.Where(x => x.Id == 1).FirstOrDefault();
            Assert.AreNotEqual(null, bannerDAO);
            Assert.AreEqual("1", Banner.Code);

        }
    }
}
