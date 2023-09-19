using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetTags(bool trackChanges);
        Tag FindTagById(int Id, bool trackChanges);
        Tag AddTag(Tag tag);
        void RemoveTag(int id);
        Tag GetTag(string text, bool trackChanges);
        void UpdateTagUsageCount(int id, int count);
    }
}
