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

namespace DMS.Services.MWard
{
    public interface IWardService :  IServiceScoped
    {
        Task<int> Count(WardFilter WardFilter);
        Task<List<Ward>> List(WardFilter WardFilter);
        Task<Ward> Get(long Id);
        Task<Ward> Create(Ward Ward);
        Task<Ward> Update(Ward Ward);
        Task<Ward> Delete(Ward Ward);
        Task<List<Ward>> BulkDelete(List<Ward> Wards);
        Task<List<Ward>> Import(DataFile DataFile);
        Task<DataFile> Export(WardFilter WardFilter);
        WardFilter ToFilter(WardFilter WardFilter);
    }

    public class WardService : BaseService, IWardService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IWardValidator WardValidator;

        public WardService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IWardValidator WardValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.WardValidator = WardValidator;
        }
        public async Task<int> Count(WardFilter WardFilter)
        {
            try
            {
                int result = await UOW.WardRepository.Count(WardFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Ward>> List(WardFilter WardFilter)
        {
            try
            {
                List<Ward> Wards = await UOW.WardRepository.List(WardFilter);
                return Wards;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Ward> Get(long Id)
        {
            Ward Ward = await UOW.WardRepository.Get(Id);
            if (Ward == null)
                return null;
            return Ward;
        }
       
        public async Task<Ward> Create(Ward Ward)
        {
            if (!await WardValidator.Create(Ward))
                return Ward;

            try
            {
                await UOW.Begin();
                await UOW.WardRepository.Create(Ward);
                await UOW.Commit();

                await Logging.CreateAuditLog(Ward, new { }, nameof(WardService));
                return await UOW.WardRepository.Get(Ward.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Ward> Update(Ward Ward)
        {
            if (!await WardValidator.Update(Ward))
                return Ward;
            try
            {
                var oldData = await UOW.WardRepository.Get(Ward.Id);

                await UOW.Begin();
                await UOW.WardRepository.Update(Ward);
                await UOW.Commit();

                var newData = await UOW.WardRepository.Get(Ward.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(WardService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Ward> Delete(Ward Ward)
        {
            if (!await WardValidator.Delete(Ward))
                return Ward;

            try
            {
                await UOW.Begin();
                await UOW.WardRepository.Delete(Ward);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Ward, nameof(WardService));
                return Ward;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Ward>> BulkDelete(List<Ward> Wards)
        {
            if (!await WardValidator.BulkDelete(Wards))
                return Wards;

            try
            {
                await UOW.Begin();
                await UOW.WardRepository.BulkDelete(Wards);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Wards, nameof(WardService));
                return Wards;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Ward>> Import(DataFile DataFile)
        {
            List<Ward> Wards = new List<Ward>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Wards;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PriorityColumn = 2 + StartColumn;
                int DistrictIdColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PriorityValue = worksheet.Cells[i + StartRow, PriorityColumn].Value?.ToString();
                    string DistrictIdValue = worksheet.Cells[i + StartRow, DistrictIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    Ward Ward = new Ward();
                    Ward.Name = NameValue;
                    Ward.Priority = long.TryParse(PriorityValue, out long Priority) ? Priority : 0;
                    Wards.Add(Ward);
                }
            }
            
            if (!await WardValidator.Import(Wards))
                return Wards;
            
            try
            {
                await UOW.Begin();
                await UOW.WardRepository.BulkMerge(Wards);
                await UOW.Commit();

                await Logging.CreateAuditLog(Wards, new { }, nameof(WardService));
                return Wards;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(WardService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(WardFilter WardFilter)
        {
            List<Ward> Wards = await UOW.WardRepository.List(WardFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Ward);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PriorityColumn = 2 + StartColumn;
                int DistrictIdColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(Ward.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Ward.Name);
                worksheet.Cells[1, PriorityColumn].Value = nameof(Ward.Priority);
                worksheet.Cells[1, DistrictIdColumn].Value = nameof(Ward.DistrictId);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Ward.StatusId);

                for(int i = 0; i < Wards.Count; i++)
                {
                    Ward Ward = Wards[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Ward.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Ward.Name;
                    worksheet.Cells[i + StartRow, PriorityColumn].Value = Ward.Priority;
                    worksheet.Cells[i + StartRow, DistrictIdColumn].Value = Ward.DistrictId;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Ward.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Ward),
                Content = MemoryStream,
            };
            return DataFile;
        }
        
        public WardFilter ToFilter(WardFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<WardFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                WardFilter subFilter = new WardFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Priority))
                    subFilter.Priority = Map(subFilter.Priority, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.DistrictId))
                    subFilter.DistrictId = Map(subFilter.DistrictId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
