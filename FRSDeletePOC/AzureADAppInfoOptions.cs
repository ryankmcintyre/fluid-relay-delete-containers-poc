namespace FRSDeletePOC
{
    public class AzureADAppInfoOptions
    {
        public const string AzureADAppInfo = "AzureADAppInfo";

        public string Instance { get; set; } = String.Empty;
        public string Tenant { get; set; } = String.Empty;
        public string ClientId { get; set; } = String.Empty;
        public string ClientSecret { get; set; } = String.Empty;
    }
}
