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
    public interface IPriceListItemMappingItemMappingRepository
    {
        Task<int> Count(PriceListItemMappingFilter PriceListItemMappingFilter);
        Task<List<PriceListItemMapping>> List(PriceListItemMappingFilter PriceListItemMappingFilter);
    }
    public class PriceListItemMappingItemMappingRepository : IPriceListItemMappingItemMappingRepository
    {
        private readonly DataContext DataContext;

        public PriceListItemMappingItemMappingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<PriceListItemMappingDAO> DynamicFilter(IQueryable<PriceListItemMappingDAO> query, PriceListItemMappingFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            query = query.Where(q => q.PriceList.DeletedAt == null);

            query = from q in query
                    join p in DataContext.PriceList on q.PriceListId equals p.Id
                    where p.StartDate.HasValue == false ||
                    (
                        p.StartDate.HasValue == true &&
                        (
                            (p.StartDate.Value <= StaticParams.DateTimeNow && p.EndDate.HasValue == false) ||
                            (p.StartDate.Value <= StaticParams.DateTimeNow && p.EndDate.HasValue == true && StaticParams.DateTimeNow <= p.EndDate.Value)
                        )
                    )
                    select q;

            if (filter.PriceListId != null)
                query = query.Where(q => q.PriceListId, filter.PriceListId);
            if (filter.ItemId != null)
                query = query.Where(q => q.ItemId, filter.ItemId);
            if (filter.Price != null)
                query = query.Where(q => q.Price, filter.Price);
            if (filter.PriceListTypeId != null)
            {
                query = query.Where(q => q.PriceList.PriceListTypeId, filter.PriceListTypeId);
            }
            if (filter.StoreGroupingId != null)
            {
                if (filter.StoreGroupingId.Equal.HasValue)
                {
                    StoreGroupingDAO StoreGroupingDAO = DataContext.StoreGrouping.Where(x => x.Id == filter.StoreGroupingId.Equal.Value).FirstOrDefault();
                    if (StoreGroupingDAO != null && StoreGroupingDAO.StatusId == Enums.StatusEnum.ACTIVE.Id)
                        query = from q in query
                                join sg in DataContext.PriceListStoreGroupingMapping on q.PriceListId equals sg.PriceListId
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
                                join st in DataContext.PriceListStoreTypeMapping on q.PriceListId equals st.PriceListId
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
                                join s in DataContext.PriceListStoreMapping on q.PriceListId equals s.PriceListId
                                where s.StoreId == StoreDAO.Id
                                select q;
                }
            }

            if (filter.OrganizationId != null)
            {
                query = query.Where(q => q.PriceList.OrganizationId, filter.OrganizationId);
            }

            if (filter.StatusId != null)
                query = query.Where(q => q.PriceList.StatusId, filter.StatusId);

            query = query.Where(q => q.PriceList.StartDate.HasValue == false || 
                (q.PriceList.StartDate.HasValue == true && q.PriceList.StartDate.Value <= StaticParams.DateTimeNow));
            query = query.Where(q => q.PriceList.EndDate.HasValue == false || 
                (q.PriceList.EndDate.HasValue && q.PriceList.EndDate.Value >= StaticParams.DateTimeNow));

            return query;
        }

        public async Task<int> Count(PriceListItemMappingFilter filter)
        {
            IQueryable<PriceListItemMappingDAO> PriceListItemMappings = DataContext.PriceListItemMapping.AsNoTracking();
            PriceListItemMappings = DynamicFilter(PriceListItemMappings, filter);
            return await PriceListItemMappings.CountAsync();
        }

        public async Task<List<PriceListItemMapping>> List(PriceListItemMappingFilter filter)
        {
            if (filter == null) return new List<PriceListItemMapping>();
            IQueryable<PriceListItemMappingDAO> PriceListItemMappingDAOs = DataContext.PriceListItemMapping.AsNoTracking();
            PriceListItemMappingDAOs = DynamicFilter(PriceListItemMappingDAOs, filter);
            PriceListItemMappingDAOs = PriceListItemMappingDAOs.OrderBy(x => x.Price).Skip(0).Take(int.MaxValue);
            List<PriceListItemMapping> PriceListItemMappings = await PriceListItemMappingDAOs.Select(x => new PriceListItemMapping
            {
                PriceListId = x.PriceListId,
                ItemId = x.ItemId,
                Price = x.Price,
                PriceList = new PriceList
                {
                    Id = x.PriceList.Id,
                    OrganizationId = x.PriceList.OrganizationId
                }
            }).ToListAsync();
            return PriceListItemMappings;
        }
    }

}
