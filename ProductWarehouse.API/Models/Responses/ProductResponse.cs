﻿namespace ProductWarehouse.API.Models.Responses;

public class ProductResponse
{
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<string> Sizes { get; set; } = new List<string>();
    public string Description { get; set; } = string.Empty;
}
