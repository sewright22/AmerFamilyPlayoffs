namespace AmerFamilyPlayoffs.Pages
{
    using AmerFamilyPlayoffs.Models;
    using Microsoft.AspNetCore.Components;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Text;
    using System.Text.Json;
    using System.Net.Http.Json;

    public partial class CreateBracket : ComponentBase
    {
        [Inject]
        HttpClient HttpClient { get; set; }

        private PlayoffBracketPrediction bracket = new PlayoffBracketPrediction();

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        private async Task HandleSubmit()
        {
            var payload = new StringContent(JsonSerializer.Serialize(bracket), Encoding.UTF8, "application/json");

            var response = await this.HttpClient.PostAsync("BracketPrediction", payload);

            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync< PlayoffBracketPrediction>();
            var test = created;
        }
    }
}
