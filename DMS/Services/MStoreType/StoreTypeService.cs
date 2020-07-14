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

namespace DMS.Services.MStoreType
{
    public interface IStoreTypeService : IServiceScoped
    {
        Task<int> Count(StoreTypeFilter StoreTypeFilter);
        Task<List<StoreType>> List(StoreTypeFilter StoreTypeFilter);
        Task<StoreType> Get(long Id);
        Task<StoreType> Create(StoreType StoreType);
        Task<StoreType> Update(StoreType StoreType);
        Task<StoreType> Delete(StoreType StoreType);
        Task<List<StoreType>> BulkDelete(List<StoreType> StoreTypes);
        Task<List<StoreType>> Import(DataFile DataFile);
        Task<DataFile> Export(StoreTypeFilter StoreTypeFilter);
    }

    public class StoreTypeService : BaseService, IStoreTypeService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreTypeValidator StoreTypeValidator;

        public StoreTypeService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreTypeValidator StoreTypeValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreTypeValidator = StoreTypeValidator;
        }
        public async Task<int> Count(StoreTypeFilter StoreTypeFilter)
        {
            try
            {
                int result = await UOW.StoreTypeRepository.Count(StoreTypeFilter);
                return result;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                };
            }
        }

        public async Task<List<StoreType>> List(StoreTypeFilter StoreTypeFilter)
        {
            try
            {
                List<StoreType> StoreTypes = await UOW.StoreTypeRepository.List(StoreTypeFilter);
                return StoreTypes;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }
        public async Task<StoreType> Get(long Id)
        {
            StoreType StoreType = await UOW.StoreTypeRepository.Get(Id);
            if (StoreType == null)
                return null;
            return StoreType;
        }

        public async Task<StoreType> Create(StoreType StoreType)
        {
            if (!await StoreTypeValidator.Create(StoreType))
                return StoreType;

            try
            {
                await UOW.Begin();
                await UOW.StoreTypeRepository.Create(StoreType);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreType, new { }, nameof(StoreTypeService));
                return await UOW.StoreTypeRepository.Get(StoreType.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<StoreType> Update(StoreType StoreType)
        {
            if (!await StoreTypeValidator.Update(StoreType))
                return StoreType;
            try
            {
                var oldData = await UOW.StoreTypeRepository.Get(StoreType.Id);

                await UOW.Begin();
                await UOW.StoreTypeRepository.Update(StoreType);
                await UOW.Commit();

                var newData = await UOW.StoreTypeRepository.Get(StoreType.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreTypeService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<StoreType> Delete(StoreType StoreType)
        {
            if (!await StoreTypeValidator.Delete(StoreType))
                return StoreType;

            try
            {
                await UOW.Begin();
                await UOW.StoreTypeRepository.Delete(StoreType);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, StoreType, nameof(StoreTypeService));
                return StoreType;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<StoreType>> BulkDelete(List<StoreType> StoreTypes)
        {
            if (!await StoreTypeValidator.BulkDelete(StoreTypes))
                return StoreTypes;

            try
            {
                await UOW.Begin();
                await UOW.StoreTypeRepository.BulkDelete(StoreTypes);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, StoreTypes, nameof(StoreTypeService));
                return StoreTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<List<StoreType>> Import(DataFile DataFile)
        {
            List<StoreType> StoreTypes = new List<StoreType>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return StoreTypes;
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
                    StoreType StoreType = new StoreType();
                    StoreType.Code = CodeValue;
                    StoreType.Name = NameValue;
                    StoreTypes.Add(StoreType);
                }
            }

            if (!await StoreTypeValidator.Import(StoreTypes))
                return StoreTypes;

            try
            {
                await UOW.Begin();
                await UOW.StoreTypeRepository.BulkMerge(StoreTypes);
                await UOW.Commit();

                await Logging.CreateAuditLog(StoreTypes, new { }, nameof(StoreTypeService));
                return StoreTypes;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(StoreTypeService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(StoreTypeService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<DataFile> Export(StoreTypeFilter StoreTypeFilter)
        {
            List<StoreType> StoreTypes = await UOW.StoreTypeRepository.List(StoreTypeFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(StoreType);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int StatusIdColumn = 3 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(StoreType.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(StoreType.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(StoreType.Name);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(StoreType.StatusId);

                for (int i = 0; i < StoreTypes.Count; i++)
                {
                    StoreType StoreType = StoreTypes[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = StoreType.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = StoreType.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = StoreType.Name;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = StoreType.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(StoreType),
                Content = MemoryStream,
            };
            return DataFile;
        }
    }
}
