using DMS.Common;
using DMS.Helpers;
using DMS.Entities;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver.Core.Authentication;

namespace DMS.Repositories
{
    public interface IKpiProductGroupingRepository
    {
        Task<int> Count(KpiProductGroupingFilter KpiProductGroupingFilter);
        Task<List<KpiProductGrouping>> List(KpiProductGroupingFilter KpiProductGroupingFilter);
        Task<List<KpiProductGrouping>> List(List<long> Ids);
        Task<KpiProductGrouping> Get(long Id);
        Task<bool> Create(KpiProductGrouping KpiProductGrouping);
        Task<bool> Update(KpiProductGrouping KpiProductGrouping);
        Task<bool> Delete(KpiProductGrouping KpiProductGrouping);
        Task<bool> BulkMerge(List<KpiProductGrouping> KpiProductGroupings);
        Task<bool> BulkDelete(List<KpiProductGrouping> KpiProductGroupings);
    }
    public class KpiProductGroupingRepository : IKpiProductGroupingRepository
    {
        private DataContext DataContext;
        public KpiProductGroupingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<KpiProductGroupingDAO> DynamicFilter(IQueryable<KpiProductGroupingDAO> query, KpiProductGroupingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.CreatedAt != null && filter.CreatedAt.HasValue)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null && filter.UpdatedAt.HasValue)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Id != null && filter.Id.HasValue)
                query = query.Where(q => q.Id, filter.Id);
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
            if (filter.KpiYearId != null && filter.KpiYearId.HasValue)
                query = query.Where(q => q.KpiYearId, filter.KpiYearId);
            if (filter.KpiPeriodId != null && filter.KpiPeriodId.HasValue)
                query = query.Where(q => q.KpiPeriodId, filter.KpiPeriodId);
            if (filter.KpiProductGroupingTypeId != null && filter.KpiProductGroupingTypeId.HasValue)
                query = query.Where(q => q.KpiProductGroupingTypeId, filter.KpiProductGroupingTypeId);
            if (filter.StatusId != null && filter.StatusId.HasValue)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.EmployeeId != null && filter.EmployeeId.HasValue)
                query = query.Where(q => q.EmployeeId, filter.EmployeeId);
            if (filter.CreatorId != null && filter.CreatorId.HasValue)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<KpiProductGroupingDAO> OrFilter(IQueryable<KpiProductGroupingDAO> query, KpiProductGroupingFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<KpiProductGroupingDAO> initQuery = query.Where(q => false);
            foreach (KpiProductGroupingFilter KpiProductGroupingFilter in filter.OrFilter)
            {
                IQueryable<KpiProductGroupingDAO> queryable = query;
                if (KpiProductGroupingFilter.Id != null && KpiProductGroupingFilter.Id.HasValue)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (KpiProductGroupingFilter.OrganizationId != null && KpiProductGroupingFilter.OrganizationId.HasValue)
                    queryable = queryable.Where(q => q.OrganizationId, filter.OrganizationId);
                if (KpiProductGroupingFilter.KpiYearId != null && KpiProductGroupingFilter.KpiYearId.HasValue)
                    queryable = queryable.Where(q => q.KpiYearId, filter.KpiYearId);
                if (KpiProductGroupingFilter.KpiPeriodId != null && KpiProductGroupingFilter.KpiPeriodId.HasValue)
                    queryable = queryable.Where(q => q.KpiPeriodId, filter.KpiPeriodId);
                if (KpiProductGroupingFilter.KpiProductGroupingTypeId != null && KpiProductGroupingFilter.KpiProductGroupingTypeId.HasValue)
                    queryable = queryable.Where(q => q.KpiProductGroupingTypeId, filter.KpiProductGroupingTypeId);
                if (KpiProductGroupingFilter.StatusId != null && KpiProductGroupingFilter.StatusId.HasValue)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (KpiProductGroupingFilter.EmployeeId != null && KpiProductGroupingFilter.EmployeeId.HasValue)
                    queryable = queryable.Where(q => q.EmployeeId, filter.EmployeeId);
                if (KpiProductGroupingFilter.CreatorId != null && KpiProductGroupingFilter.CreatorId.HasValue)
                    queryable = queryable.Where(q => q.CreatorId, filter.CreatorId);
                if (KpiProductGroupingFilter.RowId != null && KpiProductGroupingFilter.RowId.HasValue)
                    queryable = queryable.Where(q => q.RowId, filter.RowId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<KpiProductGroupingDAO> DynamicOrder(IQueryable<KpiProductGroupingDAO> query, KpiProductGroupingFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case KpiProductGroupingOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case KpiProductGroupingOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case KpiProductGroupingOrder.KpiYear:
                            query = query.OrderBy(q => q.KpiYearId);
                            break;
                        case KpiProductGroupingOrder.KpiPeriod:
                            query = query.OrderBy(q => q.KpiPeriodId);
                            break;
                        case KpiProductGroupingOrder.KpiProductGroupingType:
                            query = query.OrderBy(q => q.KpiProductGroupingTypeId);
                            break;
                        case KpiProductGroupingOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case KpiProductGroupingOrder.Employee:
                            query = query.OrderBy(q => q.EmployeeId);
                            break;
                        case KpiProductGroupingOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case KpiProductGroupingOrder.Row:
                            query = query.OrderBy(q => q.RowId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case KpiProductGroupingOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case KpiProductGroupingOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case KpiProductGroupingOrder.KpiYear:
                            query = query.OrderByDescending(q => q.KpiYearId);
                            break;
                        case KpiProductGroupingOrder.KpiPeriod:
                            query = query.OrderByDescending(q => q.KpiPeriodId);
                            break;
                        case KpiProductGroupingOrder.KpiProductGroupingType:
                            query = query.OrderByDescending(q => q.KpiProductGroupingTypeId);
                            break;
                        case KpiProductGroupingOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case KpiProductGroupingOrder.Employee:
                            query = query.OrderByDescending(q => q.EmployeeId);
                            break;
                        case KpiProductGroupingOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case KpiProductGroupingOrder.Row:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<KpiProductGrouping>> DynamicSelect(IQueryable<KpiProductGroupingDAO> query, KpiProductGroupingFilter filter)
        {
            List<KpiProductGrouping> KpiProductGroupings = await query.Select(q => new KpiProductGrouping()
            {
                Id = filter.Selects.Contains(KpiProductGroupingSelect.Id) ? q.Id : default(long),
                OrganizationId = filter.Selects.Contains(KpiProductGroupingSelect.Organization) ? q.OrganizationId : default(long),
                KpiYearId = filter.Selects.Contains(KpiProductGroupingSelect.KpiYear) ? q.KpiYearId : default(long),
                KpiPeriodId = filter.Selects.Contains(KpiProductGroupingSelect.KpiPeriod) ? q.KpiPeriodId : default(long),
                KpiProductGroupingTypeId = filter.Selects.Contains(KpiProductGroupingSelect.KpiProductGroupingType) ? q.KpiProductGroupingTypeId : default(long),
                StatusId = filter.Selects.Contains(KpiProductGroupingSelect.Status) ? q.StatusId : default(long),
                EmployeeId = filter.Selects.Contains(KpiProductGroupingSelect.Employee) ? q.EmployeeId : default(long),
                CreatorId = filter.Selects.Contains(KpiProductGroupingSelect.Creator) ? q.CreatorId : default(long),
                RowId = filter.Selects.Contains(KpiProductGroupingSelect.Row) ? q.RowId : default(Guid),
                Creator = filter.Selects.Contains(KpiProductGroupingSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    Address = q.Creator.Address,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                    SexId = q.Creator.SexId,
                    Birthday = q.Creator.Birthday,
                    Avatar = q.Creator.Avatar,
                    PositionId = q.Creator.PositionId,
                    Department = q.Creator.Department,
                    OrganizationId = q.Creator.OrganizationId,
                    ProvinceId = q.Creator.ProvinceId,
                    Longitude = q.Creator.Longitude,
                    Latitude = q.Creator.Latitude,
                    StatusId = q.Creator.StatusId,
                    GPSUpdatedAt = q.Creator.GPSUpdatedAt,
                    RowId = q.Creator.RowId,
                } : null,
                Employee = filter.Selects.Contains(KpiProductGroupingSelect.Employee) && q.Employee != null ? new AppUser
                {
                    Id = q.Employee.Id,
                    Username = q.Employee.Username,
                    DisplayName = q.Employee.DisplayName,
                    Address = q.Employee.Address,
                    Email = q.Employee.Email,
                    Phone = q.Employee.Phone,
                    SexId = q.Employee.SexId,
                    Birthday = q.Employee.Birthday,
                    Avatar = q.Employee.Avatar,
                    PositionId = q.Employee.PositionId,
                    Department = q.Employee.Department,
                    OrganizationId = q.Employee.OrganizationId,
                    ProvinceId = q.Employee.ProvinceId,
                    Longitude = q.Employee.Longitude,
                    Latitude = q.Employee.Latitude,
                    StatusId = q.Employee.StatusId,
                    GPSUpdatedAt = q.Employee.GPSUpdatedAt,
                    RowId = q.Employee.RowId,
                } : null,
                KpiPeriod = filter.Selects.Contains(KpiProductGroupingSelect.KpiPeriod) && q.KpiPeriod != null ? new KpiPeriod
                {
                    Id = q.KpiPeriod.Id,
                    Code = q.KpiPeriod.Code,
                    Name = q.KpiPeriod.Name,
                } : null,
                KpiYear = filter.Selects.Contains(KpiProductGroupingSelect.KpiYear) && q.KpiYear != null ? new KpiYear
                {
                    Id = q.KpiYear.Id,
                    Code = q.KpiYear.Code,
                    Name = q.KpiYear.Name,
                } : null,
                KpiProductGroupingType = filter.Selects.Contains(KpiProductGroupingSelect.KpiProductGroupingType) && q.KpiProductGroupingType != null ? new KpiProductGroupingType
                {
                    Id = q.KpiProductGroupingType.Id,
                    Code = q.KpiProductGroupingType.Code,
                    Name = q.KpiProductGroupingType.Name,
                } : null,
                Organization = filter.Selects.Contains(KpiProductGroupingSelect.Organization) && q.Organization != null ? new Organization
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
                Status = filter.Selects.Contains(KpiProductGroupingSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return KpiProductGroupings;
        }

        public async Task<int> Count(KpiProductGroupingFilter filter)
        {
            IQueryable<KpiProductGroupingDAO> KpiProductGroupings = DataContext.KpiProductGrouping.AsNoTracking();
            KpiProductGroupings = DynamicFilter(KpiProductGroupings, filter);
            return await KpiProductGroupings.CountAsync();
        }

        public async Task<List<KpiProductGrouping>> List(KpiProductGroupingFilter filter)
        {
            if (filter == null) return new List<KpiProductGrouping>();
            IQueryable<KpiProductGroupingDAO> KpiProductGroupingDAOs = DataContext.KpiProductGrouping.AsNoTracking();
            KpiProductGroupingDAOs = DynamicFilter(KpiProductGroupingDAOs, filter);
            KpiProductGroupingDAOs = DynamicOrder(KpiProductGroupingDAOs, filter);
            List<KpiProductGrouping> KpiProductGroupings = await DynamicSelect(KpiProductGroupingDAOs, filter);
            return KpiProductGroupings;
        }

        public async Task<List<KpiProductGrouping>> List(List<long> Ids)
        {
            List<KpiProductGrouping> KpiProductGroupings = await DataContext.KpiProductGrouping.AsNoTracking()
            .Where(x => Ids.Contains(x.Id)).Select(x => new KpiProductGrouping()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Id = x.Id,
                OrganizationId = x.OrganizationId,
                KpiYearId = x.KpiYearId,
                KpiPeriodId = x.KpiPeriodId,
                KpiProductGroupingTypeId = x.KpiProductGroupingTypeId,
                StatusId = x.StatusId,
                EmployeeId = x.EmployeeId,
                CreatorId = x.CreatorId,
                RowId = x.RowId,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                    Avatar = x.Creator.Avatar,
                    PositionId = x.Creator.PositionId,
                    Department = x.Creator.Department,
                    OrganizationId = x.Creator.OrganizationId,
                    ProvinceId = x.Creator.ProvinceId,
                    Longitude = x.Creator.Longitude,
                    Latitude = x.Creator.Latitude,
                    StatusId = x.Creator.StatusId,
                    GPSUpdatedAt = x.Creator.GPSUpdatedAt,
                    RowId = x.Creator.RowId,
                },
                Employee = x.Employee == null ? null : new AppUser
                {
                    Id = x.Employee.Id,
                    Username = x.Employee.Username,
                    DisplayName = x.Employee.DisplayName,
                    Address = x.Employee.Address,
                    Email = x.Employee.Email,
                    Phone = x.Employee.Phone,
                    SexId = x.Employee.SexId,
                    Birthday = x.Employee.Birthday,
                    Avatar = x.Employee.Avatar,
                    PositionId = x.Employee.PositionId,
                    Department = x.Employee.Department,
                    OrganizationId = x.Employee.OrganizationId,
                    ProvinceId = x.Employee.ProvinceId,
                    Longitude = x.Employee.Longitude,
                    Latitude = x.Employee.Latitude,
                    StatusId = x.Employee.StatusId,
                    GPSUpdatedAt = x.Employee.GPSUpdatedAt,
                    RowId = x.Employee.RowId,
                },
                KpiPeriod = x.KpiPeriod == null ? null : new KpiPeriod
                {
                    Id = x.KpiPeriod.Id,
                    Code = x.KpiPeriod.Code,
                    Name = x.KpiPeriod.Name,
                },
                KpiProductGroupingType = x.KpiProductGroupingType == null ? null : new KpiProductGroupingType
                {
                    Id = x.KpiProductGroupingType.Id,
                    Code = x.KpiProductGroupingType.Code,
                    Name = x.KpiProductGroupingType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).ToListAsync();


            return KpiProductGroupings;
        }

        public async Task<KpiProductGrouping> Get(long Id)
        {
            KpiProductGrouping KpiProductGrouping = await DataContext.KpiProductGrouping.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null)
            .Select(x => new KpiProductGrouping()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                OrganizationId = x.OrganizationId,
                KpiYearId = x.KpiYearId,
                KpiPeriodId = x.KpiPeriodId,
                KpiProductGroupingTypeId = x.KpiProductGroupingTypeId,
                StatusId = x.StatusId,
                EmployeeId = x.EmployeeId,
                CreatorId = x.CreatorId,
                RowId = x.RowId,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                    SexId = x.Creator.SexId,
                    Birthday = x.Creator.Birthday,
                    Avatar = x.Creator.Avatar,
                    PositionId = x.Creator.PositionId,
                    Department = x.Creator.Department,
                    OrganizationId = x.Creator.OrganizationId,
                    ProvinceId = x.Creator.ProvinceId,
                    Longitude = x.Creator.Longitude,
                    Latitude = x.Creator.Latitude,
                    StatusId = x.Creator.StatusId,
                    GPSUpdatedAt = x.Creator.GPSUpdatedAt,
                    RowId = x.Creator.RowId,
                },
                Employee = x.Employee == null ? null : new AppUser
                {
                    Id = x.Employee.Id,
                    Username = x.Employee.Username,
                    DisplayName = x.Employee.DisplayName,
                    Address = x.Employee.Address,
                    Email = x.Employee.Email,
                    Phone = x.Employee.Phone,
                    SexId = x.Employee.SexId,
                    Birthday = x.Employee.Birthday,
                    Avatar = x.Employee.Avatar,
                    PositionId = x.Employee.PositionId,
                    Department = x.Employee.Department,
                    OrganizationId = x.Employee.OrganizationId,
                    ProvinceId = x.Employee.ProvinceId,
                    Longitude = x.Employee.Longitude,
                    Latitude = x.Employee.Latitude,
                    StatusId = x.Employee.StatusId,
                    GPSUpdatedAt = x.Employee.GPSUpdatedAt,
                    RowId = x.Employee.RowId,
                },
                KpiPeriod = x.KpiPeriod == null ? null : new KpiPeriod
                {
                    Id = x.KpiPeriod.Id,
                    Code = x.KpiPeriod.Code,
                    Name = x.KpiPeriod.Name,
                },
                KpiProductGroupingType = x.KpiProductGroupingType == null ? null : new KpiProductGroupingType
                {
                    Id = x.KpiProductGroupingType.Id,
                    Code = x.KpiProductGroupingType.Code,
                    Name = x.KpiProductGroupingType.Name,
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

            if (KpiProductGrouping == null)
                return null;

            KpiProductGrouping.KpiProductGroupingContents = await DataContext.KpiProductGroupingContent.AsNoTracking()
                .Where(x => x.KpiProductGroupingId == KpiProductGrouping.Id)
                .Select(c => new KpiProductGroupingContent
                {
                    Id = c.Id,
                    KpiProductGroupingId = c.ProductGroupingId,
                    ProductGroupingId = c.ProductGroupingId,
                    RowId = c.RowId,
                    ProductGrouping = new ProductGrouping
                    {
                        Id = c.ProductGrouping.Id,
                        Code = c.ProductGrouping.Code,
                        Name = c.ProductGrouping.Name,
                        Description = c.ProductGrouping.Description,
                        ParentId = c.ProductGrouping.ParentId,
                        Path = c.ProductGrouping.Path,
                        Level = c.ProductGrouping.Level,
                    }
                })
                .ToListAsync();

            var ContentIds = KpiProductGrouping.KpiProductGroupingContents
                .Select(x => x.Id)
                .ToList();

            List<KpiProductGroupingContentCriteriaMapping> KpiProductGroupingContentCriteriaMappings = await DataContext.KpiProductGroupingContentCriteriaMapping.AsNoTracking()
                .Where(x => ContentIds.Contains(x.KpiProductGroupingContentId))
                .Select(x => new KpiProductGroupingContentCriteriaMapping
                {
                    KpiProductGroupingContentId = x.KpiProductGroupingContentId,
                    KpiProductGroupingCriteriaId = x.KpiProductGroupingCriteriaId,
                    Value = x.Value,
                    KpiProductGroupingContent = new KpiProductGroupingContent
                    {
                        Id = x.KpiProductGroupingContent.Id,
                        KpiProductGroupingId = x.KpiProductGroupingContent.KpiProductGroupingId,
                        ProductGroupingId = x.KpiProductGroupingContent.ProductGroupingId,
                    }
                }).ToListAsync();

            List<KpiProductGroupingContentItemMapping> KpiProductGroupingContentItemMappings = await DataContext.KpiProductGroupingContentItemMapping.AsNoTracking()
                .Where(x => ContentIds.Contains(x.KpiProductGroupingContentId))
                .Select(x => new KpiProductGroupingContentItemMapping
                {
                    KpiProductGroupingContentId = x.KpiProductGroupingContentId,
                    ItemId = x.KpiProductGroupingContentId,
                    Item = new Item
                    {
                        Id = x.Item.Id,
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ProductId = x.Item.ProductId,
                        StatusId = x.Item.StatusId,
                    }
                }).ToListAsync();
            foreach (KpiProductGroupingContent KpiProductGroupingContent in KpiProductGrouping.KpiProductGroupingContents)
            {
                KpiProductGroupingContent.KpiProductGroupingContentCriteriaMappings = KpiProductGroupingContentCriteriaMappings.Where(x => x.KpiProductGroupingContentId == KpiProductGroupingContent.Id).ToList();
                KpiProductGroupingContent.KpiProductGroupingContentItemMappings = KpiProductGroupingContentItemMappings.Where(x => x.KpiProductGroupingContentId == KpiProductGroupingContent.Id).ToList();
            }
            return KpiProductGrouping;
        }
        public async Task<bool> Create(KpiProductGrouping KpiProductGrouping)
        {
            KpiProductGroupingDAO KpiProductGroupingDAO = new KpiProductGroupingDAO();
            KpiProductGroupingDAO.Id = KpiProductGrouping.Id;
            KpiProductGroupingDAO.OrganizationId = KpiProductGrouping.OrganizationId;
            KpiProductGroupingDAO.KpiYearId = KpiProductGrouping.KpiYearId;
            KpiProductGroupingDAO.KpiPeriodId = KpiProductGrouping.KpiPeriodId;
            KpiProductGroupingDAO.KpiProductGroupingTypeId = KpiProductGrouping.KpiProductGroupingTypeId;
            KpiProductGroupingDAO.StatusId = KpiProductGrouping.StatusId;
            KpiProductGroupingDAO.EmployeeId = KpiProductGrouping.EmployeeId;
            KpiProductGroupingDAO.CreatorId = KpiProductGrouping.CreatorId;
            KpiProductGroupingDAO.RowId = Guid.NewGuid();
            KpiProductGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
            KpiProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.KpiProductGrouping.Add(KpiProductGroupingDAO);
            await DataContext.SaveChangesAsync();
            KpiProductGrouping.Id = KpiProductGroupingDAO.Id;
            KpiProductGrouping.RowId = KpiProductGroupingDAO.RowId;
            await SaveReference(KpiProductGrouping);
            return true;
        }

        public async Task<bool> Update(KpiProductGrouping KpiProductGrouping)
        {
            KpiProductGroupingDAO KpiProductGroupingDAO = DataContext.KpiProductGrouping.Where(x => x.Id == KpiProductGrouping.Id).FirstOrDefault();
            if (KpiProductGroupingDAO == null)
                return false;
            KpiProductGroupingDAO.Id = KpiProductGrouping.Id;
            KpiProductGroupingDAO.OrganizationId = KpiProductGrouping.OrganizationId;
            KpiProductGroupingDAO.KpiYearId = KpiProductGrouping.KpiYearId;
            KpiProductGroupingDAO.KpiPeriodId = KpiProductGrouping.KpiPeriodId;
            KpiProductGroupingDAO.KpiProductGroupingTypeId = KpiProductGrouping.KpiProductGroupingTypeId;
            KpiProductGroupingDAO.StatusId = KpiProductGrouping.StatusId;
            KpiProductGroupingDAO.EmployeeId = KpiProductGrouping.EmployeeId;
            KpiProductGroupingDAO.CreatorId = KpiProductGrouping.CreatorId;
            KpiProductGroupingDAO.RowId = KpiProductGrouping.RowId;
            KpiProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(KpiProductGrouping);
            return true;
        }

        public async Task<bool> Delete(KpiProductGrouping KpiProductGrouping)
        {
            await DeleteReference(new List<KpiProductGrouping>
            {
                KpiProductGrouping
            });
            await DataContext.KpiProductGrouping
                .Where(x => x.Id == KpiProductGrouping.Id).UpdateFromQueryAsync(x => new KpiProductGroupingDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });

            return true;
        }

        public async Task<bool> BulkMerge(List<KpiProductGrouping> KpiProductGroupings)
        {
            try
            {
                List<KpiProductGroupingDAO> KpiProductGroupingDAOs = new List<KpiProductGroupingDAO>();
                foreach (KpiProductGrouping KpiProductGrouping in KpiProductGroupings)
                {
                    KpiProductGroupingDAO KpiProductGroupingDAO = new KpiProductGroupingDAO();
                    KpiProductGroupingDAO.Id = KpiProductGrouping.Id;
                    KpiProductGroupingDAO.OrganizationId = KpiProductGrouping.OrganizationId;
                    KpiProductGroupingDAO.KpiYearId = KpiProductGrouping.KpiYearId;
                    KpiProductGroupingDAO.KpiPeriodId = KpiProductGrouping.KpiPeriodId;
                    KpiProductGroupingDAO.KpiProductGroupingTypeId = KpiProductGrouping.KpiProductGroupingTypeId;
                    KpiProductGroupingDAO.StatusId = KpiProductGrouping.StatusId;
                    KpiProductGroupingDAO.EmployeeId = KpiProductGrouping.EmployeeId;
                    KpiProductGroupingDAO.CreatorId = KpiProductGrouping.CreatorId;
                    KpiProductGroupingDAO.RowId = KpiProductGrouping.RowId;
                    KpiProductGroupingDAO.CreatedAt = StaticParams.DateTimeNow;
                    KpiProductGroupingDAO.UpdatedAt = StaticParams.DateTimeNow;
                    KpiProductGroupingDAOs.Add(KpiProductGroupingDAO);
                }
                await DataContext.BulkMergeAsync(KpiProductGroupingDAOs);
                await DeleteReference(KpiProductGroupings); // xóa references

                List<KpiProductGroupingContentDAO> KpiProductGroupingContentDAOs = new List<KpiProductGroupingContentDAO>();
                List<KpiProductGroupingContentCriteriaMappingDAO> KpiProductGroupingContentCriteriaMappingDAOs = new List<KpiProductGroupingContentCriteriaMappingDAO>();
                List<KpiProductGroupingContentItemMappingDAO> KpiProductGroupingContentItemMappingDAOs = new List<KpiProductGroupingContentItemMappingDAO>();
                foreach (var KpiProductGrouping in KpiProductGroupings)
                {
                    KpiProductGrouping.Id = KpiProductGroupingDAOs.Where(x => x.RowId == KpiProductGrouping.RowId)
                        .Select(x => x.Id)
                        .FirstOrDefault();
                    if(KpiProductGrouping.KpiProductGroupingContents != null && KpiProductGrouping.KpiProductGroupingContents.Any())
                    {
                        var ListContent = KpiProductGrouping.KpiProductGroupingContents.Select(x => new KpiProductGroupingContentDAO
                        {
                            KpiProductGroupingId = KpiProductGrouping.Id,
                            ProductGroupingId = x.ProductGroupingId,
                            KpiProductGroupingContentCriteriaMappings = x.KpiProductGroupingContentCriteriaMappings.Select(a => new KpiProductGroupingContentCriteriaMappingDAO {
                                KpiProductGroupingCriteriaId = a.KpiProductGroupingCriteriaId,
                                KpiProductGroupingContentId = a.KpiProductGroupingContentId,
                                Value = a.Value,
                            }).ToList(),
                            KpiProductGroupingContentItemMappings = x.KpiProductGroupingContentItemMappings.Select(i => new KpiProductGroupingContentItemMappingDAO {
                                KpiProductGroupingContentId = i.KpiProductGroupingContentId,
                                ItemId = i.ItemId,
                            }).ToList(),
                            RowId = Guid.NewGuid(),
                        });
                        KpiProductGroupingContentDAOs.AddRange(ListContent);
                    }
                }
                await DataContext.KpiProductGroupingContent.BulkMergeAsync(KpiProductGroupingContentDAOs);

                foreach (KpiProductGroupingContentDAO KpiProductGroupingContentDAO in KpiProductGroupingContentDAOs)
                {
                    KpiProductGroupingContentDAO.Id = KpiProductGroupingContentDAOs
                        .Where(x => x.RowId == KpiProductGroupingContentDAO.RowId).Select(x => x.Id)
                        .FirstOrDefault();
                    if(KpiProductGroupingContentDAO.KpiProductGroupingContentCriteriaMappings != null && KpiProductGroupingContentDAO.KpiProductGroupingContentCriteriaMappings.Any())
                    {
                        var ListCriteriaMappings = KpiProductGroupingContentDAO.KpiProductGroupingContentCriteriaMappings
                            .Select(x => new KpiProductGroupingContentCriteriaMappingDAO { 
                                KpiProductGroupingContentId = KpiProductGroupingContentDAO.Id,
                                KpiProductGroupingCriteriaId = x.KpiProductGroupingCriteriaId,
                                Value = x.Value,
                            })
                            .ToList();
                        KpiProductGroupingContentCriteriaMappingDAOs.AddRange(ListCriteriaMappings);
                    }
                    if(KpiProductGroupingContentDAO.KpiProductGroupingContentItemMappings != null && KpiProductGroupingContentDAO.KpiProductGroupingContentItemMappings.Any())
                    {
                        var ListItemMappings = KpiProductGroupingContentDAO.KpiProductGroupingContentItemMappings
                            .Select(x => new KpiProductGroupingContentItemMappingDAO
                            {
                                KpiProductGroupingContentId = KpiProductGroupingContentDAO.Id,
                                ItemId = x.ItemId,
                            }).ToList();
                        KpiProductGroupingContentItemMappingDAOs.AddRange(ListItemMappings);
                    }
                }
                await DataContext.KpiProductGroupingContentCriteriaMapping.BulkMergeAsync(KpiProductGroupingContentCriteriaMappingDAOs);
                await DataContext.KpiProductGroupingContentItemMapping.BulkMergeAsync(KpiProductGroupingContentItemMappingDAOs);

                return true;
            }
            catch (Exception Exception)
            {

                throw Exception;
            }
        }

        public async Task<bool> BulkDelete(List<KpiProductGrouping> KpiProductGroupings)
        {
            List<long> Ids = KpiProductGroupings.Select(x => x.Id).ToList();
            await DeleteReference(KpiProductGroupings);
            await DataContext.KpiProductGrouping
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new KpiProductGroupingDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(KpiProductGrouping KpiProductGrouping)
        {
            try
            {
                await DeleteReference(new List<KpiProductGrouping> {
                        KpiProductGrouping
                    });

                #region thêm references mới
                List<KpiProductGroupingContentDAO> KpiProductGroupingContentDAOs = new List<KpiProductGroupingContentDAO>();
                List<KpiProductGroupingContentCriteriaMappingDAO> KpiProductGroupingContentCriteriaMappingDAOs = new List<KpiProductGroupingContentCriteriaMappingDAO>();
                List<KpiProductGroupingContentItemMappingDAO> KpiProductGroupingContentItemMappingDAOs = new List<KpiProductGroupingContentItemMappingDAO>();
                if (KpiProductGrouping.KpiProductGroupingContents != null && KpiProductGrouping.KpiProductGroupingContents.Any())
                {
                    KpiProductGrouping.KpiProductGroupingContents.ForEach(x => x.RowId = Guid.NewGuid());
                    foreach (var KpiProductGroupingContent in KpiProductGrouping.KpiProductGroupingContents)
                    {
                        KpiProductGroupingContentDAO KpiProductGroupingContentDAO = new KpiProductGroupingContentDAO();
                        KpiProductGroupingContentDAO.Id = KpiProductGroupingContent.Id;
                        KpiProductGroupingContentDAO.KpiProductGroupingId = KpiProductGroupingContent.KpiProductGroupingId;
                        KpiProductGroupingContentDAO.ProductGroupingId = KpiProductGroupingContent.ProductGroupingId;
                        KpiProductGroupingContentDAO.RowId = KpiProductGroupingContent.RowId;
                        KpiProductGroupingContentDAOs.Add(KpiProductGroupingContentDAO);
                    }
                    await DataContext.KpiProductGroupingContent.BulkMergeAsync(KpiProductGroupingContentDAOs);

                    foreach (var KpiProductGroupingContent in KpiProductGrouping.KpiProductGroupingContents)
                    {
                        KpiProductGroupingContent.Id = KpiProductGroupingContentDAOs
                            .Where(x => x.RowId == KpiProductGroupingContent.RowId)
                            .Select(x => x.Id).FirstOrDefault(); // khi tạo mới thì các content chưa có id mặc định nên phải where theo RowId để tìm ra Id
                        if (KpiProductGroupingContent.KpiProductGroupingContentCriteriaMappings != null)
                        {
                            foreach (var KpiProductGroupingContentCriteriaMapping in KpiProductGroupingContent.KpiProductGroupingContentCriteriaMappings)
                            {
                                KpiProductGroupingContentCriteriaMappingDAO KpiProductGroupingContentCriteriaMappingDAO = new KpiProductGroupingContentCriteriaMappingDAO();
                                KpiProductGroupingContentCriteriaMappingDAO.KpiProductGroupingContentId = KpiProductGroupingContentCriteriaMapping.KpiProductGroupingContentId;
                                KpiProductGroupingContentCriteriaMappingDAO.KpiProductGroupingCriteriaId = KpiProductGroupingContentCriteriaMapping.KpiProductGroupingCriteriaId;
                                KpiProductGroupingContentCriteriaMappingDAO.Value = KpiProductGroupingContentCriteriaMapping.Value;
                                KpiProductGroupingContentCriteriaMappingDAOs.Add(KpiProductGroupingContentCriteriaMappingDAO);
                            }
                            await DataContext.KpiProductGroupingContentCriteriaMapping.BulkMergeAsync(KpiProductGroupingContentCriteriaMappingDAOs); // thêm mapping content và chỉ tiêu kpi

                            foreach (var KpiProductGroupingContentItemMapping in KpiProductGroupingContent.KpiProductGroupingContentItemMappings)
                            {
                                KpiProductGroupingContentItemMappingDAO KpiProductGroupingContentItemMappingDAO = new KpiProductGroupingContentItemMappingDAO();
                                KpiProductGroupingContentItemMappingDAO.KpiProductGroupingContentId = KpiProductGroupingContentItemMapping.KpiProductGroupingContentId;
                                KpiProductGroupingContentItemMappingDAO.ItemId = KpiProductGroupingContentItemMapping.ItemId;
                                KpiProductGroupingContentItemMappingDAOs.Add(KpiProductGroupingContentItemMappingDAO);
                            }
                            await DataContext.KpiProductGroupingContentItemMapping.BulkMergeAsync(KpiProductGroupingContentItemMappingDAOs); // thêm mapping content và Item
                        }
                    }
                }
                #endregion
            }
            catch (Exception Exception)
            {

                throw Exception;
            }
        }

        private async Task DeleteReference(List<KpiProductGrouping> KpiProductGroupings)
        {
            try
            {
                List<long> Ids = KpiProductGroupings.Select(x => x.Id).ToList();
                await DataContext.KpiProductGroupingContentCriteriaMapping
                      .Where(x => Ids.Contains(x.KpiProductGroupingContent.KpiProductGroupingId))
                      .DeleteFromQueryAsync();
                await DataContext.KpiProductGroupingContentItemMapping
                     .Where(x => Ids.Contains(x.KpiProductGroupingContent.KpiProductGroupingId))
                     .DeleteFromQueryAsync();
                await DataContext.KpiProductGroupingContent
                     .Where(x => Ids.Contains(x.KpiProductGroupingId))
                     .DeleteFromQueryAsync();
            }
            catch (Exception Exception)
            {

                throw Exception;
            }
        }

    }
}
