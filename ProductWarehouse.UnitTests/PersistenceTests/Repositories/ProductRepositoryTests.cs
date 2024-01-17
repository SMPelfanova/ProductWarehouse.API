﻿namespace ProductWarehouse.UnitTests.PersistenceTests.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using ProductWarehouse.Domain.Entities;
using ProductWarehouse.Infrastructure.Http;
using ProductWarehouse.Persistence.Repositories;
using System.Net;
using Xunit;

public class ProductRepositoryTests
{
    [Fact]
    public async Task GetProductsAsync_ReturnsEmptyList()
    {
        // Arrange
        string baseUrl = "https://testlink.com";
        string productUrl = "/v2/testlink";
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var httpClientMock = new HttpClient(httpMessageHandlerMock.Object);
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<MockyClientService>>();
        var httpClient = new MockyClientService(httpClientMock, loggerMock.Object);

        configurationMock.Setup(x => x.GetSection("MockyClient:BaseUrl").Value)
                         .Returns(baseUrl);

        configurationMock.Setup(x => x.GetSection("MockyClient:ProductUrl").Value)
                         .Returns(productUrl);

        var expectedProducts = new List<Product>();

        var jsonString = JsonConvert.SerializeObject(expectedProducts);

        httpMessageHandlerMock
         .Protected()
         .Setup<Task<HttpResponseMessage>>(
             "SendAsync",
             ItExpr.IsAny<HttpRequestMessage>(),
             ItExpr.IsAny<CancellationToken>()
         )
         .ReturnsAsync(new HttpResponseMessage
         {
             Content = new StringContent(jsonString),
             StatusCode = HttpStatusCode.OK,
         });

        var productRepository = new ProductRepository(httpClient, configurationMock.Object);

        var minPrice = 10m;
        var maxPrice = 100m;
        var size = "Medium";

        // Act
        var result = await productRepository.GetProductsAsync(minPrice, maxPrice, size);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProductsAsync_ReturnsFilteredProducts()
    {
        // Arrange
        string baseUrl = "https://testlink.com";
        string productUrl = "/v2/testlink";
        var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        var httpClientMock = new HttpClient(httpMessageHandlerMock.Object);
        var configurationMock = new Mock<IConfiguration>();
        var loggerMock = new Mock<ILogger<MockyClientService>>();
        var httpClient = new MockyClientService(httpClientMock, loggerMock.Object);

        configurationMock.Setup(x => x.GetSection("MockyClient:BaseUrl").Value)
                    .Returns(baseUrl);

        configurationMock.Setup(x => x.GetSection("MockyClient:ProductUrl").Value)
                         .Returns(productUrl);


        var expectedProducts = new List<Product>
        {
            new Product { Title = "Test", Description = "test", Price = 10, Sizes = new List<string>{ "Small" } },
            new Product { Title = "Test 2", Description = "test 2", Price = 10, Sizes = new List<string>{ "Medium" } },
        };

        var jsonString = JsonConvert.SerializeObject(expectedProducts);

        httpMessageHandlerMock
         .Protected()
         .Setup<Task<HttpResponseMessage>>(
             "SendAsync",
             ItExpr.IsAny<HttpRequestMessage>(),
             ItExpr.IsAny<CancellationToken>()
         )
         .ReturnsAsync(new HttpResponseMessage
         {
             Content = new StringContent(jsonString),
             StatusCode = HttpStatusCode.OK,
         });

        var productRepository = new ProductRepository(httpClient, configurationMock.Object);

        var minPrice = 10m;
        var maxPrice = 100m;
        var size = "Medium";

        // Act
        var result = await productRepository.GetProductsAsync(minPrice, maxPrice, size);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.All(p =>
            (minPrice == 0 || p.Price >= minPrice) &&
            (maxPrice == 0 || p.Price <= maxPrice) &&
            (string.IsNullOrEmpty(size) || p.Sizes.Any(s => s.ToLowerInvariant() == size.ToLowerInvariant()))
        ));
    }
}