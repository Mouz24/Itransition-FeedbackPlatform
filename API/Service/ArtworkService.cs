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
    public class ArtworkService : IArtworkService
    {
        private readonly IArtworkRepository _artworkRepository;

        public ArtworkService(IArtworkRepository artworkRepository)
        {
            _artworkRepository = artworkRepository;
        }

        public Artwork AddArtwork(string artworkName)
        {
            return _artworkRepository.AddArtwork(artworkName);
        }

        public Artwork FindDuplicateArtwork(string artworkName, bool trackChanges)
        {
            return _artworkRepository.FindDuplicateArtwork(artworkName, trackChanges);
        }

        public IEnumerable<Artwork> GetAllArtworks(bool trackChanges)
        {
            return _artworkRepository.GetAllArtworks(trackChanges);
        }

        public Artwork GetArtwork(Guid id, bool trackChanges)
        {
            return _artworkRepository.GetArtwork(id, trackChanges);
        }

        public void RateArtwork(Artwork artwork, int value)
        {
            _artworkRepository.RateArtwork(artwork, value);
        }

        public void RemoveArtwork(Guid id)
        {
            _artworkRepository.RemoveArtwork(id);
        }
    }
}
