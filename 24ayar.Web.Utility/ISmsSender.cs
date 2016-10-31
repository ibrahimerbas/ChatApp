using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24ayar.Web.Utility
{
    public interface ISmsSender
    {
        object SendSMS(string gsmNumber, string Message);
        object SendSMS(string[] gsmNumbers, string Message);
        object SendSMS(string[] gsmNumbers, string[] Message);
    }
}
