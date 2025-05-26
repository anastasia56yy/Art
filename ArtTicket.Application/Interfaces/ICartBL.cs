using ArtTicket.Domain.DTOs;

namespace ArtTicket.Application.Interfaces
{
    public interface ICartBL
    {
        CartResultDto GetCart(string userEmail);
        CartResultDto AddToCart(string userEmail, int ticketId, int quantity);
        CartResultDto RemoveFromCart(string userEmail, int cartItemId);
        CartResultDto UpdateCartItemQuantity(string userEmail, int cartItemId, int quantity);
        CartResultDto ClearCart(string userEmail);
    }
} 