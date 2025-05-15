using Fourjuris.Integracao.Configurations;
using Fourjuris.Integracao.Helpers;
using Fourjuris.Integracao.WhatsApp.Abstractions;
using Fourjuris.Integracao.WhatsApp.Events;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WhatsAppConfiguracao = Fourjuris.Integracao.Configurations.WhatsAppConfiguracao;

namespace Fourjuris.Integracao.WhatsApp.Evolution.V2
{
    /// <summary>
    /// Cliente para integração com a API Evolution do WhatsApp
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>]]>
    /// </summary>
    public class EvolutionWhatsAppClient : IWhatsAppClient
    {
        private readonly HttpClient _httpClient;
        private readonly WhatsAppConfiguracao _configuracao;
        private readonly IEventBus _eventBus;
        private readonly string _numeroTelefone;
        private readonly string _empresaId; // Novo

        public EvolutionWhatsAppClient(
            IHttpClientFactory httpClientFactory,
            WhatsAppConfiguracao configuracao,
            IEventBus eventBus,
            string numeroTelefone,
            string empresaId) // Novo parâmetro
        {
            _httpClient = httpClientFactory.CreateClient("WhatsAppEvolution");
            _configuracao = configuracao;
            _eventBus = eventBus;
            _numeroTelefone = numeroTelefone;
            _empresaId = empresaId; // Atribuir

            _httpClient.BaseAddress = new Uri(_configuracao.EvolutionApiUrl);
            _httpClient.DefaultRequestHeaders.Add("apikey", _configuracao.EvolutionApiKey);
        }

        public async Task<bool> EnviarMensagemTextoAsync(string numero, string mensagem)
        {
            var request = new
            {
                sessionId = _configuracao.EvolutionSessionId,
                to = FormatarNumeroTelefone(numero),
                message = mensagem
            };

            var response = await _httpClient.PostAsJsonAsync("/api/v1/send-message", request);

            if (response.IsSuccessStatusCode)
            {
                await _eventBus.PublicarAsync(new MensagemEnviadaEvent
                {
                    EmpresaId = _empresaId, // Usar o campo _empresaId
                    Plataforma = "WhatsApp",
                    Destinatario = numero,
                    Conteudo = mensagem,
                    DataEnvio = DateTime.UtcNow
                });

                return true;
            }

            return false;
        }

        public async Task<bool> EnviarMensagemMidiaAsync(string numero, string url, string caption, TipoMidia tipoMidia)
        {
            var tipoEvolution = tipoMidia switch
            {
                TipoMidia.Imagem => "image",
                TipoMidia.Video => "video",
                TipoMidia.Audio => "audio",
                TipoMidia.Documento => "document",
                _ => throw new ArgumentException("Tipo de mídia inválido")
            };

            var request = new
            {
                sessionId = _configuracao.EvolutionSessionId,
                to = FormatarNumeroTelefone(numero),
                url = url,
                caption = caption,
                type = tipoEvolution
            };

            var response = await _httpClient.PostAsJsonAsync("/api/v1/send-media", request);

            if (response.IsSuccessStatusCode)
            {
                await _eventBus.PublicarAsync(new MensagemEnviadaEvent
                {
                    EmpresaId = _empresaId, // Usar o campo _empresaId
                    Plataforma = "WhatsApp",
                    Destinatario = numero,
                    Conteudo = $"{tipoMidia}: {url}",
                    DataEnvio = DateTime.UtcNow
                });

                return true;
            }

            return false;
        }

        public async Task<bool> IniciarSessaoAsync()
        {
            // A Evolution mantém sessões ativas, este método verificaria se a sessão está ativa
            // ou iniciaria uma nova sessão se necessário
            var request = new
            {
                apiKey = _configuracao.EvolutionApiKey
            };

            var response = await _httpClient.PostAsJsonAsync("/api/v1/start-session", request);

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<EvolutionSessionResponse>();
                return resultado?.Success == true;
            }

            return false;
        }

        private string FormatarNumeroTelefone(string numero)
        {
            // Remove caracteres não numéricos
            var apenasNumeros = new string(numero.Where(c => char.IsDigit(c)).ToArray());

            // Garante que o número está no formato esperado para a Evolution
            if (apenasNumeros.Length == 11 && apenasNumeros.StartsWith("55"))
                return apenasNumeros;

            if (apenasNumeros.Length == 9 || apenasNumeros.Length == 8)
            {
                // Buscar o número correspondente na lista e usar o DDD
                var numeroConfig = _configuracao.NumeroTelefones.First(n => n.NumeroTelefone == _numeroTelefone);
                var ddd = numeroConfig.NumeroTelefone.Length >= 2 ? numeroConfig.NumeroTelefone.Substring(0, 2) : "11"; // Fallback para "11" se não houver DDD
                return "55" + ddd + apenasNumeros;
            }

            return apenasNumeros;
        }
    }

    public class EvolutionSessionResponse
    {
        public bool Success { get; set; }
        public string SessionId { get; set; }
    }
}
