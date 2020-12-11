namespace AmerFamilyPlayoffs.Pages
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    public partial class LeagueAdmin : ComponentBase
    {
        private string selectedYear;

        string SelectedYear
        {
            get
            {
                return this.selectedYear;
            }
            set
            {
                this.selectedYear = value;
            }
        }
    }
}
