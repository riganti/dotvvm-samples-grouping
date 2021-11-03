using System;
using System.Linq;
using System.Linq.Expressions;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Binding.Expressions;
using DotVVM.Framework.Compilation.ControlTree;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GridViewGrouping.Helpers
{
    public class GroupingGridViewRowDecorator : Decorator
    {

        [MarkupOptions(AllowHardCodedValue = false, MappingMode = MappingMode.InnerElement)]
        public ITemplate GroupHeaderTemplate
        {
            get { return (ITemplate)GetValue(GroupHeaderTemplateProperty); }
            set { SetValue(GroupHeaderTemplateProperty, value); }
        }
        public static readonly DotvvmProperty GroupHeaderTemplateProperty
            = DotvvmProperty.Register<ITemplate, GroupingGridViewRowDecorator>(c => c.GroupHeaderTemplate);
        

        protected override void OnLoad(IDotvvmRequestContext context)
        {
            var groupHeaderTemplate = GetValue<ITemplate>(GroupHeaderTemplateProperty) ?? new DefaultGroupHeaderTemplate(context.Services.GetRequiredService<BindingCompilationService>());

            var gridView = GetAllAncestors().OfType<GridView>().First();
            var row = new HtmlGenericControl("tr")
            {
                Attributes =
                {
                    { "data-bind", "if: GroupInfo() && GroupInfo().IsFirstEntryInGroup"}
                }
            };
            Children.Insert(0, row);

            var column = new HtmlGenericControl("td")
            {
                Attributes =
                {
                    { "colspan", gridView.Columns.Count.ToString() },
                    { "class", "gridview-group-header" }
                }
            };
            row.Children.Add(column);
            groupHeaderTemplate.BuildContent(context, column);

            base.OnLoad(context);
        }

        public class DefaultGroupHeaderTemplate : ITemplate
        {
            private readonly BindingCompilationService bindingCompilationService;

            public DefaultGroupHeaderTemplate(BindingCompilationService bindingCompilationService)
            {
                this.bindingCompilationService = bindingCompilationService;
            }

            public void BuildContent(IDotvvmRequestContext context, DotvvmControl container)
            {
                var literal = new Literal();
                container.Children.Add(literal);

                var dataContextStack = container.GetDataContextType();
                var expr = CreateBinding(container, dataContextStack);

                literal.SetBinding(Literal.TextProperty, ValueBindingExpression.CreateBinding(bindingCompilationService, expr, dataContextStack));
            }

            private static Expression<Func<object[], string>> CreateBinding(DotvvmControl container, DataContextStack? dataContextStack)
            {
                var groupInfoType = dataContextStack.DataContextType.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHasGroupInfo<>));
                if (groupInfoType == null)
                {
                    throw new DotvvmControlException(container, "The GroupingGridViewRowDecorator can be used only in objects which implement IHasGroupInfo<> interface!");
                }

                var param = Expression.Parameter(typeof(object[]));
                var expr = Expression.Lambda<Func<object[]?, string>>(
                    Expression.Property(
                        Expression.Property(
                            Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(0)), groupInfoType), 
                            "GroupInfo"),
                        nameof(GroupInfo.GroupDisplayName)
                    ), param);
                return expr;
            }
        }
    }
}