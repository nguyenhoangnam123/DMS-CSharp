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

namespace DMS.Services.MStoreGrouping
{
    public interface IStoreGroupingService :  IServiceScoped
    {
        Task<int> Count(StoreGroupingFilter StoreGroupingFilter);
        Task<List<StoreGrouping>> List(StoreGroupingFilter StoreGroupingFilter);
        Task<StoreGrouping> Get(long Id);
        Task<StoreGrouping> Create(StoreGrouping StoreGrouping);
        Task<StoreGrouping> Update(StoreGrouping StoreGrouping);
        Task<StoreGrouping> Delete(StoreGrouping StoreGrouping);
        Task<List<StoreGrouping>> BulkDelete(List<StoreGrouping> StoreGroupings);
        Task<List<StoreGrouping>> Import(DataFile DataFile);
        Task<DataFile> Export(StoreGroupingFilter StoreGroupingFilter);
        StoreGroupingFilter ToFilter(StoreGroupingFilter StoreGroupingFilter);
    }

    public class StoreGroupingService : BaseService, IStoreGroupingService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreGroupingValidator StoreGroupingValidator;

        public StoreGroupingService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreGroupingValidator StoreGroupingValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreGroupingValidator = StoreGroupingValidator;
        }
        public async Task<int> Count(StoreGroupingFilter StoreGroupingFilter)
        {
            try
            {
                int result = await UOW.StoreGroupingRepository.Count(StoreGroupingFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<StoreGrouping>> List(StoreGroupingFilter StoreGroupingFilter)
        {
            try
            {
                List<StoreGrouping> StoreGroupings = await UOW.StoreGroupingRepository.List(StoreGroupingFilter);
                return StoreGroupings;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<StoreGrouping> Get(long Id)
        {
            StoreGrouping StoreGrouping = await UOW.StoreGroupingRepository.Get(Id);
            if (StoreGrouping == null)
                return null;
            return StoreGrouping;
        }
       
        public async Task<StoreGrouping> Create(StoreGrouping StoreGrouping)
        {
            if (!await StoreGroupingValidator.Create(StoreGrouping))
                return StoreGrouping;

            try
            {
                await UOW.Begin();
                await UOW.StoreGroupingRepository.Create(StoreGrouping);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreGrouping, new { }, nameof(StoreGroupingService));
                return await UOW.StoreGroupingRepository.Get(StoreGrouping.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<StoreGrouping> Update(StoreGrouping StoreGrouping)
        {
            if (!await StoreGroupingValidator.Update(StoreGrouping))
                return StoreGrouping;
            try
            {
                var oldData = await UOW.StoreGroupingRepository.Get(StoreGrouping.Id);

                await UOW.Begin();
                await UOW.StoreGroupingRepository.Update(StoreGrouping);
                await UOW.Commit();

                var newData = await UOW.StoreGroupingRepository.Get(StoreGrouping.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreGroupingService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<StoreGrouping> Delete(StoreGrouping StoreGrouping)
        {
            if (!await StoreGroupingValidator.Delete(StoreGrouping))
                return StoreGrouping;

            try
            {
                await UOW.Begin();
                await UOW.StoreGroupingRepository.Delete(StoreGrouping);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, StoreGrouping, nameof(StoreGroupingService));
                return StoreGrouping;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<StoreGrouping>> BulkDelete(List<StoreGrouping> StoreGroupings)
        {
            if (!await StoreGroupingValidator.BulkDelete(StoreGroupings))
                return StoreGroupings;

            try
            {
                await UOW.Begin();
                await UOW.StoreGroupingRepository.BulkDelete(StoreGroupings);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, StoreGroupings, nameof(StoreGroupingService));
                return StoreGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<StoreGrouping>> Import(DataFile DataFile)
        {
            List<StoreGrouping> StoreGroupings = new List<StoreGrouping>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return StoreGroupings;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ParentIdColumn = 3 + StartColumn;
                int PathColumn = 4 + StartColumn;
                int LevelColumn = 5 + StartColumn;
                int IsActiveColumn = 6 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ParentIdValue = worksheet.Cells[i + StartRow, ParentIdColumn].Value?.ToString();
                    string PathValue = worksheet.Cells[i + StartRow, PathColumn].Value?.ToString();
                    string LevelValue = worksheet.Cells[i + StartRow, LevelColumn].Value?.ToString();
                    string IsActiveValue = worksheet.Cells[i + StartRow, IsActiveColumn].Value?.ToString();
                    StoreGrouping StoreGrouping = new StoreGrouping();
                    StoreGrouping.Code = CodeValue;
                    StoreGrouping.Name = NameValue;
                    StoreGrouping.Path = PathValue;
                    StoreGrouping.Level = long.TryParse(LevelValue, out long Level) ? Level : 0;
                    StoreGroupings.Add(StoreGrouping);
                }
            }
            
            if (!await StoreGroupingValidator.Import(StoreGroupings))
                return StoreGroupings;
            
            try
            {
                await UOW.Begin();
                await UOW.StoreGroupingRepository.BulkMerge(StoreGroupings);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreGroupings, new { }, nameof(StoreGroupingService));
                return StoreGroupings;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreGroupingService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(StoreGroupingFilter StoreGroupingFilter)
        {
            List<StoreGrouping> StoreGroupings = await UOW.StoreGroupingRepository.List(StoreGroupingFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(StoreGrouping);
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
                int LevelColumn = 5 + StartColumn;
                int IsActiveColumn = 6 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(StoreGrouping.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(StoreGrouping.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(StoreGrouping.Name);
                worksheet.Cells[1, ParentIdColumn].Value = nameof(StoreGrouping.ParentId);
                worksheet.Cells[1, PathColumn].Value = nameof(StoreGrouping.Path);
                worksheet.Cells[1, LevelColumn].Value = nameof(StoreGrouping.Level);
                worksheet.Cells[1, IsActiveColumn].Value = nameof(StoreGrouping.IsActive);

                for(int i = 0; i < StoreGroupings.Count; i++)
                {
                    StoreGrouping StoreGrouping = StoreGroupings[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = StoreGrouping.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = StoreGrouping.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = StoreGrouping.Name;
                    worksheet.Cells[i + StartRow, ParentIdColumn].Value = StoreGrouping.ParentId;
                    worksheet.Cells[i + StartRow, PathColumn].Value = StoreGrouping.Path;
                    worksheet.Cells[i + StartRow, LevelColumn].Value = StoreGrouping.Level;
                    worksheet.Cells[i + StartRow, IsActiveColumn].Value = StoreGrouping.IsActive;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(StoreGrouping),
                Content = MemoryStream,
            };
            return DataFile;
        }
        
        public StoreGroupingFilter ToFilter(StoreGroupingFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreGroupingFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                StoreGroupingFilter subFilter = new StoreGroupingFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ParentId))
                    subFilter.ParentId = Map(subFilter.ParentId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Path))
                    subFilter.Path = Map(subFilter.Path, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Level))
                    subFilter.Level = Map(subFilter.Level, currentFilter.Value);
            }
            return filter;
        }
    }
}
