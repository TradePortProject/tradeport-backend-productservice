using System;

namespace TradeportApi.Models
{
    public class Product
    {
        public Guid ProductID { get; set; }
        public string? ProductCode { get; set; } // Make nullable
        public Guid? ManufacturerID { get; set; }
        public string? ProductName { get; set; } // Make nullable
        public string? Description { get; set; } // Make nullable
        public int Category { get; set; }
        public decimal? WholesalePrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public int? Quantity { get; set; }
        public string? RetailCurrency { get; set; } // Make nullable
        public string? WholeSaleCurrency { get; set; } // Make nullable
        public decimal? ShippingCost { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool? IsActive { get; set; }
    }
}
