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

namespace DMS.Services.MSupplier
{
    public interface ISupplierService : IServiceScoped
    {
        Task<int> Count(SupplierFilter SupplierFilter);
        Task<List<Supplier>> List(SupplierFilter SupplierFilter);
        Task<Supplier> Get(long Id);
        Task<Supplier> Create(Supplier Supplier);
        Task<Supplier> Update(Supplier Supplier);
        Task<Supplier> Delete(Supplier Supplier);
        Task<List<Supplier>> BulkDelete(List<Supplier> Suppliers);
        Task<List<Supplier>> BulkMerge(List<Supplier> Suppliers);
        Task<DataFile> Export(SupplierFilter SupplierFilter);
        SupplierFilter ToFilter(SupplierFilter SupplierFilter);
    }

    public class SupplierService : BaseService, ISupplierService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private ISupplierValidator SupplierValidator;

        public SupplierService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            ISupplierValidator SupplierValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.SupplierValidator = SupplierValidator;
        }
        public async Task<int> Count(SupplierFilter SupplierFilter)
        {
            try
            {
                int result = await UOW.SupplierRepository.Count(SupplierFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Supplier>> List(SupplierFilter SupplierFilter)
        {
            try
            {
                List<Supplier> Suppliers = await UOW.SupplierRepository.List(SupplierFilter);
                return Suppliers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Supplier> Get(long Id)
        {
            Supplier Supplier = await UOW.SupplierRepository.Get(Id);
            if (Supplier == null)
                return null;
            return Supplier;
        }

        public async Task<Supplier> Create(Supplier Supplier)
        {
            if (!await SupplierValidator.Create(Supplier))
                return Supplier;

            try
            {
                Supplier.Id = 0;
                await UOW.Begin();
                await UOW.SupplierRepository.Create(Supplier);
                await UOW.Commit();

                await Logging.CreateAuditLog(Supplier, new { }, nameof(SupplierService));
                return await UOW.SupplierRepository.Get(Supplier.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Supplier> Update(Supplier Supplier)
        {
            if (!await SupplierValidator.Update(Supplier))
                return Supplier;
            try
            {
                var oldData = await UOW.SupplierRepository.Get(Supplier.Id);

                await UOW.Begin();
                await UOW.SupplierRepository.Update(Supplier);
                await UOW.Commit();

                var newData = await UOW.SupplierRepository.Get(Supplier.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(SupplierService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Supplier> Delete(Supplier Supplier)
        {
            if (!await SupplierValidator.Delete(Supplier))
                return Supplier;

            try
            {
                await UOW.Begin();
                await UOW.SupplierRepository.Delete(Supplier);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Supplier, nameof(SupplierService));
                return Supplier;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Supplier>> BulkDelete(List<Supplier> Suppliers)
        {
            if (!await SupplierValidator.BulkDelete(Suppliers))
                return Suppliers;

            try
            {
                await UOW.Begin();
                await UOW.SupplierRepository.BulkDelete(Suppliers);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Suppliers, nameof(SupplierService));
                return Suppliers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Supplier>> BulkMerge(List<Supplier> Suppliers)
        {
            if (!await SupplierValidator.BulkMerge(Suppliers))
                return Suppliers;

            try
            {
                await UOW.Begin();
                #region merge Supplier
                List<Supplier> dbSuppliers = await UOW.SupplierRepository.List(new SupplierFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = SupplierSelect.Id | SupplierSelect.Code,
                });
                foreach (Supplier Supplier in Suppliers)
                {
                    long SupplierId = dbSuppliers.Where(x => x.Code == Supplier.Code)
                        .Select(x => x.Id).FirstOrDefault();
                    Supplier.Id = SupplierId;
                }
                await UOW.SupplierRepository.BulkMerge(Suppliers);
                #endregion

                await UOW.Commit();

                await Logging.CreateAuditLog(Suppliers, new { }, nameof(SupplierService));
                return Suppliers;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(SupplierService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(SupplierFilter SupplierFilter)
        {
            List<Supplier> Suppliers = await UOW.SupplierRepository.List(SupplierFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Supplier);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int TaxCodeColumn = 3 + StartColumn;
                int StatusIdColumn = 4 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Supplier.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(Supplier.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(Supplier.Name);
                worksheet.Cells[1, TaxCodeColumn].Value = nameof(Supplier.TaxCode);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Supplier.StatusId);

                for (int i = 0; i < Suppliers.Count; i++)
                {
                    Supplier Supplier = Suppliers[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Supplier.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Supplier.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Supplier.Name;
                    worksheet.Cells[i + StartRow, TaxCodeColumn].Value = Supplier.TaxCode;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Supplier.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Supplier),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public SupplierFilter ToFilter(SupplierFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<SupplierFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                SupplierFilter subFilter = new SupplierFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                }
            }
            return filter;
        }
    }
}
