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
    public class ArtworkRepository : RepositoryBase<Artwork>, IArtworkRepository
    {
        public ArtworkRepository(ApplicationContext applicationContext) :
            base(applicationContext) { }

        public Artwork AddArtwork(string artworkName)
        {
            var artwork = new Artwork
            {
                Name = artworkName
            };

            Create(artwork);

            return artwork;
        }

        public Artwork FindDuplicateArtwork(string artworkName, bool trackChanges) =>
            FindByCondition(a => a.Name.Equals(artworkName), trackChanges).FirstOrDefault();

        public IEnumerable<Artwork> GetAllArtworks(bool trackChanges) =>
            FindAll(trackChanges)
            .OrderBy(artowrk => artowrk.Name)
            .ToList();

        public Artwork GetArtwork(Guid id, bool trackChanges) =>
            FindByCondition(a => a.Id.Equals(id), trackChanges).FirstOrDefault();

        public void RateArtwork(Artwork artwork, int rateValue)
        {
            artwork.Rate = rateValue;

            Update(artwork);
        }

        public void RemoveArtwork(Guid id)
        {
            var artwork = GetArtwork(id, false);

            Delete(artwork);
        }
    }
}
