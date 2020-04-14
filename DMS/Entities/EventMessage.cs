using Common;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Entities
{
    public class EventMessage<T> : DataEntity
    {
        public long Id { get; set; }
        public DateTime Time { get; set; }
        public Guid RowId { get; set; }
        public string EntityName { get; set; }
        public T Content { get; set; }

        public EventMessage() { }
        public EventMessage(T content, Guid RowId)
        {
            Time = StaticParams.DateTimeNow;
            this.RowId = RowId;
            EntityName = typeof(T).Name;
            this.Content = content;
        }
    }
}
