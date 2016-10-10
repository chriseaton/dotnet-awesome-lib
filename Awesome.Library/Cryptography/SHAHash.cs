/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Awesome.Library.Cryptography {

	public enum SHAType { SHA1, SHA256, SHA384, SHA512 };

	/// <summary>
	/// One way encryption algorithm
	/// </summary>
	public static class SHAHash {
	
		/// <summary>
		/// Get SHA1 hash from buffer of bytes
		/// </summary>
		/// <param name="input_buffer">Input buffer</param>
		/// <returns>16-byte array of hash</returns>
		public static byte[] GetSHAHash( byte[] input_buffer, SHAType shaType ) {
			switch ( shaType ) {
				case SHAType.SHA1:
					SHA1 sha1 = new SHA1CryptoServiceProvider();
					return sha1.ComputeHash( input_buffer );
				case SHAType.SHA256:
					System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
					return sha256.ComputeHash( input_buffer );
				case SHAType.SHA384:
					System.Security.Cryptography.SHA384 sha384 = System.Security.Cryptography.SHA384.Create();
					return sha384.ComputeHash( input_buffer );
				case SHAType.SHA512:
					System.Security.Cryptography.SHA512 sha512 = System.Security.Cryptography.SHA512.Create();
					return sha512.ComputeHash( input_buffer );
			}
			return null;
		}

		/// <summary>
		/// Get SHA1 hash from a string
		/// </summary>
		/// <param name="input_buffer">The string to use</param>
		/// <returns>String representation of the SHA1 hash</returns>
		public static string GetSHAHash( string input_buffer, SHAType shaType ) {
			return GetSHAHash( input_buffer, shaType, Encoding.UTF8 );
		}

		/// <summary>
		/// Get SHA1 hash from a string
		/// </summary>
		/// <param name="input_buffer">The string to use</param>
		/// <returns>String representation of the SHA1 hash</returns>
		public static string GetSHAHash( string input_buffer, SHAType shaType, Encoding strEncoding ) {
			switch ( shaType ) {
				case SHAType.SHA1:
					SHA1 sha1 = new SHA1CryptoServiceProvider();
					return BitConverter.ToString( sha1.ComputeHash( strEncoding.GetBytes( input_buffer ) ) ).Replace( "-", "" ).ToLower();
				case SHAType.SHA256:
					SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
					return BitConverter.ToString( sha256.ComputeHash( strEncoding.GetBytes( input_buffer ) ) ).Replace( "-", "" ).ToLower();
				case SHAType.SHA384:
					SHA384 sha384 = System.Security.Cryptography.SHA384.Create();
					return BitConverter.ToString( sha384.ComputeHash( strEncoding.GetBytes( input_buffer ) ) ).Replace( "-", "" ).ToLower();
				case SHAType.SHA512:
					SHA512 sha512 = System.Security.Cryptography.SHA512.Create();
					return BitConverter.ToString( sha512.ComputeHash( strEncoding.GetBytes( input_buffer ) ) ).Replace( "-", "" ).ToLower();
			}
			return null;
		}

		/// <summary>
		/// Open file and get it's SHA1 hash
		/// </summary>
		/// <param name="file_name">File name (full path)</param>
		/// <returns>String representation of the SHA1 hash</returns>
		public static string GetSHAHashFromFile( string file_name, SHAType shaType ) {
			System.IO.FileStream file = new System.IO.FileStream( file_name, System.IO.FileMode.Open, System.IO.FileAccess.Read );
			byte[] retVal = null;
			switch ( shaType ) {
				case SHAType.SHA1:
					System.Security.Cryptography.SHA1 sha1obj = new System.Security.Cryptography.SHA1CryptoServiceProvider();
					retVal = sha1obj.ComputeHash( file );
					break;
				case SHAType.SHA256:
					System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
					retVal = sha256.ComputeHash( file );
					break;
				case SHAType.SHA384:
					System.Security.Cryptography.SHA384 sha384 = System.Security.Cryptography.SHA384.Create();
					retVal = sha384.ComputeHash( file );
					break;
				case SHAType.SHA512:
					System.Security.Cryptography.SHA512 sha512 = System.Security.Cryptography.SHA512.Create();
					retVal = sha512.ComputeHash( file );
					break;
			}
			file.Close();
			return BitConverter.ToString( retVal ).Replace( "-", "" ).ToLower();
		}
	}

}
