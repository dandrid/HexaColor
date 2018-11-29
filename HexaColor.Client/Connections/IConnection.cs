using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.Connections
{
    public interface IConnection
    {
        public async Task Connect();
        public async Task Send(GameChange message);
        public async void StartListening();
    }
}
