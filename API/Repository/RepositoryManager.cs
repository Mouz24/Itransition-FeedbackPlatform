using AutoMapper;
using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private ApplicationContext _context;
        private ITagRepository _tagRepository;
        private ICommentRepository _commentRepository;
        private IGroupRepository _groupRepository;
        private IArtworkRepository _artworkRepository;
        private ILikedReviewRepository _likedReviewRepository;
        private IReviewRepository _reviewRepository;
        private IRatedArtworkRepository _ratedArtworkRepository;
        private IReviewImageRepository _reviewImageRepository;
        private IReviewTagRepository _reviewTagRepository;
        private IMapper _mapper;

        public RepositoryManager(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IArtworkRepository Artwork
        {
            get
            {
                if (_artworkRepository == null)
                {
                    _artworkRepository = new ArtworkRepository(_context);
                }

                return _artworkRepository;
            }
        }

        public IReviewRepository Review
        {
            get
            {
                if (_reviewRepository == null)
                {
                    _reviewRepository = new ReviewRepository(_context, _mapper);
                }

                return _reviewRepository;
            }
        }

        public IGroupRepository Group
        {
            get
            {
                if (_groupRepository == null)
                {
                    _groupRepository = new GroupRepository(_context);
                }

                return _groupRepository;
            }
        }

        public ITagRepository Tag
        {
            get
            {
                if (_tagRepository == null)
                {
                    _tagRepository = new TagRepository(_context);
                }

                return _tagRepository;
            }
        }

        public ICommentRepository Comment
        {
            get
            {
                if (_commentRepository == null)
                {
                    _commentRepository = new CommentRepository(_context);
                }

                return _commentRepository;
            }
        }

        public ILikedReviewRepository LikedReview
        {
            get
            {
                if (_likedReviewRepository == null)
                {
                    _likedReviewRepository = new LikedReviewsRepository(_context);
                }

                return _likedReviewRepository;
            }
        }

        public IRatedArtworkRepository RatedArtwork
        {
            get
            {
                if (_ratedArtworkRepository == null)
                {
                    _ratedArtworkRepository = new RatedArtworksRepository(_context);
                }

                return _ratedArtworkRepository;
            }
        }

        public IReviewImageRepository ReviewImage
        {
            get
            {
                if (_reviewImageRepository == null)
                {
                    _reviewImageRepository = new ReviewImageRepository(_context);
                }

                return _reviewImageRepository;
            }
        }

        public IReviewTagRepository ReviewTag
        {
            get
            {
                if (_reviewTagRepository == null)
                {
                    _reviewTagRepository = new ReviewTagRepository(_context);
                }

                return _reviewTagRepository;
            }
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public void Save() => _context.SaveChanges();
    }
}
