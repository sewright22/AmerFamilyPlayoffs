namespace AmerFamilyPlayoffs.Pages
{
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public partial class Brackets : ComponentBase
    {
        [Inject]
        NavigationManager NavigationManager { get; set; }

        private void CreateNewBracket()
        {
            this.NavigationManager.NavigateTo("brackets/create");
        }
    }
}
