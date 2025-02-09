using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Models
{
    public class Product
    {

        [Key]
        public Guid ProductID { get; set; }

        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; }

        public Guid? ManufacturerID { get; set; }

        [Required]
        [StringLength(255)]
        public string ProductName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int Category { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? WholesalePrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? RetailPrice { get; set; }

        public int? Quantity { get; set; }

        [StringLength(3)]
        public string RetailCurrency { get; set; }

        [StringLength(3)]
        public string WholeSaleCurrency { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ShippingCost { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime UpdatedOn { get; set; }

        public bool? IsActive { get; set; }
    }
}
