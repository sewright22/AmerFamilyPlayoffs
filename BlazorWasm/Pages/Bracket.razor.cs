namespace AmerFamilyPlayoffs.Pages
{
    using AmerFamilyPlayoffs.Data;
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

        [Inject]
        NavigationManager NavigationManager { get; set; }

        HttpClient HttpClient { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var response = await this.HttpClient.GetAsync($"BracketPrediction/{Id}");
            response.EnsureSuccessStatusCode();
            var bracket = await response.Content.ReadFromJsonAsync<BracketPrediction>();
            var test = bracket;
        }

        //protected override async Task OnParametersSetAsync()
        //{
        //    var response = await this.HttpClient.GetAsync($"BracketPrediction/{Id}");
        //    response.EnsureSuccessStatusCode();
        //    var bracket = await response.Content.ReadFromJsonAsync<BracketPrediction>();
        //    var test = bracket;
        //}
    }
}
