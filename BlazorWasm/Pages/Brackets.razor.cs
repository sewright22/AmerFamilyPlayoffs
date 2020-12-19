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

    public partial class Brackets : ComponentBase
    {
        [Inject]
        HttpClient HttpClient { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        PlayoffBracketPrediction[] BracketList;
        private string errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var response = await HttpClient.GetAsync("BracketPrediction");
                BracketList = await response.Content.ReadFromJsonAsync<PlayoffBracketPrediction[]>();
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }
        }

        private void OpenBracket(int bracketId)
        {

        }

        private void CreateNewBracket()
        {
            this.NavigationManager.NavigateTo("brackets/create");
        }
    }
}
