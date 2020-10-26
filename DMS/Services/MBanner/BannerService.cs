using DMS.Common;
using DMS.Entities;
using DMS.Repositories;
using DMS.Services.MImage;
using DMS.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
//using SixLabors.ImageSharp;

namespace DMS.Services.MBanner
{
    public interface IBannerService : IServiceScoped
    {
        Task<int> Count(BannerFilter BannerFilter);
        Task<List<Banner>> List(BannerFilter BannerFilter);
        Task<Banner> Get(long Id);
        Task<Banner> Create(Banner Banner);
        Task<Banner> Update(Banner Banner);
        Task<Banner> Delete(Banner Banner);
        Task<List<Banner>> BulkDelete(List<Banner> Banners);
        Task<Image> SaveImage(Image Image);
        Task<List<Banner>> Import(List<Banner> Banners);
        BannerFilter ToFilter(BannerFilter BannerFilter);
    }

    public class BannerService : BaseService, IBannerService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IBannerValidator BannerValidator;
        private IImageService ImageService;
        public BannerService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IBannerValidator BannerValidator,
            IImageService ImageService
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.BannerValidator = BannerValidator;
            this.ImageService = ImageService;
        }
        public async Task<int> Count(BannerFilter BannerFilter)
        {
            try
            {
                int result = await UOW.BannerRepository.Count(BannerFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BannerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BannerService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Banner>> List(BannerFilter BannerFilter)
        {
            try
            {
                List<Banner> Banners = await UOW.BannerRepository.List(BannerFilter);
                return Banners;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BannerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BannerService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<Banner> Get(long Id)
        {
            Banner Banner = await UOW.BannerRepository.Get(Id);
            if (Banner == null)
                return null;
            return Banner;
        }

        public async Task<Banner> Create(Banner Banner)
        {
            if (!await BannerValidator.Create(Banner))
                return Banner;

            try
            {
                var Code = await UOW.BannerRepository.Count(new BannerFilter { });
                Banner.Code = (Code + 1).ToString();
                Banner.CreatorId = CurrentContext.UserId;
                await UOW.Begin();
                await UOW.BannerRepository.Create(Banner);
                await UOW.Commit();

                await Logging.CreateAuditLog(Banner, new { }, nameof(BannerService));
                return await UOW.BannerRepository.Get(Banner.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BannerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BannerService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Banner> Update(Banner Banner)
        {
            if (!await BannerValidator.Update(Banner))
                return Banner;
            try
            {
                var oldData = await UOW.BannerRepository.Get(Banner.Id);

                await UOW.Begin();
                await UOW.BannerRepository.Update(Banner);
                await UOW.Commit();

                var newData = await UOW.BannerRepository.Get(Banner.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(BannerService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BannerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BannerService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Banner> Delete(Banner Banner)
        {
            if (!await BannerValidator.Delete(Banner))
                return Banner;

            try
            {
                await UOW.Begin();
                await UOW.BannerRepository.Delete(Banner);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Banner, nameof(BannerService));
                return Banner;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BannerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BannerService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Banner>> BulkDelete(List<Banner> Banners)
        {
            if (!await BannerValidator.BulkDelete(Banners))
                return Banners;

            try
            {
                await UOW.Begin();
                await UOW.BannerRepository.BulkDelete(Banners);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Banners, nameof(BannerService));
                return Banners;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BannerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BannerService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<Banner>> Import(List<Banner> Banners)
        {
            if (!await BannerValidator.Import(Banners))
                return Banners;
            try
            {
                await UOW.Begin();
                await UOW.BannerRepository.BulkMerge(Banners);
                await UOW.Commit();

                await Logging.CreateAuditLog(Banners, new { }, nameof(BannerService));
                return Banners;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(BannerService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(BannerService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<Image> SaveImage(Image Image)
        {
            FileInfo fileInfo = new FileInfo(Image.Name);
            string path = $"/banner/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{fileInfo.Extension}";
            //using (var image = SixLabors.ImageSharp.Image.Load(Image.Content))
            //{
            //    var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "foo.png");

            //    image.Clone(ctx => ctx.Crop(560, 300)).Save(path);
            //}
            Image = await ImageService.Create(Image, path);
            return Image;
        }

        public BannerFilter ToFilter(BannerFilter filter)
        {

            return filter;
        }
    }
}
