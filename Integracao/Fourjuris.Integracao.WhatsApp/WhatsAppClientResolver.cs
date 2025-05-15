using Fourjuris.Integracao.Configurations;
using Fourjuris.Integracao.Helpers;
using Fourjuris.Integracao.WhatsApp.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;


namespace Fourjuris.Integracao.WhatsApp
{
    /// <summary>
    /// Resolvedor de clientes de WhatsApp que tem função de criar um cliente de acordo com o tipo de integração
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class WhatsAppClientResolver : IWhatsAppClientResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<TipoIntegracao, Type> _clientTypes;

        public WhatsAppClientResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            // O dicionário mapeia os tipos de integração para os tipos concretos
            _clientTypes = new Dictionary<TipoIntegracao, Type>();
        }

        public void RegisterClientType(TipoIntegracao tipoIntegracao, Type clientType)
        {
            if (!typeof(IWhatsAppClient).IsAssignableFrom(clientType))
            {
                throw new ArgumentException($"O tipo {clientType.Name} não implementa IWhatsAppClient");
            }

            _clientTypes[tipoIntegracao] = clientType;
        }

        public IWhatsAppClient ResolveClient(TipoIntegracao tipoIntegracao, WhatsAppConfiguracao config, string empresaId, string numeroTelefone)
        {
            if (!_clientTypes.TryGetValue(tipoIntegracao, out var clientType))
                throw new NotSupportedException($"Tipo de integração não suportado: {tipoIntegracao}");
            var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
            return (IWhatsAppClient)ActivatorUtilities.CreateInstance(
                _serviceProvider,
            clientType,
                new object[] { config, eventBus, numeroTelefone, empresaId }); // Passar empresaId
        }
    }
}
