using Fourjuris.Integracao.Configurations;
using Fourjuris.Integracao.WhatsApp.Abstractions;

namespace Fourjuris.Integracao.WhatsApp
{
    /// <summary>
    /// Fábrica de clientes de WhatsApp que tem função de criar um cliente de acordo com a configuração da empresa
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class WhatsAppClientFactory : IWhatsAppClientFactory
    {
        private readonly ITenantConfigurationService _configService;
        private readonly IWhatsAppClientResolver _clientResolver;

        public WhatsAppClientFactory(
            ITenantConfigurationService configService,
            IWhatsAppClientResolver clientResolver)
        {
            _configService = configService;
            _clientResolver = clientResolver;
        }

        public async Task<IWhatsAppClient> CriarClienteAsync(string empresaId, string numeroTelefone)
        {
            var config = await _configService.ObterConfiguracaoPorEmpresaIdAsync(empresaId);

            if (config?.WhatsApp == null || !config.WhatsApp.NumeroTelefones.Any(n => n.NumeroTelefone == numeroTelefone))
                throw new InvalidOperationException($"Configuração de WhatsApp não encontrada para a empresa {empresaId} e número {numeroTelefone}");

            return _clientResolver.ResolveClient(
                config.WhatsApp.TipoIntegracao,
                config.WhatsApp,
                empresaId, // Passar empresaId
                numeroTelefone);
        }
    }
}