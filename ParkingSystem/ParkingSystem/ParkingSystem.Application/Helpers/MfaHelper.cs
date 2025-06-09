using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ParkingSystem.Application.Helpers
{
    public static class MfaHelper
    {
        public static string GenerateTotpQrCodeUri(string email, string secret, string issuer = "ParkingSystem")
        {
            return $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}?secret={secret}&issuer={Uri.EscapeDataString(issuer)}";
        }

        public static string GenerateQrCodeImage(string uri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCode(qrCodeData);
            using Bitmap qrBitmap = qrCode.GetGraphic(20);

            using var ms = new MemoryStream();
            qrBitmap.Save(ms, ImageFormat.Png);
            return $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}";
        }
    }
}
