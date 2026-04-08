using System;
using System.Collections.Generic;

namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class ProductTypeDto
    {
        public int ProductTypeId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool SupportsAllocation { get; set; }
        public bool SupportsProfitSharing { get; set; }
    }

    public class ProductDto
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int ProductTypeId { get; set; }
        public string? RiskLevel { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int? MinTenorInDays { get; set; }
        public int? MaxTenorInDays { get; set; }
        public string? ProductTypeName { get; set; }
        public List<AssetAllocationDto>? Allocations { get; set; }
    }

    public class AssetTypeDto
    {
        public int AssetTypeId { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool IsShariahCompliant { get; set; }
    }

    public class AssetAllocationDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int AssetTypeId { get; set; }
        public decimal TargetPercentage { get; set; }
        public decimal MinPercentage { get; set; }
        public decimal MaxPercentage { get; set; }
        public string? AssetTypeName { get; set; }
    }

    public class CreateProductRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public int ProductTypeId { get; set; }
        public string? RiskLevel { get; set; }
        public string? Description { get; set; }
        public int? MinTenorInDays { get; set; }
        public int? MaxTenorInDays { get; set; }
    }
}
