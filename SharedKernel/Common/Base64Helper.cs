using System;

namespace SharedKernel.Common
{
    public static class Base64Helper
    {
        public static int DataLength(string encodedString)
        {
            int padding;

            if (encodedString.EndsWith("=="))
                padding = 2;
            else if (encodedString.EndsWith("="))
                padding = 1;
            else
                padding = 0;

            return (int) (Math.Ceiling((decimal) encodedString.Length / 4) * 3) - padding;
        }
    }
}