using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Buffers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace PlayoffPool.MVC.Helpers.TagHelpers
{
    [HtmlTargetElement("pp-partial", Attributes = "pp-prefix")]
    public class PartialTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.PartialTagHelper
    {
        public PartialTagHelper(ICompositeViewEngine viewEngine, IViewBufferScope viewBufferScope)
            : base(viewEngine, viewBufferScope)
        {
        }

        [HtmlAttributeName("pp-prefix")]
        public string Prefix { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AddModelPrefix();
            return base.ProcessAsync(context, output);
        }

        protected void AddModelPrefix()
        {
            if (ViewData == null)
            {
                ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());
            }

            ViewData.TemplateInfo.HtmlFieldPrefix = Prefix;
        }
    }
}
