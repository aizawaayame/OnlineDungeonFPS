using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    interface IPostResponser
    {
        void PostProcess(Protocol.NetMessageResponse message);
    }
}
