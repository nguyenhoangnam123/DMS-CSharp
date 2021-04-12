using DMS.Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Handlers;
using DMS.Helpers;
using DMS.Models;
using DMS.Repositories;
using DMS.Services;
using DMS.Services.MProduct;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DMS.Rpc
{
    public class SetupController : ControllerBase
    {
        private DataContext DataContext;
        private IItemService ItemService;
        private IMaintenanceService MaintenanceService;
        private IRabbitManager RabbitManager;
        private IUOW UOW;
        public SetupController(DataContext DataContext, IItemService ItemService, IMaintenanceService MaintenanceService, IRabbitManager RabbitManager, IUOW UOW)
        {
            this.ItemService = ItemService;
            this.MaintenanceService = MaintenanceService;
            this.DataContext = DataContext;
            this.RabbitManager = RabbitManager;
            this.UOW = UOW;
        }

        //[HttpGet, Route("rpc/dms/setup/save-image")]
        //public async Task SaveImage()
        //{
        //    string folderPath = @"E:\Downloads\Ma SP - Dong Luc";
        //    List<FileInfo> FileInfos = new List<FileInfo>();
        //    foreach (string file in Directory.EnumerateFiles(folderPath, "*.jpg"))
        //    {
        //        FileInfo FileInfo = new FileInfo(file);
        //        FileInfos.Add(FileInfo);
        //    }

        //    var FileNames = FileInfos.Select(x => x.Name.Replace(".jpg", string.Empty)).ToList();
        //    var Items = await DataContext.Item.Where(x => FileNames.Contains(x.Code)).ToListAsync();

        //    List<ItemImageMappingDAO> ItemImageMappingDAOs = new List<ItemImageMappingDAO>();
        //    foreach (var FileInfo in FileInfos)
        //    {
        //        var contents = await System.IO.File.ReadAllBytesAsync(FileInfo.FullName);

        //        Image Image = new Image()
        //        {
        //            Name = FileInfo.Name,
        //            Content = contents
        //        };
        //        var Item = Items.Where(x => x.Code == FileInfo.Name).FirstOrDefault();
        //        if (Item != null)
        //        {
        //            Image = await ItemService.SaveImage(Image);

        //            ItemImageMappingDAO ItemImageMappingDAO = new ItemImageMappingDAO()
        //            {
        //                ItemId = Item.Id,
        //                ImageId = Image.Id
        //            };
        //            ItemImageMappingDAOs.Add(ItemImageMappingDAO);

        //        }
        //    }
        //    await DataContext.ItemImageMapping.BulkMergeAsync(ItemImageMappingDAOs);
        //}

        //[HttpGet, Route("rpc/dms/setup/fix")]
        //public async Task Count()
        //{
        //    List<IndirectSalesOrderDAO> IndirectSalesOrderDAOs = await DataContext.IndirectSalesOrder.ToListAsync();
        //    List<IndirectSalesOrderContentDAO> IndirectSalesOrderContentDAOs = await DataContext.IndirectSalesOrderContent.ToListAsync();
        //    List<IndirectSalesOrderTransactionDAO> IndirectSalesOrderTransactionDAOs = await DataContext.IndirectSalesOrderTransaction.ToListAsync();

        //    foreach (var IndirectSalesOrderDAO in IndirectSalesOrderDAOs)
        //    {
        //        var subIndirectSalesOrderContents = IndirectSalesOrderContentDAOs.Where(X => X.IndirectSalesOrderId == IndirectSalesOrderDAO.Id).ToList();
        //        var subIndirectSalesOrderTransactions = IndirectSalesOrderTransactionDAOs.Where(X => X.IndirectSalesOrderId == IndirectSalesOrderDAO.Id).ToList();
        //        foreach (var IndirectSalesOrderContentDAO in subIndirectSalesOrderContents)
        //        {
        //            IndirectSalesOrderTransactionDAO IndirectSalesOrderTransactionDAO = subIndirectSalesOrderTransactions
        //                .Where(x => x.IndirectSalesOrderId == IndirectSalesOrderDAO.Id)
        //                .Where(x => x.SalesEmployeeId == IndirectSalesOrderDAO.SaleEmployeeId)
        //                .Where(x => x.OrganizationId == IndirectSalesOrderDAO.OrganizationId)
        //                .Where(x => x.BuyerStoreId == IndirectSalesOrderDAO.BuyerStoreId)
        //                .Where(x => x.ItemId == IndirectSalesOrderContentDAO.ItemId)
        //                .Where(x => x.TypeId == TransactionTypeEnum.SALES_CONTENT.Id)
        //                .FirstOrDefault();
        //            if(IndirectSalesOrderTransactionDAO != null)
        //            {
        //                IndirectSalesOrderTransactionDAO.Revenue = IndirectSalesOrderContentDAO.Amount - IndirectSalesOrderContentDAO.GeneralDiscountAmount ?? 0;
        //            }
        //        }
        //    }

        //    await DataContext.IndirectSalesOrderTransaction.BulkMergeAsync(IndirectSalesOrderTransactionDAOs);
        //}

        //[HttpGet, Route("rpc/dms/setup/store-gen-code")]
        //public async Task StoreGenCode()
        //{
        //    List<StoreDAO> Stores = await DataContext.Store.OrderByDescending(x => x.CreatedAt).ToListAsync();
        //    List<OrganizationDAO> Organizations = await DataContext.Organization.OrderByDescending(x => x.CreatedAt).ToListAsync();
        //    List<StoreTypeDAO> StoreTypes = await DataContext.StoreType.OrderByDescending(x => x.CreatedAt).ToListAsync();
        //    var counter = Stores.Count();
        //    foreach (var Store in Stores)
        //    {
        //        var Organization = Organizations.Where(x => x.Id == Store.OrganizationId).Select(x => x.Code).FirstOrDefault();
        //        var StoreType = StoreTypes.Where(x => x.Id == Store.StoreTypeId).Select(x => x.Code).FirstOrDefault();
        //        Store.Code = $"{Organization}.{StoreType}.{(10000000 + counter--).ToString().Substring(1)}";
        //    }
        //    await DataContext.SaveChangesAsync();
        //}

        [HttpGet, Route("rpc/dms/setup/es-publish")]
        public async Task<ActionResult> ESPublish()
        {
            await ESPublishDirectSalesOrder();
            await ESPublishStore();
            await ESPublishStoreGrouping();
            await ESPublishStoreType();
            await ESPublishStoreStatus();
            
            return Ok();
        }
        [HttpGet, Route("rpc/dms/setup/es-publish-direct-sales-order")]
        public async Task<ActionResult> ESPublishDirectSalesOrder()
        {
            #region DirectSalesOrder
            List<DirectSalesOrder> DirectSalesOrders = await UOW.DirectSalesOrderRepository.List(new DirectSalesOrderFilter
            {
                Skip = 0,
                Take = int.MaxValue,

                Selects = DirectSalesOrderSelect.Id,
            });

            List<long> DirectSalesOrderIds = DirectSalesOrders.Select(x => x.Id).ToList();
            DirectSalesOrders = await UOW.DirectSalesOrderRepository.List(DirectSalesOrderIds);

            List<EventMessage<DirectSalesOrder>> DirectSalesOrderEventMessages = new List<EventMessage<DirectSalesOrder>>();
            foreach (DirectSalesOrder DirectSalesOrder in DirectSalesOrders)
            {
                DirectSalesOrderEventMessages.Add(new EventMessage<DirectSalesOrder>(DirectSalesOrder, DirectSalesOrder.RowId));
            }
            RabbitManager.PublishList(DirectSalesOrderEventMessages, RoutingKeyEnum.DirectSalesOrderSync);
            #endregion

            return Ok();
        }

        [HttpGet, Route("rpc/dms/setup/es-publish-store")]
        public async Task<ActionResult> ESPublishStore()
        {
            #region Store
            List<Store> Stores = await UOW.StoreRepository.List(new StoreFilter
            {
                Skip = 0,
                Take = int.MaxValue,

                Selects = StoreSelect.Id,
            });

            List<long> StoreIds = Stores.Select(x => x.Id).ToList();
            Stores = await UOW.StoreRepository.List(StoreIds);

            List<EventMessage<Store>> StoreEventMessages = new List<EventMessage<Store>>();
            foreach (Store Store in Stores)
            {
                StoreEventMessages.Add(new EventMessage<Store>(Store, Store.RowId));
            }
            RabbitManager.PublishList(StoreEventMessages, RoutingKeyEnum.StoreSync);
            #endregion

            return Ok();
        }

        [HttpGet, Route("rpc/dms/setup/es-publish-store-grouping")]
        public async Task<ActionResult> ESPublishStoreGrouping()
        {
            #region StoreGrouping
            List<StoreGrouping> StoreGroupings = await UOW.StoreGroupingRepository.List(new StoreGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,

                Selects = StoreGroupingSelect.Id,
            });

            List<long> StoreGroupingIds = StoreGroupings.Select(x => x.Id).ToList();
            StoreGroupings = await UOW.StoreGroupingRepository.List(StoreGroupingIds);

            List<EventMessage<StoreGrouping>> StoreGroupingEventMessages = new List<EventMessage<StoreGrouping>>();
            foreach (StoreGrouping StoreGrouping in StoreGroupings)
            {
                StoreGroupingEventMessages.Add(new EventMessage<StoreGrouping>(StoreGrouping, StoreGrouping.RowId));
            }
            RabbitManager.PublishList(StoreGroupingEventMessages, RoutingKeyEnum.StoreGroupingSync);
            #endregion

            return Ok();
        }

        [HttpGet, Route("rpc/dms/setup/es-publish-store-type")]
        public async Task<ActionResult> ESPublishStoreType()
        {
            #region StoreType
            List<StoreType> StoreTypes = await UOW.StoreTypeRepository.List(new StoreTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,

                Selects = StoreTypeSelect.Id,
            });

            List<long> StoreTypeIds = StoreTypes.Select(x => x.Id).ToList();
            StoreTypes = await UOW.StoreTypeRepository.List(StoreTypeIds);

            List<EventMessage<StoreType>> StoreTypeEventMessages = new List<EventMessage<StoreType>>();
            foreach (StoreType StoreType in StoreTypes)
            {
                StoreTypeEventMessages.Add(new EventMessage<StoreType>(StoreType, StoreType.RowId));
            }
            RabbitManager.PublishList(StoreTypeEventMessages, RoutingKeyEnum.StoreTypeSync);
            #endregion

            return Ok();
        }

        [HttpGet, Route("rpc/dms/setup/es-publish-store-status")]
        public async Task<ActionResult> ESPublishStoreStatus()
        {
            #region StoreStatus
            List<StoreStatus> StoreStatuss = await UOW.StoreStatusRepository.List(new StoreStatusFilter
            {
                Skip = 0,
                Take = int.MaxValue,

                Selects = StoreStatusSelect.Id,
            });

            List<long> StoreStatusIds = StoreStatuss.Select(x => x.Id).ToList();
            StoreStatuss = await UOW.StoreStatusRepository.List(StoreStatusIds);

            List<EventMessage<StoreStatus>> StoreStatusEventMessages = new List<EventMessage<StoreStatus>>();
            foreach (StoreStatus StoreStatus in StoreStatuss)
            {
                StoreStatusEventMessages.Add(new EventMessage<StoreStatus>(StoreStatus, Guid.NewGuid()));
            }
            RabbitManager.PublishList(StoreStatusEventMessages, RoutingKeyEnum.StoreStatusSync);
            #endregion

            return Ok();
        }

        [HttpGet, Route("rpc/dms/setup/year/{year}")]
        public bool ChangeYear(int year)
        {
            StaticParams.ChangeYear = year;
            return true;
        }

        [HttpGet, Route("rpc/dms/setup/init")]
        public ActionResult Init()
        {
            InitEnum();
            InitRoute();
            InitAdmin();
            return Ok();
        }

        #region sync
        [HttpGet, Route("rpc/dms/setup/init-data")]
        public ActionResult InitData()
        {
            RestClient RestClient = new RestClient(InternalServices.ES);
            InitPosition(RestClient);
            InitProvince(RestClient);
            InitDistrict(RestClient);
            InitWard(RestClient);
            InitOrganization(RestClient);
            InitAppUser(RestClient);

            InitProductType(RestClient);
            InitProductGrouping(RestClient);
            InitCategory(RestClient);
            InitSupplier(RestClient);
            InitBrand(RestClient);
            InitUnitOfMeasure(RestClient);
            InitUnitOfMeasureGrouping(RestClient);
            InitTaxType(RestClient);
            InitProduct(RestClient);
            return Ok();
        }

        //[HttpGet, Route("rpc/dms/setup/delete-data")]
        //public async Task<ActionResult> DeleteData()
        //{
        //    await DataContext.DirectSalesOrderTransaction.DeleteFromQueryAsync();
        //    await DataContext.DirectSalesOrderContent.DeleteFromQueryAsync();
        //    await DataContext.DirectSalesOrderPromotion.DeleteFromQueryAsync();
        //    await DataContext.DirectSalesOrder.DeleteFromQueryAsync();
        //    await DataContext.IndirectSalesOrderTransaction.DeleteFromQueryAsync();
        //    await DataContext.IndirectSalesOrderContent.DeleteFromQueryAsync();
        //    await DataContext.IndirectSalesOrderPromotion.DeleteFromQueryAsync();
        //    await DataContext.IndirectSalesOrder.DeleteFromQueryAsync();
        //    await DataContext.PriceListItemMapping.DeleteFromQueryAsync();
        //    await DataContext.PriceListItemHistory.DeleteFromQueryAsync();
        //    await DataContext.PriceListStoreGroupingMapping.DeleteFromQueryAsync();
        //    await DataContext.PriceListStoreMapping.DeleteFromQueryAsync();
        //    await DataContext.PriceListStoreTypeMapping.DeleteFromQueryAsync();
        //    await DataContext.PriceList.DeleteFromQueryAsync();
        //    await DataContext.KpiItemContentKpiCriteriaItemMapping.DeleteFromQueryAsync();
        //    await DataContext.KpiItemContent.DeleteFromQueryAsync();
        //    await DataContext.KpiItem.DeleteFromQueryAsync();
        //    await DataContext.ItemHistory.DeleteFromQueryAsync();
        //    await DataContext.ItemImageMapping.DeleteFromQueryAsync();
        //    await DataContext.InventoryHistory.DeleteFromQueryAsync();
        //    await DataContext.Inventory.DeleteFromQueryAsync();
        //    await DataContext.Item.DeleteFromQueryAsync();
        //    await DataContext.PromotionCodeProductMapping.DeleteFromQueryAsync();
        //    await DataContext.PromotionCodeOrganizationMapping.DeleteFromQueryAsync();
        //    await DataContext.PromotionCodeStoreMapping.DeleteFromQueryAsync();
        //    await DataContext.PromotionCodeHistory.DeleteFromQueryAsync();
        //    await DataContext.PromotionCode.DeleteFromQueryAsync();
        //    await DataContext.Variation.DeleteFromQueryAsync();
        //    await DataContext.VariationGrouping.DeleteFromQueryAsync();
        //    await DataContext.ProductProductGroupingMapping.DeleteFromQueryAsync();
        //    await DataContext.ProductImageMapping.DeleteFromQueryAsync();
        //    await DataContext.Product.DeleteFromQueryAsync();
        //    await DataContext.ProductGrouping.DeleteFromQueryAsync();
        //    await DataContext.ProductType.DeleteFromQueryAsync();
        //    await DataContext.Brand.DeleteFromQueryAsync();
        //    await DataContext.Supplier.DeleteFromQueryAsync();
        //    await DataContext.UnitOfMeasureGroupingContent.DeleteFromQueryAsync();
        //    await DataContext.UnitOfMeasureGrouping.DeleteFromQueryAsync();
        //    await DataContext.UnitOfMeasure.DeleteFromQueryAsync();
        //    await DataContext.TaxType.DeleteFromQueryAsync();
        //    return Ok();
        //}

        private void InitPosition(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/position/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Position>> RestResponse = RestClient.Post<List<Position>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<PositionDAO> PositionInDB = DataContext.Position.AsNoTracking().ToList();
                List<Position> Positions = RestResponse.Data;
                foreach (Position Position in Positions)
                {
                    PositionDAO PositionDAO = PositionInDB.Where(x => x.Id == Position.Id).FirstOrDefault();
                    if (PositionDAO == null)
                    {
                        PositionDAO = new PositionDAO
                        {
                            Id = Position.Id,
                        };
                        PositionInDB.Add(PositionDAO);
                    }
                    PositionDAO.Code = Position.Code;
                    PositionDAO.Name = Position.Name;
                    PositionDAO.StatusId = Position.StatusId;
                    PositionDAO.CreatedAt = Position.CreatedAt;
                    PositionDAO.UpdatedAt = Position.UpdatedAt;
                    PositionDAO.DeletedAt = Position.DeletedAt;
                    PositionDAO.RowId = Position.RowId;
                }
                DataContext.Position.BulkMerge(PositionInDB);
            }
        }

        private void InitOrganization(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/organization/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Organization>> RestResponse = RestClient.Post<List<Organization>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<OrganizationDAO> OrganizationInDB = DataContext.Organization.AsNoTracking().ToList();
                List<Organization> Organizations = RestResponse.Data;
                foreach (Organization Organization in Organizations)
                {
                    OrganizationDAO OrganizationDAO = OrganizationInDB.Where(x => x.Id == Organization.Id).FirstOrDefault();
                    if (OrganizationDAO == null)
                    {
                        OrganizationDAO = new OrganizationDAO
                        {
                            Id = Organization.Id,
                        };
                        OrganizationInDB.Add(OrganizationDAO);
                    }
                    OrganizationDAO.Code = Organization.Code;
                    OrganizationDAO.Name = Organization.Name;
                    OrganizationDAO.Path = Organization.Path;
                    OrganizationDAO.Level = Organization.Level;
                    OrganizationDAO.Address = Organization.Address;
                    OrganizationDAO.Email = Organization.Email;
                    OrganizationDAO.ParentId = Organization.ParentId;
                    OrganizationDAO.StatusId = Organization.StatusId;
                    OrganizationDAO.CreatedAt = Organization.CreatedAt;
                    OrganizationDAO.UpdatedAt = Organization.UpdatedAt;
                    OrganizationDAO.DeletedAt = Organization.DeletedAt;
                    OrganizationDAO.RowId = Organization.RowId;

                }
                DataContext.Organization.BulkMerge(OrganizationInDB);
            }

        }

        private void InitAppUser(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/app-user/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<AppUser>> RestResponse = RestClient.Post<List<AppUser>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<AppUserDAO> AppUserInDB = DataContext.AppUser.AsNoTracking().ToList();
                List<AppUser> AppUsers = RestResponse.Data;
                foreach (AppUser AppUser in AppUsers)
                {
                    AppUserDAO AppUserDAO = AppUserInDB.Where(x => x.Id == AppUser.Id).FirstOrDefault();
                    if (AppUserDAO == null)
                    {
                        AppUserDAO = new AppUserDAO
                        {
                            Id = AppUser.Id,

                        };
                        AppUserInDB.Add(AppUserDAO);
                    }
                    AppUserDAO.Username = AppUser.Username;
                    AppUserDAO.DisplayName = AppUser.DisplayName;
                    AppUserDAO.Address = AppUser.Address;
                    AppUserDAO.Email = AppUser.Email;
                    AppUserDAO.Phone = AppUser.Phone;
                    AppUserDAO.ProvinceId = AppUser.ProvinceId;
                    AppUserDAO.PositionId = AppUser.PositionId;
                    AppUserDAO.Department = AppUser.Department;
                    AppUserDAO.OrganizationId = AppUser.OrganizationId;
                    AppUserDAO.SexId = AppUser.SexId;
                    AppUserDAO.StatusId = AppUser.StatusId;
                    AppUserDAO.CreatedAt = AppUser.CreatedAt;
                    AppUserDAO.UpdatedAt = AppUser.UpdatedAt;
                    AppUserDAO.DeletedAt = AppUser.DeletedAt;
                    AppUserDAO.Avatar = AppUser.Avatar;
                    AppUserDAO.Birthday = AppUser.Birthday;
                    AppUserDAO.RowId = AppUser.RowId;
                }
                DataContext.AppUser.BulkMerge(AppUserInDB);
            }
        }

        private void InitProvince(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/province/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Province>> RestResponse = RestClient.Post<List<Province>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<ProvinceDAO> ProvinceInDB = DataContext.Province.AsNoTracking().ToList();
                List<Province> Provinces = RestResponse.Data;
                foreach (Province Province in Provinces)
                {
                    ProvinceDAO ProvinceDAO = ProvinceInDB.Where(x => x.Id == Province.Id).FirstOrDefault();
                    if (ProvinceDAO == null)
                    {
                        ProvinceDAO = new ProvinceDAO
                        {
                            Id = Province.Id,
                        };
                        ProvinceInDB.Add(ProvinceDAO);
                    }
                    ProvinceDAO.Code = Province.Code;
                    ProvinceDAO.Name = Province.Name;
                    ProvinceDAO.StatusId = Province.StatusId;
                    ProvinceDAO.CreatedAt = Province.CreatedAt;
                    ProvinceDAO.UpdatedAt = Province.UpdatedAt;
                    ProvinceDAO.DeletedAt = Province.DeletedAt;
                    ProvinceDAO.RowId = Province.RowId;
                }
                DataContext.Province.BulkMerge(ProvinceInDB);
            }
        }

        private void InitDistrict(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/district/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<District>> RestResponse = RestClient.Post<List<District>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<DistrictDAO> DistrictInDB = DataContext.District.AsNoTracking().ToList();
                List<District> Districts = RestResponse.Data;
                foreach (District District in Districts)
                {
                    DistrictDAO DistrictDAO = DistrictInDB.Where(x => x.Id == District.Id).FirstOrDefault();
                    if (DistrictDAO == null)
                    {
                        DistrictDAO = new DistrictDAO
                        {
                            Id = District.Id,
                        };
                        DistrictInDB.Add(DistrictDAO);
                    }
                    DistrictDAO.Code = District.Code;
                    DistrictDAO.Name = District.Name;
                    DistrictDAO.ProvinceId = District.ProvinceId;
                    DistrictDAO.StatusId = District.StatusId;
                    DistrictDAO.CreatedAt = District.CreatedAt;
                    DistrictDAO.UpdatedAt = District.UpdatedAt;
                    DistrictDAO.DeletedAt = District.DeletedAt;
                    DistrictDAO.RowId = District.RowId;
                }
                DataContext.District.BulkMerge(DistrictInDB);
            }
        }

        private void InitWard(RestClient RestClient)
        {
            IRestRequest RestRequest = new RestRequest("rpc/es/ward/list");
            RestRequest.Method = Method.POST;
            IRestResponse<List<Ward>> RestResponse = RestClient.Post<List<Ward>>(RestRequest);
            if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<WardDAO> WardInDB = DataContext.Ward.AsNoTracking().ToList();
                List<Ward> Wards = RestResponse.Data;
                foreach (Ward Ward in Wards)
                {
                    WardDAO WardDAO = WardInDB.Where(x => x.Id == Ward.Id).FirstOrDefault();
                    if (WardDAO == null)
                    {
                        WardDAO = new WardDAO
                        {
                            Id = Ward.Id,
                        };
                        WardInDB.Add(WardDAO);
                    }
                    WardDAO.Code = Ward.Code;
                    WardDAO.Name = Ward.Name;
                    WardDAO.DistrictId = Ward.DistrictId;
                    WardDAO.StatusId = Ward.StatusId;
                    WardDAO.CreatedAt = Ward.CreatedAt;
                    WardDAO.UpdatedAt = Ward.UpdatedAt;
                    WardDAO.DeletedAt = Ward.DeletedAt;
                    WardDAO.RowId = Ward.RowId;
                }
                DataContext.Ward.BulkMerge(WardInDB);
            }
        }

        private void InitProductType(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/product-type/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/product-type/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    ProductTypeFilter ProductTypeFilter = new ProductTypeFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(ProductTypeFilter);
                    IRestResponse<List<ProductType>> RestResponse = RestClient.Post<List<ProductType>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<ProductTypeDAO> ProductTypeInDB = DataContext.ProductType.AsNoTracking().ToList();
                        List<ProductType> ProductTypes = RestResponse.Data;
                        foreach (ProductType ProductType in ProductTypes)
                        {
                            ProductTypeDAO ProductTypeDAO = ProductTypeInDB.Where(x => x.Id == ProductType.Id).FirstOrDefault();
                            if (ProductTypeDAO == null)
                            {
                                ProductTypeDAO = new ProductTypeDAO
                                {
                                    Id = ProductType.Id,
                                };
                                ProductTypeInDB.Add(ProductTypeDAO);
                            }
                            ProductTypeDAO.Code = ProductType.Code;
                            ProductTypeDAO.Name = ProductType.Name;
                            ProductTypeDAO.Description = ProductType.Description;
                            ProductTypeDAO.StatusId = ProductType.StatusId;
                            ProductTypeDAO.CreatedAt = ProductType.CreatedAt;
                            ProductTypeDAO.UpdatedAt = ProductType.UpdatedAt;
                            ProductTypeDAO.DeletedAt = ProductType.DeletedAt;
                            ProductTypeDAO.Used = ProductType.Used;
                            ProductTypeDAO.RowId = ProductType.RowId;
                        }
                        DataContext.ProductType.BulkMerge(ProductTypeInDB);
                    }
                }
            }
        }

        private void InitProductGrouping(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/product-grouping/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/product-grouping/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(ProductGroupingFilter);
                    IRestResponse<List<ProductGrouping>> RestResponse = RestClient.Post<List<ProductGrouping>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<ProductGroupingDAO> ProductGroupingInDB = DataContext.ProductGrouping.AsNoTracking().ToList();
                        List<ProductGrouping> ProductGroupings = RestResponse.Data;
                        foreach (ProductGrouping ProductGrouping in ProductGroupings)
                        {
                            ProductGroupingDAO ProductGroupingDAO = ProductGroupingInDB.Where(x => x.Id == ProductGrouping.Id).FirstOrDefault();
                            if (ProductGroupingDAO == null)
                            {
                                ProductGroupingDAO = new ProductGroupingDAO
                                {
                                    Id = ProductGrouping.Id,
                                };
                                ProductGroupingInDB.Add(ProductGroupingDAO);
                            }
                            ProductGroupingDAO.Code = ProductGrouping.Code;
                            ProductGroupingDAO.Name = ProductGrouping.Name;
                            ProductGroupingDAO.Description = ProductGrouping.Description;
                            ProductGroupingDAO.Level = ProductGrouping.Level;
                            ProductGroupingDAO.ParentId = ProductGrouping.ParentId;
                            ProductGroupingDAO.Path = ProductGrouping.Path;
                            ProductGroupingDAO.CreatedAt = ProductGrouping.CreatedAt;
                            ProductGroupingDAO.UpdatedAt = ProductGrouping.UpdatedAt;
                            ProductGroupingDAO.DeletedAt = ProductGrouping.DeletedAt;
                            ProductGroupingDAO.RowId = ProductGrouping.RowId;
                        }
                        DataContext.ProductGrouping.BulkMerge(ProductGroupingInDB);
                    }
                }
            }
        }

        private void InitCategory(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/category/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/category/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    CategoryFilter CategoryFilter = new CategoryFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(CategoryFilter);
                    IRestResponse<List<Category>> RestResponse = RestClient.Post<List<Category>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<CategoryDAO> CategoryInDB = DataContext.Category.AsNoTracking().ToList();
                        List<ImageDAO> ImageDAOs = new List<ImageDAO>();
                        List<Category> Categorys = RestResponse.Data;
                        foreach (Category Category in Categorys)
                        {
                            CategoryDAO CategoryDAO = CategoryInDB.Where(x => x.Id == Category.Id).FirstOrDefault();
                            if (CategoryDAO == null)
                            {
                                CategoryDAO = new CategoryDAO
                                {
                                    Id = Category.Id,
                                };
                                CategoryInDB.Add(CategoryDAO);
                            }
                            CategoryDAO.Code = Category.Code;
                            CategoryDAO.Name = Category.Name;
                            CategoryDAO.StatusId = Category.StatusId;
                            CategoryDAO.Level = Category.Level;
                            CategoryDAO.ParentId = Category.ParentId;
                            CategoryDAO.ImageId = Category.ImageId;
                            CategoryDAO.Path = Category.Path;
                            CategoryDAO.CreatedAt = Category.CreatedAt;
                            CategoryDAO.UpdatedAt = Category.UpdatedAt;
                            CategoryDAO.DeletedAt = Category.DeletedAt;
                            CategoryDAO.RowId = Category.RowId;
                            CategoryDAO.Used = Category.Used;

                            if (Category.Image != null)
                            {
                                ImageDAOs.Add(new ImageDAO
                                {
                                    Id = Category.Image.Id,
                                    Url = Category.Image.Url,
                                    ThumbnailUrl = Category.Image.ThumbnailUrl,
                                    RowId = Category.Image.RowId,
                                    Name = Category.Image.Name,
                                    CreatedAt = Category.Image.CreatedAt,
                                    UpdatedAt = Category.Image.UpdatedAt,
                                    DeletedAt = Category.Image.DeletedAt,
                                });
                            }
                        }
                        DataContext.Image.BulkMerge(ImageDAOs);
                        DataContext.Category.BulkMerge(CategoryInDB);
                    }
                }
            }
        }

        private void InitSupplier(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/supplier/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/supplier/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    SupplierFilter SupplierFilter = new SupplierFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(SupplierFilter);
                    IRestResponse<List<Supplier>> RestResponse = RestClient.Post<List<Supplier>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<SupplierDAO> SupplierInDB = DataContext.Supplier.AsNoTracking().ToList();
                        List<Supplier> Suppliers = RestResponse.Data;
                        foreach (Supplier Supplier in Suppliers)
                        {
                            SupplierDAO SupplierDAO = SupplierInDB.Where(x => x.Id == Supplier.Id).FirstOrDefault();
                            if (SupplierDAO == null)
                            {
                                SupplierDAO = new SupplierDAO
                                {
                                    Id = Supplier.Id,
                                };
                                SupplierInDB.Add(SupplierDAO);
                            }
                            SupplierDAO.Code = Supplier.Code;
                            SupplierDAO.Name = Supplier.Name;
                            SupplierDAO.Description = Supplier.Description;
                            SupplierDAO.TaxCode = Supplier.TaxCode;
                            SupplierDAO.Phone = Supplier.Phone;
                            SupplierDAO.Email = Supplier.Email;
                            SupplierDAO.Address = Supplier.Address;
                            SupplierDAO.NationId = Supplier.NationId;
                            SupplierDAO.ProvinceId = Supplier.ProvinceId;
                            SupplierDAO.DistrictId = Supplier.DistrictId;
                            SupplierDAO.WardId = Supplier.WardId;
                            SupplierDAO.OwnerName = Supplier.OwnerName;
                            SupplierDAO.PersonInChargeId = Supplier.PersonInChargeId;
                            SupplierDAO.StatusId = Supplier.StatusId;
                            SupplierDAO.CreatedAt = Supplier.CreatedAt;
                            SupplierDAO.UpdatedAt = Supplier.UpdatedAt;
                            SupplierDAO.DeletedAt = Supplier.DeletedAt;
                            SupplierDAO.Used = Supplier.Used;
                            SupplierDAO.RowId = Supplier.RowId;
                        }
                        DataContext.Supplier.BulkMerge(SupplierInDB);
                    }
                }
            }
        }

        private void InitBrand(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/brand/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/brand/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    BrandFilter BrandFilter = new BrandFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(BrandFilter);
                    IRestResponse<List<Brand>> RestResponse = RestClient.Post<List<Brand>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<BrandDAO> BrandInDB = DataContext.Brand.AsNoTracking().ToList();
                        List<Brand> Brands = RestResponse.Data;
                        foreach (Brand Brand in Brands)
                        {
                            BrandDAO BrandDAO = BrandInDB.Where(x => x.Id == Brand.Id).FirstOrDefault();
                            if (BrandDAO == null)
                            {
                                BrandDAO = new BrandDAO
                                {
                                    Id = Brand.Id,
                                };
                                BrandInDB.Add(BrandDAO);
                            }
                            BrandDAO.Code = Brand.Code;
                            BrandDAO.Name = Brand.Name;
                            BrandDAO.Description = Brand.Description;
                            BrandDAO.StatusId = Brand.StatusId;
                            BrandDAO.CreatedAt = Brand.CreatedAt;
                            BrandDAO.UpdatedAt = Brand.UpdatedAt;
                            BrandDAO.DeletedAt = Brand.DeletedAt;
                            BrandDAO.RowId = Brand.RowId;
                            BrandDAO.Used = Brand.Used;
                        }
                        DataContext.Brand.BulkMerge(BrandInDB);
                    }
                }
            }
        }

        private void InitUnitOfMeasure(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/unit-of-measure/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/unit-of-measure/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(UnitOfMeasureFilter);
                    IRestResponse<List<UnitOfMeasure>> RestResponse = RestClient.Post<List<UnitOfMeasure>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<UnitOfMeasureDAO> UnitOfMeasureInDB = DataContext.UnitOfMeasure.AsNoTracking().ToList();
                        List<UnitOfMeasure> UnitOfMeasures = RestResponse.Data;
                        foreach (UnitOfMeasure UnitOfMeasure in UnitOfMeasures)
                        {
                            UnitOfMeasureDAO UnitOfMeasureDAO = UnitOfMeasureInDB.Where(x => x.Id == UnitOfMeasure.Id).FirstOrDefault();
                            if (UnitOfMeasureDAO == null)
                            {
                                UnitOfMeasureDAO = new UnitOfMeasureDAO
                                {
                                    Id = UnitOfMeasure.Id,
                                };
                                UnitOfMeasureInDB.Add(UnitOfMeasureDAO);
                            }
                            UnitOfMeasureDAO.Code = UnitOfMeasure.Code;
                            UnitOfMeasureDAO.Name = UnitOfMeasure.Name;
                            UnitOfMeasureDAO.Description = UnitOfMeasure.Description;
                            UnitOfMeasureDAO.StatusId = UnitOfMeasure.StatusId;
                            UnitOfMeasureDAO.CreatedAt = UnitOfMeasure.CreatedAt;
                            UnitOfMeasureDAO.UpdatedAt = UnitOfMeasure.UpdatedAt;
                            UnitOfMeasureDAO.DeletedAt = UnitOfMeasure.DeletedAt;
                            UnitOfMeasureDAO.RowId = UnitOfMeasure.RowId;
                        }
                        DataContext.UnitOfMeasure.BulkMerge(UnitOfMeasureInDB);
                    }
                }
            }
        }

        private void InitUnitOfMeasureGrouping(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/unit-of-measure-grouping/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/unit-of-measure-grouping/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(UnitOfMeasureGroupingFilter);
                    IRestResponse<List<UnitOfMeasureGrouping>> RestResponse = RestClient.Post<List<UnitOfMeasureGrouping>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<UnitOfMeasureGroupingDAO> UnitOfMeasureGroupingInDB = DataContext.UnitOfMeasureGrouping.AsNoTracking().ToList();
                        List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = RestResponse.Data;
                        foreach (UnitOfMeasureGrouping UnitOfMeasureGrouping in UnitOfMeasureGroupings)
                        {
                            UnitOfMeasureGroupingDAO UnitOfMeasureGroupingDAO = UnitOfMeasureGroupingInDB.Where(x => x.Id == UnitOfMeasureGrouping.Id).FirstOrDefault();
                            if (UnitOfMeasureGroupingDAO == null)
                            {
                                UnitOfMeasureGroupingDAO = new UnitOfMeasureGroupingDAO
                                {
                                    Id = UnitOfMeasureGrouping.Id,
                                };
                                UnitOfMeasureGroupingInDB.Add(UnitOfMeasureGroupingDAO);
                            }
                            UnitOfMeasureGroupingDAO.Code = UnitOfMeasureGrouping.Code;
                            UnitOfMeasureGroupingDAO.Name = UnitOfMeasureGrouping.Name;
                            UnitOfMeasureGroupingDAO.Description = UnitOfMeasureGrouping.Description;
                            UnitOfMeasureGroupingDAO.StatusId = UnitOfMeasureGrouping.StatusId;
                            UnitOfMeasureGroupingDAO.UnitOfMeasureId = UnitOfMeasureGrouping.UnitOfMeasureId;
                            UnitOfMeasureGroupingDAO.CreatedAt = UnitOfMeasureGrouping.CreatedAt;
                            UnitOfMeasureGroupingDAO.UpdatedAt = UnitOfMeasureGrouping.UpdatedAt;
                            UnitOfMeasureGroupingDAO.DeletedAt = UnitOfMeasureGrouping.DeletedAt;
                            UnitOfMeasureGroupingDAO.RowId = UnitOfMeasureGrouping.RowId;
                        }
                        DataContext.UnitOfMeasureGrouping.BulkMerge(UnitOfMeasureGroupingInDB);

                        List<UnitOfMeasureGroupingContentDAO> UnitOfMeasureGroupingContentInDB = DataContext.UnitOfMeasureGroupingContent.AsNoTracking().ToList();
                        List<UnitOfMeasureGroupingContent> UnitOfMeasureGroupingContents = UnitOfMeasureGroupings.Where(x => x.UnitOfMeasureGroupingContents != null).SelectMany(x => x.UnitOfMeasureGroupingContents).ToList();
                        foreach (UnitOfMeasureGroupingContent UnitOfMeasureGroupingContent in UnitOfMeasureGroupingContents)
                        {
                            UnitOfMeasureGroupingContentDAO UnitOfMeasureGroupingContentDAO = UnitOfMeasureGroupingContentInDB.Where(x => x.Id == UnitOfMeasureGroupingContent.Id).FirstOrDefault();
                            if (UnitOfMeasureGroupingContentDAO == null)
                            {
                                UnitOfMeasureGroupingContentDAO = new UnitOfMeasureGroupingContentDAO()
                                {
                                    Id = UnitOfMeasureGroupingContent.Id
                                };
                                UnitOfMeasureGroupingContentInDB.Add(UnitOfMeasureGroupingContentDAO);
                            }
                            UnitOfMeasureGroupingContentDAO.Factor = UnitOfMeasureGroupingContent.Factor;
                            UnitOfMeasureGroupingContentDAO.UnitOfMeasureGroupingId = UnitOfMeasureGroupingContent.UnitOfMeasureGroupingId;
                            UnitOfMeasureGroupingContentDAO.UnitOfMeasureId = UnitOfMeasureGroupingContent.UnitOfMeasureId;
                        }
                        DataContext.UnitOfMeasureGroupingContent.BulkMerge(UnitOfMeasureGroupingContentInDB);
                    }
                }
            }
        }

        private void InitTaxType(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/tax-type/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/tax-type/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    TaxTypeFilter TaxTypeFilter = new TaxTypeFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(TaxTypeFilter);
                    IRestResponse<List<TaxType>> RestResponse = RestClient.Post<List<TaxType>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<TaxTypeDAO> TaxTypeInDB = DataContext.TaxType.AsNoTracking().ToList();
                        List<TaxType> TaxTypes = RestResponse.Data;
                        foreach (TaxType TaxType in TaxTypes)
                        {
                            TaxTypeDAO TaxTypeDAO = TaxTypeInDB.Where(x => x.Id == TaxType.Id).FirstOrDefault();
                            if (TaxTypeDAO == null)
                            {
                                TaxTypeDAO = new TaxTypeDAO
                                {
                                    Id = TaxType.Id,
                                };
                                TaxTypeInDB.Add(TaxTypeDAO);
                            }
                            TaxTypeDAO.Code = TaxType.Code;
                            TaxTypeDAO.Name = TaxType.Name;
                            TaxTypeDAO.Percentage = TaxType.Percentage;
                            TaxTypeDAO.StatusId = TaxType.StatusId;
                            TaxTypeDAO.CreatedAt = TaxType.CreatedAt;
                            TaxTypeDAO.UpdatedAt = TaxType.UpdatedAt;
                            TaxTypeDAO.DeletedAt = TaxType.DeletedAt;
                            TaxTypeDAO.RowId = TaxType.RowId;
                        }
                        DataContext.TaxType.BulkMerge(TaxTypeInDB);
                    }
                }
            }
        }

        private void InitProduct(RestClient RestClient)
        {
            IRestRequest CountRequest = new RestRequest("rpc/es/product/count");
            CountRequest.RequestFormat = DataFormat.Json;
            IRestResponse<long> CountResponse = RestClient.Post<long>(CountRequest);
            if (CountResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                long count = CountResponse.Data;
                var BatchCounter = (count / 10000) + 1;

                for (int i = 0; i < BatchCounter; i++)
                {
                    IRestRequest ListRequest = new RestRequest("rpc/es/product/list");
                    ListRequest.RequestFormat = DataFormat.Json;
                    ProductFilter ProductFilter = new ProductFilter
                    {
                        Skip = i * 10000,
                        Take = 10000
                    };
                    ListRequest.Method = Method.POST;
                    ListRequest.AddJsonBody(ProductFilter);
                    IRestResponse<List<Product>> RestResponse = RestClient.Post<List<Product>>(ListRequest);
                    if (RestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<ProductDAO> ProductDAOs = new List<ProductDAO>();
                        List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
                        List<ProductImageMappingDAO> ProductImageMappingDAOs = new List<ProductImageMappingDAO>();
                        List<VariationGroupingDAO> VariationGroupingDAOs = new List<VariationGroupingDAO>();
                        List<VariationDAO> VariationDAOs = new List<VariationDAO>();
                        List<ItemDAO> ItemDAOs = new List<ItemDAO>();
                        List<ImageDAO> ImageDAOs = new List<ImageDAO>();
                        List<ItemImageMappingDAO> ItemImageMappingDAOs = new List<ItemImageMappingDAO>();

                        List<Product> Products = RestResponse.Data;
                        List<ProductProductGroupingMapping> ProductProductGroupingMappings = Products.Where(x => x.ProductProductGroupingMappings != null).SelectMany(x => x.ProductProductGroupingMappings).ToList();
                        List<ProductImageMapping> ProductImageMappings = Products.Where(x => x.ProductImageMappings != null).SelectMany(x => x.ProductImageMappings).ToList();
                        List<VariationGrouping> VariationGroupings = Products.Where(x => x.VariationGroupings != null).SelectMany(x => x.VariationGroupings).ToList();
                        List<Variation> Variations = VariationGroupings.Where(x => x.Variations != null).SelectMany(x => x.Variations).ToList();
                        List<Item> Items = Products.Where(x => x.Items != null).SelectMany(x => x.Items).ToList();
                        List<ItemImageMapping> ItemImageMappings = Items.Where(x => x.ItemImageMappings != null).SelectMany(x => x.ItemImageMappings).ToList();

                        foreach (Product Product in Products)
                        {
                            ProductDAO ProductDAO = new ProductDAO();
                            ProductDAO.Id = Product.Id;
                            ProductDAO.Code = Product.Code;
                            ProductDAO.Name = Product.Name;
                            ProductDAO.Description = Product.Description;
                            ProductDAO.ScanCode = Product.ScanCode;
                            ProductDAO.ERPCode = Product.ERPCode;
                            ProductDAO.CategoryId = Product.CategoryId;
                            ProductDAO.ProductTypeId = Product.ProductTypeId;
                            ProductDAO.BrandId = Product.BrandId;
                            ProductDAO.UnitOfMeasureId = Product.UnitOfMeasureId;
                            ProductDAO.UnitOfMeasureGroupingId = Product.UnitOfMeasureGroupingId;
                            ProductDAO.SalePrice = Product.SalePrice;
                            ProductDAO.RetailPrice = Product.RetailPrice;
                            ProductDAO.TaxTypeId = Product.TaxTypeId;
                            ProductDAO.StatusId = Product.StatusId;
                            ProductDAO.OtherName = Product.OtherName;
                            ProductDAO.TechnicalName = Product.TechnicalName;
                            ProductDAO.IsNew = Product.IsNew;
                            ProductDAO.UsedVariationId = Product.UsedVariationId;
                            ProductDAO.CreatedAt = Product.CreatedAt;
                            ProductDAO.UpdatedAt = Product.UpdatedAt;
                            ProductDAO.DeletedAt = Product.DeletedAt;
                            ProductDAO.RowId = Product.RowId;
                            ProductDAO.Used = Product.Used;
                            ProductDAOs.Add(ProductDAO);
                        }
                        DataContext.Product.BulkMerge(ProductDAOs);

                        foreach (var ProductImageMapping in ProductImageMappings)
                        {
                            ProductImageMappingDAO ProductImageMappingDAO = new ProductImageMappingDAO
                            {
                                ProductId = ProductImageMapping.ProductId,
                                ImageId = ProductImageMapping.ImageId,
                            };
                            ProductImageMappingDAOs.Add(ProductImageMappingDAO);
                            ImageDAOs.Add(new ImageDAO
                            {
                                Id = ProductImageMapping.Image.Id,
                                Url = ProductImageMapping.Image.Url,
                                ThumbnailUrl = ProductImageMapping.Image.ThumbnailUrl,
                                RowId = ProductImageMapping.Image.RowId,
                                Name = ProductImageMapping.Image.Name,
                                CreatedAt = ProductImageMapping.Image.CreatedAt,
                                UpdatedAt = ProductImageMapping.Image.UpdatedAt,
                                DeletedAt = ProductImageMapping.Image.DeletedAt,
                            });
                        }

                        foreach (ProductProductGroupingMapping ProductProductGroupingMapping in ProductProductGroupingMappings)
                        {
                            ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO();
                            ProductProductGroupingMappingDAO.ProductId = ProductProductGroupingMapping.ProductId;
                            ProductProductGroupingMappingDAO.ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId;
                            ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
                        }
                        DataContext.ProductProductGroupingMapping.BulkMerge(ProductProductGroupingMappingDAOs);

                        foreach (VariationGrouping VariationGrouping in VariationGroupings)
                        {
                            VariationGroupingDAO VariationGroupingDAO = new VariationGroupingDAO();
                            VariationGroupingDAO.Id = VariationGrouping.Id;
                            VariationGroupingDAO.Name = VariationGrouping.Name;
                            VariationGroupingDAO.ProductId = VariationGrouping.ProductId;
                            VariationGroupingDAO.CreatedAt = VariationGrouping.CreatedAt;
                            VariationGroupingDAO.UpdatedAt = VariationGrouping.UpdatedAt;
                            VariationGroupingDAO.DeletedAt = VariationGrouping.DeletedAt;
                            VariationGroupingDAO.Used = VariationGrouping.Used;
                            VariationGroupingDAO.RowId = VariationGrouping.RowId;
                            VariationGroupingDAOs.Add(VariationGroupingDAO);
                        }
                        DataContext.VariationGrouping.BulkMerge(VariationGroupingDAOs);

                        foreach (Variation Variation in Variations)
                        {
                            VariationDAO VariationDAO = new VariationDAO();
                            VariationDAO.Id = Variation.Id;
                            VariationDAO.Code = Variation.Code;
                            VariationDAO.Name = Variation.Name;
                            VariationDAO.VariationGroupingId = Variation.VariationGroupingId;
                            VariationDAO.CreatedAt = Variation.CreatedAt;
                            VariationDAO.UpdatedAt = Variation.UpdatedAt;
                            VariationDAO.DeletedAt = Variation.DeletedAt;
                            VariationDAO.RowId = Variation.RowId;
                            VariationDAO.Used = Variation.Used;
                            VariationDAOs.Add(VariationDAO);
                        }
                        DataContext.Variation.BulkMerge(VariationDAOs);

                        foreach (Item Item in Items)
                        {
                            ItemDAO ItemDAO = new ItemDAO();
                            ItemDAO.Id = Item.Id;
                            ItemDAO.Code = Item.Code;
                            ItemDAO.Name = Item.Name;
                            ItemDAO.ProductId = Item.ProductId;
                            ItemDAO.ScanCode = Item.ScanCode;
                            ItemDAO.SalePrice = Item.SalePrice;
                            ItemDAO.RetailPrice = Item.RetailPrice;
                            ItemDAO.StatusId = Item.StatusId;
                            ItemDAO.CreatedAt = Item.CreatedAt;
                            ItemDAO.UpdatedAt = Item.UpdatedAt;
                            ItemDAO.DeletedAt = Item.DeletedAt;
                            ItemDAO.RowId = Item.RowId;
                            ItemDAO.Used = Item.Used;
                            ItemDAOs.Add(ItemDAO);
                        }
                        DataContext.Item.BulkMerge(ItemDAOs);

                        foreach (var ItemImageMapping in ItemImageMappings)
                        {
                            ItemImageMappingDAO ItemImageMappingDAO = new ItemImageMappingDAO
                            {
                                ItemId = ItemImageMapping.ItemId,
                                ImageId = ItemImageMapping.ImageId
                            };
                            ItemImageMappingDAOs.Add(ItemImageMappingDAO);
                            ImageDAOs.Add(new ImageDAO
                            {
                                Id = ItemImageMapping.Image.Id,
                                Url = ItemImageMapping.Image.Url,
                                ThumbnailUrl = ItemImageMapping.Image.ThumbnailUrl,
                                RowId = ItemImageMapping.Image.RowId,
                                Name = ItemImageMapping.Image.Name,
                                CreatedAt = ItemImageMapping.Image.CreatedAt,
                                UpdatedAt = ItemImageMapping.Image.UpdatedAt,
                                DeletedAt = ItemImageMapping.Image.DeletedAt,
                            });
                        }
                        DataContext.Image.BulkMerge(ImageDAOs);
                        DataContext.ProductImageMapping.BulkMerge(ProductImageMappingDAOs);
                        DataContext.ItemImageMapping.BulkMerge(ItemImageMappingDAOs);
                    }
                }
            }
        }
        #endregion

        #region permission
        private ActionResult InitRoute()
        {
            List<Type> routeTypes = typeof(SetupController).Assembly.GetTypes()
               .Where(x => typeof(Root).IsAssignableFrom(x) && x.IsClass && x.Name != "Root")
               .ToList();

            InitMenu(routeTypes);
            InitPage(routeTypes);
            InitField(routeTypes);
            InitAction(routeTypes);

            DataContext.ActionPageMapping.Where(ap => ap.Action.IsDeleted || ap.Page.IsDeleted).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(ap => ap.Action.IsDeleted).DeleteFromQuery();
            DataContext.Action.Where(p => p.IsDeleted || p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Page.Where(p => p.IsDeleted).DeleteFromQuery();
            DataContext.PermissionContent.Where(f => f.Field.IsDeleted == true || f.Field.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Field.Where(pf => pf.IsDeleted || pf.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Permission.Where(p => p.Menu.IsDeleted).DeleteFromQuery();
            DataContext.Menu.Where(v => v.IsDeleted).DeleteFromQuery();
            return Ok();
        }

        private ActionResult InitAdmin()
        {
            RoleDAO Admin = DataContext.Role
               .Where(r => r.Name == "ADMIN")
               .FirstOrDefault();
            if (Admin == null)
            {
                Admin = new RoleDAO
                {
                    Name = "ADMIN",
                    Code = "ADMIN",
                    StatusId = StatusEnum.ACTIVE.Id,
                };
                DataContext.Role.Add(Admin);
                DataContext.SaveChanges();
            }

            AppUserDAO AppUser = DataContext.AppUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower())
                .FirstOrDefault();
            if (AppUser == null)
            {
                return Ok();
            }

            AppUserRoleMappingDAO AppUserRoleMappingDAO = DataContext.AppUserRoleMapping
                .Where(ur => ur.RoleId == Admin.Id && ur.AppUserId == AppUser.Id)
                .FirstOrDefault();
            if (AppUserRoleMappingDAO == null)
            {
                AppUserRoleMappingDAO = new AppUserRoleMappingDAO
                {
                    AppUserId = AppUser.Id,
                    RoleId = Admin.Id,
                };
                DataContext.AppUserRoleMapping.Add(AppUserRoleMappingDAO);
                DataContext.SaveChanges();
            }

            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking()
                .Include(v => v.Actions)
                .ToList();
            List<PermissionDAO> permissions = DataContext.Permission.AsNoTracking()
                .Include(p => p.PermissionActionMappings)
                .ToList();
            foreach (MenuDAO Menu in Menus)
            {
                PermissionDAO permission = permissions
                    .Where(p => p.MenuId == Menu.Id && p.RoleId == Admin.Id)
                    .FirstOrDefault();
                if (permission == null)
                {
                    permission = new PermissionDAO
                    {
                        Code = Admin + "_" + Menu.Name,
                        Name = Admin + "_" + Menu.Name,
                        MenuId = Menu.Id,
                        RoleId = Admin.Id,
                        StatusId = StatusEnum.ACTIVE.Id,
                        PermissionActionMappings = new List<PermissionActionMappingDAO>(),
                    };
                    permissions.Add(permission);
                }
                else
                {
                    permission.StatusId = StatusEnum.ACTIVE.Id;
                    if (permission.PermissionActionMappings == null)
                        permission.PermissionActionMappings = new List<PermissionActionMappingDAO>();
                }
                foreach (ActionDAO action in Menu.Actions)
                {
                    PermissionActionMappingDAO PermissionActionMappingDAO = permission.PermissionActionMappings
                        .Where(ppm => ppm.ActionId == action.Id).FirstOrDefault();
                    if (PermissionActionMappingDAO == null)
                    {
                        PermissionActionMappingDAO = new PermissionActionMappingDAO
                        {
                            ActionId = action.Id
                        };
                        permission.PermissionActionMappings.Add(PermissionActionMappingDAO);
                    }
                }

            }
            DataContext.Permission.BulkMerge(permissions);
            permissions.ForEach(p =>
            {
                foreach (var action in p.PermissionActionMappings)
                {
                    action.PermissionId = p.Id;
                }
            });

            List<PermissionActionMappingDAO> PermissionActionMappingDAOs = permissions
                .SelectMany(p => p.PermissionActionMappings).ToList();
            DataContext.PermissionContent.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
            DataContext.PermissionActionMapping.BulkMerge(PermissionActionMappingDAOs);
            return Ok();
        }

        private void InitMenu(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            Menus.ForEach(m => m.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name && m.Name != "Root").FirstOrDefault();
                if (Menu == null)
                {
                    Menu = new MenuDAO
                    {
                        Code = type.Name,
                        Name = type.Name,
                        IsDeleted = false,
                    };
                    Menus.Add(Menu);
                }
                else
                {
                    Menu.IsDeleted = false;
                }
            }
            DataContext.BulkMerge(Menus);
        }

        private void InitPage(List<Type> routeTypes)
        {
            List<PageDAO> pages = DataContext.Page.AsNoTracking().OrderBy(p => p.Path).ToList();
            pages.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                var values = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                .Select(x => (string)x.GetRawConstantValue())
                .ToList();
                foreach (string value in values)
                {
                    PageDAO page = pages.Where(p => p.Path == value).FirstOrDefault();
                    if (page == null)
                    {
                        page = new PageDAO
                        {
                            Name = value,
                            Path = value,
                            IsDeleted = false,
                        };
                        pages.Add(page);
                    }
                    else
                    {
                        page.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(pages);
        }

        private void InitField(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            List<FieldDAO> fields = DataContext.Field.AsNoTracking().ToList();
            fields.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, long>))
                .Select(x => (Dictionary<string, long>)x.GetValue(x))
                .FirstOrDefault();
                if (value == null)
                    continue;
                foreach (var pair in value)
                {
                    FieldDAO field = fields
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (field == null)
                    {
                        field = new FieldDAO
                        {
                            MenuId = Menu.Id,
                            Name = pair.Key,
                            FieldTypeId = pair.Value,
                            IsDeleted = false,
                        };
                        fields.Add(field);
                    }
                    else
                    {
                        field.FieldTypeId = pair.Value;
                        field.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(fields);
        }
        private void InitAction(List<Type> routeTypes)
        {
            List<MenuDAO> Menus = DataContext.Menu.AsNoTracking().ToList();
            List<ActionDAO> actions = DataContext.Action.AsNoTracking().ToList();
            actions.ForEach(p => p.IsDeleted = true);
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
               .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, IEnumerable<string>>))
               .Select(x => (Dictionary<string, IEnumerable<string>>)x.GetValue(x))
               .FirstOrDefault();
                if (value == null)
                    continue;
                foreach (var pair in value)
                {
                    ActionDAO action = actions
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (action == null)
                    {
                        action = new ActionDAO
                        {
                            MenuId = Menu.Id,
                            Name = pair.Key,
                            IsDeleted = false,
                        };
                        actions.Add(action);
                    }
                    else
                    {
                        action.IsDeleted = false;
                    }
                }
            }
            DataContext.BulkMerge(actions);

            actions = DataContext.Action.Where(a => a.IsDeleted == false).AsNoTracking().ToList();
            List<PageDAO> PageDAOs = DataContext.Page.AsNoTracking().ToList();
            List<ActionPageMappingDAO> ActionPageMappingDAOs = new List<ActionPageMappingDAO>();
            foreach (Type type in routeTypes)
            {
                MenuDAO Menu = Menus.Where(m => m.Code == type.Name).FirstOrDefault();
                var value = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
               .Where(fi => !fi.IsInitOnly && fi.FieldType == typeof(Dictionary<string, IEnumerable<string>>))
               .Select(x => (Dictionary<string, IEnumerable<string>>)x.GetValue(x))
               .FirstOrDefault();
                if (value == null)
                    continue;

                foreach (var pair in value)
                {
                    ActionDAO action = actions
                        .Where(p => p.MenuId == Menu.Id && p.Name == pair.Key)
                        .FirstOrDefault();
                    if (action == null)
                        continue;
                    IEnumerable<string> pages = pair.Value;
                    foreach (string page in pages)
                    {
                        PageDAO PageDAO = PageDAOs.Where(p => p.Path == page).FirstOrDefault();
                        if (PageDAO != null)
                        {
                            if (!ActionPageMappingDAOs.Any(ap => ap.ActionId == action.Id && ap.PageId == PageDAO.Id))
                            {
                                ActionPageMappingDAOs.Add(new ActionPageMappingDAO
                                {
                                    ActionId = action.Id,
                                    PageId = PageDAO.Id
                                });
                            }
                        }
                    }
                }
            }
            ActionPageMappingDAOs = ActionPageMappingDAOs.Distinct().ToList();
            DataContext.ActionPageMapping.DeleteFromQuery();
            DataContext.BulkInsert(ActionPageMappingDAOs);
        }
        #endregion

        #region enum
        private ActionResult InitEnum()
        {
            InitStoreStatusEnum();
            InitStoreStatusHistoryTypeEnum();
            InitPriceListTypeEnum();
            InitSalesOrderTypeEnum();
            InitEditedPriceStatusEnum();
            InitProblemStatusEnum();
            InitERouteTypeEnum();
            InitNotificationStatusEnum();
            InitSurveyEnum();
            InitKpiEnum();
            InitPermissionEnum();
            InitStoreScoutingStatusEnum();
            InitSystemConfigurationEnum();
            InitWorkflowEnum();
            InitPromotionTypeEnum();
            InitPromotionProductAppliedTypeEnum();
            InitPromotionPolicyEnum();
            InitPromotionDiscountTypeEnum();
            InitRewardStatusEnum();
            InitTransactionTypeEnum();
            InitEntityTypeEnum();
            InitEntityComponentEnum();
            InitStoreApprovalStateEnum();
            InitErpApprovalStateEnum();
            InitDirectSalesOrderSourceTypeEnum();
            InitExportTemplateEnum();
            return Ok();
        }

        private void InitStoreStatusHistoryTypeEnum()
        {
            List<StoreStatusHistoryTypeDAO> StoreStatusHistoryTypeDAOs = StoreStatusHistoryTypeEnum.StoreStatusHistoryTypeEnumList.Select(x => new StoreStatusHistoryTypeDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.StoreStatusHistoryType.BulkSynchronize(StoreStatusHistoryTypeDAOs);
        }

        private void InitStoreStatusEnum()
        {
            List<StoreStatusDAO> StoreStatusDAOs = StoreStatusEnum.StoreStatusEnumList.Select(x => new StoreStatusDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.StoreStatus.BulkSynchronize(StoreStatusDAOs);
            List<StoreStatus> StoreStatuses = StoreStatusDAOs.Select(x => new StoreStatus
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();

            List<EventMessage<StoreStatus>> messages = StoreStatuses.Select(x => new EventMessage<StoreStatus>(x, Guid.NewGuid())).ToList();
            RabbitManager.PublishList(messages, RoutingKeyEnum.StoreStatusSync);
        }

        private void InitERouteTypeEnum()
        {
            List<ERouteTypeDAO> ERouteTypeEnumList = ERouteTypeEnum.ERouteTypeEnumList.Select(item => new ERouteTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ERouteType.BulkSynchronize(ERouteTypeEnumList);
        }

        private void InitRewardStatusEnum()
        {
            List<RewardStatusDAO> RewardStatusEnumList = RewardStatusEnum.RewardStatusEnumList.Select(item => new RewardStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.RewardStatus.BulkSynchronize(RewardStatusEnumList);
        }

        private void InitTransactionTypeEnum()
        {
            List<TransactionTypeDAO> TransactionTypeEnumList = TransactionTypeEnum.TransactionTypeEnumList.Select(item => new TransactionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.TransactionType.BulkSynchronize(TransactionTypeEnumList);
        }

        private void InitStoreScoutingStatusEnum()
        {
            List<StoreScoutingStatusDAO> StoreScoutingStatusEnumList = StoreScoutingStatusEnum.StoreScoutingStatusEnumList.Select(item => new StoreScoutingStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.StoreScoutingStatus.BulkSynchronize(StoreScoutingStatusEnumList);
        }

        private void InitNotificationStatusEnum()
        {
            List<NotificationStatusDAO> NotificationStatusEnumList = NotificationStatusEnum.NotificationStatusEnumList.Select(item => new NotificationStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.NotificationStatus.BulkSynchronize(NotificationStatusEnumList);
        }

        private void InitKpiEnum()
        {
            List<KpiCriteriaGeneralDAO> KpiCriteriaGeneralDAOs = KpiCriteriaGeneralEnum.KpiCriteriaGeneralEnumList.Select(item => new KpiCriteriaGeneralDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiCriteriaGeneral.BulkSynchronize(KpiCriteriaGeneralDAOs);

            List<KpiCriteriaItemDAO> KpiCriteriaItemDAOs = KpiCriteriaItemEnum.KpiCriteriaItemEnumList.Select(item => new KpiCriteriaItemDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiCriteriaItem.BulkSynchronize(KpiCriteriaItemDAOs);

            List<KpiPeriodDAO> KpiPeriodDAOs = KpiPeriodEnum.KpiPeriodEnumList.Select(item => new KpiPeriodDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiPeriod.BulkSynchronize(KpiPeriodDAOs);

            List<KpiYearDAO> KpiYearDAOs = KpiYearEnum.KpiYearEnumList.Select(item => new KpiYearDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.KpiYear.BulkSynchronize(KpiYearDAOs);
        }

        private void InitEditedPriceStatusEnum()
        {
            List<EditedPriceStatusDAO> EditedPriceStatusEnumList = EditedPriceStatusEnum.EditedPriceStatusEnumList.Select(item => new EditedPriceStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.EditedPriceStatus.BulkSynchronize(EditedPriceStatusEnumList);
        }

        private void InitPriceListTypeEnum()
        {
            List<PriceListTypeDAO> PriceListTypeEnumList = PriceListTypeEnum.PriceListTypeEnumList.Select(item => new PriceListTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PriceListType.BulkSynchronize(PriceListTypeEnumList);
        }

        private void InitPromotionTypeEnum()
        {
            List<PromotionTypeDAO> PromotionTypeEnumList = PromotionTypeEnum.PromotionTypeEnumList.Select(item => new PromotionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PromotionType.BulkSynchronize(PromotionTypeEnumList);
        }

        private void InitPromotionProductAppliedTypeEnum()
        {
            List<PromotionProductAppliedTypeDAO> PromotionProductAppliedTypeDAOs = PromotionProductAppliedTypeEnum.PromotionProductAppliedTypeEnumList.Select(item => new PromotionProductAppliedTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PromotionProductAppliedType.BulkSynchronize(PromotionProductAppliedTypeDAOs);
        }

        private void InitPromotionPolicyEnum()
        {
            List<PromotionPolicyDAO> PromotionPolicyEnumList = PromotionPolicyEnum.PromotionPolicyEnumList.Select(item => new PromotionPolicyDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PromotionPolicy.BulkSynchronize(PromotionPolicyEnumList);
        }

        private void InitPromotionDiscountTypeEnum()
        {
            List<PromotionDiscountTypeDAO> PromotionDiscountTypeEnumList = PromotionDiscountTypeEnum.PromotionDiscountTypeEnumList.Select(item => new PromotionDiscountTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.PromotionDiscountType.BulkSynchronize(PromotionDiscountTypeEnumList);
        }

        private void InitSalesOrderTypeEnum()
        {
            List<SalesOrderTypeDAO> SalesOrderTypeEnumList = SalesOrderTypeEnum.SalesOrderTypeEnumList.Select(item => new SalesOrderTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.SalesOrderType.BulkSynchronize(SalesOrderTypeEnumList);
        }

        private void InitSurveyEnum()
        {
            List<SurveyQuestionTypeDAO> SurveyQuestionTypeEnumList = SurveyQuestionTypeEnum.SurveyQuestionTypeEnumList.Select(item => new SurveyQuestionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.SurveyQuestionType.BulkSynchronize(SurveyQuestionTypeEnumList);

            List<SurveyOptionTypeDAO> SurveyOptionTypeEnumList = SurveyOptionTypeEnum.SurveyOptionTypeEnumList.Select(item => new SurveyOptionTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.SurveyOptionType.BulkSynchronize(SurveyOptionTypeEnumList);

            List<SurveyRespondentTypeDAO> SurveyRespondentTypeEnumList = SurveyRespondentTypeEnum.SurveyRespondentTypeEnumList.Select(item => new SurveyRespondentTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.SurveyRespondentType.BulkSynchronize(SurveyRespondentTypeEnumList);
        }

        private void InitProblemStatusEnum()
        {
            List<ProblemStatusDAO> ProblemStatusEnumList = ProblemStatusEnum.ProblemStatusEnumList.Select(item => new ProblemStatusDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.ProblemStatus.BulkSynchronize(ProblemStatusEnumList);
        }

        private void InitPermissionEnum()
        {
            List<FieldTypeDAO> FieldTypeDAOs = FieldTypeEnum.List.Select(item => new FieldTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.FieldType.BulkSynchronize(FieldTypeDAOs);
            List<PermissionOperatorDAO> PermissionOperatorDAOs = new List<PermissionOperatorDAO>();
            List<PermissionOperatorDAO> ID = PermissionOperatorEnum.PermissionOperatorEnumForID.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.ID.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(ID);
            List<PermissionOperatorDAO> STRING = PermissionOperatorEnum.PermissionOperatorEnumForSTRING.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.STRING.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(STRING);

            List<PermissionOperatorDAO> LONG = PermissionOperatorEnum.PermissionOperatorEnumForLONG.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.LONG.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(LONG);

            List<PermissionOperatorDAO> DECIMAL = PermissionOperatorEnum.PermissionOperatorEnumForDECIMAL.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.DECIMAL.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(DECIMAL);

            List<PermissionOperatorDAO> DATE = PermissionOperatorEnum.PermissionOperatorEnumForDATE.Select(item => new PermissionOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                FieldTypeId = FieldTypeEnum.DATE.Id,
            }).ToList();
            PermissionOperatorDAOs.AddRange(DATE);

            DataContext.PermissionOperator.BulkSynchronize(PermissionOperatorDAOs);
        }

        private void InitSystemConfigurationEnum()
        {
            List<SystemConfigurationDAO> SystemConfigurationDAOs = DataContext.SystemConfiguration.ToList();
            foreach (GenericEnum item in SystemConfigurationEnum.SystemConfigurationEnumList)
            {
                SystemConfigurationDAO SystemConfigurationDAO = SystemConfigurationDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (SystemConfigurationDAO == null)
                {
                    SystemConfigurationDAO = new SystemConfigurationDAO();
                    SystemConfigurationDAO.Id = item.Id;
                    SystemConfigurationDAO.Code = item.Code;
                    SystemConfigurationDAO.Name = item.Name;
                    SystemConfigurationDAO.Value = null;
                    SystemConfigurationDAOs.Add(SystemConfigurationDAO);
                }
            }
            DataContext.SystemConfiguration.BulkSynchronize(SystemConfigurationDAOs);
        }

        private void InitWorkflowEnum()
        {
            List<WorkflowParameterTypeDAO> WorkflowParameterTypeDAOs = WorkflowParameterTypeEnum.List.Select(item => new WorkflowParameterTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowParameterType.BulkSynchronize(WorkflowParameterTypeDAOs);

            List<WorkflowOperatorDAO> WorkflowOperatorDAOs = new List<WorkflowOperatorDAO>();
            List<WorkflowOperatorDAO> ID = WorkflowOperatorEnum.WorkflowOperatorEnumForID.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = WorkflowParameterTypeEnum.ID.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(ID);

            List<WorkflowOperatorDAO> STRING = WorkflowOperatorEnum.WorkflowOperatorEnumForSTRING.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.STRING.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(STRING);

            List<WorkflowOperatorDAO> LONG = WorkflowOperatorEnum.WorkflowOperatorEnumForLONG.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.LONG.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(LONG);

            List<WorkflowOperatorDAO> DECIMAL = WorkflowOperatorEnum.WorkflowOperatorEnumForDECIMAL.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.DECIMAL.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(DECIMAL);

            List<WorkflowOperatorDAO> DATE = WorkflowOperatorEnum.WorkflowOperatorEnumForDATE.Select(item => new WorkflowOperatorDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = FieldTypeEnum.DATE.Id,
            }).ToList();
            WorkflowOperatorDAOs.AddRange(DATE);

            DataContext.WorkflowOperator.BulkSynchronize(WorkflowOperatorDAOs);


            List<WorkflowTypeDAO> WorkflowTypeEnumList = WorkflowTypeEnum.WorkflowTypeEnumList.Select(item => new WorkflowTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowType.BulkSynchronize(WorkflowTypeEnumList);

            List<WorkflowParameterDAO> WorkflowParameterDAOs = new List<WorkflowParameterDAO>();

            List<WorkflowParameterDAO> EROUTE_PARAMETER = WorkflowParameterEnum.ERouteEnumList.Select(item => new WorkflowParameterDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = long.Parse(item.Value),
                WorkflowTypeId = WorkflowTypeEnum.EROUTE.Id,
            }).ToList();
            WorkflowParameterDAOs.AddRange(EROUTE_PARAMETER);

            List<WorkflowParameterDAO> INDIRECT_SALES_ORDER_PARAMETER = WorkflowParameterEnum.IndirectSalesOrderEnumList.Select(item => new WorkflowParameterDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = long.Parse(item.Value),
                WorkflowTypeId = WorkflowTypeEnum.INDIRECT_SALES_ORDER.Id,
            }).ToList();
            WorkflowParameterDAOs.AddRange(INDIRECT_SALES_ORDER_PARAMETER);

            List<WorkflowParameterDAO> DIRECT_SALES_ORDER_PARAMETER = WorkflowParameterEnum.DirectSalesOrderEnumList.Select(item => new WorkflowParameterDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = long.Parse(item.Value),
                WorkflowTypeId = WorkflowTypeEnum.DIRECT_SALES_ORDER.Id,
            }).ToList();
            WorkflowParameterDAOs.AddRange(DIRECT_SALES_ORDER_PARAMETER);

            List<WorkflowParameterDAO> PRICE_LIST_PARAMETER = WorkflowParameterEnum.PriceListEnumList.Select(item => new WorkflowParameterDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                WorkflowParameterTypeId = long.Parse(item.Value),
                WorkflowTypeId = WorkflowTypeEnum.PRICE_LIST.Id,
            }).ToList();
            WorkflowParameterDAOs.AddRange(PRICE_LIST_PARAMETER);

            DataContext.WorkflowParameter.BulkMerge(WorkflowParameterDAOs);


            List<WorkflowStateDAO> WorkflowStateEnumList = WorkflowStateEnum.WorkflowStateEnumList.Select(item => new WorkflowStateDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.WorkflowState.BulkSynchronize(WorkflowStateEnumList);
            List<RequestStateDAO> RequestStateEnumList = RequestStateEnum.RequestStateEnumList.Select(item => new RequestStateDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.RequestState.BulkSynchronize(RequestStateEnumList);

        }

        private void InitEntityTypeEnum()
        {
            List<EntityTypeDAO> EntityTypeDAOs = EntityTypeEnum.EntityTypeEnumList.Select(item => new EntityTypeDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.EntityType.BulkSynchronize(EntityTypeDAOs);
        }

        private void InitEntityComponentEnum()
        {
            List<EntityComponentDAO> EntityComponentDAOs = EntityComponentEnum.EntityComponentEnumList.Select(item => new EntityComponentDAO
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
            }).ToList();
            DataContext.EntityComponent.BulkSynchronize(EntityComponentDAOs);
        }

        private void InitStoreApprovalStateEnum()
        {
            List<StoreApprovalStateDAO> StoreApprovalStateDAOs = DataContext.StoreApprovalState.ToList();
            foreach (GenericEnum item in StoreApprovalStateEnum.StoreApprovalStateEnumList)
            {
                StoreApprovalStateDAO StoreApprovalStateDAO = StoreApprovalStateDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (StoreApprovalStateDAO == null)
                {
                    StoreApprovalStateDAO = new StoreApprovalStateDAO();
                    StoreApprovalStateDAO.Id = item.Id;
                    StoreApprovalStateDAO.Code = item.Code;
                    StoreApprovalStateDAO.Name = item.Name;
                    StoreApprovalStateDAOs.Add(StoreApprovalStateDAO);
                }
            }
            DataContext.StoreApprovalState.BulkSynchronize(StoreApprovalStateDAOs);
        }

        private void InitErpApprovalStateEnum()
        {
            List<ErpApprovalStateDAO> ErpApprovalStateDAOs = DataContext.ErpApprovalState.ToList();
            foreach (GenericEnum item in ErpApprovalStateEnum.ErpApprovalStateEnumList)
            {
                ErpApprovalStateDAO ErpApprovalStateDAO = ErpApprovalStateDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (ErpApprovalStateDAO == null)
                {
                    ErpApprovalStateDAO = new ErpApprovalStateDAO();
                    ErpApprovalStateDAO.Id = item.Id;
                    ErpApprovalStateDAO.Code = item.Code;
                    ErpApprovalStateDAO.Name = item.Name;
                    ErpApprovalStateDAOs.Add(ErpApprovalStateDAO);
                }
            }
            DataContext.ErpApprovalState.BulkSynchronize(ErpApprovalStateDAOs);
        }

        private void InitDirectSalesOrderSourceTypeEnum()
        {
            List<DirectSalesOrderSourceTypeDAO> DirectSalesOrderSourceTypeDAOs = DataContext.DirectSalesOrderSourceType.ToList();
            foreach (GenericEnum item in DirectSalesOrderSourceTypeEnum.DirectSalesOrderSourceTypeEnumList)
            {
                DirectSalesOrderSourceTypeDAO DirectSalesOrderSourceTypeDAO = DirectSalesOrderSourceTypeDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (DirectSalesOrderSourceTypeDAO == null)
                {
                    DirectSalesOrderSourceTypeDAO = new DirectSalesOrderSourceTypeDAO();
                    DirectSalesOrderSourceTypeDAO.Id = item.Id;
                    DirectSalesOrderSourceTypeDAO.Code = item.Code;
                    DirectSalesOrderSourceTypeDAO.Name = item.Name;
                    DirectSalesOrderSourceTypeDAOs.Add(DirectSalesOrderSourceTypeDAO);
                }
            }
            DataContext.DirectSalesOrderSourceType.BulkSynchronize(DirectSalesOrderSourceTypeDAOs);
        }

        private void InitExportTemplateEnum()
        {
            List<ExportTemplateDAO> ExportTemplateDAOs = DataContext.ExportTemplate.ToList();
            foreach (GenericEnum item in ExportTemplateEnum.ExportTemplateEnumList)
            {
                ExportTemplateDAO ExportTemplateDAO = ExportTemplateDAOs.Where(sc => sc.Id == item.Id).FirstOrDefault();
                if (ExportTemplateDAO == null)
                {
                    ExportTemplateDAO = new ExportTemplateDAO();
                    ExportTemplateDAO.Id = item.Id;
                    ExportTemplateDAO.Code = item.Code;
                    ExportTemplateDAO.Name = item.Name;
                    ExportTemplateDAOs.Add(ExportTemplateDAO);
                }
            }
            DataContext.ExportTemplate.BulkSynchronize(ExportTemplateDAOs);
        }
        #endregion
    }
}