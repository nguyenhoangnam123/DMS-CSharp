using DMS.Common;
using DMS.Handlers;
using DMS.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IEventMessageRepository
    {
        Task<long> Count(EventMessageFilter EventMessageFilter);
        Task<List<EventMessage<T>>> List<T>(EventMessageFilter EventMessageFilter);
        Task<EventMessage<T>> Get<T>(long Id);
        Task<bool> Create<T>(EventMessage<T> EventMessage);
        Task<bool> BulkMerge<T>(List<EventMessage<T>> EventMessages);
    }
    public class EventMessageRepository : IEventMessageRepository
    {
        private DataContext DataContext;
        public EventMessageRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }

        private IQueryable<EventMessageDAO> DynamicFilter(IQueryable<EventMessageDAO> query, EventMessageFilter filter)
        {
            if (filter == null)
                return query.Where(q => false);
            if (filter.Id != null)
                query = query.Where(q => q.Id, filter.Id);
            if (filter.Time != null)
                query = query.Where(q => q.Time, filter.Time);
            if (filter.RowId != null)
                query = query.Where(q => q.RowId, filter.RowId);
            if (filter.EntityName != null)
                query = query.Where(q => q.EntityName, filter.EntityName);
            return query;
        }

        private IQueryable<EventMessageDAO> DynamicOrder(IQueryable<EventMessageDAO> query, EventMessageFilter filter)
        {
            switch (filter.OrderType)
            {
                case OrderType.ASC:
                    switch (filter.OrderBy)
                    {
                        case EventMessageOrder.Id:
                            query = query.OrderBy(q => q.Id);
                            break;
                        case EventMessageOrder.Time:
                            query = query.OrderBy(q => q.Time);
                            break;
                        case EventMessageOrder.RowId:
                            query = query.OrderBy(q => q.RowId);
                            break;
                        case EventMessageOrder.EntityName:
                            query = query.OrderBy(q => q.EntityName);
                            break;
                    }
                    break;
                case OrderType.DESC:
                    switch (filter.OrderBy)
                    {
                        case EventMessageOrder.Id:
                            query = query.OrderByDescending(q => q.Id);
                            break;
                        case EventMessageOrder.Time:
                            query = query.OrderByDescending(q => q.Time);
                            break;
                        case EventMessageOrder.RowId:
                            query = query.OrderByDescending(q => q.RowId);
                            break;
                        case EventMessageOrder.EntityName:
                            query = query.OrderByDescending(q => q.EntityName);
                            break;
                    }
                    break;
            }
            query = query.Skip(filter.Skip).Take(filter.Take);
            return query;
        }

        private async Task<List<EventMessage<T>>> DynamicSelect<T>(IQueryable<EventMessageDAO> query, EventMessageFilter filter)
        {
            List<EventMessage<T>> EventMessages = await query.Select(q => new EventMessage<T>()
            {
                Id = filter.Selects.Contains(EventMessageSelect.Id) ? q.Id : default(long),
                Time = filter.Selects.Contains(EventMessageSelect.Time) ? q.Time : default(DateTime),
                RowId = filter.Selects.Contains(EventMessageSelect.RowId) ? q.RowId : default(Guid),
                EntityName = filter.Selects.Contains(EventMessageSelect.EntityName) ? q.EntityName : default(string),
                Content = JsonConvert.DeserializeObject<T>(q.Content)
            }).ToListAsync();
            return EventMessages;
        }

        public async Task<long> Count(EventMessageFilter filter)
        {
            IQueryable<EventMessageDAO> EventMessages = DataContext.EventMessage;
            EventMessages = DynamicFilter(EventMessages, filter);
            return await EventMessages.CountAsync();
        }

        public async Task<List<EventMessage<T>>> List<T>(EventMessageFilter filter)
        {
            if (filter == null) return new List<EventMessage<T>>();
            IQueryable<EventMessageDAO> EventMessageDAOs = DataContext.EventMessage;
            EventMessageDAOs = DynamicFilter(EventMessageDAOs, filter);
            EventMessageDAOs = DynamicOrder(EventMessageDAOs, filter);
            List<EventMessage<T>> EventMessages = await DynamicSelect<T>(EventMessageDAOs, filter);
            return EventMessages;
        }

        public async Task<EventMessage<T>> Get<T>(long Id)
        {
            EventMessage<T> eventMessage = await DataContext.EventMessage.Where(e => e.Id == Id).AsNoTracking().Select(e => new EventMessage<T>
            {
                Id = e.Id,
                Time = e.Time,
                RowId = e.RowId,
                EntityName = e.EntityName,
                Content = JsonConvert.DeserializeObject<T>(e.Content)
            }).FirstOrDefaultAsync();

            if (eventMessage == null)
                return null;
            return eventMessage;
        }

        public async Task<bool> Create<T>(EventMessage<T> EventMessage)
        {
            EventMessageDAO eventMessageDAO = new EventMessageDAO
            {
                Id = EventMessage.Id,
                Time = EventMessage.Time,
                RowId = EventMessage.RowId,
                EntityName = typeof(T).Name,
                Content = JsonConvert.SerializeObject(EventMessage.Content)
            };
            await DataContext.EventMessage.AddAsync(eventMessageDAO);
            return true;
        }

        public async Task<bool> BulkMerge<T>(List<EventMessage<T>> EventMessages)
        {
            List<EventMessageDAO> EventMessageDAOs = new List<EventMessageDAO>();
            foreach (var EventMessage in EventMessages)
            {
                EventMessageDAO eventMessageDAO = new EventMessageDAO
                {
                    Id = EventMessage.Id,
                    Time = EventMessage.Time,
                    RowId = EventMessage.RowId,
                    EntityName = typeof(T).Name,
                    Content = JsonConvert.SerializeObject(EventMessage.Content)
                };
                EventMessageDAOs.Add(eventMessageDAO);
            }
            await DataContext.EventMessage.BulkMergeAsync(EventMessageDAOs);
            return true;
        }
    }
}
