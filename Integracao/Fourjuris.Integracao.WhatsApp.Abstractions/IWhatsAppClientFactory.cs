namespace Fourjuris.Integracao.WhatsApp.Abstractions
{
    /// <summary>
    /// Interface para fábrica de clientes de WhatsApp
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public interface IWhatsAppClientFactory
    {
        Task<IWhatsAppClient> CriarClienteAsync(string empresaId, string numero);
    }
}