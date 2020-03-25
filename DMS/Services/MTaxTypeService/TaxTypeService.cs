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

namespace DMS.Services.MTaxType
{
    public interface ITaxTypeService : IServiceScoped
    {
        Task<int> Count(TaxTypeFilter TaxTypeFilter);
        Task<List<TaxType>> List(TaxTypeFilter TaxTypeFilter);
        Task<TaxType> Get(long Id);
        Task<TaxType> Create(TaxType TaxType);
        Task<TaxType> Update(TaxType TaxType);
        Task<TaxType> Delete(TaxType TaxType);
        Task<List<TaxType>> BulkDelete(List<TaxType> TaxTypes);
        Task<List<TaxType>> Import(DataFile DataFile);
        Task<DataFile> Export(TaxTypeFilter TaxTypeFilter);
        TaxTypeFilter ToFilter(TaxTypeFilter TaxTypeFilter);
    }

    public class TaxTypeService : BaseService, ITaxTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ITaxTypeValidator TaxTypeValidator;

        public TaxTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ITaxTypeValidator TaxTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.TaxTypeValidator = TaxTypeValidator;
        }
        public async Task<int> Count(TaxTypeFilter TaxTypeFilter)
        {
            try
            {
                int result = await UOW.TaxTypeRepository.Count(TaxTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<TaxType>> List(TaxTypeFilter TaxTypeFilter)
        {
            try
            {
                List<TaxType> TaxTypes = await UOW.TaxTypeRepository.List(TaxTypeFilter);
                return TaxTypes;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<TaxType> Get(long Id)
        {
            TaxType TaxType = await UOW.TaxTypeRepository.Get(Id);
            if (TaxType == null)
                return null;
            return TaxType;
        }

        public async Task<TaxType> Create(TaxType TaxType)
        {
            if (!await TaxTypeValidator.Create(TaxType))
                return TaxType;

            try
            {
                await UOW.Begin();
                await UOW.TaxTypeRepository.Create(TaxType);
                await UOW.Commit();

                await Logging.CreateAuditLog(TaxType, new { }, nameof(TaxTypeService));
                return await UOW.TaxTypeRepository.Get(TaxType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<TaxType> Update(TaxType TaxType)
        {
            if (!await TaxTypeValidator.Update(TaxType))
                return TaxType;
            try
            {
                var oldData = await UOW.TaxTypeRepository.Get(TaxType.Id);

                await UOW.Begin();
                await UOW.TaxTypeRepository.Update(TaxType);
                await UOW.Commit();

                var newData = await UOW.TaxTypeRepository.Get(TaxType.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(TaxTypeService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<TaxType> Delete(TaxType TaxType)
        {
            if (!await TaxTypeValidator.Delete(TaxType))
                return TaxType;

            try
            {
                await UOW.Begin();
                await UOW.TaxTypeRepository.Delete(TaxType);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, TaxType, nameof(TaxTypeService));
                return TaxType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<TaxType>> BulkDelete(List<TaxType> TaxTypes)
        {
            if (!await TaxTypeValidator.BulkDelete(TaxTypes))
                return TaxTypes;

            try
            {
                await UOW.Begin();
                await UOW.TaxTypeRepository.BulkDelete(TaxTypes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, TaxTypes, nameof(TaxTypeService));
                return TaxTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<TaxType>> Import(DataFile DataFile)
        {
            List<TaxType> TaxTypes = new List<TaxType>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return TaxTypes;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int PercentageColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PercentageValue = worksheet.Cells[i + StartRow, PercentageColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    TaxType TaxType = new TaxType();
                    TaxType.Code = CodeValue;
                    TaxType.Name = NameValue;
                    TaxType.Percentage = decimal.TryParse(PercentageValue, out decimal Percentage) ? Percentage : 0;
                    TaxTypes.Add(TaxType);
                }
            }

            if (!await TaxTypeValidator.Import(TaxTypes))
                return TaxTypes;

            try
            {
                await UOW.Begin();
                await UOW.TaxTypeRepository.BulkMerge(TaxTypes);
                await UOW.Commit();

                await Logging.CreateAuditLog(TaxTypes, new { }, nameof(TaxTypeService));
                return TaxTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(TaxTypeService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(TaxTypeFilter TaxTypeFilter)
        {
            List<TaxType> TaxTypes = await UOW.TaxTypeRepository.List(TaxTypeFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(TaxType);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int PercentageColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(TaxType.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(TaxType.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(TaxType.Name);
                worksheet.Cells[1, PercentageColumn].Value = nameof(TaxType.Percentage);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(TaxType.StatusId);

                for (int i = 0; i < TaxTypes.Count; i++)
                {
                    TaxType TaxType = TaxTypes[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = TaxType.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = TaxType.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = TaxType.Name;
                    worksheet.Cells[i + StartRow, PercentageColumn].Value = TaxType.Percentage;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = TaxType.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(TaxType),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public TaxTypeFilter ToFilter(TaxTypeFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<TaxTypeFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                TaxTypeFilter subFilter = new TaxTypeFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Percentage))
                    subFilter.Percentage = Map(subFilter.Percentage, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
