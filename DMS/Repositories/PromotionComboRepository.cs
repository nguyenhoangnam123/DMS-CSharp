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
    public interface IPromotionComboRepository
    {
        Task<int> Count(PromotionComboFilter PromotionComboFilter);
        Task<List<PromotionCombo>> List(PromotionComboFilter PromotionComboFilter);
        Task<PromotionCombo> Get(long Id);
        Task<bool> Create(PromotionCombo PromotionCombo);
        Task<bool> Update(PromotionCombo PromotionCombo);
        Task<bool> Delete(PromotionCombo PromotionCombo);
        Task<bool> BulkMerge(List<PromotionCombo> PromotionCombos);
        Task<bool> BulkDelete(List<PromotionCombo> PromotionCombos);
    }
    public class PromotionComboRepository : IPromotionComboRepository
    {
        private DataContext DataContext;
        public PromotionComboRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PromotionComboDAO> DynamicFilter(IQueryable<PromotionComboDAO> query, PromotionComboFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Note != null)
                query = query.Where(q => q.Note, filter.Note);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.PromotionId != null)
                query = query.Where(q => q.PromotionId, filter.PromotionId);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<PromotionComboDAO> OrFilter(IQueryable<PromotionComboDAO> query, PromotionComboFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<PromotionComboDAO> initQuery = query.Where(q => false);
            foreach (PromotionComboFilter PromotionComboFilter in filter.OrFilter)
            {
                IQueryable<PromotionComboDAO> queryable = query;
                if (PromotionComboFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, PromotionComboFilter.Id);
                if (PromotionComboFilter.Note != null)
                    queryable = queryable.Where(q => q.Note, PromotionComboFilter.Note);
                if (PromotionComboFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, PromotionComboFilter.Name);
                if (PromotionComboFilter.PromotionId != null)
                    queryable = queryable.Where(q => q.PromotionId, PromotionComboFilter.PromotionId);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<PromotionComboDAO> DynamicOrder(IQueryable<PromotionComboDAO> query, PromotionComboFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case PromotionComboOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case PromotionComboOrder.Note:
                            query = query.OrderBy(q => q.Note);
                            break;
                        case PromotionComboOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case PromotionComboOrder.Promotion:
                            query = query.OrderBy(q => q.PromotionId);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case PromotionComboOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case PromotionComboOrder.Note:
                            query = query.OrderByDescending(q => q.Note);
                            break;
                        case PromotionComboOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case PromotionComboOrder.Promotion:
                            query = query.OrderByDescending(q => q.PromotionId);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<PromotionCombo>> DynamicSelect(IQueryable<PromotionComboDAO> query, PromotionComboFilter filter)
        {
            List<PromotionCombo> PromotionCombos = await query.Select(q => new PromotionCombo()
            {
                Id = filter.Selects.Contains(PromotionComboSelect.Id) ? q.Id : default(long),
                Note = filter.Selects.Contains(PromotionComboSelect.Note) ? q.Note : default(string),
                Name = filter.Selects.Contains(PromotionComboSelect.Name) ? q.Name : default(string),
                PromotionId = filter.Selects.Contains(PromotionComboSelect.Promotion) ? q.PromotionId : default(long),
                Promotion = filter.Selects.Contains(PromotionComboSelect.Promotion) && q.Promotion != null ? new Promotion
                {
                    Id = q.Promotion.Id,
                    Code = q.Promotion.Code,
                    Name = q.Promotion.Name,
                    StartDate = q.Promotion.StartDate,
                    EndDate = q.Promotion.EndDate,
                    OrganizationId = q.Promotion.OrganizationId,
                    PromotionTypeId = q.Promotion.PromotionTypeId,
                    Note = q.Promotion.Note,
                    Priority = q.Promotion.Priority,
                    StatusId = q.Promotion.StatusId,
                } : null,
            }).ToListAsync();
            return PromotionCombos;
        }

        public async Task<int> Count(PromotionComboFilter filter)
        {
            IQueryable<PromotionComboDAO> PromotionCombos = DataContext.PromotionCombo.AsNoTracking();
            PromotionCombos = DynamicFilter(PromotionCombos, filter);
            return await PromotionCombos.CountAsync();
        }

        public async Task<List<PromotionCombo>> List(PromotionComboFilter filter)
        {
            if (filter == null) return new List<PromotionCombo>();
            IQueryable<PromotionComboDAO> PromotionComboDAOs = DataContext.PromotionCombo.AsNoTracking();
            PromotionComboDAOs = DynamicFilter(PromotionComboDAOs, filter);
            PromotionComboDAOs = DynamicOrder(PromotionComboDAOs, filter);
            List<PromotionCombo> PromotionCombos = await DynamicSelect(PromotionComboDAOs, filter);
            return PromotionCombos;
        }

        public async Task<PromotionCombo> Get(long Id)
        {
            PromotionCombo PromotionCombo = await DataContext.PromotionCombo.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new PromotionCombo()
            {
                Id = x.Id,
                Note = x.Note,
                Name = x.Name,
                PromotionId = x.PromotionId,
                Promotion = x.Promotion == null ? null : new Promotion
                {
                    Id = x.Promotion.Id,
                    Code = x.Promotion.Code,
                    Name = x.Promotion.Name,
                    StartDate = x.Promotion.StartDate,
                    EndDate = x.Promotion.EndDate,
                    OrganizationId = x.Promotion.OrganizationId,
                    PromotionTypeId = x.Promotion.PromotionTypeId,
                    Note = x.Promotion.Note,
                    Priority = x.Promotion.Priority,
                    StatusId = x.Promotion.StatusId,
                },
            }).FirstOrDefaultAsync();

            if (PromotionCombo == null)
                return null;
            PromotionCombo.Combos = await DataContext.Combo.AsNoTracking()
                .Where(x => x.PromotionComboId == PromotionCombo.Id)
                .Where(x => x.DeletedAt == null)
                .Select(x => new Combo
                {
                    Id = x.Id,
                    PromotionComboId = x.PromotionComboId,
                    Name = x.Name,
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

            return PromotionCombo;
        }
        public async Task<bool> Create(PromotionCombo PromotionCombo)
        {
            PromotionComboDAO PromotionComboDAO = new PromotionComboDAO();
            PromotionComboDAO.Id = PromotionCombo.Id;
            PromotionComboDAO.Note = PromotionCombo.Note;
            PromotionComboDAO.Name = PromotionCombo.Name;
            PromotionComboDAO.PromotionId = PromotionCombo.PromotionId;
            DataContext.PromotionCombo.Add(PromotionComboDAO);
            await DataContext.SaveChangesAsync();
            PromotionCombo.Id = PromotionComboDAO.Id;
            await SaveReference(PromotionCombo);
            return true;
        }

        public async Task<bool> Update(PromotionCombo PromotionCombo)
        {
            PromotionComboDAO PromotionComboDAO = DataContext.PromotionCombo.Where(x => x.Id == PromotionCombo.Id).FirstOrDefault();
            if (PromotionComboDAO == null)
                return false;
            PromotionComboDAO.Id = PromotionCombo.Id;
            PromotionComboDAO.Note = PromotionCombo.Note;
            PromotionComboDAO.Name = PromotionCombo.Name;
            PromotionComboDAO.PromotionId = PromotionCombo.PromotionId;
            await DataContext.SaveChangesAsync();
            await SaveReference(PromotionCombo);
            return true;
        }

        public async Task<bool> Delete(PromotionCombo PromotionCombo)
        {
            await DataContext.PromotionCombo.Where(x => x.Id == PromotionCombo.Id).DeleteFromQueryAsync();
            return true;
        }
        
        public async Task<bool> BulkMerge(List<PromotionCombo> PromotionCombos)
        {
            List<PromotionComboDAO> PromotionComboDAOs = new List<PromotionComboDAO>();
            foreach (PromotionCombo PromotionCombo in PromotionCombos)
            {
                PromotionComboDAO PromotionComboDAO = new PromotionComboDAO();
                PromotionComboDAO.Id = PromotionCombo.Id;
                PromotionComboDAO.Note = PromotionCombo.Note;
                PromotionComboDAO.Name = PromotionCombo.Name;
                PromotionComboDAO.PromotionId = PromotionCombo.PromotionId;
                PromotionComboDAOs.Add(PromotionComboDAO);
            }
            await DataContext.BulkMergeAsync(PromotionComboDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<PromotionCombo> PromotionCombos)
        {
            List<long> Ids = PromotionCombos.Select(x => x.Id).ToList();
            await DataContext.PromotionCombo
                .Where(x => Ids.Contains(x.Id)).DeleteFromQueryAsync();
            return true;
        }

        private async Task SaveReference(PromotionCombo PromotionCombo)
        {
            List<ComboDAO> ComboDAOs = await DataContext.Combo
                .Where(x => x.PromotionComboId == PromotionCombo.Id).ToListAsync();
            ComboDAOs.ForEach(x => x.DeletedAt = StaticParams.DateTimeNow);
            if (PromotionCombo.Combos != null)
            {
                foreach (Combo Combo in PromotionCombo.Combos)
                {
                    ComboDAO ComboDAO = ComboDAOs
                        .Where(x => x.Id == Combo.Id && x.Id != 0).FirstOrDefault();
                    if (ComboDAO == null)
                    {
                        ComboDAO = new ComboDAO();
                        ComboDAO.Id = Combo.Id;
                        ComboDAO.PromotionComboId = PromotionCombo.Id;
                        ComboDAO.Name = Combo.Name;
                        ComboDAO.PromotionDiscountTypeId = Combo.PromotionDiscountTypeId;
                        ComboDAO.DiscountPercentage = Combo.DiscountPercentage;
                        ComboDAO.DiscountValue = Combo.DiscountValue;
                        ComboDAOs.Add(ComboDAO);
                        ComboDAO.CreatedAt = StaticParams.DateTimeNow;
                        ComboDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ComboDAO.DeletedAt = null;
                    }
                    else
                    {
                        ComboDAO.Id = Combo.Id;
                        ComboDAO.PromotionComboId = PromotionCombo.Id;
                        ComboDAO.Name = Combo.Name;
                        ComboDAO.PromotionDiscountTypeId = Combo.PromotionDiscountTypeId;
                        ComboDAO.DiscountPercentage = Combo.DiscountPercentage;
                        ComboDAO.DiscountValue = Combo.DiscountValue;
                        ComboDAO.UpdatedAt = StaticParams.DateTimeNow;
                        ComboDAO.DeletedAt = null;
                    }
                }
                await DataContext.Combo.BulkMergeAsync(ComboDAOs);
            }
        }
        
    }
}
