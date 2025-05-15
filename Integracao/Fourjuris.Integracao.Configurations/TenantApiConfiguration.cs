namespace Fourjuris.Integracao.Configurations
{
    /// <summary>
    /// Configurações de API para cada tenant (empresa)
    /// <author>Marcelo Miranda</author>
    /// </summary>
    public class TenantApiConfiguration
    {
        public string EmpresaId { get; set; } 
        public string Nome { get; set; }
        public WhatsAppConfiguracao WhatsApp { get; set; } = new WhatsAppConfiguracao();
        public InstagramConfiguracao Instagram { get; set; } = new InstagramConfiguracao();
        public FacebookConfiguracao Facebook { get; set; } = new FacebookConfiguracao();
        public TelegramConfiguracao Telegram { get; set; } = new TelegramConfiguracao();
    }

    public class WhatsAppConfiguracao
    {
        public TipoIntegracao TipoIntegracao { get; set; } = TipoIntegracao.Evolution;
        public string EmpresaId { get; set; }
        public string ApiKey { get; set; }
        public string ApiUrl { get; set; }
        public string WebhookUrl { get; set; }
        public string WebhookToken { get; set; }
        //public string NumeroTelefone { get; set; }
        public List<WhatsAppNumeroConfig> NumeroTelefones { get; set; } = new List<WhatsAppNumeroConfig>();

        // Específico para Evolution
        public string EvolutionApiKey { get; set; }
        public string EvolutionApiUrl { get; set; } = "https://api.evolution.com.br";
        public string EvolutionSessionId { get; set; }

        // Específico para API oficial Meta
        public string MetaToken { get; set; }
        public string MetaPhoneNumberId { get; set; }
        public string MetaBusinessAccountId { get; set; }
    }

    public class WhatsAppNumeroConfig
    {
        public string NumeroTelefone { get; set; }
        public string WebhookUrl { get; set; }
        public string WebhookToken { get; set; }
    }

    public class InstagramConfiguracao
    {
        public bool Ativo { get; set; }
        public string ApiKey { get; set; }
        public string WebhookUrl { get; set; }
    }

    public class FacebookConfiguracao
    {
        public bool Ativo { get; set; }
        public string ApiKey { get; set; }
        public string PageId { get; set; }
    }

    public class TelegramConfiguracao
    {
        public bool Ativo { get; set; }
        public string BotToken { get; set; }
    }

    public enum TipoIntegracao
    {
        Evolution,
        MetaOficial,
        Outro
    }
}
