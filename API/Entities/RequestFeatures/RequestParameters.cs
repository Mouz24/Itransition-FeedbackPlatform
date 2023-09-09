using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public class RequestParameters
    {
        public int PageNumber { get; set; } = 1;
        private const int _pageSize = 5;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
        }
    }
}
