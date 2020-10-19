﻿using Common;
using DMS.Entities;

namespace DMS.Rpc.price_list
{
    public class PriceList_RequestStateDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public PriceList_RequestStateDTO() { }
        public PriceList_RequestStateDTO(RequestState RequestState)
        {
            this.Id = RequestState.Id;
            this.Code = RequestState.Code;
            this.Name = RequestState.Name;
            this.Errors = RequestState.Errors;
        }
    }

    public class PriceList_RequestStateFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public RequestStateOrder OrderBy { get; set; }
    }
}
