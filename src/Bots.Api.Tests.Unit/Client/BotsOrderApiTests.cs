﻿using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Bots.Api.Client;
using Bots.Api.Configuration;
using Bots.Api.Models.Enums;
using Bots.Api.Models.Orders;
using FluentAssertions;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using Xunit;

namespace Bots.Api.Tests.Unit.Client {
    public class BotsOrderApiTests {
        [Fact]
        public async Task PlaceOrder_DefaultBehaviour_ResponseIsParsed() {
            // Arrange
            var options = CreateOptions();
            string expectedUrl = $"{options.Value.BaseEndpoint}v2/placeOrder";
            var orderId = "order-id";
            var response = @$"{{
    ""isBeingCanceled"": ""no"",
    ""orderId"": ""{orderId}"",
    ""status"": ""acceptedByExch"",
    ""success"": true
}}";
            var placeOrderRequest = CreatePlaceOrderRequest(options.Value);
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Post, expectedUrl)
                .WithPartialContent(CreatePlaceOrderRequestJson(placeOrderRequest, options.Value))
                .Respond("application/json", response);
            var orderClient = new BotsOrderApi(options, mockHttp.ToHttpClient());

            // Act
            var result = await orderClient.PlaceOrder(placeOrderRequest);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            result.OrderId.Should().Be(orderId);
            result.IsBeingCanceled.Should().BeFalse();
            result.Status.Should().Be(OrderStatus.AcceptedByExchange);
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetOrderState_DefaultBehaviour_ResponseIsParsed() {
            // Arrange
            var options = CreateOptions();
            var orderId = "order-id";
            string expectedUrl = $"{options.Value.BaseEndpoint}v2/getOrderState?signalProvider={options.Value.SignalProvider}&signalProviderKey={options.Value.SignalProviderKey}&orderId={orderId}";
            var placeOrderRequest = CreateGetOrderStateRequest(options.Value, orderId);
            var response =  $@"{{
    ""isBeingCanceled"": ""no"",
    ""orderId"": ""{orderId}"",
    ""status"": ""acceptedByExch"",
    ""success"": true
}}";
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, expectedUrl)
                .Respond("application/json", response);
            var orderClient = new BotsOrderApi(options, mockHttp.ToHttpClient());

            // Act
            var result = await orderClient.GetOrderState(placeOrderRequest);

            // Assert
            mockHttp.VerifyNoOutstandingExpectation();
            result.OrderId.Should().Be(orderId);
            result.IsBeingCanceled.Should().BeFalse();
            result.Status.Should().Be(OrderStatus.AcceptedByExchange);
            result.Success.Should().BeTrue();
        }

        private string CreatePlaceOrderRequestJson(PlaceOrderRequest request, BotsConfiguration options) {
            return $@"{{
  ""extId"": ""{request.ExternalId}"",
  ""exchange"": ""{request.Exchange.ToString().ToLower()}"",
  ""baseAsset"": ""{request.BaseAsset}"",
  ""quoteAsset"": ""{request.QuoteAsset}"",
  ""type"": ""{request.Type.ToString().ToLower()}"",
  ""side"": ""{request.Side.ToString().ToLower()}"",
  ""limitPrice"": ""{request.LimitPrice.ToString(CultureInfo.InvariantCulture)}"",
  ""stopPrice"": ""{request.StopPrice.ToString(CultureInfo.InvariantCulture)}"",
  ""qtyPct"": ""{request.QuantityPercent.ToString(CultureInfo.InvariantCulture)}"",
  ""qtyAbs"": ""{request.QuantityAbs.ToString(CultureInfo.InvariantCulture)}"",
  ""ttlType"": ""secs"",
  ""ttlSecs"": ""{request.TtlSecs.ToString(CultureInfo.InvariantCulture)}"",
  ""responseType"": ""{request.ResponseType.ToString().ToUpper()}"",
  ""signalProvider"": ""{options.SignalProvider}"",
  ""signalProviderKey"": ""{options.SignalProviderKey}""
}}";
        }

        private PlaceOrderRequest CreatePlaceOrderRequest(BotsConfiguration options) {
            return new PlaceOrderRequest {
                SignalProvider = options.SignalProvider,
                SignalProviderKey = options.SignalProviderKey,
                ExternalId = Guid.NewGuid().ToString(),
                Exchange = Exchange.Binance,
                BaseAsset = "BTC",
                QuoteAsset = "USDT",
                LimitPrice = 40000.56m,
                QuantityPercent = 25m,
                Side = OrderSide.Sell,
                TtlType = TtlType.Seconds,
                TtlSecs = 30,
                Type = OrderType.Limit,
                ResponseType = ResponseType.Ack
            };
        }

        private GetOrderStateRequest CreateGetOrderStateRequest(BotsConfiguration options, string orderId) {
            return new GetOrderStateRequest {
                SignalProvider = options.SignalProvider,
                SignalProviderKey = options.SignalProviderKey,
                OrderId = orderId
            };
        }
        
        private IOptions<BotsConfiguration> CreateOptions() {
            return Options.Create(new BotsConfiguration {
                BaseEndpoint = "https://signal.revenyou.io/paper/api/signal/",
                SignalProvider = "signal-provider",
                SignalProviderKey = "signal-provider-key"
            });
        }
    }
}