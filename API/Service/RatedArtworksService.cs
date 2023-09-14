using Contracts;
using Entities.Models;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RatedArtworksService : IRatedArtworkService
    {
        private readonly IRatedArtworkRepository _ratedArtworksRepository;

        public RatedArtworksService(IRatedArtworkRepository ratedArtworksRepository)
        {
            _ratedArtworksRepository = ratedArtworksRepository;
        }

        public void AddRatedArtwork(Guid userId, Guid artworkId, int rateValue)
        {
            _ratedArtworksRepository.AddRatedArtwork(userId, artworkId, rateValue);
        }

        public int GetAverageRate(Guid artworkId, int rateValue)
        {
            return _ratedArtworksRepository.GetAverageRate(artworkId, rateValue);
        }

        public bool IsRatedByUser(Guid userId, Guid artworkId)
        {
            return _ratedArtworksRepository.IsRatedByUser(userId, artworkId);
        }

        public void RemoveUserRate(Guid userId, Guid artworkId)
        {
            _ratedArtworksRepository.RemoveUserRate(userId, artworkId);
        }
    }
}
