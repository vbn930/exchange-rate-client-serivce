using System;
using System.Drawing;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using ExchangeRateClientService.Utils;

namespace ExchangeRateClientService.Azure;

public class KeyVaultWapper
{
    private readonly Logger _logger;
    private readonly SecretClient _client;

    public KeyVaultWapper(IConfiguration configuration)
    {
        if (null == configuration)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        string serviceName = configuration["Logging:ServiceName"];
        _logger = new Logger(serviceName);

        string keyVaultUri = configuration["AzureFileServer:ConnectionStrings:BlobStorageEndpoint"];
        if (string.IsNullOrEmpty(keyVaultUri))
        {
            _client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        }
        else
        {
            throw new ArgumentException(nameof(keyVaultUri));
        }
    }

    public async Task<string> GetSecretAsync(string secretKey)
    {
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentException("Secret key cannot be null or empty", nameof(secretKey));
        }

        using (var log = _logger.StartMethod(nameof(GetSecretAsync)))
        {
            log.SetAttribute("secretKey", secretKey);
            var secret = await _client.GetSecretAsync(secretKey);
            string value = secret.Value.Value;

            return value;
        }
    }
}