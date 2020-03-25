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

namespace DMS.Services.MUnitOfMeasureGroupingContent
{
    public interface IUnitOfMeasureGroupingContentService : IServiceScoped
    {
        Task<int> Count(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter);
        Task<List<UnitOfMeasureGroupingContent>> List(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter);
        Task<UnitOfMeasureGroupingContent> Get(long Id);
        Task<UnitOfMeasureGroupingContent> Create(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<UnitOfMeasureGroupingContent> Update(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<UnitOfMeasureGroupingContent> Delete(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent);
        Task<List<UnitOfMeasureGroupingContent>> BulkDelete(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents);
        Task<List<UnitOfMeasureGroupingContent>> Import(DataFile DataFile);
        Task<DataFile> Export(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter);
        UnitOfMeasureGroupingContentFilter ToFilter(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter);
    }

    public class UnitOfMeasureGroupingContentService : BaseService, IUnitOfMeasureGroupingContentService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IUnitOfMeasureGroupingContentValidator UnitOfMeasureGroupingContentValidator;

        public UnitOfMeasureGroupingContentService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IUnitOfMeasureGroupingContentValidator UnitOfMeasureGroupingContentValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.UnitOfMeasureGroupingContentValidator = UnitOfMeasureGroupingContentValidator;
        }
        public async Task<int> Count(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter)
        {
            try
            {
                int result = await UOW.UnitOfMeasureGroupingContentRepository.Count(UnitOfMeasureGroupingContentFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasureGroupingContent>> List(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter)
        {
            try
            {
                List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await UOW.UnitOfMeasureGroupingContentRepository.List(UnitOfMeasureGroupingContentFilter);
                return UnitOfMeasureGroupingContents;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<UnitOfMeasureGroupingContent> Get(long Id)
        {
            UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = await UOW.UnitOfMeasureGroupingContentRepository.Get(Id);
            if (UnitOfMeasureGroupingContent == null)
                return null;
            return UnitOfMeasureGroupingContent;
        }

        public async Task<UnitOfMeasureGroupingContent> Create(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            if (!await UnitOfMeasureGroupingContentValidator.Create(UnitOfMeasureGroupingContent))
                return UnitOfMeasureGroupingContent;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingContentRepository.Create(UnitOfMeasureGroupingContent);
                await UOW.Commit();

                await Logging.CreateAuditLog(UnitOfMeasureGroupingContent, new { }, nameof(UnitOfMeasureGroupingContentService));
                return await UOW.UnitOfMeasureGroupingContentRepository.Get(UnitOfMeasureGroupingContent.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<UnitOfMeasureGroupingContent> Update(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            if (!await UnitOfMeasureGroupingContentValidator.Update(UnitOfMeasureGroupingContent))
                return UnitOfMeasureGroupingContent;
            try
            {
                var oldData = await UOW.UnitOfMeasureGroupingContentRepository.Get(UnitOfMeasureGroupingContent.Id);

                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingContentRepository.Update(UnitOfMeasureGroupingContent);
                await UOW.Commit();

                var newData = await UOW.UnitOfMeasureGroupingContentRepository.Get(UnitOfMeasureGroupingContent.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(UnitOfMeasureGroupingContentService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<UnitOfMeasureGroupingContent> Delete(UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent)
        {
            if (!await UnitOfMeasureGroupingContentValidator.Delete(UnitOfMeasureGroupingContent))
                return UnitOfMeasureGroupingContent;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingContentRepository.Delete(UnitOfMeasureGroupingContent);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, UnitOfMeasureGroupingContent, nameof(UnitOfMeasureGroupingContentService));
                return UnitOfMeasureGroupingContent;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasureGroupingContent>> BulkDelete(List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents)
        {
            if (!await UnitOfMeasureGroupingContentValidator.BulkDelete(UnitOfMeasureGroupingContents))
                return UnitOfMeasureGroupingContents;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingContentRepository.BulkDelete(UnitOfMeasureGroupingContents);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, UnitOfMeasureGroupingContents, nameof(UnitOfMeasureGroupingContentService));
                return UnitOfMeasureGroupingContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasureGroupingContent>> Import(DataFile DataFile)
        {
            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = new List<UnitOfMeasureGroupingContent>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return UnitOfMeasureGroupingContents;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int UnitOfMeasureGroupingIdColumn = 1 + StartColumn;
                int UnitOfMeasureIdColumn = 2 + StartColumn;
                int FactorColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string UnitOfMeasureGroupingIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureGroupingIdColumn].Value?.ToString();
                    string UnitOfMeasureIdValue = worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value?.ToString();
                    string FactorValue = worksheet.Cells[i + StartRow, FactorColumn].Value?.ToString();
                    UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = new UnitOfMeasureGroupingContent();
                    UnitOfMeasureGroupingContent.Factor = long.TryParse(FactorValue, out long Factor) ? Factor : 0;
                    UnitOfMeasureGroupingContents.Add(UnitOfMeasureGroupingContent);
                }
            }

            if (!await UnitOfMeasureGroupingContentValidator.Import(UnitOfMeasureGroupingContents))
                return UnitOfMeasureGroupingContents;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureGroupingContentRepository.BulkMerge(UnitOfMeasureGroupingContents);
                await UOW.Commit();

                await Logging.CreateAuditLog(UnitOfMeasureGroupingContents, new { }, nameof(UnitOfMeasureGroupingContentService));
                return UnitOfMeasureGroupingContents;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureGroupingContentService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(UnitOfMeasureGroupingContentFilter UnitOfMeasureGroupingContentFilter)
        {
            List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = await UOW.UnitOfMeasureGroupingContentRepository.List(UnitOfMeasureGroupingContentFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(UnitOfMeasureGroupingContent);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int UnitOfMeasureGroupingIdColumn = 1 + StartColumn;
                int UnitOfMeasureIdColumn = 2 + StartColumn;
                int FactorColumn = 3 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(UnitOfMeasureGroupingContent.Id);
                worksheet.Cells[1, UnitOfMeasureGroupingIdColumn].Value = nameof(UnitOfMeasureGroupingContent.UnitOfMeasureGroupingId);
                worksheet.Cells[1, UnitOfMeasureIdColumn].Value = nameof(UnitOfMeasureGroupingContent.UnitOfMeasureId);
                worksheet.Cells[1, FactorColumn].Value = nameof(UnitOfMeasureGroupingContent.Factor);

                for (int i = 0; i < UnitOfMeasureGroupingContents.Count; i++)
                {
                    UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent = UnitOfMeasureGroupingContents[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = UnitOfMeasureGroupingContent.Id;
                    worksheet.Cells[i + StartRow, UnitOfMeasureGroupingIdColumn].Value = UnitOfMeasureGroupingContent.UnitOfMeasureGroupingId;
                    worksheet.Cells[i + StartRow, UnitOfMeasureIdColumn].Value = UnitOfMeasureGroupingContent.UnitOfMeasureId;
                    worksheet.Cells[i + StartRow, FactorColumn].Value = UnitOfMeasureGroupingContent.Factor;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(UnitOfMeasureGroupingContent),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public UnitOfMeasureGroupingContentFilter ToFilter(UnitOfMeasureGroupingContentFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<UnitOfMeasureGroupingContentFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                UnitOfMeasureGroupingContentFilter subFilter = new UnitOfMeasureGroupingContentFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.UnitOfMeasureGroupingId))
                    subFilter.UnitOfMeasureGroupingId = Map(subFilter.UnitOfMeasureGroupingId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.UnitOfMeasureId))
                    subFilter.UnitOfMeasureId = Map(subFilter.UnitOfMeasureId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Factor))
                    subFilter.Factor = Map(subFilter.Factor, currentFilter.Value);
            }
            return filter;
        }
    }
}
