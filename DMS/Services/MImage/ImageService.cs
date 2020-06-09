using Common;
using DMS.Entities;
using DMS.Helpers;
using DMS.Repositories;
using Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services.MImage
{
    public interface IImageService : IServiceScoped
    {
        Task<List<Image>> List(ImageFilter ImageFilter);
        Task<Image> Get(long Id);
        Task<Image> Create(Image Image, string path);
        Task<Image> Delete(Image Image);
    }

    public class ImageService : BaseService, IImageService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IImageValidator ImageValidator;

        public ImageService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IImageValidator ImageValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ImageValidator = ImageValidator;
        }
        public async Task<int> Count(ImageFilter ImageFilter)
        {
            try
            {
                int result = await UOW.ImageRepository.Count(ImageFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ImageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Image>> List(ImageFilter ImageFilter)
        {
            try
            {
                List<Image> Images = await UOW.ImageRepository.List(ImageFilter);
                return Images;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ImageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Image> Get(long Id)
        {
            Image Image = await UOW.ImageRepository.Get(Id);
            if (Image == null)
                return null;
            return Image;
        }

        public async Task<Image> Delete(Image Image)
        {
            if (!await ImageValidator.Delete(Image))
                return Image;

            try
            {
                await UOW.Begin();
                await UOW.ImageRepository.Delete(Image);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Image, nameof(ImageService));
                return Image;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ImageService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }


        public async Task<Image> Create(Image Image, string path)
        {
            RestClient restClient = new RestClient($"http://localhost:{Modules.Utils}");
            RestRequest restRequest = new RestRequest("/rpc/utils/file/upload");
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.Method = Method.POST;
            restRequest.AddCookie("Token", CurrentContext.Token);
            restRequest.AddCookie("X-Language", CurrentContext.Language);
            restRequest.AddHeader("Content-Type", "multipart/form-data");
            restRequest.AddFile("file", Image.Content, Image.Name);
            restRequest.AddParameter("path", path);
            try
            {
                var response = restClient.Execute<File>(restRequest);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Image.Id = response.Data.Id;
                    Image.Url = "/rpc/utils/file/download" + response.Data.Path;
                    await UOW.Begin();
                    await UOW.ImageRepository.Create(Image);
                    await UOW.Commit();
                    return Image;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public class File
        {
            public long Id { get; set; }
            public string Path { get; set; }
        }
    }
}
