namespace AmerFamilyPlayoffs.Shared
{
    using AmerFamilyPlayoffs.Data;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public partial class SeasonTeamList : ComponentBase
    {
        [Parameter]
        public string Year { get; set; }
        private string errorMessage;
        Team[] Teams;

        [Inject]
        HttpClient HttpClient { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                var response = await this.HttpClient.GetAsync($"Teams?Season={Year}");
                var teams = await response.Content.ReadFromJsonAsync<List<Team>>();
                Teams = teams.ToArray();
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }
        }
    }
}
