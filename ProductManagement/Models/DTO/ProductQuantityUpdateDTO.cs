using System;
using System.Text.Json.Serialization;

namespace ProductManagement.Models.DTO
{
    public class ProductQuantityUpdateDTO
    {
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
    }
}
