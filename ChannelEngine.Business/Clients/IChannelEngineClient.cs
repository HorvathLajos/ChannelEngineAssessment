using ChannelEngine.Business.Models;
using ChannelEngine.Business.Models.APICommunication;

namespace ChannelEngine.Business.Clients
{
    public interface IChannelEngineClient
    {
        Task<List<Order>> GetOrdersAsync(string status = "IN_PROGRESS", CancellationToken ct = default);
        Task<SetStockApiResponse> UpdateStockAsync(UpdateStockRequest request, CancellationToken ct = default);
    }
}
