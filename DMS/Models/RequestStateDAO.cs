using System;
using System.Collections.Generic;

namespace DMS.Models
{
    public partial class RequestStateDAO
    {
        public RequestStateDAO()
        {
            DirectSalesOrders = new HashSet<DirectSalesOrderDAO>();
            ERouteChangeRequests = new HashSet<ERouteChangeRequestDAO>();
            ERoutes = new HashSet<ERouteDAO>();
            IndirectSalesOrders = new HashSet<IndirectSalesOrderDAO>();
            RequestWorkflowDefinitionMappings = new HashSet<RequestWorkflowDefinitionMappingDAO>();
            Stores = new HashSet<StoreDAO>();
        }

        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DirectSalesOrderDAO> DirectSalesOrders { get; set; }
        public virtual ICollection<ERouteChangeRequestDAO> ERouteChangeRequests { get; set; }
        public virtual ICollection<ERouteDAO> ERoutes { get; set; }
        public virtual ICollection<IndirectSalesOrderDAO> IndirectSalesOrders { get; set; }
        public virtual ICollection<RequestWorkflowDefinitionMappingDAO> RequestWorkflowDefinitionMappings { get; set; }
        public virtual ICollection<StoreDAO> Stores { get; set; }
    }
}
