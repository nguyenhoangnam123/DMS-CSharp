using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.RpcPublic
{
    public class PublicRoute : Root
    {
        private const string Default = "public/" + Rpc + Module;
        public const string SingleListStoreGrouping = Default + "/single-list-store-grouping";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";

        public const string CountItem = Default + "/count-item";
        public const string ListItem = Default + "/list-item";
        public const string GetItem = Default + "/get-item";
    }
}
