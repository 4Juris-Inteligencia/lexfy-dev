using Microsoft.EntityFrameworkCore;

namespace Fourjuris.Integracao.Configurations
{
    /// <summary>
    /// Interface de repositório de configuração de integrações por empresa
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public interface ITenantConfigurationRepository
    {
        Task<TenantApiConfiguration> ObterPorEmpresaIdAsync(string empresaId);
        Task SalvarAsync(TenantApiConfiguration configuracao);
        Task<IEnumerable<TenantApiConfiguration>> ObterTodasConfiguracoesAsync();

    }

    /// <summary>
    /// Repositório de configuração de integrações por empresa
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class TenantConfigurationRepository : ITenantConfigurationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TenantConfigurationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TenantApiConfiguration> ObterPorEmpresaIdAsync(string empresaId)
        {
            return await _dbContext.TenantApiConfigurations
                .FirstOrDefaultAsync(t => t.EmpresaId == empresaId);
        }

        public async Task SalvarAsync(TenantApiConfiguration configuracao)
        {
            var existingConfig = await _dbContext.TenantApiConfigurations
                .FirstOrDefaultAsync(t => t.EmpresaId == configuracao.EmpresaId);

            if (existingConfig == null)
            {
                _dbContext.TenantApiConfigurations.Add(configuracao);
            }
            else
            {
                _dbContext.Entry(existingConfig).CurrentValues.SetValues(configuracao);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TenantApiConfiguration>> ObterTodasConfiguracoesAsync()
        {
            return await _dbContext.TenantApiConfigurations.ToListAsync();
        }
    }
}