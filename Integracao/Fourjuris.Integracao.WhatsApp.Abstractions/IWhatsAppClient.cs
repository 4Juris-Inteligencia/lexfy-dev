using Fourjuris.Integracao.Configurations; // Use os tipos existentes

namespace Fourjuris.Integracao.WhatsApp.Abstractions
{
    /// <summary>
    /// Interface para fábrica de clientes de WhatsApp
    /// Essa interface faz a criação de um cliente de acordo com a configuração da empresa
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public interface IWhatsAppClient
    {
        Task<bool> EnviarMensagemTextoAsync(string numero, string mensagem);
        Task<bool> EnviarMensagemMidiaAsync(string numero, string url, string caption, TipoMidia tipoMidia);
        Task<bool> IniciarSessaoAsync();
    }

    /// <summary>
    /// Enumeração de tipos de mídia suportados
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public enum TipoMidia
    {
        Imagem,
        Video,
        Audio,
        Documento
    }    
}
