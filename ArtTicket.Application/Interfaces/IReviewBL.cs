using System.Collections.Generic;
using ArtTicket.Domain.DTOs;

namespace ArtTicket.Application.Interfaces
{
    public interface IReviewBL
    {
        List<ReviewDto> GetReviewsByEventId(int eventId);
        ReviewDto GetReviewById(int id);
        void CreateReview(ReviewDto reviewDto);
        void UpdateReview(ReviewDto reviewDto);
        void DeleteReview(int id);
        bool CanUserAccessReview(string userEmail, int reviewId);
    }
} 