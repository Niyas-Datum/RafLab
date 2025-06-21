using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RafLab.Shared.Dto
{
    public class ApiSettings
    {
        public string BaseUrl { get; set; } = "https://reqres.in/api/";
        public string ApiKey { get; set; } = null!;

    }
}
