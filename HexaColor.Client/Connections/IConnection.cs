using HexaColor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.Connections
{
    public interface IConnection
    {
        Task Connect();
        Task Send(GameChange message);
        void StartListening();
    }
}
