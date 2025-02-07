﻿using BlazorBitcoinApp.DTOs;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace BlazorBitcoinApp.Services
{
	public class BitcoinService : IBitcoinService
	{
		private readonly HttpClient _httpClient;

        public BitcoinService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<BitcoinDataDTO>> FindBy(DateTime startDate)
        {
            var reponse = await _httpClient.GetAsync("https://data.messari.io/api/v1/markets/binance-btc-usdt/metrics/price/time-series?start=" + startDate.ToString("yyyy-MM-dd") + "&interval=1d");          
            reponse.EnsureSuccessStatusCode();


			var jsonResult = await reponse.Content.ReadAsStringAsync();


			JObject JObject = JObject.Parse(jsonResult);
            var values = JObject.SelectToken("data.values").ToString(); 


            if(string.IsNullOrWhiteSpace(values))
                return new List<BitcoinDataDTO>();


			var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<decimal[]>>(values);


			return data.Select(d => new BitcoinDataDTO(new DateTime(1970, 1, 1).AddMilliseconds((long)d[0]), d[3])).ToList();

		}
    }  
}
