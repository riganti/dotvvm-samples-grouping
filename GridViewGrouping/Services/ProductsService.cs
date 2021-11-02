using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.Controls;
using GridViewGrouping.Data;
using GridViewGrouping.Helpers;
using GridViewGrouping.Model;
using Microsoft.EntityFrameworkCore;

namespace GridViewGrouping.Services
{
    public class ProductsService
    {
        private readonly NorthwindContext db;

        public ProductsService(NorthwindContext db)
        {
            this.db = db;
        }


        public async Task GetProducts(GridViewDataSet<ProductListDto> productsDataSet, ProductGrouping productGrouping)
        {
            // load products
            var queryable = db.Products.Select(p => new ProductListDto()
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                Discontinued = p.Discontinued,
                QuantityPerUnit = p.QuantityPerUnit,
                ReorderLevel = p.ReorderLevel,
                SupplierId = p.SupplierId,
                SupplierName = p.Supplier.CompanyName,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                UnitsOnOrder = p.UnitsOnOrder
            });

            if (productGrouping == ProductGrouping.Category)
            {
                productsDataSet.SortingOptions.SortExpression = nameof(ProductListDto.CategoryName);
                productsDataSet.SortingOptions.SortDescending = false;
            }
            else if (productGrouping == ProductGrouping.Supplier)
            {
                productsDataSet.SortingOptions.SortExpression = nameof(ProductListDto.SupplierName);
                productsDataSet.SortingOptions.SortDescending = false;
            }

            await productsDataSet.LoadFromQueryableAsync(queryable);

            // load group info
            if (productGrouping == ProductGrouping.Category)
            {
                await productsDataSet.ApplyGrouping(
                    p => p.CategoryId, 
                    groupKeys => db.Categories
                        .Where(c => groupKeys.Contains(c.CategoryId))
                        .Select(c => new OrdersCountAndTotalPriceGroupInfo()
                        {
                            GroupKey = c.CategoryId.ToString(),
                            GroupDisplayName = c.CategoryName,
                            ProductsCount = c.Products.Count(),
                            TotalPrice = c.Products.SelectMany(p => p.OrderDetails).Sum(d => d.UnitPrice * d.Quantity)
                        }));
            }
            else if (productGrouping == ProductGrouping.Supplier)
            {
                await productsDataSet.ApplyGrouping(
                    p => p.SupplierId,
                    groupKeys => db.Suppliers
                        .Where(s => groupKeys.Contains(s.SupplierId))
                        .Select(s => new OrdersCountAndTotalPriceGroupInfo()
                        {
                            GroupKey = s.SupplierId.ToString(),
                            GroupDisplayName = s.CompanyName,
                            ProductsCount = s.Products.Count(),
                            TotalPrice = s.Products.SelectMany(p => p.OrderDetails).Sum(d => d.UnitPrice * d.Quantity)
                        }));
            }
            else
            {
                productsDataSet.ResetGrouping();
            }
        }

    }
}
