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

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                                 .Where(r => r.ProductId == productId)
                                 .Include(r => r.Customers)
                                 .ToListAsync();
        }
    }
}
