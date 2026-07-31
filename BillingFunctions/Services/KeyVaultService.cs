using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace BillingFunctions.Services;

public class KeyVaultService
{
    private readonly SecretClient _client;

    public KeyVaultService(IConfiguration configuration)
    {
        string vaultUrl =
            configuration["KeyVaultUrl"] ?? "";

        _client = new SecretClient(
            new Uri(vaultUrl),
            new DefaultAzureCredential());
    }

    public async Task SavePasswordAsync(
        string secretName,
        string password)
    {
        await _client.SetSecretAsync(
            secretName,
            password);
    }


    public async Task<string> GetSecretAsync(
    string secretName)
{
    KeyVaultSecret secret =
        await _client.GetSecretAsync(secretName);

    return secret.Value;
}

}