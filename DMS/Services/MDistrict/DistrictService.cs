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

namespace DMS.Services.MDistrict
{
    public interface IDistrictService : IServiceScoped
    {
        Task<int> Count(DistrictFilter DistrictFilter);
        Task<List<District>> List(DistrictFilter DistrictFilter);
        Task<District> Get(long Id);
        Task<District> Create(District District);
        Task<District> Update(District District);
        Task<District> Delete(District District);
        Task<List<District>> BulkDelete(List<District> Districts);
        Task<List<District>> Import(DataFile DataFile);
        Task<DataFile> Export(DistrictFilter DistrictFilter);
        DistrictFilter ToFilter(DistrictFilter DistrictFilter);
    }

    public class DistrictService : BaseService, IDistrictService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IDistrictValidator DistrictValidator;

        public DistrictService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IDistrictValidator DistrictValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.DistrictValidator = DistrictValidator;
        }
        public async Task<int> Count(DistrictFilter DistrictFilter)
        {
            try
            {
                int result = await UOW.DistrictRepository.Count(DistrictFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<District>> List(DistrictFilter DistrictFilter)
        {
            try
            {
                List<District> Districts = await UOW.DistrictRepository.List(DistrictFilter);
                return Districts;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<District> Get(long Id)
        {
            District District = await UOW.DistrictRepository.Get(Id);
            if (District == null)
                return null;
            return District;
        }

        public async Task<District> Create(District District)
        {
            if (!await DistrictValidator.Create(District))
                return District;

            try
            {
                await UOW.Begin();
                await UOW.DistrictRepository.Create(District);
                await UOW.Commit();

                await Logging.CreateAuditLog(District, new { }, nameof(DistrictService));
                return await UOW.DistrictRepository.Get(District.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<District> Update(District District)
        {
            if (!await DistrictValidator.Update(District))
                return District;
            try
            {
                var oldData = await UOW.DistrictRepository.Get(District.Id);

                await UOW.Begin();
                await UOW.DistrictRepository.Update(District);
                await UOW.Commit();

                var newData = await UOW.DistrictRepository.Get(District.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(DistrictService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<District> Delete(District District)
        {
            if (!await DistrictValidator.Delete(District))
                return District;

            try
            {
                await UOW.Begin();
                await UOW.DistrictRepository.Delete(District);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, District, nameof(DistrictService));
                return District;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<District>> BulkDelete(List<District> Districts)
        {
            if (!await DistrictValidator.BulkDelete(Districts))
                return Districts;

            try
            {
                await UOW.Begin();
                await UOW.DistrictRepository.BulkDelete(Districts);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Districts, nameof(DistrictService));
                return Districts;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<District>> Import(DataFile DataFile)
        {
            List<District> Districts = new List<District>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Districts;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PriorityColumn = 2 + StartColumn;
                int ProvinceIdColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string PriorityValue = worksheet.Cells[i + StartRow, PriorityColumn].Value?.ToString();
                    string ProvinceIdValue = worksheet.Cells[i + StartRow, ProvinceIdColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    District District = new District();
                    District.Name = NameValue;
                    District.Priority = long.TryParse(PriorityValue, out long Priority) ? Priority : 0;
                    Districts.Add(District);
                }
            }

            if (!await DistrictValidator.Import(Districts))
                return Districts;

            try
            {
                await UOW.Begin();
                await UOW.DistrictRepository.BulkMerge(Districts);
                await UOW.Commit();

                await Logging.CreateAuditLog(Districts, new { }, nameof(DistrictService));
                return Districts;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(DistrictService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(DistrictFilter DistrictFilter)
        {
            List<District> Districts = await UOW.DistrictRepository.List(DistrictFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(District);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int PriorityColumn = 2 + StartColumn;
                int ProvinceIdColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(District.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(District.Name);
                worksheet.Cells[1, PriorityColumn].Value = nameof(District.Priority);
                worksheet.Cells[1, ProvinceIdColumn].Value = nameof(District.ProvinceId);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(District.StatusId);

                for (int i = 0; i < Districts.Count; i++)
                {
                    District District = Districts[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = District.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = District.Name;
                    worksheet.Cells[i + StartRow, PriorityColumn].Value = District.Priority;
                    worksheet.Cells[i + StartRow, ProvinceIdColumn].Value = District.ProvinceId;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = District.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(District),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public DistrictFilter ToFilter(DistrictFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<DistrictFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                DistrictFilter subFilter = new DistrictFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Priority))
                    subFilter.Priority = Map(subFilter.Priority, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ProvinceId))
                    subFilter.ProvinceId = Map(subFilter.ProvinceId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
