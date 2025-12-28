// <copyright file="CancelButtonTagHelper.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Helpers.TagHelpers
{
    using Microsoft.AspNetCore.Razor.TagHelpers;
    using NflPlayoffPool.Web.Extensions;

    public class CancelButtonTagHelper : TagHelper
    {
        public string? TagName { get; }
        public string? Text { get; set; }
        public string? CssClass { get; set; }

        public CancelButtonTagHelper()
        {
            this.TagName = "button";
            this.Text = "Cancel";
            this.CssClass = "btn pp-button pp-button-secondary";
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = this.TagName;

            if (this.CssClass.HasValue())
            {
                output.Attributes.Add("class", this.CssClass);
            }

            output.Attributes.Add("data-bs-dismiss", "modal");

            output.Content.SetContent(this.Text);
        }
    }
}
