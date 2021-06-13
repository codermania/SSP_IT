using SSP.Enums;

namespace SSP.Models
{
    public class DeviceCommand
    {
        public Command Command { get; set; }
        public byte[] Args { get; set; }
    }
}
