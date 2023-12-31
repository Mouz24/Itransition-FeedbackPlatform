﻿using Entities.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IArtworkRepository
    {
        Artwork AddArtwork(string artworkName);
        void RemoveArtwork(Guid id);
        Artwork GetArtwork(Guid id, bool trackChanges);
        Artwork FindDuplicateArtwork(string artworkName, bool trackChanges);
        void RateArtwork(Artwork artwork, int value);
        IEnumerable<Artwork> GetAllArtworks(bool trackChanges);
    }
}
