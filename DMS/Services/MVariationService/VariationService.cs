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

namespace DMS.Services.MVariation
{
    public interface IVariationService :  IServiceScoped
    {
        Task<int> Count(VariationFilter VariationFilter);
        Task<List<Variation>> List(VariationFilter VariationFilter);
        Task<Variation> Get(long Id);
        Task<Variation> Create(Variation Variation);
        Task<Variation> Update(Variation Variation);
        Task<Variation> Delete(Variation Variation);
        Task<List<Variation>> BulkDelete(List<Variation> Variations);
        Task<List<Variation>> Import(DataFile DataFile);
        Task<DataFile> Export(VariationFilter VariationFilter);
        VariationFilter ToFilter(VariationFilter VariationFilter);
    }

    public class VariationService : BaseService, IVariationService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IVariationValidator VariationValidator;

        public VariationService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IVariationValidator VariationValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.VariationValidator = VariationValidator;
        }
        public async Task<int> Count(VariationFilter VariationFilter)
        {
            try
            {
                int result = await UOW.VariationRepository.Count(VariationFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(VariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Variation>> List(VariationFilter VariationFilter)
        {
            try
            {
                List<Variation> Variations = await UOW.VariationRepository.List(VariationFilter);
                return Variations;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(VariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Variation> Get(long Id)
        {
            Variation Variation = await UOW.VariationRepository.Get(Id);
            if (Variation == null)
                return null;
            return Variation;
        }
       
        public async Task<Variation> Create(Variation Variation)
        {
            if (!await VariationValidator.Create(Variation))
                return Variation;

            try
            {
                await UOW.Begin();
                await UOW.VariationRepository.Create(Variation);
                await UOW.Commit();

                await Logging.CreateAuditLog(Variation, new { }, nameof(VariationService));
                return await UOW.VariationRepository.Get(Variation.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(VariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Variation> Update(Variation Variation)
        {
            if (!await VariationValidator.Update(Variation))
                return Variation;
            try
            {
                var oldData = await UOW.VariationRepository.Get(Variation.Id);

                await UOW.Begin();
                await UOW.VariationRepository.Update(Variation);
                await UOW.Commit();

                var newData = await UOW.VariationRepository.Get(Variation.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(VariationService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(VariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Variation> Delete(Variation Variation)
        {
            if (!await VariationValidator.Delete(Variation))
                return Variation;

            try
            {
                await UOW.Begin();
                await UOW.VariationRepository.Delete(Variation);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Variation, nameof(VariationService));
                return Variation;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(VariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Variation>> BulkDelete(List<Variation> Variations)
        {
            if (!await VariationValidator.BulkDelete(Variations))
                return Variations;

            try
            {
                await UOW.Begin();
                await UOW.VariationRepository.BulkDelete(Variations);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Variations, nameof(VariationService));
                return Variations;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(VariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Variation>> Import(DataFile DataFile)
        {
            List<Variation> Variations = new List<Variation>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Variations;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int VariationGroupingIdColumn = 3 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string VariationGroupingIdValue = worksheet.Cells[i + StartRow, VariationGroupingIdColumn].Value?.ToString();
                    Variation Variation = new Variation();
                    Variation.Code = CodeValue;
                    Variation.Name = NameValue;
                    Variations.Add(Variation);
                }
            }
            
            if (!await VariationValidator.Import(Variations))
                return Variations;
            
            try
            {
                await UOW.Begin();
                await UOW.VariationRepository.BulkMerge(Variations);
                await UOW.Commit();

                await Logging.CreateAuditLog(Variations, new { }, nameof(VariationService));
                return Variations;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(VariationService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(VariationFilter VariationFilter)
        {
            List<Variation> Variations = await UOW.VariationRepository.List(VariationFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Variation);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int VariationGroupingIdColumn = 3 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(Variation.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(Variation.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(Variation.Name);
                worksheet.Cells[1, VariationGroupingIdColumn].Value = nameof(Variation.VariationGroupingId);

                for(int i = 0; i < Variations.Count; i++)
                {
                    Variation Variation = Variations[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Variation.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Variation.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Variation.Name;
                    worksheet.Cells[i + StartRow, VariationGroupingIdColumn].Value = Variation.VariationGroupingId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Variation),
                Content = MemoryStream,
            };
            return DataFile;
        }
        
        public VariationFilter ToFilter(VariationFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<VariationFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                VariationFilter subFilter = new VariationFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.VariationGroupingId))
                    subFilter.VariationGroupingId = Map(subFilter.VariationGroupingId, currentFilter.Value);
            }
            return filter;
        }
    }
}
