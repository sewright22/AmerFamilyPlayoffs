namespace AmerFamilyPlayoffs.Pages
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class CreateBracket : ComponentBase
    {
        private PlayoffBracketPrediction bracket = new PlayoffBracketPrediction();

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }
    }
}
