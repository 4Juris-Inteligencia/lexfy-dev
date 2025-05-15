using Fourjuris.Integracao.Configurations;
using Fourjuris.Integracao.Helpers;
using Fourjuris.Integracao.WhatsApp.Events;
using FourJuris.Integracao.Api.Endpoints;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Fourjuris.Integracao.Api.Services
{
    /// <summary>
    /// Serviço de WebSocket para integração com a API Evolution.
    /// Classe EvolutionWebSocketService é um HostedService que faz a conexão com a API Evolution via WebSocket
    /// para receber mensagens do WhatsApp em tempo real e publica eventos de mensagens recebidas.
    /// Usa construtor primário para injeção de dependências e implementa reconexão automática em caso de falhas.
    /// </summary>
    /// <author>Marcelo Miranda</author>
    /// <param name="eventBus">Barramento de eventos para publicar mensagens recebidas.</param>
    /// <param name="configService">Serviço para obter configurações do tenant.</param>
    public class EvolutionWebSocketService_(IEventBus eventBus, ITenantConfigurationService configService) : IHostedService, IDisposable
    {
        private readonly IEventBus _eventBus = eventBus;
        private readonly ITenantConfigurationService _configService = configService;
        private ClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cts = new();
        private const int ReconnectDelaySeconds = 5;

        /// <summary>
        /// Inicia o serviço de WebSocket e tenta conectar à API Evolution.
        /// Inicia um loop de conexão que lida com reconexões automáticas em caso de falhas.
        /// </summary>
        /// <author>Marcelo Miranda</author>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ConnectAsync(cancellationToken);
        }

        /// <summary>
        /// Tenta conectar ao WebSocket da API Evolution e lida com reconexões.
        /// Cria um ClientWebSocket, obtém a configuração via ITenantConfigurationService,
        /// monta a URL do WebSocket e inicia o loop de recebimento de mensagens.
        /// Em caso de falha, tenta reconectar após um intervalo definido.
        /// </summary>
        /// <author>Marcelo Miranda</author>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        private async Task ConnectAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _webSocket = new ClientWebSocket();
                    var config = await _configService.ObterConfiguracaoPorEmpresaIdAsync("default-empresa-id");
                    var wsUrl = $"{config.WhatsApp.EvolutionApiUrl.Replace("https", "wss")}/socket?apiKey={config.WhatsApp.EvolutionApiKey}";

                    await _webSocket.ConnectAsync(new Uri(wsUrl), cancellationToken);
                    await ReceiveMessages(cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro no WebSocket: {ex.Message}. Tentando reconectar em {ReconnectDelaySeconds} segundos...");
                    await Task.Delay(ReconnectDelaySeconds * 1000, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Recebe mensagens da API Evolution via WebSocket e publica eventos de mensagens recebidas.
        /// Escuta continuamente o WebSocket, desserializa mensagens JSON e publica eventos MensagemRecebidaEvent.
        /// Sai do loop se o WebSocket for fechado ou a operação for cancelada.
        /// </summary>
        /// <author>Marcelo Miranda</author>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        private async Task ReceiveMessages(CancellationToken cancellationToken)
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var mensagemRecebida = JsonSerializer.Deserialize<MensagemRecebidaWebhook>(message);

                    await _eventBus.PublicarAsync(new MensagemRecebidaEvent
                    {
                        EmpresaId = mensagemRecebida.EmpresaId,
                        Plataforma = "WhatsApp",
                        Remetente = mensagemRecebida.Remetente,
                        Conteudo = mensagemRecebida.Conteudo,
                        MidiaUrl = mensagemRecebida.MidiaUrl,
                        TipoMidia = mensagemRecebida.TipoMidia,
                        ProfilePictureUrl = mensagemRecebida.ProfilePictureUrl,
                        DataRecebimento = DateTime.UtcNow
                    });
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fechado no servidor", cancellationToken);
                    break;
                }
            }
        }

        /// <summary>
        /// Para o serviço de WebSocket.
        /// Cancela o loop de recebimento e fecha a conexão WebSocket.
        /// </summary>
        /// <author>Marcelo Miranda</author>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Task representando a operação assíncrona.</returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();
            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Fechamento", cancellationToken);
            }
        }

        /// <summary>
        /// Libera os recursos do serviço de WebSocket.
        /// Desaloca o CancellationTokenSource e o ClientWebSocket.
        /// </summary>
        /// <author>Marcelo Miranda</author>
        public void Dispose()
        {
            _cts.Dispose();
            _webSocket?.Dispose();
        }
    }
}