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
        private string selectedConference;

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

        string SelectedConference
        {
            get
            {
                return this.selectedConference;
            }
            set
            {
                this.selectedConference = value;
            }
        }
    }
}
