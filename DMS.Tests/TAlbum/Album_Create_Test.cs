using DMS.Rpc.album;
using DMS.Services.MAlbum;
using DMS.Services.MStatus;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.NUnit3;
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
            AlbumController = new AlbumController(StatusService, AlbumService, CurrentContext);
        }

        //[Scenario]
        //[Label("List Album")]
        //public async Task Valid_ReturnTrue()
        //{
        //    await Clean();
        //    await Runner.AddAsyncSteps(
        //        _ => Given_Admin_Context(),
        //        _ => Given_Permission(),
        //        _ => When_List_Action(Input),
        //        _ => Then_Assert_Valid_GoodsReceiptPO(resultList)
        //    ).RunAsync();
        //}

        
    }
}
