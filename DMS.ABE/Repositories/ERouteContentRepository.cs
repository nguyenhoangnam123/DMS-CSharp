using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ABE.Repositories
{
    public interface IERouteContentRepository
    {
        Task<int> Count(ERouteContentFilter ERouteContentFilter);
        Task<List<ERouteContent>> List(ERouteContentFilter ERouteContentFilter);
        Task<ERouteContent> Get(long Id);
        Task<bool> Create(ERouteContent ERouteContent);
        Task<bool> Update(ERouteContent ERouteContent);
        Task<bool> Delete(ERouteContent ERouteContent);
        Task<bool> BulkMerge(List<ERouteContent> ERouteContents);
        Task<bool> BulkDelete(List<ERouteContent> ERouteContents);
    }
    public class ERouteContentRepository : IERouteContentRepository
    {
        private DataContext DataContext;
        public ERouteContentRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ERouteContentDAO> DynamicFilter(IQueryable<ERouteContentDAO> query, ERouteContentFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.ERouteId != null)
                query = query.Where(q => q.ERouteId, filter.ERouteId);
            if (filter.StoreId != null)
                query = query.Where(q => q.StoreId, filter.StoreId);
            if (filter.OrderNumber != null)
                query = query.Where(q => q.OrderNumber, filter.OrderNumber);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ERouteContentDAO> OrFilter(IQueryable<ERouteContentDAO> query, ERouteContentFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ERouteContentDAO> initQuery = query.Where(q => false);
            foreach (ERouteContentFilter ERouteContentFilter in filter.OrFilter)
            {
                IQueryable<ERouteContentDAO> queryable = query;
                if (ERouteContentFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ERouteContentFilter.Id);
                if (ERouteContentFilter.ERouteId != null)
                    queryable = queryable.Where(q => q.ERouteId, ERouteContentFilter.ERouteId);
                if (ERouteContentFilter.StoreId != null)
                    queryable = queryable.Where(q => q.StoreId, ERouteContentFilter.StoreId);
                if (ERouteContentFilter.OrderNumber != null)
                    queryable = queryable.Where(q => q.OrderNumber, ERouteContentFilter.OrderNumber);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ERouteContentDAO> DynamicOrder(IQueryable<ERouteContentDAO> query, ERouteContentFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ERouteContentOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ERouteContentOrder.ERoute:
                            query = query.OrderBy(q => q.ERouteId);
                            break;
                        case ERouteContentOrder.Store:
                            query = query.OrderBy(q => q.StoreId);
                            break;
                        case ERouteContentOrder.OrderNumber:
                            query = query.OrderBy(q => q.OrderNumber);
                            break;
                        case ERouteContentOrder.Monday:
                            query = query.OrderBy(q => q.Monday);
                            break;
                        case ERouteContentOrder.Tuesday:
                            query = query.OrderBy(q => q.Tuesday);
                            break;
                        case ERouteContentOrder.Wednesday:
                            query = query.OrderBy(q => q.Wednesday);
                            break;
                        case ERouteContentOrder.Thursday:
                            query = query.OrderBy(q => q.Thursday);
                            break;
                        case ERouteContentOrder.Friday:
                            query = query.OrderBy(q => q.Friday);
                            break;
                        case ERouteContentOrder.Saturday:
                            query = query.OrderBy(q => q.Saturday);
                            break;
                        case ERouteContentOrder.Sunday:
                            query = query.OrderBy(q => q.Sunday);
                            break;
                        case ERouteContentOrder.Week1:
                            query = query.OrderBy(q => q.Week1);
                            break;
                        case ERouteContentOrder.Week2:
                            query = query.OrderBy(q => q.Week2);
                            break;
                        case ERouteContentOrder.Week3:
                            query = query.OrderBy(q => q.Week3);
                            break;
                        case ERouteContentOrder.Week4:
                            query = query.OrderBy(q => q.Week4);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ERouteContentOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ERouteContentOrder.ERoute:
                            query = query.OrderByDescending(q => q.ERouteId);
                            break;
                        case ERouteContentOrder.Store:
                            query = query.OrderByDescending(q => q.StoreId);
                            break;
                        case ERouteContentOrder.OrderNumber:
                            query = query.OrderByDescending(q => q.OrderNumber);
                            break;
                        case ERouteContentOrder.Monday:
                            query = query.OrderByDescending(q => q.Monday);
                            break;
                        case ERouteContentOrder.Tuesday:
                            query = query.OrderByDescending(q => q.Tuesday);
                            break;
                        case ERouteContentOrder.Wednesday:
                            query = query.OrderByDescending(q => q.Wednesday);
                            break;
                        case ERouteContentOrder.Thursday:
                            query = query.OrderByDescending(q => q.Thursday);
                            break;
                        case ERouteContentOrder.Friday:
                            query = query.OrderByDescending(q => q.Friday);
                            break;
                        case ERouteContentOrder.Saturday:
                            query = query.OrderByDescending(q => q.Saturday);
                            break;
                        case ERouteContentOrder.Sunday:
                            query = query.OrderByDescending(q => q.Sunday);
                            break;
                        case ERouteContentOrder.Week1:
                            query = query.OrderByDescending(q => q.Week1);
                            break;
                        case ERouteContentOrder.Week2:
                            query = query.OrderByDescending(q => q.Week2);
                            break;
                        case ERouteContentOrder.Week3:
                            query = query.OrderByDescending(q => q.Week3);
                            break;
                        case ERouteContentOrder.Week4:
                            query = query.OrderByDescending(q => q.Week4);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ERouteContent>> DynamicSelect(IQueryable<ERouteContentDAO> query, ERouteContentFilter filter)
        {
            List<ERouteContent> ERouteContents = await query.Select(q => new ERouteContent()
            {
                Id = filter.Selects.Contains(ERouteContentSelect.Id) ? q.Id : default(long),
                ERouteId = filter.Selects.Contains(ERouteContentSelect.ERoute) ? q.ERouteId : default(long),
                StoreId = filter.Selects.Contains(ERouteContentSelect.Store) ? q.StoreId : default(long),
                OrderNumber = filter.Selects.Contains(ERouteContentSelect.OrderNumber) ? q.OrderNumber : default(long?),
                Monday = filter.Selects.Contains(ERouteContentSelect.Monday) ? q.Monday : default(bool),
                Tuesday = filter.Selects.Contains(ERouteContentSelect.Tuesday) ? q.Tuesday : default(bool),
                Wednesday = filter.Selects.Contains(ERouteContentSelect.Wednesday) ? q.Wednesday : default(bool),
                Thursday = filter.Selects.Contains(ERouteContentSelect.Thursday) ? q.Thursday : default(bool),
                Friday = filter.Selects.Contains(ERouteContentSelect.Friday) ? q.Friday : default(bool),
                Saturday = filter.Selects.Contains(ERouteContentSelect.Saturday) ? q.Saturday : default(bool),
                Sunday = filter.Selects.Contains(ERouteContentSelect.Sunday) ? q.Sunday : default(bool),
                Week1 = filter.Selects.Contains(ERouteContentSelect.Week1) ? q.Week1 : default(bool),
                Week2 = filter.Selects.Contains(ERouteContentSelect.Week2) ? q.Week2 : default(bool),
                Week3 = filter.Selects.Contains(ERouteContentSelect.Week3) ? q.Week3 : default(bool),
                Week4 = filter.Selects.Contains(ERouteContentSelect.Week4) ? q.Week4 : default(bool),
                ERoute = filter.Selects.Contains(ERouteContentSelect.ERoute) && q.ERoute != null ? new ERoute
                {
                    Id = q.ERoute.Id,
                    Code = q.ERoute.Code,
                    Name = q.ERoute.Name,
                    SaleEmployeeId = q.ERoute.SaleEmployeeId,
                    StartDate = q.ERoute.StartDate,
                    RealStartDate = q.ERoute.RealStartDate,
                    EndDate = q.ERoute.EndDate,
                    StatusId = q.ERoute.StatusId,
                    CreatorId = q.ERoute.CreatorId,
                } : null,
                Store = filter.Selects.Contains(ERouteContentSelect.Store) && q.Store != null ? new Store
                {
                    Id = q.Store.Id,
                    Code = q.Store.Code,
                    CodeDraft = q.Store.CodeDraft,
                    Name = q.Store.Name,
                    ParentStoreId = q.Store.ParentStoreId,
                    OrganizationId = q.Store.OrganizationId,
                    StoreTypeId = q.Store.StoreTypeId,
                    StoreGroupingId = q.Store.StoreGroupingId,
                    Telephone = q.Store.Telephone,
                    ProvinceId = q.Store.ProvinceId,
                    DistrictId = q.Store.DistrictId,
                    WardId = q.Store.WardId,
                    Address = q.Store.Address,
                    DeliveryAddress = q.Store.DeliveryAddress,
                    Latitude = q.Store.Latitude,
                    Longitude = q.Store.Longitude,
                    DeliveryLatitude = q.Store.DeliveryLatitude,
                    DeliveryLongitude = q.Store.DeliveryLongitude,
                    OwnerName = q.Store.OwnerName,
                    OwnerPhone = q.Store.OwnerPhone,
                    OwnerEmail = q.Store.OwnerEmail,
                    TaxCode = q.Store.TaxCode,
                    LegalEntity = q.Store.LegalEntity,
                    StatusId = q.Store.StatusId,
                } : null,
            }).ToListAsync();

            var Ids = ERouteContents.Select(x => x.Id).ToList();
            var ERouteContentDays = await DataContext.ERouteContentDay.Where(x => Ids.Contains(x.ERouteContentId)).Select(x => new ERouteContentDay
            {
                Id = x.Id,
                ERouteContentId = x.ERouteContentId,
                OrderDay = x.OrderDay,
                Planned = x.Planned,
            }).ToListAsync();

            foreach (var ERouteContent in ERouteContents)
            {
                ERouteContent.ERouteContentDays = ERouteContentDays.Where(x => x.ERouteContentId == ERouteContent.Id).ToList();
            }
            return ERouteContents;
        }

        public async Task<int> Count(ERouteContentFilter filter)
        {
            IQueryable<ERouteContentDAO> ERouteContents = DataContext.ERouteContent.AsNoTracking();
            ERouteContents = DynamicFilter(ERouteContents, filter);
            return await ERouteContents.CountAsync();
        }

        public async Task<List<ERouteContent>> List(ERouteContentFilter filter)
        {
            if (filter == null) return new List<ERouteContent>();
            IQueryable<ERouteContentDAO> ERouteContentDAOs = DataContext.ERouteContent.AsNoTracking();
            ERouteContentDAOs = DynamicFilter(ERouteContentDAOs, filter);
            ERouteContentDAOs = DynamicOrder(ERouteContentDAOs, filter);
            List<ERouteContent> ERouteContents = await DynamicSelect(ERouteContentDAOs, filter);
            return ERouteContents;
        }

        public async Task<ERouteContent> Get(long Id)
        {
            ERouteContent ERouteContent = await DataContext.ERouteContent.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new ERouteContent()
            {
                Id = x.Id,
                ERouteId = x.ERouteId,
                StoreId = x.StoreId,
                OrderNumber = x.OrderNumber,
                Monday = x.Monday,
                Tuesday = x.Tuesday,
                Wednesday = x.Wednesday,
                Thursday = x.Thursday,
                Friday = x.Friday,
                Saturday = x.Saturday,
                Sunday = x.Sunday,
                Week1 = x.Week1,
                Week2 = x.Week2,
                Week3 = x.Week3,
                Week4 = x.Week4,
                ERoute = x.ERoute == null ? null : new ERoute
                {
                    Id = x.ERoute.Id,
                    Code = x.ERoute.Code,
                    Name = x.ERoute.Name,
                    SaleEmployeeId = x.ERoute.SaleEmployeeId,
                    StartDate = x.ERoute.StartDate,
                    EndDate = x.ERoute.EndDate,
                    StatusId = x.ERoute.StatusId,
                    CreatorId = x.ERoute.CreatorId,
                },
                Store = x.Store == null ? null : new Store
                {
                    Id = x.Store.Id,
                    Code = x.Store.Code,
                    CodeDraft = x.Store.CodeDraft,
                    Name = x.Store.Name,
                    ParentStoreId = x.Store.ParentStoreId,
                    OrganizationId = x.Store.OrganizationId,
                    StoreTypeId = x.Store.StoreTypeId,
                    StoreGroupingId = x.Store.StoreGroupingId,
                    Telephone = x.Store.Telephone,
                    ProvinceId = x.Store.ProvinceId,
                    DistrictId = x.Store.DistrictId,
                    WardId = x.Store.WardId,
                    Address = x.Store.Address,
                    DeliveryAddress = x.Store.DeliveryAddress,
                    Latitude = x.Store.Latitude,
                    Longitude = x.Store.Longitude,
                    DeliveryLatitude = x.Store.DeliveryLatitude,
                    DeliveryLongitude = x.Store.DeliveryLongitude,
                    OwnerName = x.Store.OwnerName,
                    OwnerPhone = x.Store.OwnerPhone,
                    OwnerEmail = x.Store.OwnerEmail,
                    TaxCode = x.Store.TaxCode,
                    LegalEntity = x.Store.LegalEntity,
                    StatusId = x.Store.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (ERouteContent == null)
                return null;

            return ERouteContent;
        }
        public async Task<bool> Create(ERouteContent ERouteContent)
        {
            ERouteContentDAO ERouteContentDAO = new ERouteContentDAO();
            ERouteContentDAO.Id = ERouteContent.Id;
            ERouteContentDAO.ERouteId = ERouteContent.ERouteId;
            ERouteContentDAO.StoreId = ERouteContent.StoreId;
            ERouteContentDAO.OrderNumber = ERouteContent.OrderNumber;
            ERouteContentDAO.Monday = ERouteContent.Monday;
            ERouteContentDAO.Tuesday = ERouteContent.Tuesday;
            ERouteContentDAO.Wednesday = ERouteContent.Wednesday;
            ERouteContentDAO.Thursday = ERouteContent.Thursday;
            ERouteContentDAO.Friday = ERouteContent.Friday;
            ERouteContentDAO.Saturday = ERouteContent.Saturday;
            ERouteContentDAO.Sunday = ERouteContent.Sunday;
            ERouteContentDAO.Week1 = ERouteContent.Week1;
            ERouteContentDAO.Week2 = ERouteContent.Week2;
            ERouteContentDAO.Week3 = ERouteContent.Week3;
            ERouteContentDAO.Week4 = ERouteContent.Week4;
            DataContext.ERouteContent.Add(ERouteContentDAO);
            await DataContext.SaveChangesAsync();
            ERouteContent.Id = ERouteContentDAO.Id;
            await SaveReference(ERouteContent);
            return true;
        }

        public async Task<bool> Update(ERouteContent ERouteContent)
        {
            ERouteContentDAO ERouteContentDAO = DataContext.ERouteContent.Where(x => x.Id == ERouteContent.Id).FirstOrDefault();
            if (ERouteContentDAO == null)
                return false;
            ERouteContentDAO.Id = ERouteContent.Id;
            ERouteContentDAO.ERouteId = ERouteContent.ERouteId;
            ERouteContentDAO.StoreId = ERouteContent.StoreId;
            ERouteContentDAO.OrderNumber = ERouteContent.OrderNumber;
            ERouteContentDAO.Monday = ERouteContent.Monday;
            ERouteContentDAO.Tuesday = ERouteContent.Tuesday;
            ERouteContentDAO.Wednesday = ERouteContent.Wednesday;
            ERouteContentDAO.Thursday = ERouteContent.Thursday;
            ERouteContentDAO.Friday = ERouteContent.Friday;
            ERouteContentDAO.Saturday = ERouteContent.Saturday;
            ERouteContentDAO.Sunday = ERouteContent.Sunday;
            ERouteContentDAO.Week1 = ERouteContent.Week1;
            ERouteContentDAO.Week2 = ERouteContent.Week2;
            ERouteContentDAO.Week3 = ERouteContent.Week3;
            ERouteContentDAO.Week4 = ERouteContent.Week4;
            await DataContext.SaveChangesAsync();
            await SaveReference(ERouteContent);
            return true;
        }

        public async Task<bool> Delete(ERouteContent ERouteContent)
        {
            await DataContext.ERouteContent.Where(x => x.Id == ERouteContent.Id).DeleteFromQueryAsync();
            return true;
        }

        public async Task<bool> BulkMerge(List<ERouteContent> ERouteContents)
        {
            List<ERouteContentDAO> ERouteContentDAOs = new List<ERouteContentDAO>();
            foreach (ERouteContent ERouteContent in ERouteContents)
            {
                ERouteContentDAO ERouteContentDAO = new ERouteContentDAO();
                ERouteContentDAO.Id = ERouteContent.Id;
                ERouteContentDAO.ERouteId = ERouteContent.ERouteId;
                ERouteContentDAO.StoreId = ERouteContent.StoreId;
                ERouteContentDAO.OrderNumber = ERouteContent.OrderNumber;
                ERouteContentDAO.Monday = ERouteContent.Monday;
                ERouteContentDAO.Tuesday = ERouteContent.Tuesday;
                ERouteContentDAO.Wednesday = ERouteContent.Wednesday;
                ERouteContentDAO.Thursday = ERouteContent.Thursday;
                ERouteContentDAO.Friday = ERouteContent.Friday;
                ERouteContentDAO.Saturday = ERouteContent.Saturday;
                ERouteContentDAO.Sunday = ERouteContent.Sunday;
                ERouteContentDAO.Week1 = ERouteContent.Week1;
                ERouteContentDAO.Week2 = ERouteContent.Week2;
                ERouteContentDAO.Week3 = ERouteContent.Week3;
                ERouteContentDAO.Week4 = ERouteContent.Week4;
                ERouteContentDAOs.Add(ERouteContentDAO);
            }
            await DataContext.BulkMergeAsync(ERouteContentDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ERouteContent> ERouteContents)
        {
            List<long> Ids = ERouteContents.Select(x => x.Id).ToList();
            await DataContext.ERouteContent
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(ERouteContent ERouteContent)
        {
        }

    }
}
