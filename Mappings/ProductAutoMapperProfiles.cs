// File: Profiles/ProductProfile.cs
using AutoMapper;
using ProductManagement.Models;
using ProductManagement.Models.DTO;
using ProductManagement.Utilities;  // Assuming EnumHelper is defined here

namespace ProductManagement.Mappings
{
    public class ProductAutoMapperProfiles : Profile
    {
        public ProductAutoMapperProfiles()
        {
            // Map from Product entity to ProductDTO.
            CreateMap<Product, ProductDTO>();
            //.ForMember(
            //    dest => dest.CategoryDescription,
            //    opt => opt.MapFrom(src => EnumHelper.GetDescription<Category>(src.Category))
            //);

            // Map from CreateProductDTO to Product entity.
            CreateMap<CreateProductDTO, Product>();
            //.ForMember(
            //    dest => dest.Category,
            //    opt => opt.MapFrom(src => EnumHelper.GetEnumFromDescription<Category>(src.CategoryDescription))
            //);

            // Map from UpdateProductDTO to Product entity.
            CreateMap<UpdateProductDTO, Product>();
            //.ForMember(
            //    dest => dest.Category,
            //    opt => opt.MapFrom(src => EnumHelper.GetEnumFromDescription<Category>(src.CategoryDescription))
            //);

            // ✅ New Mappings for ProductImage and ProductImageDTO
            CreateMap<ProductImage, ProductImageDTO>().ReverseMap();
        }
    }
}

