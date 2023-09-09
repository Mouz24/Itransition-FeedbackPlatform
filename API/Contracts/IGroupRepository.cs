using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IGroupRepository
    {
        Group AddGroup(Group group);
        void RemoveGroup(int id);

        Group GetGroup(int id, bool trackChanges);
        IEnumerable<Group> GetAllGroups(bool trackChanges);
    }
}
