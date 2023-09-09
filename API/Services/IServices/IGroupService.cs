using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IGroupService
    {
        Group AddGroup(Group group);
        void RemoveGroup(int id);
        Group GetGroup(int id, bool trackChanges);
        IEnumerable<Group> GetAllGroups(bool trackChanges);
    }
}
