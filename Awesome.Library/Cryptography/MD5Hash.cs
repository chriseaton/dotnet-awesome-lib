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

	/// <summary>
	/// One way encryption algorithm
	/// </summary>
	public static class MD5Hash {

		/// <summary>
		/// Get MD5 hash from buffer of bytes
		/// </summary>
		/// <param name="input_buffer">Input buffer</param>
		/// <returns>16-byte array of hash</returns>
		public static byte[] GetMD5Hash(byte[] input_buffer) {
			// create implementation of MD5
			MD5 md5 = new MD5CryptoServiceProvider();
			// get hash
			return md5.ComputeHash(input_buffer);
		}

		/// <summary>
		/// Gets the MD5 hash from a string
		/// </summary>
		/// <param name="input_buffer">The string to use</param>
		/// <returns>String representation of the MD5 hash</returns>
		public static string GetMD5Hash(string input_buffer) {
			// create implementation of MD5
			MD5 md5 = new MD5CryptoServiceProvider();
			// get hash
			return BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.Unicode.GetBytes(input_buffer))).Replace("-","").ToLower();;
		}

		/// <summary>
		/// Open a file and get it's MD5 hash
		/// </summary>
		/// <param name="file_name">File name (full path)</param>
		/// <returns>String representation of the MD5 hash</returns>
		public static string GetMD5HashFromFile(string file_name) {
			System.IO.FileStream file = new System.IO.FileStream( file_name, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite);
			System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			return BitConverter.ToString(retVal).Replace("-","").ToLower();
		}

	}

}
