using Fourjuris.Integracao.WhatsApp.Evolution.V2.Model;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
namespace Fourjuris.Integracao.WhatsApp.Evolution.V2.Services
{
    public class ChatService
    {
        private readonly HttpClient _client;
        private readonly string baseUrl;
        private readonly string apiKeyGlobal;
        public ChatService(IHttpClientFactory factory, IConfiguration configuration)
        {
            _client = factory.CreateClient("WhatsAppEvolution");
            baseUrl = configuration["WhatsApp:EvolutionApiUrl"];
            apiKeyGlobal = configuration["WhatsApp:EvolutionApiKey"];
        }

        public async Task<FetchProfilePictureResponse> FetchProfilePictureAsync(string instance, string sender, string instanceApiKey)
        {
            string url = $"{baseUrl}/chat/fetchProfilePictureUrl/{instance}";
            var requestBodyObject = new { number = sender };

            string jsonString = JsonSerializer.Serialize(requestBodyObject);
            StringContent requestBody = new StringContent(jsonString, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("apikey", instanceApiKey);

            HttpResponseMessage response = await _client.PostAsync(url, requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erro ao buscar foto de perfil: {response.StatusCode} - {error}");
            }
            string result = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<FetchProfilePictureResponse>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseObject;
        }

    }
}
