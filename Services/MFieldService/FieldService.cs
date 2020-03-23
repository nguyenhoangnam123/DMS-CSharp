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

namespace DMS.Services.MField
{
    public interface IFieldService :  IServiceScoped
    {
        Task<int> Count(FieldFilter FieldFilter);
        Task<List<Field>> List(FieldFilter FieldFilter);
        Task<Field> Get(long Id);
        Task<Field> Create(Field Field);
        Task<Field> Update(Field Field);
        Task<Field> Delete(Field Field);
        Task<List<Field>> BulkDelete(List<Field> Fields);
        Task<List<Field>> Import(DataFile DataFile);
        Task<DataFile> Export(FieldFilter FieldFilter);
        FieldFilter ToFilter(FieldFilter FieldFilter);
    }

    public class FieldService : BaseService, IFieldService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IFieldValidator FieldValidator;

        public FieldService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IFieldValidator FieldValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.FieldValidator = FieldValidator;
        }
        public async Task<int> Count(FieldFilter FieldFilter)
        {
            try
            {
                int result = await UOW.FieldRepository.Count(FieldFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Field>> List(FieldFilter FieldFilter)
        {
            try
            {
                List<Field> Fields = await UOW.FieldRepository.List(FieldFilter);
                return Fields;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Field> Get(long Id)
        {
            Field Field = await UOW.FieldRepository.Get(Id);
            if (Field == null)
                return null;
            return Field;
        }
       
        public async Task<Field> Create(Field Field)
        {
            if (!await FieldValidator.Create(Field))
                return Field;

            try
            {
                await UOW.Begin();
                await UOW.FieldRepository.Create(Field);
                await UOW.Commit();

                await Logging.CreateAuditLog(Field, new { }, nameof(FieldService));
                return await UOW.FieldRepository.Get(Field.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Field> Update(Field Field)
        {
            if (!await FieldValidator.Update(Field))
                return Field;
            try
            {
                var oldData = await UOW.FieldRepository.Get(Field.Id);

                await UOW.Begin();
                await UOW.FieldRepository.Update(Field);
                await UOW.Commit();

                var newData = await UOW.FieldRepository.Get(Field.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(FieldService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Field> Delete(Field Field)
        {
            if (!await FieldValidator.Delete(Field))
                return Field;

            try
            {
                await UOW.Begin();
                await UOW.FieldRepository.Delete(Field);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Field, nameof(FieldService));
                return Field;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Field>> BulkDelete(List<Field> Fields)
        {
            if (!await FieldValidator.BulkDelete(Fields))
                return Fields;

            try
            {
                await UOW.Begin();
                await UOW.FieldRepository.BulkDelete(Fields);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Fields, nameof(FieldService));
                return Fields;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        
        public async Task<List<Field>> Import(DataFile DataFile)
        {
            List<Field> Fields = new List<Field>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Fields;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int TypeColumn = 2 + StartColumn;
                int MenuIdColumn = 3 + StartColumn;
                int IsDeletedColumn = 4 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string TypeValue = worksheet.Cells[i + StartRow, TypeColumn].Value?.ToString();
                    string MenuIdValue = worksheet.Cells[i + StartRow, MenuIdColumn].Value?.ToString();
                    string IsDeletedValue = worksheet.Cells[i + StartRow, IsDeletedColumn].Value?.ToString();
                    Field Field = new Field();
                    Field.Name = NameValue;
                    Field.Type = TypeValue;
                    Fields.Add(Field);
                }
            }
            
            if (!await FieldValidator.Import(Fields))
                return Fields;
            
            try
            {
                await UOW.Begin();
                await UOW.FieldRepository.BulkMerge(Fields);
                await UOW.Commit();

                await Logging.CreateAuditLog(Fields, new { }, nameof(FieldService));
                return Fields;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(FieldService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }    

        public async Task<DataFile> Export(FieldFilter FieldFilter)
        {
            List<Field> Fields = await UOW.FieldRepository.List(FieldFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Field);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;
                int TypeColumn = 2 + StartColumn;
                int MenuIdColumn = 3 + StartColumn;
                int IsDeletedColumn = 4 + StartColumn;
                
                worksheet.Cells[1, IdColumn].Value = nameof(Field.Id);
                worksheet.Cells[1, NameColumn].Value = nameof(Field.Name);
                worksheet.Cells[1, TypeColumn].Value = nameof(Field.Type);
                worksheet.Cells[1, MenuIdColumn].Value = nameof(Field.MenuId);
                worksheet.Cells[1, IsDeletedColumn].Value = nameof(Field.IsDeleted);

                for(int i = 0; i < Fields.Count; i++)
                {
                    Field Field = Fields[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Field.Id;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Field.Name;
                    worksheet.Cells[i + StartRow, TypeColumn].Value = Field.Type;
                    worksheet.Cells[i + StartRow, MenuIdColumn].Value = Field.MenuId;
                    worksheet.Cells[i + StartRow, IsDeletedColumn].Value = Field.IsDeleted;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Field),
                Content = MemoryStream,
            };
            return DataFile;
        }
        
        public FieldFilter ToFilter(FieldFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<FieldFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                FieldFilter subFilter = new FieldFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Type))
                    subFilter.Type = Map(subFilter.Type, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.MenuId))
                    subFilter.MenuId = Map(subFilter.MenuId, currentFilter.Value);
            }
            return filter;
        }
    }
}
