using DMS.Rpc.album;
using DMS.Services.MAlbum;
using DMS.Services.MStatus;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DMS.Tests.TAlbum
{
    [TestFixture]
    public class Album_Create_Test : BaseTest
    {
        protected AlbumController AlbumController;
        [SetUp]
        public void SetUp()
        {
            var StatusService = provider.GetService<IStatusService>();
            var AlbumService = provider.GetService<IAlbumService>();
            AlbumController = new AlbumController(StatusService, AlbumService, currentContext);
        }

        [Test]
        public async Task FullData()
        {
            // arrange
            Album_AlbumDTO Album_AlbumDTO = new Album_AlbumDTO
            {
                Name = "ABC",
                StatusId = 1,
            };

            //act
            await AlbumController.Create(Album_AlbumDTO);

            //assert
        }
    }
}
