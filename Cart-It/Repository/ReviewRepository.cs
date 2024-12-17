using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface IReviewRepository
    {
        Task<Review> AddReviewAsync(Review review);
        Task<Review> UpdateReviewAsync(int reviewId, ReviewDTO reviewDto);
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<ReviewDTO>> GetReviewsByCustomerIdAsync(int customerId);
        Task<IEnumerable<ReviewDTO>> GetReviewsBySellerIdAsync(int sellerId);
        Task<IEnumerable<Review>> GetAllReviewsAsync();


    }

    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReviewRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Review> AddReviewAsync(Review review)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductId == review.ProductId);
            if (!productExists)
            {
                throw new InvalidOperationException("Product not found.");
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == review.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review> UpdateReviewAsync(int reviewId, ReviewDTO reviewDto)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review == null)
            {
                throw new InvalidOperationException("Review not found.");
            }

            // Only update changed fields
            _mapper.Map(reviewDto, review);

            var productExists = await _context.Products.AnyAsync(p => p.ProductId == review.ProductId);
            if (!productExists)
            {
                throw new InvalidOperationException("Product not found.");
            }

            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == review.CustomerId);
            if (!customerExists)
            {
                throw new InvalidOperationException("Customer not found.");
            }

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _context.Reviews
                                 .Include(r => r.Products)
                                 .Include(r => r.Customers)
                                 .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }
        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _context.Reviews
                .Include(r => r.Customers)
                .Include(r => r.Products)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                                 .Where(r => r.ProductId == productId)
                                 .Include(r => r.Customers)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsByCustomerIdAsync(int customerId)
        {
            return await _context.Reviews
                .Where(r => r.CustomerId == customerId)
                .Select(r => new ReviewDTO
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText
                }).ToListAsync();
        }

        public async Task<IEnumerable<ReviewDTO>> GetReviewsBySellerIdAsync(int sellerId)
        {
            return await _context.Reviews
                .Where(r => r.Products.SellerId == sellerId)
                .Select(r => new ReviewDTO
                {
                    ReviewId = r.ReviewId,
                    ProductId = r.ProductId,
                    Rating = r.Rating,
                    ReviewText = r.ReviewText,
                    ReviewDate = r.ReviewDate
                })
                .ToListAsync();
        }
    }
}
