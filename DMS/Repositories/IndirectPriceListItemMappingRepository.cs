using DMS.Entities;
using DMS.Models;
using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DMS.Repositories
{
    public interface IIndirectPriceListItemMappingItemMappingRepository
    {
        Task<int> Count(IndirectPriceListItemMappingFilter IndirectPriceListItemMappingFilter);
        Task<List<IndirectPriceListItemMapping>> List(IndirectPriceListItemMappingFilter IndirectPriceListItemMappingFilter);
    }
    public class IndirectPriceListItemMappingItemMappingRepository : IIndirectPriceListItemMappingItemMappingRepository
    {
        private readonly DataContext DataContext;

        public IndirectPriceListItemMappingItemMappingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<IndirectPriceListItemMappingDAO> DynamicFilter(IQueryable<IndirectPriceListItemMappingDAO> query, IndirectPriceListItemMappingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.IndirectPriceListId != null)
                query = query.Where(q => q.IndirectPriceListId, filter.IndirectPriceListId);
            if (filter.ItemId != null)
                query = query.Where(q => q.ItemId, filter.ItemId);
            if (filter.Price != null)
                query = query.Where(q => q.Price, filter.Price);
            if (filter.IndirectPriceListTypeId != null)
            {
                query = query.Where(q => q.IndirectPriceList.IndirectPriceListTypeId, filter.IndirectPriceListTypeId);
            }
            if (filter.StoreGroupingId != null)
            {
                if (filter.StoreGroupingId.Equal.HasValue)
                {
                    query = from q in query
                            join sg in DataContext.IndirectPriceListStoreGroupingMapping on q.IndirectPriceListId equals sg.IndirectPriceListId
                            where sg.StoreGroupingId == filter.StoreGroupingId.Equal.Value
                            select q;
                }
            }
            if (filter.StoreTypeId != null)
            {
                if (filter.StoreTypeId.Equal.HasValue)
                {
                    query = from q in query
                            join st in DataContext.IndirectPriceListStoreTypeMapping on q.IndirectPriceListId equals st.IndirectPriceListId
                            where st.StoreTypeId == filter.StoreTypeId.Equal.Value
                            select q;
                }
            }
                
            if (filter.StoreId != null)
            {
                if (filter.StoreId.Equal.HasValue)
                {
                    query = from q in query
                            join s in DataContext.IndirectPriceListStoreMapping on q.IndirectPriceListId equals s.IndirectPriceListId
                            where s.StoreId == filter.StoreId.Equal.Value
                            select q;
                }
            }

            if (filter.OrganizationId != null)
            {
                query = query.Where(q => q.IndirectPriceList.OrganizationId, filter.OrganizationId);
            }
            return query;
        }

        public async Task<int> Count(IndirectPriceListItemMappingFilter filter)
        {
            IQueryable<IndirectPriceListItemMappingDAO> IndirectPriceListItemMappings = DataContext.IndirectPriceListItemMapping.AsNoTracking();
            IndirectPriceListItemMappings = DynamicFilter(IndirectPriceListItemMappings, filter);
            return await IndirectPriceListItemMappings.CountAsync();
        }

        public async Task<List<IndirectPriceListItemMapping>> List(IndirectPriceListItemMappingFilter filter)
        {
            if (filter == null) return new List<IndirectPriceListItemMapping>();
            IQueryable<IndirectPriceListItemMappingDAO> IndirectPriceListItemMappingDAOs = DataContext.IndirectPriceListItemMapping.AsNoTracking();
            IndirectPriceListItemMappingDAOs = DynamicFilter(IndirectPriceListItemMappingDAOs, filter);
            IndirectPriceListItemMappingDAOs = IndirectPriceListItemMappingDAOs.OrderBy(x => x.Price).Skip(0).Take(int.MaxValue);
            List<IndirectPriceListItemMapping> IndirectPriceListItemMappings = await IndirectPriceListItemMappingDAOs.Select(x => new IndirectPriceListItemMapping
            {
                IndirectPriceListId = x.IndirectPriceListId,
                ItemId =x.ItemId,
                Price = x.Price,
                IndirectPriceList = new IndirectPriceList
                {
                    Id = x.IndirectPriceList.Id,
                    OrganizationId = x.IndirectPriceList.OrganizationId
                }
            }).ToListAsync();
            return IndirectPriceListItemMappings;
        }
    }

}
