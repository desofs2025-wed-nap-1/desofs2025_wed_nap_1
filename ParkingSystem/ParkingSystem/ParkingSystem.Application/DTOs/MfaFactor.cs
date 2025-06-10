namespace ParkingSystem.Application.DTOs{
 public class MfaFactor
     {
         public string id { get; set; } = string.Empty;
         public string friendly_name { get; set; } = string.Empty;
         public string factor_type { get; set; } = string.Empty;
         public string status { get; set; } = string.Empty;

         public string Id => id;
         public string FriendlyName => friendly_name;
         public string FactorType => factor_type;
         public bool Verified => status == "verified";
     }

}


