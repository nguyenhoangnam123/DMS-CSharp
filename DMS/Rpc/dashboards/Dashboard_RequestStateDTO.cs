﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.dashboards
{
    public class Dashboard_RequestStateDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Dashboard_RequestStateDTO() { }
        public Dashboard_RequestStateDTO(RequestState RequestState)
        {
            this.Id = RequestState.Id;
            this.Code = RequestState.Code;
            this.Name = RequestState.Name;
            this.Errors = RequestState.Errors;
        }
    }

    public class Dashboard_RequestStateFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public RequestStateOrder OrderBy { get; set; }
    }
}