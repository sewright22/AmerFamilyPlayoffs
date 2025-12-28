// <copyright file="EnumExtensions.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Extensions
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    public static class EnumExtensions
    {
        public static SelectListItem AsSelectListItem<TEnum>(this TEnum enumValue) where TEnum : Enum
        {
            return new SelectListItem
            {
                Value = Convert.ToInt32(enumValue).ToString(),
                Text = enumValue.ToString(),
            };
        }
    }
}
