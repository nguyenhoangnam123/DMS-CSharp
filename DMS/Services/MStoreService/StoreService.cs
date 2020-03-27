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

namespace DMS.Services.MStore
{
    public interface IStoreService : IServiceScoped
    {
        Task<int> Count(StoreFilter StoreFilter);
        Task<List<Store>> List(StoreFilter StoreFilter);
        Task<Store> Get(long Id);
        Task<Store> Create(Store Store);
        Task<Store> Update(Store Store);
        Task<Store> Delete(Store Store);
        Task<List<Store>> BulkDelete(List<Store> Stores);
        Task<List<Store>> Import(DataFile DataFile);
        Task<DataFile> Export(StoreFilter StoreFilter);
        StoreFilter ToFilter(StoreFilter StoreFilter);
    }

    public class StoreService : BaseService, IStoreService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreValidator StoreValidator;

        public StoreService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreValidator StoreValidator
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreValidator = StoreValidator;
        }
        public async Task<int> Count(StoreFilter StoreFilter)
        {
            try
            {
                int result = await UOW.StoreRepository.Count(StoreFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Store>> List(StoreFilter StoreFilter)
        {
            try
            {
                List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
                return Stores;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }
        public async Task<Store> Get(long Id)
        {
            Store Store = await UOW.StoreRepository.Get(Id);
            if (Store == null)
                return null;
            return Store;
        }

        public async Task<Store> Create(Store Store)
        {
            if (!await StoreValidator.Create(Store))
                return Store;

            try
            {
                Store.Id = 0;
                await UOW.Begin();
                await UOW.StoreRepository.Create(Store);
                await UOW.Commit();

                await Logging.CreateAuditLog(Store, new { }, nameof(StoreService));
                return await UOW.StoreRepository.Get(Store.Id);
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Store> Update(Store Store)
        {
            if (!await StoreValidator.Update(Store))
                return Store;
            try
            {
                var oldData = await UOW.StoreRepository.Get(Store.Id);

                await UOW.Begin();
                await UOW.StoreRepository.Update(Store);
                await UOW.Commit();

                var newData = await UOW.StoreRepository.Get(Store.Id);
                await Logging.CreateAuditLog(newData, oldData, nameof(StoreService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<Store> Delete(Store Store)
        {
            if (!await StoreValidator.Delete(Store))
                return Store;

            try
            {
                await UOW.Begin();
                await UOW.StoreRepository.Delete(Store);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Store, nameof(StoreService));
                return Store;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Store>> BulkDelete(List<Store> Stores)
        {
            if (!await StoreValidator.BulkDelete(Stores))
                return Stores;

            try
            {
                await UOW.Begin();
                await UOW.StoreRepository.BulkDelete(Stores);
                await UOW.Commit();
                await Logging.CreateAuditLog(new { }, Stores, nameof(StoreService));
                return Stores;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<List<Store>> Import(DataFile DataFile)
        {
            List<Store> Stores = new List<Store>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return Stores;
                int StartColumn = 1;
                int StartRow = 1;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ParentStoreIdColumn = 3 + StartColumn;
                int OrganizationIdColumn = 4 + StartColumn;
                int StoreTypeIdColumn = 5 + StartColumn;
                int StoreGroupingIdColumn = 6 + StartColumn;
                int TelephoneColumn = 7 + StartColumn;
                int ProvinceIdColumn = 8 + StartColumn;
                int DistrictIdColumn = 9 + StartColumn;
                int WardIdColumn = 10 + StartColumn;
                int Address1Column = 11 + StartColumn;
                int Address2Column = 12 + StartColumn;
                int LatitudeColumn = 13 + StartColumn;
                int LongitudeColumn = 14 + StartColumn;
                int OwnerNameColumn = 15 + StartColumn;
                int OwnerPhoneColumn = 16 + StartColumn;
                int OwnerEmailColumn = 17 + StartColumn;
                int StatusIdColumn = 18 + StartColumn;
                for (int i = 1; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, IdColumn].Value?.ToString()))
                        break;
                    string IdValue = worksheet.Cells[i + StartRow, IdColumn].Value?.ToString();
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ParentStoreIdValue = worksheet.Cells[i + StartRow, ParentStoreIdColumn].Value?.ToString();
                    string OrganizationIdValue = worksheet.Cells[i + StartRow, OrganizationIdColumn].Value?.ToString();
                    string StoreTypeIdValue = worksheet.Cells[i + StartRow, StoreTypeIdColumn].Value?.ToString();
                    string StoreGroupingIdValue = worksheet.Cells[i + StartRow, StoreGroupingIdColumn].Value?.ToString();
                    string TelephoneValue = worksheet.Cells[i + StartRow, TelephoneColumn].Value?.ToString();
                    string ProvinceIdValue = worksheet.Cells[i + StartRow, ProvinceIdColumn].Value?.ToString();
                    string DistrictIdValue = worksheet.Cells[i + StartRow, DistrictIdColumn].Value?.ToString();
                    string WardIdValue = worksheet.Cells[i + StartRow, WardIdColumn].Value?.ToString();
                    string Address1Value = worksheet.Cells[i + StartRow, Address1Column].Value?.ToString();
                    string Address2Value = worksheet.Cells[i + StartRow, Address2Column].Value?.ToString();
                    string LatitudeValue = worksheet.Cells[i + StartRow, LatitudeColumn].Value?.ToString();
                    string LongitudeValue = worksheet.Cells[i + StartRow, LongitudeColumn].Value?.ToString();
                    string OwnerNameValue = worksheet.Cells[i + StartRow, OwnerNameColumn].Value?.ToString();
                    string OwnerPhoneValue = worksheet.Cells[i + StartRow, OwnerPhoneColumn].Value?.ToString();
                    string OwnerEmailValue = worksheet.Cells[i + StartRow, OwnerEmailColumn].Value?.ToString();
                    string StatusIdValue = worksheet.Cells[i + StartRow, StatusIdColumn].Value?.ToString();
                    Store Store = new Store();
                    Store.Code = CodeValue;
                    Store.Name = NameValue;
                    Store.Telephone = TelephoneValue;
                    Store.Address = Address1Value;
                    Store.DeliveryAddress = Address2Value;
                    Store.Latitude = decimal.TryParse(LatitudeValue, out decimal Latitude) ? Latitude : 0;
                    Store.Longitude = decimal.TryParse(LongitudeValue, out decimal Longitude) ? Longitude : 0;
                    Store.OwnerName = OwnerNameValue;
                    Store.OwnerPhone = OwnerPhoneValue;
                    Store.OwnerEmail = OwnerEmailValue;
                    Stores.Add(Store);
                }
            }

            if (!await StoreValidator.Import(Stores))
                return Stores;

            try
            {
                Stores.ForEach(s => s.Id = 0);
                await UOW.Begin();
                await UOW.StoreRepository.BulkMerge(Stores);
                await UOW.Commit();

                await Logging.CreateAuditLog(Stores, new { }, nameof(StoreService));
                return Stores;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                await Logging.CreateSystemLog(ex.InnerException, nameof(StoreService));
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task<DataFile> Export(StoreFilter StoreFilter)
        {
            List<Store> Stores = await UOW.StoreRepository.List(StoreFilter);
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(MemoryStream))
            {
                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = CurrentContext.UserName;
                excelPackage.Workbook.Properties.Title = nameof(Store);
                excelPackage.Workbook.Properties.Created = StaticParams.DateTimeNow;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
                int StartColumn = 1;
                int StartRow = 2;
                int IdColumn = 0 + StartColumn;
                int CodeColumn = 1 + StartColumn;
                int NameColumn = 2 + StartColumn;
                int ParentStoreIdColumn = 3 + StartColumn;
                int OrganizationIdColumn = 4 + StartColumn;
                int StoreTypeIdColumn = 5 + StartColumn;
                int StoreGroupingIdColumn = 6 + StartColumn;
                int TelephoneColumn = 7 + StartColumn;
                int ProvinceIdColumn = 8 + StartColumn;
                int DistrictIdColumn = 9 + StartColumn;
                int WardIdColumn = 10 + StartColumn;
                int Address1Column = 11 + StartColumn;
                int Address2Column = 12 + StartColumn;
                int LatitudeColumn = 13 + StartColumn;
                int LongitudeColumn = 14 + StartColumn;
                int OwnerNameColumn = 15 + StartColumn;
                int OwnerPhoneColumn = 16 + StartColumn;
                int OwnerEmailColumn = 17 + StartColumn;
                int StatusIdColumn = 18 + StartColumn;

                worksheet.Cells[1, IdColumn].Value = nameof(Store.Id);
                worksheet.Cells[1, CodeColumn].Value = nameof(Store.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(Store.Name);
                worksheet.Cells[1, ParentStoreIdColumn].Value = nameof(Store.ParentStoreId);
                worksheet.Cells[1, OrganizationIdColumn].Value = nameof(Store.OrganizationId);
                worksheet.Cells[1, StoreTypeIdColumn].Value = nameof(Store.StoreTypeId);
                worksheet.Cells[1, StoreGroupingIdColumn].Value = nameof(Store.StoreGroupingId);
                worksheet.Cells[1, TelephoneColumn].Value = nameof(Store.Telephone);
                worksheet.Cells[1, ProvinceIdColumn].Value = nameof(Store.ProvinceId);
                worksheet.Cells[1, DistrictIdColumn].Value = nameof(Store.DistrictId);
                worksheet.Cells[1, WardIdColumn].Value = nameof(Store.WardId);
                worksheet.Cells[1, Address1Column].Value = nameof(Store.Address);
                worksheet.Cells[1, Address2Column].Value = nameof(Store.DeliveryAddress);
                worksheet.Cells[1, LatitudeColumn].Value = nameof(Store.Latitude);
                worksheet.Cells[1, LongitudeColumn].Value = nameof(Store.Longitude);
                worksheet.Cells[1, OwnerNameColumn].Value = nameof(Store.OwnerName);
                worksheet.Cells[1, OwnerPhoneColumn].Value = nameof(Store.OwnerPhone);
                worksheet.Cells[1, OwnerEmailColumn].Value = nameof(Store.OwnerEmail);
                worksheet.Cells[1, StatusIdColumn].Value = nameof(Store.StatusId);

                for (int i = 0; i < Stores.Count; i++)
                {
                    Store Store = Stores[i];
                    worksheet.Cells[i + StartRow, IdColumn].Value = Store.Id;
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Store.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Store.Name;
                    worksheet.Cells[i + StartRow, ParentStoreIdColumn].Value = Store.ParentStoreId;
                    worksheet.Cells[i + StartRow, OrganizationIdColumn].Value = Store.OrganizationId;
                    worksheet.Cells[i + StartRow, StoreTypeIdColumn].Value = Store.StoreTypeId;
                    worksheet.Cells[i + StartRow, StoreGroupingIdColumn].Value = Store.StoreGroupingId;
                    worksheet.Cells[i + StartRow, TelephoneColumn].Value = Store.Telephone;
                    worksheet.Cells[i + StartRow, ProvinceIdColumn].Value = Store.ProvinceId;
                    worksheet.Cells[i + StartRow, DistrictIdColumn].Value = Store.DistrictId;
                    worksheet.Cells[i + StartRow, WardIdColumn].Value = Store.WardId;
                    worksheet.Cells[i + StartRow, Address1Column].Value = Store.Address;
                    worksheet.Cells[i + StartRow, Address2Column].Value = Store.DeliveryAddress;
                    worksheet.Cells[i + StartRow, LatitudeColumn].Value = Store.Latitude;
                    worksheet.Cells[i + StartRow, LongitudeColumn].Value = Store.Longitude;
                    worksheet.Cells[i + StartRow, OwnerNameColumn].Value = Store.OwnerName;
                    worksheet.Cells[i + StartRow, OwnerPhoneColumn].Value = Store.OwnerPhone;
                    worksheet.Cells[i + StartRow, OwnerEmailColumn].Value = Store.OwnerEmail;
                    worksheet.Cells[i + StartRow, StatusIdColumn].Value = Store.StatusId;
                }
                excelPackage.Save();
            }

            DataFile DataFile = new DataFile
            {
                Name = nameof(Store),
                Content = MemoryStream,
            };
            return DataFile;
        }

        public StoreFilter ToFilter(StoreFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<StoreFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                StoreFilter subFilter = new StoreFilter();
                filter.OrFilter.Add(subFilter);
                if (currentFilter.Value.Name == nameof(subFilter.Id))
                    subFilter.Id = Map(subFilter.Id, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Code))
                    subFilter.Code = Map(subFilter.Code, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Name))
                    subFilter.Name = Map(subFilter.Name, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ParentStoreId))
                    subFilter.ParentStoreId = Map(subFilter.ParentStoreId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.OrganizationId))
                    subFilter.OrganizationId = Map(subFilter.OrganizationId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StoreTypeId))
                    subFilter.StoreTypeId = Map(subFilter.StoreTypeId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StoreGroupingId))
                    subFilter.StoreGroupingId = Map(subFilter.StoreGroupingId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Telephone))
                    subFilter.Telephone = Map(subFilter.Telephone, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.ProvinceId))
                    subFilter.ProvinceId = Map(subFilter.ProvinceId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.DistrictId))
                    subFilter.DistrictId = Map(subFilter.DistrictId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.WardId))
                    subFilter.WardId = Map(subFilter.WardId, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Address))
                    subFilter.Address = Map(subFilter.Address, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.DeliveryAddress))
                    subFilter.DeliveryAddress = Map(subFilter.DeliveryAddress, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Latitude))
                    subFilter.Latitude = Map(subFilter.Latitude, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.Longitude))
                    subFilter.Longitude = Map(subFilter.Longitude, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.OwnerName))
                    subFilter.OwnerName = Map(subFilter.OwnerName, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.OwnerPhone))
                    subFilter.OwnerPhone = Map(subFilter.OwnerPhone, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.OwnerEmail))
                    subFilter.OwnerEmail = Map(subFilter.OwnerEmail, currentFilter.Value);
                if (currentFilter.Value.Name == nameof(subFilter.StatusId))
                    subFilter.StatusId = Map(subFilter.StatusId, currentFilter.Value);
            }
            return filter;
        }
    }
}
