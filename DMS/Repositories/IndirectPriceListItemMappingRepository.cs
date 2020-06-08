using Common;
using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                    StoreGroupingDAO StoreGroupingDAO = DataContext.StoreGrouping.Where(x => x.Id == filter.StoreGroupingId.Equal.Value).FirstOrDefault();
                    if (StoreGroupingDAO != null && StoreGroupingDAO.StatusId == Enums.StatusEnum.ACTIVE.Id)
                        query = from q in query
                                join sg in DataContext.IndirectPriceListStoreGroupingMapping on q.IndirectPriceListId equals sg.IndirectPriceListId
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
                                join st in DataContext.IndirectPriceListStoreTypeMapping on q.IndirectPriceListId equals st.IndirectPriceListId
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
                                join s in DataContext.IndirectPriceListStoreMapping on q.IndirectPriceListId equals s.IndirectPriceListId
                                where s.StoreId == StoreDAO.Id
                                select q;
                }
            }

            if (filter.OrganizationId != null)
            {
                query = query.Where(q => q.IndirectPriceList.OrganizationId, filter.OrganizationId);
            }

            if (filter.StatusId != null)
                query = query.Where(q => q.IndirectPriceList.StatusId, filter.StatusId);

            query = query.Where(q => q.IndirectPriceList.StartDate <= StaticParams.DateTimeNow);
            query = query.Where(q => q.IndirectPriceList.EndDate.HasValue == false || (q.IndirectPriceList.EndDate.HasValue && q.IndirectPriceList.EndDate >= StaticParams.DateTimeNow));

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
                ItemId = x.ItemId,
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
