﻿using Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMS.Rpc.store_checking
{
    public class StoreChecking_IndirectSalesOrderDTO : DataDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long BuyerStoreId { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public long SellerStoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long RequestStateId { get; set; }
        public long EditedPriceStatusId { get; set; }
        public string Note { get; set; }
        public long SubTotal { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public long? GeneralDiscountAmount { get; set; }
        public long TotalTaxAmount { get; set; }
        public long Total { get; set; }
        public long? StoreCheckingId { get; set; }
        public StoreChecking_StoreDTO BuyerStore { get; set; }
        public StoreChecking_EditedPriceStatusDTO EditedPriceStatus { get; set; }
        public StoreChecking_RequestStateDTO RequestState { get; set; }
        public StoreChecking_AppUserDTO SaleEmployee { get; set; }
        public StoreChecking_StoreDTO SellerStore { get; set; }
        public List<StoreChecking_IndirectSalesOrderContentDTO> IndirectSalesOrderContents { get; set; }
        public List<StoreChecking_IndirectSalesOrderPromotionDTO> IndirectSalesOrderPromotions { get; set; }
        public StoreChecking_IndirectSalesOrderDTO() { }
        public StoreChecking_IndirectSalesOrderDTO(IndirectSalesOrder IndirectSalesOrder)
        {
            this.Id = IndirectSalesOrder.Id;
            this.Code = IndirectSalesOrder.Code;
            this.BuyerStoreId = IndirectSalesOrder.BuyerStoreId;
            this.PhoneNumber = IndirectSalesOrder.PhoneNumber;
            this.StoreAddress = IndirectSalesOrder.StoreAddress;
            this.DeliveryAddress = IndirectSalesOrder.DeliveryAddress;
            this.SellerStoreId = IndirectSalesOrder.SellerStoreId;
            this.SaleEmployeeId = IndirectSalesOrder.SaleEmployeeId;
            this.OrderDate = IndirectSalesOrder.OrderDate;
            this.DeliveryDate = IndirectSalesOrder.DeliveryDate;
            this.RequestStateId = IndirectSalesOrder.RequestStateId;
            this.EditedPriceStatusId = IndirectSalesOrder.EditedPriceStatusId;
            this.Note = IndirectSalesOrder.Note;
            this.SubTotal = IndirectSalesOrder.SubTotal;
            this.GeneralDiscountPercentage = IndirectSalesOrder.GeneralDiscountPercentage;
            this.GeneralDiscountAmount = IndirectSalesOrder.GeneralDiscountAmount;
            this.TotalTaxAmount = IndirectSalesOrder.TotalTaxAmount;
            this.Total = IndirectSalesOrder.Total;
            this.StoreCheckingId = StoreCheckingId;
            this.BuyerStore = IndirectSalesOrder.BuyerStore == null ? null : new StoreChecking_StoreDTO(IndirectSalesOrder.BuyerStore);
            this.EditedPriceStatus = IndirectSalesOrder.EditedPriceStatus == null ? null : new StoreChecking_EditedPriceStatusDTO(IndirectSalesOrder.EditedPriceStatus);
            this.RequestState = IndirectSalesOrder.RequestState == null ? null : new StoreChecking_RequestStateDTO(IndirectSalesOrder.RequestState);
            this.SaleEmployee = IndirectSalesOrder.SaleEmployee == null ? null : new StoreChecking_AppUserDTO(IndirectSalesOrder.SaleEmployee);
            this.SellerStore = IndirectSalesOrder.SellerStore == null ? null : new StoreChecking_StoreDTO(IndirectSalesOrder.SellerStore);
            this.IndirectSalesOrderContents = IndirectSalesOrder.IndirectSalesOrderContents?.Select(x => new StoreChecking_IndirectSalesOrderContentDTO(x)).ToList();
            this.IndirectSalesOrderPromotions = IndirectSalesOrder.IndirectSalesOrderPromotions?.Select(x => new StoreChecking_IndirectSalesOrderPromotionDTO(x)).ToList();
            this.Errors = IndirectSalesOrder.Errors;
        }
    }

    public class StoreChecking_IndirectSalesOrderFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Code { get; set; }
        public IdFilter BuyerStoreId { get; set; }
        public StringFilter PhoneNumber { get; set; }
        public StringFilter StoreAddress { get; set; }
        public StringFilter DeliveryAddress { get; set; }
        public IdFilter SellerStoreId { get; set; }
        public IdFilter SaleEmployeeId { get; set; }
        public DateFilter OrderDate { get; set; }
        public DateFilter DeliveryDate { get; set; }
        public IdFilter RequestStateId { get; set; }
        public IdFilter EditedPriceStatusId { get; set; }
        public StringFilter Note { get; set; }
        public LongFilter SubTotal { get; set; }
        public LongFilter GeneralDiscountPercentage { get; set; }
        public LongFilter GeneralDiscountAmount { get; set; }
        public LongFilter TotalTaxAmount { get; set; }
        public LongFilter Total { get; set; }
        public IndirectSalesOrderOrder OrderBy { get; set; }
    }
}
