using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Service.IService;
using Service;
using System.Data;
using Entities;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace FeedbackPlatform.Hubs
{
    public class ArtworkHub : Hub
    {
        private readonly IServiceManager _serviceManager;
        private object locker = new();
        private readonly ApplicationContext _applicationContext;
        private readonly ILogger<ArtworkHub> _logger;

        public ArtworkHub(IServiceManager serviceManager, ApplicationContext applicationContext, ILogger<ArtworkHub> logger)
        {
            _serviceManager = serviceManager;
            _applicationContext = applicationContext;
            _logger = logger;
        }

        public async Task RateArtwork(Guid artworkId, Guid userId, int rateValue)
        {
            using var transaction = _applicationContext.Database.BeginTransaction();

            try
            {
                lock (locker)
                {
                    if (_serviceManager.RatedArtwork.IsRatedByUser(userId, artworkId))
                    {
                        _serviceManager.RatedArtwork.RemoveUserRate(userId, artworkId);
                        _serviceManager.Save();
                    }

                    _serviceManager.RatedArtwork.AddRatedArtwork(userId, artworkId, rateValue);
                    _serviceManager.Save();

                    var averageRate = _serviceManager.RatedArtwork.GetAverageRate(artworkId, rateValue);

                    var artwork = _serviceManager.Artwork.GetArtwork(artworkId, true);

                    _serviceManager.Artwork.RateArtwork(artwork, averageRate);
                    _serviceManager.Save();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }

            await Clients.All.SendAsync("RatedArtwork");
        }
    }
}
