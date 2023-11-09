using AutoMapper;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Service.IService;

namespace FeedbackPlatform.Extensions.ModelsManipulationLogics
{
    public class TagExtension
    {
        private readonly IMapper _mapper;
        private readonly IServiceManager _serviceManager;

        public TagExtension(IMapper mapper, IServiceManager serviceManager)
        {
            _mapper = mapper;
            _serviceManager = serviceManager;
        }

        public virtual async Task AddTags(IEnumerable<string> tags, Review review)
        {
            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    var duplicateTag = _serviceManager.Tag.GetTag(tag, true);
                    if (duplicateTag == null)
                    {
                        var tagToAdd = _mapper.Map<Tag>(tag);

                        var addedTag = _serviceManager.Tag.AddTag(tagToAdd);

                        await _serviceManager.SaveAsync();
                        _serviceManager.ReviewTag.AddReviewTag(review.Id, addedTag.Id);
                        _serviceManager.Tag.UpdateTagUsageCount(addedTag.Id, 1);
                    }
                    else
                    {
                        _serviceManager.ReviewTag.AddReviewTag(review.Id, duplicateTag.Id);

                        await _serviceManager.SaveAsync();

                        var tagCount = _serviceManager.ReviewTag.CountTagUsage(duplicateTag.Id);

                        _serviceManager.Tag.UpdateTagUsageCount(duplicateTag.Id, tagCount);
                    }
                }
            }
        }

        public virtual async Task EditReviewTags(IEnumerable<string> tags, Guid reviewId, ApplicationContext _context)
        {
            if (tags != null)
            {
                var newTags = _serviceManager.ReviewTag.GetNewTags(reviewId, tags);
                foreach (var tag in newTags)
                {
                    var duplicateTag = _serviceManager.Tag.GetTag(tag, true);
                    if (duplicateTag == null)
                    {
                        var tagToAdd = _mapper.Map<Tag>(tag);

                        var addedTag = _serviceManager.Tag.AddTag(tagToAdd);

                        await _serviceManager.SaveAsync();
                        _serviceManager.ReviewTag.AddReviewTag(reviewId, addedTag.Id);
                        _serviceManager.Tag.UpdateTagUsageCount(addedTag.Id, 1);
                    }
                    else
                    {
                        _serviceManager.ReviewTag.AddReviewTag(reviewId, duplicateTag.Id);

                        await _serviceManager.SaveAsync();

                        var tagCount = _serviceManager.ReviewTag.CountTagUsage(duplicateTag.Id);

                        _serviceManager.Tag.UpdateTagUsageCount(duplicateTag.Id, tagCount);
                    }
                }

                var removedTags = _serviceManager.ReviewTag.GetRemovedTags(reviewId, tags);
                foreach (var tag in removedTags)
                {
                    var removedTag = _serviceManager.Tag.GetTag(tag, true);

                    _context.Entry(removedTag).State = EntityState.Detached;

                    _serviceManager.ReviewTag.RemoveTagFromReview(reviewId, removedTag.Id);

                    await _serviceManager.SaveAsync();

                    var tagCount = _serviceManager.ReviewTag.CountTagUsage(removedTag.Id);

                    _serviceManager.Tag.UpdateTagUsageCount(removedTag.Id, tagCount);
                }
            }
            else
            {
                _serviceManager.ReviewTag.RemoveTagsFromReview(reviewId);
            }
        }
    }
}
