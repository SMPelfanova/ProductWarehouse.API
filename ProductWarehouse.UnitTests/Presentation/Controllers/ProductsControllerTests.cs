﻿namespace ProductWarehouse.UnitTests.API.Controllers;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductWarehouse.API.Controllers;
using ProductWarehouse.API.Models.Requests;
using ProductWarehouse.API.Models.Responses;
using ProductWarehouse.Application.Features.Queries.GetProducts;
using ProductWarehouse.Application.Models;
using ProductWarehouse.UnitTests;
using Xunit;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetProducts_ReturnsNotFound_WhenNoProducts()
    {
        // Arrange
        var loggerMock = A.Fake<ILogger<ProductsController>>();
        var mediatorMock = A.Fake<IMediator>();
        var mapperMock = A.Fake<IMapper>();

        var controller = new ProductsController(loggerMock, mediatorMock, mapperMock);

        A.CallTo(() => mediatorMock.Send(A<ProductsQuery>._, CancellationToken.None))
                    .Returns(new ProductsFilterDto());

        // Act
        var result = await controller.GetProducts();

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetProducts_ReturnsOk_WithProducts()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        var loggerMock = A.Fake<ILogger<ProductsController>>();
        var mediatorMock = A.Fake<IMediator>();
        var mapperMock = TestStartup.CreateMapper();
        var controller = new ProductsController(loggerMock, mediatorMock, mapperMock);

        var products = fixture.CreateMany<ProductDto>(2);

        A.CallTo(() => mediatorMock.Send(A<ProductsQuery>._, CancellationToken.None))
                    .Returns(new ProductsFilterDto()
                    {
                        Products = products
                    });

        // Act
        var result = await controller.GetProducts();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task GetProducts_WithFilter_ReturnsOk_WithFilteredProducts()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customize(new AutoMoqCustomization());
        var loggerMock = A.Fake<ILogger<ProductsController>>();
        var mediatorMock = A.Fake<IMediator>();
        var mapperMock = TestStartup.CreateMapper();

        var searchFilter = new ProductsQuery { Highlight = "string", MaxPrice = 13, MinPrice = 1, Size = "Small" };
        var filteredProducts = fixture.CreateMany<ProductResponse>(2);

        var products = fixture.CreateMany<ProductDto>(2);
        A.CallTo(() => mediatorMock.Send(A<ProductsQuery>._, CancellationToken.None))
                   .Returns(new ProductsFilterDto
                   {
                       Products = products
                   });

        var controller = new ProductsController(loggerMock, mediatorMock, mapperMock);

        // Act
        var result = await controller.GetProducts(new FilterProductsRequest { Size = searchFilter.Size, MinPrice = searchFilter.MinPrice, MaxPrice = searchFilter.MaxPrice, Highlight = searchFilter.Highlight });

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Should().NotBeNull();
    }
}