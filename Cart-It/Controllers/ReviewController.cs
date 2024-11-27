using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cart_It.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, IMapper mapper, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _mapper = mapper;
            _logger = logger;
        }

        // POST: api/review
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewDto)
        {
            if (reviewDto == null)
            {
                _logger.LogWarning("AddReview: Received null review data.");
                return BadRequest("Review data is null.");
            }

            try
            {
                _logger.LogInformation("AddReview: Adding a new review.");
                var review = await _reviewService.AddReviewAsync(reviewDto);
                var reviewResponse = _mapper.Map<ReviewDTO>(review);

                _logger.LogInformation("AddReview: Successfully added review with ID {ReviewId}.", review.ReviewId);
                return CreatedAtAction(nameof(GetReview), new { reviewId = review.ReviewId }, reviewResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddReview: An error occurred while adding a new review.");
                return BadRequest(new { message = "An error occurred while adding the review." });
            }
        }

        // PUT: api/review/{reviewId}
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] ReviewDTO reviewDto)
        {
            if (reviewDto == null || reviewId != reviewDto.ReviewId)
            {
                _logger.LogWarning("UpdateReview: Invalid review data or mismatched ReviewId {ReviewId}.", reviewId);
                return BadRequest("Invalid review data.");
            }

            try
            {
                _logger.LogInformation("UpdateReview: Updating review with ID {ReviewId}.", reviewId);
                var review = await _reviewService.UpdateReviewAsync(reviewId, reviewDto);

                if (review == null)
                {
                    _logger.LogWarning("UpdateReview: Review with ID {ReviewId} not found.", reviewId);
                    return NotFound("Review not found.");
                }

                var reviewResponse = _mapper.Map<ReviewDTO>(review);
                _logger.LogInformation("UpdateReview: Successfully updated review with ID {ReviewId}.", reviewId);
                return Ok(reviewResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateReview: An error occurred while updating review with ID {ReviewId}.", reviewId);
                return BadRequest(new { message = "An error occurred while updating the review." });
            }
        }

        // GET: api/review/{reviewId}
        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReview(int reviewId)
        {
            try
            {
                _logger.LogInformation("GetReview: Fetching review with ID {ReviewId}.", reviewId);
                var review = await _reviewService.GetReviewByIdAsync(reviewId);

                if (review == null)
                {
                    _logger.LogWarning("GetReview: Review with ID {ReviewId} not found.", reviewId);
                    return NotFound("Review not found.");
                }

                var reviewResponse = _mapper.Map<ReviewDTO>(review);
                _logger.LogInformation("GetReview: Successfully fetched review with ID {ReviewId}.", reviewId);
                return Ok(reviewResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReview: An error occurred while fetching review with ID {ReviewId}.", reviewId);
                return StatusCode(500, new { message = "An error occurred while fetching the review." });
            }
        }

        // GET: api/review/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProductId(int productId)
        {
            try
            {
                _logger.LogInformation("GetReviewsByProductId: Fetching reviews for ProductId {ProductId}.", productId);
                var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);

                if (reviews == null || !reviews.Any())
                {
                    _logger.LogWarning("GetReviewsByProductId: No reviews found for ProductId {ProductId}.", productId);
                    return NotFound("No reviews found for this product.");
                }

                var reviewResponses = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
                _logger.LogInformation("GetReviewsByProductId: Successfully fetched {Count} reviews for ProductId {ProductId}.", reviewResponses.Count(), productId);
                return Ok(reviewResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetReviewsByProductId: An error occurred while fetching reviews for ProductId {ProductId}.", productId);
                return StatusCode(500, new { message = "An error occurred while fetching the reviews." });
            }
        }
    }
}
