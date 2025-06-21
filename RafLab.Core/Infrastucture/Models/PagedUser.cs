using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafLab.Core.Infrastucture.Models
{
    public class PagedUser
    {
        public int Page { get; set; }
        public int Per_Page { get; set; }
        public int Total { get; set; }
        public int Total_Pages { get; set; }
        public List<User> Data { get; set; } = new();
        public SupportInfo Support { get; set; } = new();
    }
}
