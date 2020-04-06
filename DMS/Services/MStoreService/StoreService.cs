using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
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
        Task<List<Store>> BulkMerge(List<Store> Stores);
        Task<List<Store>> BulkMergeParentStore(List<Store> Stores);
        Task<DataFile> Export(StoreFilter StoreFilter);
        StoreFilter ToFilter(StoreFilter StoreFilter);
    }

    public class StoreService : BaseService, IStoreService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IStoreValidator StoreValidator;
        private IRabbitManager RabbitManager;
        public StoreService(
            IUOW UOW,
            ILogging Logging,
            ICurrentContext CurrentContext,
            IStoreValidator StoreValidator,
            IRabbitManager rabbitManager
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.CurrentContext = CurrentContext;
            this.StoreValidator = StoreValidator;
            this.RabbitManager = rabbitManager;
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
                var obj = await UOW.StoreRepository.Get(Store.Id);
                RabbitManager.Publish(new List<Store> { obj }, RoutingKeyEnum.StorenSync);
                return obj;
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
                RabbitManager.Publish(new List<Store> { newData }, RoutingKeyEnum.StorenSync);
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
                RabbitManager.Publish(new List<Store> { Store }, RoutingKeyEnum.StorenSync);
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
                RabbitManager.Publish(Stores, RoutingKeyEnum.StorenSync);
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

        public async Task<List<Store>> BulkMerge(List<Store> Stores)
        {
            if (!await StoreValidator.BulkMerge(Stores))
                return Stores;

            try
            {
                await UOW.Begin();
                #region merge Store
                List<Store> dbStores = await UOW.StoreRepository.List(new StoreFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = StoreSelect.Id | StoreSelect.Code,
                });
                foreach (Store Store in Stores)
                {
                    long StoreId = dbStores.Where(x => x.Code == Store.Code)
                        .Select(x => x.Id).FirstOrDefault();
                    Store.Id = StoreId;
                }
                await UOW.StoreRepository.BulkMerge(Stores);
                #endregion

                await UOW.Commit();

                await Logging.CreateAuditLog(Stores, new { }, nameof(StoreService));
                RabbitManager.Publish(Stores, RoutingKeyEnum.StorenSync);
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

        public async Task<List<Store>> BulkMergeParentStore(List<Store> Stores)
        {
            if (!await StoreValidator.BulkMergeParentStore(Stores))
                return Stores;

            try
            {
                await UOW.Begin();
                #region merge Store
                List<Store> dbStores = await UOW.StoreRepository.List(new StoreFilter
                {
                    Skip = 0,
                    Take = int.MaxValue,
                    Selects = StoreSelect.Id | StoreSelect.Code,
                });
                foreach (Store Store in Stores)
                {
                    long StoreId = dbStores.Where(x => x.Code == Store.Code)
                        .Select(x => x.Id).FirstOrDefault();
                    Store.Id = StoreId;
                }
                await UOW.StoreRepository.BulkMerge(Stores);
                #endregion

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

                int CodeColumn = 0 + StartColumn;
                int NameColumn = 1 + StartColumn;

                int ParentStoreCodeColumn = 2 + StartColumn;
                int OrganizationCodeColumn = 3 + StartColumn;
                int StoreTypeCodeColumn = 4 + StartColumn;
                int StoreGroupingCodeColumn = 5 + StartColumn;

                int TelephoneColumn = 6 + StartColumn;

                int ProvinceCodeColumn = 7 + StartColumn;
                int DistrictCodeColumn = 8 + StartColumn;
                int WardCodeColumn = 9 + StartColumn;

                int AddressColumn = 10 + StartColumn;
                int DeliveryAddressColumn = 11 + StartColumn;

                int LatitudeColumn = 12 + StartColumn;
                int LongitudeColumn = 13 + StartColumn;

                int OwnerNameColumn = 14 + StartColumn;
                int OwnerPhoneColumn = 15 + StartColumn;
                int OwnerEmailColumn = 16 + StartColumn;

                int StatusCodeColumn = 17 + StartColumn;

                worksheet.Cells[1, CodeColumn].Value = nameof(Store.Code);
                worksheet.Cells[1, NameColumn].Value = nameof(Store.Name);
                worksheet.Cells[1, ParentStoreCodeColumn].Value = nameof(Store.ParentStoreId);
                worksheet.Cells[1, OrganizationCodeColumn].Value = nameof(Store.OrganizationId);
                worksheet.Cells[1, StoreTypeCodeColumn].Value = nameof(Store.StoreTypeId);
                worksheet.Cells[1, StoreGroupingCodeColumn].Value = nameof(Store.StoreGroupingId);
                worksheet.Cells[1, TelephoneColumn].Value = nameof(Store.Telephone);
                worksheet.Cells[1, ProvinceCodeColumn].Value = nameof(Store.ProvinceId);
                worksheet.Cells[1, DistrictCodeColumn].Value = nameof(Store.DistrictId);
                worksheet.Cells[1, WardCodeColumn].Value = nameof(Store.WardId);
                worksheet.Cells[1, AddressColumn].Value = nameof(Store.Address);
                worksheet.Cells[1, DeliveryAddressColumn].Value = nameof(Store.DeliveryAddress);
                worksheet.Cells[1, LatitudeColumn].Value = nameof(Store.Latitude);
                worksheet.Cells[1, LongitudeColumn].Value = nameof(Store.Longitude);
                worksheet.Cells[1, OwnerNameColumn].Value = nameof(Store.OwnerName);
                worksheet.Cells[1, OwnerPhoneColumn].Value = nameof(Store.OwnerPhone);
                worksheet.Cells[1, OwnerEmailColumn].Value = nameof(Store.OwnerEmail);
                worksheet.Cells[1, StatusCodeColumn].Value = nameof(Store.StatusId);

                for (int i = 0; i < Stores.Count; i++)
                {
                    Store Store = Stores[i];
                    worksheet.Cells[i + StartRow, CodeColumn].Value = Store.Code;
                    worksheet.Cells[i + StartRow, NameColumn].Value = Store.Name;
                    worksheet.Cells[i + StartRow, ParentStoreCodeColumn].Value = Store.ParentStore.Code;
                    worksheet.Cells[i + StartRow, OrganizationCodeColumn].Value = Store.Organization.Code;
                    worksheet.Cells[i + StartRow, StoreTypeCodeColumn].Value = Store.StoreType.Code;
                    worksheet.Cells[i + StartRow, StoreGroupingCodeColumn].Value = Store.StoreGrouping.Code;
                    worksheet.Cells[i + StartRow, TelephoneColumn].Value = Store.Telephone;
                    worksheet.Cells[i + StartRow, ProvinceCodeColumn].Value = Store.Province.Code;
                    worksheet.Cells[i + StartRow, DistrictCodeColumn].Value = Store.District.Code;
                    worksheet.Cells[i + StartRow, WardCodeColumn].Value = Store.Ward.Code;
                    worksheet.Cells[i + StartRow, AddressColumn].Value = Store.Address;
                    worksheet.Cells[i + StartRow, DeliveryAddressColumn].Value = Store.DeliveryAddress;
                    worksheet.Cells[i + StartRow, LatitudeColumn].Value = Store.Latitude;
                    worksheet.Cells[i + StartRow, LongitudeColumn].Value = Store.Longitude;
                    worksheet.Cells[i + StartRow, OwnerNameColumn].Value = Store.OwnerName;
                    worksheet.Cells[i + StartRow, OwnerPhoneColumn].Value = Store.OwnerPhone;
                    worksheet.Cells[i + StartRow, OwnerEmailColumn].Value = Store.OwnerEmail;
                    worksheet.Cells[i + StartRow, StatusCodeColumn].Value = Store.Status.Code;
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
