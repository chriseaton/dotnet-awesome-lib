/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using Awesome.Library.Utilities;

namespace Awesome.Library.Cryptography {

	public class PasswordGenerator {

		public const int DefaultMinLength = 8;
		public const int DefaultMaxLength = 16;

		private char[] m_AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-*!#_+=?".ToCharArray();

		#region " Properties "

		public string[] CustomWords { get; set; }

		public char[] AllowedCharacters {
			get { return m_AllowedCharacters; }
			set { m_AllowedCharacters = value; }
		}

		public bool AllowRepetition { get; set; }

		#endregion

		#region " Constructor(s) "

		public PasswordGenerator() { }

		public PasswordGenerator( char[] allowedChars, string[] customWords ) {
			this.CustomWords = customWords;
			this.AllowedCharacters = allowedChars;
		}

		#endregion

		#region " Methods "

		public string Generate() {
			return Generate( DefaultMinLength, DefaultMaxLength );
		}

		public string Generate( int minLength, int maxLength ) {
			if ( minLength > maxLength ) {
				throw new ArgumentOutOfRangeException( "minLength", "minLength cannot be greater than maxLength." );
			}
			if ( minLength <= 0 ) {
				throw new ArgumentOutOfRangeException( "minLength", "minLength must be greater than 0." );
			}
			if ( this.AllowedCharacters == null || ( this.AllowedCharacters != null && this.AllowedCharacters.Length == 0 ) ) {
				throw new ArgumentNullException( "", "AllowedCharacters must contain at least 1 item." );
			}
			StringBuilder pw = new StringBuilder();
			Random r = new Random();
			List<string> words = new List<string>();
			int lenRemaining = r.Next( minLength, maxLength );
			if ( this.CustomWords != null && this.CustomWords.Length > 0 ) {
				string[] cw = this.CustomWords.Randomize( r );
				for ( int x = 0; x < cw.Length; x++ ) {
					if ( cw[x].Length < lenRemaining ) {
						words.Add( cw[x] );
						lenRemaining -= cw[x].Length;
						if ( r.Next( 0, 2 ) == 2 ) {
							break;
						}
					}
				}
			}
			char? lastChar = null;
			for ( int x = 0; x < lenRemaining; x++ ) {
				int index = r.Next( 0, this.AllowedCharacters.Length );
				char c = this.AllowedCharacters[index];
				if ( AllowRepetition || c != lastChar || this.AllowedCharacters.Length < 2 ) {
					pw.Append( this.AllowedCharacters[index] );
					lastChar = c;
				} else {
					x--;
				}
			}
			for ( int x = 0; x < words.Count; x++ ) {
				int index = r.Next( 0, pw.Length );
				pw.Insert( index, words[x] );
			}
			return pw.ToString();
		}

		#endregion

		#region " Static Methods "

		public static byte[] EnsureValidPassword( byte[] password, SymmetricAlgorithm alg ) {
			if ( alg.KeySize != password.Length * 8 ) {
				byte[] temppw = new byte[alg.KeySize / 8];
				for ( int x = 0; x < password.Length; x++ ) {
					if ( x < temppw.Length ) {
						temppw[x] = password[x];
					}
				}
				password = temppw;
			}
			return password;
		}

		#endregion

	}

}
