using Common;
using DMS.Entities;
using DMS.Services.MBrand;
using DMS.Services.MImage;
using DMS.Services.MInventory;
using DMS.Services.MItem;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using DMS.Services.MTaxType;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGrouping;
using DMS.Services.MUsedVariation;
using DMS.Services.MVariationGrouping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Rpc.product
{

    public partial class ProductController : RpcController
    {
        private readonly IWebHostEnvironment _env;

        private IBrandService BrandService;
        private IProductTypeService ProductTypeService;
        private IStatusService StatusService;
        private ISupplierService SupplierService;
        private ITaxTypeService TaxTypeService;
        private IUnitOfMeasureService UnitOfMeasureService;
        private IUnitOfMeasureGroupingService UnitOfMeasureGroupingService;
        private IItemService ItemService;
        private IImageService ImageService;
        private IInventoryService InventoryService;
        private IProductGroupingService ProductGroupingService;
        private IVariationGroupingService VariationGroupingService;
        private IProductService ProductService;
        private IUsedVariationService UsedVariationService;
        private ICurrentContext CurrentContext;
        public ProductController(
            IWebHostEnvironment env,
            IBrandService BrandService,
            IProductTypeService ProductTypeService,
            IStatusService StatusService,
            ISupplierService SupplierService,
            ITaxTypeService TaxTypeService,
            IUnitOfMeasureService UnitOfMeasureService,
            IUnitOfMeasureGroupingService UnitOfMeasureGroupingService,
            IItemService ItemService,
            IImageService ImageService,
            IInventoryService InventoryService,
            IProductGroupingService ProductGroupingService,
            IVariationGroupingService VariationGroupingService,
            IProductService ProductService,
            IUsedVariationService UsedVariationService,
            ICurrentContext CurrentContext
        )
        {
            _env = env;
            this.BrandService = BrandService;
            this.ProductTypeService = ProductTypeService;
            this.StatusService = StatusService;
            this.SupplierService = SupplierService;
            this.TaxTypeService = TaxTypeService;
            this.UnitOfMeasureService = UnitOfMeasureService;
            this.UnitOfMeasureGroupingService = UnitOfMeasureGroupingService;
            this.ItemService = ItemService;
            this.ImageService = ImageService;
            this.InventoryService = InventoryService;
            this.ProductGroupingService = ProductGroupingService;
            this.VariationGroupingService = VariationGroupingService;
            this.ProductService = ProductService;
            this.UsedVariationService = UsedVariationService;
            this.CurrentContext = CurrentContext;
        }

        [Route(ProductRoute.Count), HttpPost]
        public async Task<ActionResult<int>> Count([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            int count = await ProductService.Count(ProductFilter);
            return count;
        }

        [Route(ProductRoute.List), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> List([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            List<Product> Products = await ProductService.List(ProductFilter);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
        }

        [Route(ProductRoute.Get), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Get([FromBody]Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = await ProductService.Get(Product_ProductDTO.Id);
            return new Product_ProductDTO(Product);
        }

        [Route(ProductRoute.Create), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Create([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);

            Product = await ProductService.Create(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.Update), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Update([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);
            Product = await ProductService.Update(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.Delete), HttpPost]
        public async Task<ActionResult<Product_ProductDTO>> Delete([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            if (!await HasPermission(Product_ProductDTO.Id))
                return Forbid();

            Product Product = ConvertDTOToEntity(Product_ProductDTO);
            Product = await ProductService.Delete(Product);
            Product_ProductDTO = new Product_ProductDTO(Product);
            if (Product.IsValidated)
                return Product_ProductDTO;
            else
                return BadRequest(Product_ProductDTO);
        }

        [Route(ProductRoute.Import), HttpPost]
        public async Task<ActionResult<List<Product_ProductDTO>>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(new ProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductGroupingSelect.ALL
            });
            List<ProductType> ProductTypes = await ProductTypeService.List(new ProductTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductTypeSelect.ALL
            });
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL
            });
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.ALL
            });
            List<Supplier> Suppliers = await SupplierService.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.ALL
            });
            List<Brand> Brands = await BrandService.List(new BrandFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = BrandSelect.ALL
            });
            List<TaxType> TaxTypes = await TaxTypeService.List(new TaxTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = TaxTypeSelect.ALL
            });
            List<UsedVariation> UsedVariations = await UsedVariationService.List(new UsedVariationFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UsedVariationSelect.ALL
            });
            List<VariationGrouping> VariationGroupings = await VariationGroupingService.List(new VariationGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = VariationGroupingSelect.ALL
            });
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };
            List<Product> Products = new List<Product>();
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                #region ProductSheet
                ExcelWorksheet ProductSheet = excelPackage.Workbook.Worksheets["Product"];
                if (ProductSheet == null)
                    return null;

                #region Khai báo thứ tự các cột trong Exel file 
                int StartColumn = 1;
                int StartRow = 1;
                //Mã sản phẩm
                int CodeColumn = 1 + StartColumn;
                //Tên sản phẩm
                int NameColumn = 2 + StartColumn;
                //Nhóm sản phẩm
                int ProductGroupCodeColumn = 3 + StartColumn;
                //Loại sản phẩm
                int ProductTypeCodeColumn = 4 + StartColumn;
                //Đơn vị tính
                int UoMCodeColumn = 5 + StartColumn;
                //NHóm đơn vị tính
                int UoMGroupCodeColumn = 6 + StartColumn;
                //Nhà cung cấp
                int SupplierCodeColumn = 7 + StartColumn;
                //Mã ERP
                int ERPCodeColumn = 8 + StartColumn;
                //Mã nhận diện sản phẩm
                int ScanCodeColumn = 9 + StartColumn;
                //Nhãn hiệu
                int BrandCodeColumn = 10 + StartColumn;
                //Tên khác
                int OtherNameColumn = 11 + StartColumn;
                //Tên kỹ thuật
                int TechnicalNameColumn = 12 + StartColumn;
                //Mã thuế
                int TaxTypeCodeColumn = 13 + StartColumn;
                //Mô tả
                int DescriptionColumn = 14 + StartColumn;
                //Giá bánr
                int RetailPriceColumn = 15 + StartColumn;
                //Giá bán đề xuất
                int SalePriceColumn = 16 + StartColumn;

                //Có tạo phiên bản
                int UsedVariationCodeColumn = 17 + StartColumn;
                //Thuộc tính 1
                int Property1Column = 18 + StartColumn;
                //Giá trị 1
                int PropertyValue1Column = 19 + StartColumn;
                //Thuộc tính 2
                int Property2Column = 20 + StartColumn;
                //Giá trị 2
                int PropertyValue2Column = 21 + StartColumn;
                //Thuộc tính 3
                int Property3Column = 22 + StartColumn;
                //Giá trị 3
                int PropertyValue3Column = 23 + StartColumn;
                //Thuộc tính 4
                int Property4Column = 24 + StartColumn;
                //Giá trị 4
                int PropertyValue4Column = 25 + StartColumn;
                #endregion

                for (int i = StartRow; i <= ProductSheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(ProductSheet.Cells[i + StartRow, CodeColumn].Value?.ToString()))
                        break;
                    string CodeValue = ProductSheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = ProductSheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ProductGroupCodeValue = ProductSheet.Cells[i + StartRow, ProductGroupCodeColumn].Value?.ToString();
                    string ProductTypeCodeValue = ProductSheet.Cells[i + StartRow, ProductTypeCodeColumn].Value?.ToString();
                    string UoMCodeValue = ProductSheet.Cells[i + StartRow, UoMCodeColumn].Value?.ToString();
                    string UoMGroupCodeValue = ProductSheet.Cells[i + StartRow, UoMGroupCodeColumn].Value?.ToString();
                    string SupplierCodeValue = ProductSheet.Cells[i + StartRow, SupplierCodeColumn].Value?.ToString();
                    string ERPCodeValue = ProductSheet.Cells[i + StartRow, ERPCodeColumn].Value?.ToString();
                    string ScanCodeValue = ProductSheet.Cells[i + StartRow, ScanCodeColumn].Value?.ToString();
                    string BrandCodeValue = ProductSheet.Cells[i + StartRow, BrandCodeColumn].Value?.ToString();
                    string OtherNameValue = ProductSheet.Cells[i + StartRow, OtherNameColumn].Value?.ToString();
                    string TechnicalNameValue = ProductSheet.Cells[i + StartRow, TechnicalNameColumn].Value?.ToString();
                    string TaxTypeCodeValue = ProductSheet.Cells[i + StartRow, TaxTypeCodeColumn].Value?.ToString();
                    string DescriptionValue = ProductSheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();
                    string RetailPriceValue = ProductSheet.Cells[i + StartRow, RetailPriceColumn].Value?.ToString();
                    string SalePriceValue = ProductSheet.Cells[i + StartRow, SalePriceColumn].Value?.ToString();
                    //Thuộc tính
                    string UsedVariationCodeValue = ProductSheet.Cells[i + StartRow, UsedVariationCodeColumn].Value?.ToString();
                    string Property1Value = ProductSheet.Cells[i + StartRow, Property1Column].Value?.ToString();
                    string PropertyValue1Value = ProductSheet.Cells[i + StartRow, PropertyValue1Column].Value?.ToString();
                    string Property2Value = ProductSheet.Cells[i + StartRow, Property2Column].Value?.ToString();
                    string PropertyValue2Value = ProductSheet.Cells[i + StartRow, PropertyValue2Column].Value?.ToString();
                    string Property3Value = ProductSheet.Cells[i + StartRow, Property3Column].Value?.ToString();
                    string PropertyValue3Value = ProductSheet.Cells[i + StartRow, PropertyValue3Column].Value?.ToString();
                    string Property4Value = ProductSheet.Cells[i + StartRow, Property4Column].Value?.ToString();
                    string PropertyValue4Value = ProductSheet.Cells[i + StartRow, PropertyValue4Column].Value?.ToString();

                    Product Product = new Product();
                    Product.Code = CodeValue;
                    Product.Name = NameValue;
                    if (!string.IsNullOrEmpty(ProductGroupCodeValue))
                    {
                        ProductGrouping ProductGrouping = ProductGroupings.Where(pg => pg.Code.Equals(ProductGroupCodeValue)).FirstOrDefault();
                        if (ProductGrouping != null)
                        {
                            ProductProductGroupingMapping ProductProductGroupingMapping = new ProductProductGroupingMapping();
                            Product.ProductProductGroupingMappings = new List<ProductProductGroupingMapping>();

                            ProductProductGroupingMapping.ProductGrouping = ProductGrouping;
                            Product.ProductProductGroupingMappings.Add(ProductProductGroupingMapping);
                        }
                    }

                    Product.ProductTypeId = ProductTypes.Where(x => x.Code.Equals(ProductTypeCodeValue)).Select(x => x.Id).FirstOrDefault();
                    Product.UnitOfMeasureId = UnitOfMeasures.Where(x => x.Code.Equals(UoMCodeValue)).Select(x => x.Id).FirstOrDefault();
                    Product.UnitOfMeasureGroupingId = UnitOfMeasureGroupings.Where(x => x.Code.Equals(UoMGroupCodeValue)).Select(x => x.Id).FirstOrDefault();
                    Product.SupplierId = Suppliers.Where(x => x.Code.Equals(SupplierCodeValue)).Select(x => x.Id).FirstOrDefault();
                    Product.BrandId = Brands.Where(x => x.Code.Equals(BrandCodeValue)).Select(x => x.Id).FirstOrDefault();

                    Product.ERPCode = ERPCodeValue;
                    Product.ScanCode = ScanCodeValue;

                    Product.OtherName = OtherNameValue;
                    Product.TechnicalName = TechnicalNameValue;
                    Product.TaxTypeId = TaxTypes.Where(x => x.Code.Equals(TaxTypeCodeValue.Trim())).Select(x => x.Id).FirstOrDefault();
                    Product.UsedVariationId = UsedVariations.Where(x => x.Code.ToLower().Equals(UsedVariationCodeValue.Trim().ToLower())).Select(x => x.Id).FirstOrDefault();
                    Product.Description = DescriptionValue;
                    Product.RetailPrice = string.IsNullOrEmpty(RetailPriceValue) ? 0 : decimal.Parse(RetailPriceValue);
                    Product.SalePrice = string.IsNullOrEmpty(SalePriceValue) ? 0 : decimal.Parse(SalePriceValue);

                    if(Product.UsedVariationId == Enums.UsedVariationEnum.USED.Id)
                    {
                        #region Variation
                        if (!string.IsNullOrEmpty(Property1Value))
                        {
                            Product.VariationGroupings = new List<VariationGrouping>();
                            VariationGrouping VariationGrouping = VariationGroupings.Where(v => v.Name.Equals(Property1Value)).FirstOrDefault();
                            if (VariationGrouping == null)
                            {
                                VariationGrouping = new VariationGrouping
                                {
                                    Name = Property1Value
                                };

                            }
                            VariationGrouping.Variations = new List<Variation>();
                            var Values = PropertyValue1Value.Split(',');
                            foreach (var Value in Values)
                            {
                                var splitValue = Value.Split('-');
                                Variation Variation = new Variation
                                {
                                    Code = splitValue[0],
                                    Name = splitValue[1]
                                };
                                VariationGrouping.Variations.Add(Variation);
                            }
                            Product.VariationGroupings.Add(VariationGrouping);

                        }

                        if (!string.IsNullOrEmpty(Property2Value))
                        {
                            Product.VariationGroupings = new List<VariationGrouping>();
                            VariationGrouping VariationGrouping = VariationGroupings.Where(v => v.Name.Equals(Property2Value)).FirstOrDefault();
                            if (VariationGrouping == null)
                            {
                                VariationGrouping = new VariationGrouping
                                {
                                    Name = Property2Value
                                };

                            }
                            VariationGrouping.Variations = new List<Variation>();
                            var Values = PropertyValue2Value.Split(',');
                            foreach (var Value in Values)
                            {
                                var splitValue = Value.Split('-');
                                Variation Variation = new Variation
                                {
                                    Code = splitValue[0],
                                    Name = splitValue[1]
                                };
                                VariationGrouping.Variations.Add(Variation);
                            }
                            Product.VariationGroupings.Add(VariationGrouping);

                        }

                        if (!string.IsNullOrEmpty(Property3Value))
                        {
                            Product.VariationGroupings = new List<VariationGrouping>();
                            VariationGrouping VariationGrouping = VariationGroupings.Where(v => v.Name.Equals(Property3Value)).FirstOrDefault();
                            if (VariationGrouping == null)
                            {
                                VariationGrouping = new VariationGrouping
                                {
                                    Name = Property3Value
                                };

                            }
                            VariationGrouping.Variations = new List<Variation>();
                            var Values = PropertyValue3Value.Split(',');
                            foreach (var Value in Values)
                            {
                                var splitValue = Value.Split('-');
                                Variation Variation = new Variation
                                {
                                    Code = splitValue[0],
                                    Name = splitValue[1]
                                };
                                VariationGrouping.Variations.Add(Variation);
                            }
                            Product.VariationGroupings.Add(VariationGrouping);

                        }

                        if (!string.IsNullOrEmpty(Property4Value))
                        {
                            Product.VariationGroupings = new List<VariationGrouping>();
                            VariationGrouping VariationGrouping = VariationGroupings.Where(v => v.Name.Equals(Property4Value)).FirstOrDefault();
                            if (VariationGrouping == null)
                            {
                                VariationGrouping = new VariationGrouping
                                {
                                    Name = Property4Value
                                };

                            }
                            VariationGrouping.Variations = new List<Variation>();
                            var Values = PropertyValue4Value.Split(',');
                            foreach (var Value in Values)
                            {
                                var splitValue = Value.Split('-');
                                Variation Variation = new Variation
                                {
                                    Code = splitValue[0],
                                    Name = splitValue[1]
                                };
                                VariationGrouping.Variations.Add(Variation);
                            }
                            Product.VariationGroupings.Add(VariationGrouping);
                        }
                        #endregion
                    }
                    Products.Add(Product);
                }
                #endregion

                #region ItemSheet
                ExcelWorksheet ItemSheet = excelPackage.Workbook.Worksheets["Item"];
                if (ItemSheet != null)
                {
                    StartColumn = 1;
                    StartRow = 1;

                    CodeColumn = 1 + StartColumn;
                    int CodeProductItemColumn = 2 + StartColumn;
                    int NameProductItemColumn = 3 + StartColumn;
                    int ScanCodeItemColumn = 4 + StartColumn;
                    SalePriceColumn = 5 + StartColumn;
                    RetailPriceColumn = 6 + StartColumn;

                    for (int i = StartRow; i <= ItemSheet.Dimension.End.Row; i++)
                    {
                        if (string.IsNullOrEmpty(ItemSheet.Cells[i + StartRow, CodeColumn].Value?.ToString()))
                            break;
                        string CodeValue = ItemSheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                        string CodeProductItemValue = ItemSheet.Cells[i + StartRow, CodeProductItemColumn].Value?.ToString();
                        string NameProductItemValue = ItemSheet.Cells[i + StartRow, NameProductItemColumn].Value?.ToString();
                        string ScanCodeItemValue = ItemSheet.Cells[i + StartRow, ScanCodeItemColumn].Value?.ToString();
                        string SalePriceValue = ItemSheet.Cells[i + StartRow, SalePriceColumn].Value?.ToString();
                        string RetailPriceValue = ItemSheet.Cells[i + StartRow, RetailPriceColumn].Value?.ToString();

                        Item Item = new Item();
                        Item.Code = CodeProductItemValue;
                        Item.Name = NameProductItemValue;
                        Item.ScanCode = ScanCodeItemValue;
                        Item.SalePrice = string.IsNullOrEmpty(SalePriceValue) ? 0 : decimal.Parse(SalePriceValue);
                        Item.RetailPrice = string.IsNullOrEmpty(RetailPriceValue) ? 0 : decimal.Parse(RetailPriceValue);
                        var Product = Products.Where(p => p.Code == CodeValue).FirstOrDefault();
                        if (Product != null)
                        {
                            Product.Items.Add(Item);
                        }

                    }
                    #endregion
                }
            }
            #region Tạo mới item nếu chưa có
            foreach (var Product in Products)
            {
                if (Product.Items != null && Product.Items.Count > 0) continue;
                Product.Items = new List<Item>();
                Product.Items.Add(new Item
                {
                    Code = Product.Code,
                    Name = Product.Name,
                    ScanCode = Product.ScanCode,
                    RetailPrice = Product.RetailPrice,
                    SalePrice = Product.SalePrice,
                    ProductId = Product.Id
                });
            }
            #endregion

            Products = await ProductService.Import(Products);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
        }

        [Route(ProductRoute.Export), HttpPost]
        public async Task<ActionResult> Export([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter.Skip = 0;
            ProductFilter.Take = int.MaxValue;
            ProductFilter.Selects = ProductSelect.ALL;
            ProductFilter = ProductService.ToFilter(ProductFilter);


            List<Product> Products = await ProductService.List(ProductFilter);
            List<Item> Items = await ItemService.List(new ItemFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ItemSelect.ALL,
                ProductId = new IdFilter { In = Products.Select(p => p.Id).ToList() }
            });
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(new ProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductGroupingSelect.ALL,
            });
            List<ProductType> ProductTypes = await ProductTypeService.List(new ProductTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductTypeSelect.ALL,
            });
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL,
            });
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.ALL
            });
            List<Supplier> Suppliers = await SupplierService.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.ALL
            });
            List<Brand> Brands = await BrandService.List(new BrandFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = BrandSelect.ALL
            });
            MemoryStream MemoryStream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excel = new ExcelPackage(MemoryStream))
            {
                #region sheet product 
                var ProductHeader = new List<string[]>()
                {
                    new string[] { "STT",
                        "Mã sản phẩm*",
                        "Tên sản phẩm*",
                        "Nhóm sản phẩm",
                        "Loại sản phẩm*",
                        "Đơn vị tính*",
                        "Nhóm đơn vị chuyển đổi",
                        "Nhà cung cấp",
                        "Mã từ ERP*",
                        "Mã nhận diện sản phẩm*",
                        "Nhãn hiệu",
                        "Tên khác",
                        "Tên kỹ thuật",
                        "% VAT*",
                        "Mô tả",
                        "Giá bán",
                        "Giá bán lẻ đề xuất",
                        "Có tạo phiên bản*",
                        "Thuộc tính 1",
                        "Giá trị 1",
                        "Thuộc tính 2",
                        "Giá trị 2",
                        "Thuộc tính 3",
                        "Giá trị 3",
                        "Thuộc tính 4",
                        "Giá trị 4",
                    }
                };
                List<object[]> data = new List<object[]>();
                for (int i = 0; i < Products.Count; i++)
                {
                    Product Product = Products[i];
                    string ProductGroupingName = "";
                    if (Product.ProductProductGroupingMappings != null)
                    {
                        ProductGroupingName = Product.ProductProductGroupingMappings.FirstOrDefault() == null ? null : Product.ProductProductGroupingMappings.FirstOrDefault().ProductGrouping.Name;
                    }
                    string VariationGrouping1 = "";
                    string VariationValue1 = "";
                    string VariationGrouping2 = "";
                    string VariationValue2 = "";
                    string VariationGrouping3 = "";
                    string VariationValue3 = "";
                    string VariationGrouping4 = "";
                    string VariationValue4 = "";

                    if (Product.VariationGroupings != null)
                    {
                        if (Product.VariationGroupings.Count > 0)
                        {
                            VariationGrouping1 += Product.VariationGroupings[0].Name;
                            VariationValue1 = string.Join(",", Product.VariationGroupings[0].Variations.Select(v => v.Code + '-' + v.Name).ToArray());
                        }

                        if (Product.VariationGroupings.Count > 1)
                        {
                            VariationGrouping2 += Product.VariationGroupings[1].Name;
                            VariationValue2 = string.Join(",", Product.VariationGroupings[1].Variations.Select(v => v.Code + '-' + v.Name).ToArray());
                        }

                        if (Product.VariationGroupings.Count > 2)
                        {
                            VariationGrouping3 += Product.VariationGroupings[2].Name;
                            VariationValue3 = string.Join(",", Product.VariationGroupings[2].Variations.Select(v => v.Code + '-' + v.Name).ToArray());
                        }

                        if (Product.VariationGroupings.Count > 3)
                        {
                            VariationGrouping4 += Product.VariationGroupings[3].Name;
                            VariationValue4 = string.Join(",", Product.VariationGroupings[3].Variations.Select(v => v.Code + '-' + v.Name).ToArray());
                        }
                    }
                    data.Add(new object[]
                    {
                        i+1,
                        Product.Code,
                        Product.Name,
                        ProductGroupingName,
                        Product.ProductType?.Name,
                        Product.UnitOfMeasure?.Name,
                        Product.UnitOfMeasureGrouping?.Name,
                        Product.Supplier?.Name,
                        Product.ERPCode,
                        Product.ScanCode,
                        Product.Brand?.Name,
                        Product.OtherName,
                        Product.TechnicalName,
                        Product.TaxType.Code,
                        Product.Description,
                        Product.SalePrice,
                        Product.RetailPrice,
                        Product.UsedVariation.Code,
                        VariationGrouping1,
                        VariationValue1,
                        VariationGrouping2,
                        VariationValue2,
                        VariationGrouping3,
                        VariationValue3,
                        VariationGrouping4,
                        VariationValue4,
                    });
                }
                excel.GenerateWorksheet("Product", ProductHeader, data);
                #endregion

                #region sheet item  
                data.Clear();
                var ItemHeader = new List<string[]>()
                {
                    new string[] {
                        "STT",
                        "Mã sản phẩm",
                        "Mã sản phẩm thuộc tính",
                        "Tên sản phẩm thuộc tính",
                        "Mã nhận diện sản phẩm",
                        "Giá bán",
                        "Giá bán lẻ đề xuất",
                    }
                };
                for (int i = 0; i < Items.Count; i++)
                {
                    Item Item = Items[i];
                    data.Add(new object[] {
                                i+1,
                                Item.Product.Code,
                                Item.Code,
                                Item.Name,
                                Item.ScanCode,
                                Item.SalePrice,
                                Item.RetailPrice,
                                });
                }
                excel.GenerateWorksheet("Item", ItemHeader, data);
                #endregion 

                #region Sheet Product Group
                data.Clear();
                var ProductGroupHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã nhóm sản phẩm",
                        "Tên nhóm sản phẩm",
                    }
                };
                foreach (var ProductGrouping in ProductGroupings)
                {
                    data.Add(new object[]
                    {
                        ProductGrouping.Code,
                        ProductGrouping.Name
                    });
                }
                excel.GenerateWorksheet("ProductGroup", ProductGroupHeader, data);
                #endregion 

                #region Sheet Product Type
                data.Clear();
                var ProductTypeHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã loại sản phẩm",
                        "Tên loại sản phẩm",
                    }
                };
                foreach (var ProductType in ProductTypes)
                {
                    data.Add(new object[]
                    {
                        ProductType.Code,
                        ProductType.Name
                    });
                }
                excel.GenerateWorksheet("ProductType", ProductTypeHeader, data);
                #endregion 

                #region Sheet UOM
                data.Clear();
                var UOMHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã đơn vị tính",
                        "Tên đơn vị tính",
                    }
                };
                foreach (var UnitOfMeasure in UnitOfMeasures)
                {
                    data.Add(new object[]
                    {
                        UnitOfMeasure.Code,
                        UnitOfMeasure.Name
                    });
                }
                excel.GenerateWorksheet("UnitOfMeasure", UOMHeader, data);
                #endregion 

                #region Sheet UOMGrouping
                data.Clear();
                var UOMGroupingHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã nhóm đơn vị chuyển đổi",
                        "Tên nhóm đơn vị chuyển đổi",
                    }
                };
                foreach (var UnitOfMeasureGrouping in UnitOfMeasureGroupings)
                {
                    data.Add(new object[]
                    {
                        UnitOfMeasureGrouping.Code,
                        UnitOfMeasureGrouping.Name
                    });
                }
                excel.GenerateWorksheet("UnitOfMeasureGrouping", UOMGroupingHeader, data);
                #endregion 

                #region Sheet Supplier
                data.Clear();
                var SupplierHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã nhà cung cấp",
                        "Tên nhà cung cấp",
                    }
                };
                foreach (var Supplier in Suppliers)
                {
                    data.Add(new object[]
                    {
                        Supplier.Code,
                        Supplier.Name
                    });
                }
                excel.GenerateWorksheet("Supplier", SupplierHeader, data);
                #endregion 

                #region Sheet Brand
                data.Clear();
                var BrandHeader = new List<string[]>()
                {
                    new string[] {
                        "Mã nhãn hiệu",
                        "Tên nhãn hiệu",
                    }
                };
                foreach (var Brand in Brands)
                {
                    data.Add(new object[]
                    {
                        Brand.Code,
                        Brand.Name
                    });
                }
                excel.GenerateWorksheet("Brand", BrandHeader, data);
                #endregion 
                excel.Save();
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Product" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

        [Route(ProductRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate()
        {
            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(new ProductGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductGroupingSelect.ALL,
            });
            List<ProductType> ProductTypes = await ProductTypeService.List(new ProductTypeFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = ProductTypeSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(new UnitOfMeasureFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(new UnitOfMeasureGroupingFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = UnitOfMeasureGroupingSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<Supplier> Suppliers = await SupplierService.List(new SupplierFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = SupplierSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            List<Brand> Brands = await BrandService.List(new BrandFilter
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = BrandSelect.ALL,
                StatusId = new IdFilter { Equal = Enums.StatusEnum.ACTIVE.Id }
            });
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            MemoryStream MemoryStream = new MemoryStream();
            string tempPath = "Templates/Product_Export.xlsx";
            using (var xlPackage = new ExcelPackage(new FileInfo(tempPath)))
            {
                #region sheet ProductGrouping 
                var worksheet_ProductGroup = xlPackage.Workbook.Worksheets["ProductGroup"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_ProductGroup = 2;
                int numberCell_ProductGroup = 1;
                for (var i = 0; i < ProductGroupings.Count; i++)
                {
                    ProductGrouping ProductGrouping = ProductGroupings[i];
                    worksheet_ProductGroup.Cells[startRow_ProductGroup + i, numberCell_ProductGroup].Value = ProductGrouping.Code;
                    worksheet_ProductGroup.Cells[startRow_ProductGroup + i, numberCell_ProductGroup + 1].Value = ProductGrouping.Name;
                }
                #endregion

                #region sheet ProductType
                var worksheet_ProductType = xlPackage.Workbook.Worksheets["ProductType"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_ProductType = 2;
                int numberCell_ProductType = 1;
                for (var i = 0; i < ProductTypes.Count; i++)
                {
                    ProductType ProductType = ProductTypes[i];
                    worksheet_ProductType.Cells[startRow_ProductType + i, numberCell_ProductType].Value = ProductType.Code;
                    worksheet_ProductType.Cells[startRow_ProductType + i, numberCell_ProductType + 1].Value = ProductType.Name;
                }
                #endregion

                #region sheet UoM
                var worksheet_UoM = xlPackage.Workbook.Worksheets["UoM"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_UoM = 2;
                int numberCell_UoM = 1;
                for (var i = 0; i < UnitOfMeasures.Count; i++)
                {
                    UnitOfMeasure UnitOfMeasure = UnitOfMeasures[i];
                    worksheet_UoM.Cells[startRow_UoM + i, numberCell_UoM].Value = UnitOfMeasure.Code;
                    worksheet_UoM.Cells[startRow_UoM + i, numberCell_UoM + 1].Value = UnitOfMeasure.Name;
                }
                #endregion

                #region sheet UoMGroup
                var worksheet_UoMGroup = xlPackage.Workbook.Worksheets["UoMGroup"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_UoMGroup = 2;
                int numberCell_UoMGroup = 1;
                for (var i = 0; i < UnitOfMeasureGroupings.Count; i++)
                {
                    UnitOfMeasureGrouping UnitOfMeasureGrouping = UnitOfMeasureGroupings[i];
                    worksheet_UoMGroup.Cells[startRow_UoMGroup + i, numberCell_UoMGroup].Value = UnitOfMeasureGrouping.Code;
                    worksheet_UoMGroup.Cells[startRow_UoMGroup + i, numberCell_UoMGroup + 1].Value = UnitOfMeasureGrouping.Name;
                }
                #endregion

                #region sheet Supplier
                var worksheet_Supplier = xlPackage.Workbook.Worksheets["Supplier"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Supplier = 2;
                int numberCell_Supplier = 1;
                for (var i = 0; i < Suppliers.Count; i++)
                {
                    Supplier Supplier = Suppliers[i];
                    worksheet_Supplier.Cells[startRow_Supplier + i, numberCell_Supplier].Value = Supplier.Code;
                    worksheet_Supplier.Cells[startRow_Supplier + i, numberCell_Supplier + 1].Value = Supplier.Name;
                }
                #endregion

                #region sheet Brand
                var worksheet_Brand = xlPackage.Workbook.Worksheets["Brand"];
                xlPackage.Workbook.CalcMode = ExcelCalcMode.Manual;
                int startRow_Brand = 2;
                int numberCell_Brand = 1;
                for (var i = 0; i < Brands.Count; i++)
                {
                    Brand Brand = Brands[i];
                    worksheet_Brand.Cells[startRow_Brand + i, numberCell_Brand].Value = Brand.Code;
                    worksheet_Brand.Cells[startRow_Brand + i, numberCell_Brand + 1].Value = Brand.Name;
                }
                #endregion

                var nameexcel = "Export sản phẩm" + DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
                xlPackage.Workbook.Properties.Title = string.Format("{0}", nameexcel);
                xlPackage.Workbook.Properties.Author = "Sonhx5";
                xlPackage.Workbook.Properties.Subject = string.Format("{0}", "RD-DMS");
                xlPackage.Workbook.Properties.Category = "RD-DMS";
                xlPackage.Workbook.Properties.Company = "FPT-FIS-ERP-ESC";
                xlPackage.SaveAs(MemoryStream);
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Product" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
        }

        [Route(ProductRoute.BulkDelete), HttpPost]
        public async Task<ActionResult<bool>> BulkDelete([FromBody] List<long> Ids)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Id = new IdFilter { In = Ids };
            ProductFilter.Selects = ProductSelect.Id;
            ProductFilter.Skip = 0;
            ProductFilter.Take = int.MaxValue;

            List<Product> Products = await ProductService.List(ProductFilter);
            Products = await ProductService.BulkDelete(Products);
            if (Products.Any(x => !x.IsValidated))
                return BadRequest(Products.Where(x => !x.IsValidated));
            return true;
        }

        [Route(ProductRoute.SaveImage), HttpPost]
        public async Task<ActionResult<Product_ImageDTO>> SaveImage(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Image Image = new Image
            {
                Name = file.FileName,
                Content = memoryStream.ToArray()
            };
            Image = await ProductService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Product_ImageDTO product_ImageDTO = new Product_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
            };
            return Ok(product_ImageDTO);
        }

        [Route(ProductRoute.SaveItemImage), HttpPost]
        public async Task<ActionResult<Product_ImageDTO>> SaveItemImage(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            MemoryStream memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            Image Image = new Image
            {
                Name = file.FileName,
                Content = memoryStream.ToArray()
            };
            Image = await ItemService.SaveImage(Image);
            if (Image == null)
                return BadRequest();
            Product_ImageDTO product_ImageDTO = new Product_ImageDTO
            {
                Id = Image.Id,
                Name = Image.Name,
                Url = Image.Url,
            };
            return Ok(product_ImageDTO);
        }

        private async Task<bool> HasPermission(long Id)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter = ProductService.ToFilter(ProductFilter);
            if (Id == 0)
            {

            }
            else
            {
                ProductFilter.Id = new IdFilter { Equal = Id };
                int count = await ProductService.Count(ProductFilter);
                if (count == 0)
                    return false;
            }
            return true;
        }

        private Product ConvertDTOToEntity(Product_ProductDTO Product_ProductDTO)
        {
            Product Product = new Product();
            Product.Id = Product_ProductDTO.Id;
            Product.Code = Product_ProductDTO.Code;
            Product.SupplierCode = Product_ProductDTO.SupplierCode;
            Product.Name = Product_ProductDTO.Name;
            Product.ERPCode = Product_ProductDTO.ERPCode;
            Product.Description = Product_ProductDTO.Description;
            Product.ScanCode = Product_ProductDTO.ScanCode;
            Product.ProductTypeId = Product_ProductDTO.ProductTypeId;
            Product.SupplierId = Product_ProductDTO.SupplierId;
            Product.BrandId = Product_ProductDTO.BrandId;
            Product.UnitOfMeasureId = Product_ProductDTO.UnitOfMeasureId;
            Product.UnitOfMeasureGroupingId = Product_ProductDTO.UnitOfMeasureGroupingId;
            Product.SalePrice = Product_ProductDTO.SalePrice;
            Product.RetailPrice = Product_ProductDTO.RetailPrice;
            Product.TaxTypeId = Product_ProductDTO.TaxTypeId;
            Product.StatusId = Product_ProductDTO.StatusId;
            Product.OtherName = Product_ProductDTO.OtherName;
            Product.TechnicalName = Product_ProductDTO.TechnicalName;
            Product.Note = Product_ProductDTO.Note;
            Product.UsedVariationId = Product_ProductDTO.UsedVariationId;
            Product.Brand = Product_ProductDTO.Brand == null ? null : new Brand
            {
                Id = Product_ProductDTO.Brand.Id,
                Code = Product_ProductDTO.Brand.Code,
                Name = Product_ProductDTO.Brand.Name,
                Description = Product_ProductDTO.Brand.Description,
                StatusId = Product_ProductDTO.Brand.StatusId,
            };
            Product.ProductType = Product_ProductDTO.ProductType == null ? null : new ProductType
            {
                Id = Product_ProductDTO.ProductType.Id,
                Code = Product_ProductDTO.ProductType.Code,
                Name = Product_ProductDTO.ProductType.Name,
                Description = Product_ProductDTO.ProductType.Description,
                StatusId = Product_ProductDTO.ProductType.StatusId,
            };
            Product.Status = Product_ProductDTO.Status == null ? null : new Status
            {
                Id = Product_ProductDTO.Status.Id,
                Code = Product_ProductDTO.Status.Code,
                Name = Product_ProductDTO.Status.Name,
            };
            Product.Supplier = Product_ProductDTO.Supplier == null ? null : new Supplier
            {
                Id = Product_ProductDTO.Supplier.Id,
                Code = Product_ProductDTO.Supplier.Code,
                Name = Product_ProductDTO.Supplier.Name,
                TaxCode = Product_ProductDTO.Supplier.TaxCode,
                StatusId = Product_ProductDTO.Supplier.StatusId,
            };
            Product.TaxType = Product_ProductDTO.TaxType == null ? null : new TaxType
            {
                Id = Product_ProductDTO.TaxType.Id,
                Code = Product_ProductDTO.TaxType.Code,
                Name = Product_ProductDTO.TaxType.Name,
                Percentage = Product_ProductDTO.TaxType.Percentage,
                StatusId = Product_ProductDTO.TaxType.StatusId,
            };
            Product.UnitOfMeasure = Product_ProductDTO.UnitOfMeasure == null ? null : new UnitOfMeasure
            {
                Id = Product_ProductDTO.UnitOfMeasure.Id,
                Code = Product_ProductDTO.UnitOfMeasure.Code,
                Name = Product_ProductDTO.UnitOfMeasure.Name,
                Description = Product_ProductDTO.UnitOfMeasure.Description,
                StatusId = Product_ProductDTO.UnitOfMeasure.StatusId,
            };
            Product.UnitOfMeasureGrouping = Product_ProductDTO.UnitOfMeasureGrouping == null ? null : new UnitOfMeasureGrouping
            {
                Id = Product_ProductDTO.UnitOfMeasureGrouping.Id,
                Name = Product_ProductDTO.UnitOfMeasureGrouping.Name,
                UnitOfMeasureId = Product_ProductDTO.UnitOfMeasureGrouping.UnitOfMeasureId,
                StatusId = Product_ProductDTO.UnitOfMeasureGrouping.StatusId,
            };
            Product.UsedVariation = Product_ProductDTO.UsedVariation == null ? null : new UsedVariation
            {
                Id = Product_ProductDTO.UsedVariation.Id,
                Code = Product_ProductDTO.UsedVariation.Code,
                Name = Product_ProductDTO.UsedVariation.Name,
            };
            Product.Items = Product_ProductDTO.Items?
                .Select(x => new Item
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ScanCode = x.ScanCode,
                    SalePrice = x.SalePrice,
                    RetailPrice = x.RetailPrice,
                    CanDelete = x.CanDelete,
                    StatusId = x.StatusId,
                    ItemImageMappings = x.ItemImageMappings?.Select(x => new ItemImageMapping
                    {
                        ItemId = x.ItemId,
                        ImageId = x.ImageId,
                    }).ToList()
                }).ToList();
            Product.ProductImageMappings = Product_ProductDTO.ProductImageMappings?
                .Select(x => new ProductImageMapping
                {
                    ImageId = x.ImageId,
                    Image = new Image
                    {
                        Id = x.Image.Id,
                        Name = x.Image.Name,
                        Url = x.Image.Url,
                    },
                }).ToList();
            Product.ProductProductGroupingMappings = Product_ProductDTO.ProductProductGroupingMappings?
                .Select(x => new ProductProductGroupingMapping
                {
                    ProductGroupingId = x.ProductGroupingId,
                    ProductGrouping = x.ProductGrouping == null ? null : new ProductGrouping
                    {
                        Id = x.ProductGrouping.Id,
                        Code = x.ProductGrouping.Code,
                        Name = x.ProductGrouping.Name,
                        ParentId = x.ProductGrouping.ParentId,
                        Path = x.ProductGrouping.Path,
                        Description = x.ProductGrouping.Description,
                    },
                }).ToList();
            Product.VariationGroupings = Product_ProductDTO.VariationGroupings?
                .Select(x => new VariationGrouping
                {
                    Id = x.Id,
                    Name = x.Name,
                    Variations = x.Variations.Select(v => new Variation
                    {
                        Id = v.Id,
                        Code = v.Code,
                        Name = v.Name,
                        VariationGroupingId = x.Id
                    }).ToList()
                }).ToList();
            Product.BaseLanguage = CurrentContext.Language;
            return Product;
        }

        private ProductFilter ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Selects = ProductSelect.Code | ProductSelect.Name
                | ProductSelect.Id | ProductSelect.ProductProductGroupingMapping
                | ProductSelect.ProductType | ProductSelect.Supplier
                | ProductSelect.Status | ProductSelect.UsedVariation;
            ProductFilter.Skip = Product_ProductFilterDTO.Skip;
            ProductFilter.Take = Product_ProductFilterDTO.Take;
            ProductFilter.OrderBy = Product_ProductFilterDTO.OrderBy;
            ProductFilter.OrderType = Product_ProductFilterDTO.OrderType;

            ProductFilter.Id = Product_ProductFilterDTO.Id;
            ProductFilter.Code = Product_ProductFilterDTO.Code;
            ProductFilter.SupplierCode = Product_ProductFilterDTO.SupplierCode;
            ProductFilter.Name = Product_ProductFilterDTO.Name;
            ProductFilter.Description = Product_ProductFilterDTO.Description;
            ProductFilter.ScanCode = Product_ProductFilterDTO.ScanCode;
            ProductFilter.ProductTypeId = Product_ProductFilterDTO.ProductTypeId;
            ProductFilter.SupplierId = Product_ProductFilterDTO.SupplierId;
            ProductFilter.BrandId = Product_ProductFilterDTO.BrandId;
            ProductFilter.UnitOfMeasureId = Product_ProductFilterDTO.UnitOfMeasureId;
            ProductFilter.UnitOfMeasureGroupingId = Product_ProductFilterDTO.UnitOfMeasureGroupingId;
            ProductFilter.SalePrice = Product_ProductFilterDTO.SalePrice;
            ProductFilter.RetailPrice = Product_ProductFilterDTO.RetailPrice;
            ProductFilter.TaxTypeId = Product_ProductFilterDTO.TaxTypeId;
            ProductFilter.StatusId = Product_ProductFilterDTO.StatusId;
            ProductFilter.OtherName = Product_ProductFilterDTO.OtherName;
            ProductFilter.TechnicalName = Product_ProductFilterDTO.TechnicalName;
            ProductFilter.Note = Product_ProductFilterDTO.Note;
            ProductFilter.Search = Product_ProductFilterDTO.Search;
            ProductFilter.ProductGroupingId = Product_ProductFilterDTO.ProductGroupingId;
            ProductFilter.UsedVariationId = Product_ProductFilterDTO.UsedVariationId;
            return ProductFilter;
        }

        [Route(ProductRoute.CountProductGrouping), HttpPost]
        public async Task<long> CountProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Id = Product_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = Product_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = Product_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.ParentId = Product_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = Product_ProductGroupingFilterDTO.Path;
            ProductGroupingFilter.Description = Product_ProductGroupingFilterDTO.Description;
            return await ProductGroupingService.Count(ProductGroupingFilter);
        }

        [Route(ProductRoute.ListProductGrouping), HttpPost]
        public async Task<List<Product_ProductGroupingDTO>> ListProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = Product_ProductGroupingFilterDTO.Skip;
            ProductGroupingFilter.Take = Product_ProductGroupingFilterDTO.Take;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.ALL;
            ProductGroupingFilter.Id = Product_ProductGroupingFilterDTO.Id;
            ProductGroupingFilter.Code = Product_ProductGroupingFilterDTO.Code;
            ProductGroupingFilter.Name = Product_ProductGroupingFilterDTO.Name;
            ProductGroupingFilter.ParentId = Product_ProductGroupingFilterDTO.ParentId;
            ProductGroupingFilter.Path = Product_ProductGroupingFilterDTO.Path;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<Product_ProductGroupingDTO> Product_ProductGroupingDTOs = ProductGroupings
                .Select(x => new Product_ProductGroupingDTO(x)).ToList();
            return Product_ProductGroupingDTOs;
        }

        [Route(ProductRoute.CountItem), HttpPost]
        public async Task<long> CountItem([FromBody] Product_ItemFilterDTO Product_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Id = Product_ItemFilterDTO.Id;
            ItemFilter.Code = Product_ItemFilterDTO.Code;
            ItemFilter.Name = Product_ItemFilterDTO.Name;
            ItemFilter.ProductId = Product_ItemFilterDTO.ProductId;
            ItemFilter.SalePrice = Product_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Product_ItemFilterDTO.ScanCode;
            ItemFilter.RetailPrice = Product_ItemFilterDTO.RetailPrice;
            return await ItemService.Count(ItemFilter);
        }

        [Route(ProductRoute.ListItem), HttpPost]
        public async Task<List<Product_ItemDTO>> ListItem([FromBody] Product_ItemFilterDTO Product_ItemFilterDTO)
        {
            ItemFilter ItemFilter = new ItemFilter();
            ItemFilter.Skip = Product_ItemFilterDTO.Skip;
            ItemFilter.Take = Product_ItemFilterDTO.Take;
            ItemFilter.OrderBy = ItemOrder.Id;
            ItemFilter.OrderType = OrderType.ASC;
            ItemFilter.Selects = ItemSelect.ALL;
            ItemFilter.Id = Product_ItemFilterDTO.Id;
            ItemFilter.Code = Product_ItemFilterDTO.Code;
            ItemFilter.Name = Product_ItemFilterDTO.Name;
            ItemFilter.ProductId = Product_ItemFilterDTO.ProductId;
            ItemFilter.SalePrice = Product_ItemFilterDTO.SalePrice;
            ItemFilter.ScanCode = Product_ItemFilterDTO.ScanCode;
            ItemFilter.RetailPrice = Product_ItemFilterDTO.RetailPrice;

            List<Item> Items = await ItemService.List(ItemFilter);
            List<Product_ItemDTO> Product_ItemDTOs = Items
                .Select(x => new Product_ItemDTO(x)).ToList();
            return Product_ItemDTOs;
        }

        [Route(ProductRoute.CountInventory), HttpPost]
        public async Task<long> CountInventory([FromBody] Product_InventoryFilterDTO Product_InventoryFilterDTO)
        {
            InventoryFilter InventoryFilter = new InventoryFilter();
            InventoryFilter.Id = Product_InventoryFilterDTO.ItemId;
            return await InventoryService.Count(InventoryFilter);
        }
        [Route(ProductRoute.ListInventory), HttpPost]
        public async Task<List<Product_InventoryDTO>> ListInventory([FromBody] Product_InventoryFilterDTO Product_InventoryFilterDTO)
        {
            InventoryFilter InventoryFilter = new InventoryFilter();
            InventoryFilter.Skip = Product_InventoryFilterDTO.Skip;
            InventoryFilter.Take = Product_InventoryFilterDTO.Take;
            InventoryFilter.OrderBy = InventoryOrder.Id;
            InventoryFilter.OrderType = OrderType.ASC;
            InventoryFilter.Selects = InventorySelect.ALL;
            InventoryFilter.Id = Product_InventoryFilterDTO.ItemId;

            List<Inventory> Inventories = await InventoryService.List(InventoryFilter);
            List<Product_InventoryDTO> Product_InventoryDTOs = Inventories
                .Select(x => new Product_InventoryDTO(x)).ToList();
            return Product_InventoryDTOs;
        }
    }
}

