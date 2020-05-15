﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_ERouteDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? ERouteTypeId { get; set; }
        public long RequestStateId { get; set; }
        public long StatusId { get; set; }
        public long CreatorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public StoreChecking_ERouteDTO() { }
        public StoreChecking_ERouteDTO(ERoute ERoute)
        {
            this.Id = ERoute.Id;
            this.Code = ERoute.Code;
            this.Name = ERoute.Name;
            this.SaleEmployeeId = ERoute.SaleEmployeeId;
            this.StartDate = ERoute.StartDate;
            this.EndDate = ERoute.EndDate;
            this.ERouteTypeId = ERoute.ERouteTypeId;
            this.RequestStateId = ERoute.RequestStateId;
            this.StatusId = ERoute.StatusId;
            this.CreatorId = ERoute.CreatorId;
            this.CreatedAt = ERoute.CreatedAt;
            this.UpdatedAt = ERoute.UpdatedAt;
            this.Errors = ERoute.Errors;
        }
    }

    public class StoreChecking_ERouteFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public StringFilter Name { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter StartDate { get; set; }
        public DateFilter EndDate { get; set; }
        public IdFilter ERouteTypeId { get; set; }
        public IdFilter RequestStateId { get; set; }
        public IdFilter StatusId { get; set; }
        public IdFilter CreatorId { get; set; }
        public IdFilter StoreId { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public ERouteOrder OrderBy { get; set; }
    }
}
