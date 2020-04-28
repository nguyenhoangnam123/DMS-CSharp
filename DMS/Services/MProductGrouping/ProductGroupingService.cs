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

namespace DMS.Services.MProductGrouping
{
    public interface IProductGroupingService : IServiceScoped
    {
        Task<int> Count(ProductGroupingFilter ProductGroupingFilter);
        Task<List<ProductGrouping>> List(ProductGroupingFilter ProductGroupingFilter);
        Task<ProductGrouping> Get(long Id);
        Task<ProductGrouping> Create(ProductGrouping ProductGrouping);
        Task<ProductGrouping> Update(ProductGrouping ProductGrouping);
        Task<ProductGrouping> Delete(ProductGrouping ProductGrouping);
        Task<List<ProductGrouping>> BulkDelete(List<ProductGrouping> ProductGroupings);
        Task<List<ProductGrouping>> Import(DataFile DataFile);
        Task<DataFile> Export(ProductGroupingFilter ProductGroupingFilter);
        ProductGroupingFilter ToFilter(ProductGroupingFilter ProductGroupingFilter);
    }

    public class ProductGroupingService : BaseService, IProductGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IProductGroupingValidator ProductGroupingValidator;

        public ProductGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IProductGroupingValidator ProductGroupingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.ProductGroupingValidator = ProductGroupingValidator;
        }
        public async Task<int> Count(ProductGroupingFilter ProductGroupingFilter)
        {
            try
            {
                int result = await UOW.ProductGroupingRepository.Count(ProductGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ProductGrouping>> List(ProductGroupingFilter ProductGroupingFilter)
        {
            try
            {
                List<ProductGrouping> ProductGroupings = await UOW.ProductGroupingRepository.List(ProductGroupingFilter);
                return ProductGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<ProductGrouping> Get(long Id)
        {
            ProductGrouping ProductGrouping = await UOW.ProductGroupingRepository.Get(Id);
            if (ProductGrouping == null)
                return null;
            return ProductGrouping;
        }

        public async Task<ProductGrouping> Create(ProductGrouping ProductGrouping)
        {
            if (!await ProductGroupingValidator.Create(ProductGrouping))
                return ProductGrouping;

            try
            {
                await UOW.Begin();
                await UOW.ProductGroupingRepository.Create(ProductGrouping);
                await UOW.Commit();

                await Logging.CreateAuditLog(ProductGrouping, new { }, nameof(ProductGroupingService));
                return await UOW.ProductGroupingRepository.Get(ProductGrouping.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ProductGrouping> Update(ProductGrouping ProductGrouping)
        {
            if (!await ProductGroupingValidator.Update(ProductGrouping))
                return ProductGrouping;
            try
            {
                var oldData = await UOW.ProductGroupingRepository.Get(ProductGrouping.Id);

                await UOW.Begin();
                await UOW.ProductGroupingRepository.Update(ProductGrouping);
                await UOW.Commit();

                var newData = await UOW.ProductGroupingRepository.Get(ProductGrouping.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(ProductGroupingService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<ProductGrouping> Delete(ProductGrouping ProductGrouping)
        {
            if (!await ProductGroupingValidator.Delete(ProductGrouping))
                return ProductGrouping;

            try
            {
                await UOW.Begin();
                await UOW.ProductGroupingRepository.Delete(ProductGrouping);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ProductGrouping, nameof(ProductGroupingService));
                return ProductGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ProductGrouping>> BulkDelete(List<ProductGrouping> ProductGroupings)
        {
            if (!await ProductGroupingValidator.BulkDelete(ProductGroupings))
                return ProductGroupings;

            try
            {
                await UOW.Begin();
                await UOW.ProductGroupingRepository.BulkDelete(ProductGroupings);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, ProductGroupings, nameof(ProductGroupingService));
                return ProductGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<ProductGrouping>> Import(DataFile DataFile)
        {
            List<ProductGrouping> ProductGroupings = new List<ProductGrouping>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return ProductGroupings;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ParentIdColumn = 3 + StartColumn;
                int PathColumn = 4 + StartColumn;
                int DescriptionColumn = 5 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ParentIdValue = worksheet.Cells[i + StartRow, ParentIdColumn].Value?.ToString();
                    string PathValue = worksheet.Cells[i + StartRow, PathColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();
                    ProductGrouping ProductGrouping = new ProductGrouping();
                    ProductGrouping.Code = CodeValue;
                    ProductGrouping.Name = NameValue;
                    ProductGrouping.Path = PathValue;
                    ProductGrouping.Description = DescriptionValue;
                    ProductGroupings.Add(ProductGrouping);
                }
            }

            if (!await ProductGroupingValidator.Import(ProductGroupings))
                return ProductGroupings;

            try
            {
                await UOW.Begin();
                await UOW.ProductGroupingRepository.BulkMerge(ProductGroupings);
                await UOW.Commit();

                await Logging.CreateAuditLog(ProductGroupings, new { }, nameof(ProductGroupingService));
                return ProductGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(ProductGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(ProductGroupingFilter ProductGroupingFilter)
        {
            List<ProductGrouping> ProductGroupings = await UOW.ProductGroupingRepository.List(ProductGroupingFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(ProductGrouping);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ParentIdColumn = 3 + StartColumn;
                int PathColumn = 4 + StartColumn;
                int DescriptionColumn = 5 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(ProductGrouping.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(ProductGrouping.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(ProductGrouping.Name);
                worksheet.Cells[1, ParentIdColumn].Value = nameof(ProductGrouping.ParentId);
                worksheet.Cells[1, PathColumn].Value = nameof(ProductGrouping.Path);
                worksheet.Cells[1, DescriptionColumn].Value = nameof(ProductGrouping.Description);

                for (int i = 0; i < ProductGroupings.Count; i++)
                {
                    ProductGrouping ProductGrouping = ProductGroupings[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = ProductGrouping.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = ProductGrouping.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = ProductGrouping.Name;
                    worksheet.Cells[i + StartRow, ParentIdColumn].Value = ProductGrouping.ParentId;
                    worksheet.Cells[i + StartRow, PathColumn].Value = ProductGrouping.Path;
                    worksheet.Cells[i + StartRow, DescriptionColumn].Value = ProductGrouping.Description;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(ProductGrouping),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public ProductGroupingFilter ToFilter(ProductGroupingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<ProductGroupingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                ProductGroupingFilter subFilter = new ProductGroupingFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = Map(subFilter.Id, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Code))
                        subFilter.Code = Map(subFilter.Code, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Name))
                        subFilter.Name = Map(subFilter.Name, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Description))
                        subFilter.Description = Map(subFilter.Description, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.ParentId))
                        subFilter.ParentId = Map(subFilter.ParentId, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Path))
                        subFilter.Path = Map(subFilter.Path, FilterPermissionDefinition);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Level))
                        subFilter.Level = Map(subFilter.Level, FilterPermissionDefinition);
                }
            }
            return filter;
        }
    }
}
