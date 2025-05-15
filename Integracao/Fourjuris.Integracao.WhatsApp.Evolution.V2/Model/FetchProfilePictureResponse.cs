using System.Text.Json.Serialization;

namespace Fourjuris.Integracao.WhatsApp.Evolution.V2.Model
{
    public class FetchProfilePictureResponse
    {
        [JsonPropertyName("wuid")]
        public string WuId { get; set; }
        [JsonPropertyName("profilePictureUrl")]
        public string ProfilePictureUrl { get; set; }
    }
}
