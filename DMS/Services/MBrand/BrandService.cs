using Common;
using DMS.Entities;
using DMS.Repositories;
using Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.MBrand
{
    public interface IBrandService : IServiceScoped
    {
        Task<int> Count(BrandFilter BrandFilter);
        Task<List<Brand>> List(BrandFilter BrandFilter);
        Task<Brand> Get(long Id);
        Task<Brand> Create(Brand Brand);
        Task<Brand> Update(Brand Brand);
        Task<Brand> Delete(Brand Brand);
        Task<List<Brand>> BulkDelete(List<Brand> Brands);
        Task<List<Brand>> Import(DataFile DataFile);
        Task<DataFile> Export(BrandFilter BrandFilter);
        BrandFilter ToFilter(BrandFilter BrandFilter);
    }

    public class BrandService : BaseService, IBrandService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IBrandValidator BrandValidator;

        public BrandService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IBrandValidator BrandValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.BrandValidator = BrandValidator;
        }
        public async Task<int> Count(BrandFilter BrandFilter)
        {
            try
            {
                int result = await UOW.BrandRepository.Count(BrandFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Brand>> List(BrandFilter BrandFilter)
        {
            try
            {
                List<Brand> Brands = await UOW.BrandRepository.List(BrandFilter);
                return Brands;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Brand> Get(long Id)
        {
            Brand Brand = await UOW.BrandRepository.Get(Id);
            if (Brand == null)
                return null;
            return Brand;
        }

        public async Task<Brand> Create(Brand Brand)
        {
            if (!await BrandValidator.Create(Brand))
                return Brand;

            try
            {
                await UOW.Begin();
                await UOW.BrandRepository.Create(Brand);
                await UOW.Commit();

                await Logging.CreateAuditLog(Brand, new { }, nameof(BrandService));
                return await UOW.BrandRepository.Get(Brand.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Brand> Update(Brand Brand)
        {
            if (!await BrandValidator.Update(Brand))
                return Brand;
            try
            {
                var oldData = await UOW.BrandRepository.Get(Brand.Id);

                await UOW.Begin();
                await UOW.BrandRepository.Update(Brand);
                await UOW.Commit();

                var newData = await UOW.BrandRepository.Get(Brand.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(BrandService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Brand> Delete(Brand Brand)
        {
            if (!await BrandValidator.Delete(Brand))
                return Brand;

            try
            {
                await UOW.Begin();
                await UOW.BrandRepository.Delete(Brand);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Brand, nameof(BrandService));
                return Brand;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Brand>> BulkDelete(List<Brand> Brands)
        {
            if (!await BrandValidator.BulkDelete(Brands))
                return Brands;

            try
            {
                await UOW.Begin();
                await UOW.BrandRepository.BulkDelete(Brands);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Brands, nameof(BrandService));
                return Brands;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Brand>> Import(DataFile DataFile)
        {
            List<Brand> Brands = new List<Brand>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Brands;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    Brand Brand = new Brand();
                    Brand.Code = CodeValue;
                    Brand.Name = NameValue;
                    Brands.Add(Brand);
                }
            }

            if (!await BrandValidator.Import(Brands))
                return Brands;

            try
            {
                await UOW.Begin();
                await UOW.BrandRepository.BulkMerge(Brands);
                await UOW.Commit();

                await Logging.CreateAuditLog(Brands, new { }, nameof(BrandService));
                return Brands;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(BrandService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(BrandFilter BrandFilter)
        {
            List<Brand> Brands = await UOW.BrandRepository.List(BrandFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Brand);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Brand.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(Brand.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(Brand.Name);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Brand.StatusId);

                for (int i = 0; i < Brands.Count; i++)
                {
                    Brand Brand = Brands[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Brand.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Brand.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Brand.Name;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Brand.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Brand),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public BrandFilter ToFilter(BrandFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<BrandFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                BrandFilter subFilter = new BrandFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
