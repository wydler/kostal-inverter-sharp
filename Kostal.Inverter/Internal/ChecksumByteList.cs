using System.Collections.Generic;

namespace Kostal.Inverter.Internal
{
    internal class ChecksumByteList : List<byte>
    {
        public void AddChecksum()
        {
            Add(GetChecksum());
        }

        public byte GetChecksum()
        {
            byte sum = 0;

            unchecked // Let overflow occur without exceptions
            {
                foreach (byte b in this)
                {
                    sum -= b;
                }
            }

            return sum;
        }
    }
}
