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
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public Group AddGroup(Group group)
        {
            return _groupRepository.AddGroup(group);
        }

        public IEnumerable<Group> GetAllGroups(bool trackChanges)
        {
            return _groupRepository.GetAllGroups(trackChanges);
        }

        public Group GetGroup(int id, bool trackChanges)
        {
            return _groupRepository.GetGroup(id, trackChanges);
        }

        public void RemoveGroup(int id)
        {
            _groupRepository.RemoveGroup(id);
        }
    }
}
