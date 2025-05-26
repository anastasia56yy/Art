using ArtTicket.Application.BLogic;
using ArtTicket.Application.Interfaces;
using ArtTicket.Infrastructure.Data;

namespace ArtTicket.Application
{
    public class BusinessLogicFactory
    {
        private static BusinessLogicFactory _instance;
        private readonly ArtTicketDbContext _dbContext;

        private BusinessLogicFactory()
        {
            _dbContext = new ArtTicketDbContext();
        }

        public static BusinessLogicFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new BusinessLogicFactory();
                }
                return _instance;
            }
        }

        public IUserBL GetUserBL()
        {
            return new UserBL(_dbContext);
        }

        public IEventBL GetEventBL()
        {
            return new EventBL(_dbContext);
        }

        public IReviewBL GetReviewBL()
        {
            return new ReviewBL(_dbContext);
        }

        public ICartBL GetCartBL()
        {
            return new CartBL(_dbContext);
        }

        public IOrderBL GetOrderBL()
        {
            return new OrderBL(_dbContext);
        }
    }
} 