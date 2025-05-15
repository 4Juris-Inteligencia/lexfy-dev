using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Fourjuris.Integracao.Configurations;

namespace Fourjuris.Integracao.Api.Endpoints
{
    /// <summary>
    /// Classe de endpoints para configuração de integrações
    /// Ao salvar uma configuração, registrar o webhook na Evolution API automaticamente.
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public static class ConfiguracaoEndpoints
    {
        public static void MapConfiguracaoEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/configuracao")
                .WithTags("Configuração")
                .WithOpenApi();

            group.MapGet("/{empresaId}", ObterConfiguracao)
                .WithName("ObterConfiguracao")
                .WithDescription("Obtém a configuração de integrações para uma empresa específica");

            group.MapPost("/", SalvarConfiguracao)
                .WithName("SalvarConfiguracao")
                .WithDescription("Salva a configuração de integrações para uma empresa");
        }

        private static async Task<IResult> ObterConfiguracao(
            string empresaId,
            ITenantConfigurationService configurationService)
        {
            var config = await configurationService.ObterConfiguracaoPorEmpresaIdAsync(empresaId);

            if (config == null)
                return Results.NotFound();

            return Results.Ok(config);
        }

        private static async Task<IResult> SalvarConfiguracao(
        TenantApiConfiguration configuracao,
        ITenantConfigurationService configurationService,
        IHttpClientFactory httpClientFactory)
        {
            await configurationService.SalvarConfiguracaoAsync(configuracao);

            var client = httpClientFactory.CreateClient("WhatsAppEvolution");
            foreach (var numero in configuracao.WhatsApp.NumeroTelefones)
            {
                var request = new { webhookUrl = numero.WebhookUrl, numeroTelefone = numero.NumeroTelefone };
                await client.PostAsJsonAsync("/api/v1/set-webhook", request);
            }

            return Results.Ok();
        }
    }
}