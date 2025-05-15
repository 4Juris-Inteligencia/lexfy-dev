using Fourjuris.Integracao.Api.Services;
using Fourjuris.Integracao.WhatsApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FourJuris.Integracao.Api.Endpoints
{
    /// <summary>
    /// Classe de endpoints para mensagens.
    /// Gerencia endpoints para obter e adicionar mensagens de usuários e do ChatBot.
    /// O endpoint /whatsapp/send-text permite enviar mensagens de texto via WhatsApp.
    /// <author>Marcelo Miranda</author>
    /// </summary>
    public static class WhatsAppEndpoints
    {
        public static void MapWhatsAppEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/whatsapp/send-text", EnviarMensagemTexto)
                .WithName("EnviarMensagemTexto")
                .WithDescription("Envia uma mensagem de texto via WhatsApp");
        }

        private static async Task<IResult> EnviarMensagemTexto(
            [FromBody] EnviarMensagemRequest request,
            IWhatsAppMensagensService service)
        {
            var sucesso = await service.EnviarMensagemTextoAsync(request.EmpresaId, request.Numero, request.Mensagem);
            return sucesso ? Results.Ok() : Results.BadRequest();
        }
    }

    /// <summary>
    /// Request para envio de mensagem de texto
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class EnviarMensagemRequest
    {
        public string EmpresaId { get; set; }
        public string Numero { get; set; }
        public string Mensagem { get; set; }
    }

    /// <summary>
    /// Request para envio de mensagem com mídia
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class EnviarMidiaMensagemRequest
    {
        public string EmpresaId { get; set; }
        public string Numero { get; set; }
        public string Url { get; set; }
        public string Legenda { get; set; }
        public TipoMidia TipoMidia { get; set; }
    }

    /// <summary>
    /// Mensagem recebida via Webhook
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>

    public class MensagemRecebidaWebhook
    {
        public string EmpresaId { get; set; }
        public string Destinatario { get; set; }
        public string Remetente { get; set; }
        public string Conteudo { get; set; }
        public string? MidiaUrl { get; set; }
        public string? TipoMidia { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}