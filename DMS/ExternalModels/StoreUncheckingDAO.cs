using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public partial class StoreUncheckingDAO : IEquatable<StoreUncheckingDAO>
    {
        public bool Equals(StoreUncheckingDAO other)
        {
            return other != null && AppUserId == other.AppUserId &&
                StoreId == other.StoreId &&
                OrganizationId == other.OrganizationId &&
                Date == other.Date;

        }

        public override int GetHashCode()
        {
            return AppUserId.GetHashCode() ^
                StoreId.GetHashCode() ^
                OrganizationId.GetHashCode() ^
                Date.GetHashCode();
        }
    }
}
