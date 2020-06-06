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
    public interface IProblemRepository
    {
        Task<int> Count(ProblemFilter ProblemFilter);
        Task<List<Problem>> List(ProblemFilter ProblemFilter);
        Task<Problem> Get(long Id);
        Task<bool> Create(Problem Problem);
        Task<bool> Update(Problem Problem);
        Task<bool> Delete(Problem Problem);
        Task<bool> BulkMerge(List<Problem> Problems);
        Task<bool> BulkDelete(List<Problem> Problems);
    }
    public class ProblemRepository : IProblemRepository
    {
        private DataContext DataContext;
        public ProblemRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ProblemDAO> DynamicFilter(IQueryable<ProblemDAO> query, ProblemFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.StoreCheckingId != null)
                query = query.Where(q => q.StoreCheckingId, filter.StoreCheckingId);
            if (filter.StoreId != null)
                query = query.Where(q => q.StoreId, filter.StoreId);
            if (filter.CreatorId != null)
                query = query.Where(q => q.CreatorId, filter.CreatorId);
            if (filter.ProblemTypeId != null)
                query = query.Where(q => q.ProblemTypeId, filter.ProblemTypeId);
            if (filter.NoteAt != null)
                query = query.Where(q => q.NoteAt, filter.NoteAt);
            if (filter.CompletedAt != null)
                query = query.Where(q => q.CompletedAt, filter.CompletedAt);
            if (filter.Content != null)
                query = query.Where(q => q.Content, filter.Content);
            if (filter.ProblemStatusId != null)
                query = query.Where(q => q.ProblemStatusId, filter.ProblemStatusId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ProblemDAO> OrFilter(IQueryable<ProblemDAO> query, ProblemFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ProblemDAO> initQuery = query.Where(q => false);
            foreach (ProblemFilter ProblemFilter in filter.OrFilter)
            {
                IQueryable<ProblemDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.StoreCheckingId != null)
                    queryable = queryable.Where(q => q.StoreCheckingId, filter.StoreCheckingId);
                if (filter.StoreId != null)
                    queryable = queryable.Where(q => q.StoreId, filter.StoreId);
                if (filter.CreatorId != null)
                    queryable = queryable.Where(q => q.CreatorId, filter.CreatorId);
                if (filter.ProblemTypeId != null)
                    queryable = queryable.Where(q => q.ProblemTypeId, filter.ProblemTypeId);
                if (filter.NoteAt != null)
                    queryable = queryable.Where(q => q.NoteAt, filter.NoteAt);
                if (filter.CompletedAt != null)
                    queryable = queryable.Where(q => q.CompletedAt, filter.CompletedAt);
                if (filter.Content != null)
                    queryable = queryable.Where(q => q.Content, filter.Content);
                if (filter.ProblemStatusId != null)
                    queryable = queryable.Where(q => q.ProblemStatusId, filter.ProblemStatusId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ProblemDAO> DynamicOrder(IQueryable<ProblemDAO> query, ProblemFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ProblemOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ProblemOrder.StoreChecking:
                            query = query.OrderBy(q => q.StoreCheckingId);
                            break;
                        case ProblemOrder.Store:
                            query = query.OrderBy(q => q.StoreId);
                            break;
                        case ProblemOrder.Creator:
                            query = query.OrderBy(q => q.CreatorId);
                            break;
                        case ProblemOrder.ProblemType:
                            query = query.OrderBy(q => q.ProblemTypeId);
                            break;
                        case ProblemOrder.NoteAt:
                            query = query.OrderBy(q => q.NoteAt);
                            break;
                        case ProblemOrder.CompletedAt:
                            query = query.OrderBy(q => q.CompletedAt);
                            break;
                        case ProblemOrder.Content:
                            query = query.OrderBy(q => q.Content);
                            break;
                        case ProblemOrder.ProblemStatus:
                            query = query.OrderBy(q => q.ProblemStatusId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ProblemOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ProblemOrder.StoreChecking:
                            query = query.OrderByDescending(q => q.StoreCheckingId);
                            break;
                        case ProblemOrder.Store:
                            query = query.OrderByDescending(q => q.StoreId);
                            break;
                        case ProblemOrder.Creator:
                            query = query.OrderByDescending(q => q.CreatorId);
                            break;
                        case ProblemOrder.ProblemType:
                            query = query.OrderByDescending(q => q.ProblemTypeId);
                            break;
                        case ProblemOrder.NoteAt:
                            query = query.OrderByDescending(q => q.NoteAt);
                            break;
                        case ProblemOrder.CompletedAt:
                            query = query.OrderByDescending(q => q.CompletedAt);
                            break;
                        case ProblemOrder.Content:
                            query = query.OrderByDescending(q => q.Content);
                            break;
                        case ProblemOrder.ProblemStatus:
                            query = query.OrderByDescending(q => q.ProblemStatusId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Problem>> DynamicSelect(IQueryable<ProblemDAO> query, ProblemFilter filter)
        {
            List<Problem> Problems = await query.Select(q => new Problem()
            {
                Id = filter.Selects.Contains(ProblemSelect.Id) ? q.Id : default(long),
                StoreCheckingId = filter.Selects.Contains(ProblemSelect.StoreChecking) ? q.StoreCheckingId : default(long?),
                StoreId = filter.Selects.Contains(ProblemSelect.Store) ? q.StoreId : default(long),
                CreatorId = filter.Selects.Contains(ProblemSelect.Creator) ? q.CreatorId : default(long),
                ProblemTypeId = filter.Selects.Contains(ProblemSelect.ProblemType) ? q.ProblemTypeId : default(long),
                NoteAt = filter.Selects.Contains(ProblemSelect.NoteAt) ? q.NoteAt : default(DateTime),
                CompletedAt = filter.Selects.Contains(ProblemSelect.CompletedAt) ? q.CompletedAt : default(DateTime?),
                Content = filter.Selects.Contains(ProblemSelect.Content) ? q.Content : default(string),
                ProblemStatusId = filter.Selects.Contains(ProblemSelect.ProblemStatus) ? q.ProblemStatusId : default(long),
                Creator = filter.Selects.Contains(ProblemSelect.Creator) && q.Creator != null ? new AppUser
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
                ProblemStatus = filter.Selects.Contains(ProblemSelect.ProblemStatus) && q.ProblemStatus != null ? new ProblemStatus
                {
                    Id = q.ProblemStatus.Id,
                    Code = q.ProblemStatus.Code,
                    Name = q.ProblemStatus.Name,
                } : null,
                ProblemType = filter.Selects.Contains(ProblemSelect.ProblemType) && q.ProblemType != null ? new ProblemType
                {
                    Id = q.ProblemType.Id,
                    Code = q.ProblemType.Code,
                    Name = q.ProblemType.Name,
                } : null,
                Store = filter.Selects.Contains(ProblemSelect.Store) && q.Store != null ? new Store
                {
                    Id = q.Store.Id,
                    Code = q.Store.Code,
                    Name = q.Store.Name,
                    ParentStoreId = q.Store.ParentStoreId,
                    OrganizationId = q.Store.OrganizationId,
                    StoreTypeId = q.Store.StoreTypeId,
                    StoreGroupingId = q.Store.StoreGroupingId,
                    ResellerId = q.Store.ResellerId,
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
                StoreChecking = filter.Selects.Contains(ProblemSelect.StoreChecking) && q.StoreChecking != null ? new StoreChecking
                {
                    Id = q.StoreChecking.Id,
                    StoreId = q.StoreChecking.StoreId,
                    SaleEmployeeId = q.StoreChecking.SaleEmployeeId,
                    Longtitude = q.StoreChecking.Longtitude,
                    Latitude = q.StoreChecking.Latitude,
                    CheckInAt = q.StoreChecking.CheckInAt,
                    CheckOutAt = q.StoreChecking.CheckOutAt,
                } : null,
            }).ToListAsync();
            return Problems;
        }

        public async Task<int> Count(ProblemFilter filter)
        {
            IQueryable<ProblemDAO> Problems = DataContext.Problem.AsNoTracking();
            Problems = DynamicFilter(Problems, filter);
            return await Problems.CountAsync();
        }

        public async Task<List<Problem>> List(ProblemFilter filter)
        {
            if (filter == null) return new List<Problem>();
            IQueryable<ProblemDAO> ProblemDAOs = DataContext.Problem.AsNoTracking();
            ProblemDAOs = DynamicFilter(ProblemDAOs, filter);
            ProblemDAOs = DynamicOrder(ProblemDAOs, filter);
            List<Problem> Problems = await DynamicSelect(ProblemDAOs, filter);
            return Problems;
        }

        public async Task<Problem> Get(long Id)
        {
            Problem Problem = await DataContext.Problem.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Problem()
            {
                Id = x.Id,
                StoreCheckingId = x.StoreCheckingId,
                StoreId = x.StoreId,
                CreatorId = x.CreatorId,
                ProblemTypeId = x.ProblemTypeId,
                NoteAt = x.NoteAt,
                CompletedAt = x.CompletedAt,
                Content = x.Content,
                ProblemStatusId = x.ProblemStatusId,
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
                ProblemStatus = x.ProblemStatus == null ? null : new ProblemStatus
                {
                    Id = x.ProblemStatus.Id,
                    Code = x.ProblemStatus.Code,
                    Name = x.ProblemStatus.Name,
                },
                ProblemType = x.ProblemType == null ? null : new ProblemType
                {
                    Id = x.ProblemType.Id,
                    Code = x.ProblemType.Code,
                    Name = x.ProblemType.Name,
                },
                Store = x.Store == null ? null : new Store
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
                StoreChecking = x.StoreChecking == null ? null : new StoreChecking
                {
                    Id = x.StoreChecking.Id,
                    StoreId = x.StoreChecking.StoreId,
                    SaleEmployeeId = x.StoreChecking.SaleEmployeeId,
                    Longtitude = x.StoreChecking.Longtitude,
                    Latitude = x.StoreChecking.Latitude,
                    CheckInAt = x.StoreChecking.CheckInAt,
                    CheckOutAt = x.StoreChecking.CheckOutAt,
                },
            }).FirstOrDefaultAsync();

            if (Problem == null)
                return null;
            Problem.ProblemImageMappings = await DataContext.ProblemImageMapping.AsNoTracking()
                .Where(x => x.ProblemId == Problem.Id)
                .Where(x => x.Image.DeletedAt == null)
                .Select(x => new ProblemImageMapping
                {
                    ProblemId = x.ProblemId,
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                    },
                }).ToListAsync();

            return Problem;
        }
        public async Task<bool> Create(Problem Problem)
        {
            ProblemDAO ProblemDAO = new ProblemDAO();
            ProblemDAO.Id = Problem.Id;
            ProblemDAO.StoreCheckingId = Problem.StoreCheckingId;
            ProblemDAO.StoreId = Problem.StoreId;
            ProblemDAO.CreatorId = Problem.CreatorId;
            ProblemDAO.ProblemTypeId = Problem.ProblemTypeId;
            ProblemDAO.NoteAt = StaticParams.DateTimeNow;
            ProblemDAO.CompletedAt = Problem.CompletedAt;
            ProblemDAO.Content = Problem.Content;
            ProblemDAO.ProblemStatusId = Problem.ProblemStatusId;
            DataContext.Problem.Add(ProblemDAO);
            await DataContext.SaveChangesAsync();
            Problem.Id = ProblemDAO.Id;
            await SaveReference(Problem);
            return true;
        }

        public async Task<bool> Update(Problem Problem)
        {
            ProblemDAO ProblemDAO = DataContext.Problem.Where(x => x.Id == Problem.Id).FirstOrDefault();
            if (ProblemDAO == null)
                return false;
            ProblemDAO.Id = Problem.Id;
            ProblemDAO.StoreCheckingId = Problem.StoreCheckingId;
            ProblemDAO.StoreId = Problem.StoreId;
            ProblemDAO.CreatorId = Problem.CreatorId;
            ProblemDAO.ProblemTypeId = Problem.ProblemTypeId;
            ProblemDAO.CompletedAt = Problem.CompletedAt;
            ProblemDAO.Content = Problem.Content;
            ProblemDAO.ProblemStatusId = Problem.ProblemStatusId;
            await DataContext.SaveChangesAsync();
            await SaveReference(Problem);
            return true;
        }

        public async Task<bool> Delete(Problem Problem)
        {
            await DataContext.ProblemImageMapping.Where(x => x.ProblemId == Problem.Id).DeleteFromQueryAsync();
            await DataContext.Problem.Where(x => x.Id == Problem.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Problem> Problems)
        {
            List<ProblemDAO> ProblemDAOs = new List<ProblemDAO>();
            foreach (Problem Problem in Problems)
            {
                ProblemDAO ProblemDAO = new ProblemDAO();
                ProblemDAO.Id = Problem.Id;
                ProblemDAO.StoreCheckingId = Problem.StoreCheckingId;
                ProblemDAO.StoreId = Problem.StoreId;
                ProblemDAO.CreatorId = Problem.CreatorId;
                ProblemDAO.ProblemTypeId = Problem.ProblemTypeId;
                ProblemDAO.NoteAt = Problem.NoteAt;
                ProblemDAO.CompletedAt = Problem.CompletedAt;
                ProblemDAO.Content = Problem.Content;
                ProblemDAO.ProblemStatusId = Problem.ProblemStatusId;
                ProblemDAOs.Add(ProblemDAO);
            }
            await DataContext.BulkMergeAsync(ProblemDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Problem> Problems)
        {
            List<long> Ids = Problems.Select(x => x.Id).ToList();
            await DataContext.ProblemImageMapping.Where(x => Ids.Contains(x.ProblemId)).DeleteFromQueryAsync();
            await DataContext.Problem
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(Problem Problem)
        {
            await DataContext.ProblemImageMapping
                .Where(x => x.ProblemId == Problem.Id)
                .DeleteFromQueryAsync();
            List<ProblemImageMappingDAO> ProblemImageMappingDAOs = new List<ProblemImageMappingDAO>();
            if (Problem.ProblemImageMappings != null)
            {
                foreach (ProblemImageMapping ProblemImageMapping in Problem.ProblemImageMappings)
                {
                    ProblemImageMappingDAO ProblemImageMappingDAO = new ProblemImageMappingDAO();
                    ProblemImageMappingDAO.ProblemId = Problem.Id;
                    ProblemImageMappingDAO.ImageId = ProblemImageMapping.ImageId;
                    ProblemImageMappingDAOs.Add(ProblemImageMappingDAO);
                }
                await DataContext.ProblemImageMapping.BulkMergeAsync(ProblemImageMappingDAOs);
            }
        }
        
    }
}
