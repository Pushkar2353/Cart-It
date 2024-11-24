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

        public ReviewController(IReviewService reviewService, IMapper mapper)
        {
            _reviewService = reviewService;
            _mapper = mapper;
        }

        // POST: api/review
        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewDto)
        {
            try
            {
                var review = await _reviewService.AddReviewAsync(reviewDto);
                var reviewResponse = _mapper.Map<ReviewDTO>(review);
                return CreatedAtAction(nameof(GetReview), new { reviewId = review.ReviewId }, reviewResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/review/{reviewId}
        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] ReviewDTO reviewDto)
        {
            try
            {
                var review = await _reviewService.UpdateReviewAsync(reviewId, reviewDto);
                if (review == null)
                {
                    return NotFound("Review not found.");
                }
                var reviewResponse = _mapper.Map<ReviewDTO>(review);
                return Ok(reviewResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/review/{reviewId}
        [HttpGet("{reviewId}")]
        public async Task<IActionResult> GetReview(int reviewId)
        {
            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            if (review == null)
            {
                return NotFound("Review not found.");
            }

            var reviewResponse = _mapper.Map<ReviewDTO>(review);
            return Ok(reviewResponse);
        }

        // GET: api/review/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsByProductId(int productId)
        {
            var reviews = await _reviewService.GetReviewsByProductIdAsync(productId);
            var reviewResponses = _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
            return Ok(reviewResponses);
        }
    }
}
