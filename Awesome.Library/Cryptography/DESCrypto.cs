/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Awesome.Library.Cryptography {

	/// <summary>
	/// DES Cryptographic Algorithm
	/// </summary>
	public class DESCrypto : IEncryptor {

		#region " Methods "

		byte[] IEncryptor.Encrypt( string password, byte[] buffer ) {
			return DESCrypto.Encrypt( Encoding.UTF8.GetBytes( password ), buffer );
		}

		byte[] IEncryptor.Encrypt( string password, string text, Encoding encodingType ) {
			return DESCrypto.Encrypt( password, text, encodingType );
		}

		void IEncryptor.Encrypt( string password, string filePath ) {
			DESCrypto.Encrypt( password, filePath );
		}

		byte[] IEncryptor.Encrypt( byte[] password, byte[] buffer ) {
			return DESCrypto.Encrypt( password, buffer );
		}

		byte[] IEncryptor.Decrypt( string password, byte[] buffer ) {
			return DESCrypto.Decrypt( Encoding.UTF8.GetBytes( password ), buffer );
		}

		string IEncryptor.Decrypt( string password, byte[] buffer, Encoding encodingType ) {
			return DESCrypto.Decrypt( password, buffer, encodingType );
		}

		byte[] IEncryptor.Decrypt( string password, string filePath, bool OverwriteFile ) {
			return DESCrypto.Decrypt( password, filePath, OverwriteFile );
		}

		byte[] IEncryptor.Decrypt( byte[] password, byte[] buffer ) {
			return DESCrypto.Decrypt( password, buffer );
		}

		#endregion

		#region " Static Methods "

		/// <summary>
		/// Encrypt a byte array
		/// </summary>
		/// <param name="password">The password to use</param>
		/// <param name="buffer">The bytes to encrypt</param>
		/// <returns>Encrypted bytes</returns>
		public static byte[] Encrypt( string password, byte[] buffer ) {
			return Encrypt( Encoding.UTF8.GetBytes( password ), buffer );
		}

		/// <summary>
		/// Encrypt a string
		/// </summary>
		/// <param name="password">The password to use</param>
		/// <param name="text">The text to encrypt</param>
		/// <param name="encodingType">The text encoding type</param>
		/// <returns>Encrypted bytes</returns>
		public static byte[] Encrypt( string password, string text, Encoding encodingType ) {
			return Encrypt( password, encodingType.GetBytes( text ) );
		}

		/// <summary>
		/// Encrypts a file
		/// </summary>
		/// <param name="password">The password to use</param>
		/// <param name="filePath">The path to the file to encrypt</param>
		public static void Encrypt( string password, string filePath ) {
			System.IO.FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read );
			byte[] bt = new byte[fs.Length];
			fs.Read( bt, 0, (int)fs.Length );
			bt = Encrypt( password, bt );
			fs.Close();
			fs = new FileStream( filePath, FileMode.Create, FileAccess.Write, FileShare.None );
			fs.Write( bt, 0, bt.Length );
			fs.Close();
		}

		/// <summary>
		/// Encrypts bytes with a certain password
		/// </summary>
		/// <param name="password">The password to use</param>
		/// <param name="input_buffer">The bytes to be entered</param>
		/// <returns>The bytes encrypted</returns>
		private static byte[] Encrypt( byte[] password, byte[] input_buffer ) {
			MemoryStream memStream = new MemoryStream();
			DES des = new DESCryptoServiceProvider();
			// init IV of algo
			byte[] des_IV = new Byte[] { 0xF9, 0xFE, 0x9C, 0x1A, 0x61, 0xD1, 0x34, 0x89 };
			// init key of algo
			des.Key = PasswordGenerator.EnsureValidPassword( password, des ); ;
			des.IV = des_IV;
			//des.GenerateIV();
			CryptoStream encStream = new CryptoStream( memStream, des.CreateEncryptor(), CryptoStreamMode.Write | CryptoStreamMode.Read );
			int block_size = (int)( des.BlockSize / 8 );
			// encyption here
			for ( int i = 0; i < input_buffer.Length; i += block_size )
				if ( i + block_size <= input_buffer.Length ) {
					encStream.Write( input_buffer, i, block_size );
				} else
					encStream.Write( input_buffer, i, input_buffer.Length - i );
			encStream.FlushFinalBlock();
			long ret_size = memStream.Length;
			encStream.Close();
			byte[] ret = new byte[ret_size];
			Array.Copy( memStream.GetBuffer(), 0, ret, 0, ret.Length );
			return ret;
		}

		/// <summary>
		/// Decrypt a byte array
		/// </summary>
		/// <param name="password">The password to use</param>
		/// <param name="buffer">The bytes to decrypt</param>
		/// <returns>The decrypted bytes</returns>
		public static byte[] Decrypt( string password, byte[] buffer ) {
			return Decrypt( Encoding.UTF8.GetBytes( password ), buffer );
		}

		/// <summary>
		/// Decrypt a string
		/// </summary>
		/// <param name="password">The password to use</param>
		/// <param name="buffer">The encrypted bytes to get the string from</param>
		/// <param name="encodingType">The encoding type of the decrypted string</param>
		/// <returns>The decrypted string</returns>
		public static string Decrypt( string password, byte[] buffer, Encoding encodingType ) {
			return encodingType.GetString( Decrypt( password, buffer ) );
		}

		/// <summary>
		/// Decrypts an RC2 encrypted file
		/// </summary>
		/// <param name="password">The password to use</param>
		/// <param name="filePath">The path to the target file</param>
		/// <param name="OverwriteFile">Should the file be overwritten with the decrypted version on disk?</param>
		/// <returns>Decrypted bytes of the file</returns>
		public static byte[] Decrypt( string password, string filePath, bool OverwriteFile ) {
			System.IO.FileStream fs = new FileStream( filePath, FileMode.Open, FileAccess.Read );
			byte[] bt = new byte[fs.Length];
			fs.Read( bt, 0, (int)fs.Length );
			bt = Decrypt( password, bt );
			fs.Close();
			if ( OverwriteFile ) {
				fs = new FileStream( filePath, FileMode.Create, FileAccess.Write, FileShare.None );
				fs.Write( bt, 0, bt.Length );
				fs.Close();
			}
			return bt;
		}

		/// <summary>
		/// Decrypts bytes using a certain password
		/// </summary>
		/// <param name="password">The password to use</param>
		/// <param name="input_buffer">The encrypted bytes to be decrypted</param>
		/// <returns>The bytes decrypted</returns>
		private static byte[] Decrypt( byte[] password, byte[] input_buffer ) {
			MemoryStream memStream = new MemoryStream();
			DES des = new DESCryptoServiceProvider();
			// init IV of algo
			byte[] des_IV = new Byte[] { 0xF9, 0xFE, 0x9C, 0x1A, 0x61, 0xD1, 0x34, 0x89 };
			// init key of algo
			des.Key = PasswordGenerator.EnsureValidPassword( password, des ); ;
			des.IV = des_IV;
			//des.GenerateIV();
			CryptoStream encStream = new CryptoStream( memStream, des.CreateDecryptor(), CryptoStreamMode.Write | CryptoStreamMode.Read );
			int block_size = (int)( des.BlockSize / 8 );
			for ( int i = 0; i < input_buffer.Length; i += block_size )
				if ( i + block_size <= input_buffer.Length ) {
					encStream.Write( input_buffer, i, block_size );
				} else
					encStream.Write( input_buffer, i, input_buffer.Length - i );
			encStream.FlushFinalBlock();
			long ret_size = memStream.Length;
			encStream.Close();
			byte[] ret = new byte[ret_size];
			Array.Copy( memStream.GetBuffer(), 0, ret, 0, ret.Length );
			memStream.Close();
			return ret;
		}

		#endregion

	}


}
