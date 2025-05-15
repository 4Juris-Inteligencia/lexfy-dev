namespace Fourjuris.Integracao.WhatsApp.Events
{
    /// <summary>
    /// Eventos de mensagens enviada
    /// <![CDATA[<author>Marcelo Miranda</author>]]>]]>
    /// </summary>
    public class MensagemEnviadaEvent
    {
        public string EmpresaId { get; set; }
        public string Instance { get; set; }
        public string Plataforma { get; set; }
        public string Destinatario { get; set; }
        public string Conteudo { get; set; }
        public DateTime DataEnvio { get; set; }
        public string UsuarioId { get; set; }
    }
    /// <summary>
    /// Eventos de mensagens recebidas
    /// </summary>
    public class MensagemRecebidaEvent
    {
        public string EmpresaId { get; set; }
        public string Plataforma { get; set; }
        public string Remetente { get; set; }
        public string Destinatario { get; set; }
        public string Conteudo { get; set; }
        public string? MidiaUrl { get; set; }
        public string? TipoMidia { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime DataRecebimento { get; set; }
    }
}