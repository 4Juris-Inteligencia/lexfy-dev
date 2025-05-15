using Fourjuris.Integracao.Configurations;
using Fourjuris.Integracao.Helpers;
using Fourjuris.Integracao.WhatsApp.Events;
using Fourjuris.Integracao.WhatsApp.Evolution.V2.Model;
using Fourjuris.Integracao.WhatsApp.Evolution.V2.Payloads;
using Fourjuris.Integracao.WhatsApp.Evolution.V2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Fourjuris.Integracao.Api.Endpoints
{
    public static class WebhookEndpoints
    {
        public static void MapWebhookEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/webhook/whatsapp/messages-upsert", ReceberMensagemWebhook)
                .WithName("ReceberMensagemWebhook")
                .WithDescription("Recebe mensagens do WhatsApp via Evolution API webhook");
        }

        private static async Task<IResult> ReceberMensagemWebhook(
        [FromBody] MessageUpsertEvent mensagem,
        IEventBus eventBus,
        ITenantConfigurationService configService,
        ChatService chatServiceEvolution,
        ILogger<Program> logger)
        {
            string numeroDestinatario = mensagem.Sender.Split('@')[0]; //o destinatário é o usuário do sistema.
            string numeroRemetente = mensagem.Data.Key.RemoteJid.Split('@')[0]; //o remetente é o número do cliente.
            FetchProfilePictureResponse profilePictureResponse = await chatServiceEvolution.FetchProfilePictureAsync(mensagem.Instance, numeroRemetente, mensagem.ApiKey);

            string? urlFotoPerfil = profilePictureResponse?.ProfilePictureUrl;

            if (mensagem.Data.Key.FromMe)
            {
                logger.LogInformation("Mensagem recebida de um número do sistema, ignorando: {Numero}", numeroDestinatario);
                return Results.BadRequest("O usuario nao pode receber mensagem de si mesmo.");
            }

            if (mensagem.Data.Key.RemoteJid.EndsWith("@g.us"))
            {
                logger.LogInformation("Mensagem recebida de um grupo, ignorando: {Numero}", numeroDestinatario);
                return Results.BadRequest("O usuario nao pode receber mensagem de um grupo.");
            }

            TenantApiConfiguration config = await configService.ObterConfiguracaoPorNumeroTelefoneAsync(numeroDestinatario);
            if (config == null)
            {
                logger.LogWarning("Empresa não encontrada para o número: {Numero}", numeroDestinatario);
                return Results.BadRequest("Empresa não encontrada para o número de telefone");
            }

            await eventBus.PublicarAsync(
                new MensagemRecebidaEvent
                {
                    EmpresaId = config.EmpresaId,
                    Plataforma = "WhatsApp",
                    Remetente = numeroRemetente,
                    Destinatario = numeroDestinatario,
                    Conteudo = mensagem.Data.Message.Conversation,
                    ProfilePictureUrl = urlFotoPerfil ?? string.Empty,
                    DataRecebimento = DateTime.UtcNow
                });

            return Results.Ok();
        }
    }
}