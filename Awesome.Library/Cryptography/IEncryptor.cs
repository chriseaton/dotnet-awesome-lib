/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace Awesome.Library.Cryptography {

	public interface IEncryptor {

		byte[] Encrypt( byte[] password, byte[] buffer );

		byte[] Encrypt( string password, byte[] buffer );

		byte[] Encrypt( string password, string text, Encoding encodingType );

		void Encrypt( string password, string filePath );

		byte[] Decrypt( byte[] password, byte[] buffer );

		byte[] Decrypt( string password, byte[] buffer );

		string Decrypt( string password, byte[] buffer, Encoding encodingType );

		byte[] Decrypt( string password, string filePath, bool OverwriteFile );

	}

}
