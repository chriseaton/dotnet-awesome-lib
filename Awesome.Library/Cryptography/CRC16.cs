/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Awesome.Library.Cryptography {

    public static class CRC16 {

        private const ushort polynomial = 0xA001;

        /// <summary>
        /// Get a CRC16 hash from bytes
        /// </summary>
        /// <param name="input_buffer">Input buffer</param>
        /// <returns>16-byte array of hash</returns>
        public static byte[] GetCRC16( byte[] input_buffer ) {
            // get hash
            return ComputeChecksumBytes(input_buffer);
        }

        /// <summary>
        /// Get a CRC16 hash from string
        /// </summary>
        /// <param name="input_buffer">The string to use</param>
        /// <returns>String representation of the MD5 hash</returns>
        public static string GetCRC16( string input_buffer ) {
            // get hash
            return BitConverter.ToString(ComputeChecksumBytes(System.Text.Encoding.Unicode.GetBytes(input_buffer))).Replace("-", "").ToLower(); ;
        }

        private static ushort[] GetTable() {
            ushort[] table = new ushort[256];
            ushort value;
            ushort temp;
            for ( ushort i = 0; i < table.Length; i++ ) {
                value = 0;
                temp = i;
                for ( byte j = 0; j < 8; j++ ) {
                    if ( ( ( value ^ temp ) & 0x0001 ) != 0 ) {
                        value = (ushort)( ( value >> 1 ) ^ polynomial );
                    } else {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
            return table;
        }

        private static ushort ComputeChecksum( byte[] bytes ) {
            ushort[] table = GetTable();
            ushort crc = 0;
            for ( int i = 0; i < bytes.Length; i++ ) {
                byte index = (byte)( crc ^ bytes[i] );
                crc = (ushort)( ( crc >> 8 ) ^ table[index] );
            }
            return crc;
        }

        private static byte[] ComputeChecksumBytes( byte[] bytes ) {
            ushort crc = ComputeChecksum(bytes);
            return new byte[] { (byte)( crc >> 8 ), (byte)( crc & 0x00ff ) };
        }

    }
}
