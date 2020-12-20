namespace AmerFamilyPlayoffs.Pages
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public partial class Bracket : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        PlayoffBracketPrediction BracketPrediction { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var response = await this.HttpClient.GetAsync($"BracketPrediction/{Id}");
            response.EnsureSuccessStatusCode();
            BracketPrediction = await response.Content.ReadFromJsonAsync<PlayoffBracketPrediction>();
        }
    }
}
