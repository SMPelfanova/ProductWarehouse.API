﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductWarehouse.Application.Contracts;
using ProductWarehouse.Application.Extensions;
using ProductWarehouse.Application.Models;

namespace ProductWarehouse.Application.Features.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<ProductsQuery, ProductsFilterDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsHandler> _logger;

    public GetProductsHandler(IProductRepository productRepository, IMapper mapper, ILogger<GetProductsHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductsFilterDto> Handle(ProductsQuery request, CancellationToken cancellationToken)
    {
        //add the fluent validation here
        var products = _productRepository.GetProductsAsync(request.MinPrice, request.MaxPrice, request.Size).Result.ToList();
        if (products.Count <= 0)
        {
            _logger.LogInformation($"No products found for filter: minPrice={request.MinPrice} maxPrice={request.MaxPrice} size={request.Size}");
            return new ProductsFilterDto();
        }

        var productFilter = _mapper.Map<ProductsFilterDto>(products);

        if (!string.IsNullOrEmpty(request.Highlight))
        {
            foreach (var product in products)
            {
                product.Description = product.Description.HighlightKeywords(request.Highlight);
            }
        }

        return productFilter;
    }
}