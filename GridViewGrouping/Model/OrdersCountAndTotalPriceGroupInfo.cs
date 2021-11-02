using GridViewGrouping.Helpers;

namespace GridViewGrouping.Model
{
    public record OrdersCountAndTotalPriceGroupInfo : GroupInfo
    {

        public decimal TotalPrice { get; set; }

        public int ProductsCount { get; set; }
    }
}