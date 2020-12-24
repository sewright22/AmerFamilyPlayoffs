namespace AmerFamilyPlayoffs.Shared
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class MatchupList : ComponentBase
    {
        [Parameter]
        public List<GameModel> Games { get; set; }
        [Parameter] public Action Save { get; set; }
    }
}
