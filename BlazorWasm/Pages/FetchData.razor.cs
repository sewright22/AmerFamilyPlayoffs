namespace AmerFamilyPlayoffs.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Net.Http.Json;
    using Microsoft.AspNetCore.Components;
    using System.Diagnostics;

    public partial class FetchData : ComponentBase
    {
        [Inject]
        HttpClient HttpClient { get; set; }
        WeatherForecast[] forecasts;
        private string errorMessage;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var response = await HttpClient.GetAsync("https://localhost:44325/WeatherForecast/");
                forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
            }
        }

        public class WeatherForecast
        {
            public DateTime Date { get; set; }

            public int TemperatureC { get; set; }

            public int TemperatureF { get; set; }

            public string Summary { get; set; }
        }
    }
}
