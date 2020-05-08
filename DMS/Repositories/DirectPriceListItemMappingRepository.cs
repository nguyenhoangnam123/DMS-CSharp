using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IDirectPriceListItemMappingItemMappingRepository
    {
        Task<int> Count(DirectPriceListItemMappingFilter DirectPriceListItemMappingFilter);
        Task<List<DirectPriceListItemMapping>> List(DirectPriceListItemMappingFilter DirectPriceListItemMappingFilter);
    }
    public class DirectPriceListItemMappingItemMappingRepository : IDirectPriceListItemMappingItemMappingRepository
    {
        private readonly DataContext DataContext;

        public DirectPriceListItemMappingItemMappingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<DirectPriceListItemMappingDAO> DynamicFilter(IQueryable<DirectPriceListItemMappingDAO> query, DirectPriceListItemMappingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.DirectPriceListId != null)
                query = query.Where(q => q.DirectPriceListId, filter.DirectPriceListId);
            if (filter.ItemId != null)
                query = query.Where(q => q.ItemId, filter.ItemId);
            if (filter.Price != null)
                query = query.Where(q => q.Price, filter.Price);
            if (filter.DirectPriceListTypeId != null)
            {
                query = query.Where(q => q.DirectPriceList.DirectPriceListTypeId, filter.DirectPriceListTypeId);
            }
            if (filter.StoreGroupingId != null)
            {
                if (filter.StoreGroupingId.Equal.HasValue)
                {
                    StoreGroupingDAO StoreGroupingDAO = DataContext.StoreGrouping.Where(x => x.Id == filter.StoreGroupingId.Equal.Value).FirstOrDefault();
                    if (StoreGroupingDAO != null && StoreGroupingDAO.StatusId == Enums.StatusEnum.ACTIVE.Id)
                        query = from q in query
                                join sg in DataContext.DirectPriceListStoreGroupingMapping on q.DirectPriceListId equals sg.DirectPriceListId
                                where sg.StoreGroupingId == StoreGroupingDAO.Id
                                select q;
                }
            }
            if (filter.StoreTypeId != null)
            {
                if (filter.StoreTypeId.Equal.HasValue)
                {
                    StoreTypeDAO StoreTypeDAO = DataContext.StoreType.Where(x => x.Id == filter.StoreTypeId.Equal.Value).FirstOrDefault();
                    if (StoreTypeDAO != null && StoreTypeDAO.StatusId == Enums.StatusEnum.ACTIVE.Id)
                        query = from q in query
                                join st in DataContext.DirectPriceListStoreTypeMapping on q.DirectPriceListId equals st.DirectPriceListId
                                where st.StoreTypeId == StoreTypeDAO.Id
                                select q;
                }
            }

            if (filter.StoreId != null)
            {
                if (filter.StoreId.Equal.HasValue)
                {
                    StoreDAO StoreDAO = DataContext.Store.Where(x => x.Id == filter.StoreId.Equal.Value).FirstOrDefault();
                    if (StoreDAO != null && StoreDAO.StatusId == Enums.StatusEnum.ACTIVE.Id)
                        query = from q in query
                                join s in DataContext.DirectPriceListStoreMapping on q.DirectPriceListId equals s.DirectPriceListId
                                where s.StoreId == StoreDAO.Id
                                select q;
                }
            }

            if (filter.OrganizationId != null)
            {
                query = query.Where(q => q.DirectPriceList.OrganizationId, filter.OrganizationId);
            }

            if (filter.StatusId != null)
                query = query.Where(q => q.DirectPriceList.StatusId, filter.StatusId);

            query = query.Where(q => q.DirectPriceList.StartDate <= StaticParams.DateTimeNow);
            query = query.Where(q => q.DirectPriceList.EndDate.HasValue == false || (q.DirectPriceList.EndDate.HasValue && q.DirectPriceList.EndDate >= StaticParams.DateTimeNow));

            return query;
        }

        public async Task<int> Count(DirectPriceListItemMappingFilter filter)
        {
            IQueryable<DirectPriceListItemMappingDAO> DirectPriceListItemMappings = DataContext.DirectPriceListItemMapping.AsNoTracking();
            DirectPriceListItemMappings = DynamicFilter(DirectPriceListItemMappings, filter);
            return await DirectPriceListItemMappings.CountAsync();
        }

        public async Task<List<DirectPriceListItemMapping>> List(DirectPriceListItemMappingFilter filter)
        {
            if (filter == null) return new List<DirectPriceListItemMapping>();
            IQueryable<DirectPriceListItemMappingDAO> DirectPriceListItemMappingDAOs = DataContext.DirectPriceListItemMapping.AsNoTracking();
            DirectPriceListItemMappingDAOs = DynamicFilter(DirectPriceListItemMappingDAOs, filter);
            DirectPriceListItemMappingDAOs = DirectPriceListItemMappingDAOs.OrderBy(x => x.Price).Skip(0).Take(int.MaxValue);
            List<DirectPriceListItemMapping> DirectPriceListItemMappings = await DirectPriceListItemMappingDAOs.Select(x => new DirectPriceListItemMapping
            {
                DirectPriceListId = x.DirectPriceListId,
                ItemId = x.ItemId,
                Price = x.Price,
                DirectPriceList = new DirectPriceList
                {
                    Id = x.DirectPriceList.Id,
                    OrganizationId = x.DirectPriceList.OrganizationId
                }
            }).ToListAsync();
            return DirectPriceListItemMappings;
        }
    }
}
