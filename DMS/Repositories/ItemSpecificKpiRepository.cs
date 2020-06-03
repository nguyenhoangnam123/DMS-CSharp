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
    public interface IItemSpecificKpiRepository
    {
        Task<int> Count(ItemSpecificKpiFilter ItemSpecificKpiFilter);
        Task<List<ItemSpecificKpi>> List(ItemSpecificKpiFilter ItemSpecificKpiFilter);
        Task<ItemSpecificKpi> Get(long Id);
        Task<bool> Create(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> Update(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> Delete(ItemSpecificKpi ItemSpecificKpi);
        Task<bool> BulkMerge(List<ItemSpecificKpi> ItemSpecificKpis);
        Task<bool> BulkDelete(List<ItemSpecificKpi> ItemSpecificKpis);
    }
    public class ItemSpecificKpiRepository : IItemSpecificKpiRepository
    {
        private DataContext DataContext;
        public ItemSpecificKpiRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ItemSpecificKpiDAO> DynamicFilter(IQueryable<ItemSpecificKpiDAO> query, ItemSpecificKpiFilter filter)
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

         private IQueryable<ItemSpecificKpiDAO> OrFilter(IQueryable<ItemSpecificKpiDAO> query, ItemSpecificKpiFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ItemSpecificKpiDAO> initQuery = query.Where(q => false);
            foreach (ItemSpecificKpiFilter ItemSpecificKpiFilter in filter.OrFilter)
            {
                IQueryable<ItemSpecificKpiDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (filter.KpiPeriodId != null)
                    queryable = queryable.Where(q => q.KpiPeriodId, filter.KpiPeriodId);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.EmployeeId != null)
                    queryable = queryable.Where(q => q.EmployeeId, filter.EmployeeId);
                if (filter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, filter.CreatorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ItemSpecificKpiDAO> DynamicOrder(IQueryable<ItemSpecificKpiDAO> query, ItemSpecificKpiFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ItemSpecificKpiOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ItemSpecificKpiOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case ItemSpecificKpiOrder.KpiPeriod:
                            query = query.OrderBy(q => q.KpiPeriodId);
                            break;
                        case ItemSpecificKpiOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ItemSpecificKpiOrder.Employee:
                            query = query.OrderBy(q => q.EmployeeId);
                            break;
                        case ItemSpecificKpiOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ItemSpecificKpiOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ItemSpecificKpiOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case ItemSpecificKpiOrder.KpiPeriod:
                            query = query.OrderByDescending(q => q.KpiPeriodId);
                            break;
                        case ItemSpecificKpiOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ItemSpecificKpiOrder.Employee:
                            query = query.OrderByDescending(q => q.EmployeeId);
                            break;
                        case ItemSpecificKpiOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ItemSpecificKpi>> DynamicSelect(IQueryable<ItemSpecificKpiDAO> query, ItemSpecificKpiFilter filter)
        {
            List<ItemSpecificKpi> ItemSpecificKpis = await query.Select(q => new ItemSpecificKpi()
            {
                Id = filter.Selects.Contains(ItemSpecificKpiSelect.Id) ? q.Id : default(long),
                OrganizationId = filter.Selects.Contains(ItemSpecificKpiSelect.Organization) ? q.OrganizationId : default(long),
                KpiPeriodId = filter.Selects.Contains(ItemSpecificKpiSelect.KpiPeriod) ? q.KpiPeriodId : default(long),
                StatusId = filter.Selects.Contains(ItemSpecificKpiSelect.Status) ? q.StatusId : default(long),
                EmployeeId = filter.Selects.Contains(ItemSpecificKpiSelect.Employee) ? q.EmployeeId : default(long),
                CreatorId = filter.Selects.Contains(ItemSpecificKpiSelect.Creator) ? q.CreatorId : default(long),
                Creator = filter.Selects.Contains(ItemSpecificKpiSelect.Creator) && q.Creator != null ? new AppUser
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
                Employee = filter.Selects.Contains(ItemSpecificKpiSelect.Employee) && q.Employee != null ? new AppUser
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
                KpiPeriod = filter.Selects.Contains(ItemSpecificKpiSelect.KpiPeriod) && q.KpiPeriod != null ? new KpiPeriod
                {
                    Id = q.KpiPeriod.Id,
                    Code = q.KpiPeriod.Code,
                    Name = q.KpiPeriod.Name,
                } : null,
                Organization = filter.Selects.Contains(ItemSpecificKpiSelect.Organization) && q.Organization != null ? new Organization
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
                Status = filter.Selects.Contains(ItemSpecificKpiSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return ItemSpecificKpis;
        }

        public async Task<int> Count(ItemSpecificKpiFilter filter)
        {
            IQueryable<ItemSpecificKpiDAO> ItemSpecificKpis = DataContext.ItemSpecificKpi.AsNoTracking();
            ItemSpecificKpis = DynamicFilter(ItemSpecificKpis, filter);
            return await ItemSpecificKpis.CountAsync();
        }

        public async Task<List<ItemSpecificKpi>> List(ItemSpecificKpiFilter filter)
        {
            if (filter == null) return new List<ItemSpecificKpi>();
            IQueryable<ItemSpecificKpiDAO> ItemSpecificKpiDAOs = DataContext.ItemSpecificKpi.AsNoTracking();
            ItemSpecificKpiDAOs = DynamicFilter(ItemSpecificKpiDAOs, filter);
            ItemSpecificKpiDAOs = DynamicOrder(ItemSpecificKpiDAOs, filter);
            List<ItemSpecificKpi> ItemSpecificKpis = await DynamicSelect(ItemSpecificKpiDAOs, filter);
            return ItemSpecificKpis;
        }

        public async Task<ItemSpecificKpi> Get(long Id)
        {
            ItemSpecificKpi ItemSpecificKpi = await DataContext.ItemSpecificKpi.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new ItemSpecificKpi()
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

            if (ItemSpecificKpi == null)
                return null;
            ItemSpecificKpi.ItemSpecificKpiContents = await DataContext.ItemSpecificKpiContent.AsNoTracking()
                .Where(x => x.ItemSpecificKpiId == ItemSpecificKpi.Id)
                .Select(x => new ItemSpecificKpiContent
                {
                    Id = x.Id,
                    ItemSpecificKpiId = x.ItemSpecificKpiId,
                    ItemId = x.ItemId,
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
            var ItemSpecificKpiContentIds = ItemSpecificKpi.ItemSpecificKpiContents.Select(x => x.Id).ToList();
            List<ItemSpecificKpiContentItemSpecificKpiCriteriaMapping> ItemSpecificKpiContentItemSpecificKpiCriteriaMappings = await DataContext.ItemSpecificKpiContentItemSpecificKpiCriteriaMapping
                .Where(x => ItemSpecificKpiContentIds.Contains(x.ItemSpecificKpiContentId))
                .Select(x => new ItemSpecificKpiContentItemSpecificKpiCriteriaMapping
                {
                    ItemSpecificCriteriaId = x.ItemSpecificCriteriaId,
                    ItemSpecificKpiContentId = x.ItemSpecificKpiContentId,
                    Value = x.Value,
                    ItemSpecificKpiContent = new ItemSpecificKpiContent
                    {
                        Id = x.ItemSpecificKpiContent.Id,
                        ItemId = x.ItemSpecificKpiContent.ItemId,
                        ItemSpecificKpiId = x.ItemSpecificKpiContent.ItemSpecificKpiId,
                        Item = new Item
                        {
                            Id = x.ItemSpecificKpiContent.Item.Id,
                            ProductId = x.ItemSpecificKpiContent.Item.ProductId,
                            Code = x.ItemSpecificKpiContent.Item.Code,
                            Name = x.ItemSpecificKpiContent.Item.Name,
                            ScanCode = x.ItemSpecificKpiContent.Item.ScanCode,
                            SalePrice = x.ItemSpecificKpiContent.Item.SalePrice,
                            RetailPrice = x.ItemSpecificKpiContent.Item.RetailPrice,
                            StatusId = x.ItemSpecificKpiContent.Item.StatusId,
                        },
                    }
                }).ToListAsync();

            ItemSpecificKpi.ItemSpecificKpiTotalItemSpecificCriteriaMappings = await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping.AsNoTracking()
                .Where(x => x.ItemSpecificKpiId == ItemSpecificKpi.Id)
                .Select(x => new ItemSpecificKpiTotalItemSpecificCriteriaMapping
                {
                    ItemSpecificKpiId = x.ItemSpecificKpiId,
                    TotalItemSpecificCriteriaId = x.TotalItemSpecificCriteriaId,
                    Value = x.Value,
                    TotalItemSpecificCriteria = new TotalItemSpecificCriteria
                    {
                        Id = x.TotalItemSpecificCriteria.Id,
                        Code = x.TotalItemSpecificCriteria.Code,
                        Name = x.TotalItemSpecificCriteria.Name,
                    },
                }).ToListAsync();

            return ItemSpecificKpi;
        }
        public async Task<bool> Create(ItemSpecificKpi ItemSpecificKpi)
        {
            ItemSpecificKpiDAO ItemSpecificKpiDAO = new ItemSpecificKpiDAO();
            ItemSpecificKpiDAO.Id = ItemSpecificKpi.Id;
            ItemSpecificKpiDAO.OrganizationId = ItemSpecificKpi.OrganizationId;
            ItemSpecificKpiDAO.KpiPeriodId = ItemSpecificKpi.KpiPeriodId;
            ItemSpecificKpiDAO.StatusId = ItemSpecificKpi.StatusId;
            ItemSpecificKpiDAO.EmployeeId = ItemSpecificKpi.EmployeeId;
            ItemSpecificKpiDAO.CreatorId = ItemSpecificKpi.CreatorId;
            ItemSpecificKpiDAO.CreatedAt = StaticParams.DateTimeNow;
            ItemSpecificKpiDAO.UpdatedAt = StaticParams.DateTimeNow;
            ItemSpecificKpiDAO.RowId = Guid.NewGuid();
            DataContext.ItemSpecificKpi.Add(ItemSpecificKpiDAO);
            await DataContext.SaveChangesAsync();
            ItemSpecificKpi.Id = ItemSpecificKpiDAO.Id;
            ItemSpecificKpi.RowId = ItemSpecificKpiDAO.RowId;
            await SaveReference(ItemSpecificKpi);
            return true;
        }

        public async Task<bool> Update(ItemSpecificKpi ItemSpecificKpi)
        {
            ItemSpecificKpiDAO ItemSpecificKpiDAO = DataContext.ItemSpecificKpi.Where(x => x.Id == ItemSpecificKpi.Id).FirstOrDefault();
            if (ItemSpecificKpiDAO == null)
                return false;
            ItemSpecificKpiDAO.Id = ItemSpecificKpi.Id;
            ItemSpecificKpiDAO.OrganizationId = ItemSpecificKpi.OrganizationId;
            ItemSpecificKpiDAO.KpiPeriodId = ItemSpecificKpi.KpiPeriodId;
            ItemSpecificKpiDAO.StatusId = ItemSpecificKpi.StatusId;
            ItemSpecificKpiDAO.EmployeeId = ItemSpecificKpi.EmployeeId;
            ItemSpecificKpiDAO.CreatorId = ItemSpecificKpi.CreatorId;
            ItemSpecificKpiDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ItemSpecificKpi);
            return true;
        }

        public async Task<bool> Delete(ItemSpecificKpi ItemSpecificKpi)
        {
            await DataContext.ItemSpecificKpiContent.Where(x => x.ItemSpecificKpiId == ItemSpecificKpi.Id).DeleteFromQueryAsync();
            await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping.Where(x => x.ItemSpecificKpiId == ItemSpecificKpi.Id).DeleteFromQueryAsync();
            await DataContext.ItemSpecificKpi.Where(x => x.Id == ItemSpecificKpi.Id).UpdateFromQueryAsync(x => new ItemSpecificKpiDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<ItemSpecificKpi> ItemSpecificKpis)
        {
            List<ItemSpecificKpiDAO> ItemSpecificKpiDAOs = new List<ItemSpecificKpiDAO>();
            foreach (ItemSpecificKpi ItemSpecificKpi in ItemSpecificKpis)
            {
                ItemSpecificKpiDAO ItemSpecificKpiDAO = new ItemSpecificKpiDAO();
                ItemSpecificKpiDAO.Id = ItemSpecificKpi.Id;
                ItemSpecificKpiDAO.OrganizationId = ItemSpecificKpi.OrganizationId;
                ItemSpecificKpiDAO.KpiPeriodId = ItemSpecificKpi.KpiPeriodId;
                ItemSpecificKpiDAO.StatusId = ItemSpecificKpi.StatusId;
                ItemSpecificKpiDAO.EmployeeId = ItemSpecificKpi.EmployeeId;
                ItemSpecificKpiDAO.CreatorId = ItemSpecificKpi.CreatorId;
                ItemSpecificKpiDAO.CreatedAt = StaticParams.DateTimeNow;
                ItemSpecificKpiDAO.UpdatedAt = StaticParams.DateTimeNow;
                ItemSpecificKpiDAO.RowId = Guid.NewGuid();
                ItemSpecificKpiDAOs.Add(ItemSpecificKpiDAO);
                ItemSpecificKpi.RowId = ItemSpecificKpiDAO.RowId;
            }
            await DataContext.BulkMergeAsync(ItemSpecificKpiDAOs);

            var ItemSpecificKpiContentDAOs = new List<ItemSpecificKpiContentDAO>();
            foreach (var ItemSpecificKpi in ItemSpecificKpis)
            {
                ItemSpecificKpi.Id = ItemSpecificKpiDAOs.Where(x => x.RowId == ItemSpecificKpi.RowId).Select(x => x.Id).FirstOrDefault();
                if(ItemSpecificKpi.ItemSpecificKpiContents != null && ItemSpecificKpi.ItemSpecificKpiContents.Any())
                {
                    var listContent = ItemSpecificKpi.ItemSpecificKpiContents.Select(x => new ItemSpecificKpiContentDAO
                    {
                        ItemId = x.ItemId,
                        ItemSpecificKpiId = ItemSpecificKpi.Id,
                    }).ToList();
                    ItemSpecificKpiContentDAOs.AddRange(listContent);
                }
                
            }

            var ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs = new List<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO>();
            foreach (var ItemSpecificKpi in ItemSpecificKpis)
            {
                if(ItemSpecificKpi.ItemSpecificKpiTotalItemSpecificCriteriaMappings != null && ItemSpecificKpi.ItemSpecificKpiTotalItemSpecificCriteriaMappings.Any())
                {
                    var listTotal = ItemSpecificKpi.ItemSpecificKpiTotalItemSpecificCriteriaMappings.Select(x => new ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO
                    {
                        ItemSpecificKpiId = ItemSpecificKpi.Id,
                        TotalItemSpecificCriteriaId = x.TotalItemSpecificCriteriaId,
                        Value = x.Value,
                    }).ToList();
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs.AddRange(listTotal);
                }
                
            }

            await DataContext.ItemSpecificKpiContent.BulkMergeAsync(ItemSpecificKpiContentDAOs);
            await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping.BulkMergeAsync(ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ItemSpecificKpi> ItemSpecificKpis)
        {
            List<long> Ids = ItemSpecificKpis.Select(x => x.Id).ToList();
            await DataContext.ItemSpecificKpiContent.Where(x => Ids.Contains(x.ItemSpecificKpiId)).DeleteFromQueryAsync();
            await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping.Where(x => Ids.Contains(x.ItemSpecificKpiId)).DeleteFromQueryAsync();
            await DataContext.ItemSpecificKpi
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ItemSpecificKpiDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ItemSpecificKpi ItemSpecificKpi)
        {
            await DataContext.ItemSpecificKpiContent
                .Where(x => x.ItemSpecificKpiId == ItemSpecificKpi.Id)
                .DeleteFromQueryAsync();
            List<ItemSpecificKpiContentDAO> ItemSpecificKpiContentDAOs = new List<ItemSpecificKpiContentDAO>();
            if (ItemSpecificKpi.ItemSpecificKpiContents != null)
            {
                foreach (ItemSpecificKpiContent ItemSpecificKpiContent in ItemSpecificKpi.ItemSpecificKpiContents)
                {
                    ItemSpecificKpiContentDAO ItemSpecificKpiContentDAO = new ItemSpecificKpiContentDAO();
                    ItemSpecificKpiContentDAO.Id = ItemSpecificKpiContent.Id;
                    ItemSpecificKpiContentDAO.ItemSpecificKpiId = ItemSpecificKpi.Id;
                    ItemSpecificKpiContentDAO.ItemId = ItemSpecificKpiContent.ItemId;
                    ItemSpecificKpiContentDAOs.Add(ItemSpecificKpiContentDAO);
                }
                await DataContext.ItemSpecificKpiContent.BulkMergeAsync(ItemSpecificKpiContentDAOs);
            }
            await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping
                .Where(x => x.ItemSpecificKpiId == ItemSpecificKpi.Id)
                .DeleteFromQueryAsync();
            List<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO> ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs = new List<ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO>();
            if (ItemSpecificKpi.ItemSpecificKpiTotalItemSpecificCriteriaMappings != null)
            {
                foreach (ItemSpecificKpiTotalItemSpecificCriteriaMapping ItemSpecificKpiTotalItemSpecificCriteriaMapping in ItemSpecificKpi.ItemSpecificKpiTotalItemSpecificCriteriaMappings)
                {
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO = new ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO();
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO.ItemSpecificKpiId = ItemSpecificKpi.Id;
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO.TotalItemSpecificCriteriaId = ItemSpecificKpiTotalItemSpecificCriteriaMapping.TotalItemSpecificCriteriaId;
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO.Value = ItemSpecificKpiTotalItemSpecificCriteriaMapping.Value;
                    ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs.Add(ItemSpecificKpiTotalItemSpecificCriteriaMappingDAO);
                }
                await DataContext.ItemSpecificKpiTotalItemSpecificCriteriaMapping.BulkMergeAsync(ItemSpecificKpiTotalItemSpecificCriteriaMappingDAOs);
            }
        }
        
    }
}
