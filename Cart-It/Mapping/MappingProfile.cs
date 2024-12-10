using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;

namespace Cart_It.Mapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDTO>().ReverseMap();
        }
    }
    public class SellerProfile : Profile
    {
        public SellerProfile()
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
            CreateMap<ProductInventory, ProductInventoryDTO>().ReverseMap();
        }
    }

    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartDTO>().ReverseMap()
            .ForMember(dest => dest.Amount, opt => opt.Ignore());
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

