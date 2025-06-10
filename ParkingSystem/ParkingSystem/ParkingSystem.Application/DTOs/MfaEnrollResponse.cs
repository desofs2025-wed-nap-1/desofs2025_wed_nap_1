namespace ParkingSystem.Application.DTOs
{
    public class MfaEnrollResponse
        {
            public string id { get; set; } = string.Empty;
            public string type { get; set; } = string.Empty;
            public string friendly_name { get; set; } = string.Empty;
            public TotpInfo totp { get; set; } = new TotpInfo();

            public string Id => id;
            public string Type => type;
            public string FriendlyName => friendly_name;
            public TotpInfo Totp => totp;

            public class TotpInfo
            {
                public string qr_code { get; set; } = string.Empty;
                public string secret { get; set; } = string.Empty;
                public string uri { get; set; } = string.Empty;

                public string QrCode => qr_code;
                public string Secret => secret;
                public string Uri => uri;
            }
        }
}
