using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace Fourjuris.Integracao.Configurations
{
    /// <summary>
    /// Interface de serviço de configuração de integrações por empresa
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public interface ITenantConfigurationService
    {
        Task<TenantApiConfiguration> ObterConfiguracaoPorEmpresaIdAsync(string empresaId);
        Task SalvarConfiguracaoAsync(TenantApiConfiguration configuracao);
        Task<TenantApiConfiguration> ObterConfiguracaoPorNumeroTelefoneAsync(string numeroTelefone); // Novo
    }

    /// <summary>
    /// Implementação do serviço de configuração de integrações por empresa
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class TenantConfigurationService : ITenantConfigurationService
    {
        private readonly IMemoryCache _cache;
        private readonly ITenantConfigurationRepository _repository;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public TenantConfigurationService(
            IMemoryCache cache,
            ITenantConfigurationRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        public async Task<TenantApiConfiguration> ObterConfiguracaoPorEmpresaIdAsync(string empresaId)
        {
            string cacheKey = $"tenant_config_{empresaId}";

            if (!_cache.TryGetValue(cacheKey, out TenantApiConfiguration config))
            {
                config = await _repository.ObterPorEmpresaIdAsync(empresaId);

                if (config != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(_cacheDuration);

                    _cache.Set(cacheKey, config, cacheOptions);
                }
            }

            return config ?? new TenantApiConfiguration { EmpresaId = empresaId };
        }

        public async Task SalvarConfiguracaoAsync(TenantApiConfiguration configuracao)
        {
            await _repository.SalvarAsync(configuracao);

            // Atualiza o cache
            string cacheKey = $"tenant_config_{configuracao.EmpresaId}";
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_cacheDuration);

            _cache.Set(cacheKey, configuracao, cacheOptions);
        }
        public async Task<TenantApiConfiguration> ObterConfiguracaoPorNumeroTelefoneAsync(string numeroTelefone)
        {
            var todasConfiguracoes = await _repository.ObterTodasConfiguracoesAsync();
            var config = todasConfiguracoes.FirstOrDefault(c => c.WhatsApp.NumeroTelefones.Any(n => n.NumeroTelefone == numeroTelefone));

            if (config != null)
            {
                string cacheKey = $"tenant_config_numero_{numeroTelefone}";
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheDuration);
                _cache.Set(cacheKey, config, cacheOptions);
            }

            return config;
        }
    }
}
