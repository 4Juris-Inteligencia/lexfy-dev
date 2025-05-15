using Fourjuris.Integracao.Configurations;

namespace Fourjuris.Integracao.WhatsApp.Abstractions
{
    /// <summary>
    /// Classe WhatsAppConfiguracao para configuração de integração com WhatsApp
    /// /// Descontinuado: Utilize a classe WhatsAppConfiguracao de Fourjuris.Integracao.Configurations
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class WhatsAppConfiguracao_
    {
        public string EmpresaId { get; set; }
        public TipoIntegracao TipoIntegracao { get; set; }
        public string NumeroTelefone { get; set; }

        // Configurações comuns a todas implementações

        // Configurações específicas da Evolution
        public string EvolutionApiUrl { get; set; }
        public string EvolutionApiKey { get; set; }
        public string EvolutionSessionId { get; set; }

        // Configurações específicas do Meta
        public string MetaToken { get; set; }
        public string MetaPhoneNumberId { get; set; }
    }
}