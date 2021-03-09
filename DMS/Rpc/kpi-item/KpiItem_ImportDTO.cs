using DMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.kpi_item
{
    public class KpiItem_ImportDTO : DataDTO
    {
        public long Stt { get; set; }
        public string UsernameValue { get; set; }
        public string ItemCodeValue { get; set; }
        public string IndirectQuantity { get; set; }
        public string IndirectRevenue { get; set; }
        public string IndirectCounter { get; set; }
        public string IndirectStoreCounter { get; set; }
        public string DirectQuantity { get; set; }
        public string DirectRevenue { get; set; }
        public string DirectCounter { get; set; }
        public string DirectStoreCounter { get; set; }
        public bool HasValue
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(ItemCodeValue) ||
                    !string.IsNullOrWhiteSpace(IndirectQuantity) ||
                    !string.IsNullOrWhiteSpace(IndirectRevenue) ||
                    !string.IsNullOrWhiteSpace(IndirectCounter) ||
                    !string.IsNullOrWhiteSpace(IndirectStoreCounter) ||
                    !string.IsNullOrWhiteSpace(DirectQuantity) ||
                    !string.IsNullOrWhiteSpace(DirectRevenue) ||
                    !string.IsNullOrWhiteSpace(DirectCounter) ||
                    !string.IsNullOrWhiteSpace(DirectStoreCounter))
                    return true;
                return false;
            }
        }

        public bool IsNew { get; set; }
        public long EmployeeId { get; set; }
        public long ItemId { get; set; }
        public long KpiYearId { get; set; }
        public long KpiPeriodId { get; set; }
        public long KpiItemTypeId { get; set; }
        public long OrganizationId { get; set; }
    }

    public class KpiItem_RowDTO : IEquatable<KpiItem_RowDTO>
    {
        public long AppUserId { get; set; }
        public long KpiYearId { get; set; }
        public long KpiPeriodId { get; set; }
        public long KpiItemTypeId { get; set; }
        public bool Equals(KpiItem_RowDTO other)
        {
            return other != null && this.AppUserId == other.AppUserId && this.KpiYearId == other.KpiYearId && this.KpiPeriodId == other.KpiPeriodId && this.KpiItemTypeId == other.KpiItemTypeId;
        }
        public override int GetHashCode()
        {
            return AppUserId.GetHashCode() ^ KpiYearId.GetHashCode() ^ KpiPeriodId.GetHashCode() ^ KpiItemTypeId.GetHashCode();
        }
    }
}
