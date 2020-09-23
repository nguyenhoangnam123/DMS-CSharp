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
    public interface IComboRepository
    {
        Task<int> Count(ComboFilter ComboFilter);
        Task<List<Combo>> List(ComboFilter ComboFilter);
        Task<Combo> Get(long Id);
        Task<bool> Create(Combo Combo);
        Task<bool> Update(Combo Combo);
        Task<bool> Delete(Combo Combo);
        Task<bool> BulkMerge(List<Combo> Combos);
        Task<bool> BulkDelete(List<Combo> Combos);
    }
    public class ComboRepository : IComboRepository
    {
        private DataContext DataContext;
        public ComboRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<ComboDAO> DynamicFilter(IQueryable<ComboDAO> query, ComboFilter filter)
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
            if (filter.PromotionComboId != null)
                query = query.Where(q => q.PromotionComboId, filter.PromotionComboId);
            if (filter.Name != null)
                query = query.Where(q => q.Name, filter.Name);
            if (filter.PromotionDiscountTypeId != null)
                query = query.Where(q => q.PromotionDiscountTypeId, filter.PromotionDiscountTypeId);
            if (filter.DiscountPercentage != null)
                query = query.Where(q => q.DiscountPercentage.HasValue).Where(q => q.DiscountPercentage, filter.DiscountPercentage);
            if (filter.DiscountValue != null)
                query = query.Where(q => q.DiscountValue.HasValue).Where(q => q.DiscountValue, filter.DiscountValue);
            query = OrFilter(query, filter);
            return query;
        }

        private IQueryable<ComboDAO> OrFilter(IQueryable<ComboDAO> query, ComboFilter filter)
        {
            if (filter.OrFilter == null || filter.OrFilter.Count == 0)
                return query;
            IQueryable<ComboDAO> initQuery = query.Where(q => false);
            foreach (ComboFilter ComboFilter in filter.OrFilter)
            {
                IQueryable<ComboDAO> queryable = query;
                if (ComboFilter.Id != null)
                    queryable = queryable.Where(q => q.Id, ComboFilter.Id);
                if (ComboFilter.PromotionComboId != null)
                    queryable = queryable.Where(q => q.PromotionComboId, ComboFilter.PromotionComboId);
                if (ComboFilter.Name != null)
                    queryable = queryable.Where(q => q.Name, ComboFilter.Name);
                if (ComboFilter.PromotionDiscountTypeId != null)
                    queryable = queryable.Where(q => q.PromotionDiscountTypeId, ComboFilter.PromotionDiscountTypeId);
                if (ComboFilter.DiscountPercentage != null)
                    queryable = queryable.Where(q => q.DiscountPercentage.HasValue).Where(q => q.DiscountPercentage, ComboFilter.DiscountPercentage);
                if (ComboFilter.DiscountValue != null)
                    queryable = queryable.Where(q => q.DiscountValue.HasValue).Where(q => q.DiscountValue, ComboFilter.DiscountValue);
                initQuery = initQuery.Union(queryable);
            }
            return initQuery;
        }    

        private IQueryable<ComboDAO> DynamicOrder(IQueryable<ComboDAO> query, ComboFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case ComboOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case ComboOrder.PromotionCombo:
                            query = query.OrderBy(q => q.PromotionComboId);
                            break;
                        case ComboOrder.Name:
                            query = query.OrderBy(q => q.Name);
                            break;
                        case ComboOrder.PromotionDiscountType:
                            query = query.OrderBy(q => q.PromotionDiscountTypeId);
                            break;
                        case ComboOrder.DiscountPercentage:
                            query = query.OrderBy(q => q.DiscountPercentage);
                            break;
                        case ComboOrder.DiscountValue:
                            query = query.OrderBy(q => q.DiscountValue);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case ComboOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case ComboOrder.PromotionCombo:
                            query = query.OrderByDescending(q => q.PromotionComboId);
                            break;
                        case ComboOrder.Name:
                            query = query.OrderByDescending(q => q.Name);
                            break;
                        case ComboOrder.PromotionDiscountType:
                            query = query.OrderByDescending(q => q.PromotionDiscountTypeId);
                            break;
                        case ComboOrder.DiscountPercentage:
                            query = query.OrderByDescending(q => q.DiscountPercentage);
                            break;
                        case ComboOrder.DiscountValue:
                            query = query.OrderByDescending(q => q.DiscountValue);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<Combo>> DynamicSelect(IQueryable<ComboDAO> query, ComboFilter filter)
        {
            List<Combo> Combos = await query.Select(q => new Combo()
            {
                Id = filter.Selects.Contains(ComboSelect.Id) ? q.Id : default(long),
                PromotionComboId = filter.Selects.Contains(ComboSelect.PromotionCombo) ? q.PromotionComboId : default(long),
                Name = filter.Selects.Contains(ComboSelect.Name) ? q.Name : default(string),
                PromotionDiscountTypeId = filter.Selects.Contains(ComboSelect.PromotionDiscountType) ? q.PromotionDiscountTypeId : default(long),
                DiscountPercentage = filter.Selects.Contains(ComboSelect.DiscountPercentage) ? q.DiscountPercentage : default(decimal?),
                DiscountValue = filter.Selects.Contains(ComboSelect.DiscountValue) ? q.DiscountValue : default(decimal?),
                PromotionCombo = filter.Selects.Contains(ComboSelect.PromotionCombo) && q.PromotionCombo != null ? new PromotionCombo
                {
                    Id = q.PromotionCombo.Id,
                    Note = q.PromotionCombo.Note,
                    PromotionId = q.PromotionCombo.PromotionId,
                } : null,
                PromotionDiscountType = filter.Selects.Contains(ComboSelect.PromotionDiscountType) && q.PromotionDiscountType != null ? new PromotionDiscountType
                {
                    Id = q.PromotionDiscountType.Id,
                    Code = q.PromotionDiscountType.Code,
                    Name = q.PromotionDiscountType.Name,
                } : null,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            }).ToListAsync();
            return Combos;
        }

        public async Task<int> Count(ComboFilter filter)
        {
            IQueryable<ComboDAO> Combos = DataContext.Combo.AsNoTracking();
            Combos = DynamicFilter(Combos, filter);
            return await Combos.CountAsync();
        }

        public async Task<List<Combo>> List(ComboFilter filter)
        {
            if (filter == null) return new List<Combo>();
            IQueryable<ComboDAO> ComboDAOs = DataContext.Combo.AsNoTracking();
            ComboDAOs = DynamicFilter(ComboDAOs, filter);
            ComboDAOs = DynamicOrder(ComboDAOs, filter);
            List<Combo> Combos = await DynamicSelect(ComboDAOs, filter);
            return Combos;
        }

        public async Task<Combo> Get(long Id)
        {
            Combo Combo = await DataContext.Combo.AsNoTracking()
            .Where(x => x.Id == Id).Select(x => new Combo()
            {
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Id = x.Id,
                PromotionComboId = x.PromotionComboId,
                Name = x.Name,
                PromotionDiscountTypeId = x.PromotionDiscountTypeId,
                DiscountPercentage = x.DiscountPercentage,
                DiscountValue = x.DiscountValue,
                PromotionCombo = x.PromotionCombo == null ? null : new PromotionCombo
                {
                    Id = x.PromotionCombo.Id,
                    Note = x.PromotionCombo.Note,
                    PromotionId = x.PromotionCombo.PromotionId,
                },
                PromotionDiscountType = x.PromotionDiscountType == null ? null : new PromotionDiscountType
                {
                    Id = x.PromotionDiscountType.Id,
                    Code = x.PromotionDiscountType.Code,
                    Name = x.PromotionDiscountType.Name,
                },
            }).FirstOrDefaultAsync();

            if (Combo == null)
                return null;
            Combo.ComboInItemMappings = await DataContext.ComboInItemMapping.AsNoTracking()
                .Where(x => x.ComboId == Combo.Id)
                .Where(x => x.Item.DeletedAt == null)
                .Select(x => new ComboInItemMapping
                {
                    ComboId = x.ComboId,
                    ItemId = x.ItemId,
                    From = x.From,
                    To = x.To,
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
                        Used = x.Item.Used,
                    },
                }).ToListAsync();
            Combo.ComboOutItemMappings = await DataContext.ComboOutItemMapping.AsNoTracking()
                .Where(x => x.ComboId == Combo.Id)
                .Where(x => x.Item.DeletedAt == null)
                .Select(x => new ComboOutItemMapping
                {
                    ComboId = x.ComboId,
                    ItemId = x.ItemId,
                    Quantity = x.Quantity,
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
                        Used = x.Item.Used,
                    },
                }).ToListAsync();

            return Combo;
        }
        public async Task<bool> Create(Combo Combo)
        {
            ComboDAO ComboDAO = new ComboDAO();
            ComboDAO.Id = Combo.Id;
            ComboDAO.PromotionComboId = Combo.PromotionComboId;
            ComboDAO.Name = Combo.Name;
            ComboDAO.PromotionDiscountTypeId = Combo.PromotionDiscountTypeId;
            ComboDAO.DiscountPercentage = Combo.DiscountPercentage;
            ComboDAO.DiscountValue = Combo.DiscountValue;
            ComboDAO.CreatedAt = StaticParams.DateTimeNow;
            ComboDAO.UpdatedAt = StaticParams.DateTimeNow;
            DataContext.Combo.Add(ComboDAO);
            await DataContext.SaveChangesAsync();
            Combo.Id = ComboDAO.Id;
            await SaveReference(Combo);
            return true;
        }

        public async Task<bool> Update(Combo Combo)
        {
            ComboDAO ComboDAO = DataContext.Combo.Where(x => x.Id == Combo.Id).FirstOrDefault();
            if (ComboDAO == null)
                return false;
            ComboDAO.Id = Combo.Id;
            ComboDAO.PromotionComboId = Combo.PromotionComboId;
            ComboDAO.Name = Combo.Name;
            ComboDAO.PromotionDiscountTypeId = Combo.PromotionDiscountTypeId;
            ComboDAO.DiscountPercentage = Combo.DiscountPercentage;
            ComboDAO.DiscountValue = Combo.DiscountValue;
            ComboDAO.UpdatedAt = StaticParams.DateTimeNow;
            await DataContext.SaveChangesAsync();
            await SaveReference(Combo);
            return true;
        }

        public async Task<bool> Delete(Combo Combo)
        {
            await DataContext.Combo.Where(x => x.Id == Combo.Id).UpdateFromQueryAsync(x => new ComboDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }
        
        public async Task<bool> BulkMerge(List<Combo> Combos)
        {
            List<ComboDAO> ComboDAOs = new List<ComboDAO>();
            foreach (Combo Combo in Combos)
            {
                ComboDAO ComboDAO = new ComboDAO();
                ComboDAO.Id = Combo.Id;
                ComboDAO.PromotionComboId = Combo.PromotionComboId;
                ComboDAO.Name = Combo.Name;
                ComboDAO.PromotionDiscountTypeId = Combo.PromotionDiscountTypeId;
                ComboDAO.DiscountPercentage = Combo.DiscountPercentage;
                ComboDAO.DiscountValue = Combo.DiscountValue;
                ComboDAO.CreatedAt = StaticParams.DateTimeNow;
                ComboDAO.UpdatedAt = StaticParams.DateTimeNow;
                ComboDAOs.Add(ComboDAO);
            }
            await DataContext.BulkMergeAsync(ComboDAOs);
            return true;
        }

        public async Task<bool> BulkDelete(List<Combo> Combos)
        {
            List<long> Ids = Combos.Select(x => x.Id).ToList();
            await DataContext.Combo
                .Where(x => Ids.Contains(x.Id))
                .UpdateFromQueryAsync(x => new ComboDAO { DeletedAt = StaticParams.DateTimeNow, UpdatedAt = StaticParams.DateTimeNow });
            return true;
        }

        private async Task SaveReference(Combo Combo)
        {
            await DataContext.ComboInItemMapping
                .Where(x => x.ComboId == Combo.Id)
                .DeleteFromQueryAsync();
            List<ComboInItemMappingDAO> ComboInItemMappingDAOs = new List<ComboInItemMappingDAO>();
            if (Combo.ComboInItemMappings != null)
            {
                foreach (ComboInItemMapping ComboInItemMapping in Combo.ComboInItemMappings)
                {
                    ComboInItemMappingDAO ComboInItemMappingDAO = new ComboInItemMappingDAO();
                    ComboInItemMappingDAO.ComboId = Combo.Id;
                    ComboInItemMappingDAO.ItemId = ComboInItemMapping.ItemId;
                    ComboInItemMappingDAO.From = ComboInItemMapping.From;
                    ComboInItemMappingDAO.To = ComboInItemMapping.To;
                    ComboInItemMappingDAOs.Add(ComboInItemMappingDAO);
                }
                await DataContext.ComboInItemMapping.BulkMergeAsync(ComboInItemMappingDAOs);
            }
            await DataContext.ComboOutItemMapping
                .Where(x => x.ComboId == Combo.Id)
                .DeleteFromQueryAsync();
            List<ComboOutItemMappingDAO> ComboOutItemMappingDAOs = new List<ComboOutItemMappingDAO>();
            if (Combo.ComboOutItemMappings != null)
            {
                foreach (ComboOutItemMapping ComboOutItemMapping in Combo.ComboOutItemMappings)
                {
                    ComboOutItemMappingDAO ComboOutItemMappingDAO = new ComboOutItemMappingDAO();
                    ComboOutItemMappingDAO.ComboId = Combo.Id;
                    ComboOutItemMappingDAO.ItemId = ComboOutItemMapping.ItemId;
                    ComboOutItemMappingDAO.Quantity = ComboOutItemMapping.Quantity;
                    ComboOutItemMappingDAOs.Add(ComboOutItemMappingDAO);
                }
                await DataContext.ComboOutItemMapping.BulkMergeAsync(ComboOutItemMappingDAOs);
            }
        }
        
    }
}
