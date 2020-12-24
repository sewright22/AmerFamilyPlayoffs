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

        public void Refresh()
        {
            if (this.BracketPrediction.WildCardRound.AFCGames.Any(game => game.Winner == null)==false)
            {
                var winners = this.BracketPrediction.WildCardRound.AFCGames.Select(x => x.Winner).OrderBy(x => x.Seed);
                var lowestSeedWinner = winners.ToList()[0];
                var highestSeedWinner = winners.ToList()[1];

                var game1 = this.BracketPrediction.DivisionalRound.AFCGames.Select(x => x).FirstOrDefault(x => x.HomeTeam.Seed == 1);
                game1.AwayTeam = highestSeedWinner;

                var game2 = this.BracketPrediction.DivisionalRound.AFCGames.Select(x => x).FirstOrDefault(x => x.HomeTeam.Seed == 2);
                game2.AwayTeam = lowestSeedWinner;
            }

            if (this.BracketPrediction.WildCardRound.NFCGames.Any(game => game.Winner == null) == false)
            {
                var winners = this.BracketPrediction.WildCardRound.NFCGames.Select(x => x.Winner).OrderBy(x => x.Seed);
                var lowestSeedWinner = winners.ToList()[0];
                var highestSeedWinner = winners.ToList()[1];

                var game1 = this.BracketPrediction.DivisionalRound.NFCGames.Select(x => x).FirstOrDefault(x => x.HomeTeam.Seed == 1);
                game1.AwayTeam = highestSeedWinner;

                var game2 = this.BracketPrediction.DivisionalRound.NFCGames.Select(x => x).FirstOrDefault(x => x.HomeTeam.Seed == 2);
                game2.AwayTeam = lowestSeedWinner;
            }

            this.StateHasChanged();
        }
    }
}
