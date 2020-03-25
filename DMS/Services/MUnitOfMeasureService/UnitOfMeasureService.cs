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

namespace DMS.Services.MUnitOfMeasure
{
    public interface IUnitOfMeasureService : IServiceScoped
    {
        Task<int> Count(UnitOfMeasureFilter UnitOfMeasureFilter);
        Task<List<UnitOfMeasure>> List(UnitOfMeasureFilter UnitOfMeasureFilter);
        Task<UnitOfMeasure> Get(long Id);
        Task<UnitOfMeasure> Create(UnitOfMeasure UnitOfMeasure);
        Task<UnitOfMeasure> Update(UnitOfMeasure UnitOfMeasure);
        Task<UnitOfMeasure> Delete(UnitOfMeasure UnitOfMeasure);
        Task<List<UnitOfMeasure>> BulkDelete(List<UnitOfMeasure> UnitOfMeasures);
        Task<List<UnitOfMeasure>> Import(DataFile DataFile);
        Task<DataFile> Export(UnitOfMeasureFilter UnitOfMeasureFilter);
        UnitOfMeasureFilter ToFilter(UnitOfMeasureFilter UnitOfMeasureFilter);
    }

    public class UnitOfMeasureService : BaseService, IUnitOfMeasureService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IUnitOfMeasureValidator UnitOfMeasureValidator;

        public UnitOfMeasureService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IUnitOfMeasureValidator UnitOfMeasureValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.UnitOfMeasureValidator = UnitOfMeasureValidator;
        }
        public async Task<int> Count(UnitOfMeasureFilter UnitOfMeasureFilter)
        {
            try
            {
                int result = await UOW.UnitOfMeasureRepository.Count(UnitOfMeasureFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasure>> List(UnitOfMeasureFilter UnitOfMeasureFilter)
        {
            try
            {
                List<UnitOfMeasure> UnitOfMeasures = await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter);
                return UnitOfMeasures;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<UnitOfMeasure> Get(long Id)
        {
            UnitOfMeasure UnitOfMeasure = await UOW.UnitOfMeasureRepository.Get(Id);
            if (UnitOfMeasure == null)
                return null;
            return UnitOfMeasure;
        }

        public async Task<UnitOfMeasure> Create(UnitOfMeasure UnitOfMeasure)
        {
            if (!await UnitOfMeasureValidator.Create(UnitOfMeasure))
                return UnitOfMeasure;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.Create(UnitOfMeasure);
                await UOW.Commit();

                await Logging.CreateAuditLog(UnitOfMeasure, new { }, nameof(UnitOfMeasureService));
                return await UOW.UnitOfMeasureRepository.Get(UnitOfMeasure.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<UnitOfMeasure> Update(UnitOfMeasure UnitOfMeasure)
        {
            if (!await UnitOfMeasureValidator.Update(UnitOfMeasure))
                return UnitOfMeasure;
            try
            {
                var oldData = await UOW.UnitOfMeasureRepository.Get(UnitOfMeasure.Id);

                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.Update(UnitOfMeasure);
                await UOW.Commit();

                var newData = await UOW.UnitOfMeasureRepository.Get(UnitOfMeasure.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(UnitOfMeasureService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<UnitOfMeasure> Delete(UnitOfMeasure UnitOfMeasure)
        {
            if (!await UnitOfMeasureValidator.Delete(UnitOfMeasure))
                return UnitOfMeasure;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.Delete(UnitOfMeasure);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, UnitOfMeasure, nameof(UnitOfMeasureService));
                return UnitOfMeasure;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasure>> BulkDelete(List<UnitOfMeasure> UnitOfMeasures)
        {
            if (!await UnitOfMeasureValidator.BulkDelete(UnitOfMeasures))
                return UnitOfMeasures;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.BulkDelete(UnitOfMeasures);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, UnitOfMeasures, nameof(UnitOfMeasureService));
                return UnitOfMeasures;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<UnitOfMeasure>> Import(DataFile DataFile)
        {
            List<UnitOfMeasure> UnitOfMeasures = new List<UnitOfMeasure>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return UnitOfMeasures;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int DescriptionColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    UnitOfMeasure UnitOfMeasure = new UnitOfMeasure();
                    UnitOfMeasure.Code = CodeValue;
                    UnitOfMeasure.Name = NameValue;
                    UnitOfMeasure.Description = DescriptionValue;
                    UnitOfMeasures.Add(UnitOfMeasure);
                }
            }

            if (!await UnitOfMeasureValidator.Import(UnitOfMeasures))
                return UnitOfMeasures;

            try
            {
                await UOW.Begin();
                await UOW.UnitOfMeasureRepository.BulkMerge(UnitOfMeasures);
                await UOW.Commit();

                await Logging.CreateAuditLog(UnitOfMeasures, new { }, nameof(UnitOfMeasureService));
                return UnitOfMeasures;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(UnitOfMeasureService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(UnitOfMeasureFilter UnitOfMeasureFilter)
        {
            List<UnitOfMeasure> UnitOfMeasures = await UOW.UnitOfMeasureRepository.List(UnitOfMeasureFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(UnitOfMeasure);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int DescriptionColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(UnitOfMeasure.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(UnitOfMeasure.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(UnitOfMeasure.Name);
                worksheet.Cells[1, DescriptionColumn].Value = nameof(UnitOfMeasure.Description);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(UnitOfMeasure.StatusId);

                for (int i = 0; i < UnitOfMeasures.Count; i++)
                {
                    UnitOfMeasure UnitOfMeasure = UnitOfMeasures[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = UnitOfMeasure.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = UnitOfMeasure.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = UnitOfMeasure.Name;
                    worksheet.Cells[i + StartRow, DescriptionColumn].Value = UnitOfMeasure.Description;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = UnitOfMeasure.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(UnitOfMeasure),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public UnitOfMeasureFilter ToFilter(UnitOfMeasureFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<UnitOfMeasureFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                UnitOfMeasureFilter subFilter = new UnitOfMeasureFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Description))
                    subFilter.Description = Map(subFilter.Description, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
