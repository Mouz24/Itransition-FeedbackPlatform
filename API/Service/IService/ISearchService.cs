using Entities.DTOs;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface ISearchService
    {
        Task<IEnumerable<ReviewDTO>> Search(string wordToSearch);
    }
}
