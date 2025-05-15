using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Fourjuris.Integracao.Configurations
{
    /// <summary>
    /// Classe de banco de dados para configuração de integrações
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<TenantApiConfiguration> TenantApiConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TenantApiConfiguration>(entity =>
            {
                entity.HasKey(e => e.EmpresaId);
                entity.Property(e => e.Nome).HasMaxLength(100);



                // Configurando a serialização das propriedades complexas como JSON
                entity.Property(e => e.WhatsApp).HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                    v => System.Text.Json.JsonSerializer.Deserialize<WhatsAppConfiguracao>(v, new System.Text.Json.JsonSerializerOptions()));

                entity.Property(e => e.Instagram).HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                    v => System.Text.Json.JsonSerializer.Deserialize<InstagramConfiguracao>(v, new System.Text.Json.JsonSerializerOptions()));

                entity.Property(e => e.Facebook).HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                    v => System.Text.Json.JsonSerializer.Deserialize<FacebookConfiguracao>(v, new System.Text.Json.JsonSerializerOptions()));

                entity.Property(e => e.Telegram).HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                    v => System.Text.Json.JsonSerializer.Deserialize<TelegramConfiguracao>(v, new System.Text.Json.JsonSerializerOptions()));

            });
        }
    }
}
