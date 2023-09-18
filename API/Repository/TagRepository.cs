using Contracts;
using Entities;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(ApplicationContext context) : 
            base(context) { }

        public Tag AddTag(Tag tag)
        {
            Create(tag);

            return tag;
        }

        public Tag GetTag(string text, bool trackChanges) =>
            FindByCondition(t => t.Value.Equals(text), trackChanges).FirstOrDefault();

        public Tag FindTagById(int Id, bool trackChanges) =>
            FindByCondition(t => t.Id.Equals(Id), trackChanges).FirstOrDefault();

        public IEnumerable<Tag> GetTags(bool trackChanges) =>
            FindAll(trackChanges)
            .OrderBy(t => t.Id)
            .ToList();

        public void UpdateTagUsageCount(int id, int count)
        {
            var tag = FindByCondition(tag => tag.Id == id, true).FirstOrDefault();

            tag.Count = count;
        }

        public void RemoveTag(int id)
        {
            var tag = FindTagById(id, false);

            Delete(tag);
        }
    }
}
