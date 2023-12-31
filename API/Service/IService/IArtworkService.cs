﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IArtworkService
    {
        Artwork AddArtwork(string artworkName);
        void RemoveArtwork(Guid id);
        Artwork GetArtwork(Guid id, bool trackChanges);
        Artwork FindDuplicateArtwork(string artworkName, bool trackChanges);
        void RateArtwork(Artwork artwork, int value);
        IEnumerable<Artwork> GetAllArtworks(bool trackChanges);
    }
}
