using System;
using System.Linq;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Compilation.ControlTree;
using DotVVM.Framework.Controls;

namespace GridViewGrouping.Helpers
{
    public class GroupInfoPropertyDataContextChangeAttribute : DataContextChangeAttribute
    {
        public override int Order { get; }

        public GroupInfoPropertyDataContextChangeAttribute(int order)
        {
            Order = order;
        }

        public override ITypeDescriptor? GetChildDataContextType(ITypeDescriptor dataContext, IDataContextStack controlContextStack, IAbstractControl control, IPropertyDescriptor? property = null)
        {
            return dataContext.TryGetPropertyType("GroupInfo");
        }

        public override Type? GetChildDataContextType(Type dataContext, DataContextStack controlContextStack, DotvvmBindableObject control, DotvvmProperty? property = null)
        {
            if (!dataContext.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IHasGroupInfo<>)))
            {
                throw new DotvvmControlException(control, "The GroupingGridView control can be used only on DataSets which items implement IHasGroupInfo<T> interface.");
            }

            return dataContext.GetProperty("GroupInfo")!.PropertyType;
        }
    }
}