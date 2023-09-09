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
    public class GroupRepository : RepositoryBase<Group>, IGroupRepository
    {
        public GroupRepository(ApplicationContext applicationContext) :
            base(applicationContext) { }

        public Group AddGroup(Group group)
        {
            Create(group);

            return group;
        }

        public IEnumerable<Group> GetAllGroups(bool trackChanges) =>
            FindAll(trackChanges)
            .OrderBy(group => group.Name)
            .ToList();

        public Group GetGroup(int id, bool trackChanges) =>
            FindByCondition(g => g.Id.Equals(id), trackChanges).FirstOrDefault();

        public void RemoveGroup(int id)
        {
            var group = GetGroup(id, false);

            Delete(group);
        }
    }
}
