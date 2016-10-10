/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace Awesome.Library.Cryptography {

	public class DSACrypto {

		private const string HashAlgorithm = "SHA1";

		public static byte[] SignHash( byte[] SHA1Digest, byte[] cspBlob ) {
			DSACryptoServiceProvider dsp = new DSACryptoServiceProvider();
			dsp.ImportCspBlob( cspBlob );
			DSASignatureFormatter DSAFormatter = new DSASignatureFormatter( dsp );
			DSAFormatter.SetHashAlgorithm( HashAlgorithm );
			return DSAFormatter.CreateSignature( SHA1Digest );
		}

		public static byte[] SignHash( byte[] SHA1Digest, DSAParameters DSAKeyInfo ) {
			DSACryptoServiceProvider dsp = new DSACryptoServiceProvider();
			dsp.ImportParameters( DSAKeyInfo );
			DSASignatureFormatter DSAFormatter = new DSASignatureFormatter( dsp );
			DSAFormatter.SetHashAlgorithm( HashAlgorithm );
			return DSAFormatter.CreateSignature( SHA1Digest );
		}

		public static bool VerifyHash( byte[] SHA1Digest, byte[] Signature, byte[] cspBlob ) {
			try {
				DSACryptoServiceProvider dsp = new DSACryptoServiceProvider();
				dsp.ImportCspBlob( cspBlob );
				DSASignatureDeformatter DSADeformatter = new DSASignatureDeformatter( dsp );
				DSADeformatter.SetHashAlgorithm( HashAlgorithm );
				return DSADeformatter.VerifySignature( SHA1Digest, Signature );
			} catch ( CryptographicException e ) {
				Debug.WriteLine( e );
				return false;
			}
		}

		public static bool VerifyHash( byte[] SHA1Digest, byte[] Signature, DSAParameters DSAKeyInfo ) {
			try {
				DSACryptoServiceProvider dsp = new DSACryptoServiceProvider();
				dsp.ImportParameters( DSAKeyInfo );
				DSASignatureDeformatter DSADeformatter = new DSASignatureDeformatter( dsp );
				DSADeformatter.SetHashAlgorithm( HashAlgorithm );
				return DSADeformatter.VerifySignature( SHA1Digest, Signature );
			} catch ( CryptographicException e ) {
				Debug.WriteLine( e );
				return false;
			}
		}


	}

}
