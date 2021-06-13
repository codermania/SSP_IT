namespace SSP
{
    public static class Crc16
    {
        private const ushort POLYNOMIAL = 0xC003;//0x4129;
        private static readonly ushort[] Table = new ushort[256];

        public static ushort ComputeChecksum(byte[] bytes)
        {
            ushort crc = 0xFFFF;

            foreach (var t in bytes)
            {
                var index = (byte)(crc ^ t);
                crc = (ushort)((crc >> 8) ^ Table[index]);
            }

            return crc;
        }

        static Crc16()
        {
            for (ushort i = 0; i < Table.Length; ++i)
            {
                ushort value = 0;
                var temp = i;

                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ POLYNOMIAL);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }

                Table[i] = value;
            }
        }
    }
}
