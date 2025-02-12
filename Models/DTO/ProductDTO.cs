using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models.DTO
{
    public class ProductDTO
    {
        public Guid ProductID { get; set; }
       
        public string ProductCode { get; set; }

        public Guid? ManufacturerID { get; set; }

        public string ProductName { get; set; }
      
        public string Description { get; set; }

        //public int Category { get; set; }
        public string Category { get; set; }

        public decimal? WholesalePrice { get; set; }
      
        public decimal? RetailPrice { get; set; }

        public int? Quantity { get; set; }
        
        public string RetailCurrency { get; set; }
       
        public string WholeSaleCurrency { get; set; }
      
        public decimal? ShippingCost { get; set; }
       
        public DateTime CreatedOn { get; set; }
        
        public DateTime UpdatedOn { get; set; }

        public bool? IsActive { get; set; }
    }
}
