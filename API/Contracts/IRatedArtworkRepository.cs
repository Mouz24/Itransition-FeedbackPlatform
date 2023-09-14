using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRatedArtworkRepository
    {
        void AddRatedArtwork(Guid userId, Guid artworkId, int rateValue);
        int GetAverageRate(Guid artworkId, int rateValue);
        void RemoveUserRate(Guid userId, Guid artworkId);
        bool IsRatedByUser(Guid userId, Guid artworkId);
    }
}
