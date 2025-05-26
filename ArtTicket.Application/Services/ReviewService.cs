using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Application.Services
{
    public class ReviewService
    {
        private readonly ArtTicketDbContext _dbContext;

        public ReviewService(ArtTicketDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Review> GetReviewsByEventId(int eventId)
        {
            return _dbContext.Reviews
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public List<Review> GetReviewsByUserId(int userId)
        {
            return _dbContext.Reviews
                .Include(r => r.Event)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public Review GetReviewById(int id)
        {
            return _dbContext.Reviews
                .Include(r => r.Event)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id);
        }

        public bool UserHasReviewForEvent(int userId, int eventId)
        {
            return _dbContext.Reviews
                .Any(r => r.EventId == eventId && r.UserId == userId);
        }

        public void CreateReview(Review review)
        {
            _dbContext.Reviews.Add(review);
            _dbContext.SaveChanges();
        }

        public void UpdateReview(Review review)
        {
            var existingReview = _dbContext.Reviews.Find(review.Id);
            if (existingReview != null)
            {
                existingReview.Text = review.Text;
                existingReview.Rating = review.Rating;
                _dbContext.SaveChanges();
            }
        }

        public void DeleteReview(int id)
        {
            var review = _dbContext.Reviews.Find(id);
            if (review != null)
            {
                _dbContext.Reviews.Remove(review);
                _dbContext.SaveChanges();
            }
        }
    }
} 