using Common;
using DMS.Entities;
using DMS.Enums;
using DMS.Services.MBrand;
using DMS.Services.MImage;
using DMS.Services.MItem;
using DMS.Services.MProduct;
using DMS.Services.MProductGrouping;
using DMS.Services.MProductType;
using DMS.Services.MStatus;
using DMS.Services.MSupplier;
using DMS.Services.MTaxType;
using DMS.Services.MUnitOfMeasure;
using DMS.Services.MUnitOfMeasureGrouping;
using DMS.Services.MVariationGrouping;
using Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using DMS.Helpers;
using System;
using System.Data;
using Microsoft.AspNetCore.Hosting;

namespace DMS.Rpc.product
{
    public class ProductRoute : Root
    {
        public const string Master = Module + "/product/product-master";
        public const string Detail = Module + "/product/product-detail";
        private const string Default = Rpc + Module + "/product";
        public const string Count = Default + "/count";
        public const string List = Default + "/list";
        public const string Get = Default + "/get";
        public const string Create = Default + "/create";
        public const string Update = Default + "/update";
        public const string Delete = Default + "/delete";
        public const string Import = Default + "/import";
        public const string Export = Default + "/export";
        public const string ExportTemplate = Default + "/exportTemplate";
        public const string BulkDelete = Default + "/bulk-delete";

        public const string SingleListBrand = Default + "/single-list-brand";
        public const string SingleListProductType = Default + "/single-list-product-type";
        public const string SingleListStatus = Default + "/single-list-status";
        public const string SingleListSupplier = Default + "/single-list-supplier";
        public const string SingleListTaxType = Default + "/single-list-tax-type";
        public const string SingleListUnitOfMeasure = Default + "/single-list-unit-of-measure";
        public const string SingleListUnitOfMeasureGrouping = Default + "/single-list-unit-of-measure-grouping";
        public const string SingleListItem = Default + "/single-list-item";
        public const string SingleListImage = Default + "/single-list-image";
        public const string SingleListProductGrouping = Default + "/single-list-product-grouping";
        public const string SingleListVariationGrouping = Default + "/single-list-variation-grouping";
        public const string CountImage = Default + "/count-image";
        public const string ListImage = Default + "/list-image";
        public const string CountProductGrouping = Default + "/count-product-grouping";
        public const string ListProductGrouping = Default + "/list-product-grouping";
        public static Dictionary<string, FieldType> Filters = new Dictionary<string, FieldType>
        {
            { nameof(ProductFilter.Id), FieldType.ID },
            { nameof(ProductFilter.Code), FieldType.STRING },
            { nameof(ProductFilter.SupplierCode), FieldType.STRING },
            { nameof(ProductFilter.Name), FieldType.STRING },
            { nameof(ProductFilter.Description), FieldType.STRING },
            { nameof(ProductFilter.ScanCode), FieldType.STRING },
            { nameof(ProductFilter.ProductTypeId), FieldType.ID },
            { nameof(ProductFilter.SupplierId), FieldType.ID },
            { nameof(ProductFilter.BrandId), FieldType.ID },
            { nameof(ProductFilter.UnitOfMeasureId), FieldType.ID },
            { nameof(ProductFilter.UnitOfMeasureGroupingId), FieldType.ID },
            { nameof(ProductFilter.SalePrice), FieldType.DECIMAL },
            { nameof(ProductFilter.RetailPrice), FieldType.DECIMAL },
            { nameof(ProductFilter.TaxTypeId), FieldType.ID },
            { nameof(ProductFilter.StatusId), FieldType.ID },
        };
    }

    public class ProductController : RpcController
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
        private IProductGroupingService ProductGroupingService;
        private IVariationGroupingService VariationGroupingService;
        private IProductService ProductService;
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
            IProductGroupingService ProductGroupingService,
            IVariationGroupingService VariationGroupingService,
            IProductService ProductService,
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
            this.ProductGroupingService = ProductGroupingService;
            this.VariationGroupingService = VariationGroupingService;
            this.ProductService = ProductService;
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
        public async Task<ActionResult<bool>> Import(IFormFile file)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            string Error = "";
            List<string> Errors = new List<string>();
            List<Product> Products = new List<Product>();

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
            DataFile DataFile = new DataFile
            {
                Name = file.FileName,
                Content = file.OpenReadStream(),
            };
            using (ExcelPackage excelPackage = new ExcelPackage(DataFile.Content))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return null;

                #region Khai báo thứ tự các cột trong Exel file 
                int StartColumn = 1;
                int StartRow = 1;
                //Mã sản phẩm
                int CodeColumn = 1 + StartColumn;
                //Tên sản phẩm
                int NameColumn = 2 + StartColumn;
                //Nhóm sản phẩm
                int ProductGroupNameColumn = 3 + StartColumn;
                //Loại sản phẩm
                int ProductTypeNameColumn = 4 + StartColumn;
                //Đơn vị tính
                int UoMNameColumn = 5 + StartColumn;
                //NHóm đơn vị tính
                int UoMGroupNameColumn = 6 + StartColumn;
                //Nhà cung cấp
                int SupplierNameColumn = 7 + StartColumn;
                //Mã ERP
                int ERPCodeColumn = 8 + StartColumn;
                //Mã nhận diện sản phẩm
                int ScanCodeColumn = 9 + StartColumn;
                //Nhãn hiệu
                int BrandNameColumn = 10 + StartColumn;
                //Tên khác
                int OtherNameColumn = 11 + StartColumn;
                //Tên kỹ thuật
                int TechnicalNameColumn = 12 + StartColumn;
                //Mô tả
                int DescriptionColumn = 13 + StartColumn;
                //Giá bánr
                int RetailPriceColumn = 14 + StartColumn;
                //Giá bán đề xuất
                int SalePriceColumn = 15 + StartColumn;

                //Danh sách thuộc tính làm sau nhé
                //Thuộc tính 1
                int Property1Column = 16 + StartColumn;
                //Giá trị 1
                int PropertyValue1Column = 17 + StartColumn;
                //Thuộc tính 2
                int Property2Column = 18 + StartColumn;
                //Giá trị 2
                int PropertyValue2Column = 19 + StartColumn;
                //Thuộc tính 3
                int Property3Column = 20 + StartColumn;
                //Giá trị 3
                int PropertyValue3Column = 21 + StartColumn;
                //Thuộc tính 4
                int Property4Column = 22 + StartColumn;
                //Giá trị 4
                int PropertyValue4Column = 23 + StartColumn;
                #endregion

                for (int i = StartRow; i <= worksheet.Dimension.End.Row; i++)
                {
                    if (string.IsNullOrEmpty(worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString()))
                        break;
                    string CodeValue = worksheet.Cells[i + StartRow, CodeColumn].Value?.ToString();
                    string NameValue = worksheet.Cells[i + StartRow, NameColumn].Value?.ToString();
                    string ProductGroupNameValue = worksheet.Cells[i + StartRow, ProductGroupNameColumn].Value?.ToString();
                    string ProductTypeNameValue = worksheet.Cells[i + StartRow, ProductTypeNameColumn].Value?.ToString();
                    string UoMNameValue = worksheet.Cells[i + StartRow, UoMNameColumn].Value?.ToString();
                    string UoMGroupNameValue = worksheet.Cells[i + StartRow, UoMGroupNameColumn].Value?.ToString();
                    string SupplierNameValue = worksheet.Cells[i + StartRow, SupplierNameColumn].Value?.ToString();
                    string ERPCodeValue = worksheet.Cells[i + StartRow, ERPCodeColumn].Value?.ToString();
                    string ScanCodeValue = worksheet.Cells[i + StartRow, ScanCodeColumn].Value?.ToString();
                    string BrandNameValue = worksheet.Cells[i + StartRow, BrandNameColumn].Value?.ToString();
                    string OtherNameValue = worksheet.Cells[i + StartRow, OtherNameColumn].Value?.ToString();
                    string TechnicalNameValue = worksheet.Cells[i + StartRow, TechnicalNameColumn].Value?.ToString();
                    string DescriptionValue = worksheet.Cells[i + StartRow, DescriptionColumn].Value?.ToString();
                    string RetailPriceValue = worksheet.Cells[i + StartRow, RetailPriceColumn].Value?.ToString();
                    string SalePriceValue = worksheet.Cells[i + StartRow, SalePriceColumn].Value?.ToString();
                    //Thuộc tính
                    string Property1Value = worksheet.Cells[i + StartRow, Property1Column].Value?.ToString();
                    string PropertyValue1Value = worksheet.Cells[i + StartRow, PropertyValue1Column].Value?.ToString();
                    string Property2Value = worksheet.Cells[i + StartRow, Property2Column].Value?.ToString();
                    string PropertyValue2Value = worksheet.Cells[i + StartRow, PropertyValue2Column].Value?.ToString();
                    string Property3Value = worksheet.Cells[i + StartRow, Property3Column].Value?.ToString();
                    string PropertyValue3Value = worksheet.Cells[i + StartRow, PropertyValue3Column].Value?.ToString();
                    string Property4Value = worksheet.Cells[i + StartRow, Property4Column].Value?.ToString();
                    string PropertyValue4Value = worksheet.Cells[i + StartRow, PropertyValue4Column].Value?.ToString();

                    Product Product = new Product();
                    Product.Code = CodeValue;
                    Product.Name = NameValue;
                    //Thêm vào bảng ProductProductGroupingMapping
                    ProductGrouping ProductGrouping = ProductGroupings.FirstOrDefault(p => !string.IsNullOrEmpty(p.Name) && p.Name.Trim().ToLower() == ProductGroupNameValue.Trim().ToLower());
                    if (ProductGrouping != null)
                    {
                        ProductProductGroupingMapping ProductProductGroupingMapping = new ProductProductGroupingMapping();
                        List<ProductProductGroupingMapping> ProductProductGroupingMappings = new List<ProductProductGroupingMapping>();

                        ProductProductGroupingMapping.ProductGroupingId = ProductGrouping.Id;
                        ProductProductGroupingMappings.Add(ProductProductGroupingMapping);
                        Product.ProductProductGroupingMappings = ProductProductGroupingMappings;
                    }

                    if (!string.IsNullOrEmpty(ProductTypeNameValue))
                    {
                        ProductType ProductType = ProductTypes.FirstOrDefault(p => p.Name.Equals(ProductTypeNameValue));
                        Product.ProductType = ProductType;
                        Product.ProductTypeId = ProductType != null ? ProductType.Id : 0;
                    }
                    if (!string.IsNullOrEmpty(UoMNameValue))
                    {
                        UnitOfMeasure UnitOfMeasure = UnitOfMeasures.FirstOrDefault(p => p.Name.Equals(UoMNameValue));
                        Product.UnitOfMeasure = UnitOfMeasure;
                        Product.UnitOfMeasureId = UnitOfMeasure != null ? UnitOfMeasure.Id : 0;
                    }
                    if (!string.IsNullOrEmpty(UoMGroupNameValue))
                    {
                        UnitOfMeasureGrouping UnitOfMeasureGrouping = UnitOfMeasureGroupings.FirstOrDefault(p => p.Name.Equals(UoMGroupNameValue));
                        Product.UnitOfMeasureGrouping = UnitOfMeasureGrouping;
                        Product.UnitOfMeasureGroupingId = UnitOfMeasureGrouping != null ? UnitOfMeasureGrouping.Id : 0;
                    }
                    if (!string.IsNullOrEmpty(SupplierNameValue))
                    {
                        Supplier Supplier = Suppliers.FirstOrDefault(p => p.Name.Equals(SupplierNameValue));
                        Product.Supplier = Supplier;
                        Product.SupplierId = Supplier != null ? Supplier.Id : 0;
                    }

                    Product.ERPCode = ERPCodeValue;
                    Product.ScanCode = ScanCodeValue;
                    if (!string.IsNullOrEmpty(BrandNameValue))
                    {
                        Brand Brand = Brands.FirstOrDefault(p => p.Name.Equals(BrandNameValue));
                        Product.Brand = Brand;
                        Product.BrandId = Brand != null ? Brand.Id : 0;
                    }

                    Product.OtherName = OtherNameValue;
                    Product.TechnicalName = TechnicalNameValue;
                    Product.Description = DescriptionValue;
                    Product.RetailPrice = string.IsNullOrEmpty(RetailPriceValue) ? 0 : decimal.Parse(RetailPriceValue);
                    Product.SalePrice = string.IsNullOrEmpty(SalePriceValue) ? 0 : decimal.Parse(SalePriceValue);

                    Products.Add(Product);
                }
            }
            #region Check code trùng trong danh sách 
            var errorCode = false;
            for (int i = 0; i < Products.Count; i++)
            {
                Product Product = Products[i];
                if (Products.Where(p => p.Code == Product.Code).Count() >= 2)
                {
                    errorCode = true;
                    Error = $"Dòng {i + 1} có lỗi:";
                    Error += "Code trống hoặc đã tồn tại trong file,";
                    Errors.Add(Error);
                }
                if (Products.Where(p => p.ScanCode == Product.ScanCode).Count() >= 2)
                {
                    errorCode = true;
                    Error = $"Dòng {i + 1} có lỗi:";
                    Error += "ScanCode trống hoặc đã tồn tại trong file,";
                    Errors.Add(Error);
                }
                if (Products.Where(p => p.ERPCode == Product.ERPCode).Count() >= 2)
                {
                    errorCode = true;
                    Error = $"Dòng {i + 1} có lỗi:";
                    Error += "ERPCode trống hoặc đã tồn tại trong file,";
                    Errors.Add(Error);
                }
            }
            if (errorCode == true)
                return BadRequest(Errors);

            #endregion

            Products = await ProductService.Import(Products);

            if (Products.All(au => au.IsValidated))
                return true;
            else
            {
                for (int i = 0; i < Products.Count; i++)
                {
                    Product Product = Products[i];
                    if (!Product.IsValidated)
                    {
                        Error += $"Dòng {i + 1} có lỗi:";
                        if (Product.Errors.ContainsKey(nameof(Product.Name)))
                            Error += Product.Errors[nameof(Product.Name)] + " ";
                        if (Product.Errors.ContainsKey(nameof(Product.Code)))
                            Error += Product.Errors[nameof(Product.Code)] + " ";
                        if (Product.Errors.ContainsKey(nameof(Product.ScanCode)))
                            Error += Product.Errors[nameof(Product.ScanCode)] + " ";
                        if (Product.Errors.ContainsKey(nameof(Product.ERPCode)))
                            Error += Product.Errors[nameof(Product.ERPCode)] + " ";
                        if (Product.Errors.ContainsKey(nameof(Product.ProductTypeId)))
                            Error += Product.Errors[nameof(Product.ProductTypeId)] + " ";
                        if (Product.Errors.ContainsKey(nameof(Product.UnitOfMeasureId)))
                            Error += Product.Errors[nameof(Product.UnitOfMeasureId)] + " ";
                        Errors.Add(Error);
                    }
                }
            }
            return BadRequest(Errors);
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
                Selects = ItemSelect.ALL
            });
            MemoryStream MemoryStream = new MemoryStream();
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
                        "Mô tả",
                        "Giá bán",
                        "Giá bán lẻ đề xuất",
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
                    data.Add(new object[]
                    {
                        i+1,
                        Product.Code,
                        Product.Name,
                        string.Empty,
                        //Product.ProductProductGroupingMappings.FirstOrDefault() != null ? Product.ProductProductGroupingMappings.FirstOrDefault().ProductGrouping.Name : string.Empty,
                        Product.ProductType != null ? Product.ProductType.Name : string.Empty,
                        Product.UnitOfMeasure !=null ? Product.UnitOfMeasure.Name : string.Empty,
                        Product.UnitOfMeasureGrouping !=null ? Product.UnitOfMeasureGrouping.Name : string.Empty,
                        Product.Supplier !=null ? Product.Supplier.Name : string.Empty,
                        Product.ERPCode,
                        Product.ScanCode,
                        Product.Brand !=null ? Product.Brand.Name : string.Empty,
                        Product.OtherName,
                        Product.TechnicalName,
                        Product.Description,
                        Product.SalePrice,
                        Product.RetailPrice,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                    }
                        );
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
                                Item.Product !=null ? Item.Product.Code : string.Empty,
                                Item.Code,
                                Item.Name,
                                Item.ScanCode,
                                Item.SalePrice,
                                Item.RetailPrice,
                                });
                    excel.GenerateWorksheet("Item", ItemHeader, data);
                }
                #endregion 
                excel.Save();
            }

            return File(MemoryStream.ToArray(), "application/octet-stream", "Product" + Utils.ConvertDateTimeToString(DateTime.Now) + ".xlsx");
        }

        [Route(ProductRoute.ExportTemplate), HttpPost]
        public async Task<ActionResult> ExportTemplate()
        {
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

            var fileName = string.Format("{0}_{1}.xlsx", "ProductExport", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

            var filePath = Path.Combine(_env.ContentRootPath, "File\\Export", fileName);
            var newFile = new FileInfo(filePath);
            string tempPath = _env.ContentRootPath + "\\File\\Template\\Product_Import.xlsx";
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
                xlPackage.SaveAs(newFile);
            }
            MemoryStream MemoryStream = new MemoryStream();
            using (ExcelPackage excel = new ExcelPackage(MemoryStream))
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    MemoryStream.Write(bytes, 0, (int)file.Length);
                }
            }
            return File(MemoryStream.ToArray(), "application/octet-stream", "Product" + Utils.ConvertDateTimeToString(DateTime.Now) + ".xlsx");
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
            return true;
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
            Product.Brand = Product_ProductDTO.Brand == null ? null : new Brand
            {
                Id = Product_ProductDTO.Brand.Id,
                Code = Product_ProductDTO.Brand.Code,
                Name = Product_ProductDTO.Brand.Name,
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
            Product.Items = Product_ProductDTO.Items?
                .Select(x => new Item
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ScanCode = x.ScanCode,
                    SalePrice = x.SalePrice,
                    RetailPrice = x.RetailPrice,
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
                }).ToList();
            Product.BaseLanguage = CurrentContext.Language;
            return Product;
        }

        private ProductFilter ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Selects = ProductSelect.ALL;
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
            return ProductFilter;
        }

        [Route(ProductRoute.SingleListBrand), HttpPost]
        public async Task<List<Product_BrandDTO>> SingleListBrand([FromBody] Product_BrandFilterDTO Product_BrandFilterDTO)
        {
            BrandFilter BrandFilter = new BrandFilter();
            BrandFilter.Skip = 0;
            BrandFilter.Take = 20;
            BrandFilter.OrderBy = BrandOrder.Id;
            BrandFilter.OrderType = OrderType.ASC;
            BrandFilter.Selects = BrandSelect.ALL;
            BrandFilter.Id = Product_BrandFilterDTO.Id;
            BrandFilter.Code = Product_BrandFilterDTO.Code;
            BrandFilter.Name = Product_BrandFilterDTO.Name;
            BrandFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Brand> Brands = await BrandService.List(BrandFilter);
            List<Product_BrandDTO> Product_BrandDTOs = Brands
                .Select(x => new Product_BrandDTO(x)).ToList();
            return Product_BrandDTOs;
        }
        [Route(ProductRoute.SingleListProductType), HttpPost]
        public async Task<List<Product_ProductTypeDTO>> SingleListProductType([FromBody] Product_ProductTypeFilterDTO Product_ProductTypeFilterDTO)
        {
            ProductTypeFilter ProductTypeFilter = new ProductTypeFilter();
            ProductTypeFilter.Skip = 0;
            ProductTypeFilter.Take = 20;
            ProductTypeFilter.OrderBy = ProductTypeOrder.Id;
            ProductTypeFilter.OrderType = OrderType.ASC;
            ProductTypeFilter.Selects = ProductTypeSelect.ALL;
            ProductTypeFilter.Id = Product_ProductTypeFilterDTO.Id;
            ProductTypeFilter.Code = Product_ProductTypeFilterDTO.Code;
            ProductTypeFilter.Name = Product_ProductTypeFilterDTO.Name;
            ProductTypeFilter.Description = Product_ProductTypeFilterDTO.Description;
            ProductTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<ProductType> ProductTypes = await ProductTypeService.List(ProductTypeFilter);
            List<Product_ProductTypeDTO> Product_ProductTypeDTOs = ProductTypes
                .Select(x => new Product_ProductTypeDTO(x)).ToList();
            return Product_ProductTypeDTOs;
        }
        [Route(ProductRoute.SingleListStatus), HttpPost]
        public async Task<List<Product_StatusDTO>> SingleListStatus([FromBody] Product_StatusFilterDTO Product_StatusFilterDTO)
        {
            StatusFilter StatusFilter = new StatusFilter();
            StatusFilter.Skip = 0;
            StatusFilter.Take = 20;
            StatusFilter.OrderBy = StatusOrder.Id;
            StatusFilter.OrderType = OrderType.ASC;
            StatusFilter.Selects = StatusSelect.ALL;

            List<Status> Statuses = await StatusService.List(StatusFilter);
            List<Product_StatusDTO> Product_StatusDTOs = Statuses
                .Select(x => new Product_StatusDTO(x)).ToList();
            return Product_StatusDTOs;
        }
        [Route(ProductRoute.SingleListSupplier), HttpPost]
        public async Task<List<Product_SupplierDTO>> SingleListSupplier([FromBody] Product_SupplierFilterDTO Product_SupplierFilterDTO)
        {
            SupplierFilter SupplierFilter = new SupplierFilter();
            SupplierFilter.Skip = 0;
            SupplierFilter.Take = 20;
            SupplierFilter.OrderBy = SupplierOrder.Id;
            SupplierFilter.OrderType = OrderType.ASC;
            SupplierFilter.Selects = SupplierSelect.ALL;
            SupplierFilter.Id = Product_SupplierFilterDTO.Id;
            SupplierFilter.Code = Product_SupplierFilterDTO.Code;
            SupplierFilter.Name = Product_SupplierFilterDTO.Name;
            SupplierFilter.TaxCode = Product_SupplierFilterDTO.TaxCode;
            SupplierFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<Supplier> Suppliers = await SupplierService.List(SupplierFilter);
            List<Product_SupplierDTO> Product_SupplierDTOs = Suppliers
                .Select(x => new Product_SupplierDTO(x)).ToList();
            return Product_SupplierDTOs;
        }
        [Route(ProductRoute.SingleListTaxType), HttpPost]
        public async Task<List<Product_TaxTypeDTO>> SingleListTaxType([FromBody] Product_TaxTypeFilterDTO Product_TaxTypeFilterDTO)
        {
            TaxTypeFilter TaxTypeFilter = new TaxTypeFilter();
            TaxTypeFilter.Skip = 0;
            TaxTypeFilter.Take = 20;
            TaxTypeFilter.OrderBy = TaxTypeOrder.Id;
            TaxTypeFilter.OrderType = OrderType.ASC;
            TaxTypeFilter.Selects = TaxTypeSelect.ALL;
            TaxTypeFilter.Id = Product_TaxTypeFilterDTO.Id;
            TaxTypeFilter.Code = Product_TaxTypeFilterDTO.Code;
            TaxTypeFilter.Name = Product_TaxTypeFilterDTO.Name;
            TaxTypeFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<TaxType> TaxTypes = await TaxTypeService.List(TaxTypeFilter);
            List<Product_TaxTypeDTO> Product_TaxTypeDTOs = TaxTypes
                .Select(x => new Product_TaxTypeDTO(x)).ToList();
            return Product_TaxTypeDTOs;
        }
        [Route(ProductRoute.SingleListUnitOfMeasure), HttpPost]
        public async Task<List<Product_UnitOfMeasureDTO>> SingleListUnitOfMeasure([FromBody] Product_UnitOfMeasureFilterDTO Product_UnitOfMeasureFilterDTO)
        {
            UnitOfMeasureFilter UnitOfMeasureFilter = new UnitOfMeasureFilter();
            UnitOfMeasureFilter.Skip = 0;
            UnitOfMeasureFilter.Take = 20;
            UnitOfMeasureFilter.OrderBy = UnitOfMeasureOrder.Id;
            UnitOfMeasureFilter.OrderType = OrderType.ASC;
            UnitOfMeasureFilter.Selects = UnitOfMeasureSelect.ALL;
            UnitOfMeasureFilter.Id = Product_UnitOfMeasureFilterDTO.Id;
            UnitOfMeasureFilter.Code = Product_UnitOfMeasureFilterDTO.Code;
            UnitOfMeasureFilter.Name = Product_UnitOfMeasureFilterDTO.Name;
            UnitOfMeasureFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<UnitOfMeasure> UnitOfMeasures = await UnitOfMeasureService.List(UnitOfMeasureFilter);
            List<Product_UnitOfMeasureDTO> Product_UnitOfMeasureDTOs = UnitOfMeasures
                .Select(x => new Product_UnitOfMeasureDTO(x)).ToList();
            return Product_UnitOfMeasureDTOs;
        }
        [Route(ProductRoute.SingleListUnitOfMeasureGrouping), HttpPost]
        public async Task<List<Product_UnitOfMeasureGroupingDTO>> SingleListUnitOfMeasureGrouping([FromBody] Product_UnitOfMeasureGroupingFilterDTO Product_UnitOfMeasureGroupingFilterDTO)
        {
            UnitOfMeasureGroupingFilter UnitOfMeasureGroupingFilter = new UnitOfMeasureGroupingFilter();
            UnitOfMeasureGroupingFilter.Skip = 0;
            UnitOfMeasureGroupingFilter.Take = 20;
            UnitOfMeasureGroupingFilter.OrderBy = UnitOfMeasureGroupingOrder.Id;
            UnitOfMeasureGroupingFilter.OrderType = OrderType.ASC;
            UnitOfMeasureGroupingFilter.Selects = UnitOfMeasureGroupingSelect.ALL;
            UnitOfMeasureGroupingFilter.Id = Product_UnitOfMeasureGroupingFilterDTO.Id;
            UnitOfMeasureGroupingFilter.Name = Product_UnitOfMeasureGroupingFilterDTO.Name;
            UnitOfMeasureGroupingFilter.UnitOfMeasureId = Product_UnitOfMeasureGroupingFilterDTO.UnitOfMeasureId;
            UnitOfMeasureGroupingFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id };

            List<UnitOfMeasureGrouping> UnitOfMeasureGroupings = await UnitOfMeasureGroupingService.List(UnitOfMeasureGroupingFilter);
            List<Product_UnitOfMeasureGroupingDTO> Product_UnitOfMeasureGroupingDTOs = UnitOfMeasureGroupings
                .Select(x => new Product_UnitOfMeasureGroupingDTO(x)).ToList();
            return Product_UnitOfMeasureGroupingDTOs;
        }

        [Route(ProductRoute.SingleListProductGrouping), HttpPost]
        public async Task<List<Product_ProductGroupingDTO>> SingleListProductGrouping([FromBody] Product_ProductGroupingFilterDTO Product_ProductGroupingFilterDTO)
        {
            ProductGroupingFilter ProductGroupingFilter = new ProductGroupingFilter();
            ProductGroupingFilter.Skip = 0;
            ProductGroupingFilter.Take = int.MaxValue;
            ProductGroupingFilter.OrderBy = ProductGroupingOrder.Id;
            ProductGroupingFilter.OrderType = OrderType.ASC;
            ProductGroupingFilter.Selects = ProductGroupingSelect.Id | ProductGroupingSelect.Code
                | ProductGroupingSelect.Name | ProductGroupingSelect.Parent;

            List<ProductGrouping> ProductGroupings = await ProductGroupingService.List(ProductGroupingFilter);
            List<Product_ProductGroupingDTO> Product_ProductGroupingDTOs = ProductGroupings
                .Select(x => new Product_ProductGroupingDTO(x)).ToList();
            return Product_ProductGroupingDTOs;
        }

        [Route(ProductRoute.CountImage), HttpPost]
        public async Task<long> CountImage([FromBody] Product_ImageFilterDTO Product_ImageFilterDTO)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Id = Product_ImageFilterDTO.Id;
            ImageFilter.Name = Product_ImageFilterDTO.Name;
            ImageFilter.Url = Product_ImageFilterDTO.Url;

            return await ImageService.Count(ImageFilter);
        }

        [Route(ProductRoute.ListImage), HttpPost]
        public async Task<List<Product_ImageDTO>> ListImage([FromBody] Product_ImageFilterDTO Product_ImageFilterDTO)
        {
            ImageFilter ImageFilter = new ImageFilter();
            ImageFilter.Skip = Product_ImageFilterDTO.Skip;
            ImageFilter.Take = Product_ImageFilterDTO.Take;
            ImageFilter.OrderBy = ImageOrder.Id;
            ImageFilter.OrderType = OrderType.ASC;
            ImageFilter.Selects = ImageSelect.ALL;
            ImageFilter.Id = Product_ImageFilterDTO.Id;
            ImageFilter.Name = Product_ImageFilterDTO.Name;
            ImageFilter.Url = Product_ImageFilterDTO.Url;

            List<Image> Images = await ImageService.List(ImageFilter);
            List<Product_ImageDTO> Product_ImageDTOs = Images
                .Select(x => new Product_ImageDTO(x)).ToList();
            return Product_ImageDTOs;
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
    }
}

