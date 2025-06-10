using System.Text.Json.Serialization;

namespace ParkingSystem.Application.DTOs
{
    public class MfaEnrollResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("friendly_name")]
        public string FriendlyName { get; set; } = string.Empty;

        [JsonPropertyName("totp")]
        public TotpInfo Totp { get; set; } = new TotpInfo();

        public class TotpInfo
        {
            [JsonPropertyName("qr_code")]
            public string QrCode { get; set; } = string.Empty;

            [JsonPropertyName("secret")]
            public string Secret { get; set; } = string.Empty;

            [JsonPropertyName("uri")]
            public string Uri { get; set; } = string.Empty;
        }
    }
}
