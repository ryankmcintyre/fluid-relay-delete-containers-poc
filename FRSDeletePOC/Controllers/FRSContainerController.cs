using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace FRSDeletePOC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FRSContainerController : ControllerBase
    {
        private readonly AzureADAppInfoOptions azureADAppInfoOptions;
        private string baseAddress = "https://management.azure.com/subscriptions/";
        private string azureMgmtScope = "https://management.azure.com/.default";

        public FRSContainerController(IOptions<AzureADAppInfoOptions> azureADAppInfoOptions)
        {
            this.azureADAppInfoOptions = azureADAppInfoOptions.Value;
        }

        /// <param name="subscriptionId">The Azure subscription ID for the Fluid Relay Service</param>
        /// <param name="resourceGroup">The Azure resource group name for the Fluid Relay Service</param>
        /// <param name="frsResourceName">The Fluid Relay Service name</param>
        /// <param name="apiVersion">The Azure API version to use for the request (minimum 2022-06-01)</param>
        // GET: api/<FRSContainerController>
        [HttpGet]
        public async Task<FRSContainers> Get(string subscriptionId, string resourceGroup, string frsResourceName, string apiVersion = "2022-06-01")
        {
            // TODO: Check min API version

            string uri = $"{subscriptionId}/resourcegroups/{resourceGroup}/providers/Microsoft.FluidRelay/FluidRelayServers/{frsResourceName}/FluidRelayContainers?api-version={apiVersion}";

            // Authenticate so we can get a token for the Azure API call
            var authResult = await authenticateClient();

            using HttpClient httpClient = createHttpClient(authResult.AccessToken);
            
            var response = await httpClient.GetAsync(uri);

            if(response.IsSuccessStatusCode)
            {
                var fRSContainers = await response.Content.ReadFromJsonAsync<FRSContainers>();

                // TODO: Handle empty
                return fRSContainers;
            }
            else
            {
                // TODO: Add more friendly error response
                string badResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Unsuccessful Azure API call with response: {badResponse}");
            }
        }

        /// <param name="containerId">The Container ID of the Fluid Container to be deleted</param>
        /// <param name="subscriptionId">The Azure subscription ID for the Fluid Relay Service</param>
        /// <param name="resourceGroup">The Azure resource group name for the Fluid Relay Service</param>
        /// <param name="frsResourceName">The Fluid Relay Service name</param>
        /// <param name="apiVersion">The Azure API version to use for the request (minimum 2022-06-01)</param>
        // DELETE api/<FRSContainerController>/5
        [HttpDelete("{containerId}")]
        public async void Delete(string containerId, string subscriptionId, string resourceGroup, string frsResourceName, string apiVersion = "2022-06-01")
        {
            // TODO: Check min API version

            string uri = $"{subscriptionId}/resourcegroups/{resourceGroup}/providers/Microsoft.FluidRelay/FluidRelayServers/{frsResourceName}/FluidRelayContainers/{containerId}?api-version={apiVersion}";

            // Authenticate so we can get a token for the Azure API call
            var authResult = await authenticateClient();

            using HttpClient httpClient = createHttpClient(authResult.AccessToken);

            var response = await httpClient.DeleteAsync(uri);

            if (!response.IsSuccessStatusCode)
            { 
                // TODO: Add more friendly error response
                string badResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Unsuccessful Azure API call with response: {badResponse}");
            }
        }

        private async Task<AuthenticationResult> authenticateClient()
        {
            var authorityUri = new Uri(String.Format(azureADAppInfoOptions.Instance, azureADAppInfoOptions.Tenant));

            var app = ConfidentialClientApplicationBuilder.Create(azureADAppInfoOptions.ClientId)
                .WithClientSecret(azureADAppInfoOptions.ClientSecret)
                .WithAuthority(authorityUri)
                .Build();

            string[] scopes = new string[] { azureMgmtScope };
            AuthenticationResult result = null;
            try
            {
                result = await app.AcquireTokenForClient(scopes)
                                 .ExecuteAsync();
            }
            catch (Exception ex)
            {
                // TODO: Handle internally and add more friendly error response
                throw new Exception($"Error retrieving Azure AD auth token with message: {ex.Message}");
            }

            return result;
        }

        private HttpClient createHttpClient(string accessToken)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(baseAddress)
            };
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return httpClient;
        }
    }
}
