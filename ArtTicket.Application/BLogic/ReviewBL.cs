using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ArtTicket.Application.Interfaces;
using ArtTicket.Domain.DTOs;
using ArtTicket.Domain.Models;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Application.BLogic
{
    public class ReviewBL : IReviewBL
    {
        private readonly ArtTicketDbContext _dbContext;
        private readonly IUserBL _userBL;

        public ReviewBL(ArtTicketDbContext dbContext)
        {
            _dbContext = dbContext;
            _userBL = new UserBL(dbContext); // Временное решение, в идеале использовать DI
        }

        public List<ReviewDto> GetReviewsByEventId(int eventId)
        {
            var reviews = _dbContext.Reviews
                .Include(r => r.Event)
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            return reviews.Select(MapToDto).ToList();
        }

        public ReviewDto GetReviewById(int id)
        {
            var review = _dbContext.Reviews
                .Include(r => r.Event)
                .Include(r => r.User)
                .FirstOrDefault(r => r.Id == id);

            if (review == null)
            {
                return null;
            }

            return MapToDto(review);
        }

        public void CreateReview(ReviewDto reviewDto)
        {
            var review = new Review
            {
                Text = reviewDto.Text,
                Rating = reviewDto.Rating,
                CreatedDate = DateTime.Now,
                EventId = reviewDto.EventId,
                UserId = reviewDto.UserId
            };

            _dbContext.Reviews.Add(review);
            _dbContext.SaveChanges();
        }

        public void UpdateReview(ReviewDto reviewDto)
        {
            var review = _dbContext.Reviews.Find(reviewDto.Id);
            
            if (review != null)
            {
                review.Text = reviewDto.Text;
                review.Rating = reviewDto.Rating;
                
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

        public bool CanUserAccessReview(string userEmail, int reviewId)
        {
            return _userBL.CanUserAccessReview(userEmail, reviewId);
        }

        private ReviewDto MapToDto(Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                Text = review.Text,
                Rating = review.Rating,
                CreatedDate = review.CreatedDate,
                EventId = review.EventId,
                EventTitle = review.Event?.Title,
                UserId = review.UserId,
                UserName = $"{review.User?.FirstName} {review.User?.LastName}",
                UserEmail = review.User?.Email
            };
        }
    }
} 