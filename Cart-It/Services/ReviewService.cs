using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface IReviewService
    {
        Task<Review> AddReviewAsync(ReviewDTO reviewDto);
        Task<Review> UpdateReviewAsync(int reviewId, ReviewDTO reviewDto);
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
    }

    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<Review> AddReviewAsync(ReviewDTO reviewDto)
        {
            var review = _mapper.Map<Review>(reviewDto);
            return await _reviewRepository.AddReviewAsync(review);
        }

        public async Task<Review> UpdateReviewAsync(int reviewId, ReviewDTO reviewDto)
        {
            return await _reviewRepository.UpdateReviewAsync(reviewId, reviewDto);
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _reviewRepository.GetReviewByIdAsync(reviewId);
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _reviewRepository.GetReviewsByProductIdAsync(productId);
        }
    }
}
