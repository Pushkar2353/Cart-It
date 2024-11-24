using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;

namespace Cart_It.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Seller, SellerDTO>().ReverseMap();
        }
    }

    public class AdministratorProfile : Profile
    {
        public AdministratorProfile()
        {
            CreateMap<Administrator, AdministratorDTO>().ReverseMap();
        }
    }

    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
        }
    }

    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();
        }
    }

    public class ProductInventoryProfile : Profile
    {
        public ProductInventoryProfile() 
        {
            // Map between ProductInventory and ProductInventoryDto
            CreateMap<ProductInventory, ProductInventoryDTO>().ReverseMap();
        }
    }

    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartDTO>().ReverseMap();
        }
    }

    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDTO>().ReverseMap();
        }
    }

    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDTO>().ReverseMap();
        }
    }

    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDTO>().ReverseMap();
        }
    }
}

