using Common;
using System;
using System.Linq;
using System.Collections.Generic;
using DMS.Entities;

namespace DMS.Rpc.indirect_price_list
{
    public class IndirectPriceList_IndirectPriceListTypeDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Code { get; set; }
        
        public string Name { get; set; }
        

        public IndirectPriceList_IndirectPriceListTypeDTO() {}
        public IndirectPriceList_IndirectPriceListTypeDTO(IndirectPriceListType IndirectPriceListType)
        {
            
            this.Id = IndirectPriceListType.Id;
            
            this.Code = IndirectPriceListType.Code;
            
            this.Name = IndirectPriceListType.Name;
            
            this.Errors = IndirectPriceListType.Errors;
        }
    }

    public class IndirectPriceList_IndirectPriceListTypeFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Code { get; set; }
        
        public StringFilter Name { get; set; }
        
        public IndirectPriceListTypeOrder OrderBy { get; set; }
    }
}