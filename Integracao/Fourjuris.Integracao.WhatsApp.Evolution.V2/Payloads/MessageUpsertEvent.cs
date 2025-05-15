using System.Text.Json.Serialization;

namespace Fourjuris.Integracao.WhatsApp.Evolution.V2.Payloads
{
    public class MessageUpsertEvent
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("instance")]
        public string Instance { get; set; }

        [JsonPropertyName("data")]
        public Data Data { get; set; }

        [JsonPropertyName("destination")]
        public string Destination { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("sender")]
        public string Sender { get; set; }

        [JsonPropertyName("server_url")]
        public string ServerUrl { get; set; }

        [JsonPropertyName("apikey")]
        public string ApiKey { get; set; }
    }
    public class Data
    {
        [JsonPropertyName("key")]
        public Key Key { get; set; }

        [JsonPropertyName("pushName")]
        public string PushName { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("messageType")]
        public string MessageType { get; set; }

        [JsonPropertyName("messageTimestamp")]
        public long MessageTimestamp { get; set; }

        [JsonPropertyName("instanceId")]
        public string InstanceId { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }
    public class Key
    {
        [JsonPropertyName("remoteJid")]
        public string RemoteJid { get; set; }

        [JsonPropertyName("fromMe")]
        public bool FromMe { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
    public class Message
    {
        [JsonPropertyName("conversation")]
        public string Conversation { get; set; }

        [JsonPropertyName("messageContextInfo")]
        public MessageContextInfo MessageContextInfo { get; set; }
    }
    public class MessageContextInfo
    {
        [JsonPropertyName("deviceListMetadata")]
        public DeviceListMetadata DeviceListMetadata { get; set; }

        [JsonPropertyName("deviceListMetadataVersion")]
        public int DeviceListMetadataVersion { get; set; }

        [JsonPropertyName("messageSecret")]
        public string MessageSecret { get; set; }
    }
    public class DeviceListMetadata
    {
        [JsonPropertyName("senderKeyHash")]
        public string SenderKeyHash { get; set; }

        [JsonPropertyName("senderTimestamp")]
        public string SenderTimestamp { get; set; }

        [JsonPropertyName("recipientKeyHash")]
        public string RecipientKeyHash { get; set; }

        [JsonPropertyName("recipientTimestamp")]
        public string RecipientTimestamp { get; set; }
    }
}
