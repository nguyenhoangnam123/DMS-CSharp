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
    public interface IResellerRepository
    {
        Task<int> Count(ResellerFilter ResellerFilter);
        Task<List<Reseller>> List(ResellerFilter ResellerFilter);
        Task<Reseller> Get(long Id);
        Task<bool> Create(Reseller Reseller);
        Task<bool> Update(Reseller Reseller);
        Task<bool> Delete(Reseller Reseller);
        Task<bool> BulkMerge(List<Reseller> Resellers);
        Task<bool> BulkDelete(List<Reseller> Resellers);
    }
    public class ResellerRepository : IResellerRepository
    {
        private DataContext DataContext;
        public ResellerRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ResellerDAO> DynamicFilter(IQueryable<ResellerDAO> query, ResellerFilter filter)
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
            if (filter.Phone != null)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.Email != null)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.TaxCode != null)
                query = query.Where(q => q.TaxCode, filter.TaxCode);
            if (filter.CompanyName != null)
                query = query.Where(q => q.CompanyName, filter.CompanyName);
            if (filter.DeputyName != null)
                query = query.Where(q => q.DeputyName, filter.DeputyName);
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.ResellerTypeId != null)
                query = query.Where(q => q.ResellerTypeId, filter.ResellerTypeId);
            if (filter.ResellerStatusId != null)
                query = query.Where(q => q.ResellerStatusId, filter.ResellerStatusId);
            if (filter.StaffId != null)
                query = query.Where(q => q.StaffId, filter.StaffId);
            query = OrFilter(query, filter);
            return query;
        }

         private IQueryable<ResellerDAO> OrFilter(IQueryable<ResellerDAO> query, ResellerFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ResellerDAO> initQuery = query.Where(q => false);
            foreach (ResellerFilter ResellerFilter in filter.OrFilter)
            {
                IQueryable<ResellerDAO> queryable = query;
                if (filter.Id != null)
                    queryable = queryable.Where(q => q.Id, filter.Id);
                if (filter.Code != null)
                    queryable = queryable.Where(q => q.Code, filter.Code);
                if (filter.Name != null)
                    queryable = queryable.Where(q => q.Name, filter.Name);
                if (filter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, filter.Phone);
                if (filter.Email != null)
                    queryable = queryable.Where(q => q.Email, filter.Email);
                if (filter.Address != null)
                    queryable = queryable.Where(q => q.Address, filter.Address);
                if (filter.TaxCode != null)
                    queryable = queryable.Where(q => q.TaxCode, filter.TaxCode);
                if (filter.CompanyName != null)
                    queryable = queryable.Where(q => q.CompanyName, filter.CompanyName);
                if (filter.DeputyName != null)
                    queryable = queryable.Where(q => q.DeputyName, filter.DeputyName);
                if (filter.Description != null)
                    queryable = queryable.Where(q => q.Description, filter.Description);
                if (filter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, filter.StatusId);
                if (filter.ResellerTypeId != null)
                    queryable = queryable.Where(q => q.ResellerTypeId, filter.ResellerTypeId);
                if (filter.ResellerStatusId != null)
                    queryable = queryable.Where(q => q.ResellerStatusId, filter.ResellerStatusId);
                if (filter.StaffId != null)
                    queryable = queryable.Where(q => q.StaffId, filter.StaffId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ResellerDAO> DynamicOrder(IQueryable<ResellerDAO> query, ResellerFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ResellerOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ResellerOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case ResellerOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ResellerOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case ResellerOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case ResellerOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case ResellerOrder.TaxCode:
                            query = query.OrderBy(q => q.TaxCode);
                            break;
                        case ResellerOrder.CompanyName:
                            query = query.OrderBy(q => q.CompanyName);
                            break;
                        case ResellerOrder.DeputyName:
                            query = query.OrderBy(q => q.DeputyName);
                            break;
                        case ResellerOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case ResellerOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case ResellerOrder.ResellerType:
                            query = query.OrderBy(q => q.ResellerTypeId);
                            break;
                        case ResellerOrder.ResellerStatus:
                            query = query.OrderBy(q => q.ResellerStatusId);
                            break;
                        case ResellerOrder.Staff:
                            query = query.OrderBy(q => q.StaffId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ResellerOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ResellerOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case ResellerOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ResellerOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case ResellerOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case ResellerOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case ResellerOrder.TaxCode:
                            query = query.OrderByDescending(q => q.TaxCode);
                            break;
                        case ResellerOrder.CompanyName:
                            query = query.OrderByDescending(q => q.CompanyName);
                            break;
                        case ResellerOrder.DeputyName:
                            query = query.OrderByDescending(q => q.DeputyName);
                            break;
                        case ResellerOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case ResellerOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case ResellerOrder.ResellerType:
                            query = query.OrderByDescending(q => q.ResellerTypeId);
                            break;
                        case ResellerOrder.ResellerStatus:
                            query = query.OrderByDescending(q => q.ResellerStatusId);
                            break;
                        case ResellerOrder.Staff:
                            query = query.OrderByDescending(q => q.StaffId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Reseller>> DynamicSelect(IQueryable<ResellerDAO> query, ResellerFilter filter)
        {
            List<Reseller> Resellers = await query.Select(q => new Reseller()
            {
                Id = filter.Selects.Contains(ResellerSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(ResellerSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(ResellerSelect.Name) ? q.Name : default(string),
                Phone = filter.Selects.Contains(ResellerSelect.Phone) ? q.Phone : default(string),
                Email = filter.Selects.Contains(ResellerSelect.Email) ? q.Email : default(string),
                Address = filter.Selects.Contains(ResellerSelect.Address) ? q.Address : default(string),
                TaxCode = filter.Selects.Contains(ResellerSelect.TaxCode) ? q.TaxCode : default(string),
                CompanyName = filter.Selects.Contains(ResellerSelect.CompanyName) ? q.CompanyName : default(string),
                DeputyName = filter.Selects.Contains(ResellerSelect.DeputyName) ? q.DeputyName : default(string),
                Description = filter.Selects.Contains(ResellerSelect.Description) ? q.Description : default(string),
                StatusId = filter.Selects.Contains(ResellerSelect.Status) ? q.StatusId : default(long),
                ResellerTypeId = filter.Selects.Contains(ResellerSelect.ResellerType) ? q.ResellerTypeId : default(long),
                ResellerStatusId = filter.Selects.Contains(ResellerSelect.ResellerStatus) ? q.ResellerStatusId : default(long),
                StaffId = filter.Selects.Contains(ResellerSelect.Staff) ? q.StaffId : default(long),
                ResellerStatus = filter.Selects.Contains(ResellerSelect.ResellerStatus) && q.ResellerStatus != null ? new ResellerStatus
                {
                    Id = q.ResellerStatus.Id,
                    Code = q.ResellerStatus.Code,
                    Name = q.ResellerStatus.Name,
                } : null,
                ResellerType = filter.Selects.Contains(ResellerSelect.ResellerType) && q.ResellerType != null ? new ResellerType
                {
                    Id = q.ResellerType.Id,
                    Code = q.ResellerType.Code,
                    Name = q.ResellerType.Name,
                    StatusId = q.ResellerType.StatusId,
                } : null,
                Staff = filter.Selects.Contains(ResellerSelect.Staff) && q.Staff != null ? new AppUser
                {
                    Id = q.Staff.Id,
                    Username = q.Staff.Username,
                    Password = q.Staff.Password,
                    DisplayName = q.Staff.DisplayName,
                    SexId = q.Staff.SexId,
                    Address = q.Staff.Address,
                    Email = q.Staff.Email,
                    Phone = q.Staff.Phone,
                    StatusId = q.Staff.StatusId,
                } : null,
                Status = filter.Selects.Contains(ResellerSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
            }).AsNoTracking().ToListAsync();
            return Resellers;
        }

        public async Task<int> Count(ResellerFilter filter)
        {
            IQueryable<ResellerDAO> Resellers = DataContext.Reseller;
            Resellers = DynamicFilter(Resellers, filter);
            return await Resellers.CountAsync();
        }

        public async Task<List<Reseller>> List(ResellerFilter filter)
        {
            if (filter == null) return new List<Reseller>();
            IQueryable<ResellerDAO> ResellerDAOs = DataContext.Reseller;
            ResellerDAOs = DynamicFilter(ResellerDAOs, filter);
            ResellerDAOs = DynamicOrder(ResellerDAOs, filter);
            List<Reseller> Resellers = await DynamicSelect(ResellerDAOs, filter);
            return Resellers;
        }

        public async Task<Reseller> Get(long Id)
        {
            Reseller Reseller = await DataContext.Reseller.Where(x => x.Id == Id).Select(x => new Reseller()
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address,
                TaxCode = x.TaxCode,
                CompanyName = x.CompanyName,
                DeputyName = x.DeputyName,
                Description = x.Description,
                StatusId = x.StatusId,
                ResellerTypeId = x.ResellerTypeId,
                ResellerStatusId = x.ResellerStatusId,
                StaffId = x.StaffId,
                ResellerStatus = x.ResellerStatus == null ? null : new ResellerStatus
                {
                    Id = x.ResellerStatus.Id,
                    Code = x.ResellerStatus.Code,
                    Name = x.ResellerStatus.Name,
                },
                ResellerType = x.ResellerType == null ? null : new ResellerType
                {
                    Id = x.ResellerType.Id,
                    Code = x.ResellerType.Code,
                    Name = x.ResellerType.Name,
                    StatusId = x.ResellerType.StatusId,
                },
                Staff = x.Staff == null ? null : new AppUser
                {
                    Id = x.Staff.Id,
                    Username = x.Staff.Username,
                    Password = x.Staff.Password,
                    DisplayName = x.Staff.DisplayName,
                    SexId = x.Staff.SexId,
                    Address = x.Staff.Address,
                    Email = x.Staff.Email,
                    Phone = x.Staff.Phone,
                    StatusId = x.Staff.StatusId,
                },
                Status = x.Status == null ? null : new Status
                {
                    Id = x.Status.Id,
                    Code = x.Status.Code,
                    Name = x.Status.Name,
                },
            }).AsNoTracking().FirstOrDefaultAsync();

            if (Reseller == null)
                return null;

            return Reseller;
        }
        public async Task<bool> Create(Reseller Reseller)
        {
            ResellerDAO ResellerDAO = new ResellerDAO();
            ResellerDAO.Id = Reseller.Id;
            ResellerDAO.Code = Reseller.Code;
            ResellerDAO.Name = Reseller.Name;
            ResellerDAO.Phone = Reseller.Phone;
            ResellerDAO.Email = Reseller.Email;
            ResellerDAO.Address = Reseller.Address;
            ResellerDAO.TaxCode = Reseller.TaxCode;
            ResellerDAO.CompanyName = Reseller.CompanyName;
            ResellerDAO.DeputyName = Reseller.DeputyName;
            ResellerDAO.Description = Reseller.Description;
            ResellerDAO.StatusId = Reseller.StatusId;
            ResellerDAO.ResellerTypeId = Reseller.ResellerTypeId;
            ResellerDAO.ResellerStatusId = Reseller.ResellerStatusId;
            ResellerDAO.StaffId = Reseller.StaffId;
            ResellerDAO.CreatedAt = StaticParams.DateTimeNow;
            ResellerDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Reseller.Add(ResellerDAO);
            await DataContext.SaveChangesAsync();
            Reseller.Id = ResellerDAO.Id;
            await SaveReference(Reseller);
            return true;
        }

        public async Task<bool> Update(Reseller Reseller)
        {
            ResellerDAO ResellerDAO = DataContext.Reseller.Where(x => x.Id == Reseller.Id).FirstOrDefault();
            if (ResellerDAO == null)
                return false;
            ResellerDAO.Id = Reseller.Id;
            ResellerDAO.Code = Reseller.Code;
            ResellerDAO.Name = Reseller.Name;
            ResellerDAO.Phone = Reseller.Phone;
            ResellerDAO.Email = Reseller.Email;
            ResellerDAO.Address = Reseller.Address;
            ResellerDAO.TaxCode = Reseller.TaxCode;
            ResellerDAO.CompanyName = Reseller.CompanyName;
            ResellerDAO.DeputyName = Reseller.DeputyName;
            ResellerDAO.Description = Reseller.Description;
            ResellerDAO.StatusId = Reseller.StatusId;
            ResellerDAO.ResellerTypeId = Reseller.ResellerTypeId;
            ResellerDAO.ResellerStatusId = Reseller.ResellerStatusId;
            ResellerDAO.StaffId = Reseller.StaffId;
            ResellerDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Reseller);
            return true;
        }

        public async Task<bool> Delete(Reseller Reseller)
        {
            await DataContext.Reseller.Where(x => x.Id == Reseller.Id).UpdateFromQueryAsync(x => new ResellerDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Reseller> Resellers)
        {
            List<ResellerDAO> ResellerDAOs = new List<ResellerDAO>();
            foreach (Reseller Reseller in Resellers)
            {
                ResellerDAO ResellerDAO = new ResellerDAO();
                ResellerDAO.Id = Reseller.Id;
                ResellerDAO.Code = Reseller.Code;
                ResellerDAO.Name = Reseller.Name;
                ResellerDAO.Phone = Reseller.Phone;
                ResellerDAO.Email = Reseller.Email;
                ResellerDAO.Address = Reseller.Address;
                ResellerDAO.TaxCode = Reseller.TaxCode;
                ResellerDAO.CompanyName = Reseller.CompanyName;
                ResellerDAO.DeputyName = Reseller.DeputyName;
                ResellerDAO.Description = Reseller.Description;
                ResellerDAO.StatusId = Reseller.StatusId;
                ResellerDAO.ResellerTypeId = Reseller.ResellerTypeId;
                ResellerDAO.ResellerStatusId = Reseller.ResellerStatusId;
                ResellerDAO.StaffId = Reseller.StaffId;
                ResellerDAO.CreatedAt = StaticParams.DateTimeNow;
                ResellerDAO.UpdatedAt = StaticParams.DateTimeNow;
                ResellerDAOs.Add(ResellerDAO);
            }
            await DataContext.BulkMergeAsync(ResellerDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Reseller> Resellers)
        {
            List<long> Ids = Resellers.Select(x => x.Id).ToList();
            await DataContext.Reseller
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ResellerDAO { DeletedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Reseller Reseller)
        {
        }
        
    }
}
