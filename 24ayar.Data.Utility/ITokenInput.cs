using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24ayar.Data.Utility
{
    public interface ITokenInput
    {
        int id { get; set; }
        string name { get; set; }
        string editLink { get; set; }
    }
}
