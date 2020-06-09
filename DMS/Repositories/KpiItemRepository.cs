using Common;
using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IKpiItemRepository
    {
        Task<int> Count(KpiItemFilter KpiItemFilter);
        Task<List<KpiItem>> List(KpiItemFilter KpiItemFilter);
        Task<KpiItem> Get(long Id);
        Task<bool> Create(KpiItem KpiItem);
        Task<bool> Update(KpiItem KpiItem);
        Task<bool> Delete(KpiItem KpiItem);
        Task<bool> BulkMerge(List<KpiItem> KpiItems);
        Task<bool> BulkDelete(List<KpiItem> KpiItems);
    }
    public class KpiItemRepository : IKpiItemRepository
    {
        private DataContext DataContext;
        public KpiItemRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<KpiItemDAO> DynamicFilter(IQueryable<KpiItemDAO> query, KpiItemFilter filter)
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
            if (filter.OrganizationId != null)
                query = query.Where(q => q.OrganizationId, filter.OrganizationId);
            if (filter.KpiPeriodId != null)
                query = query.Where(q => q.KpiPeriodId, filter.KpiPeriodId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.EmployeeId != null)
                query = query.Where(q => q.EmployeeId, filter.EmployeeId);
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<KpiItemDAO> OrFilter(IQueryable<KpiItemDAO> query, KpiItemFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<KpiItemDAO> initQuery = query.Where(q => false);
            foreach (KpiItemFilter KpiItemFilter in filter.OrFilter)
            {
                IQueryable<KpiItemDAO> queryable = query;
                if (KpiItemFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, KpiItemFilter.Id);
                if (KpiItemFilter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, KpiItemFilter.OrganizationId);
                if (KpiItemFilter.KpiPeriodId != null)
                    queryable = queryable.Where(q => q.KpiPeriodId, KpiItemFilter.KpiPeriodId);
                if (KpiItemFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, KpiItemFilter.StatusId);
                if (KpiItemFilter.EmployeeId != null)
                    queryable = queryable.Where(q => q.EmployeeId, KpiItemFilter.EmployeeId);
                if (KpiItemFilter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, KpiItemFilter.CreatorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<KpiItemDAO> DynamicOrder(IQueryable<KpiItemDAO> query, KpiItemFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case KpiItemOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case KpiItemOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case KpiItemOrder.KpiPeriod:
                            query = query.OrderBy(q => q.KpiPeriodId);
                            break;
                        case KpiItemOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case KpiItemOrder.Employee:
                            query = query.OrderBy(q => q.EmployeeId);
                            break;
                        case KpiItemOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case KpiItemOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case KpiItemOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case KpiItemOrder.KpiPeriod:
                            query = query.OrderByDescending(q => q.KpiPeriodId);
                            break;
                        case KpiItemOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case KpiItemOrder.Employee:
                            query = query.OrderByDescending(q => q.EmployeeId);
                            break;
                        case KpiItemOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<KpiItem>> DynamicSelect(IQueryable<KpiItemDAO> query, KpiItemFilter filter)
        {
            List<KpiItem> KpiItems = await query.Select(q => new KpiItem()
            {
                Id = filter.Selects.Contains(KpiItemSelect.Id) ? q.Id : default(long),
                OrganizationId = filter.Selects.Contains(KpiItemSelect.Organization) ? q.OrganizationId : default(long),
                KpiPeriodId = filter.Selects.Contains(KpiItemSelect.KpiPeriod) ? q.KpiPeriodId : default(long),
                StatusId = filter.Selects.Contains(KpiItemSelect.Status) ? q.StatusId : default(long),
                EmployeeId = filter.Selects.Contains(KpiItemSelect.Employee) ? q.EmployeeId : default(long),
                CreatorId = filter.Selects.Contains(KpiItemSelect.Creator) ? q.CreatorId : default(long),
                Creator = filter.Selects.Contains(KpiItemSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    Address = q.Creator.Address,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                    PositionId = q.Creator.PositionId,
                    Department = q.Creator.Department,
                    OrganizationId = q.Creator.OrganizationId,
                    StatusId = q.Creator.StatusId,
                    Avatar = q.Creator.Avatar,
                    ProvinceId = q.Creator.ProvinceId,
                    SexId = q.Creator.SexId,
                    Birthday = q.Creator.Birthday,
                } : null,
                Employee = filter.Selects.Contains(KpiItemSelect.Employee) && q.Employee != null ? new AppUser
                {
                    Id = q.Employee.Id,
                    Username = q.Employee.Username,
                    DisplayName = q.Employee.DisplayName,
                    Address = q.Employee.Address,
                    Email = q.Employee.Email,
                    Phone = q.Employee.Phone,
                    PositionId = q.Employee.PositionId,
                    Department = q.Employee.Department,
                    OrganizationId = q.Employee.OrganizationId,
                    StatusId = q.Employee.StatusId,
                    Avatar = q.Employee.Avatar,
                    ProvinceId = q.Employee.ProvinceId,
                    SexId = q.Employee.SexId,
                    Birthday = q.Employee.Birthday,
                } : null,
                KpiPeriod = filter.Selects.Contains(KpiItemSelect.KpiPeriod) && q.KpiPeriod != null ? new KpiPeriod
                {
                    Id = q.KpiPeriod.Id,
                    Code = q.KpiPeriod.Code,
                    Name = q.KpiPeriod.Name,
                } : null,
                Organization = filter.Selects.Contains(KpiItemSelect.Organization) && q.Organization != null ? new Organization
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
                Status = filter.Selects.Contains(KpiItemSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return KpiItems;
        }

        public async Task<int> Count(KpiItemFilter filter)
        {
            IQueryable<KpiItemDAO> KpiItems = DataContext.KpiItem.AsNoTracking();
            KpiItems = DynamicFilter(KpiItems, filter);
            return await KpiItems.CountAsync();
        }

        public async Task<List<KpiItem>> List(KpiItemFilter filter)
        {
            if (filter == null) return new List<KpiItem>();
            IQueryable<KpiItemDAO> KpiItemDAOs = DataContext.KpiItem.AsNoTracking();
            KpiItemDAOs = DynamicFilter(KpiItemDAOs, filter);
            KpiItemDAOs = DynamicOrder(KpiItemDAOs, filter);
            List<KpiItem> KpiItems = await DynamicSelect(KpiItemDAOs, filter);
            return KpiItems;
        }

        public async Task<KpiItem> Get(long Id)
        {
            KpiItem KpiItem = await DataContext.KpiItem.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new KpiItem()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                OrganizationId = x.OrganizationId,
                KpiPeriodId = x.KpiPeriodId,
                StatusId = x.StatusId,
                EmployeeId = x.EmployeeId,
                CreatorId = x.CreatorId,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    PositionId = x.Creator.PositionId,
                    Department = x.Creator.Department,
                    OrganizationId = x.Creator.OrganizationId,
                    StatusId = x.Creator.StatusId,
                    Avatar = x.Creator.Avatar,
                    ProvinceId = x.Creator.ProvinceId,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                },
                Employee = x.Employee == null ? null : new AppUser
                {
                    Id = x.Employee.Id,
                    Username = x.Employee.Username,
                    DisplayName = x.Employee.DisplayName,
                    Address = x.Employee.Address,
                    Email = x.Employee.Email,
                    Phone = x.Employee.Phone,
                    PositionId = x.Employee.PositionId,
                    Department = x.Employee.Department,
                    OrganizationId = x.Employee.OrganizationId,
                    StatusId = x.Employee.StatusId,
                    Avatar = x.Employee.Avatar,
                    ProvinceId = x.Employee.ProvinceId,
                    SexId = x.Employee.SexId,
                    Birthday = x.Employee.Birthday,
                },
                KpiPeriod = x.KpiPeriod == null ? null : new KpiPeriod
                {
                    Id = x.KpiPeriod.Id,
                    Code = x.KpiPeriod.Code,
                    Name = x.KpiPeriod.Name,
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
                    Email = x.Organization.Email,
                    Address = x.Organization.Address,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (KpiItem == null)
                return null;
            KpiItem.KpiItemContents = await DataContext.KpiItemContent.AsNoTracking()
                .Where(x => x.KpiItemId == KpiItem.Id)
                .Select(x => new KpiItemContent
                {
                    Id = x.Id,
                    KpiItemId = x.KpiItemId,
                    ItemId = x.ItemId,
                    RowId = x.RowId,
                    Item = new Item
                    {
                        Id = x.Item.Id,
                        ProductId = x.Item.ProductId,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ScanCode = x.Item.ScanCode,
                        SalePrice = x.Item.SalePrice,
                        RetailPrice = x.Item.RetailPrice,
                        StatusId = x.Item.StatusId,
                    },
                }).ToListAsync();
            var KpiItemContentIds = KpiItem.KpiItemContents.Select(x => x.Id).ToList();
            List<KpiItemContentKpiCriteriaItemMapping> KpiItemContentKpiItemCriteriaMappings = await DataContext.KpiItemContentKpiCriteriaItemMapping
                .Where(x => KpiItemContentIds.Contains(x.KpiItemContentId))
                .Select(x => new KpiItemContentKpiCriteriaItemMapping
                {
                    KpiCriteriaItemId = x.KpiCriteriaItemId,
                    KpiItemContentId = x.KpiItemContentId,
                    Value = x.Value,
                    KpiItemContent = new KpiItemContent
                    {
                        Id = x.KpiItemContent.Id,
                        ItemId = x.KpiItemContent.ItemId,
                        KpiItemId = x.KpiItemContent.KpiItemId,
                        Item = new Item
                        {
                            Id = x.KpiItemContent.Item.Id,
                            ProductId = x.KpiItemContent.Item.ProductId,
                            Code = x.KpiItemContent.Item.Code,
                            Name = x.KpiItemContent.Item.Name,
                            ScanCode = x.KpiItemContent.Item.ScanCode,
                            SalePrice = x.KpiItemContent.Item.SalePrice,
                            RetailPrice = x.KpiItemContent.Item.RetailPrice,
                            StatusId = x.KpiItemContent.Item.StatusId,
                        },
                    }
                }).ToListAsync();
            foreach (KpiItemContent KpiItemContent in KpiItem.KpiItemContents)
            {
                KpiItemContent.KpiItemContentKpiCriteriaItemMappings = KpiItemContentKpiItemCriteriaMappings.Where(x => x.KpiItemContentId == KpiItemContent.Id).ToList();
            }
            KpiItem.KpiItemKpiCriteriaTotalMappings = await DataContext.KpiItemKpiCriteriaTotalMapping.AsNoTracking()
                .Where(x => x.KpiItemId == KpiItem.Id)
                .Select(x => new KpiItemKpiCriteriaTotalMapping
                {
                    KpiItemId = x.KpiItemId,
                    KpiCriteriaTotalId = x.KpiCriteriaTotalId,
                    Value = x.Value,
                    KpiCriteriaTotal = new KpiCriteriaTotal
                    {
                        Id = x.KpiCriteriaTotal.Id,
                        Code = x.KpiCriteriaTotal.Code,
                        Name = x.KpiCriteriaTotal.Name,
                    },
                }).ToListAsync();

            return KpiItem;
        }
        public async Task<bool> Create(KpiItem KpiItem)
        {
            KpiItemDAO KpiItemDAO = new KpiItemDAO();
            KpiItemDAO.Id = KpiItem.Id;
            KpiItemDAO.OrganizationId = KpiItem.OrganizationId;
            KpiItemDAO.KpiPeriodId = KpiItem.KpiPeriodId;
            KpiItemDAO.StatusId = KpiItem.StatusId;
            KpiItemDAO.EmployeeId = KpiItem.EmployeeId;
            KpiItemDAO.CreatorId = KpiItem.CreatorId;
            KpiItemDAO.CreatedAt = StaticParams.DateTimeNow;
            KpiItemDAO.UpdatedAt = StaticParams.DateTimeNow;
            KpiItemDAO.RowId = Guid.NewGuid();
            DataContext.KpiItem.Add(KpiItemDAO);
            await DataContext.SaveChangesAsync();
            KpiItem.Id = KpiItemDAO.Id;
            KpiItem.RowId = KpiItemDAO.RowId;
            await SaveReference(KpiItem);
            return true;
        }

        public async Task<bool> Update(KpiItem KpiItem)
        {
            KpiItemDAO KpiItemDAO = DataContext.KpiItem.Where(x => x.Id == KpiItem.Id).FirstOrDefault();
            if (KpiItemDAO == null)
                return false;
            KpiItemDAO.Id = KpiItem.Id;
            KpiItemDAO.OrganizationId = KpiItem.OrganizationId;
            KpiItemDAO.KpiPeriodId = KpiItem.KpiPeriodId;
            KpiItemDAO.StatusId = KpiItem.StatusId;
            KpiItemDAO.EmployeeId = KpiItem.EmployeeId;
            KpiItemDAO.CreatorId = KpiItem.CreatorId;
            KpiItemDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(KpiItem);
            return true;
        }

        public async Task<bool> Delete(KpiItem KpiItem)
        {
            await DataContext.KpiItemContentKpiCriteriaItemMapping.Where(x => x.KpiItemContent.KpiItemId == KpiItem.Id).DeleteFromQueryAsync();
            await DataContext.KpiItemContent.Where(x => x.KpiItemId == KpiItem.Id).DeleteFromQueryAsync();
            await DataContext.KpiItemKpiCriteriaTotalMapping.Where(x => x.KpiItemId == KpiItem.Id).DeleteFromQueryAsync();
            await DataContext.KpiItem.Where(x => x.Id == KpiItem.Id).UpdateFromQueryAsync(x => new KpiItemDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<KpiItem> KpiItems)
        {
            List<KpiItemDAO> KpiItemDAOs = new List<KpiItemDAO>();
            foreach (KpiItem KpiItem in KpiItems)
            {
                KpiItemDAO KpiItemDAO = new KpiItemDAO();
                KpiItemDAO.Id = KpiItem.Id;
                KpiItemDAO.OrganizationId = KpiItem.OrganizationId;
                KpiItemDAO.KpiPeriodId = KpiItem.KpiPeriodId;
                KpiItemDAO.StatusId = KpiItem.StatusId;
                KpiItemDAO.EmployeeId = KpiItem.EmployeeId;
                KpiItemDAO.CreatorId = KpiItem.CreatorId;
                KpiItemDAO.CreatedAt = StaticParams.DateTimeNow;
                KpiItemDAO.UpdatedAt = StaticParams.DateTimeNow;
                KpiItemDAO.RowId = Guid.NewGuid();
                KpiItemDAOs.Add(KpiItemDAO);
                KpiItem.RowId = KpiItemDAO.RowId;
            }
            await DataContext.BulkMergeAsync(KpiItemDAOs);

            var KpiItemContentDAOs = new List<KpiItemContentDAO>();
            foreach (var KpiItem in KpiItems)
            {
                KpiItem.Id = KpiItemDAOs.Where(x => x.RowId == KpiItem.RowId).Select(x => x.Id).FirstOrDefault();
                if (KpiItem.KpiItemContents != null && KpiItem.KpiItemContents.Any())
                {
                    var listContent = KpiItem.KpiItemContents.Select(x => new KpiItemContentDAO
                    {
                        ItemId = x.ItemId,
                        KpiItemId = KpiItem.Id,
                    }).ToList();
                    KpiItemContentDAOs.AddRange(listContent);
                }

            }

            var KpiItemKpiCriteriaTotalMappingDAOs = new List<KpiItemKpiCriteriaTotalMappingDAO>();
            foreach (var KpiItem in KpiItems)
            {
                if (KpiItem.KpiItemKpiCriteriaTotalMappings != null && KpiItem.KpiItemKpiCriteriaTotalMappings.Any())
                {
                    var listTotal = KpiItem.KpiItemKpiCriteriaTotalMappings.Select(x => new KpiItemKpiCriteriaTotalMappingDAO
                    {
                        KpiItemId = KpiItem.Id,
                        KpiCriteriaTotalId = x.KpiCriteriaTotalId,
                        Value = x.Value,
                    }).ToList();
                    KpiItemKpiCriteriaTotalMappingDAOs.AddRange(listTotal);
                }

            }

            await DataContext.KpiItemContent.BulkMergeAsync(KpiItemContentDAOs);
            await DataContext.KpiItemKpiCriteriaTotalMapping.BulkMergeAsync(KpiItemKpiCriteriaTotalMappingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<KpiItem> KpiItems)
        {
            List<long> Ids = KpiItems.Select(x => x.Id).ToList();
            await DataContext.KpiItemContent.Where(x => Ids.Contains(x.KpiItemId)).DeleteFromQueryAsync();
            await DataContext.KpiItemKpiCriteriaTotalMapping.Where(x => Ids.Contains(x.KpiItemId)).DeleteFromQueryAsync();
            await DataContext.KpiItem
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new KpiItemDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(KpiItem KpiItem)
        {
            var KpiItemContentIds = KpiItem.KpiItemContents.Select(x => x.Id).ToList();
            await DataContext.KpiItemContentKpiCriteriaItemMapping
                .Where(x => KpiItemContentIds
                .Contains(x.KpiCriteriaItemId))
                .DeleteFromQueryAsync();
            await DataContext.KpiItemContent
                .Where(x => KpiItemContentIds.Contains(x.Id))
                .DeleteFromQueryAsync();
            List<KpiItemContentDAO> KpiItemContentDAOs = new List<KpiItemContentDAO>();
            List<KpiItemContentKpiCriteriaItemMappingDAO> KpiItemContentKpiCriteriaItemMappingDAOs = new List<KpiItemContentKpiCriteriaItemMappingDAO>();
            if (KpiItem.KpiItemContents != null)
            {
                KpiItem.KpiItemContents.ForEach(x => x.RowId = Guid.NewGuid());
                foreach (KpiItemContent KpiItemContent in KpiItem.KpiItemContents)
                {
                    KpiItemContentDAO KpiItemContentDAO = new KpiItemContentDAO();
                    KpiItemContentDAO.Id = KpiItemContent.Id;
                    KpiItemContentDAO.KpiItemId = KpiItem.Id;
                    KpiItemContentDAO.ItemId = KpiItemContent.ItemId;
                    KpiItemContentDAO.RowId = KpiItemContent.RowId;
                    KpiItemContentDAOs.Add(KpiItemContentDAO);
                }
                await DataContext.KpiItemContent.BulkMergeAsync(KpiItemContentDAOs);

                foreach (KpiItemContent KpiItemContent in KpiItem.KpiItemContents)
                {
                    KpiItemContent.Id = KpiItemContentDAOs.Where(x => x.RowId == KpiItemContent.RowId).Select(x => x.Id).FirstOrDefault();
                    if (KpiItemContent.KpiItemContentKpiCriteriaItemMappings != null)
                    {
                        foreach (KpiItemContentKpiCriteriaItemMapping KpiItemContentKpiCriteriaItemMapping in KpiItemContent.KpiItemContentKpiCriteriaItemMappings)
                        {
                            KpiItemContentKpiCriteriaItemMappingDAO KpiItemContentKpiCriteriaItemMappingDAO = new KpiItemContentKpiCriteriaItemMappingDAO
                            {
                                KpiItemContentId = KpiItemContent.Id,
                                KpiCriteriaItemId = KpiItemContentKpiCriteriaItemMapping.KpiCriteriaItemId,
                                Value = KpiItemContentKpiCriteriaItemMapping.Value
                            };
                            KpiItemContentKpiCriteriaItemMappingDAOs.Add(KpiItemContentKpiCriteriaItemMappingDAO);
                        }
                    }
                }

                await DataContext.KpiItemContentKpiCriteriaItemMapping.BulkMergeAsync(KpiItemContentKpiCriteriaItemMappingDAOs);
            }
            await DataContext.KpiItemKpiCriteriaTotalMapping
                .Where(x => x.KpiItemId == KpiItem.Id)
                .DeleteFromQueryAsync();
            List<KpiItemKpiCriteriaTotalMappingDAO> KpiItemKpiCriteriaTotalMappingDAOs = new List<KpiItemKpiCriteriaTotalMappingDAO>();
            if (KpiItem.KpiItemKpiCriteriaTotalMappings != null)
            {
                foreach (KpiItemKpiCriteriaTotalMapping KpiItemKpiCriteriaTotalMapping in KpiItem.KpiItemKpiCriteriaTotalMappings)
                {
                    KpiItemKpiCriteriaTotalMappingDAO KpiItemKpiCriteriaTotalMappingDAO = new KpiItemKpiCriteriaTotalMappingDAO();
                    KpiItemKpiCriteriaTotalMappingDAO.KpiItemId = KpiItem.Id;
                    KpiItemKpiCriteriaTotalMappingDAO.KpiCriteriaTotalId = KpiItemKpiCriteriaTotalMapping.KpiCriteriaTotalId;
                    KpiItemKpiCriteriaTotalMappingDAO.Value = KpiItemKpiCriteriaTotalMapping.Value;
                    KpiItemKpiCriteriaTotalMappingDAOs.Add(KpiItemKpiCriteriaTotalMappingDAO);
                }
                await DataContext.KpiItemKpiCriteriaTotalMapping.BulkMergeAsync(KpiItemKpiCriteriaTotalMappingDAOs);
            }
        }

    }
}
