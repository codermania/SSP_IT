using System;
using SSP.Enums;
using SSP.Events;

namespace SSP.Interfaces
{
    public interface ISSPDevice
    {
        event EventHandler<DataReceivedEventArgs> DataReceived;

        void Disconnect();

        void SendCommand(Command command, byte[] args = null);
    }
}
