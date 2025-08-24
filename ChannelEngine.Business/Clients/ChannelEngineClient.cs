using ChannelEngine.Business.Models;
using ChannelEngine.Business.Models.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace ChannelEngine.Business.Clients
{
    public class ChannelEngineClient : IChannelEngineClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ChannelEngineClient> _logger;
        private readonly string _apiKey;

        public ChannelEngineClient(
            HttpClient httpClient,
            ILogger<ChannelEngineClient> logger,
            IOptions<ChannelEngineOptions> options)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var opts = options?.Value ?? throw new ArgumentNullException(nameof(options));

            _apiKey = opts.ApiKey ?? string.Empty;

            _httpClient.BaseAddress = string.IsNullOrEmpty(opts.BaseUrl) 
                ? null 
                : new Uri(opts.BaseUrl);

            _httpClient.DefaultRequestHeaders.Add("X-CE-KEY", _apiKey);
        }

        private void EnsureConfigured()
        {
            if (string.IsNullOrWhiteSpace(_httpClient.BaseAddress?.ToString()))
                throw new InvalidOperationException("ChannelEngine BaseUrl is missing in configuration, please review your appsettings.json file!");

            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("ChannelEngine API key is missing in configuration, please review your appsettings.json file!");
        }

        public async Task<List<Order>> GetOrdersAsync(string status = "IN_PROGRESS", CancellationToken ct = default)
        {
            EnsureConfigured();

            var response = await _httpClient.GetAsync($"orders?statuses={status}", ct);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(ct);

            var wrapper = JsonSerializer.Deserialize<OrderResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return wrapper?.Content ?? [];
        }

        public async Task<SetStockApiResponse> UpdateStockAsync(UpdateStockRequest request, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            EnsureConfigured();

            var payload = new[] { request };

            var response = await _httpClient.PutAsJsonAsync("v2/offer/stock", payload, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Stock update failed: {Response}", response.ReasonPhrase);
                throw new ChannelEngineApiException($"Stock quantity update failed for MerchantProductNo: {request.MerchantProductNo} - API Response: {response.ReasonPhrase}");
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<SetStockApiResponse>(cancellationToken: ct);

            return apiResponse == null
                ? throw new ChannelEngineApiException("Stock update API returned an empty or invalid response.")
                : apiResponse;
        }
    }
}
