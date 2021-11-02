using System.Linq;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Hosting;

namespace GridViewGrouping.Helpers
{
    public class GroupingGridViewRowDecorator : Decorator
    {
        
        protected override void OnLoad(IDotvvmRequestContext context)
        {
            var gridView = GetAllAncestors().OfType<GroupingGridView>().First();

            var row = new HtmlGenericControl("tr")
            {
                Attributes =
                {
                    { "data-bind", "if: GroupInfo() && GroupInfo().IsFirstEntryInGroup"}
                }
            };
            var column = new HtmlGenericControl("td")
            {
                Attributes =
                {
                    { "data-bind", "with: GroupInfo" },
                    { "colspan", gridView.Columns.Count.ToString() },
                    { "class", "gridview-group-header" }
                }
            };
            gridView.GroupHeaderTemplate.BuildContent(context, column);
            row.Children.Add(column);
            this.Children.Insert(0, row);

            base.OnLoad(context);
        }
    }
}