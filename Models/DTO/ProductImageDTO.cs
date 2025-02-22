

using System;

namespace ProductManagement.Models.DTO
{
    public class ProductImageDTO
    {
        public Guid ProductID { get; set; }

        public string ProductImageURL { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }
    }
}

