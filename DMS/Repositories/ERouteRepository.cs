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
    public interface IERouteRepository
    {
        Task<int> Count(ERouteFilter ERouteFilter);
        Task<List<ERoute>> List(ERouteFilter ERouteFilter);
        Task<ERoute> Get(long Id);
        Task<bool> Create(ERoute ERoute);
        Task<bool> Update(ERoute ERoute);
        Task<bool> Delete(ERoute ERoute);
        Task<bool> BulkMerge(List<ERoute> ERoutes);
        Task<bool> BulkDelete(List<ERoute> ERoutes);
    }
    public class ERouteRepository : IERouteRepository
    {
        private DataContext DataContext;
        public ERouteRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ERouteDAO> DynamicFilter(IQueryable<ERouteDAO> query, ERouteFilter filter)
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
                    query = query.Where(q =>  Ids.Contains(q.OrganizationId));
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
            if (filter.AppUserId != null)
                query = query.Where(q => q.SaleEmployeeId, filter.AppUserId);
            if (filter.StartDate != null)
                query = query.Where(q => q.StartDate, filter.StartDate);
            if (filter.EndDate != null)
                query = query.Where(q => q.EndDate == null).Union(query.Where(q => q.EndDate, filter.EndDate));
            if (filter.ERouteTypeId != null)
                query = query.Where(q => q.ERouteTypeId, filter.ERouteTypeId);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            if (filter.StoreId != null)
            {
                if (filter.StoreId.Equal.HasValue)
                {
                    query = from q in query
                            join ec in DataContext.ERouteContent on q.Id equals ec.ERouteId
                            where ec.StoreId == filter.StoreId.Equal
                            select q;
                    query = query.Distinct();
                }
            }
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ERouteDAO> OrFilter(IQueryable<ERouteDAO> query, ERouteFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ERouteDAO> initQuery = query.Where(q => false);
            foreach (ERouteFilter ERouteFilter in filter.OrFilter)
            {
                IQueryable<ERouteDAO> queryable = query;
                if (ERouteFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ERouteFilter.Id);
                if (ERouteFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, ERouteFilter.Code);
                if (ERouteFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ERouteFilter.Name);
                if (ERouteFilter.OrganizationId != null)
                    queryable = queryable.Where(q => q.OrganizationId, ERouteFilter.OrganizationId);
                if (ERouteFilter.AppUserId != null)
                    queryable = queryable.Where(q => q.SaleEmployeeId, ERouteFilter.AppUserId);
                if (ERouteFilter.StartDate != null)
                    queryable = queryable.Where(q => q.StartDate, ERouteFilter.StartDate);
                if (ERouteFilter.EndDate != null)
                    queryable = queryable.Where(q => q.EndDate, ERouteFilter.EndDate);
                if (ERouteFilter.ERouteTypeId != null)
                    queryable = queryable.Where(q => q.ERouteTypeId, ERouteFilter.ERouteTypeId);
                if (ERouteFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, ERouteFilter.StatusId);
                if (ERouteFilter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, ERouteFilter.CreatorId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<ERouteDAO> DynamicOrder(IQueryable<ERouteDAO> query, ERouteFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ERouteOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ERouteOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ERouteOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ERouteOrder.SaleEmployee:
                            query = query.OrderBy(q => q.SaleEmployeeId);
                            break;
                        case ERouteOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case ERouteOrder.StartDate:
                            query = query.OrderBy(q => q.StartDate);
                            break;
                        case ERouteOrder.EndDate:
                            query = query.OrderBy(q => q.EndDate);
                            break;
                        case ERouteOrder.ERouteType:
                            query = query.OrderBy(q => q.ERouteTypeId);
                            break;
                        case ERouteOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ERouteOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ERouteOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ERouteOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ERouteOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ERouteOrder.SaleEmployee:
                            query = query.OrderByDescending(q => q.SaleEmployeeId);
                            break;
                        case ERouteOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case ERouteOrder.StartDate:
                            query = query.OrderByDescending(q => q.StartDate);
                            break;
                        case ERouteOrder.EndDate:
                            query = query.OrderByDescending(q => q.EndDate);
                            break;
                        case ERouteOrder.ERouteType:
                            query = query.OrderByDescending(q => q.ERouteTypeId);
                            break;
                        case ERouteOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ERouteOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<ERoute>> DynamicSelect(IQueryable<ERouteDAO> query, ERouteFilter filter)
        {
            List<ERoute> ERoutes = await query.Select(q => new ERoute()
            {
                Id = filter.Selects.Contains(ERouteSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ERouteSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ERouteSelect.Name) ? q.Name : default(string),
                SaleEmployeeId = filter.Selects.Contains(ERouteSelect.SaleEmployee) ? q.SaleEmployeeId : default(long),
                OrganizationId = filter.Selects.Contains(ERouteSelect.Organization) ? q.OrganizationId : default(long),
                StartDate = filter.Selects.Contains(ERouteSelect.StartDate) ? q.StartDate : default(DateTime),
                EndDate = filter.Selects.Contains(ERouteSelect.EndDate) ? q.EndDate : default(DateTime?),
                ERouteTypeId = filter.Selects.Contains(ERouteSelect.ERouteType) ? q.ERouteTypeId : default(long),
                StatusId = filter.Selects.Contains(ERouteSelect.Status) ? q.StatusId : default(long),
                CreatorId = filter.Selects.Contains(ERouteSelect.Creator) ? q.CreatorId : default(long),
                Creator = filter.Selects.Contains(ERouteSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    Address = q.Creator.Address,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                } : null,
                Organization = filter.Selects.Contains(ERouteSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    Address = q.Organization.Address,
                    Phone = q.Organization.Phone,
                    Path = q.Organization.Path,
                    ParentId = q.Organization.ParentId,
                    Email = q.Organization.Email,
                    StatusId = q.Organization.StatusId,
                    Level = q.Organization.Level
                } : null,
                ERouteType = filter.Selects.Contains(ERouteSelect.ERouteType) && q.ERouteType != null ? new ERouteType
                {
                    Id = q.ERouteType.Id,
                    Code = q.ERouteType.Code,
                    Name = q.ERouteType.Name,
                } : null,
                
                SaleEmployee = filter.Selects.Contains(ERouteSelect.SaleEmployee) && q.SaleEmployee != null ? new AppUser
                {
                    Id = q.SaleEmployee.Id,
                    Username = q.SaleEmployee.Username,
                    DisplayName = q.SaleEmployee.DisplayName,
                    Address = q.SaleEmployee.Address,
                    Email = q.SaleEmployee.Email,
                    Phone = q.SaleEmployee.Phone,
                } : null,
                Status = filter.Selects.Contains(ERouteSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();

            if (filter.Selects.Contains(ERouteSelect.ERouteContent))
            {
                List<long> ERouteIds = ERoutes.Select(x => x.Id).ToList();
                List<ERouteContent> ERouteContents = await DataContext.ERouteContent
                    .Where(x => ERouteIds.Contains(x.ERouteId))
                    .Select(x => new ERouteContent
                    {
                        Id = x.Id,
                        StoreId = x.StoreId,
                    }).ToListAsync();
            }    
            return ERoutes;
        }

        public async Task<int> Count(ERouteFilter filter)
        {
            IQueryable<ERouteDAO> ERoutes = DataContext.ERoute.AsNoTracking();
            ERoutes = DynamicFilter(ERoutes, filter);
            return await ERoutes.CountAsync();
        }

        public async Task<List<ERoute>> List(ERouteFilter filter)
        {
            if (filter == null) return new List<ERoute>();
            IQueryable<ERouteDAO> ERouteDAOs = DataContext.ERoute.AsNoTracking();
            ERouteDAOs = DynamicFilter(ERouteDAOs, filter);
            ERouteDAOs = DynamicOrder(ERouteDAOs, filter);
            List<ERoute> ERoutes = await DynamicSelect(ERouteDAOs, filter);
            return ERoutes;
        }

        public async Task<ERoute> Get(long Id)
        {
            ERoute ERoute = await DataContext.ERoute.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new ERoute()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                SaleEmployeeId = x.SaleEmployeeId,
                OrganizationId = x.OrganizationId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                ERouteTypeId = x.ERouteTypeId,
                StatusId = x.StatusId,
                CreatorId = x.CreatorId,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                },
                ERouteType = x.ERouteType == null ? null : new ERouteType
                {
                    Id = x.ERouteType.Id,
                    Code = x.ERouteType.Code,
                    Name = x.ERouteType.Name,
                },
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    Address = x.Organization.Address,
                    Phone = x.Organization.Phone,
                    Path = x.Organization.Path,
                    ParentId = x.Organization.ParentId,
                    Email = x.Organization.Email,
                    StatusId = x.Organization.StatusId,
                    Level = x.Organization.Level
                },
                SaleEmployee = x.SaleEmployee == null ? null : new AppUser
                {
                    Id = x.SaleEmployee.Id,
                    Username = x.SaleEmployee.Username,
                    DisplayName = x.SaleEmployee.DisplayName,
                    Address = x.SaleEmployee.Address,
                    Email = x.SaleEmployee.Email,
                    Phone = x.SaleEmployee.Phone,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).FirstOrDefaultAsync();

            if (ERoute == null)
                return null;

            ERoute.ERouteContents = await DataContext.ERouteContent.Where(x => x.ERouteId == Id).Select(x => new ERouteContent
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
                Store = new Store
                {
                    Id = x.Store.Id,
                    Code = x.Store.Code,
                    Name = x.Store.Name,
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
            }).ToListAsync();
            return ERoute;
        }
        public async Task<bool> Create(ERoute ERoute)
        {
            ERouteDAO ERouteDAO = new ERouteDAO();
            ERouteDAO.Id = ERoute.Id;
            ERouteDAO.Code = ERoute.Code;
            ERouteDAO.Name = ERoute.Name;
            ERouteDAO.SaleEmployeeId = ERoute.SaleEmployeeId;
            ERouteDAO.OrganizationId = ERoute.OrganizationId;
            ERouteDAO.StartDate = ERoute.StartDate;
            ERouteDAO.RealStartDate = ERoute.RealStartDate;
            ERouteDAO.EndDate = ERoute.EndDate;
            ERouteDAO.ERouteTypeId = ERoute.ERouteTypeId;
            ERouteDAO.StatusId = ERoute.StatusId;
            ERouteDAO.CreatorId = ERoute.CreatorId;
            ERouteDAO.CreatedAt = StaticParams.DateTimeNow;
            ERouteDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.ERoute.Add(ERouteDAO);
            await DataContext.SaveChangesAsync();
            ERoute.Id = ERouteDAO.Id;
            await SaveReference(ERoute);
            return true;
        }

        public async Task<bool> Update(ERoute ERoute)
        {
            ERouteDAO ERouteDAO = DataContext.ERoute.Where(x => x.Id == ERoute.Id).FirstOrDefault();
            if (ERouteDAO == null)
                return false;
            ERouteDAO.Id = ERoute.Id;
            ERouteDAO.Code = ERoute.Code;
            ERouteDAO.Name = ERoute.Name;
            ERouteDAO.SaleEmployeeId = ERoute.SaleEmployeeId;
            ERouteDAO.OrganizationId = ERoute.OrganizationId;
            ERouteDAO.StartDate = ERoute.StartDate;
            ERouteDAO.EndDate = ERoute.EndDate;
            ERouteDAO.RealStartDate = ERoute.RealStartDate;
            ERouteDAO.ERouteTypeId = ERoute.ERouteTypeId;
            ERouteDAO.StatusId = ERoute.StatusId;
            ERouteDAO.CreatorId = ERoute.CreatorId;
            ERouteDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(ERoute);
            return true;
        }

        public async Task<bool> Delete(ERoute ERoute)
        {
            var ERouteContentIds = await DataContext.ERouteContent.Where(x => x.ERouteId == ERoute.Id).Select(x => x.Id).ToListAsync();
            var ERouteChangeRequestIds = await DataContext.ERouteChangeRequest.Where(x => x.ERouteId == ERoute.Id).Select(x => x.Id).ToListAsync();
            await DataContext.ERouteChangeRequestContent.Where(x => ERouteChangeRequestIds.Contains(x.ERouteChangeRequestId)).DeleteFromQueryAsync();
            await DataContext.ERouteChangeRequest.Where(x => x.ERouteId == ERoute.Id).DeleteFromQueryAsync();
            await DataContext.ERouteContentDay.Where(x => ERouteContentIds.Contains(x.ERouteContentId)).DeleteFromQueryAsync();
            await DataContext.ERouteContent.Where(x => x.ERouteId == ERoute.Id).DeleteFromQueryAsync();
            await DataContext.ERoute.Where(x => x.Id == ERoute.Id).UpdateFromQueryAsync(x => new ERouteDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<ERoute> ERoutes)
        {
            List<ERouteDAO> ERouteDAOs = new List<ERouteDAO>();
            foreach (ERoute ERoute in ERoutes)
            {
                ERouteDAO ERouteDAO = new ERouteDAO();
                ERouteDAO.Id = ERoute.Id;
                ERouteDAO.Code = ERoute.Code;
                ERouteDAO.Name = ERoute.Name;
                ERouteDAO.SaleEmployeeId = ERoute.SaleEmployeeId;
                ERouteDAO.OrganizationId = ERoute.OrganizationId;
                ERouteDAO.StartDate = ERoute.StartDate;
                ERouteDAO.EndDate = ERoute.EndDate;
                ERouteDAO.ERouteTypeId = ERoute.ERouteTypeId;
                ERouteDAO.StatusId = ERoute.StatusId;
                ERouteDAO.CreatorId = ERoute.CreatorId;
                ERouteDAO.CreatedAt = StaticParams.DateTimeNow;
                ERouteDAO.UpdatedAt = StaticParams.DateTimeNow;
                ERouteDAOs.Add(ERouteDAO);
            }
            await DataContext.BulkMergeAsync(ERouteDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<ERoute> ERoutes)
        {
            List<long> Ids = ERoutes.Select(x => x.Id).ToList();
            await DataContext.ERouteContentDay.Where(x => Ids.Contains(x.ERouteContent.ERouteId)).DeleteFromQueryAsync();
            await DataContext.ERouteContent.Where(x => Ids.Contains(x.ERouteId)).DeleteFromQueryAsync();
            await DataContext.ERoute
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ERouteDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(ERoute ERoute)
        {
            var ERouteContentIds = await DataContext.ERouteContent
                .Where(x => x.ERouteId == ERoute.Id).Select(x => x.Id).ToListAsync();
            await DataContext.ERouteContentDay
                .Where(x => ERouteContentIds.Contains(x.ERouteContentId)).DeleteFromQueryAsync();
            await DataContext.ERouteContent
                .Where(x => x.ERouteId == ERoute.Id).DeleteFromQueryAsync();
            if (ERoute.ERouteContents != null)
            {
                ERoute.ERouteContents.ForEach(x => x.RowId = Guid.NewGuid());
                List<ERouteContentDAO> ERouteContentDAOs = new List<ERouteContentDAO>();
                foreach (ERouteContent ERouteContent in ERoute.ERouteContents)
                {
                    ERouteContentDAO ERouteContentDAO = new ERouteContentDAO();
                    ERouteContentDAO.Id = ERouteContent.Id;
                    ERouteContentDAO.ERouteId = ERoute.Id;
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
                    ERouteContentDAO.RowId = ERouteContent.RowId;
                    ERouteContentDAOs.Add(ERouteContentDAO);
                }
                await DataContext.ERouteContent.BulkMergeAsync(ERouteContentDAOs);

                List<ERouteContentDayDAO> ERouteContentDayDAOs = new List<ERouteContentDayDAO>();
                foreach (ERouteContent ERouteContent in ERoute.ERouteContents)
                {
                    ERouteContent.Id = ERouteContentDAOs.Where(x => x.RowId == ERouteContent.RowId).Select(x => x.Id).FirstOrDefault();
                    List<ERouteContentDay> ERouteContentDays = await BuildERouteContentDays(ERouteContent);
                    List<ERouteContentDayDAO> eRouteContentDayDAOs = ERouteContentDays.Select(x => new ERouteContentDayDAO
                    {
                        ERouteContentId = ERouteContent.Id,
                        Planned = x.Planned,
                        OrderDay = x.OrderDay
                    }).ToList();
                    ERouteContentDayDAOs.AddRange(eRouteContentDayDAOs);
                }

                await DataContext.ERouteContentDay.BulkMergeAsync(ERouteContentDayDAOs);
            }
        }

        private async Task<List<ERouteContentDay>> BuildERouteContentDays(ERouteContent ERouteContent)
        {
            List<ERouteContentDay> ERouteContentDays = new List<ERouteContentDay>();
            for (int i = 0; i < 28; i++)
            {
                ERouteContentDays.Add(new ERouteContentDay
                {
                    ERouteContentId = ERouteContent.Id,
                    OrderDay = i,
                    Planned = false
                });
            }
            
            if (ERouteContent.Week1)
            {
                if (ERouteContent.Monday)
                {
                    ERouteContentDays[0].Planned = true;
                }
                if (ERouteContent.Tuesday)
                {
                    ERouteContentDays[1].Planned = true;
                }
                if (ERouteContent.Wednesday)
                {
                    ERouteContentDays[2].Planned = true;
                }
                if (ERouteContent.Thursday)
                {
                    ERouteContentDays[3].Planned = true;
                }
                if (ERouteContent.Friday)
                {
                    ERouteContentDays[4].Planned = true;
                }
                if (ERouteContent.Saturday)
                {
                    ERouteContentDays[5].Planned = true;
                }
                if (ERouteContent.Sunday)
                {
                    ERouteContentDays[6].Planned = true;
                }
            }
            if (ERouteContent.Week2)
            {
                if (ERouteContent.Monday)
                {
                    ERouteContentDays[7].Planned = true;
                }
                if (ERouteContent.Tuesday)
                {
                    ERouteContentDays[8].Planned = true;
                }
                if (ERouteContent.Wednesday)
                {
                    ERouteContentDays[9].Planned = true;
                }
                if (ERouteContent.Thursday)
                {
                    ERouteContentDays[10].Planned = true;
                }
                if (ERouteContent.Friday)
                {
                    ERouteContentDays[11].Planned = true;
                }
                if (ERouteContent.Saturday)
                {
                    ERouteContentDays[12].Planned = true;
                }
                if (ERouteContent.Sunday)
                {
                    ERouteContentDays[13].Planned = true;
                }
            }
            if (ERouteContent.Week3)
            {
                if (ERouteContent.Monday)
                {
                    ERouteContentDays[14].Planned = true;
                }
                if (ERouteContent.Tuesday)
                {
                    ERouteContentDays[15].Planned = true;
                }
                if (ERouteContent.Wednesday)
                {
                    ERouteContentDays[16].Planned = true;
                }
                if (ERouteContent.Thursday)
                {
                    ERouteContentDays[17].Planned = true;
                }
                if (ERouteContent.Friday)
                {
                    ERouteContentDays[18].Planned = true;
                }
                if (ERouteContent.Saturday)
                {
                    ERouteContentDays[19].Planned = true;
                }
                if (ERouteContent.Sunday)
                {
                    ERouteContentDays[20].Planned = true;
                }
            }
            if (ERouteContent.Week4)
            {
                if (ERouteContent.Monday)
                {
                    ERouteContentDays[21].Planned = true;
                }
                if (ERouteContent.Tuesday)
                {
                    ERouteContentDays[22].Planned = true;
                }
                if (ERouteContent.Wednesday)
                {
                    ERouteContentDays[23].Planned = true;
                }
                if (ERouteContent.Thursday)
                {
                    ERouteContentDays[24].Planned = true;
                }
                if (ERouteContent.Friday)
                {
                    ERouteContentDays[25].Planned = true;
                }
                if (ERouteContent.Saturday)
                {
                    ERouteContentDays[26].Planned = true;
                }
                if (ERouteContent.Sunday)
                {
                    ERouteContentDays[27].Planned = true;
                }
            }
            
            return ERouteContentDays;
        }
    }
}
