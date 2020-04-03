using Common;
using DMS.Entities;
using DMS.Models;
using Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Repositories
{
    public interface IProductProductGroupingMappingRepository
    {
        Task<bool> BulkMerge(List<ProductProductGroupingMapping> ProductProductGroupingMappings);
    }
    public class ProductProductGroupingMappingRepository : IProductProductGroupingMappingRepository
    {
        private DataContext DataContext;
        public ProductProductGroupingMappingRepository(DataContext DataContext)
        {
            this.DataContext = DataContext;
        }
         
        public async Task<bool> BulkMerge(List<ProductProductGroupingMapping> ProductProductGroupingMappings)
        {
            List<ProductProductGroupingMappingDAO> ProductProductGroupingMappingDAOs = new List<ProductProductGroupingMappingDAO>();
            foreach (ProductProductGroupingMapping ProductProductGroupingMapping in ProductProductGroupingMappings)
            {
                ProductProductGroupingMappingDAO ProductProductGroupingMappingDAO = new ProductProductGroupingMappingDAO();
                ProductProductGroupingMappingDAO.ProductId = ProductProductGroupingMapping.ProductId;
                ProductProductGroupingMappingDAO.ProductGroupingId = ProductProductGroupingMapping.ProductGroupingId;
                ProductProductGroupingMappingDAOs.Add(ProductProductGroupingMappingDAO);
            }
            await DataContext.BulkMergeAsync(ProductProductGroupingMappingDAOs); 
            return true;
        } 
    }
}
