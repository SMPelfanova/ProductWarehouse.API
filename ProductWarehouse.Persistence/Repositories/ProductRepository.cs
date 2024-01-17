﻿using Microsoft.Extensions.Configuration;
using ProductWarehouse.Application.Contracts;
using ProductWarehouse.Domain.Entities;
using ProductWarehouse.Infrastructure.Http;

namespace ProductWarehouse.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly MockyClientService _httpClientService;
        private readonly Uri _productURI;

        public ProductRepository(MockyClientService httpClientService, IConfiguration config)
        {
            _httpClientService = httpClientService;

            string baseUrl = config.GetSection("MockyClient:BaseUrl")?.Value ?? throw new ArgumentNullException("MockyClient BaseUrl is missing in configuration");
            string productUrl = config.GetSection("MockyClient:ProductUrl")?.Value ?? throw new ArgumentNullException("MockyClient ProductUrl is missing in configuration");

            Uri baseUri = new Uri(baseUrl);
            _productURI = new Uri(baseUri, productUrl);
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(decimal? minPrice, decimal? maxPrice, string? size)
        {
            var products = await _httpClientService.GetProductListAsync(_productURI);

            products = products.Where(x => (minPrice == 0 || x.Price >= minPrice))
                .Where(x => (maxPrice == 0 || x.Price <= maxPrice))
                .Where(x => (string.IsNullOrEmpty(size) || x.Sizes.Any(s => s.ToLowerInvariant() == size.ToLowerInvariant()))).ToList();

            return products;
        }
    }
}