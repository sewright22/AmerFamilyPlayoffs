namespace AmerFamilyPlayoffs.Shared
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class Matchup : ComponentBase
    {
        [Parameter]
        public GameModel Game { get; set; }
    }
}
