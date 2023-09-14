using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IRatedArtworkService
    {
        void AddRatedArtwork(Guid userId, Guid artworkId, int rateValue);
        int GetAverageRate(Guid artworkId, int rateValue);
        void RemoveUserRate(Guid userId, Guid artworkId);
        bool IsRatedByUser(Guid userId, Guid artworkId);
    }
}
