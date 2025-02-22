
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductManagement.Models
{
    [Table("ProductImage")]
    public class ProductImage
    {
        [Key]
        public Guid ImageID { get; set; }

        [Required]
        public Guid ProductID { get; set; }

        [Required]
        [StringLength(1000)]
        public string ProductImageURL { get; set; }

        [StringLength(50)]
        public string FileName { get; set; }

        [StringLength(10)]
        public string FileExtension { get; set; }
    }
}
