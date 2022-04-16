﻿using System.Net.Http;
using System.Threading.Tasks;
using Bots.Api.Client;
using Bots.Api.Models.Enums;
using Bots.Api.Models.Positions;
using FluentAssertions;
using Xunit;

namespace Bots.Api.Tests.Client {
    public class BotsIntegrationTestPositionApi : BaseBotsIntegrationTestClient {
        [Fact]
        public async Task PlaceOrder_CanPlaceOrder() {
            // Arrange
            var options = CreateOptions();
            var httpClient = new HttpClient();
            var client = new BotsPositionApi(options, httpClient);
            var request = new GetBotPositionsRequest {
                SignalProvider = options.Value.SignalProvider,
                SignalProviderKey = options.Value.SignalProviderKey,
                Exchange = Exchange.Binance,
                BaseAsset = "USDT"
            };
            
            // Act
            var positions = await client.GetBotPositions(request);

            // Assert
            positions.Should().NotBeNull();
        }
    }
    
    // TODO: Unit tests
    
    // TODO: Exception handling
    
    // TODO: Automatically run unit tests
}