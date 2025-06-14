using Microsoft.EntityFrameworkCore;
using ParkingSystem.Core.Aggregates;
using ParkingSystem.Core.Interfaces;
using ParkingSystem.Infrastructure.Data;
namespace ParkingSystem.Infrastructure.Repositories
{
    public class SubscriptionRepository : ISubscriptionRepository
    {
        private readonly AppDbContext _context;

        public SubscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ActivateSubscription(long userId, long parkId)
        {
            try
            {
                var subscription = new Subscription
                {
                    userId = userId,
                    parkId = parkId,
                    isActive = true,
                    startDate = DateTime.UtcNow,
                    endDate = DateTime.UtcNow.AddMonths(1)
                };
                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error activating subscription" + ex.Message);
            }
        }

        public async Task<Subscription?> GetActiveSubscription(long userId, long parkId)
        {
            return await _context.Subscriptions
        .FirstOrDefaultAsync(s => s.userId == userId 
                               && s.parkId == parkId 
                               && s.isActive);
        }

    }
}
