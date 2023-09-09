using Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly IRepositoryManager _repositoryManager;
        private IArtworkService _artworkService;
        private ICommentService _commentService;
        private ITagService _tagService;
        private IReviewService _reviewService;
        private IGroupService _groupService;
        private IImageCloudService _imageCloudService;
        private ILikedReviewService _likedReviewService;
        private IRatedArtworkService _ratedArtworkService;
        private ISearchService _searchService;
        private IReviewImageService _reviewImageService;
        private readonly IElasticClient _client;
        private readonly IConfiguration _configuration;

        public ServiceManager(IRepositoryManager repositoryManager, IConfiguration configuration, IElasticClient client)
        {
            _repositoryManager = repositoryManager;
            _configuration = configuration;
            _client = client;
        }

        public IGroupService Group
        {
            get
            {
                if (_groupService == null)
                {
                    _groupService = new GroupService(_repositoryManager.Group);
                }

                return _groupService;
            }
        }

        public ICommentService Comment
        {
            get
            {
                if (_commentService == null)
                {
                    _commentService = new CommentService(_repositoryManager.Comment);
                }

                return _commentService;
            }
        }

        public ITagService Tag
        {
            get
            {
                if (_tagService == null)
                {
                    _tagService = new TagService(_repositoryManager.Tag);
                }

                return _tagService;
            }
        }

        public IArtworkService Artwork
        {
            get
            {
                if (_artworkService == null)
                {
                    _artworkService = new ArtworkService(_repositoryManager.Artwork);
                }

                return _artworkService;
            }
        }

        public IReviewService Review
        {
            get
            {
                if (_reviewService == null)
                {
                    _reviewService = new ReviewService(_repositoryManager.Review);
                }

                return _reviewService;
            }
        }

        public IImageCloudService ImageCloud
        {
            get
            {
                if (_imageCloudService == null)
                {
                    _imageCloudService = new S3ImageUploadService(
                        _configuration.GetSection("AWS").GetSection("bucketName").Value,
                        _configuration.GetSection("AWS").GetSection("accessKey").Value,
                        _configuration.GetSection("AWS").GetSection("secretKey").Value
                        );
                }

                return _imageCloudService;
            }
        }

        public ILikedReviewService LikedReview
        {
            get
            {
                if (_likedReviewService == null)
                {
                    _likedReviewService = new LikedReviewsService(_repositoryManager.LikedReview);
                }

                return _likedReviewService;
            }
        }

        public ISearchService Search
        {
            get
            {
                if (_searchService == null)
                {
                    _searchService = new SearchService(_client);
                }

                return _searchService;
            }
        }

        public IRatedArtworkService RatedArtwork
        {
            get
            {
                if (_ratedArtworkService == null)
                {
                    _ratedArtworkService = new RatedArtworksService(_repositoryManager.RatedArtwork);
                }

                return _ratedArtworkService;
            }
        }

        public IReviewImageService ReviewImage
        {
            get
            {
                if (_reviewImageService == null)
                {
                    _reviewImageService = new ReviewImageService(_repositoryManager.ReviewImage);
                }

                return _reviewImageService;
            }
        }

        public async Task SaveAsync()
        {
            await _repositoryManager.SaveAsync();
        }
    }
}
