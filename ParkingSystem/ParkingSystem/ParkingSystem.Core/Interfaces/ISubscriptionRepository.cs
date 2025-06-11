using ParkingSystem.Core.Aggregates;
namespace ParkingSystem.Core.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<bool> ActivateSubscription(long userId, long parkId);
        Task<Subscription?> GetActiveSubscription(long userId, long parkId);
    }
}
