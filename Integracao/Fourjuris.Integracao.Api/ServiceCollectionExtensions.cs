using Fourjuris.Integracao.Configurations;
using Fourjuris.Integracao.WhatsApp;
using Fourjuris.Integracao.WhatsApp.Abstractions;
using Fourjuris.Integracao.WhatsApp.Evolution.V2;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWhatsAppIntegracao(this IServiceCollection services)
    {
        // Registra as abstrações
        services.AddScoped<IWhatsAppClientFactory, WhatsAppClientFactory>();
        services.AddSingleton<IWhatsAppClientResolver, WhatsAppClientResolver>();

        // Registra os clientes HTTP necessários
        services.AddHttpClient("WhatsAppEvolution");

        // Registra as implementações concretas
        services.AddTransient<EvolutionWhatsAppClient>();

        // Configura o resolver
        services.AddSingleton(provider =>
        {
            var resolver = provider.GetRequiredService<IWhatsAppClientResolver>();

            if (resolver is WhatsAppClientResolver whatsAppResolver)
            {
                whatsAppResolver.RegisterClientType(TipoIntegracao.Evolution, typeof(EvolutionWhatsAppClient));
                // Adicione mais implementações aqui quando disponíveis
            }

            return resolver;
        });

        return services;
    }
}