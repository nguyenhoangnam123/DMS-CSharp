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
    public interface IWorkflowDefinitionRepository
    {
        Task<int> Count(WorkflowDefinitionFilter WorkflowDefinitionFilter);
        Task<List<WorkflowDefinition>> List(WorkflowDefinitionFilter WorkflowDefinitionFilter);
        Task<WorkflowDefinition> Get(long Id);
        Task<bool> Create(WorkflowDefinition WorkflowDefinition);
        Task<bool> Update(WorkflowDefinition WorkflowDefinition);
        Task<bool> Delete(WorkflowDefinition WorkflowDefinition);
        Task<bool> BulkMerge(List<WorkflowDefinition> WorkflowDefinitions);
        Task<bool> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions);
    }
    public class WorkflowDefinitionRepository : IWorkflowDefinitionRepository
    {
        private DataContext DataContext;
        public WorkflowDefinitionRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<WorkflowDefinitionDAO> DynamicFilter(IQueryable<WorkflowDefinitionDAO> query, WorkflowDefinitionFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => !q.DeletedAt.HasValue);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Code != null)
                query = query.Where(q => q.Code, filter.Code);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            if (filter.ModifierId != null)
                query = query.Where(q => q.ModifierId, filter.ModifierId);
            if (filter.WorkflowTypeId != null)
                query = query.Where(q => q.WorkflowTypeId, filter.WorkflowTypeId);
            if (filter.StartDate != null)
                query = query.Where(q => q.StartDate, filter.StartDate);
            if (filter.EndDate != null)
                query = query.Where(q => q.EndDate, filter.EndDate);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.CreatedAt != null)
                query = query.Where(q => q.CreatedAt, filter.CreatedAt);
            if (filter.UpdatedAt != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedAt);
            if (filter.Used != null)
                query = query.Where(q => q.Used == filter.Used.Value);
            if (filter.OrganizationId != null)
            {
                if (filter.OrganizationId.Equal != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.Equal.Value && o.StatusId == 1).FirstOrDefault();
                    query = query.Where(q => q.Organization.Path.StartsWith(OrganizationDAO.Path));
                }
                if (filter.OrganizationId.NotEqual != null)
                {
                    OrganizationDAO OrganizationDAO = DataContext.Organization
                        .Where(o => o.Id == filter.OrganizationId.NotEqual.Value && o.StatusId == 1).FirstOrDefault();
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
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<WorkflowDefinitionDAO> OrFilter(IQueryable<WorkflowDefinitionDAO> query, WorkflowDefinitionFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<WorkflowDefinitionDAO> initQuery = query.Where(q => false);
            foreach (WorkflowDefinitionFilter WorkflowDefinitionFilter in filter.OrFilter)
            {
                IQueryable<WorkflowDefinitionDAO> queryable = query;
                if (WorkflowDefinitionFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, WorkflowDefinitionFilter.Id);
                if (WorkflowDefinitionFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, WorkflowDefinitionFilter.Code);
                if (WorkflowDefinitionFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, WorkflowDefinitionFilter.Name);
                if (WorkflowDefinitionFilter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, WorkflowDefinitionFilter.CreatorId);
                if (WorkflowDefinitionFilter.ModifierId != null)
                    queryable = queryable.Where(q => q.ModifierId, WorkflowDefinitionFilter.ModifierId);
                if (WorkflowDefinitionFilter.WorkflowTypeId != null)
                    queryable = queryable.Where(q => q.WorkflowTypeId, WorkflowDefinitionFilter.WorkflowTypeId);
                if (WorkflowDefinitionFilter.StartDate != null)
                    queryable = queryable.Where(q => q.StartDate, WorkflowDefinitionFilter.StartDate);
                if (WorkflowDefinitionFilter.EndDate != null)
                    queryable = queryable.Where(q => q.EndDate, WorkflowDefinitionFilter.EndDate);
                if (WorkflowDefinitionFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, WorkflowDefinitionFilter.StatusId);
                if (WorkflowDefinitionFilter.UpdatedAt != null)
                    queryable = queryable.Where(q => q.UpdatedAt, WorkflowDefinitionFilter.UpdatedAt);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<WorkflowDefinitionDAO> DynamicOrder(IQueryable<WorkflowDefinitionDAO> query, WorkflowDefinitionFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowDefinitionOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case WorkflowDefinitionOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case WorkflowDefinitionOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case WorkflowDefinitionOrder.WorkflowType:
                            query = query.OrderBy(q => q.WorkflowTypeId);
                            break;
                        case WorkflowDefinitionOrder.Modifier:
                            query = query.OrderBy(q => q.Modifier);
                            break;
                        case WorkflowDefinitionOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case WorkflowDefinitionOrder.StartDate:
                            query = query.OrderBy(q => q.StartDate);
                            break;
                        case WorkflowDefinitionOrder.EndDate:
                            query = query.OrderBy(q => q.EndDate);
                            break;
                        case WorkflowDefinitionOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case WorkflowDefinitionOrder.CreatedAt:
                            query = query.OrderBy(q => q.CreatedAt);
                            break;
                        case WorkflowDefinitionOrder.UpdatedAt:
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case WorkflowDefinitionOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case WorkflowDefinitionOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case WorkflowDefinitionOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case WorkflowDefinitionOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case WorkflowDefinitionOrder.Modifier:
                            query = query.OrderByDescending(q => q.Modifier);
                            break;
                        case WorkflowDefinitionOrder.WorkflowType:
                            query = query.OrderByDescending(q => q.WorkflowTypeId);
                            break;
                        case WorkflowDefinitionOrder.StartDate:
                            query = query.OrderByDescending(q => q.StartDate);
                            break;
                        case WorkflowDefinitionOrder.EndDate:
                            query = query.OrderByDescending(q => q.EndDate);
                            break;
                        case WorkflowDefinitionOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case WorkflowDefinitionOrder.UpdatedAt:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                        case WorkflowDefinitionOrder.CreatedAt:
                            query = query.OrderByDescending(q => q.CreatedAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<WorkflowDefinition>> DynamicSelect(IQueryable<WorkflowDefinitionDAO> query, WorkflowDefinitionFilter filter)
        {
            List<WorkflowDefinition> WorkflowDefinitions = await query.Select(q => new WorkflowDefinition()
            {
                Id = filter.Selects.Contains(WorkflowDefinitionSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(WorkflowDefinitionSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(WorkflowDefinitionSelect.Name) ? q.Name : default(string),
                CreatorId = filter.Selects.Contains(WorkflowDefinitionSelect.Creator) ? q.CreatorId : default(long),
                ModifierId = filter.Selects.Contains(WorkflowDefinitionSelect.Modifier) ? q.ModifierId : default(long),
                WorkflowTypeId = filter.Selects.Contains(WorkflowDefinitionSelect.WorkflowType) ? q.WorkflowTypeId : default(long),
                OrganizationId = filter.Selects.Contains(WorkflowDefinitionSelect.Organization) ? q.OrganizationId : default(long),
                StartDate = filter.Selects.Contains(WorkflowDefinitionSelect.StartDate) ? q.StartDate : default(DateTime?),
                EndDate = filter.Selects.Contains(WorkflowDefinitionSelect.EndDate) ? q.EndDate : default(DateTime?),
                StatusId = filter.Selects.Contains(WorkflowDefinitionSelect.Status) ? q.StatusId : default(long),
                CreatedAt = filter.Selects.Contains(WorkflowDefinitionSelect.CreatedAt) ? q.CreatedAt : default(DateTime),
                UpdatedAt = filter.Selects.Contains(WorkflowDefinitionSelect.UpdatedAt) ? q.UpdatedAt : default(DateTime),
                Organization = filter.Selects.Contains(WorkflowDefinitionSelect.Organization) && q.Organization != null ? new Organization
                {
                    Id = q.Organization.Id,
                    Code = q.Organization.Code,
                    Name = q.Organization.Name,
                    ParentId = q.Organization.ParentId,
                    Path = q.Organization.Path,
                    Level = q.Organization.Level,
                    StatusId = q.Organization.StatusId,
                } : null,
                Creator = filter.Selects.Contains(WorkflowDefinitionSelect.Creator) && q.Creator != null ? new AppUser
                {
                    Id = q.Creator.Id,
                    Username = q.Creator.Username,
                    DisplayName = q.Creator.DisplayName,
                    Address = q.Creator.Address,
                    Email = q.Creator.Email,
                    Phone = q.Creator.Phone,
                } : null,
                Modifier = filter.Selects.Contains(WorkflowDefinitionSelect.Modifier) && q.Modifier != null ? new AppUser
                {
                    Id = q.Modifier.Id,
                    Username = q.Modifier.Username,
                    DisplayName = q.Modifier.DisplayName,
                    Address = q.Modifier.Address,
                    Email = q.Modifier.Email,
                    Phone = q.Modifier.Phone,
                } : null,
                WorkflowType = filter.Selects.Contains(WorkflowDefinitionSelect.WorkflowType) && q.WorkflowType != null ? new WorkflowType
                {
                    Id = q.WorkflowType.Id,
                    Code = q.WorkflowType.Code,
                    Name = q.WorkflowType.Name,
                } : null,
                Status = filter.Selects.Contains(WorkflowDefinitionSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Used = q.Used,
            }).ToListAsync();

            return WorkflowDefinitions;
        }

        public async Task<int> Count(WorkflowDefinitionFilter filter)
        {
            IQueryable<WorkflowDefinitionDAO> WorkflowDefinitions = DataContext.WorkflowDefinition;
            WorkflowDefinitions = DynamicFilter(WorkflowDefinitions, filter);
            return await WorkflowDefinitions.CountAsync();
        }

        public async Task<List<WorkflowDefinition>> List(WorkflowDefinitionFilter filter)
        {
            if (filter == null) return new List<WorkflowDefinition>();
            IQueryable<WorkflowDefinitionDAO> WorkflowDefinitionDAOs = DataContext.WorkflowDefinition;
            WorkflowDefinitionDAOs = DynamicFilter(WorkflowDefinitionDAOs, filter);
            WorkflowDefinitionDAOs = DynamicOrder(WorkflowDefinitionDAOs, filter);
            List<WorkflowDefinition> WorkflowDefinitions = await DynamicSelect(WorkflowDefinitionDAOs, filter);
            return WorkflowDefinitions;
        }

        public async Task<WorkflowDefinition> Get(long Id)
        {
            WorkflowDefinition WorkflowDefinition = await DataContext.WorkflowDefinition.Where(x => x.Id == Id).Select(x => new WorkflowDefinition()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                CreatorId = x.CreatorId,
                ModifierId = x.ModifierId,
                WorkflowTypeId = x.WorkflowTypeId,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StatusId = x.StatusId,
                OrganizationId = x.OrganizationId,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Used = x.Used,
                Organization = x.Organization == null ? null : new Organization
                {
                    Id = x.Organization.Id,
                    Code = x.Organization.Code,
                    Name = x.Organization.Name,
                    ParentId = x.Organization.ParentId,
                    Path = x.Organization.Path,
                    Level = x.Organization.Level,
                    StatusId = x.Organization.StatusId,
                },
                Creator = x.Creator == null ? null : new AppUser
                {
                    Id = x.Creator.Id,
                    Username = x.Creator.Username,
                    DisplayName = x.Creator.DisplayName,
                    Address = x.Creator.Address,
                    Email = x.Creator.Email,
                    Phone = x.Creator.Phone,
                },
                Modifier = x.Modifier == null ? null : new AppUser
                {
                    Id = x.Modifier.Id,
                    Username = x.Modifier.Username,
                    DisplayName = x.Modifier.DisplayName,
                    Address = x.Modifier.Address,
                    Email = x.Modifier.Email,
                    Phone = x.Modifier.Phone,
                },
                WorkflowType = x.WorkflowType == null ? null : new WorkflowType
                {
                    Id = x.WorkflowType.Id,
                    Code = x.WorkflowType.Code,
                    Name = x.WorkflowType.Name,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).AsNoTracking().FirstOrDefaultAsync();

            if (WorkflowDefinition == null)
                return null;
            WorkflowDefinition.WorkflowSteps = await DataContext.WorkflowStep
                .Where(x => x.WorkflowDefinitionId == Id).Select(x => new WorkflowStep
                {
                    BodyMailForReject = x.BodyMailForReject,
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    RoleId = x.RoleId,
                    SubjectMailForReject = x.SubjectMailForReject,
                    WorkflowDefinitionId = x.WorkflowDefinitionId,
                    Role = x.Role == null ? null : new Role
                    {
                        Code = x.Role.Code,
                        Name = x.Role.Name,
                    },
                }).ToListAsync();
            WorkflowDefinition.WorkflowDirections = await DataContext.WorkflowDirection
                .Where(x => x.WorkflowDefinitionId == Id).Select(x => new WorkflowDirection
                {
                    Id = x.Id,
                    FromStepId = x.FromStepId,
                    ToStepId = x.ToStepId,
                    SubjectMailForCreator = x.SubjectMailForCreator,
                    SubjectMailForNextStep = x.SubjectMailForNextStep,
                    BodyMailForCreator = x.BodyMailForCreator,
                    BodyMailForNextStep = x.BodyMailForNextStep,
                    WorkflowDefinitionId = x.WorkflowDefinitionId,
                    UpdatedAt = x.UpdatedAt,
                    FromStep = new WorkflowStep
                    {
                        Id = x.FromStep.Id,
                        Code = x.FromStep.Code,
                        Name = x.FromStep.Name,
                        RoleId = x.FromStep.RoleId,
                        WorkflowDefinitionId = x.FromStep.WorkflowDefinitionId,
                        SubjectMailForReject = x.FromStep.SubjectMailForReject,
                        BodyMailForReject = x.FromStep.BodyMailForReject,
                        Role = x.FromStep.Role == null ? null : new Role
                        {
                            Code = x.FromStep.Role.Code,
                            Name = x.FromStep.Role.Name,
                        }
                    },
                    ToStep = new WorkflowStep
                    {
                        Id = x.ToStep.Id,
                        Code = x.ToStep.Code,
                        Name = x.ToStep.Name,
                        RoleId = x.ToStep.RoleId,
                        WorkflowDefinitionId = x.ToStep.WorkflowDefinitionId,
                        SubjectMailForReject = x.ToStep.SubjectMailForReject,
                        BodyMailForReject = x.ToStep.BodyMailForReject,
                        Role = x.ToStep.Role == null ? null : new Role
                        {
                            Code = x.ToStep.Role.Code,
                            Name = x.ToStep.Role.Name,
                        }
                    },
                }).ToListAsync();


            List<WorkflowDirectionCondition> WorkflowDirectionConditions = await DataContext.WorkflowDirectionCondition
                .Where(x => x.WorkflowDirection.WorkflowDefinitionId == Id)
                .Select(x => new WorkflowDirectionCondition
                {
                    Id = x.Id,
                    Value = x.Value,
                    WorkflowDirectionId = x.WorkflowDirectionId,
                    WorkflowOperatorId = x.WorkflowOperatorId,
                    WorkflowParameterId = x.WorkflowParameterId,
                    WorkflowParameter = x.WorkflowParameter == null ? null : new WorkflowParameter
                    {
                        Id = x.WorkflowParameter.Id,
                        WorkflowParameterTypeId = x.WorkflowParameter.WorkflowParameterTypeId,
                        WorkflowTypeId = x.WorkflowParameter.WorkflowTypeId,
                    }
                }).ToListAsync();

            foreach (WorkflowDirection WorkflowDirection in WorkflowDefinition.WorkflowDirections)
            {
                WorkflowDirection.WorkflowDirectionConditions = WorkflowDirectionConditions.Where(x => x.WorkflowDirectionId == WorkflowDirection.Id).ToList();
            }

            WorkflowDefinition.WorkflowParameters = await DataContext.WorkflowParameter
                .Where(x => x.WorkflowTypeId == WorkflowDefinition.WorkflowTypeId)
                .Select(x => new WorkflowParameter
                {
                    Id = x.Id,
                    Name = x.Name,
                    WorkflowTypeId = x.WorkflowTypeId
                }).ToListAsync();

            return WorkflowDefinition;
        }
        public async Task<bool> Create(WorkflowDefinition WorkflowDefinition)
        {
            WorkflowDefinitionDAO WorkflowDefinitionDAO = new WorkflowDefinitionDAO();
            WorkflowDefinitionDAO.Id = WorkflowDefinition.Id;
            WorkflowDefinitionDAO.Name = WorkflowDefinition.Name;
            WorkflowDefinitionDAO.Code = WorkflowDefinition.Code;
            WorkflowDefinitionDAO.CreatorId = WorkflowDefinition.CreatorId;
            WorkflowDefinitionDAO.ModifierId = WorkflowDefinition.ModifierId;
            WorkflowDefinitionDAO.WorkflowTypeId = WorkflowDefinition.WorkflowTypeId;
            WorkflowDefinitionDAO.OrganizationId = WorkflowDefinition.OrganizationId;
            WorkflowDefinitionDAO.StartDate = WorkflowDefinition.StartDate;
            WorkflowDefinitionDAO.EndDate = WorkflowDefinition.EndDate;
            WorkflowDefinitionDAO.StatusId = WorkflowDefinition.StatusId;
            WorkflowDefinitionDAO.CreatedAt = StaticParams.DateTimeNow;
            WorkflowDefinitionDAO.UpdatedAt = StaticParams.DateTimeNow;
            WorkflowDefinitionDAO.Used = false;
            DataContext.WorkflowDefinition.Add(WorkflowDefinitionDAO);
            await DataContext.SaveChangesAsync();
            WorkflowDefinition.Id = WorkflowDefinitionDAO.Id;
            return true;
        }

        public async Task<bool> Update(WorkflowDefinition WorkflowDefinition)
        {
            WorkflowDefinitionDAO WorkflowDefinitionDAO = DataContext.WorkflowDefinition.Where(x => x.Id == WorkflowDefinition.Id).FirstOrDefault();
            if (WorkflowDefinitionDAO == null)
                return false;
            WorkflowDefinitionDAO.Id = WorkflowDefinition.Id;
            WorkflowDefinitionDAO.Name = WorkflowDefinition.Name;
            WorkflowDefinitionDAO.Code = WorkflowDefinition.Code;
            WorkflowDefinitionDAO.CreatorId = WorkflowDefinition.CreatorId;
            WorkflowDefinitionDAO.ModifierId = WorkflowDefinition.ModifierId;
            WorkflowDefinitionDAO.OrganizationId = WorkflowDefinition.OrganizationId;
            WorkflowDefinitionDAO.WorkflowTypeId = WorkflowDefinition.WorkflowTypeId;
            WorkflowDefinitionDAO.StartDate = WorkflowDefinition.StartDate;
            WorkflowDefinitionDAO.EndDate = WorkflowDefinition.EndDate;
            WorkflowDefinitionDAO.StatusId = WorkflowDefinition.StatusId;
            WorkflowDefinitionDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(WorkflowDefinition WorkflowDefinition)
        {
            await DataContext.WorkflowDefinition.Where(x => x.Id == WorkflowDefinition.Id).UpdateFromQueryAsync(x => new WorkflowDefinitionDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        public async Task<bool> BulkMerge(List<WorkflowDefinition> WorkflowDefinitions)
        {
            List<WorkflowDefinitionDAO> WorkflowDefinitionDAOs = new List<WorkflowDefinitionDAO>();
            foreach (WorkflowDefinition WorkflowDefinition in WorkflowDefinitions)
            {
                WorkflowDefinitionDAO WorkflowDefinitionDAO = new WorkflowDefinitionDAO();
                WorkflowDefinitionDAO.Id = WorkflowDefinition.Id;
                WorkflowDefinitionDAO.Name = WorkflowDefinition.Name;
                WorkflowDefinitionDAO.Code = WorkflowDefinition.Code;
                WorkflowDefinitionDAO.CreatorId = WorkflowDefinition.CreatorId;
                WorkflowDefinitionDAO.ModifierId = WorkflowDefinition.ModifierId;
                WorkflowDefinitionDAO.WorkflowTypeId = WorkflowDefinition.WorkflowTypeId;
                WorkflowDefinitionDAO.StartDate = WorkflowDefinition.StartDate;
                WorkflowDefinitionDAO.EndDate = WorkflowDefinition.EndDate;
                WorkflowDefinitionDAO.StatusId = WorkflowDefinition.StatusId;
                WorkflowDefinitionDAO.CreatedAt = StaticParams.DateTimeNow;
                WorkflowDefinitionDAO.UpdatedAt = StaticParams.DateTimeNow;
                WorkflowDefinitionDAOs.Add(WorkflowDefinitionDAO);
            }
            await DataContext.BulkMergeAsync(WorkflowDefinitionDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<WorkflowDefinition> WorkflowDefinitions)
        {
            List<long> Ids = WorkflowDefinitions.Select(x => x.Id).ToList();
            await DataContext.WorkflowDefinition
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new WorkflowDefinitionDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
    }
}
