namespace AmerFamilyPlayoffs.Shared
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class TeamSelectionTable : ComponentBase
    {
        [Parameter]
        public List<TeamModel> Teams { get; set; }
    }
}
