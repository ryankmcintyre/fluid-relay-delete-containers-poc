## Sample app showing how to list and delete Fluid Relay Service containers

A basic .NET 6 API that demonstrates how to get an authentication token for making an Azure Management API call to list and delete Azure Fluid Relay Service containers.

To configure this sample:

1. Create Azure AD application
2. Grant contributor permission for created AD app to FRS instance
3. Create new client secret for AD app
4. Create local `user-secret` using Secret Manager
    a. Enable local secret storage: `dotnet user-secrets init`
    b. Add secret for ClientSecret using value from step 3: `dotnet user-secrets set "AzureADAppInfo:ClientSecret" "<<Secret value>>"`
    c. Add secret for TenantId using your Azure Tenant ID value: `dotnet user-secrets set "AzureADAppInfo:TenantId" "<<TenantId value>>"`
    d. Add secret for ClientId using the ClientId value from step 1: `dotnet user-secrets set "AzureADAppInfo:ClientId" "<<ClientId value>>"`