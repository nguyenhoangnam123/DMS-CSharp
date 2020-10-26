using DMS.Common;
using DMS.Entities;
using DMS.Models;
using DMS.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IPriceListRepository
    {
        Task<int> Count(PriceListFilter PriceListFilter);
        Task<List<PriceList>> List(PriceListFilter PriceListFilter);

        Task<int> CountNew(PriceListFilter PriceListFilter);
        Task<List<PriceList>> ListNew(PriceListFilter PriceListFilter);

        Task<int> CountPending(PriceListFilter PriceListFilter);
        Task<List<PriceList>> ListPending(PriceListFilter PriceListFilter);

        Task<int> CountCompleted(PriceListFilter PriceListFilter);
        Task<List<PriceList>> ListCompleted(PriceListFilter PriceListFilter);

        Task<PriceList> Get(long Id);
        Task<bool> Create(PriceList PriceList);
        Task<bool> Update(PriceList PriceList);
        Task<bool> UpdateState(PriceList PriceList);
        Task<bool> Delete(PriceList PriceList);
        Task<bool> BulkMerge(List<PriceList> PriceLists);
        Task<bool> BulkDelete(List<PriceList> PriceLists);
    }
    public class PriceListRepository : IPriceListRepository
    {
        private DataContext DataContext;
        public PriceListRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PriceListDAO> DynamicFilter(IQueryable<PriceListDAO> query, PriceListFilter filter)
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
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            if (filter.StartDate != null)
                query = query.Where(q => q.StartDate, filter.StartDate);
            if (filter.EndDate != null)
                query = query.Where(q => q.EndDate, filter.EndDate);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
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
            if (filter.PriceListTypeId != null)
                query = query.Where(q => q.PriceListTypeId, filter.PriceListTypeId);
            if (filter.SalesOrderTypeId != null)
                query = query.Where(q => q.SalesOrderTypeId, filter.SalesOrderTypeId);
            if (filter.RequestStateId != null)
                query = query.Where(q => q.RequestStateId, filter.RequestStateId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PriceListDAO> OrFilter(IQueryable<PriceListDAO> query, PriceListFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PriceListDAO> initQuery = query.Where(q => false);
            foreach (PriceListFilter PriceListFilter in filter.OrFilter)
            {
                IQueryable<PriceListDAO> queryable = query;
                if (PriceListFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PriceListFilter.Id);
                if (PriceListFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, PriceListFilter.Code);
                if (PriceListFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, PriceListFilter.Name);
                if (PriceListFilter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, PriceListFilter.CreatorId);
                if (PriceListFilter.StartDate != null)
                    queryable = queryable.Where(q => q.StartDate, PriceListFilter.StartDate);
                if (PriceListFilter.EndDate != null)
                    queryable = queryable.Where(q => q.EndDate, PriceListFilter.EndDate);
                if (PriceListFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, PriceListFilter.StatusId);
                if (PriceListFilter.OrganizationId != null)
                {
                    if (PriceListFilter.OrganizationId.Equal != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == PriceListFilter.OrganizationId.Equal.Value).FirstOrDefault();
                        queryable = queryable.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (PriceListFilter.OrganizationId.NotEqual != null)
                    {
                        OrganizationDAO OrganizationDAO = DataContext.Organization
                            .Where(o => o.Id == PriceListFilter.OrganizationId.NotEqual.Value).FirstOrDefault();
                        queryable = queryable.Where(q => !q.Organization.Path.StartsWith(OrganizationDAO.Path));
                    }
                    if (PriceListFilter.OrganizationId.In != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => PriceListFilter.OrganizationId.In.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => Ids.Contains(q.OrganizationId));
                    }
                    if (PriceListFilter.OrganizationId.NotIn != null)
                    {
                        List<OrganizationDAO> OrganizationDAOs = DataContext.Organization
                            .Where(o => o.DeletedAt == null && o.StatusId == 1).ToList();
                        List<OrganizationDAO> Parents = OrganizationDAOs.Where(o => PriceListFilter.OrganizationId.NotIn.Contains(o.Id)).ToList();
                        List<OrganizationDAO> Branches = OrganizationDAOs.Where(o => Parents.Any(p => o.Path.StartsWith(p.Path))).ToList();
                        List<long> Ids = Branches.Select(o => o.Id).ToList();
                        queryable = queryable.Where(q => !Ids.Contains(q.OrganizationId));
                    }
                }
                if (PriceListFilter.PriceListTypeId != null)
                    queryable = queryable.Where(q => q.PriceListTypeId, PriceListFilter.PriceListTypeId);
                if (PriceListFilter.SalesOrderTypeId != null)
                    queryable = queryable.Where(q => q.SalesOrderTypeId, PriceListFilter.SalesOrderTypeId);
                if (PriceListFilter.RequestStateId != null)
                    queryable = queryable.Where(q => q.RequestStateId, PriceListFilter.RequestStateId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<PriceListDAO> DynamicOrder(IQueryable<PriceListDAO> query, PriceListFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PriceListOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PriceListOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case PriceListOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PriceListOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case PriceListOrder.StartDate:
                            query = query.OrderBy(q => q.StartDate);
                            break;
                        case PriceListOrder.EndDate:
                            query = query.OrderBy(q => q.EndDate);
                            break;
                        case PriceListOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case PriceListOrder.Organization:
                            query = query.OrderBy(q => q.OrganizationId);
                            break;
                        case PriceListOrder.PriceListType:
                            query = query.OrderBy(q => q.PriceListTypeId);
                            break;
                        case PriceListOrder.SalesOrderType:
                            query = query.OrderBy(q => q.SalesOrderTypeId);
                            break;
                        case PriceListOrder.RequestState:
                            query = query.OrderBy(q => q.RequestStateId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PriceListOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PriceListOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case PriceListOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case PriceListOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PriceListOrder.StartDate:
                            query = query.OrderByDescending(q => q.StartDate);
                            break;
                        case PriceListOrder.EndDate:
                            query = query.OrderByDescending(q => q.EndDate);
                            break;
                        case PriceListOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case PriceListOrder.Organization:
                            query = query.OrderByDescending(q => q.OrganizationId);
                            break;
                        case PriceListOrder.PriceListType:
                            query = query.OrderByDescending(q => q.PriceListTypeId);
                            break;
                        case PriceListOrder.SalesOrderType:
                            query = query.OrderByDescending(q => q.SalesOrderTypeId);
                            break;
                        case PriceListOrder.RequestState:
                            query = query.OrderByDescending(q => q.RequestStateId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PriceList>> DynamicSelect(IQueryable<PriceListDAO> query, PriceListFilter filter)
        {
            List<PriceList> PriceLists = await query.Select(q => new PriceList()
            {
                Id = filter.Selects.Contains(PriceListSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(PriceListSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(PriceListSelect.Name) ? q.Name : default(string),
                CreatorId = filter.Selects.Contains(PriceListSelect.Creator) ? q.CreatorId : default(long),
                StartDate = filter.Selects.Contains(PriceListSelect.StartDate) ? q.StartDate : default(DateTime?),
                EndDate = filter.Selects.Contains(PriceListSelect.EndDate) ? q.EndDate : default(DateTime?),
                StatusId = filter.Selects.Contains(PriceListSelect.Status) ? q.StatusId : default(long),
                OrganizationId = filter.Selects.Contains(PriceListSelect.Organization) ? q.OrganizationId : default(long),
                PriceListTypeId = filter.Selects.Contains(PriceListSelect.PriceListType) ? q.PriceListTypeId : default(long),
                SalesOrderTypeId = filter.Selects.Contains(PriceListSelect.SalesOrderType) ? q.SalesOrderTypeId : default(long),
                RequestStateId = filter.Selects.Contains(PriceListSelect.RequestState) ? q.RequestStateId : default(long),
                Creator = filter.Selects.Contains(PriceListSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                } : null,
                PriceListType = filter.Selects.Contains(PriceListSelect.PriceListType) && q.PriceListType != null ? new PriceListType
                {
                    Id = q.PriceListType.Id,
                    Code = q.PriceListType.Code,
                    Name = q.PriceListType.Name,
                } : null,
                SalesOrderType = filter.Selects.Contains(PriceListSelect.SalesOrderType) && q.SalesOrderType != null ? new SalesOrderType
                {
                    Id = q.SalesOrderType.Id,
                    Code = q.SalesOrderType.Code,
                    Name = q.SalesOrderType.Name,
                } : null,
                Organization = filter.Selects.Contains(PriceListSelect.Organization) && q.Organization != null ? new Organization
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
                Status = filter.Selects.Contains(PriceListSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                RequestState = filter.Selects.Contains(PriceListSelect.RequestState) && q.RequestState != null ? new RequestState
                {
                    Id = q.RequestState.Id,
                    Code = q.RequestState.Code,
                    Name = q.RequestState.Name,
                } : null,
                RowId = q.RowId,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return PriceLists;
        }

        public async Task<int> Count(PriceListFilter filter)
        {
            IQueryable<PriceListDAO> PriceLists = DataContext.PriceList.AsNoTracking();
            PriceLists = DynamicFilter(PriceLists, filter);
            return await PriceLists.CountAsync();
        }

        public async Task<List<PriceList>> List(PriceListFilter filter)
        {
            if (filter == null) return new List<PriceList>();
            IQueryable<PriceListDAO> PriceListDAOs = DataContext.PriceList.AsNoTracking();
            PriceListDAOs = DynamicFilter(PriceListDAOs, filter);
            PriceListDAOs = DynamicOrder(PriceListDAOs, filter);
            List<PriceList> PriceLists = await DynamicSelect(PriceListDAOs, filter);
            return PriceLists;
        }

        public async Task<int> CountNew(PriceListFilter filter)
        {
            IQueryable<PriceListDAO> PriceListDAOs = DataContext.PriceList.AsNoTracking();
            PriceListDAOs = DynamicFilter(PriceListDAOs, filter);
            PriceListDAOs = from q in PriceListDAOs
                                     where q.RequestStateId == RequestStateEnum.NEW.Id &&
                                     q.CreatorId == filter.ApproverId.Equal
                                     select q;

            return await PriceListDAOs.Distinct().CountAsync();
        }

        public async Task<List<PriceList>> ListNew(PriceListFilter filter)
        {
            if (filter == null) return new List<PriceList>();
            IQueryable<PriceListDAO> PriceListDAOs = DataContext.PriceList.AsNoTracking();
            PriceListDAOs = DynamicFilter(PriceListDAOs, filter);
            PriceListDAOs = from q in PriceListDAOs
                                     where q.RequestStateId == RequestStateEnum.NEW.Id &&
                                     q.CreatorId == filter.ApproverId.Equal
                                     select q;

            PriceListDAOs = DynamicOrder(PriceListDAOs, filter);
            List<PriceList> PriceLists = await DynamicSelect(PriceListDAOs, filter);
            return PriceLists;
        }

        public async Task<int> CountPending(PriceListFilter filter)
        {
            IQueryable<PriceListDAO> PriceListDAOs = DataContext.PriceList.AsNoTracking();
            PriceListDAOs = DynamicFilter(PriceListDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                PriceListDAOs = from q in PriceListDAOs
                                         join r in DataContext.RequestWorkflowDefinitionMapping.Where(x => x.RequestStateId == RequestStateEnum.PENDING.Id) on q.RowId equals r.RequestId
                                         join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                                         join rstep in DataContext.RequestWorkflowStepMapping.Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id) on step.Id equals rstep.WorkflowStepId
                                         join ra in DataContext.AppUserRoleMapping on step.RoleId equals ra.RoleId
                                         where ra.AppUserId == filter.ApproverId.Equal && q.RowId == rstep.RequestId
                                         select q;
            }
            return await PriceListDAOs.Distinct().CountAsync();
        }

        public async Task<List<PriceList>> ListPending(PriceListFilter filter)
        {
            if (filter == null) return new List<PriceList>();
            IQueryable<PriceListDAO> PriceListDAOs = DataContext.PriceList.AsNoTracking();
            PriceListDAOs = DynamicFilter(PriceListDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                PriceListDAOs = from q in PriceListDAOs
                                         join r in DataContext.RequestWorkflowDefinitionMapping.Where(x => x.RequestStateId == RequestStateEnum.PENDING.Id) on q.RowId equals r.RequestId
                                         join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                                         join rstep in DataContext.RequestWorkflowStepMapping.Where(x => x.WorkflowStateId == WorkflowStateEnum.PENDING.Id) on step.Id equals rstep.WorkflowStepId
                                         join ra in DataContext.AppUserRoleMapping on step.RoleId equals ra.RoleId
                                         where ra.AppUserId == filter.ApproverId.Equal && q.RowId == rstep.RequestId
                                         select q;
            }
            PriceListDAOs = DynamicOrder(PriceListDAOs, filter);
            List<PriceList> PriceLists = await DynamicSelect(PriceListDAOs, filter);
            return PriceLists;
        }

        public async Task<int> CountCompleted(PriceListFilter filter)
        {
            IQueryable<PriceListDAO> PriceListDAOs = DataContext.PriceList.AsNoTracking();
            PriceListDAOs = DynamicFilter(PriceListDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                var query1 = from q in PriceListDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId
                             join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                             join rstep in DataContext.RequestWorkflowStepMapping on step.Id equals rstep.WorkflowStepId
                             where
                             (q.RequestStateId != RequestStateEnum.NEW.Id) &&
                             (rstep.WorkflowStateId == WorkflowStateEnum.APPROVED.Id || rstep.WorkflowStateId == WorkflowStateEnum.REJECTED.Id) &&
                             rstep.AppUserId == filter.ApproverId.Equal
                             select q;
                var query2 = from q in PriceListDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId into result
                             from r in result.DefaultIfEmpty()
                             where r == null && q.RequestStateId != RequestStateEnum.NEW.Id && q.CreatorId == filter.ApproverId.Equal
                             select q;
                PriceListDAOs = query1.Union(query2);
            }
            return await PriceListDAOs.Distinct().CountAsync();
        }

        public async Task<List<PriceList>> ListCompleted(PriceListFilter filter)
        {
            if (filter == null) return new List<PriceList>();
            IQueryable<PriceListDAO> PriceListDAOs = DataContext.PriceList.AsNoTracking();
            PriceListDAOs = DynamicFilter(PriceListDAOs, filter);
            if (filter.ApproverId.Equal.HasValue)
            {
                var query1 = from q in PriceListDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId
                             join step in DataContext.WorkflowStep on r.WorkflowDefinitionId equals step.WorkflowDefinitionId
                             join rstep in DataContext.RequestWorkflowStepMapping on step.Id equals rstep.WorkflowStepId
                             where
                             (q.RequestStateId != RequestStateEnum.NEW.Id) &&
                             (rstep.WorkflowStateId == WorkflowStateEnum.APPROVED.Id || rstep.WorkflowStateId == WorkflowStateEnum.REJECTED.Id) &&
                             rstep.AppUserId == filter.ApproverId.Equal
                             select q;
                var query2 = from q in PriceListDAOs
                             join r in DataContext.RequestWorkflowDefinitionMapping on q.RowId equals r.RequestId into result
                             from r in result.DefaultIfEmpty()
                             where r == null && q.RequestStateId != RequestStateEnum.NEW.Id && q.CreatorId == filter.ApproverId.Equal
                             select q;
                PriceListDAOs = query1.Union(query2);
            }
            PriceListDAOs = DynamicOrder(PriceListDAOs, filter);
            List<PriceList> PriceLists = await DynamicSelect(PriceListDAOs, filter);
            return PriceLists;
        }

        public async Task<PriceList> Get(long Id)
        {
            PriceList PriceList = await DataContext.PriceList.AsNoTracking()
            .Where(x => x.Id == Id)
            .Where(x => x.DeletedAt == null).Select(x => new PriceList()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                StartDate = x.StartDate,
                CreatorId = x.CreatorId,
                EndDate = x.EndDate,
                StatusId = x.StatusId,
                OrganizationId = x.OrganizationId,
                PriceListTypeId = x.PriceListTypeId,
                SalesOrderTypeId = x.SalesOrderTypeId,
                RequestStateId = x.RequestStateId,
                RowId = x.RowId,
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                },
                PriceListType = x.PriceListType == null ? null : new PriceListType
                {
                    Id = x.PriceListType.Id,
                    Code = x.PriceListType.Code,
                    Name = x.PriceListType.Name,
                },
                RequestState = x.RequestState == null ? null : new RequestState
                {
                    Id = x.RequestState.Id,
                    Code = x.RequestState.Code,
                    Name = x.RequestState.Name,
                },
                SalesOrderType = x.SalesOrderType == null ? null : new SalesOrderType
                {
                    Id = x.SalesOrderType.Id,
                    Code = x.SalesOrderType.Code,
                    Name = x.SalesOrderType.Name,
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

            if (PriceList == null)
                return null;

            RequestWorkflowDefinitionMappingDAO RequestWorkflowDefinitionMappingDAO = await DataContext.RequestWorkflowDefinitionMapping
               .Where(x => PriceList.RowId == x.RequestId)
               .Include(x => x.RequestState)
               .AsNoTracking()
               .FirstOrDefaultAsync();
            if (RequestWorkflowDefinitionMappingDAO != null)
            {
                PriceList.RequestStateId = RequestWorkflowDefinitionMappingDAO.RequestStateId;
                PriceList.RequestState = new RequestState
                {
                    Id = RequestWorkflowDefinitionMappingDAO.RequestState.Id,
                    Code = RequestWorkflowDefinitionMappingDAO.RequestState.Code,
                    Name = RequestWorkflowDefinitionMappingDAO.RequestState.Name,
                };
            }

            PriceList.PriceListItemMappings = await DataContext.PriceListItemMapping.AsNoTracking()
                .Where(x => x.PriceListId == PriceList.Id)
                .Where(x => x.Item.DeletedAt == null)
                .Select(x => new PriceListItemMapping
                {
                    PriceListId = x.PriceListId,
                    ItemId = x.ItemId,
                    Price = x.Price,
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
                        Product = x.Item.Product == null ? null : new Product
                        {
                            Id = x.Item.Product.Id,
                            ProductProductGroupingMappings = x.Item.Product.ProductProductGroupingMappings != null ?
                            x.Item.Product.ProductProductGroupingMappings.Select(p => new ProductProductGroupingMapping
                            {
                                ProductId = p.ProductId,
                                ProductGroupingId = p.ProductGroupingId,
                                ProductGrouping = new ProductGrouping
                                {
                                    Id = p.ProductGrouping.Id,
                                    Code = p.ProductGrouping.Code,
                                    Name = p.ProductGrouping.Name,
                                    ParentId = p.ProductGrouping.ParentId,
                                    Path = p.ProductGrouping.Path,
                                    Description = p.ProductGrouping.Description,
                                },
                            }).ToList() : null,
                        }
                    },
                }).ToListAsync();
            PriceList.PriceListStoreGroupingMappings = await DataContext.PriceListStoreGroupingMapping.AsNoTracking()
                .Where(x => x.PriceListId == PriceList.Id)
                .Where(x => x.StoreGrouping.DeletedAt == null)
                .Select(x => new PriceListStoreGroupingMapping
                {
                    PriceListId = x.PriceListId,
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
            PriceList.PriceListStoreMappings = await DataContext.PriceListStoreMapping.AsNoTracking()
                .Where(x => x.PriceListId == PriceList.Id)
                .Where(x => x.Store.DeletedAt == null)
                .Select(x => new PriceListStoreMapping
                {
                    PriceListId = x.PriceListId,
                    StoreId = x.StoreId,
                    Store = new Store
                    {
                        Id = x.Store.Id,
                        Code = x.Store.Code,
                        CodeDraft = x.Store.CodeDraft,
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
                        Province = x.Store.Province == null ? null : new Province
                        {
                            Id = x.Store.Province.Id,
                            Name = x.Store.Province.Name,
                            Priority = x.Store.Province.Priority,
                            StatusId = x.Store.Province.StatusId,
                        },
                        StoreGrouping = x.Store.StoreGrouping == null ? null : new StoreGrouping
                        {
                            Id = x.Store.StoreGrouping.Id,
                            Code = x.Store.StoreGrouping.Code,
                            Name = x.Store.StoreGrouping.Name,
                            ParentId = x.Store.StoreGrouping.ParentId,
                            Path = x.Store.StoreGrouping.Path,
                            Level = x.Store.StoreGrouping.Level,
                        },
                        StoreType = x.Store.StoreType == null ? null : new StoreType
                        {
                            Id = x.Store.StoreType.Id,
                            Code = x.Store.StoreType.Code,
                            Name = x.Store.StoreType.Name,
                            StatusId = x.Store.StoreType.StatusId,
                        },
                    },
                }).ToListAsync();
            PriceList.PriceListStoreTypeMappings = await DataContext.PriceListStoreTypeMapping.AsNoTracking()
                .Where(x => x.PriceListId == PriceList.Id)
                .Where(x => x.StoreType.DeletedAt == null)
                .Select(x => new PriceListStoreTypeMapping
                {
                    PriceListId = x.PriceListId,
                    StoreTypeId = x.StoreTypeId,
                    StoreType = new StoreType
                    {
                        Id = x.StoreType.Id,
                        Code = x.StoreType.Code,
                        Name = x.StoreType.Name,
                        StatusId = x.StoreType.StatusId,
                    },
                }).ToListAsync();

            if (PriceList.PriceListItemMappings != null && PriceList.PriceListItemMappings.Any())
            {
                var ItemIds = PriceList.PriceListItemMappings.Select(x => x.ItemId).ToList();
                var PriceListItemHistories = await DataContext.PriceListItemHistory
                    .Where(x => x.PriceListId == PriceList.Id && ItemIds.Contains(x.ItemId))
                    .Select(x => new PriceListItemHistory
                    {
                        Id = x.Id,
                        ItemId = x.ItemId,
                        PriceListId = x.PriceListId,
                        ModifierId = x.ModifierId,
                        UpdatedAt = x.UpdatedAt,
                        NewPrice = x.NewPrice,
                        OldPrice = x.OldPrice,
                        Modifier = x.Modifier == null ? null : new AppUser
                        {
                            Username = x.Modifier.Username,
                            DisplayName = x.Modifier.DisplayName,
                        },
                    }).ToListAsync();

                foreach (var PriceListItemMapping in PriceList.PriceListItemMappings)
                {
                    PriceListItemMapping.PriceListItemHistories = PriceListItemHistories
                        .Where(x => x.ItemId == PriceListItemMapping.ItemId && x.PriceListId == PriceListItemMapping.PriceListId)
                        .ToList();
                }
            }
            return PriceList;
        }
        public async Task<bool> Create(PriceList PriceList)
        {
            PriceListDAO PriceListDAO = new PriceListDAO();
            PriceListDAO.Id = PriceList.Id;
            PriceListDAO.Code = PriceList.Code;
            PriceListDAO.Name = PriceList.Name;
            PriceListDAO.StartDate = PriceList.StartDate;
            PriceListDAO.CreatorId = PriceList.CreatorId;
            PriceListDAO.EndDate = PriceList.EndDate;
            PriceListDAO.StatusId = PriceList.StatusId;
            PriceListDAO.OrganizationId = PriceList.OrganizationId;
            PriceListDAO.PriceListTypeId = PriceList.PriceListTypeId;
            PriceListDAO.SalesOrderTypeId = PriceList.SalesOrderTypeId;
            PriceListDAO.RowId = Guid.NewGuid();
            PriceListDAO.RequestStateId = PriceList.RequestStateId;
            PriceListDAO.CreatedAt = StaticParams.DateTimeNow;
            PriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.PriceList.Add(PriceListDAO);
            await DataContext.SaveChangesAsync();
            PriceList.Id = PriceListDAO.Id;
            await SaveReference(PriceList);
            return true;
        }

        public async Task<bool> Update(PriceList PriceList)
        {
            PriceListDAO PriceListDAO = DataContext.PriceList.Where(x => x.Id == PriceList.Id).FirstOrDefault();
            if (PriceListDAO == null)
                return false;
            PriceListDAO.Id = PriceList.Id;
            PriceListDAO.Code = PriceList.Code;
            PriceListDAO.Name = PriceList.Name;
            PriceListDAO.StartDate = PriceList.StartDate;
            PriceListDAO.CreatorId = PriceList.CreatorId;
            PriceListDAO.EndDate = PriceList.EndDate;
            PriceListDAO.StatusId = PriceList.StatusId;
            PriceListDAO.OrganizationId = PriceList.OrganizationId;
            PriceListDAO.PriceListTypeId = PriceList.PriceListTypeId;
            PriceListDAO.SalesOrderTypeId = PriceList.SalesOrderTypeId;
            PriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(PriceList);
            return true;
        }

        public async Task<bool> Delete(PriceList PriceList)
        {
            await DataContext.PriceList.Where(x => x.Id == PriceList.Id).UpdateFromQueryAsync(x => new PriceListDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<PriceList> PriceLists)
        {
            List<PriceListDAO> PriceListDAOs = new List<PriceListDAO>();
            foreach (PriceList PriceList in PriceLists)
            {
                PriceListDAO PriceListDAO = new PriceListDAO();
                PriceListDAO.Id = PriceList.Id;
                PriceListDAO.Code = PriceList.Code;
                PriceListDAO.Name = PriceList.Name;
                PriceListDAO.StartDate = PriceList.StartDate;
                PriceListDAO.CreatorId = PriceList.CreatorId;
                PriceListDAO.EndDate = PriceList.EndDate;
                PriceListDAO.StatusId = PriceList.StatusId;
                PriceListDAO.OrganizationId = PriceList.OrganizationId;
                PriceListDAO.PriceListTypeId = PriceList.PriceListTypeId;
                PriceListDAO.SalesOrderTypeId = PriceList.SalesOrderTypeId;
                PriceListDAO.CreatedAt = StaticParams.DateTimeNow;
                PriceListDAO.UpdatedAt = StaticParams.DateTimeNow;
                PriceListDAOs.Add(PriceListDAO);
            }
            await DataContext.BulkMergeAsync(PriceListDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PriceList> PriceLists)
        {
            List<long> Ids = PriceLists.Select(x => x.Id).ToList();
            await DataContext.PriceList
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new PriceListDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(PriceList PriceList)
        {
            await DataContext.PriceListItemMapping
                .Where(x => x.PriceListId == PriceList.Id)
                .DeleteFromQueryAsync();
            List<PriceListItemMappingDAO> PriceListItemMappingDAOs = new List<PriceListItemMappingDAO>();
            if (PriceList.PriceListItemMappings != null)
            {
                foreach (PriceListItemMapping PriceListItemMapping in PriceList.PriceListItemMappings)
                {
                    PriceListItemMappingDAO PriceListItemMappingDAO = new PriceListItemMappingDAO();
                    PriceListItemMappingDAO.PriceListId = PriceList.Id;
                    PriceListItemMappingDAO.ItemId = PriceListItemMapping.ItemId;
                    PriceListItemMappingDAO.Price = PriceListItemMapping.Price;
                    PriceListItemMappingDAOs.Add(PriceListItemMappingDAO);
                }
                await DataContext.PriceListItemMapping.BulkMergeAsync(PriceListItemMappingDAOs);

                List<long> ItemIds = PriceListItemMappingDAOs.Select(x => x.ItemId).ToList();
                List<PriceListItemHistoryDAO> PriceListItemHistoryDAOs = await DataContext.PriceListItemHistory
                    .Where(x => x.PriceListId == PriceList.Id && ItemIds.Contains(x.ItemId)).ToListAsync();

                foreach (PriceListItemMapping PriceListItemMapping in PriceList.PriceListItemMappings)
                {
                    PriceListItemMappingDAO PriceListItemMappingDAO = PriceListItemMappingDAOs
                       .Where(x => x.ItemId == PriceListItemMapping.ItemId).FirstOrDefault();
                    if (PriceListItemMappingDAO != null)
                    {
                        if (PriceListItemMapping.PriceListItemHistories != null)
                        {
                            foreach (PriceListItemHistory PriceListItemHistory in PriceListItemMapping.PriceListItemHistories)
                            {
                                PriceListItemHistoryDAO PriceListItemHistoryDAO = PriceListItemHistoryDAOs.Where(x => x.Id == PriceListItemHistory.Id && x.Id != 0).FirstOrDefault();
                                if (PriceListItemHistoryDAO == null)
                                {
                                    PriceListItemHistoryDAO = new PriceListItemHistoryDAO
                                    {
                                        ItemId = PriceListItemMappingDAO.ItemId,
                                        ModifierId = PriceListItemHistory.ModifierId,
                                        OldPrice = PriceListItemHistory.OldPrice,
                                        NewPrice = PriceListItemHistory.NewPrice,
                                        PriceListId = PriceList.Id,
                                        UpdatedAt = StaticParams.DateTimeNow
                                    };
                                    PriceListItemHistoryDAOs.Add(PriceListItemHistoryDAO);
                                }
                            }
                        }
                    }
                }

                await DataContext.PriceListItemHistory.BulkMergeAsync(PriceListItemHistoryDAOs);
            }
            await DataContext.PriceListStoreGroupingMapping
                .Where(x => x.PriceListId == PriceList.Id)
                .DeleteFromQueryAsync();
            List<PriceListStoreGroupingMappingDAO> PriceListStoreGroupingMappingDAOs = new List<PriceListStoreGroupingMappingDAO>();
            if (PriceList.PriceListStoreGroupingMappings != null)
            {
                foreach (PriceListStoreGroupingMapping PriceListStoreGroupingMapping in PriceList.PriceListStoreGroupingMappings)
                {
                    PriceListStoreGroupingMappingDAO PriceListStoreGroupingMappingDAO = new PriceListStoreGroupingMappingDAO();
                    PriceListStoreGroupingMappingDAO.PriceListId = PriceList.Id;
                    PriceListStoreGroupingMappingDAO.StoreGroupingId = PriceListStoreGroupingMapping.StoreGroupingId;
                    PriceListStoreGroupingMappingDAOs.Add(PriceListStoreGroupingMappingDAO);
                }
                await DataContext.PriceListStoreGroupingMapping.BulkMergeAsync(PriceListStoreGroupingMappingDAOs);
            }
            await DataContext.PriceListStoreMapping
                .Where(x => x.PriceListId == PriceList.Id)
                .DeleteFromQueryAsync();
            List<PriceListStoreMappingDAO> PriceListStoreMappingDAOs = new List<PriceListStoreMappingDAO>();
            if (PriceList.PriceListStoreMappings != null)
            {
                foreach (PriceListStoreMapping PriceListStoreMapping in PriceList.PriceListStoreMappings)
                {
                    PriceListStoreMappingDAO PriceListStoreMappingDAO = new PriceListStoreMappingDAO();
                    PriceListStoreMappingDAO.PriceListId = PriceList.Id;
                    PriceListStoreMappingDAO.StoreId = PriceListStoreMapping.StoreId;
                    PriceListStoreMappingDAOs.Add(PriceListStoreMappingDAO);
                }
                await DataContext.PriceListStoreMapping.BulkMergeAsync(PriceListStoreMappingDAOs);
            }
            await DataContext.PriceListStoreTypeMapping
                .Where(x => x.PriceListId == PriceList.Id)
                .DeleteFromQueryAsync();
            List<PriceListStoreTypeMappingDAO> PriceListStoreTypeMappingDAOs = new List<PriceListStoreTypeMappingDAO>();
            if (PriceList.PriceListStoreTypeMappings != null)
            {
                foreach (PriceListStoreTypeMapping PriceListStoreTypeMapping in PriceList.PriceListStoreTypeMappings)
                {
                    PriceListStoreTypeMappingDAO PriceListStoreTypeMappingDAO = new PriceListStoreTypeMappingDAO();
                    PriceListStoreTypeMappingDAO.PriceListId = PriceList.Id;
                    PriceListStoreTypeMappingDAO.StoreTypeId = PriceListStoreTypeMapping.StoreTypeId;
                    PriceListStoreTypeMappingDAOs.Add(PriceListStoreTypeMappingDAO);
                }
                await DataContext.PriceListStoreTypeMapping.BulkMergeAsync(PriceListStoreTypeMappingDAOs);
            }
        }

        public async Task<bool> UpdateState(PriceList PriceList)
        {
            await DataContext.PriceList.Where(x => x.Id == PriceList.Id)
                .UpdateFromQueryAsync(x => new PriceListDAO
                {
                    RequestStateId = PriceList.RequestStateId,
                    UpdatedAt = StaticParams.DateTimeNow
                });
            return true;
        }

    }
}
