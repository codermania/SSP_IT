using System;
using System.Collections.Generic;
using System.Text;
using SSP.Enums;

namespace SSP.Events
{
    public class DataReceivedEventArgs
    {
        public Response Response { get; set; }

        public DeviceEvent? Event { get; set; }

        public byte[] ResponseBytes { get; set; }

        public Command Command { get; set; }
    }
}
