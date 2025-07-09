using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatcher.Tests.Examples
{


    public class UserUpdated : IEvent
    {
        public string? UserName { get; set; }
    }
}
