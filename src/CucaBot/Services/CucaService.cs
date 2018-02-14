using CucaBot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace CucaBot.Services
{
    [Serializable]
    public class CucaService
    {
        private readonly string _cucaApi;

        private readonly HttpService _httpService;

        public CucaService()
        {
            _cucaApi = ConfigurationManager.AppSettings["CucaApi"];
            _httpService = new HttpService();
        }

        public async Task<IEnumerable<CucaModel>> ListNext()
        {
            var url = $"{_cucaApi}/api/cucas/next?limit=5";
            return await _httpService.Send<IEnumerable<CucaModel>>(HttpMethod.Get, url);
        }

        public async Task<CucaModel> Create(CucaModel cuca)
        {
            var url = $"{_cucaApi}/api/cucas";
            return await _httpService.Send<CucaModel>(HttpMethod.Post, url, cuca);
        }

        public async Task<HttpResponseMessage> Join(int idCuca, UserModel userModel)
        {
            var url = $"{_cucaApi}/api/cucas/{idCuca}/join";
            return await _httpService.Send(HttpMethod.Put, url, userModel);
        }
    }
}