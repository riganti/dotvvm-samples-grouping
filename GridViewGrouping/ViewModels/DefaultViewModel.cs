using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotVVM.Framework.Controls;
using DotVVM.Framework.ViewModel;
using DotVVM.Framework.Hosting;
using GridViewGrouping.Model;
using GridViewGrouping.Services;

namespace GridViewGrouping.ViewModels
{
    public class DefaultViewModel : MasterPageViewModel
    {
        private readonly ProductsService productsService;

        public GridViewDataSet<ProductListDto> Products { get; set; } = new()
        {
            SortingOptions = 
            {
                SortExpression = nameof(ProductListDto.ProductId)
            },
            PagingOptions =
            {
                PageSize = 20
            }
        };

        public ProductGrouping ProductGrouping { get; set; } = ProductGrouping.None;

        public ProductGrouping[] AllGroupings => new[] { ProductGrouping.None, ProductGrouping.Category, ProductGrouping.Supplier };

        public DefaultViewModel(ProductsService productsService)
        {
            this.productsService = productsService;
        }

        public override async Task PreRender()
        {
            if (Products.IsRefreshRequired)
            {
                await productsService.GetProducts(Products, ProductGrouping);
            }

            await base.PreRender();
        }
    }
}
