
using FourJuris.Integracao.Api.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Fourjuris.Integracao.Api.Endpoints
{
    /// <summary>
    /// Classe de extensões para mapeamento de endpoints
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public static class EndpointExtensions
    {
        public static void MapAllEndpoints(this WebApplication app)
        {
            app.MapConfiguracaoEndpoints();
            app.MapWebhookEndpoints();
            app.MapWhatsAppEndpoints();

            // No futuro, aqui serão adicionados outros endpoints para:
            // - Instagram
            // - Facebook
            // - Telegram
            // - Webhook
        }
    }
}
