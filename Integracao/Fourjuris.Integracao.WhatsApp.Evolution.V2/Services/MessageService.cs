using Fourjuris.Integracao.WhatsApp.Evolution.V2.Model;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text;
using static System.Net.WebRequestMethods;

namespace Fourjuris.Integracao.WhatsApp.Evolution.V2.Services
{
    public class MessageService
    {
        private readonly HttpClient _client;
        private readonly string baseUrl;
        private readonly string apiKeyGlobal;
        public MessageService(IHttpClientFactory factory, IConfiguration configuration)
        {
            _client = factory.CreateClient("WhatsAppEvolution");
            //baseUrl = configuration["WhatsApp:EvolutionApiUrl"];
            baseUrl = "http://168.231.88.109:8080/";
            apiKeyGlobal = configuration["WhatsApp:EvolutionApiKey"];
        }

        public async Task<SendTextResponse> SendTextAsync(
         string instance,
         string number,
         string text,
         int delay = 0,
         bool linkPreview = false,
         bool mentionsEveryone = false,
         IEnumerable<string> mentioned = null
     )
        {
            // Monta o payload conforme a spec da Evolution API
            var payload = new
            {
                number,
                text,
                delay,
                linkPreview,
                mentionsEveryOne = mentionsEveryone, // notar a grafia exata
                mentioned = new[] { number }
            };
            
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            instance = "Gisely Araujo";
            // Usa URI relativa: "message/sendText/{instance}"
            var endpoint = $"{baseUrl}message/sendText/{instance}";

            _client.DefaultRequestHeaders.Add("apikey", "44DBDFF1AC06-477C-8501-0D7642D9D24A");

            HttpResponseMessage response = await _client.PostAsync(endpoint, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"Erro ao enviar texto (Status {response.StatusCode}): {errorBody}"
                );
            }
            return new SendTextResponse { };
        }
    }

    // Exemplo da classe de resposta
    public class SendTextResponse
    {
        public KeyInfo Key { get; set; } = default!;
        public MessageInfo Message { get; set; } = default!;
        public string MessageTimestamp { get; set; } = default!;
        public string Status { get; set; } = default!;
    }

    public class KeyInfo
    {
        public string RemoteJid { get; set; } = default!;
        public bool FromMe { get; set; }
        public string Id { get; set; } = default!;
    }

    public class MessageInfo
    {
        public ExtendedText ExtendedTextMessage { get; set; } = default!;
    }

    public class ExtendedText
    {
        public string Text { get; set; } = default!;
    }
}

