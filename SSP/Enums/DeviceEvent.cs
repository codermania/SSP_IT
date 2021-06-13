namespace SSP.Enums
{
    public enum DeviceEvent : byte
    {
        RefillNoteCredit = 158,
        TicketPrinting = 165,
        TickedPrinted = 166,
        TickedInBezelAtStartup = 167,
        TickedPrintingError = 168,
        TickedInBezel = 173,
        PrintHalted = 174,
        PrintedToCashBox = 175,
        ChannelDisabled = 181,
        Initialising = 182,
        Stacking = 204,
        NoteClearedFromFront = 225,
        NoteClearedIntoCashBox = 226,
        FraudAttempt = 230,
        StackerFull = 231,
        Disabled = 232,
        UnsafeJam = 233,
        Stacked = 235,
        Rejected = 236,
        Rejecting = 237,
        NoteCredit = 238,
        Read = 239,
        SlaveReset = 241,
    }
}
