using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public partial class ERouteContentDAO : IEquatable<ERouteContentDAO>
    {
        public bool Equals(ERouteContentDAO other)
        {
            return other != null && StoreId == other.StoreId &&
                Monday == other.Monday &&
                Tuesday == other.Tuesday &&
                Wednesday == other.Wednesday &&
                Thursday == other.Thursday &&
                Friday == other.Friday &&
                Saturday == other.Saturday &&
                Sunday == other.Sunday &&
                Week1 == other.Week1 &&
                Week2 == other.Week2 &&
                Week3 == other.Week3 &&
                Week4 == other.Week4;
        }
        public override int GetHashCode()
        {
            return StoreId.GetHashCode() ^
                Monday.GetHashCode() ^
                Tuesday.GetHashCode() ^
                Wednesday.GetHashCode() ^
                Thursday.GetHashCode() ^
                Friday.GetHashCode() ^
                Saturday.GetHashCode() ^
                Sunday.GetHashCode() ^
                Week1 .GetHashCode() ^
                Week2 .GetHashCode() ^
                Week3 .GetHashCode() ^
                Week4.GetHashCode();
        }
    }
}
