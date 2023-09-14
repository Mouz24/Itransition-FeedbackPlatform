using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IServiceManager
    {
        IGroupService Group { get; }
        ICommentService Comment { get; }
        ITagService Tag { get; }
        IArtworkService Artwork { get; }
        IReviewService Review { get; }
        IImageCloudService ImageCloud { get; }
        ILikedReviewService LikedReview { get; }
        ISearchService Search { get; }
        IRatedArtworkService RatedArtwork { get; }
        IReviewImageService ReviewImage { get; }
        Task SaveAsync();
        void Save();
    }
}
