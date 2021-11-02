using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotVVM.Framework.Binding;
using DotVVM.Framework.Compilation.Parser.Dothtml.Parser;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Controls.Infrastructure;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.ResourceManagement;

namespace GridViewGrouping.Helpers
{
    public class GroupingGridView : GridView 
    {
        private PlaceHolder? groupHeaderTemplate;
        private string? groupHeaderTemplateId;


        [MarkupOptions(AllowHardCodedValue = false, MappingMode = MappingMode.InnerElement, Required = true)]
        [ControlPropertyBindingDataContextChange(nameof(GridView.DataSource), order: 0)]
        [CollectionElementDataContextChange(order: 1)]
        [GroupInfoPropertyDataContextChange(2)]
        public ITemplate GroupHeaderTemplate
        {
            get { return (ITemplate)GetValue(GroupHeaderTemplateProperty); }
            set { SetValue(GroupHeaderTemplateProperty, value); }
        }

        public static readonly DotvvmProperty GroupHeaderTemplateProperty
            = DotvvmProperty.Register<ITemplate, GroupingGridView>(c => c.GroupHeaderTemplate, null);

        public GroupingGridView()
        {
            RowDecorators.Add(new GroupingGridViewRowDecorator());
        }


        protected override void OnLoad(IDotvvmRequestContext context)
        {
            base.OnLoad(context);

            //groupHeaderTemplate = CreateGroupHeaderTemplate(context);
        }

        protected override void OnPreRender(IDotvvmRequestContext context)
        {
            base.OnPreRender(context);

            //groupHeaderTemplate = CreateGroupHeaderTemplate(context);
            
            //if (!context.IsPostBack || context.IsInPartialRenderingMode)
            //{
            //    groupHeaderTemplateId = context.ResourceManager.AddTemplateResource(context, groupHeaderTemplate!);
            //}
        }

        private PlaceHolder CreateGroupHeaderTemplate(IDotvvmRequestContext context)
        {
            var host = new PlaceHolder();
            GroupHeaderTemplate.BuildContent(context, host);
            return host;
        }

        protected override void RenderBeginTag(IHtmlWriter writer, IDotvvmRequestContext context)
        {
            //writer.WriteKnockoutDataBindComment("let", $"{{\"grouping-gridview-templateId\": \"{groupHeaderTemplateId}\"}}");

            base.RenderBeginTag(writer, context);
        }

        protected override void RenderEndTag(IHtmlWriter writer, IDotvvmRequestContext context)
        {
            base.RenderEndTag(writer, context);

            //writer.WriteKnockoutDataBindEndComment();
        }
    }
}
