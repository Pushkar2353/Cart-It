using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface ISellerService
    {
        Task<SellerDTO> GetSellerByIdAsync(int sellerId);
        Task<IEnumerable<SellerDTO>> GetAllSellersAsync();
        Task<SellerDTO> CreateSellerAsync(SellerDTO sellerDto);
        Task<SellerDTO> UpdateSellerAsync(int sellerId, SellerDTO sellerDto);
        Task<bool> DeleteSellerAsync(int sellerId);
        Task<IEnumerable<OrderDTO>> GetSellerOrdersAsync(int sellerId);
        Task<IEnumerable<ReviewDTO>> GetSellerProductReviewsAsync(int sellerId);
        Task<IEnumerable<PaymentDTO>> GetSellerPaymentsAsync(int sellerId);

    }

    public class SellerService : ISellerService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public SellerService(ISellerRepository sellerRepository, IMapper mapper, IOrderRepository orderRepository,
        IReviewRepository reviewRepository,
        IPaymentRepository paymentRepository)
        {
            _sellerRepository = sellerRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<SellerDTO> GetSellerByIdAsync(int sellerId)
        {
            var seller = await _sellerRepository.GetSellerByIdAsync(sellerId);
            if (seller == null) return null;
            return _mapper.Map<SellerDTO>(seller);
        }

        public async Task<IEnumerable<SellerDTO>> GetAllSellersAsync()
        {
            var sellers = await _sellerRepository.GetAllSellersAsync();
            return _mapper.Map<IEnumerable<SellerDTO>>(sellers);
        }

        public async Task<SellerDTO> CreateSellerAsync(SellerDTO sellerDto)
        {
            var seller = _mapper.Map<Seller>(sellerDto);
            var createdSeller = await _sellerRepository.AddSellerAsync(seller);
            return _mapper.Map<SellerDTO>(createdSeller);
        }

        public async Task<SellerDTO> UpdateSellerAsync(int id, SellerDTO sellerDto)
        {
            var seller = await _sellerRepository.GetSellerByIdAsync(id);
            if (seller == null)
            {
                return null;
            }

            // Perform partial update logic: update only the fields provided in the DTO
            if (!string.IsNullOrEmpty(sellerDto.FirstName)) seller.FirstName = sellerDto.FirstName;
            if (!string.IsNullOrEmpty(sellerDto.LastName)) seller.LastName = sellerDto.LastName;
            if (!string.IsNullOrEmpty(sellerDto.Email)) seller.Email = sellerDto.Email;
            if (!string.IsNullOrEmpty(sellerDto.SellerPhoneNumber)) seller.SellerPhoneNumber = sellerDto.SellerPhoneNumber;
            if (!string.IsNullOrEmpty(sellerDto.Gender)) seller.Gender = sellerDto.Gender;
            if (!string.IsNullOrEmpty(sellerDto.CompanyName)) seller.CompanyName = sellerDto.CompanyName;
            if (!string.IsNullOrEmpty(sellerDto.AddressLine1)) seller.AddressLine1 = sellerDto.AddressLine1;
            if (!string.IsNullOrEmpty(sellerDto.AddressLine2)) seller.AddressLine2 = sellerDto.AddressLine2;
            if (!string.IsNullOrEmpty(sellerDto.Street)) seller.Street = sellerDto.Street;
            if (!string.IsNullOrEmpty(sellerDto.City)) seller.City = sellerDto.City;
            if (!string.IsNullOrEmpty(sellerDto.State)) seller.State = sellerDto.State;
            if (!string.IsNullOrEmpty(sellerDto.Country)) seller.Country = sellerDto.Country;
            if (!string.IsNullOrEmpty(sellerDto.PinCode)) seller.PinCode = sellerDto.PinCode;
            if (!string.IsNullOrEmpty(sellerDto.GSTIN)) seller.GSTIN = sellerDto.GSTIN;
            if (!string.IsNullOrEmpty(sellerDto.BankAccountNumber)) seller.BankAccountNumber = sellerDto.BankAccountNumber;

            // Optionally map other properties as needed...

            await _sellerRepository.UpdateSellerAsync(seller);
            return _mapper.Map<SellerDTO>(seller);
        }

        public async Task<bool> DeleteSellerAsync(int sellerId)
        {
            return await _sellerRepository.DeleteSellerAsync(sellerId);
        }

        public async Task<IEnumerable<OrderDTO>> GetSellerOrdersAsync(int sellerId)
        {
            return await _orderRepository.GetOrdersBySellerIdAsync(sellerId);
        }

        public async Task<IEnumerable<ReviewDTO>> GetSellerProductReviewsAsync(int sellerId)
        {
            return await _reviewRepository.GetReviewsBySellerIdAsync(sellerId);
        }

        public async Task<IEnumerable<PaymentDTO>> GetSellerPaymentsAsync(int sellerId)
        {
            return await _paymentRepository.GetPaymentsBySellerIdAsync(sellerId);
        }
    }

}
