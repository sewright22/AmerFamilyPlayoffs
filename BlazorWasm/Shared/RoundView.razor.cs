namespace AmerFamilyPlayoffs.Shared
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class RoundView : ComponentBase
    {
        [Parameter]
        public RoundModel Round { get; set; }

        [Parameter]
        public string RoundName { get; set; }
    }
}
