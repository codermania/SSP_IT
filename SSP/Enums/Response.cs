namespace SSP.Enums
{
    public enum Response
    {
        Ok = 0xF0,
        CommandNotKnown = 0xF2,
        WrongNoParameters = 0xF3,
        Parameters = 0xF4,
        CommandCanNotBeProcessed = 0xF5,
        SoftwareError = 0xF6,
        Fail = 0xF8,
        KeyNotSet = 0xFA
    }
}
