using System.Text;
using System.Text.Json;

namespace ExchangeRateClientService.Services
{
    public class APIRequestWrapper
    {
        private readonly HttpClient _httpClient;

        public APIRequestWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string?> PostAsync(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    return responseText;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in PostAsync: {ex.Message}");
            }
        }

        public async Task<string?> PostAsync<T>(string url, T? payload)
        {
            try
            {
                HttpContent? content = null;

                if (payload != null)
                {
                    var json = JsonSerializer.Serialize(payload);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }
                else
                {
                    content = new StringContent("", Encoding.UTF8, "application/json");
                }

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    return responseText;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in PostAsync: {ex.Message}");
            }
        }

        public async Task<string?> GetAsync(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    return responseText;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetAsync: {ex.Message}");
            }
        }


        public async Task<string?> PutAsync(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Put, url);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    return responseText;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in PostAsync: {ex.Message}");
            }
        }

        public async Task<string?> PutAsync<T>(string url, T payload)
        {
            try
            {
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = content
                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    return responseText;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in PutAsync: {ex.Message}");
            }
        }

        public async Task<string?> DeleteAsync(string url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Delete, url);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseText = await response.Content.ReadAsStringAsync();
                    return responseText;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in DeleteAsync: {ex.Message}");
            }
        }
    }
}
