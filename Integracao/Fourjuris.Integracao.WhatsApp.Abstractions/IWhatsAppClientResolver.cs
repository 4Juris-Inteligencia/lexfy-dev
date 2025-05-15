using Fourjuris.Integracao.Configurations;
namespace Fourjuris.Integracao.WhatsApp.Abstractions
{
    /// <summary>
    /// Interface para fábrica de clientes de WhatsApp
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public interface IWhatsAppClientResolver
    {
        IWhatsAppClient ResolveClient(TipoIntegracao tipoIntegracao, WhatsAppConfiguracao config, string empresaId, string numeroTelefone);
    }
}