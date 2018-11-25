using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaColor.Client.Connections
{
    public interface IConnection
    {

        void Open();
        void Close();
        void Send(Model.GameChange gameChangeEvent);
        Model.GameUpdate Receive();
        bool IsOpen();


    }
}
