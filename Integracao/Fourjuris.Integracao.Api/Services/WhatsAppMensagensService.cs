using Fourjuris.Integracao.WhatsApp;
using Fourjuris.Integracao.WhatsApp.Abstractions;
using Fourjuris.Integracao.WhatsApp.Models;
using System.Threading.Tasks;
using TipoMidia = Fourjuris.Integracao.WhatsApp.Abstractions.TipoMidia;

namespace Fourjuris.Integracao.Api.Services
{
    /// <summary>
    /// Interface para envio de mensagens via WhatsApp
    /// </summary>
    public interface IWhatsAppMensagensService
    {
        Task<bool> EnviarMensagemTextoAsync(string empresaId, string numero, string mensagem);
        Task<bool> EnviarMensagemMidiaAsync(string empresaId, string numero, string url, string caption, TipoMidia tipoMidia);
    }

    /// <summary>
    /// Implementação do serviço de envio de mensagens via WhatsApp
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class WhatsAppMensagensService : IWhatsAppMensagensService
    {
        private readonly IWhatsAppClientFactory _clientFactory;

        public WhatsAppMensagensService(IWhatsAppClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool> EnviarMensagemTextoAsync(string empresaId, string numero, string mensagem)
        {
            var cliente = await _clientFactory.CriarClienteAsync(empresaId, numero);
            return await cliente.EnviarMensagemTextoAsync(numero, mensagem);
        }

        public async Task<bool> EnviarMensagemMidiaAsync(string empresaId, string numero, string url, string caption, TipoMidia tipoMidia)
        {
            var cliente = await _clientFactory.CriarClienteAsync(empresaId, numero);
            return await cliente.EnviarMensagemMidiaAsync(numero, url, caption, tipoMidia);
        }
    }
}