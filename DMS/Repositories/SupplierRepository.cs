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
    public interface ISupplierRepository
    {
        Task<int> Count(SupplierFilter SupplierFilter);
        Task<List<Supplier>> List(SupplierFilter SupplierFilter);
        Task<Supplier> Get(long Id);
    }
    public class SupplierRepository : ISupplierRepository
    {
        private DataContext DataContext;
        public SupplierRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<SupplierDAO> DynamicFilter(IQueryable<SupplierDAO> query, SupplierFilter filter)
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
            if (filter.TaxCode != null)
                query = query.Where(q => q.TaxCode, filter.TaxCode);
            if (filter.Phone != null)
                query = query.Where(q => q.Phone, filter.Phone);
            if (filter.Email != null)
                query = query.Where(q => q.Email, filter.Email);
            if (filter.Address != null)
                query = query.Where(q => q.Address, filter.Address);
            if (filter.ProvinceId != null)
                query = query.Where(q => q.ProvinceId, filter.ProvinceId);
            if (filter.DistrictId != null)
                query = query.Where(q => q.DistrictId, filter.DistrictId);
            if (filter.WardId != null)
                query = query.Where(q => q.WardId, filter.WardId);
            if (filter.OwnerName != null)
                query = query.Where(q => q.OwnerName, filter.OwnerName);
            if (filter.PersonInChargeId != null)
                query = query.Where(q => q.PersonInChargeId, filter.PersonInChargeId);
            if (filter.Description != null)
                query = query.Where(q => q.Description, filter.Description);
            if (filter.StatusId != null)
                query = query.Where(q => q.StatusId, filter.StatusId);
            if (filter.UpdatedTime != null)
                query = query.Where(q => q.UpdatedAt, filter.UpdatedTime);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<SupplierDAO> OrFilter(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<SupplierDAO> initQuery = query.Where(q => false);
            foreach (SupplierFilter SupplierFilter in filter.OrFilter)
            {
                IQueryable<SupplierDAO> queryable = query;
                if (SupplierFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, SupplierFilter.Id);
                if (SupplierFilter.Code != null)
                    queryable = queryable.Where(q => q.Code, SupplierFilter.Code);
                if (SupplierFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, SupplierFilter.Name);
                if (SupplierFilter.TaxCode != null)
                    queryable = queryable.Where(q => q.TaxCode, SupplierFilter.TaxCode);
                if (SupplierFilter.Phone != null)
                    queryable = queryable.Where(q => q.Phone, SupplierFilter.Phone);
                if (SupplierFilter.Email != null)
                    queryable = queryable.Where(q => q.Email, SupplierFilter.Email);
                if (SupplierFilter.Address != null)
                    queryable = queryable.Where(q => q.Address, SupplierFilter.Address);
                if (SupplierFilter.ProvinceId != null)
                    queryable = queryable.Where(q => q.ProvinceId, SupplierFilter.ProvinceId);
                if (SupplierFilter.DistrictId != null)
                    queryable = queryable.Where(q => q.DistrictId, SupplierFilter.DistrictId);
                if (SupplierFilter.WardId != null)
                    queryable = queryable.Where(q => q.WardId, SupplierFilter.WardId);
                if (SupplierFilter.OwnerName != null)
                    queryable = queryable.Where(q => q.OwnerName, SupplierFilter.OwnerName);
                if (SupplierFilter.PersonInChargeId != null)
                    queryable = queryable.Where(q => q.PersonInChargeId, SupplierFilter.PersonInChargeId);
                if (SupplierFilter.Description != null)
                    queryable = queryable.Where(q => q.Description, SupplierFilter.Description);
                if (SupplierFilter.StatusId != null)
                    queryable = queryable.Where(q => q.StatusId, SupplierFilter.StatusId);
                if (SupplierFilter.UpdatedTime != null)
                    queryable = queryable.Where(q => q.UpdatedAt, SupplierFilter.UpdatedTime);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }

        private IQueryable<SupplierDAO> DynamicOrder(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case SupplierOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case SupplierOrder.Code:
                            query = query.OrderBy(q => q.Code);
                            break;
                        case SupplierOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case SupplierOrder.TaxCode:
                            query = query.OrderBy(q => q.TaxCode);
                            break;
                        case SupplierOrder.Phone:
                            query = query.OrderBy(q => q.Phone);
                            break;
                        case SupplierOrder.Email:
                            query = query.OrderBy(q => q.Email);
                            break;
                        case SupplierOrder.Address:
                            query = query.OrderBy(q => q.Address);
                            break;
                        case SupplierOrder.Province:
                            query = query.OrderBy(q => q.ProvinceId);
                            break;
                        case SupplierOrder.District:
                            query = query.OrderBy(q => q.DistrictId);
                            break;
                        case SupplierOrder.Ward:
                            query = query.OrderBy(q => q.WardId);
                            break;
                        case SupplierOrder.OwnerName:
                            query = query.OrderBy(q => q.OwnerName);
                            break;
                        case SupplierOrder.PersonInCharge:
                            query = query.OrderBy(q => q.PersonInChargeId);
                            break;
                        case SupplierOrder.Description:
                            query = query.OrderBy(q => q.Description);
                            break;
                        case SupplierOrder.Status:
                            query = query.OrderBy(q => q.StatusId);
                            break;
                        case SupplierOrder.UpdatedTime:
                            query = query.OrderBy(q => q.UpdatedAt);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case SupplierOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case SupplierOrder.Code:
                            query = query.OrderByDescending(q => q.Code);
                            break;
                        case SupplierOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case SupplierOrder.TaxCode:
                            query = query.OrderByDescending(q => q.TaxCode);
                            break;
                        case SupplierOrder.Phone:
                            query = query.OrderByDescending(q => q.Phone);
                            break;
                        case SupplierOrder.Email:
                            query = query.OrderByDescending(q => q.Email);
                            break;
                        case SupplierOrder.Address:
                            query = query.OrderByDescending(q => q.Address);
                            break;
                        case SupplierOrder.Province:
                            query = query.OrderByDescending(q => q.ProvinceId);
                            break;
                        case SupplierOrder.District:
                            query = query.OrderByDescending(q => q.DistrictId);
                            break;
                        case SupplierOrder.Ward:
                            query = query.OrderByDescending(q => q.WardId);
                            break;
                        case SupplierOrder.OwnerName:
                            query = query.OrderByDescending(q => q.OwnerName);
                            break;
                        case SupplierOrder.PersonInCharge:
                            query = query.OrderByDescending(q => q.PersonInChargeId);
                            break;
                        case SupplierOrder.Description:
                            query = query.OrderByDescending(q => q.Description);
                            break;
                        case SupplierOrder.Status:
                            query = query.OrderByDescending(q => q.StatusId);
                            break;
                        case SupplierOrder.UpdatedTime:
                            query = query.OrderByDescending(q => q.UpdatedAt);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Supplier>> DynamicSelect(IQueryable<SupplierDAO> query, SupplierFilter filter)
        {
            List<Supplier> Suppliers = await query.Select(q => new Supplier()
            {
                Id = filter.Selects.Contains(SupplierSelect.Id) ? q.Id : default(long),
                Code = filter.Selects.Contains(SupplierSelect.Code) ? q.Code : default(string),
                Name = filter.Selects.Contains(SupplierSelect.Name) ? q.Name : default(string),
                TaxCode = filter.Selects.Contains(SupplierSelect.TaxCode) ? q.TaxCode : default(string),
                Phone = filter.Selects.Contains(SupplierSelect.Phone) ? q.Phone : default(string),
                Email = filter.Selects.Contains(SupplierSelect.Email) ? q.Email : default(string),
                Address = filter.Selects.Contains(SupplierSelect.Address) ? q.Address : default(string),
                ProvinceId = filter.Selects.Contains(SupplierSelect.Province) ? q.ProvinceId : default(long),
                DistrictId = filter.Selects.Contains(SupplierSelect.District) ? q.DistrictId : default(long),
                WardId = filter.Selects.Contains(SupplierSelect.Ward) ? q.WardId : default(long),
                PersonInChargeId = filter.Selects.Contains(SupplierSelect.PersonInCharge) ? q.PersonInChargeId : default(long),
                OwnerName = filter.Selects.Contains(SupplierSelect.OwnerName) ? q.OwnerName : default(string),
                Description = filter.Selects.Contains(SupplierSelect.Description) ? q.Description : default(string),
                StatusId = filter.Selects.Contains(SupplierSelect.Status) ? q.StatusId : default(long),
                UpdatedTime = filter.Selects.Contains(SupplierSelect.UpdatedTime) ? q.UpdatedAt : default(DateTime),
                District = filter.Selects.Contains(SupplierSelect.District) && q.District != null ? new District
                {
                    Id = q.District.Id,
                    Name = q.District.Name,
                    Priority = q.District.Priority,
                    ProvinceId = q.District.ProvinceId,
                    StatusId = q.District.StatusId,
                } : null,
                PersonInCharge = filter.Selects.Contains(SupplierSelect.PersonInCharge) && q.PersonInCharge != null ? new AppUser
                {
                    Id = q.PersonInCharge.Id,
                    DisplayName = q.PersonInCharge.DisplayName,
                    Address = q.PersonInCharge.Address,
                    Phone = q.PersonInCharge.Phone,
                    Email = q.Email,
                    SexId = q.PersonInCharge.SexId,
                    StatusId = q.StatusId
                } : null,
                Province = filter.Selects.Contains(SupplierSelect.Province) && q.Province != null ? new Province
                {
                    Id = q.Province.Id,
                    Name = q.Province.Name,
                    Priority = q.Province.Priority,
                    StatusId = q.Province.StatusId,
                } : null,
                Status = filter.Selects.Contains(SupplierSelect.Status) && q.Status != null ? new Status
                {
                    Id = q.Status.Id,
                    Code = q.Status.Code,
                    Name = q.Status.Name,
                } : null,
                Ward = filter.Selects.Contains(SupplierSelect.Ward) && q.Ward != null ? new Ward
                {
                    Id = q.Ward.Id,
                    Name = q.Ward.Name,
                    Priority = q.Ward.Priority,
                    DistrictId = q.Ward.DistrictId,
                    StatusId = q.Ward.StatusId,
                } : null,
                Used = q.Used,
            }).ToListAsync();
            return Suppliers;
        }

        public async Task<int> Count(SupplierFilter filter)
        {
            IQueryable<SupplierDAO> Suppliers = DataContext.Supplier;
            Suppliers = DynamicFilter(Suppliers, filter);
            return await Suppliers.CountAsync();
        }

        public async Task<List<Supplier>> List(SupplierFilter filter)
        {
            if (filter == null) return new List<Supplier>();
            IQueryable<SupplierDAO> SupplierDAOs = DataContext.Supplier.AsNoTracking();
            SupplierDAOs = DynamicFilter(SupplierDAOs, filter);
            SupplierDAOs = DynamicOrder(SupplierDAOs, filter);
            List<Supplier> Suppliers = await DynamicSelect(SupplierDAOs, filter);
            return Suppliers;
        }

        public async Task<Supplier> Get(long Id)
        {
            Supplier Supplier = await DataContext.Supplier.AsNoTracking()
                .Where(x => x.Id == Id).Select(x => new Supplier()
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    TaxCode = x.TaxCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    Address = x.Address,
                    ProvinceId = x.ProvinceId,
                    DistrictId = x.DistrictId,
                    WardId = x.WardId,
                    OwnerName = x.OwnerName,
                    PersonInChargeId = x.PersonInChargeId,
                    Description = x.Description,
                    StatusId = x.StatusId,
                    UpdatedTime = x.UpdatedAt,
                    Used = x.Used,
                    District = x.District == null ? null : new District
                    {
                        Id = x.District.Id,
                        Name = x.District.Name,
                        Priority = x.District.Priority,
                        ProvinceId = x.District.ProvinceId,
                        StatusId = x.District.StatusId,
                    },
                    PersonInCharge = x.PersonInCharge == null ? null : new AppUser
                    {
                        Id = x.PersonInCharge.Id,
                        DisplayName = x.PersonInCharge.DisplayName,
                        Address = x.PersonInCharge.Address,
                        Phone = x.PersonInCharge.Phone,
                        Email = x.Email,
                        SexId = x.PersonInCharge.SexId,
                        StatusId = x.StatusId
                    },
                    Province = x.Province == null ? null : new Province
                    {
                        Id = x.Province.Id,
                        Name = x.Province.Name,
                        Priority = x.Province.Priority,
                        StatusId = x.Province.StatusId,
                    },
                    Status = x.Status == null ? null : new Status
                    {
                        Id = x.Status.Id,
                        Code = x.Status.Code,
                        Name = x.Status.Name,
                    },
                    Ward = x.Ward == null ? null : new Ward
                    {
                        Id = x.Ward.Id,
                        Name = x.Ward.Name,
                        Priority = x.Ward.Priority,
                        DistrictId = x.Ward.DistrictId,
                        StatusId = x.Ward.StatusId,
                    },
                }).FirstOrDefaultAsync();

            if (Supplier == null)
                return null;

            return Supplier;
        }
    }
}
