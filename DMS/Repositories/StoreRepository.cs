using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Models;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thinktecture;
using Thinktecture.EntityFrameworkCore.TempTables;

namespace DMS.Repositories
{
    public interface IStoreRepository
    {
        Task<int> Count(StoreFilter StoreFilter);
        Task<List<Store>> List(StoreFilter StoreFilter);
        Task<int> CountInScoped(StoreFilter filter, long AppUserId);
        Task<List<Store>> ListInScoped(StoreFilter filter, long AppUserId);
        Task<Store> Get(long Id);
        Task<bool> Create(Store Store);
        Task<bool> Update(Store Store);
        Task<bool> Delete(Store Store);
        Task<bool> BulkMerge(List<Store> Stores);
        Task<bool> BulkDelete(List<Store> Stores);
    }
    public class StoreRepository : IStoreRepository
    {
        private DataContext DataContext;
        public StoreRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private async Task<IQueryable<StoreDAO>> DynamicFilter(IQueryable<StoreDAO> query, StoreFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);


            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Search != null)
            {
                List<string> Tokens = filter.Search.ToLower().Split(" ").ToList();
                var queryForCodeDraft = query;
                var queryForCode = query;
                var queryForAddress = query;
                var queryForUnsignAddress = query;
                var queryForUnsignName = query;
                var queryForName = query;
                foreach (string Token in Tokens)
                {
                    queryForCodeDraft = queryForCodeDraft.Where(x => x.CodeDraft.ToLower().Contains(Token));
                    queryForCode = queryForCode.Where(x => x.Code.ToLower().Contains(Token));
                    queryForAddress = queryForAddress.Where(x => x.Address.ToLower().Contains(Token));
                    queryForUnsignAddress = queryForUnsignAddress.Where(x => x.UnsignAddress.ToLower().Contains(Token));
                    queryForUnsignName = queryForUnsignName.Where(x => x.UnsignName.ToLower().Contains(Token));
                    queryForName = queryForName.Where(x => x.Name.ToLower().Contains(Token));
                }
                query = queryForCodeDraft
                    .Union(queryForCode)
                    .Union(queryForAddress)
                    .Union(queryForUnsignAddress)
                    .Union(queryForUnsignName)
                    .Union(queryForName);
            }
            if (filter.Id != null)
            {
                if (filter.Id.In != null && filter.Id.In.Count > 0)
                {
                    if (filter.Id.NotIn == null || filter.Id.NotIn.Count == 0)
                    {
                        ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(filter.Id.In.Distinct().ToList());
                        query = query.Join(tempTableQuery.Query,
                                           c => c.Id,
                                           t => t.Column1,
                                           (c, t) => c);
                    }
                    else
                    {
                        ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                            .BulkInsertValuesIntoTempTableAsync<long>(filter.Id.In.Distinct().ToList());
                        query = query.Where(x => !filter.Id.NotIn.Contains(x.Id));
                        query = query.Join(tempTableQuery.Query,
                                           c => c.Id,
                                           t => t.Column1,
                                           (c, t) => c);
                    }

                }
                else
                {
                    query = query.Where(q => q.Id, filter.Id);
                }
            }
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.CodeDraft != null)
                query = query.Where(q => q.CodeDraft, filter.CodeDraft);
            if (filter.Name != null && filter.Name.HasValue)
                query = query.Where(q => q.Name, filter.Name).Union(query.Where(q => q.UnsignName, filter.Name));
            if (filter.ParentStoreId != null && filter.ParentStoreId.HasValue)
                query = query.Where(q => q.ParentStoreId.HasValue).Where(q => q.ParentStoreId.Value, filter.ParentStoreId);
            if (filter.StoreCheckingStatusId != null)
            {
                if (filter.StoreCheckingStatusId.Equal.HasValue)
                {
                    var Start = StaticParams.DateTimeNow.AddHours(filter.TimeZone).Date.AddHours(0 - filter.TimeZone);
                    var End = StaticParams.DateTimeNow.AddHours(filter.TimeZone).Date.AddHours(0 - filter.TimeZone).AddDays(1).AddSeconds(-1);
                    var storeCheckingQuery = DataContext.StoreChecking
                           .Where(sc => sc.CheckOutAt.HasValue && Start <= sc.CheckOutAt.Value && sc.CheckOutAt.Value <= End);
                    if (filter.SalesEmployeeId != null && filter.SalesEmployeeId.Equal.HasValue)
                    {
                        storeCheckingQuery = storeCheckingQuery.Where(x => x.SaleEmployeeId == filter.SalesEmployeeId.Equal.Value);
                    }
                    var storeIds = storeCheckingQuery.Select(x => x.StoreId).ToList();
                    if (filter.StoreCheckingStatusId.Equal.Value == StoreCheckingStatusEnum.CHECKED.Id)
                        query = query.Where(q => storeIds.Contains(q.Id));
                    if (filter.StoreCheckingStatusId.Equal.Value == StoreCheckingStatusEnum.NOTCHECKED.Id)
                        query = query.Where(q => !storeIds.Contains(q.Id));
                }
            }
            if (filter.OrganizationId != null)
            {
                if (filter.OrganizationId.Equal != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.Equal.Value).FirstOrDefault();
                    query = query.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.NotEqual.Value).FirstOrDefault();
                    query = query.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.In != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.In.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => Ids.Contains(q.OrganizationId));
                }
                if (filter.OrganizationId.NotIn != null)
                {
                    List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                        .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                    List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => filter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                    List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                    List<long> Ids = Branches.Select(o => o.Id).ToList();
                    query = query.Where(q => !Ids.Contains(q.OrganizationId));
                }
            }

            if (filter.StoreTypeId != null)
                query = query.Where(q => q.StoreTypeId, filter.StoreTypeId);
            if (filter.StoreGroupingId != null && filter.StoreGroupingId.HasValue)
                query = query.Where(q => q.StoreGroupingId.HasValue)
                    .Where(q => q.StoreGroupingId.Value, filter.StoreGroupingId);
            if (filter.ResellerId != null)
                query = query.Where(q => q.ResellerId, filter.ResellerId);
            if (filter.Telephone != null)
                query = query.Where(q => q.Telephone, filter.Telephone);
            if (filter.ProvinceId != null)
                query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            if (filter.DistrictId != null)
                query = query.Where(q => q.DistrictId, filter.DistrictId);
            if (filter.WardId != null)
                query = query.Where(q => q.WardId, filter.WardId);
            if (filter.Address != null && filter.Address.HasValue)
                query = query.Where(q => q.Address, filter.Address).Union(query.Where(q => q.UnsignAddress, filter.Address));
            if (filter.DeliveryAddress != null)
                query = query.Where(q => q.DeliveryAddress, filter.DeliveryAddress);
            if (filter.Latitude != null)
                query = query.Where(q => q.Latitude, filter.Latitude);
            if (filter.Longitude != null)
                query = query.Where(q => q.Longitude, filter.Longitude);
            if (filter.DeliveryLatitude != null)
                query = query.Where(q => q.DeliveryLatitude, filter.DeliveryLatitude);
            if (filter.DeliveryLongitude != null)
                query = query.Where(q => q.DeliveryLongitude, filter.DeliveryLongitude);
            if (filter.OwnerName != null)
                query = query.Where(q => q.OwnerName, filter.OwnerName);
            if (filter.OwnerPhone != null)
                query = query.Where(q => q.OwnerPhone, filter.OwnerPhone);
            if (filter.OwnerEmail != null)
                query = query.Where(q => q.OwnerEmail, filter.OwnerEmail);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.AppUserId != null)
                query = query.Where(q => q.AppUserId, filter.AppUserId);
            if (filter.StoreStatusId != null && filter.StoreStatusId.Equal.HasValue && filter.StoreStatusId.Equal != StoreStatusEnum.ALL.Id)
                query = query.Where(q => q.StoreStatusId, filter.StoreStatusId);
            query = OrFilter(query, filter);
            query = query.Distinct();
            return query;
        }

        private IQueryable<StoreDAO> OrFilter(IQueryable<StoreDAO> query, StoreFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<StoreDAO> initQuery = query.Where(q => false);
            foreach (StoreFilter StoreFilter in filter.OrFilter)
            {
                IQueryable<StoreDAO> queryable = query;
                if (StoreFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, StoreFilter.Id);
                if (StoreFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, StoreFilter.Code);
                if (StoreFilter.CodeDraft != null)
                    queryable = queryable.Where(q => q.CodeDraft, StoreFilter.CodeDraft);
                if (StoreFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, StoreFilter.Name);
                if (StoreFilter.ParentStoreId != null)
                    queryable = queryable.Where(q => q.ParentStoreId, StoreFilter.ParentStoreId);
                if (StoreFilter.OrganizationId != null)
                {
                    if (StoreFilter.OrganizationId.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == StoreFilter.OrganizationId.Equal.Value).FirstOrDefault();
                        queryable = queryable.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (StoreFilter.OrganizationId.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == StoreFilter.OrganizationId.NotEqual.Value).FirstOrDefault();
                        queryable = queryable.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (StoreFilter.OrganizationId.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => StoreFilter.OrganizationId.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => Ids.Contains(q.OrganizationId));
                    }
                    if (StoreFilter.OrganizationId.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => StoreFilter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => !Ids.Contains(q.OrganizationId));
                    }
                }
                if (StoreFilter.StoreTypeId != null)
                    queryable = queryable.Where(q => q.StoreTypeId, StoreFilter.StoreTypeId);
                if (StoreFilter.StoreGroupingId != null)
                    queryable = queryable.Where(q => q.StoreGroupingId.HasValue).Where(q => q.StoreGroupingId.Value, StoreFilter.StoreGroupingId);
                if (StoreFilter.ResellerId != null)
                    queryable = queryable.Where(q => q.ResellerId, StoreFilter.ResellerId);
                if (StoreFilter.Telephone != null)
                    queryable = queryable.Where(q => q.Telephone, StoreFilter.Telephone);
                if (StoreFilter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, StoreFilter.ProvinceId);
                if (StoreFilter.DistrictId != null)
                    queryable = queryable.Where(q => q.DistrictId, StoreFilter.DistrictId);
                if (StoreFilter.WardId != null)
                    queryable = queryable.Where(q => q.WardId, StoreFilter.WardId);
                if (StoreFilter.Address != null)
                    queryable = queryable.Where(q => q.Address, StoreFilter.Address);
                if (StoreFilter.DeliveryAddress != null)
                    queryable = queryable.Where(q => q.DeliveryAddress, StoreFilter.DeliveryAddress);
                if (StoreFilter.Latitude != null)
                    queryable = queryable.Where(q => q.Latitude, StoreFilter.Latitude);
                if (StoreFilter.Longitude != null)
                    queryable = queryable.Where(q => q.Longitude, StoreFilter.Longitude);
                if (StoreFilter.DeliveryLatitude != null)
                    queryable = queryable.Where(q => q.DeliveryLatitude, StoreFilter.DeliveryLatitude);
                if (StoreFilter.DeliveryLongitude != null)
                    queryable = queryable.Where(q => q.DeliveryLongitude, StoreFilter.DeliveryLongitude);
                if (StoreFilter.OwnerName != null)
                    queryable = queryable.Where(q => q.OwnerName, StoreFilter.OwnerName);
                if (StoreFilter.OwnerPhone != null)
                    queryable = queryable.Where(q => q.OwnerPhone, StoreFilter.OwnerPhone);
                if (StoreFilter.OwnerEmail != null)
                    queryable = queryable.Where(q => q.OwnerEmail, StoreFilter.OwnerEmail);
                if (StoreFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, StoreFilter.StatusId);
                if (StoreFilter.AppUserId != null)
                    queryable = queryable.Where(q => q.AppUserId, StoreFilter.AppUserId);
                if (StoreFilter.StoreStatusId != null && StoreFilter.StoreStatusId.Equal.HasValue && StoreFilter.StoreStatusId.Equal != StoreStatusEnum.ALL.Id)
                    queryable = queryable.Where(q => q.StoreStatusId, StoreFilter.StoreStatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<StoreDAO> DynamicOrder(IQueryable<StoreDAO> query, StoreFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case StoreOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case StoreOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case StoreOrder.CodeDraft:
                            query = query.OrderBy(q => q.CodeDraft);
                            break;
                        case StoreOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case StoreOrder.ParentStore:
                            query = query.OrderBy(q => q.ParentStoreId);
                            break;
                        case StoreOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case StoreOrder.StoreType:
                            query = query.OrderBy(q => q.StoreTypeId);
                            break;
                        case StoreOrder.StoreGrouping:
                            query = query.OrderBy(q => q.StoreGroupingId);
                            break;
                        case StoreOrder.Reseller:
                            query = query.OrderBy(q => q.ResellerId);
                            break;
                        case StoreOrder.Telephone:
                            query = query.OrderBy(q => q.Telephone);
                            break;
                        case StoreOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case StoreOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case StoreOrder.Ward:
                            query = query.OrderBy(q => q.WardId);
                            break;
                        case StoreOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case StoreOrder.DeliveryAddress:
                            query = query.OrderBy(q => q.DeliveryAddress);
                            break;
                        case StoreOrder.Latitude:
                            query = query.OrderBy(q => q.Latitude);
                            break;
                        case StoreOrder.Longitude:
                            query = query.OrderBy(q => q.Longitude);
                            break;
                        case StoreOrder.DeliveryLatitude:
                            query = query.OrderBy(q => q.DeliveryLatitude);
                            break;
                        case StoreOrder.DeliveryLongitude:
                            query = query.OrderBy(q => q.DeliveryLongitude);
                            break;
                        case StoreOrder.OwnerName:
                            query = query.OrderBy(q => q.OwnerName);
                            break;
                        case StoreOrder.OwnerPhone:
                            query = query.OrderBy(q => q.OwnerPhone);
                            break;
                        case StoreOrder.OwnerEmail:
                            query = query.OrderBy(q => q.OwnerEmail);
                            break;
                        case StoreOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case StoreOrder.AppUser:
                            query = query.OrderBy(q => q.AppUserId);
                            break;
                        case StoreOrder.StoreStatus:
                            query = query.OrderBy(q => q.StoreStatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case StoreOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case StoreOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case StoreOrder.CodeDraft:
                            query = query.OrderByDescending(q => q.CodeDraft);
                            break;
                        case StoreOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case StoreOrder.ParentStore:
                            query = query.OrderByDescending(q => q.ParentStoreId);
                            break;
                        case StoreOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case StoreOrder.StoreType:
                            query = query.OrderByDescending(q => q.StoreTypeId);
                            break;
                        case StoreOrder.StoreGrouping:
                            query = query.OrderByDescending(q => q.StoreGroupingId);
                            break;
                        case StoreOrder.Reseller:
                            query = query.OrderByDescending(q => q.ResellerId);
                            break;
                        case StoreOrder.Telephone:
                            query = query.OrderByDescending(q => q.Telephone);
                            break;
                        case StoreOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case StoreOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case StoreOrder.Ward:
                            query = query.OrderByDescending(q => q.WardId);
                            break;
                        case StoreOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case StoreOrder.DeliveryAddress:
                            query = query.OrderByDescending(q => q.DeliveryAddress);
                            break;
                        case StoreOrder.Latitude:
                            query = query.OrderByDescending(q => q.Latitude);
                            break;
                        case StoreOrder.Longitude:
                            query = query.OrderByDescending(q => q.Longitude);
                            break;
                        case StoreOrder.DeliveryLatitude:
                            query = query.OrderByDescending(q => q.DeliveryLatitude);
                            break;
                        case StoreOrder.DeliveryLongitude:
                            query = query.OrderByDescending(q => q.DeliveryLongitude);
                            break;
                        case StoreOrder.OwnerName:
                            query = query.OrderByDescending(q => q.OwnerName);
                            break;
                        case StoreOrder.OwnerPhone:
                            query = query.OrderByDescending(q => q.OwnerPhone);
                            break;
                        case StoreOrder.OwnerEmail:
                            query = query.OrderByDescending(q => q.OwnerEmail);
                            break;
                        case StoreOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case StoreOrder.AppUser:
                            query = query.OrderByDescending(q => q.AppUserId);
                            break;
                        case StoreOrder.StoreStatus:
                            query = query.OrderByDescending(q => q.StoreStatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Store>> DynamicSelect(IQueryable<StoreDAO> query, StoreFilter filter)
        {
            List<Store> Stores = await query.Select(q => new Store()
            {
                Id = filter.Selects.Contains(StoreSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(StoreSelect.Code) ? q.Code : default(string),
                CodeDraft = filter.Selects.Contains(StoreSelect.CodeDraft) ? q.CodeDraft : default(string),
                Name = filter.Selects.Contains(StoreSelect.Name) ? q.Name : default(string),
                UnsignName = filter.Selects.Contains(StoreSelect.UnsignName) ? q.UnsignName : default(string),
                ParentStoreId = filter.Selects.Contains(StoreSelect.ParentStore) ? q.ParentStoreId : default(long?),
                OrganizationId = filter.Selects.Contains(StoreSelect.Organization) ? q.OrganizationId : default(long),
                StoreTypeId = filter.Selects.Contains(StoreSelect.StoreType) ? q.StoreTypeId : default(long),
                StoreGroupingId = filter.Selects.Contains(StoreSelect.StoreGrouping) ? q.StoreGroupingId : default(long?),
                ResellerId = filter.Selects.Contains(StoreSelect.Reseller) ? q.ResellerId : default(long?),
                Telephone = filter.Selects.Contains(StoreSelect.Telephone) ? q.Telephone : default(string),
                ProvinceId = filter.Selects.Contains(StoreSelect.Province) ? q.ProvinceId : default(long),
                DistrictId = filter.Selects.Contains(StoreSelect.District) ? q.DistrictId : default(long),
                WardId = filter.Selects.Contains(StoreSelect.Ward) ? q.WardId : default(long),
                Address = filter.Selects.Contains(StoreSelect.Address) ? q.Address : default(string),
                UnsignAddress = filter.Selects.Contains(StoreSelect.UnsignAddress) ? q.UnsignAddress : default(string),
                DeliveryAddress = filter.Selects.Contains(StoreSelect.DeliveryAddress) ? q.DeliveryAddress : default(string),
                Latitude = filter.Selects.Contains(StoreSelect.Latitude) ? q.Latitude : default(decimal),
                Longitude = filter.Selects.Contains(StoreSelect.Longitude) ? q.Longitude : default(decimal),
                DeliveryLatitude = filter.Selects.Contains(StoreSelect.DeliveryLatitude) ? q.DeliveryLatitude : default(decimal?),
                DeliveryLongitude = filter.Selects.Contains(StoreSelect.DeliveryLongitude) ? q.DeliveryLongitude : default(decimal?),
                OwnerName = filter.Selects.Contains(StoreSelect.OwnerName) ? q.OwnerName : default(string),
                OwnerPhone = filter.Selects.Contains(StoreSelect.OwnerPhone) ? q.OwnerPhone : default(string),
                OwnerEmail = filter.Selects.Contains(StoreSelect.OwnerEmail) ? q.OwnerEmail : default(string),
                TaxCode = filter.Selects.Contains(StoreSelect.TaxCode) ? q.TaxCode : default(string),
                LegalEntity = filter.Selects.Contains(StoreSelect.LegalEntity) ? q.LegalEntity : default(string),
                StatusId = filter.Selects.Contains(StoreSelect.Status) ? q.StatusId : default(long),
                StoreScoutingId = filter.Selects.Contains(StoreSelect.StoreScouting) ? q.StoreScoutingId : default(long?),
                AppUserId = filter.Selects.Contains(StoreSelect.AppUser) ? q.AppUserId : default(long?),
                StoreStatusId = filter.Selects.Contains(StoreSelect.StoreStatus) ? q.StoreStatusId : default(long),
                District = filter.Selects.Contains(StoreSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Code = q.District.Code,
                    Name = q.District.Name,
                    Priority = q.District.Priority,
                    ProvinceId = q.District.ProvinceId,
                    StatusId = q.District.StatusId,
                } : null,
                Organization = filter.Selects.Contains(StoreSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    ParentId = q.Organization.ParentId,
                    Path = q.Organization.Path,
                    Level = q.Organization.Level,
                    StatusId = q.Organization.StatusId,
                    Phone = q.Organization.Phone,
                    Address = q.Organization.Address,
                    Email = q.Organization.Email,
                } : null,
                ParentStore = filter.Selects.Contains(StoreSelect.ParentStore) && q.ParentStore != null ? new Store
                {
                    Id = q.ParentStore.Id,
                    Code = q.ParentStore.Code,
                    Name = q.ParentStore.Name,
                    ParentStoreId = q.ParentStore.ParentStoreId,
                    OrganizationId = q.ParentStore.OrganizationId,
                    StoreTypeId = q.ParentStore.StoreTypeId,
                    StoreGroupingId = q.ParentStore.StoreGroupingId,
                    Telephone = q.ParentStore.Telephone,
                    ResellerId = q.ParentStore.ResellerId,
                    ProvinceId = q.ParentStore.ProvinceId,
                    DistrictId = q.ParentStore.DistrictId,
                    WardId = q.ParentStore.WardId,
                    Address = q.ParentStore.Address,
                    DeliveryAddress = q.ParentStore.DeliveryAddress,
                    Latitude = q.ParentStore.Latitude,
                    Longitude = q.ParentStore.Longitude,
                    DeliveryLatitude = q.ParentStore.DeliveryLatitude,
                    DeliveryLongitude = q.ParentStore.DeliveryLongitude,
                    OwnerName = q.ParentStore.OwnerName,
                    OwnerPhone = q.ParentStore.OwnerPhone,
                    OwnerEmail = q.ParentStore.OwnerEmail,
                    TaxCode = q.ParentStore.TaxCode,
                    LegalEntity = q.ParentStore.LegalEntity,
                    StatusId = q.ParentStore.StatusId,
                } : null,
                Reseller = filter.Selects.Contains(StoreSelect.Reseller) && q.Reseller != null ? new Reseller
                {
                    Id = q.Reseller.Id,
                    Name = q.Reseller.Name,
                    Code = q.Reseller.Code,
                    Email = q.Reseller.Email,
                    Phone = q.Reseller.Phone,
                    CompanyName = q.Reseller.CompanyName,
                    DeputyName = q.Reseller.DeputyName,
                    Address = q.Reseller.Address,
                    Description = q.Reseller.Description,
                    OrganizationId = q.Reseller.OrganizationId,
                    ResellerStatusId = q.Reseller.ResellerStatusId,
                    ResellerTypeId = q.Reseller.ResellerTypeId,
                    StaffId = q.Reseller.StaffId,
                    TaxCode = q.Reseller.TaxCode,
                    StatusId = q.Reseller.StatusId,
                } : null,
                Province = filter.Selects.Contains(StoreSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Code = q.Province.Code,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                } : null,
                AppUser = filter.Selects.Contains(StoreSelect.AppUser) && q.AppUser != null ? new AppUser
                {
                    Id = q.AppUser.Id,
                    Username = q.AppUser.Username,
                    DisplayName = q.AppUser.DisplayName,
                    Address = q.AppUser.Address,
                    Email = q.AppUser.Email,
                    Phone = q.AppUser.Phone,
                    PositionId = q.AppUser.PositionId,
                    Department = q.AppUser.Department,
                    OrganizationId = q.AppUser.OrganizationId,
                    StatusId = q.AppUser.StatusId,
                    Avatar = q.AppUser.Avatar,
                    ProvinceId = q.AppUser.ProvinceId,
                    SexId = q.AppUser.SexId,
                    Birthday = q.AppUser.Birthday,
                } : null,
                Status = filter.Selects.Contains(StoreSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                StoreGrouping = filter.Selects.Contains(StoreSelect.StoreGrouping) && q.StoreGrouping != null ? new StoreGrouping
                {
                    Id = q.StoreGrouping.Id,
                    Code = q.StoreGrouping.Code,
                    Name = q.StoreGrouping.Name,
                    ParentId = q.StoreGrouping.ParentId,
                    Path = q.StoreGrouping.Path,
                    Level = q.StoreGrouping.Level,
                } : null,
                StoreType = filter.Selects.Contains(StoreSelect.StoreType) && q.StoreType != null ? new StoreType
                {
                    Id = q.StoreType.Id,
                    Code = q.StoreType.Code,
                    Name = q.StoreType.Name,
                    StatusId = q.StoreType.StatusId,
                    ColorId = q.StoreType.ColorId,
                    Color = q.StoreType.Color == null ? null : new Color
                    {
                        Id = q.StoreType.Color.Id,
                        Code = q.StoreType.Color.Code,
                        Name = q.StoreType.Color.Name,
                    },
                } : null,
                Ward = filter.Selects.Contains(StoreSelect.Ward) && q.Ward != null ? new Ward
                {
                    Id = q.Ward.Id,
                    Code = q.Ward.Code,
                    Name = q.Ward.Name,
                    Priority = q.Ward.Priority,
                    DistrictId = q.Ward.DistrictId,
                    StatusId = q.Ward.StatusId,
                } : null,
                StoreStatus = filter.Selects.Contains(StoreSelect.StoreStatus) && q.StoreStatus != null ? new StoreStatus
                {
                    Id = q.StoreStatus.Id,
                    Code = q.StoreStatus.Code,
                    Name = q.StoreStatus.Name,
                } : null,
                Used = q.Used,
                CreatedAt = q.CreatedAt,
                RowId = q.RowId,
            }).ToListAsync();

            if (filter.Selects.Contains(StoreSelect.StoreImageMappings))
            {
                var StoreIds = Stores.Select(x => x.Id).ToList();
                ITempTableQuery<TempTable<long>> tempTableQuery = await DataContext
                        .BulkInsertValuesIntoTempTableAsync<long>(StoreIds);
                query = query.Join(tempTableQuery.Query,
                                   c => c.Id,
                                   t => t.Column1,
                                   (c, t) => c);
                var StoreImageMappings = await DataContext.StoreImageMapping
                    .Join(tempTableQuery.Query,
                            c => c.StoreId,
                            t => t.Column1,
                            (c, t) => c).Select(x => new StoreImageMapping
                            {
                                StoreId = x.StoreId,
                                ImageId = x.ImageId,
                                Image = x.Image == null ? null : new Image
                                {
                                    Id = x.Image.Id,
                                    Name = x.Image.Name,
                                    Url = x.Image.Url,
                                    ThumbnailUrl = x.Image.ThumbnailUrl,
                                }
                            }).ToListAsync();
                foreach (var Store in Stores)
                {
                    Store.StoreImageMappings = StoreImageMappings.Where(x => x.StoreId == Store.Id).Skip(0).Take(1).ToList();
                }
            }

            return Stores;
        }

        public async Task<int> Count(StoreFilter filter)
        {
            IQueryable<StoreDAO> Stores = DataContext.Store;
            Stores = await DynamicFilter(Stores, filter);
            int count = await Stores.CountAsync();
            return count;
        }

        public async Task<List<Store>> List(StoreFilter filter)
        {
            if (filter == null) return new List<Store>();
            IQueryable<StoreDAO> StoreDAOs = DataContext.Store.AsNoTracking();
            StoreDAOs = await DynamicFilter(StoreDAOs, filter);
            StoreDAOs = DynamicOrder(StoreDAOs, filter);
            List<Store> Stores = await DynamicSelect(StoreDAOs, filter);
            return Stores;
        }

        public async Task<int> CountInScoped(StoreFilter filter, long AppUserId)
        {
            if (filter == null) return 0;
            AppUserDAO AppUserDAO = await DataContext.AppUser.Where(x => x.Id == AppUserId)
                .Include(x => x.Organization)
                .FirstOrDefaultAsync();
            List<long> StoreIds = await DataContext.AppUserStoreMapping
                .Where(au => au.AppUserId == AppUserId)
                .Select(au => au.StoreId)
                .ToListAsync();
            if (StoreIds.Count == 0)
            {
                OrganizationDAO OrganizationDAO = await DataContext.Organization.Where(x => x.Id == AppUserDAO.OrganizationId).FirstOrDefaultAsync();
                StoreIds = await DataContext.Store
                .Where(x => x.Organization.Path.StartsWith(OrganizationDAO.Path))
                .Select(x => x.Id)
                .ToListAsync();
            }

            List<long> DraftStoreIds = await DataContext.Store
                .Where(x =>
                    x.StoreStatusId == StoreStatusEnum.DRAFT.Id &&
                    x.Organization.Path.StartsWith(AppUserDAO.Organization.Path) &&
                    (x.AppUserId.HasValue == false || x.AppUserId == AppUserId) &&
                    x.DeletedAt == null)
              .Select(x => x.Id).ToListAsync();
            //cộng thêm đại lý dự thảo
            StoreIds.AddRange(DraftStoreIds);
            StoreIds = StoreIds.Distinct().ToList();
            if (filter.Id == null) filter.Id = new IdFilter();
            if (filter.Id.In == null || filter.Id.In.Count == 0)
                filter.Id.In = StoreIds;

            //nếu nhân viên đã có phạm vi đi tuyến, lấy giao giữa tập đc phân quyền và phạm vi đi tuyến
            filter.Id.In = filter.Id.In.Intersect(StoreIds).ToList();

            IQueryable<StoreDAO> Stores = DataContext.Store;
            Stores = await DynamicFilter(Stores, filter);
            int count = await Stores.CountAsync();
            return count;
        }

        public async Task<List<Store>> ListInScoped(StoreFilter filter, long AppUserId)
        {
            if (filter == null) return new List<Store>();
            AppUserDAO AppUserDAO = await DataContext.AppUser.Where(x => x.Id == AppUserId)
                .Include(x => x.Organization)
                .FirstOrDefaultAsync();
            List<long> StoreIds = await DataContext.AppUserStoreMapping
                .Where(au => au.AppUserId == AppUserId)
                .Select(au => au.StoreId)
                .ToListAsync();
            if (StoreIds.Count == 0)
            {
                OrganizationDAO OrganizationDAO = await DataContext.Organization.Where(x => x.Id == AppUserDAO.OrganizationId).FirstOrDefaultAsync();
                StoreIds = await DataContext.Store
                .Where(x => x.Organization.Path.StartsWith(OrganizationDAO.Path))
                .Select(x => x.Id)
                .ToListAsync();
            }
            List<long> DraftStoreIds = await DataContext.Store
                .Where(x =>
                    x.StoreStatusId == StoreStatusEnum.DRAFT.Id &&
                    x.Organization.Path.StartsWith(AppUserDAO.Organization.Path) &&
                    (x.AppUserId.HasValue == false || x.AppUserId == AppUserId) &&
                    x.DeletedAt == null)
              .Select(x => x.Id).ToListAsync();

            //cộng thêm đại lý dự thảo
            StoreIds.AddRange(DraftStoreIds);
            StoreIds = StoreIds.Distinct().ToList();
            if (filter.Id == null) filter.Id = new IdFilter();
            if (filter.Id.In == null || filter.Id.In.Count == 0)
                filter.Id.In = StoreIds;

            //nếu nhân viên đã có phạm vi đi tuyến, lấy giao giữa tập đc phân quyền và phạm vi đi tuyến
            filter.Id.In = filter.Id.In.Intersect(StoreIds).ToList();

            IQueryable<StoreDAO> StoreDAOs = DataContext.Store.AsNoTracking();
            StoreDAOs = await DynamicFilter(StoreDAOs, filter);
            StoreDAOs = DynamicOrder(StoreDAOs, filter);
            List<Store> Stores = await DynamicSelect(StoreDAOs, filter);
            return Stores;
        }

        public async Task<Store> Get(long Id)
        {
            Store Store = await DataContext.Store.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Store()
                {
                    Id = x.Id,
                    Code = x.Code,
                    CodeDraft = x.CodeDraft,
                    Name = x.Name,
                    UnsignName = x.UnsignName,
                    ParentStoreId = x.ParentStoreId,
                    OrganizationId = x.OrganizationId,
                    StoreTypeId = x.StoreTypeId,
                    StoreGroupingId = x.StoreGroupingId,
                    Telephone = x.Telephone,
                    ResellerId = x.ResellerId,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    WardId = x.WardId,
                    Address = x.Address,
                    UnsignAddress = x.UnsignAddress,
                    DeliveryAddress = x.DeliveryAddress,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    DeliveryLatitude = x.DeliveryLatitude,
                    DeliveryLongitude = x.DeliveryLongitude,
                    OwnerName = x.OwnerName,
                    OwnerPhone = x.OwnerPhone,
                    OwnerEmail = x.OwnerEmail,
                    TaxCode = x.TaxCode,
                    LegalEntity = x.LegalEntity,
                    StatusId = x.StatusId,
                    RowId = x.RowId,
                    Used = x.Used,
                    StoreScoutingId = x.StoreScoutingId,
                    AppUserId = x.AppUserId,
                    StoreStatusId = x.StoreStatusId,
                    StoreScouting = x.StoreScouting == null ? null : new StoreScouting
                    {
                        Id = x.StoreScouting.Id,
                        Code = x.StoreScouting.Code,
                        Name = x.StoreScouting.Name,
                    },
                    District = x.District == null ? null : new District
                    {
                        Id = x.District.Id,
                        Name = x.District.Name,
                        Priority = x.District.Priority,
                        ProvinceId = x.District.ProvinceId,
                        StatusId = x.District.StatusId,
                    },
                    Organization = x.Organization == null ? null : new Organization
                    {
                        Id = x.Organization.Id,
                        Code = x.Organization.Code,
                        Name = x.Organization.Name,
                        ParentId = x.Organization.ParentId,
                        Path = x.Organization.Path,
                        Level = x.Organization.Level,
                        StatusId = x.Organization.StatusId,
                        Phone = x.Organization.Phone,
                        Address = x.Organization.Address,
                        Email = x.Organization.Email,
                    },
                    ParentStore = x.ParentStore == null ? null : new Store
                    {
                        Id = x.ParentStore.Id,
                        Code = x.ParentStore.Code,
                        Name = x.ParentStore.Name,
                        ParentStoreId = x.ParentStore.ParentStoreId,
                        OrganizationId = x.ParentStore.OrganizationId,
                        StoreTypeId = x.ParentStore.StoreTypeId,
                        StoreGroupingId = x.ParentStore.StoreGroupingId,
                        Telephone = x.ParentStore.Telephone,
                        ProvinceId = x.ParentStore.ProvinceId,
                        DistrictId = x.ParentStore.DistrictId,
                        WardId = x.ParentStore.WardId,
                        Address = x.ParentStore.Address,
                        DeliveryAddress = x.ParentStore.DeliveryAddress,
                        Latitude = x.ParentStore.Latitude,
                        Longitude = x.ParentStore.Longitude,
                        OwnerName = x.ParentStore.OwnerName,
                        OwnerPhone = x.ParentStore.OwnerPhone,
                        OwnerEmail = x.ParentStore.OwnerEmail,
                        TaxCode = x.ParentStore.TaxCode,
                        LegalEntity = x.ParentStore.LegalEntity,
                        StatusId = x.ParentStore.StatusId,
                    },
                    Province = x.Province == null ? null : new Province
                    {
                        Id = x.Province.Id,
                        Name = x.Province.Name,
                        Priority = x.Province.Priority,
                        StatusId = x.Province.StatusId,
                    },
                    Reseller = x.Reseller == null ? null : new Reseller
                    {
                        Id = x.Reseller.Id,
                        Name = x.Reseller.Name,
                        Code = x.Reseller.Code,
                        Email = x.Reseller.Email,
                        Phone = x.Reseller.Phone,
                        CompanyName = x.Reseller.CompanyName,
                        DeputyName = x.Reseller.DeputyName,
                        Address = x.Reseller.Address,
                        Description = x.Reseller.Description,
                        OrganizationId = x.Reseller.OrganizationId,
                        ResellerStatusId = x.Reseller.ResellerStatusId,
                        ResellerTypeId = x.Reseller.ResellerTypeId,
                        StaffId = x.Reseller.StaffId,
                        TaxCode = x.Reseller.TaxCode,
                        StatusId = x.Reseller.StatusId,
                    },
                    AppUser = x.AppUser == null ? null : new AppUser
                    {
                        Id = x.AppUser.Id,
                        Username = x.AppUser.Username,
                        DisplayName = x.AppUser.DisplayName,
                        Address = x.AppUser.Address,
                        Email = x.AppUser.Email,
                        Phone = x.AppUser.Phone,
                        PositionId = x.AppUser.PositionId,
                        Department = x.AppUser.Department,
                        OrganizationId = x.AppUser.OrganizationId,
                        StatusId = x.AppUser.StatusId,
                        Avatar = x.AppUser.Avatar,
                        ProvinceId = x.AppUser.ProvinceId,
                        SexId = x.AppUser.SexId,
                        Birthday = x.AppUser.Birthday,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    StoreGrouping = x.StoreGrouping == null ? null : new StoreGrouping
                    {
                        Id = x.StoreGrouping.Id,
                        Code = x.StoreGrouping.Code,
                        Name = x.StoreGrouping.Name,
                        ParentId = x.StoreGrouping.ParentId,
                        Path = x.StoreGrouping.Path,
                        Level = x.StoreGrouping.Level,
                    },
                    StoreType = x.StoreType == null ? null : new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        ColorId = x.StoreType.ColorId,
                        StatusId = x.StoreType.StatusId,
                        Color = x.StoreType.Color == null ? null : new Color
                        {
                            Id = x.StoreType.Color.Id,
                            Code = x.StoreType.Color.Code,
                            Name = x.StoreType.Color.Name,
                        }
                    },
                    Ward = x.Ward == null ? null : new Ward
                    {
                        Id = x.Ward.Id,
                        Name = x.Ward.Name,
                        Priority = x.Ward.Priority,
                        DistrictId = x.Ward.DistrictId,
                        StatusId = x.Ward.StatusId,
                    },
                    StoreStatus = x.StoreStatus == null ? null : new StoreStatus
                    {
                        Id = x.StoreStatus.Id,
                        Code = x.StoreStatus.Code,
                        Name = x.StoreStatus.Name,
                    },
                }).FirstOrDefaultAsync();

            if (Store == null)
                return null;

            Store.StoreImageMappings = await DataContext.StoreImageMapping
                .Where(x => x.StoreId == Id).Select(x => new StoreImageMapping
                {
                    StoreId = x.StoreId,
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                        ThumbnailUrl = x.Image.ThumbnailUrl,
                    }
                }).ToListAsync();

            Store.AlbumImageMappings = await DataContext.AlbumImageMapping
                .Where(x => x.StoreId == Id).Select(x => new AlbumImageMapping
                {
                    StoreId = x.StoreId,
                    ImageId = x.ImageId,
                    ShootingAt = x.ShootingAt,
                    SaleEmployeeId = x.SaleEmployeeId,
                    Distance = x.Distance,
                    AlbumId = x.AlbumId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                        ThumbnailUrl = x.Image.ThumbnailUrl,
                    }
                }).ToListAsync();
            return Store;
        }
        public async Task<bool> Create(Store Store)
        {
            StoreDAO StoreDAO = new StoreDAO();
            StoreDAO.Id = Store.Id;
            StoreDAO.Code = Store.Code;
            StoreDAO.CodeDraft = Store.CodeDraft;
            StoreDAO.Name = Store.Name;
            StoreDAO.UnsignName = Store.UnsignName;
            StoreDAO.ParentStoreId = Store.ParentStoreId;
            StoreDAO.OrganizationId = Store.OrganizationId;
            StoreDAO.StoreTypeId = Store.StoreTypeId;
            StoreDAO.StoreGroupingId = Store.StoreGroupingId;
            StoreDAO.ResellerId = Store.ResellerId;
            StoreDAO.Telephone = Store.Telephone;
            StoreDAO.ProvinceId = Store.ProvinceId;
            StoreDAO.DistrictId = Store.DistrictId;
            StoreDAO.WardId = Store.WardId;
            StoreDAO.Address = Store.Address;
            StoreDAO.UnsignAddress = Store.UnsignAddress;
            StoreDAO.DeliveryAddress = Store.DeliveryAddress;
            StoreDAO.Latitude = Store.Latitude;
            StoreDAO.Longitude = Store.Longitude;
            StoreDAO.DeliveryLatitude = Store.DeliveryLatitude;
            StoreDAO.DeliveryLongitude = Store.DeliveryLongitude;
            StoreDAO.OwnerName = Store.OwnerName;
            StoreDAO.OwnerPhone = Store.OwnerPhone;
            StoreDAO.OwnerEmail = Store.OwnerEmail;
            StoreDAO.TaxCode = Store.TaxCode;
            StoreDAO.LegalEntity = Store.LegalEntity;
            StoreDAO.StatusId = Store.StatusId;
            StoreDAO.AppUserId = Store.AppUserId;
            StoreDAO.StoreStatusId = Store.StoreStatusId;
            StoreDAO.StoreScoutingId = Store.StoreScoutingId;
            StoreDAO.RowId = Guid.NewGuid();
            StoreDAO.CreatedAt = StaticParams.DateTimeNow;
            StoreDAO.UpdatedAt = StaticParams.DateTimeNow;
            StoreDAO.Used = false;
            DataContext.Store.Add(StoreDAO);
            await DataContext.SaveChangesAsync();
            Store.Id = StoreDAO.Id;
            Store.RowId = StoreDAO.RowId;
            await SaveReference(Store);
            return true;
        }

        public async Task<bool> Update(Store Store)
        {
            StoreDAO StoreDAO = DataContext.Store.Where(x => x.Id == Store.Id).FirstOrDefault();
            if (StoreDAO == null)
                return false;
            StoreDAO.Id = Store.Id;
            StoreDAO.Code = Store.Code;
            StoreDAO.CodeDraft = Store.CodeDraft;
            StoreDAO.Name = Store.Name;
            StoreDAO.UnsignName = Store.UnsignName;
            StoreDAO.ParentStoreId = Store.ParentStoreId;
            StoreDAO.OrganizationId = Store.OrganizationId;
            StoreDAO.StoreTypeId = Store.StoreTypeId;
            StoreDAO.StoreGroupingId = Store.StoreGroupingId;
            StoreDAO.Telephone = Store.Telephone;
            StoreDAO.ResellerId = Store.ResellerId;
            StoreDAO.ProvinceId = Store.ProvinceId;
            StoreDAO.DistrictId = Store.DistrictId;
            StoreDAO.WardId = Store.WardId;
            StoreDAO.Address = Store.Address;
            StoreDAO.UnsignAddress = Store.UnsignAddress;
            StoreDAO.DeliveryAddress = Store.DeliveryAddress;
            StoreDAO.Latitude = Store.Latitude;
            StoreDAO.Longitude = Store.Longitude;
            StoreDAO.DeliveryLatitude = Store.DeliveryLatitude;
            StoreDAO.DeliveryLongitude = Store.DeliveryLongitude;
            StoreDAO.OwnerName = Store.OwnerName;
            StoreDAO.OwnerPhone = Store.OwnerPhone;
            StoreDAO.OwnerEmail = Store.OwnerEmail;
            StoreDAO.TaxCode = Store.TaxCode;
            StoreDAO.LegalEntity = Store.LegalEntity;
            StoreDAO.StatusId = Store.StatusId;
            StoreDAO.AppUserId = Store.AppUserId;
            StoreDAO.StoreStatusId = Store.StoreStatusId;
            StoreDAO.StoreScoutingId = Store.StoreScoutingId;
            StoreDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Store);
            return true;
        }

        public async Task<bool> Delete(Store Store)
        {
            await DataContext.Store.Where(x => x.ParentStoreId == Store.Id).UpdateFromQueryAsync(x => new StoreDAO { ParentStoreId = null });
            await DataContext.Store.Where(x => x.Id == Store.Id).UpdateFromQueryAsync(x => new StoreDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<Store> Stores)
        {
            List<StoreDAO> StoreDAOs = new List<StoreDAO>();
            foreach (Store Store in Stores)
            {
                StoreDAO StoreDAO = new StoreDAO();
                StoreDAO.Id = Store.Id;
                StoreDAO.Code = Store.Code;
                StoreDAO.CodeDraft = Store.CodeDraft;
                StoreDAO.Name = Store.Name;
                StoreDAO.UnsignName = Store.UnsignName;
                StoreDAO.ParentStoreId = Store.ParentStoreId;
                StoreDAO.OrganizationId = Store.OrganizationId;
                StoreDAO.StoreTypeId = Store.StoreTypeId;
                StoreDAO.StoreGroupingId = Store.StoreGroupingId;
                StoreDAO.Telephone = Store.Telephone;
                StoreDAO.ResellerId = Store.ResellerId;
                StoreDAO.ProvinceId = Store.ProvinceId;
                StoreDAO.DistrictId = Store.DistrictId;
                StoreDAO.WardId = Store.WardId;
                StoreDAO.Address = Store.Address;
                StoreDAO.UnsignAddress = Store.UnsignAddress;
                StoreDAO.DeliveryAddress = Store.DeliveryAddress;
                StoreDAO.Latitude = Store.Latitude;
                StoreDAO.Longitude = Store.Longitude;
                StoreDAO.DeliveryLatitude = Store.DeliveryLatitude;
                StoreDAO.DeliveryLongitude = Store.DeliveryLongitude;
                StoreDAO.OwnerName = Store.OwnerName;
                StoreDAO.OwnerPhone = Store.OwnerPhone;
                StoreDAO.OwnerEmail = Store.OwnerEmail;
                StoreDAO.TaxCode = Store.TaxCode;
                StoreDAO.LegalEntity = Store.LegalEntity;
                StoreDAO.StatusId = Store.StatusId;
                StoreDAO.AppUserId = Store.AppUserId;
                StoreDAO.StoreStatusId = StoreStatusEnum.OFFICIAL.Id;
                StoreDAO.StoreScoutingId = Store.StoreScoutingId;
                StoreDAO.RowId = Store.RowId;
                StoreDAO.CreatedAt = StaticParams.DateTimeNow;
                StoreDAO.UpdatedAt = StaticParams.DateTimeNow;
                StoreDAOs.Add(StoreDAO);
            }
            await DataContext.BulkMergeAsync(StoreDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Store> Stores)
        {
            List<long> Ids = Stores.Select(x => x.Id).ToList();
            await DataContext.Store.Where(x => Ids.Contains(x.ParentStoreId.Value)).UpdateFromQueryAsync(x => new StoreDAO { ParentStoreId = null });
            await DataContext.Store
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new StoreDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Store Store)
        {
            List<AlbumImageMappingDAO> AlbumImageMappingDAOs = await DataContext.AlbumImageMapping.Where(x => x.StoreId == Store.Id).ToListAsync();
            AlbumImageMappingDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (Store.AlbumImageMappings != null)
            {
                foreach (var AlbumImageMapping in Store.AlbumImageMappings)
                {
                    AlbumImageMappingDAO AlbumImageMappingDAO = AlbumImageMappingDAOs.Where(x => x.AlbumId == AlbumImageMapping.AlbumId && x.ImageId == AlbumImageMapping.ImageId).FirstOrDefault();
                    if (AlbumImageMappingDAO == null)
                    {
                        AlbumImageMappingDAO = new AlbumImageMappingDAO();
                        AlbumImageMappingDAO.AlbumId = AlbumImageMapping.AlbumId;
                        AlbumImageMappingDAO.ImageId = AlbumImageMapping.ImageId;
                        AlbumImageMappingDAO.StoreId = Store.Id;
                        AlbumImageMappingDAO.SaleEmployeeId = AlbumImageMapping.SaleEmployeeId;
                        AlbumImageMappingDAO.ShootingAt = StaticParams.DateTimeNow;
                        AlbumImageMappingDAO.DeletedAt = null;
                        AlbumImageMappingDAOs.Add(AlbumImageMappingDAO);
                    }
                    else
                    {
                        AlbumImageMappingDAO.AlbumId = AlbumImageMapping.AlbumId;
                        AlbumImageMappingDAO.ShootingAt = AlbumImageMapping.ShootingAt;
                        AlbumImageMappingDAO.DeletedAt = null;
                    }
                }
            }
            await DataContext.AlbumImageMapping.BulkMergeAsync(AlbumImageMappingDAOs);

            await DataContext.StoreImageMapping.Where(x => x.StoreId == Store.Id).DeleteFromQueryAsync();
            List<StoreImageMappingDAO> StoreImageMappingDAOs = new List<StoreImageMappingDAO>();
            if (Store.StoreImageMappings != null)
            {
                foreach (StoreImageMapping StoreImageMapping in Store.StoreImageMappings)
                {
                    StoreImageMappingDAO StoreImageMappingDAO = new StoreImageMappingDAO();
                    StoreImageMappingDAO.StoreId = Store.Id;
                    StoreImageMappingDAO.ImageId = StoreImageMapping.ImageId;
                    StoreImageMappingDAOs.Add(StoreImageMappingDAO);
                }
                await DataContext.StoreImageMapping.BulkMergeAsync(StoreImageMappingDAOs);
            }
        }

    }
}
