using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.monitor.monitor_salesman
{
    public class MonitorSalesman_StoreImageDTO : DataDTO
    {
        public long ImageId { get; set; }
        public long AlbumId { get; set; }
        public long StoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime ShootingAt { get; set; }
        public decimal? Distance { get; set; }
        public MonitorSalesman_AlbumDTO Album { get; set; }
        public MonitorSalesman_AppUserDTO SaleEmployee { get; set; }
        public MonitorSalesman_ImageDTO Image { get; set; }
        public MonitorSalesman_StoreDTO Store { get; set; }
    }
}
