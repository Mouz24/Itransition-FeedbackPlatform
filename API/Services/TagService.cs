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
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public  TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public Tag AddTag(Tag tag)
        {
            return _tagRepository.AddTag(tag);
        }

        public Tag FindDuplicateTag(string text, bool trackChanges)
        {
            return _tagRepository.FindDuplicateTag(text, trackChanges);
        }

        public Tag FindTagById(int Id, bool trackChanges)
        {
            return _tagRepository.FindTagById(Id, trackChanges);
        }

        public IEnumerable<Tag> GetTags(bool trackChanges)
        {
            return _tagRepository.GetTags(trackChanges);
        }

        public void RemoveTag(int id)
        {
            _tagRepository.RemoveTag(id);
        }
    }
}
