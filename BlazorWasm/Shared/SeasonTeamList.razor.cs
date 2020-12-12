namespace AmerFamilyPlayoffs.Shared
{
    using AmerFamilyPlayoffs.Data;
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public partial class SeasonTeamList : ComponentBase
    {
        [Parameter]
        public string Year { get; set; }
        public string Message { get; set; }
        public bool IsBusy;
        private string errorMessage;
        List<TeamModel> Teams;

        [Inject]
        HttpClient HttpClient { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                var response = await this.HttpClient.GetAsync($"Teams?Season={Year}");
                var teams = await response.Content.ReadFromJsonAsync<List<TeamModel>>();
                Teams = teams;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }
        }

        private async void SavePlayoffTeams()
        {
            await this.SavePlayoffTeamsAsync();
        }

        private async Task SavePlayoffTeamsAsync()
        {
            this.Message = "Saving...";
            var task = this.HttpClient.PostAsJsonAsync("Teams", Teams.Where(x => x.IsInPlayoffs));

            await task;

            if (task.IsCompletedSuccessfully)
            {
                this.Message = "Success";
            }
            else
            {
                this.Message = "Failed";
            }

            this.StateHasChanged();
        }
    }
}
