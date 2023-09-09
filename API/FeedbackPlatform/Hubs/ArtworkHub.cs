using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Service.IService;
using Service;
using System.Data;

namespace FeedbackPlatform.Hubs
{
    [Authorize(Roles = "Administrator, User")]
    public class ArtworkHub : Hub
    {
        private readonly IServiceManager _serviceManager;
        private object locker = new();

        public ArtworkHub(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public async Task RateArtwork(Guid artworkId, Guid userId, int rateValue)
        {
            lock(locker)
            {
                if (_serviceManager.RatedArtwork.IsRatedByUser(userId, artworkId))
                {
                    _serviceManager.RatedArtwork.RemoveUserRate(userId, artworkId);
                }

                _serviceManager.RatedArtwork.AddRatedArtwork(userId, artworkId, rateValue);

                var averageRate = _serviceManager.RatedArtwork.GetAverageRate(artworkId);
                
                _serviceManager.Artwork.RateArtwork(artworkId, rateValue);
            }

            await _serviceManager.SaveAsync();

            await Clients.All.SendAsync("RatedReview");
        }
    }
}
