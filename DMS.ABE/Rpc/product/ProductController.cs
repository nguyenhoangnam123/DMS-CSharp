using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.ABE.Common;
using DMS.ABE.Entities;
using DMS.ABE.Enums;
using DMS.ABE.Services.MCategory;
using DMS.ABE.Services.MProduct;
using Microsoft.AspNetCore.Mvc;

namespace DMS.ABE.Rpc.product
{
    public class ProductController : SimpleController
    {
        private IProductService ProductService;
        private IItemService ItemService;
        private ICategoryService CategoryService;
        private ICurrentContext CurrentContext;
        public ProductController(
             IProductService ProductService,
             ICurrentContext CurrentContext,
             ICategoryService CategoryService,
             IItemService ItemService
        )
        {
            this.ProductService = ProductService;
            this.CategoryService = CategoryService;
            this.CurrentContext = CurrentContext;
            this.ItemService = ItemService;
        }

        [Route(ProductRoute.List), HttpPost]
        public async Task<List<Product_ProductDTO>> List([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
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

        [Route(ProductRoute.ListPotential), HttpPost]
        public async Task<List<Product_ProductDTO>> ListPotential([FromBody] Product_ProductPotentialDTO Product_ProductPotentialDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if(Product_ProductPotentialDTO.ProductId == 0)
            {
                return null;
            }
            List<Product> ProductEntitiess = await ProductService.List(new ProductFilter{ 
                Skip = 0,
                Take = 1,
                Selects = ProductSelect.Category,
            });
            Product Product = ProductEntitiess.FirstOrDefault();
            if (Product == null)
            {
                return null;
            }
            CategoryFilter CategoryFilter = new CategoryFilter
            {
                Id = new IdFilter { Equal = Product.Category.Id },
                Skip = 0,
                Take = 1,
                Selects = CategorySelect.Id
            };
            List<Category> Categories = await CategoryService.List(CategoryFilter);
            Category Category = Categories.FirstOrDefault();
            if(Category == null)
            {
                return null;
            }
            ProductFilter ProductFilter = new ProductFilter {
                Id = new IdFilter { NotEqual = Product_ProductPotentialDTO.ProductId },
                CategoryId = new IdFilter { Equal = Category.Id },
                StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id },
                Skip = 0,
                Take = 20,
                Selects = ProductSelect.Code | ProductSelect.Status | ProductSelect.Name | ProductSelect.Id | ProductSelect.Category | ProductSelect.UsedVariation
            }; // lấy ra 20 sản phẩm đầu tiên
            List<Product> Products = await ProductService.List(ProductFilter);
            List<Product_ProductDTO> Product_ProductDTOs = Products
                .Select(c => new Product_ProductDTO(c)).ToList();
            return Product_ProductDTOs;
        } // chọn các sản phẩm trong nhóm sản phẩm loại trừ sản phẩm đang view detail

        [Route(ProductRoute.Count), HttpPost]
        public async Task<int> Count([FromBody] Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            ProductFilter ProductFilter = ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO);
            ProductFilter = ProductService.ToFilter(ProductFilter);
            int count = await ProductService.Count(ProductFilter);
            return count;
        }

        [Route(ProductRoute.Get), HttpPost]
        public async Task<Product_ProductDTO> Get([FromBody] Product_ProductDTO Product_ProductDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            Product Product = await ProductService.Get(Product_ProductDTO.Id);
            if (Product == null)
                return null;
            return new Product_ProductDTO(Product);
        }

        [Route(ProductRoute.GetItem), HttpPost]
        public async Task<Product_ItemDTO> GetItem([FromBody] Product_ItemDTO Product_ItemDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long Id = Product_ItemDTO.Id;
            if (Id == 0)
            {
                return null;
            }
            Item Item = await ItemService.Get(Id);
            if (Item == null)
                return null;
            return new Product_ItemDTO(Item);
        }

        [Route(ProductRoute.GetItemByVariation), HttpPost]
        public async Task<Product_ItemDTO> GetItemByVariation([FromBody] Product_ProductDetailDTO Product_ProductDetailDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            long ProductId = Product_ProductDetailDTO.ProductId;
            List<long> VariationIds = Product_ProductDetailDTO.VariationIds;
            if (ProductId == 0)
            {
                return null;
            }
            Item Item = await ItemService.GetItemByVariation(ProductId, VariationIds);
            if (Item == null)
                return null;
            return new Product_ItemDTO(Item);
        }

        [Route(ProductRoute.CountCategory), HttpPost]
        public async Task<int> CountCategory([FromBody] Product_CategoryFilterDTO Product_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            CategoryFilter CategoryFilter = new CategoryFilter
            {
                Id = Product_CategoryFilterDTO.Id,
                Code = Product_CategoryFilterDTO.Code,
                Level = Product_CategoryFilterDTO.Level,
                ParentId = Product_CategoryFilterDTO.ParentId,
                Path = Product_CategoryFilterDTO.Path,
                Name = Product_CategoryFilterDTO.Name,
                OrderBy = Product_CategoryFilterDTO.OrderBy,
                OrderType = Product_CategoryFilterDTO.OrderType,
                Skip = Product_CategoryFilterDTO.Skip,
                Take = Product_CategoryFilterDTO.Take,
                HasChildren = Product_CategoryFilterDTO.HasChildren,
                Selects = CategorySelect.Id | CategorySelect.Code | CategorySelect.Name | CategorySelect.Image | CategorySelect.Parent | CategorySelect.Path | CategorySelect.Level
            };
            int count = await CategoryService.Count(CategoryFilter);
            return count;
        }

        [Route(ProductRoute.ListCategory), HttpPost]
        public async Task<List<Product_CategoryDTO>> ListCategory([FromBody] Product_CategoryFilterDTO Product_CategoryFilterDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            CategoryFilter CategoryFilter = new CategoryFilter
            {
                Id = Product_CategoryFilterDTO.Id,
                Code = Product_CategoryFilterDTO.Code,
                Level = Product_CategoryFilterDTO.Level,
                ParentId = Product_CategoryFilterDTO.ParentId,
                Path = Product_CategoryFilterDTO.Path,
                Name = Product_CategoryFilterDTO.Name,
                OrderBy = Product_CategoryFilterDTO.OrderBy,
                OrderType = Product_CategoryFilterDTO.OrderType,
                Skip = Product_CategoryFilterDTO.Skip,
                Take = Product_CategoryFilterDTO.Take,
                HasChildren = Product_CategoryFilterDTO.HasChildren,
                Selects = CategorySelect.Id | CategorySelect.Code | CategorySelect.Name | CategorySelect.Image | CategorySelect.Parent | CategorySelect.Path | CategorySelect.Level
            };
            List<Category> Categories = await CategoryService.List(CategoryFilter);
            List<Product_CategoryDTO> Category_CategoryDTOs = Categories
                .Select(c => new Product_CategoryDTO(c)).ToList();
            return Category_CategoryDTOs;
        }

        public ProductFilter ConvertFilterDTOToFilterEntity(Product_ProductFilterDTO Product_ProductFilterDTO)
        {
            ProductFilter ProductFilter = new ProductFilter();
            ProductFilter.Selects = ProductSelect.Code | ProductSelect.Name
                | ProductSelect.Id | ProductSelect.ProductProductGroupingMapping | ProductSelect.ProductType
                | ProductSelect.Status | ProductSelect.UsedVariation | ProductSelect.Category;
            ProductFilter.Skip = Product_ProductFilterDTO.Skip;
            ProductFilter.Take = Product_ProductFilterDTO.Take;
            ProductFilter.OrderBy = Product_ProductFilterDTO.OrderBy;
            ProductFilter.OrderType = OrderType.DESC; // mặc định từ mới -> cũ


            ProductFilter.Description = Product_ProductFilterDTO.Description;
            ProductFilter.StatusId = new IdFilter { Equal = StatusEnum.ACTIVE.Id }; // mặc định sản phẩm đang active
            ProductFilter.Search = Product_ProductFilterDTO.Search;
            ProductFilter.CategoryId = Product_ProductFilterDTO.CategoryId;
            ProductFilter.IsNew = Product_ProductFilterDTO.IsNew; // filter sản phẩm mới
            return ProductFilter;
        }

        public CategoryFilter ConvertCategoryFilterDTOToEntity(Product_CategoryFilterDTO Product_CategoryFilterDTO)
        {
            CategoryFilter CategoryFilter = new CategoryFilter();
            CategoryFilter.Selects = CategorySelect.ALL;
            CategoryFilter.Skip = Product_CategoryFilterDTO.Skip;
            CategoryFilter.Take = Product_CategoryFilterDTO.Take;
            CategoryFilter.OrderBy = Product_CategoryFilterDTO.OrderBy;
            CategoryFilter.OrderType = OrderType.DESC; // mặc định từ mới -> cũ


            CategoryFilter.Id = Product_CategoryFilterDTO.Id;
            CategoryFilter.Code = Product_CategoryFilterDTO.Code;
            CategoryFilter.Name = Product_CategoryFilterDTO.Name;
            CategoryFilter.ParentId = Product_CategoryFilterDTO.ParentId;
            CategoryFilter.Path = Product_CategoryFilterDTO.Path;
            CategoryFilter.Level = Product_CategoryFilterDTO.Level;
            CategoryFilter.StatusId = Product_CategoryFilterDTO.StatusId;
            CategoryFilter.ImageId = Product_CategoryFilterDTO.ImageId;
            CategoryFilter.RowId = Product_CategoryFilterDTO.RowId;
            CategoryFilter.CreatedAt = Product_CategoryFilterDTO.CreatedAt;
            CategoryFilter.UpdatedAt = Product_CategoryFilterDTO.UpdatedAt;
            return CategoryFilter;
        }
    }
}
