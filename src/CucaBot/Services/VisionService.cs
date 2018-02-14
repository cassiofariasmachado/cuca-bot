using CucaBot.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace CucaBot.Services
{
    [Serializable]
    public class VisionService
    {
        private readonly HttpService _httpService;

        private readonly string _customVisionApiKey;

        private readonly string _customVisionUri;

        public VisionService()
        {
            _httpService = new HttpService();
            _customVisionApiKey = ConfigurationManager.AppSettings["CustomVisionApiKey"];
            _customVisionUri = ConfigurationManager.AppSettings["CustomVisionUri"];
        }

        public async Task<CustomVisionModel> Analyse(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _customVisionUri);

            request.Headers.Add("Prediction-key", _customVisionApiKey);

            return await _httpService.Send<CustomVisionModel>(request, new { Url = url });
        }
    }
}