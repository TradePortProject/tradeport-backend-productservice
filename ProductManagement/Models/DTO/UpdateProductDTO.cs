namespace ProductManagement.Models.DTO
{
    public class UpdateProductDTO
    {
      //  public Guid? ManufacturerID { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public decimal? WholesalePrice { get; set; }

        public decimal? RetailPrice { get; set; }

        public int? Quantity { get; set; }

        public string RetailCurrency { get; set; }

        public string WholeSaleCurrency { get; set; }

        public decimal? ShippingCost { get; set; }

    }
}
