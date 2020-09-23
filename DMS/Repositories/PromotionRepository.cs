using Common;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;

namespace DMS.Repositories
{
    public interface IPromotionRepository
    {
        Task<int> Count(PromotionFilter PromotionFilter);
        Task<List<Promotion>> List(PromotionFilter PromotionFilter);
        Task<Promotion> Get(long Id);
        Task<bool> Create(Promotion Promotion);
        Task<bool> Update(Promotion Promotion);
        Task<bool> Delete(Promotion Promotion);
        Task<bool> BulkMerge(List<Promotion> Promotions);
        Task<bool> BulkDelete(List<Promotion> Promotions);
    }
    public class PromotionRepository : IPromotionRepository
    {
        private DataContext DataContext;
        public PromotionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PromotionDAO> DynamicFilter(IQueryable<PromotionDAO> query, PromotionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.StartDate != null)
                query = query.Where(q => q.StartDate, filter.StartDate);
            if (filter.EndDate != null)
                query = query.Where(q => q.EndDate == null).Union(query.Where(q => q.EndDate.HasValue).Where(q => q.EndDate, filter.EndDate));
            if (filter.OrganizationId != null)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.PromotionTypeId != null)
                query = query.Where(q => q.PromotionTypeId, filter.PromotionTypeId);
            if (filter.Note != null)
                query = query.Where(q => q.Note, filter.Note);
            if (filter.Priority != null)
                query = query.Where(q => q.Priority, filter.Priority);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PromotionDAO> OrFilter(IQueryable<PromotionDAO> query, PromotionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PromotionDAO> initQuery = query.Where(q => false);
            foreach (PromotionFilter PromotionFilter in filter.OrFilter)
            {
                IQueryable<PromotionDAO> queryable = query;
                if (PromotionFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PromotionFilter.Id);
                if (PromotionFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, PromotionFilter.Code);
                if (PromotionFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, PromotionFilter.Name);
                if (PromotionFilter.StartDate != null)
                    queryable = queryable.Where(q => q.StartDate, PromotionFilter.StartDate);
                if (PromotionFilter.EndDate != null)
                    queryable = queryable.Where(q => q.EndDate.HasValue).Where(q => q.EndDate, PromotionFilter.EndDate);
                if (PromotionFilter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, PromotionFilter.OrganizationId);
                if (PromotionFilter.PromotionTypeId != null)
                    queryable = queryable.Where(q => q.PromotionTypeId, PromotionFilter.PromotionTypeId);
                if (PromotionFilter.Note != null)
                    queryable = queryable.Where(q => q.Note, PromotionFilter.Note);
                if (PromotionFilter.Priority != null)
                    queryable = queryable.Where(q => q.Priority, PromotionFilter.Priority);
                if (PromotionFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, PromotionFilter.StatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PromotionDAO> DynamicOrder(IQueryable<PromotionDAO> query, PromotionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PromotionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PromotionOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PromotionOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PromotionOrder.StartDate:
                            query = query.OrderBy(q => q.StartDate);
                            break;
                        case PromotionOrder.EndDate:
                            query = query.OrderBy(q => q.EndDate);
                            break;
                        case PromotionOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case PromotionOrder.PromotionType:
                            query = query.OrderBy(q => q.PromotionTypeId);
                            break;
                        case PromotionOrder.Note:
                            query = query.OrderBy(q => q.Note);
                            break;
                        case PromotionOrder.Priority:
                            query = query.OrderBy(q => q.Priority);
                            break;
                        case PromotionOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PromotionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PromotionOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PromotionOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PromotionOrder.StartDate:
                            query = query.OrderByDescending(q => q.StartDate);
                            break;
                        case PromotionOrder.EndDate:
                            query = query.OrderByDescending(q => q.EndDate);
                            break;
                        case PromotionOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case PromotionOrder.PromotionType:
                            query = query.OrderByDescending(q => q.PromotionTypeId);
                            break;
                        case PromotionOrder.Note:
                            query = query.OrderByDescending(q => q.Note);
                            break;
                        case PromotionOrder.Priority:
                            query = query.OrderByDescending(q => q.Priority);
                            break;
                        case PromotionOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Promotion>> DynamicSelect(IQueryable<PromotionDAO> query, PromotionFilter filter)
        {
            List<Promotion> Promotions = await query.Select(q => new Promotion()
            {
                Id = filter.Selects.Contains(PromotionSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PromotionSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PromotionSelect.Name) ? q.Name : default(string),
                StartDate = filter.Selects.Contains(PromotionSelect.StartDate) ? q.StartDate : default(DateTime),
                EndDate = filter.Selects.Contains(PromotionSelect.EndDate) ? q.EndDate : default(DateTime?),
                OrganizationId = filter.Selects.Contains(PromotionSelect.Organization) ? q.OrganizationId : default(long),
                PromotionTypeId = filter.Selects.Contains(PromotionSelect.PromotionType) ? q.PromotionTypeId : default(long),
                Note = filter.Selects.Contains(PromotionSelect.Note) ? q.Note : default(string),
                Priority = filter.Selects.Contains(PromotionSelect.Priority) ? q.Priority : default(long),
                StatusId = filter.Selects.Contains(PromotionSelect.Status) ? q.StatusId : default(long),
                Organization = filter.Selects.Contains(PromotionSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    ParentId = q.Organization.ParentId,
                    Path = q.Organization.Path,
                    Level = q.Organization.Level,
                    StatusId = q.Organization.StatusId,
                    Phone = q.Organization.Phone,
                    Email = q.Organization.Email,
                    Address = q.Organization.Address,
                } : null,
                PromotionType = filter.Selects.Contains(PromotionSelect.PromotionType) && q.PromotionType != null ? new PromotionType
                {
                    Id = q.PromotionType.Id,
                    Code = q.PromotionType.Code,
                    Name = q.PromotionType.Name,
                } : null,
                Status = filter.Selects.Contains(PromotionSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return Promotions;
        }

        public async Task<int> Count(PromotionFilter filter)
        {
            IQueryable<PromotionDAO> Promotions = DataContext.Promotion.AsNoTracking();
            Promotions = DynamicFilter(Promotions, filter);
            return await Promotions.CountAsync();
        }

        public async Task<List<Promotion>> List(PromotionFilter filter)
        {
            if (filter == null) return new List<Promotion>();
            IQueryable<PromotionDAO> PromotionDAOs = DataContext.Promotion.AsNoTracking();
            PromotionDAOs = DynamicFilter(PromotionDAOs, filter);
            PromotionDAOs = DynamicOrder(PromotionDAOs, filter);
            List<Promotion> Promotions = await DynamicSelect(PromotionDAOs, filter);
            return Promotions;
        }

        public async Task<Promotion> Get(long Id)
        {
            Promotion Promotion = await DataContext.Promotion.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Promotion()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                OrganizationId = x.OrganizationId,
                PromotionTypeId = x.PromotionTypeId,
                Note = x.Note,
                Priority = x.Priority,
                StatusId = x.StatusId,
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
                    Email = x.Organization.Email,
                    Address = x.Organization.Address,
                },
                PromotionType = x.PromotionType == null ? null : new PromotionType
                {
                    Id = x.PromotionType.Id,
                    Code = x.PromotionType.Code,
                    Name = x.PromotionType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (Promotion == null)
                return null;
            Promotion.PromotionCombos = await DataContext.PromotionCombo.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Select(x => new PromotionCombo
                {
                    Id = x.Id,
                    Note = x.Note,
                    PromotionId = x.PromotionId,
                }).ToListAsync();
            Promotion.PromotionDirectSalesOrders = await DataContext.PromotionDirectSalesOrder.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Select(x => new PromotionDirectSalesOrder
                {
                    Id = x.Id,
                    PromotionId = x.PromotionId,
                    Note = x.Note,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountValue = x.DiscountValue,
                    PromotionDiscountType = new PromotionDiscountType
                    {
                        Id = x.PromotionDiscountType.Id,
                        Code = x.PromotionDiscountType.Code,
                        Name = x.PromotionDiscountType.Name,
                    },
                }).ToListAsync();
            Promotion.PromotionPromotionPolicyMappings = await DataContext.PromotionPromotionPolicyMapping.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Select(x => new PromotionPromotionPolicyMapping
                {
                    PromotionId = x.PromotionId,
                    PromotionPolicyId = x.PromotionPolicyId,
                    Note = x.Note,
                    StatusId = x.StatusId,
                    PromotionPolicy = new PromotionPolicy
                    {
                        Id = x.PromotionPolicy.Id,
                        Code = x.PromotionPolicy.Code,
                        Name = x.PromotionPolicy.Name,
                    },
                    Status = new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                }).ToListAsync();
            Promotion.PromotionSamePrices = await DataContext.PromotionSamePrice.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Select(x => new PromotionSamePrice
                {
                    Id = x.Id,
                    Note = x.Note,
                    PromotionId = x.PromotionId,
                    Price = x.Price,
                }).ToListAsync();
            Promotion.PromotionStoreGroupingMappings = await DataContext.PromotionStoreGroupingMapping.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Where(x => x.StoreGrouping.DeletedAt == null)
                .Select(x => new PromotionStoreGroupingMapping
                {
                    PromotionId = x.PromotionId,
                    StoreGroupingId = x.StoreGroupingId,
                    StoreGrouping = new StoreGrouping
                    {
                        Id = x.StoreGrouping.Id,
                        Code = x.StoreGrouping.Code,
                        Name = x.StoreGrouping.Name,
                        ParentId = x.StoreGrouping.ParentId,
                        Path = x.StoreGrouping.Path,
                        Level = x.StoreGrouping.Level,
                        StatusId = x.StoreGrouping.StatusId,
                    },
                }).ToListAsync();
            Promotion.PromotionStoreGroupings = await DataContext.PromotionStoreGrouping.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Select(x => new PromotionStoreGrouping
                {
                    Id = x.Id,
                    PromotionId = x.PromotionId,
                    Note = x.Note,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountValue = x.DiscountValue,
                    PromotionDiscountType = new PromotionDiscountType
                    {
                        Id = x.PromotionDiscountType.Id,
                        Code = x.PromotionDiscountType.Code,
                        Name = x.PromotionDiscountType.Name,
                    },
                }).ToListAsync();
            Promotion.PromotionStoreMappings = await DataContext.PromotionStoreMapping.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Where(x => x.Store.DeletedAt == null)
                .Select(x => new PromotionStoreMapping
                {
                    PromotionId = x.PromotionId,
                    StoreId = x.StoreId,
                    Store = new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        Name = x.Store.Name,
                        UnsignName = x.Store.UnsignName,
                        ParentStoreId = x.Store.ParentStoreId,
                        OrganizationId = x.Store.OrganizationId,
                        StoreTypeId = x.Store.StoreTypeId,
                        StoreGroupingId = x.Store.StoreGroupingId,
                        ResellerId = x.Store.ResellerId,
                        Telephone = x.Store.Telephone,
                        ProvinceId = x.Store.ProvinceId,
                        DistrictId = x.Store.DistrictId,
                        WardId = x.Store.WardId,
                        Address = x.Store.Address,
                        UnsignAddress = x.Store.UnsignAddress,
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
                        Used = x.Store.Used,
                        StoreScoutingId = x.Store.StoreScoutingId,
                    },
                }).ToListAsync();
            Promotion.PromotionStoreTypeMappings = await DataContext.PromotionStoreTypeMapping.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Where(x => x.StoreType.DeletedAt == null)
                .Select(x => new PromotionStoreTypeMapping
                {
                    PromotionId = x.PromotionId,
                    StoreTypeId = x.StoreTypeId,
                    StoreType = new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        ColorId = x.StoreType.ColorId,
                        StatusId = x.StoreType.StatusId,
                        Used = x.StoreType.Used,
                    },
                }).ToListAsync();
            Promotion.PromotionStoreTypes = await DataContext.PromotionStoreType.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Select(x => new PromotionStoreType
                {
                    Id = x.Id,
                    PromotionId = x.PromotionId,
                    Note = x.Note,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountValue = x.DiscountValue,
                    PromotionDiscountType = new PromotionDiscountType
                    {
                        Id = x.PromotionDiscountType.Id,
                        Code = x.PromotionDiscountType.Code,
                        Name = x.PromotionDiscountType.Name,
                    },
                }).ToListAsync();
            Promotion.PromotionStores = await DataContext.PromotionStore.AsNoTracking()
                .Where(x => x.PromotionId == Promotion.Id)
                .Select(x => new PromotionStore
                {
                    Id = x.Id,
                    PromotionId = x.PromotionId,
                    Note = x.Note,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                    DiscountPercentage = x.DiscountPercentage,
                    DiscountValue = x.DiscountValue,
                    PromotionDiscountType = new PromotionDiscountType
                    {
                        Id = x.PromotionDiscountType.Id,
                        Code = x.PromotionDiscountType.Code,
                        Name = x.PromotionDiscountType.Name,
                    },
                }).ToListAsync();

            return Promotion;
        }
        public async Task<bool> Create(Promotion Promotion)
        {
            PromotionDAO PromotionDAO = new PromotionDAO();
            PromotionDAO.Id = Promotion.Id;
            PromotionDAO.Code = Promotion.Code;
            PromotionDAO.Name = Promotion.Name;
            PromotionDAO.StartDate = Promotion.StartDate;
            PromotionDAO.EndDate = Promotion.EndDate;
            PromotionDAO.OrganizationId = Promotion.OrganizationId;
            PromotionDAO.PromotionTypeId = Promotion.PromotionTypeId;
            PromotionDAO.Note = Promotion.Note;
            PromotionDAO.Priority = Promotion.Priority;
            PromotionDAO.StatusId = Promotion.StatusId;
            PromotionDAO.CreatedAt = StaticParams.DateTimeNow;
            PromotionDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Promotion.Add(PromotionDAO);
            await DataContext.SaveChangesAsync();
            Promotion.Id = PromotionDAO.Id;
            await SaveReference(Promotion);
            return true;
        }

        public async Task<bool> Update(Promotion Promotion)
        {
            PromotionDAO PromotionDAO = DataContext.Promotion.Where(x => x.Id == Promotion.Id).FirstOrDefault();
            if (PromotionDAO == null)
                return false;
            PromotionDAO.Id = Promotion.Id;
            PromotionDAO.Code = Promotion.Code;
            PromotionDAO.Name = Promotion.Name;
            PromotionDAO.StartDate = Promotion.StartDate;
            PromotionDAO.EndDate = Promotion.EndDate;
            PromotionDAO.OrganizationId = Promotion.OrganizationId;
            PromotionDAO.PromotionTypeId = Promotion.PromotionTypeId;
            PromotionDAO.Note = Promotion.Note;
            PromotionDAO.Priority = Promotion.Priority;
            PromotionDAO.StatusId = Promotion.StatusId;
            PromotionDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Promotion);
            return true;
        }

        public async Task<bool> Delete(Promotion Promotion)
        {
            await DataContext.Promotion.Where(x => x.Id == Promotion.Id).UpdateFromQueryAsync(x => new PromotionDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Promotion> Promotions)
        {
            List<PromotionDAO> PromotionDAOs = new List<PromotionDAO>();
            foreach (Promotion Promotion in Promotions)
            {
                PromotionDAO PromotionDAO = new PromotionDAO();
                PromotionDAO.Id = Promotion.Id;
                PromotionDAO.Code = Promotion.Code;
                PromotionDAO.Name = Promotion.Name;
                PromotionDAO.StartDate = Promotion.StartDate;
                PromotionDAO.EndDate = Promotion.EndDate;
                PromotionDAO.OrganizationId = Promotion.OrganizationId;
                PromotionDAO.PromotionTypeId = Promotion.PromotionTypeId;
                PromotionDAO.Note = Promotion.Note;
                PromotionDAO.Priority = Promotion.Priority;
                PromotionDAO.StatusId = Promotion.StatusId;
                PromotionDAO.CreatedAt = StaticParams.DateTimeNow;
                PromotionDAO.UpdatedAt = StaticParams.DateTimeNow;
                PromotionDAOs.Add(PromotionDAO);
            }
            await DataContext.BulkMergeAsync(PromotionDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Promotion> Promotions)
        {
            List<long> Ids = Promotions.Select(x => x.Id).ToList();
            await DataContext.Promotion
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new PromotionDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Promotion Promotion)
        {
            await DataContext.PromotionCombo
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionComboDAO> PromotionComboDAOs = new List<PromotionComboDAO>();
            if (Promotion.PromotionCombos != null)
            {
                foreach (PromotionCombo PromotionCombo in Promotion.PromotionCombos)
                {
                    PromotionComboDAO PromotionComboDAO = new PromotionComboDAO();
                    PromotionComboDAO.Id = PromotionCombo.Id;
                    PromotionComboDAO.Note = PromotionCombo.Note;
                    PromotionComboDAO.PromotionId = Promotion.Id;
                    PromotionComboDAOs.Add(PromotionComboDAO);
                }
                await DataContext.PromotionCombo.BulkMergeAsync(PromotionComboDAOs);
            }
            await DataContext.PromotionDirectSalesOrder
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionDirectSalesOrderDAO> PromotionDirectSalesOrderDAOs = new List<PromotionDirectSalesOrderDAO>();
            if (Promotion.PromotionDirectSalesOrders != null)
            {
                foreach (PromotionDirectSalesOrder PromotionDirectSalesOrder in Promotion.PromotionDirectSalesOrders)
                {
                    PromotionDirectSalesOrderDAO PromotionDirectSalesOrderDAO = new PromotionDirectSalesOrderDAO();
                    PromotionDirectSalesOrderDAO.Id = PromotionDirectSalesOrder.Id;
                    PromotionDirectSalesOrderDAO.PromotionId = Promotion.Id;
                    PromotionDirectSalesOrderDAO.Note = PromotionDirectSalesOrder.Note;
                    PromotionDirectSalesOrderDAO.FromValue = PromotionDirectSalesOrder.FromValue;
                    PromotionDirectSalesOrderDAO.ToValue = PromotionDirectSalesOrder.ToValue;
                    PromotionDirectSalesOrderDAO.PromotionDiscountTypeId = PromotionDirectSalesOrder.PromotionDiscountTypeId;
                    PromotionDirectSalesOrderDAO.DiscountPercentage = PromotionDirectSalesOrder.DiscountPercentage;
                    PromotionDirectSalesOrderDAO.DiscountValue = PromotionDirectSalesOrder.DiscountValue;
                    PromotionDirectSalesOrderDAOs.Add(PromotionDirectSalesOrderDAO);
                }
                await DataContext.PromotionDirectSalesOrder.BulkMergeAsync(PromotionDirectSalesOrderDAOs);
            }
            await DataContext.PromotionPromotionPolicyMapping
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionPromotionPolicyMappingDAO> PromotionPromotionPolicyMappingDAOs = new List<PromotionPromotionPolicyMappingDAO>();
            if (Promotion.PromotionPromotionPolicyMappings != null)
            {
                foreach (PromotionPromotionPolicyMapping PromotionPromotionPolicyMapping in Promotion.PromotionPromotionPolicyMappings)
                {
                    PromotionPromotionPolicyMappingDAO PromotionPromotionPolicyMappingDAO = new PromotionPromotionPolicyMappingDAO();
                    PromotionPromotionPolicyMappingDAO.PromotionId = Promotion.Id;
                    PromotionPromotionPolicyMappingDAO.PromotionPolicyId = PromotionPromotionPolicyMapping.PromotionPolicyId;
                    PromotionPromotionPolicyMappingDAO.Note = PromotionPromotionPolicyMapping.Note;
                    PromotionPromotionPolicyMappingDAO.StatusId = PromotionPromotionPolicyMapping.StatusId;
                    PromotionPromotionPolicyMappingDAOs.Add(PromotionPromotionPolicyMappingDAO);
                }
                await DataContext.PromotionPromotionPolicyMapping.BulkMergeAsync(PromotionPromotionPolicyMappingDAOs);
            }
            await DataContext.PromotionSamePrice
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionSamePriceDAO> PromotionSamePriceDAOs = new List<PromotionSamePriceDAO>();
            if (Promotion.PromotionSamePrices != null)
            {
                foreach (PromotionSamePrice PromotionSamePrice in Promotion.PromotionSamePrices)
                {
                    PromotionSamePriceDAO PromotionSamePriceDAO = new PromotionSamePriceDAO();
                    PromotionSamePriceDAO.Id = PromotionSamePrice.Id;
                    PromotionSamePriceDAO.Note = PromotionSamePrice.Note;
                    PromotionSamePriceDAO.PromotionId = Promotion.Id;
                    PromotionSamePriceDAO.Price = PromotionSamePrice.Price;
                    PromotionSamePriceDAOs.Add(PromotionSamePriceDAO);
                }
                await DataContext.PromotionSamePrice.BulkMergeAsync(PromotionSamePriceDAOs);
            }
            await DataContext.PromotionStoreGroupingMapping
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionStoreGroupingMappingDAO> PromotionStoreGroupingMappingDAOs = new List<PromotionStoreGroupingMappingDAO>();
            if (Promotion.PromotionStoreGroupingMappings != null)
            {
                foreach (PromotionStoreGroupingMapping PromotionStoreGroupingMapping in Promotion.PromotionStoreGroupingMappings)
                {
                    PromotionStoreGroupingMappingDAO PromotionStoreGroupingMappingDAO = new PromotionStoreGroupingMappingDAO();
                    PromotionStoreGroupingMappingDAO.PromotionId = Promotion.Id;
                    PromotionStoreGroupingMappingDAO.StoreGroupingId = PromotionStoreGroupingMapping.StoreGroupingId;
                    PromotionStoreGroupingMappingDAOs.Add(PromotionStoreGroupingMappingDAO);
                }
                await DataContext.PromotionStoreGroupingMapping.BulkMergeAsync(PromotionStoreGroupingMappingDAOs);
            }
            await DataContext.PromotionStoreGrouping
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionStoreGroupingDAO> PromotionStoreGroupingDAOs = new List<PromotionStoreGroupingDAO>();
            if (Promotion.PromotionStoreGroupings != null)
            {
                foreach (PromotionStoreGrouping PromotionStoreGrouping in Promotion.PromotionStoreGroupings)
                {
                    PromotionStoreGroupingDAO PromotionStoreGroupingDAO = new PromotionStoreGroupingDAO();
                    PromotionStoreGroupingDAO.Id = PromotionStoreGrouping.Id;
                    PromotionStoreGroupingDAO.PromotionId = Promotion.Id;
                    PromotionStoreGroupingDAO.Note = PromotionStoreGrouping.Note;
                    PromotionStoreGroupingDAO.FromValue = PromotionStoreGrouping.FromValue;
                    PromotionStoreGroupingDAO.ToValue = PromotionStoreGrouping.ToValue;
                    PromotionStoreGroupingDAO.PromotionDiscountTypeId = PromotionStoreGrouping.PromotionDiscountTypeId;
                    PromotionStoreGroupingDAO.DiscountPercentage = PromotionStoreGrouping.DiscountPercentage;
                    PromotionStoreGroupingDAO.DiscountValue = PromotionStoreGrouping.DiscountValue;
                    PromotionStoreGroupingDAOs.Add(PromotionStoreGroupingDAO);
                }
                await DataContext.PromotionStoreGrouping.BulkMergeAsync(PromotionStoreGroupingDAOs);
            }
            await DataContext.PromotionStoreMapping
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionStoreMappingDAO> PromotionStoreMappingDAOs = new List<PromotionStoreMappingDAO>();
            if (Promotion.PromotionStoreMappings != null)
            {
                foreach (PromotionStoreMapping PromotionStoreMapping in Promotion.PromotionStoreMappings)
                {
                    PromotionStoreMappingDAO PromotionStoreMappingDAO = new PromotionStoreMappingDAO();
                    PromotionStoreMappingDAO.PromotionId = Promotion.Id;
                    PromotionStoreMappingDAO.StoreId = PromotionStoreMapping.StoreId;
                    PromotionStoreMappingDAOs.Add(PromotionStoreMappingDAO);
                }
                await DataContext.PromotionStoreMapping.BulkMergeAsync(PromotionStoreMappingDAOs);
            }
            await DataContext.PromotionStoreTypeMapping
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionStoreTypeMappingDAO> PromotionStoreTypeMappingDAOs = new List<PromotionStoreTypeMappingDAO>();
            if (Promotion.PromotionStoreTypeMappings != null)
            {
                foreach (PromotionStoreTypeMapping PromotionStoreTypeMapping in Promotion.PromotionStoreTypeMappings)
                {
                    PromotionStoreTypeMappingDAO PromotionStoreTypeMappingDAO = new PromotionStoreTypeMappingDAO();
                    PromotionStoreTypeMappingDAO.PromotionId = Promotion.Id;
                    PromotionStoreTypeMappingDAO.StoreTypeId = PromotionStoreTypeMapping.StoreTypeId;
                    PromotionStoreTypeMappingDAOs.Add(PromotionStoreTypeMappingDAO);
                }
                await DataContext.PromotionStoreTypeMapping.BulkMergeAsync(PromotionStoreTypeMappingDAOs);
            }
            await DataContext.PromotionStoreType
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionStoreTypeDAO> PromotionStoreTypeDAOs = new List<PromotionStoreTypeDAO>();
            if (Promotion.PromotionStoreTypes != null)
            {
                foreach (PromotionStoreType PromotionStoreType in Promotion.PromotionStoreTypes)
                {
                    PromotionStoreTypeDAO PromotionStoreTypeDAO = new PromotionStoreTypeDAO();
                    PromotionStoreTypeDAO.Id = PromotionStoreType.Id;
                    PromotionStoreTypeDAO.PromotionId = Promotion.Id;
                    PromotionStoreTypeDAO.Note = PromotionStoreType.Note;
                    PromotionStoreTypeDAO.FromValue = PromotionStoreType.FromValue;
                    PromotionStoreTypeDAO.ToValue = PromotionStoreType.ToValue;
                    PromotionStoreTypeDAO.PromotionDiscountTypeId = PromotionStoreType.PromotionDiscountTypeId;
                    PromotionStoreTypeDAO.DiscountPercentage = PromotionStoreType.DiscountPercentage;
                    PromotionStoreTypeDAO.DiscountValue = PromotionStoreType.DiscountValue;
                    PromotionStoreTypeDAOs.Add(PromotionStoreTypeDAO);
                }
                await DataContext.PromotionStoreType.BulkMergeAsync(PromotionStoreTypeDAOs);
            }
            await DataContext.PromotionStore
                .Where(x => x.PromotionId == Promotion.Id)
                .DeleteFromQueryAsync();
            List<PromotionStoreDAO> PromotionStoreDAOs = new List<PromotionStoreDAO>();
            if (Promotion.PromotionStores != null)
            {
                foreach (PromotionStore PromotionStore in Promotion.PromotionStores)
                {
                    PromotionStoreDAO PromotionStoreDAO = new PromotionStoreDAO();
                    PromotionStoreDAO.Id = PromotionStore.Id;
                    PromotionStoreDAO.PromotionId = Promotion.Id;
                    PromotionStoreDAO.Note = PromotionStore.Note;
                    PromotionStoreDAO.FromValue = PromotionStore.FromValue;
                    PromotionStoreDAO.ToValue = PromotionStore.ToValue;
                    PromotionStoreDAO.PromotionDiscountTypeId = PromotionStore.PromotionDiscountTypeId;
                    PromotionStoreDAO.DiscountPercentage = PromotionStore.DiscountPercentage;
                    PromotionStoreDAO.DiscountValue = PromotionStore.DiscountValue;
                    PromotionStoreDAOs.Add(PromotionStoreDAO);
                }
                await DataContext.PromotionStore.BulkMergeAsync(PromotionStoreDAOs);
            }
        }
        
    }
}
