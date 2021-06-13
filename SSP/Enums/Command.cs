namespace SSP.Enums
{
    public enum Command
    {
        Sync = 17,
        Reset = 1,
        HostProtocolVersion = 6,
        GetSerialNumber = 12,
        Enable = 10,
        Disable = 9,
        Poll = 7,
        GetFirmwareVersion = 32,
        GetDataSetVersion = 33,
        SetInhibits = 2,
        DisplayOn = 3,
        DisplayOff = 4,
        Reject = 8,
        UnitData = 13,
        ChannelValueData = 14,
        ChannelSecurityData = 15,
        LastRejectCode = 23,
        ConfigureBezel = 84,
        PollWithAck = 86,
        EventAck = 87,
        SetupRequest = 5
    }
}
