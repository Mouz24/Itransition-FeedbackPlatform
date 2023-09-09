using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RatedArtworksRepository : RepositoryBase<RatedArtwork>, IRatedArtworkRepository
    {
        public RatedArtworksRepository(ApplicationContext applicationContext) :
            base(applicationContext) { }

        public void AddRatedArtwork(Guid userId, Guid artworkId, int rateValue)
        {
            var ratedArtwork = new RatedArtwork
            {
                Rate = rateValue,
                UserId = userId,
                ArtworkId = artworkId
            };

            Create(ratedArtwork);
        }

        public int GetAverageRate(Guid artworkId)
        {
            var rates = FindByCondition(artwork => artwork.ArtworkId == artworkId, false).Select(artwork => artwork.Rate).ToList();

            return rates.Sum() / rates.Count;
        }

        public bool IsRatedByUser(Guid userId, Guid artworkId)
        {
            var ratedArtworks = FindAll(false).ToList();
            var isRatedByUser = ratedArtworks.Any(artwork => artwork.UserId == userId && artwork.ArtworkId == artworkId);

            return isRatedByUser;
        }

        public void RemoveUserRate(Guid userId, Guid artworkId)
        {
            var userRate = FindByCondition(like => like.UserId == userId && like.ArtworkId == artworkId, false).FirstOrDefault();

            Delete(userRate);
        }
    }
}
