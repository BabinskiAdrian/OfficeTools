using System;
using System.Collections.Generic;
using System.Text;

namespace OfficeTool.Core.Shered;

public class ChecksumHelper
{
    /// <summary>
    /// Calculating check sume - RFC 1071
    /// </summary>
    public static string CalculateScalanceChecksum(string textToCalculate)
    {
        long sum = 0;

        // Download bytes in ASCII
        byte[] bytes = Encoding.ASCII.GetBytes(textToCalculate);

        // Takeing 2 words at once - Little Endian
        for (int i = 0; i < bytes.Length; i += 2)
        {
            int word = bytes[i];
            if (i + 1 < bytes.Length)
            {
                word += (bytes[i + 1] << 8);
            }
            sum += word;
        }

        // fold carry
        while ((sum >> 16) != 0)
        {
            sum = (sum & 0xFFFF) + (sum >> 16);
        }

        // One's complement
        long checksum = (~sum) & 0xFFFF;

        // return as 4-digit hexadecimal string (small letters)
        return checksum.ToString("x4");
    }
}
