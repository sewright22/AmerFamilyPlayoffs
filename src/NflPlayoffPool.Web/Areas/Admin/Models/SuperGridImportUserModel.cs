// <copyright file="SuperGridImportUserModel.cs" company="stevencodeswright">
// Copyright (c) stevencodeswright. All rights reserved.
// </copyright>

namespace NflPlayoffPool.Web.Areas.Admin.Models
{
    public class SuperGridImportUserModel
    {
        public string Name { get; set; }

        public List<string> Picks { get; set; } = new List<string>();
        public string? MatchedUserId { get; set; }
    }
}
