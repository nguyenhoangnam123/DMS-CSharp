using Common;
using DMS.Entities;
using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.mobile_sync
{
    public class MobileSync_IndirectSalesOrderDTO
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public long BuyerStoreId { get; set; }
        public string PhoneNumber { get; set; }
        public string StoreAddress { get; set; }
        public string DeliveryAddress { get; set; }
        public long SellerStoreId { get; set; }
        public long SaleEmployeeId { get; set; }
        public long OrganizationId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public long RequestStateId { get; set; }
        public long EditedPriceStatusId { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal? GeneralDiscountPercentage { get; set; }
        public decimal? GeneralDiscountAmount { get; set; }
        public decimal TotalTaxAmount { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public MobileSync_StoreDTO BuyerStore { get; set; }
        public MobileSync_EditedPriceStatusDTO EditedPriceStatus { get; set; }
        public MobileSync_RequestStateDTO RequestState { get; set; }
        public MobileSync_AppUserDTO SaleEmployee { get; set; }
        public MobileSync_OrganizationDTO Organization { get; set; }
        public MobileSync_StoreDTO SellerStore { get; set; }
        public List<MobileSync_IndirectSalesOrderContentDTO> IndirectSalesOrderContents { get; set; }
        public List<MobileSync_IndirectSalesOrderPromotionDTO> IndirectSalesOrderPromotions { get; set; }
        public List<MobileSync_RequestWorkflowStepMappingDTO> RequestWorkflowStepMappings { get; set; }
        public MobileSync_IndirectSalesOrderDTO() { }
        public MobileSync_IndirectSalesOrderDTO(IndirectSalesOrderDAO IndirectSalesOrderDAO)
        {
            this.Id = IndirectSalesOrderDAO.Id;
            this.Code = IndirectSalesOrderDAO.Code;
            this.BuyerStoreId = IndirectSalesOrderDAO.BuyerStoreId;
            this.PhoneNumber = IndirectSalesOrderDAO.PhoneNumber;
            this.StoreAddress = IndirectSalesOrderDAO.StoreAddress;
            this.DeliveryAddress = IndirectSalesOrderDAO.DeliveryAddress;
            this.SellerStoreId = IndirectSalesOrderDAO.SellerStoreId;
            this.SaleEmployeeId = IndirectSalesOrderDAO.SaleEmployeeId;
            this.OrganizationId = IndirectSalesOrderDAO.OrganizationId;
            this.OrderDate = IndirectSalesOrderDAO.OrderDate;
            this.DeliveryDate = IndirectSalesOrderDAO.DeliveryDate;
            this.RequestStateId = IndirectSalesOrderDAO.RequestStateId;
            this.EditedPriceStatusId = IndirectSalesOrderDAO.EditedPriceStatusId;
            this.Note = IndirectSalesOrderDAO.Note;
            this.SubTotal = Math.Round(IndirectSalesOrderDAO.SubTotal, 0);
            this.GeneralDiscountPercentage = IndirectSalesOrderDAO.GeneralDiscountPercentage;
            this.GeneralDiscountAmount = IndirectSalesOrderDAO.GeneralDiscountAmount;
            this.TotalTaxAmount = IndirectSalesOrderDAO.TotalTaxAmount;
            this.Total = Math.Round(IndirectSalesOrderDAO.Total, 0);
            this.CreatedAt = IndirectSalesOrderDAO.CreatedAt;
            this.UpdatedAt = IndirectSalesOrderDAO.UpdatedAt;
            this.BuyerStore = IndirectSalesOrderDAO.BuyerStore == null ? null : new MobileSync_StoreDTO(IndirectSalesOrderDAO.BuyerStore);
            this.EditedPriceStatus = IndirectSalesOrderDAO.EditedPriceStatus == null ? null : new MobileSync_EditedPriceStatusDTO(IndirectSalesOrderDAO.EditedPriceStatus);
            this.RequestState = IndirectSalesOrderDAO.RequestState == null ? null : new MobileSync_RequestStateDTO(IndirectSalesOrderDAO.RequestState);
            this.SaleEmployee = IndirectSalesOrderDAO.SaleEmployee == null ? null : new MobileSync_AppUserDTO(IndirectSalesOrderDAO.SaleEmployee);
            this.Organization = IndirectSalesOrderDAO.Organization == null ? null : new MobileSync_OrganizationDTO(IndirectSalesOrderDAO.Organization);
            this.SellerStore = IndirectSalesOrderDAO.SellerStore == null ? null : new MobileSync_StoreDTO(IndirectSalesOrderDAO.SellerStore);
            this.IndirectSalesOrderContents = IndirectSalesOrderDAO.IndirectSalesOrderContents?.Select(x => new MobileSync_IndirectSalesOrderContentDTO(x)).ToList();
            this.IndirectSalesOrderPromotions = IndirectSalesOrderDAO.IndirectSalesOrderPromotions?.Select(x => new MobileSync_IndirectSalesOrderPromotionDTO(x)).ToList();
            
        }
    }
}
