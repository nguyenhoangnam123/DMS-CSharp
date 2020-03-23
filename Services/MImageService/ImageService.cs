using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using DMS.Repositories;
using DMS.Entities;

namespace DMS.Services.MImage
{
    public interface IImageService :  IServiceScoped
    {
        Task<int> Count(ImageFilter ImageFilter);
        Task<List<Image>> List(ImageFilter ImageFilter);
        Task<Image> Get(long Id);
        Task<Image> Create(Image Image);
        Task<Image> Update(Image Image);
        Task<Image> Delete(Image Image);
        Task<List<Image>> BulkDelete(List<Image> Images);
        Task<List<Image>> Import(DataFile DataFile);
        Task<DataFile> Export(ImageFilter ImageFilter);
        ImageFilter ToFilter(ImageFilter ImageFilter);
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
       
        public async Task<Image> Create(Image Image)
        {
            if (!await ImageValidator.Create(Image))
                return Image;

            try
            {
                await UOW.Begin();
                await UOW.ImageRepository.Create(Image);
                await UOW.Commit();

                await Logging.CreateAuditLog(Image, new { }, nameof(ImageService));
                return await UOW.ImageRepository.Get(Image.Id);
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

        public async Task<Image> Update(Image Image)
        {
            if (!await ImageValidator.Update(Image))
                return Image;
            try
            {
                var oldData = await UOW.ImageRepository.Get(Image.Id);

                await UOW.Begin();
                await UOW.ImageRepository.Update(Image);
                await UOW.Commit();

                var newData = await UOW.ImageRepository.Get(Image.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ImageService));
                return newData;
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

        public async Task<List<Image>> BulkDelete(List<Image> Images)
        {
            if (!await ImageValidator.BulkDelete(Images))
                return Images;

            try
            {
                await UOW.Begin();
                await UOW.ImageRepository.BulkDelete(Images);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Images, nameof(ImageService));
                return Images;
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
        
        public async Task<List<Image>> Import(DataFile DataFile)
        {
            List<Image> Images = new List<Image>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Images;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int UrlColumn = 2 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string UrlValue = worksheet.Cells[i + StartRow, UrlColumn].Value?.ToString();
                    Image Image = new Image();
                    Image.Name = NameValue;
                    Image.Url = UrlValue;
                    Images.Add(Image);
                }
            }
            
            if (!await ImageValidator.Import(Images))
                return Images;
            
            try
            {
                await UOW.Begin();
                await UOW.ImageRepository.BulkMerge(Images);
                await UOW.Commit();

                await Logging.CreateAuditLog(Images, new { }, nameof(ImageService));
                return Images;
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

        public async Task<DataFile> Export(ImageFilter ImageFilter)
        {
            List<Image> Images = await UOW.ImageRepository.List(ImageFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Image);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int UrlColumn = 2 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(Image.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Image.Name);
                worksheet.Cells[1, UrlColumn].Value = nameof(Image.Url);

                for(int i = 0; i < Images.Count; i++)
                {
                    Image Image = Images[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Image.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Image.Name;
                    worksheet.Cells[i + StartRow, UrlColumn].Value = Image.Url;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Image),
                Content = MemoryStream,
            };
            return DataFile;
        }
        
        public ImageFilter ToFilter(ImageFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ImageFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ImageFilter subFilter = new ImageFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Url))
                    subFilter.Url = Map(subFilter.Url, currentFilter.Value);
            }
            return filter;
        }
    }
}
