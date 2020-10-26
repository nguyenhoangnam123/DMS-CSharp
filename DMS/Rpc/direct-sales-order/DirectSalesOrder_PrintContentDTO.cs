﻿using DMS.Common;
using DMS.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.direct_sales_order
{
    public class DirectSalesOrder_PrintContentDTO : DataDTO
    {
        public long STT { get; set; }
        public long Id { get; set; }
        public long Quantity { get; set; }
        public string QuantityString { get; set; }
        public long RequestedQuantity { get; set; }
        public string RequestedQuantityString { get; set; }
        public decimal PrimaryPrice { get; set; }
        public string PrimaryPriceString { get; set; }
        public decimal SalePrice { get; set; }
        public string SalePriceString { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public string DiscountString { get; set; }
        public decimal Amount { get; set; }
        public string AmountString { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxAmount { get; set; }
        public string TaxPercentageString { get; set; }
        public long? Factor { get; set; }
        public DirectSalesOrder_ItemDTO Item { get; set; }
        public DirectSalesOrder_UnitOfMeasureDTO PrimaryUnitOfMeasure { get; set; }
        public DirectSalesOrder_TaxTypeDTO TaxType { get; set; }
        public DirectSalesOrder_UnitOfMeasureDTO UnitOfMeasure { get; set; }

        public DirectSalesOrder_PrintContentDTO() { }
        public DirectSalesOrder_PrintContentDTO(DirectSalesOrderContent DirectSalesOrderContent)
        {
            this.Id = DirectSalesOrderContent.Id;
            this.Quantity = DirectSalesOrderContent.Quantity;
            this.PrimaryPrice = DirectSalesOrderContent.PrimaryPrice;
            this.RequestedQuantity = DirectSalesOrderContent.RequestedQuantity;
            this.SalePrice = DirectSalesOrderContent.SalePrice;
            this.DiscountPercentage = DirectSalesOrderContent.DiscountPercentage;
            this.DiscountAmount = DirectSalesOrderContent.DiscountAmount;
            this.GeneralDiscountPercentage = DirectSalesOrderContent.GeneralDiscountPercentage;
            this.GeneralDiscountAmount = DirectSalesOrderContent.GeneralDiscountAmount;
            this.Amount = DirectSalesOrderContent.Amount;
            this.TaxPercentage = DirectSalesOrderContent.TaxPercentage;
            this.TaxAmount = DirectSalesOrderContent.TaxAmount;
            this.Factor = DirectSalesOrderContent.Factor;
            this.Item = DirectSalesOrderContent.Item == null ? null : new DirectSalesOrder_ItemDTO(DirectSalesOrderContent.Item);
            this.PrimaryUnitOfMeasure = DirectSalesOrderContent.PrimaryUnitOfMeasure == null ? null : new DirectSalesOrder_UnitOfMeasureDTO(DirectSalesOrderContent.PrimaryUnitOfMeasure);
            this.UnitOfMeasure = DirectSalesOrderContent.UnitOfMeasure == null ? null : new DirectSalesOrder_UnitOfMeasureDTO(DirectSalesOrderContent.UnitOfMeasure);
            this.Errors = DirectSalesOrderContent.Errors;
        }
    }
}
