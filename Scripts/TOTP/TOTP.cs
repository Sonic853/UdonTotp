using System;
using System.Text.RegularExpressions;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon
{
    public class TOTP : UdonSharpBehaviour
    {
        private string base_32_chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private string[] Base32Chars = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "2", "3", "4", "5", "6", "7" };
        [Header("编译后密钥")]
        [SerializeField] private string secret = "GEZDGNBVGY3TQOJQGEZDGNBVGY3TQOJQ";
        /// <summary>
        /// interval in seconds
        /// </summary>
        [Header("时间间隔")]
        [SerializeField] public int period = 30;
        /// <summary>
        /// The number of digits in the TOTP code
        /// </summary>
        [Header("位数")]
        [Range(1, 9)]
        [SerializeField] public int digits = 6;
        /// <summary>
        /// tolerance for clock skew
        /// </summary>
        [Header("容错倍数")]
        [SerializeField] private int tolerance = 1;
        /// <summary>
        /// mode: 0 = SHA1, 1 = SHA256, 2 = SHA512
        /// </summary>
        [Header("模式")]
        [Range(0, 2)]
        [SerializeField] private int mode = 0;
        /// <summary>
        /// 1970/01/01 00:00:00 UTC
        /// </summary>
        const long unixEpochTicks = 621355968000000000L;
        /// <summary>
        /// 100ns ticks per second
        /// </summary>
        const long ticksToSeconds = 10000000L;
        public void SetSecret(string _secret) => secret = _secret;
        public string ComputeTotp()
        {
            return ComputeTotpWithTime(Networking.GetNetworkDateTime().ToUniversalTime());
        }
        public string ComputeTotpWithTime(DateTime time)
        {
            long unixTimestamp = (time.Ticks - unixEpochTicks) / ticksToSeconds;
            var counter = unixTimestamp / (long)period;
            byte[] hmacComputedHash;
            switch (mode)
            {
                case 0:
                    {
                        hmacComputedHash = UdonHashLib_HMAC.HMAC_SHA1(
                            GetBigEndianBytes(counter),
                            Base32DecodeByte(secret)
                        );
                    }
                    break;
                case 1:
                    {
                        hmacComputedHash = UdonHashLib_HMAC.HMAC_SHA256(
                            GetBigEndianBytes(counter),
                            Base32DecodeByte(secret)
                        );
                    }
                    break;
                case 2:
                    {
                        hmacComputedHash = UdonHashLib_HMAC.HMAC_SHA512(
                            GetBigEndianBytes(counter),
                            Base32DecodeByte(secret)
                        );
                    }
                    break;
                default:
                    return null;
            }
            int offset = hmacComputedHash[hmacComputedHash.Length - 1] & 0xf;
            long binary =
                ((hmacComputedHash[offset] & 0x7f) << 24) |
                ((hmacComputedHash[offset + 1] & 0xff) << 16) |
                ((hmacComputedHash[offset + 2] & 0xff) << 8) |
                (hmacComputedHash[offset + 3] & 0xff) % 1000000;
            return Digits(binary, digits);
        }
        public bool VerifyTotp(string code)
        {
            if (code == null || code.Length == 0 || code.Length != digits)
            {
                return false;
            }
            for (int i = -tolerance; i <= tolerance; i++)
            {
                if (ComputeTotpWithTime(Networking.GetNetworkDateTime().ToUniversalTime().AddSeconds(i * period)) == code)
                {
                    return true;
                }
            }
            return false;
        }
        byte[] GetUTF8Bytes(string input)
        {
            char[] characters = input.ToCharArray();
            byte[] buffer = new byte[characters.Length * 4];

            int writeIndex = 0;
            for (int i = 0; i < characters.Length; i++)
            {
                uint character = characters[i];

                if (character < 0x80)
                {
                    buffer[writeIndex++] = (byte)character;
                }
                else if (character < 0x800)
                {
                    buffer[writeIndex++] = (byte)(0b11000000 | ((character >> 6) & 0b11111));
                    buffer[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
                }
                else if (character < 0x10000)
                {
                    buffer[writeIndex++] = (byte)(0b11100000 | ((character >> 12) & 0b1111));
                    buffer[writeIndex++] = (byte)(0b10000000 | ((character >> 6) & 0b111111));
                    buffer[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
                }
                else
                {
                    buffer[writeIndex++] = (byte)(0b11110000 | ((character >> 18) & 0b111));
                    buffer[writeIndex++] = (byte)(0b10000000 | ((character >> 12) & 0b111111));
                    buffer[writeIndex++] = (byte)(0b10000000 | ((character >> 6) & 0b111111));
                    buffer[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
                }
            }

            // We do this to truncate off the end of the array
            // This would be a lot easier with Array.Resize, but Udon once again does not allow access to it.
            byte[] output = new byte[writeIndex];

            for (int i = 0; i < writeIndex; i++)
                output[i] = buffer[i];

            return output;
        }
        byte[] GetBigEndianBytes(long input)
        {
            byte[] output = new byte[8];
            for (int i = 7; i >= 0; i--)
            {
                output[i] = (byte)(input & 0xff);
                input >>= 8;
            }
            return output;
        }
        string Digits(long input, int digitCount)
        {
            var truncatedValue = ((int)input % (int)Math.Pow(10, digitCount));
            return truncatedValue.ToString().PadLeft(digitCount, '0');
        }
        string Base32Decode(string input)
        {
            input = input.ToUpper();
            int l = input.Length;
            int n = 0;
            int j = 0;
            string binary = "";
            for (int i = 0; i < l; i++)
            {
                n = n << 5;
                n = n + base_32_chars.IndexOf(input[i]);
                j = j + 5;
                if (j >= 8)
                {
                    j = j - 8;
                    binary += (char)((n & (0xFF << j)) >> j);
                }
            }
            return binary;
        }
        byte[] Base32DecodeByte(string input)
        {
            input = input.ToUpper();
            int unpadded_data_length = input.Length;
            for (int i = 1; i < (Math.Min(6, input.Length)) + 1; i++)
            {
                if (input[input.Length - i] != '=') break;
                unpadded_data_length -= 1;
            }

            int output_length = unpadded_data_length * 5 / 8;
            byte[] _output = new byte[output_length];
            char[] bytes = input.ToCharArray();
            int index = 0;
            for (int bitIndex = 0; bitIndex < input.Length * 5; bitIndex += 8)
            {
                int dualbyte = base_32_chars.IndexOf(bytes[bitIndex / 5]) << 10;
                if (bitIndex / 5 + 1 < bytes.Length)
                    dualbyte |= base_32_chars.IndexOf(bytes[bitIndex / 5 + 1]) << 5;
                if (bitIndex / 5 + 2 < bytes.Length)
                    dualbyte |= base_32_chars.IndexOf(bytes[bitIndex / 5 + 2]);
                dualbyte = 0xff & (dualbyte >> (15 - bitIndex % 5 - 8));
                _output[index] = (byte)(dualbyte);
                index++;
            }

            return _output;
        }
    }
}
