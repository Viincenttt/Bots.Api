using System.Threading.Tasks;
using Bots.Api.Models.Orders;

namespace Bots.Api.Client {
    public interface IBotsOrderApi {
        Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request);
        Task<GetOrderStateResponse> GetOrderState(GetOrderStateRequest request);
        Task<GetOrderInfoResponse> GetOrderInfo(GetOrderInfoRequest request);
        Task<GetOrdersResponse> GetOrders();
    }
}