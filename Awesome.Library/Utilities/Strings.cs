/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using Awesome.Library.Resources;
using System.Linq;
using System.Text.RegularExpressions;

namespace Awesome.Library.Utilities {

	#region " Enumerators "

	public enum StringConversion {
		Money,
		MoneyAutoD,
		Money5D,
		Percent,
		PhoneNumber,
		Number,
		NumberNoDecimals,
		NumberOneDecimal,
		NumberTwoDecimals,
		NumberThreeDecimals,
		NumberFourDecimals,
		URL,
		YesNo,
		OnOff,
		AllowedDenied,
		URLEncode,
		URLDecode,
		ZipCode,
		EscapeXHTMLText,
		UnEscapeXHTMLText,
		HtmlCharacterCodes,
		None
	}

	public enum StringOp {
		StripQuotes,
		StripNumbers,
		StripAlpha,
		StripSpecial,
		StripWhiteSpace,
		StripHTML,
		UnMnemonic,
		AllowOnlyNumbers,
		AllowOnlyNumeric,
		AllowOnlyAlphaNumeric,
		AllowOnlyAlpha,
		AllowOnlySpecial,
		AllowOnlyGoodHTML,
		None
	}

	public enum StringForm {
		None,
		Email,
		URL
	}

	#endregion

	public static class Strings {

		private static readonly string[] GoodHtmlTags = new string[] {
			"p", "b", "i", "ul", "u", "em", "address", "span", "blockquote", "pre", "br", "hr",
			"ol", "li", "strong", "a", "h1", "h2","h3","h4","h5","h6", "sub", "sup", "font"
		};

		private static readonly string[] GoodAttributes = new string[] {
			"style", "href", "target", "border", "align", "title", "size", "color"
		};

		private static readonly string[] GoodStyles = new string[] {
			"padding", "background-color","text-decoration","font-size","color", "font-family"
		};

		#region " Methods "

		#region " Display Values "

		/// <summary>
		/// Combines the city/state/zip into a readable address line, even if one or some of the values are missing.
		/// </summary>
		public static string CityStateZip( string city, string state, string zip ) {
			bool hasCity = ( String.IsNullOrEmpty( city ) == false );
			bool hasState = ( String.IsNullOrEmpty( state ) == false );
			bool hasZip = ( String.IsNullOrEmpty( zip ) == false );
			string result = String.Empty;
			if ( hasCity )
				result += city;
			if ( hasCity && hasState )
				result += ' ';
			if ( hasState )
				result += state;
			if ( ( hasCity || hasState ) && hasZip )
				result += ", ";
			if ( hasZip )
				result += zip;
			return result;
		}

		/// <summary>
		/// Returns a business name, or if not specified, the full name from the given first and last name values. (First Last)
		/// </summary>
		public static string BusinessOrFullName( string businessName, string firstName, string lastName ) {
			return BusinessOrFullName( businessName, firstName, null, lastName );
		}

		/// <summary>
		/// Returns a business name, or if not specified, the full name from the given first, middle, and last name values. (First M.I. Last)
		/// </summary>
		public static string BusinessOrFullName( string businessName, string firstName, string middleName, string lastName ) {
			if ( String.IsNullOrEmpty( businessName ) ) {
				return FullName( firstName, middleName, lastName );
			}
			return businessName;
		}

		/// <summary>
		/// Returns a full name from the given first and last name values. (First Last)
		/// </summary>
		public static string FullName( string firstName, string lastName ) {
			return FullName( firstName, null, lastName );
		}

		/// <summary>
		/// Returns a full name from the given first, middle, and last name values. (First M.I. Last)
		/// </summary>
		public static string FullName( string firstName, string middleName, string lastName ) {
			StringBuilder sb = new StringBuilder();
			bool hasFirst = ( String.IsNullOrEmpty( firstName ) == false );
			bool hasMiddle = ( String.IsNullOrEmpty( middleName ) == false );
			bool hasLast = ( String.IsNullOrEmpty( lastName ) == false );
			if ( hasFirst ) {
				sb.Append( firstName.Trim() );
				if ( hasMiddle || hasLast ) {
					sb.Append( ' ' );
				}
			}
			if ( hasMiddle ) {
				sb.Append( middleName.Trim()[0].ToString().ToUpper() );
				if ( middleName.Trim().Length == 1 ) {
					sb.Append( '.' );
				}
				if ( hasLast ) {
					sb.Append( ' ' );
				}
			}
			if ( hasLast ) {
				sb.Append( lastName.Trim() );
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns a business name, or if not specified, the formal name from the given first, middle, and last name values. (Last, First)
		/// </summary>
		public static string BusinessOrFormalName( string businessName, string firstName, string lastName ) {
			return BusinessOrFormalName( businessName, firstName, null, lastName );
		}

		/// <summary>
		/// Returns a business name, or if not specified, the formal name from the given first, middle, and last name values. (Last, First M.I.)
		/// </summary>
		public static string BusinessOrFormalName( string businessName, string firstName, string middleName, string lastName ) {
			if ( String.IsNullOrEmpty( businessName ) ) {
				return FormalName( firstName, middleName, lastName );
			}
			return businessName;
		}

		/// <summary>
		/// Returns a formal name from the given first and last name values. (Last, First)
		/// </summary>
		public static string FormalName( string firstName, string lastName ) {
			return FormalName( firstName, null, lastName );
		}

		/// <summary>
		/// Returns a formal name from the given first, middle, and last name values. (Last, First M.I.)
		/// </summary>
		public static string FormalName( string firstName, string middleName, string lastName ) {
			StringBuilder sb = new StringBuilder();
			bool hasFirst = ( String.IsNullOrEmpty( firstName ) == false );
			bool hasMiddle = ( String.IsNullOrEmpty( middleName ) == false );
			bool hasLast = ( String.IsNullOrEmpty( lastName ) == false );
			if ( hasLast ) {
				sb.Append( lastName.Trim() );
				if ( hasFirst || hasMiddle ) {
					sb.Append( ", " );
				}
			}
			if ( hasFirst ) {
				sb.Append( firstName.Trim() );
				if ( hasMiddle ) {
					sb.Append( ' ' );
				}
			}
			if ( hasMiddle ) {
				sb.Append( middleName.Trim()[0].ToString().ToUpper() );
				if ( middleName.Trim().Length == 1 ) {
					sb.Append( '.' );
				}
			}
			return sb.ToString();
		}

		#endregion

		public static string CreateRandom() {
			return CreateRandom( 5, 8 );
		}

		/// <summary>
		/// Generates random text with the random amount of specified chars
		/// </summary>
		public static string CreateRandom( int minChars, int maxChars ) {
			Random r = new Random();
			return CreateRandom( r, minChars, maxChars );
		}

		/// <summary>
		/// Generates random text with the random amount of specified chars
		/// </summary>
		public static string CreateRandom( Random rand, int minChars, int maxChars ) {
			// Generate random text
			string s = "";
			char[] chars = "acdefijkqrstuxyzABCDEFGHJKLNPQRSTUXYZ23456789".ToCharArray();
			int index;
			int length = rand.Next( minChars, maxChars );
			for ( int i = 0; i < length; i++ ) {
				index = rand.Next( chars.Length - 1 );
				s += chars[index].ToString();
			}
			return s;
		}

		/// <summary>
		/// Randomizes the given string, moving chars around in a random order
		/// </summary>
		public static string Randomize( this string str ) {
			// The random number sequence
			Random num = new Random();
			// Create new string from the reordered char array
			return new string( str.ToCharArray().OrderBy( s => ( num.Next( 2 ) % 2 ) == 0 ).ToArray() );
		}

		/// <summary>
		/// Randomizes the order of strings in a string array
		/// </summary>
		public static string[] Randomize( this string[] str ) {
			return Randomize( str, new Random() );
		}

		/// <summary>
		/// Randomizes the order of strings in a string array
		/// </summary>
		public static string[] Randomize( this string[] str, Random rand ) {
			return str.OrderBy( s => ( rand.Next( 2 ) % 2 ) == 0 ).ToArray();
		}

		/// <summary>
		/// Moves a portion of a string to the end.
		/// </summary>
		public static string MoveToEnd( this string str, int startIndex, int length ) {
			string temp = str.Substring( startIndex, length );
			return str.Substring( 0, startIndex ) + str.Substring( startIndex + length ) + temp;
		}

		/// <summary>
		/// Moves a portion of a string to the beginning.
		/// </summary>
		public static string MoveToStart( this string str, int startIndex, int length ) {
			string temp = str.Substring( startIndex, length );
			return temp + str.Substring( 0, startIndex ) + str.Substring( startIndex + length );
		}

		/// <summary>
		/// Reverses the string
		/// </summary>
		public static string Reverse( this string str ) {
			char[] charArray = new char[str.Length];
			int len = str.Length - 1;
			for ( int i = 0; i <= len; i++ )
				charArray[i] = str[len - i];
			return new string( charArray );
		}

		/// <summary>
		/// Replaces all strings with the new string value
		/// </summary>
		public static string ReplaceAll( this string str, string newStr, params string[] strsToReplace ) {
			if ( str != null ) {
				foreach ( string s in strsToReplace ) {
					str = str.Replace( s, newStr );
				}
			}
			return str;
		}

		/// <summary>
		/// Replaces all strings with the new string value
		/// </summary>
		public static string ReplaceAll( this string str, string newStr, bool ignoreCase, params string[] strsToReplace ) {
			if ( ignoreCase == false )
				return ReplaceAll( str, newStr, strsToReplace );
			if ( str != null ) {
				foreach ( string s in strsToReplace ) {
					string temp = s.ToLower();
					int lastIndex = str.ToLower().IndexOf( temp );
					while ( lastIndex > 0 ) {
						str = str.Substring( 0, lastIndex ) + newStr + str.Substring( lastIndex + temp.Length );
						if ( lastIndex + 1 < str.Length )
							lastIndex = str.ToLower().IndexOf( temp, lastIndex + 1 );
						else
							lastIndex = -1;
					}
				}
			}
			return str;
		}


		/// <summary>
		/// Converts a string containing individual and ranges of numeric values to a single array of ints.
		/// </summary>
		public static int[] Expand( this string input ) {
			List<int> results = new List<int>();
			if ( input != null ) {
				string[] parts = input.Split( new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries );
				for ( int x = 0; x < parts.Length; x++ ) {
					int number = -1;
					if ( parts[x].IndexOf( '-' ) >= 0 ) {
						string[] range = parts[x].Split( new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries );
						if ( range.Length > 1 ) {
							int start = -1;
							int end = -1;
							if ( int.TryParse( range[0], out start ) && int.TryParse( range[range.Length - 1], out end ) ) {
								for ( int r = Math.Min( start, end ); r <= Math.Max( start, end ); r++ ) {
									if ( results.Contains( r ) == false ) {
										results.Add( r );
									}
								}
							}
						} else if ( range.Length == 1 && int.TryParse( range[0], out number ) && results.Contains( number ) == false ) {
							results.Add( number );
						}
					} else if ( int.TryParse( parts[x], out number ) && results.Contains( number ) == false ) {
						results.Add( number );
					}
				}
			}
			return results.ToArray();
		}

		/// <summary>
		/// Compacts the given characters in the given input string, allowing no more than 1 occurrance.
		/// </summary>
		public static string Compact( this string input, params char[] compact ) {
			if ( input != null && input.Length > 1 ) {
				StringBuilder output = new StringBuilder( input.Length );
				char last = input[0];  // anything other then space 
				int i, y;
				for ( i = 1; i < input.Length; i++ ) {
					char current = input[i];
					bool matched = false;
					for ( y = 0; y < compact.Length; y++ ) {
						if ( current == compact[y] && last == compact[y] ) {
							matched = true;
							break;
						}
					}
					if ( matched == false ) {
						output.Append( current );
					}
					last = current;
				}

				return output.ToString();
			}
			return input;
		}

		/// <summary>
		/// Attempts to seperate a scrunched together name with spaces.
		/// <para>
		/// Example: 'ThisIsASentance' would become 'This Is A Sentance'
		/// </para>
		/// </summary>
		public static string CapSpace( this string str ) {
			if ( str != null ) {
				StringBuilder sb = new StringBuilder();
				bool lastCharUpper = false;
				for ( int x = 0; x < str.Length; x++ ) {
					char c = str[x];
					if ( char.IsUpper( c ) ) {
						if ( x > 0 && x + 1 < str.Length && char.IsUpper( str[x + 1] ) == false ) {
							lastCharUpper = false;
						}
						if ( lastCharUpper == false ) { //add space
							sb.Append( ' ' );
						}
						sb.Append( c );
						lastCharUpper = true;
					} else {
						sb.Append( c );
						lastCharUpper = false;
					}
				}
				return sb.ToString();
			}
			return str;
		}

		/// <summary>
		/// Compares two strings for equality, case-insensitive, and trimmed
		/// </summary>
		public static bool SafeEquals( this string str, string str2 ) {
			if ( str != null && str2 != null ) {
				if ( str.ToString().ToLower().Trim() == str2.ToLower().Trim() )
					return true;
				return false;
			}
			return str == str2;
		}

		/// <summary>
		/// Compares two strings for equality, case-insensitive, and trimmed
		/// </summary>
		public static bool SafeEqualsAny( this string str, params string[] matches ) {
			if ( matches != null ) {
				for ( int x = 0; x < matches.Length; x++ ) {
					if ( str == matches[x] ) {
						return true;
					}
					if ( str != null && matches[x] != null ) {
						if ( str.ToString().ToLower().Trim() == matches[x].ToLower().Trim() )
							return true;
					}
				}
				return false;
			}
			return false;
		}

		/// <summary>
		/// Converts an empty (trimmed) string to null
		/// </summary>
		public static string EmptyToNull( this string str ) {
			if ( str != null ) {
				if ( str.Trim().Length == 0 )
					str = null;
			}
			return str;
		}

		/// <summary>
		/// Searches the target string to see if it contains any of the items specified
		/// </summary>
		public static bool ContainsAny( this string target, params string[] items ) {
			return ContainsAny( target, false, items );
		}

		/// <summary>
		/// Searches the target string to see if it contains any of the items specified
		/// </summary>
		public static bool ContainsAny( this string target, bool caseSensitive, params string[] items ) {
			if ( target != null && !caseSensitive )
				target = target.ToLower();
			foreach ( string s in items ) {
				string str = s ?? "";
				if ( target != null ) {
					if ( !caseSensitive )
						str = str.ToLower();
					if ( target.Contains( str ) )
						return true;
				} else if ( str == null )
					return true;
			}
			if ( items != null && items.Length == 0 && target != null ) //should return true if target non-empty but not items specified
				return true;
			return false;
		}

		/// <summary>
		/// Searches the target string to see if it contains all of the items specified (in no particular order)
		/// </summary>
		public static bool ContainsAll( this string target, params string[] items ) {
			return ContainsAny( target, false, items );
		}

		/// <summary>
		/// Searches the target string to see if it contains all of the items specified (in no particular order)
		/// </summary>
		public static bool ContainsAll( this string target, bool caseSensitive, params string[] items ) {
			if ( target != null && !caseSensitive )
				target = target.ToLower();
			foreach ( string s in items ) {
				string str = s ?? "";
				if ( target != null ) {
					if ( !caseSensitive )
						str = str.ToLower();
					if ( target.Contains( str ) == false )
						return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Searches the target string to see if it ends with any of the items specified (in no particular order)
		/// </summary>
		public static bool EndsWithAny( this string target, params string[] items ) {
			return ContainsAny( target, false, items );
		}

		/// <summary>
		/// Searches the target string to see if it ends with any of the items specified (in no particular order)
		/// </summary>
		public static bool EndsWithAny( this string target, bool caseSensitive, params string[] items ) {
			if ( target != null && !caseSensitive )
				target = target.ToLower();
			foreach ( string s in items ) {
				string str = s ?? "";
				if ( target != null ) {
					if ( !caseSensitive )
						str = str.ToLower();
					if ( target.EndsWith( str ) )
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Searches the target string to see if it starts with any of the items specified (in no particular order)
		/// </summary>
		public static bool StartsWithAny( this string target, params string[] items ) {
			return ContainsAny( target, false, items );
		}

		/// <summary>
		/// Searches the target string to see if it starts with any of the items specified (in no particular order)
		/// </summary>
		public static bool StartsWithAny( this string target, bool caseSensitive, params string[] items ) {
			if ( target != null && !caseSensitive )
				target = target.ToLower();
			foreach ( string s in items ) {
				string str = s ?? "";
				if ( target != null ) {
					if ( !caseSensitive )
						str = str.ToLower();
					if ( target.StartsWith( str ) )
						return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Counts the number of letters in a string
		/// </summary>
		public static int CountLetters( this string inputString ) {
			int count = 0;
			if ( inputString != null ) {
				for ( int x = 0; x < inputString.Length; x++ ) {
					if ( char.IsLetter( inputString[x] ) )
						count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Counts the number of numbers in a string
		/// </summary>
		public static int CountNumbers( this string inputString ) {
			int count = 0;
			if ( inputString != null ) {
				for ( int x = 0; x < inputString.Length; x++ ) {
					if ( char.IsDigit( inputString[x] ) )
						count++;
				}
			}
			return count;
		}

		/// <summary>
		/// Counts the number of non-alphanumeric characters in a string
		/// </summary>
		public static int CountSpecial( this string inputString ) {
			int count = 0;
			if ( inputString != null ) {
				for ( int x = 0; x < inputString.Length; x++ ) {
					if ( char.IsLetterOrDigit( inputString[x] ) == false )
						count++;
				}
			}
			return count;
		}

		public static int CountWords( this string inputString, int minChars ) {
			return System.Text.RegularExpressions.Regex.Matches( inputString, "\\b\\w{" + minChars + ",}\\b" ).Count;
		}

		/// <summary>
		/// Lists out all items in an enumerable string set.
		/// </summary>
		public static string ListValues( this IEnumerable<string> strs, string seperator ) {
			StringBuilder sb = new StringBuilder();
			foreach ( string s in strs ) {
				sb.Append( s );
				sb.Append( seperator );
			}
			string final = sb.ToString();
			if ( final.SafeToString().Length > 0 ) {
				final = final.Substring( 0, final.Length - seperator.Length );
			}
			return final;
		}

		/// <summary>
		/// Attempts to read a line from a multiline string. If nothing is found, an empty string is returned.
		/// </summary>
		public static int CountLines( this string value ) {
			if ( value != null ) {
				string[] lines = value.Split( new string[] { Environment.NewLine }, StringSplitOptions.None );
				return lines.Length;
			}
			return 0;
		}

		/// <summary>
		/// Attempts to read a line from a multiline string. If nothing is found, an empty string is returned.
		/// </summary>
		public static string ReadLine( this string value, int line ) {
			string retVal = String.Empty;
			string[] lines = value.Split( new string[] { Environment.NewLine }, StringSplitOptions.None );
			if ( lines.Length > line )
				retVal = lines[line];
			return retVal;
		}

		/// <summary>
		/// Combines 2 strings by xor'ing the byte values.
		/// </summary>
		/// <returns>Mutated string value containing both values</returns>
		public static string XORMerge( string value1, string value2 ) {
			int len = ( value1.Length > value2.Length ) ? value1.Length : value2.Length;
			StringBuilder sbLarger = new StringBuilder( ( value1.Length > value2.Length ) ? value1 : value2 );
			StringBuilder sbSmaller = new StringBuilder( ( value1.Length < value2.Length ) ? value1 : value2 );
			StringBuilder sbResult = new StringBuilder( sbLarger.Length );
			int counter = 0;
			for ( int x = 0; x < sbResult.Length; x++ ) {
				byte p = (byte)sbLarger[x];
				if ( sbSmaller.Length > 0 ) {
					p ^= (byte)sbSmaller[counter];
					counter++;
					if ( counter > sbSmaller.Length )
						counter = 0;
				}
				sbResult.Append( (char)p );
			}
			return sbResult.ToString();
		}

		/// <summary>
		/// Skips the given number of characters and returns the rest of ths string
		/// </summary>
		public static string Skip( this string str, int characters ) {
			if ( String.IsNullOrEmpty( str ) == false && str.Length - characters >= 0 )
				return Last( str, str.Length - characters );
			return str;
		}

		/// <summary>
		/// Returns the first few characters of a string
		/// </summary>
		/// <param name="characters">The number of characters to get from the start of the string</param>
		public static string First( this string str, int characters ) {
			if ( str != null ) {
				if ( str.Length > characters ) {
					return str.Substring( 0, characters );
				} else {
					return str;
				}
			}
			return str;
		}

		/// <summary>
		/// Returns the last few characters of a string
		/// </summary>
		/// <param name="characters">The number of characters to get from the end of the string</param>
		public static string Last( this string str, int characters ) {
			if ( str != null ) {
				if ( str.Length >= characters ) {
					return str.Substring( str.Length - characters, characters );
				} else {
					return str;
				}
			}
			return str;
		}

		/// <summary>
		/// Creates a URL friendly slug string from the given input
		/// </summary>
		public static string Slugify( string input ) {
			StringBuilder sb = new StringBuilder();
			bool space = false;
			for ( int x = 0; x < input.Length; x++ ) {
				char c = input[x];
				if ( Char.IsLetterOrDigit( c ) ) {
					sb.Append( Char.ToLower( c ) );
					space = false;
				} else if ( ( Char.IsSeparator( c ) || c == '@' || c == '.' || c == ';' || c == ':' ) && space == false ) {
					sb.Append( '-' );
					space = true;
				}
			}
			return sb.ToString().Trim( '-' );
		}

		/// <summary>
		/// Checks if a string contains another, case-insensitive, and provides null checking
		/// </summary>
		public static bool SafeContains( this string str, string match ) {
			if ( str == null && match == null )
				return true;
			if ( str != null && match != null ) {
				return str.ToLower().Contains( match.ToLower() );
			}
			return false;
		}

		/// <summary>
		/// Places the prefix text before the given string, if the given string is not empty or null
		/// </summary>
		/// <returns></returns>
		public static string SafePrefix( this string str, string prefix ) {
			if ( String.IsNullOrEmpty( str ) == false && prefix != null )
				return prefix + str;
			return str;
		}

		/// <summary>
		/// Places the postfix text after the given string, if the given string is not empty or null
		/// </summary>
		/// <returns></returns>
		public static string SafePostfix( this string str, string postfix ) {
			if ( String.IsNullOrEmpty( str ) == false && postfix != null )
				return str + postfix;
			return str;
		}

		/// <summary>
		/// Converts a null object to a blank string, or returns the 'ToString()' value
		/// </summary>
		public static string SafeToString( this object o ) {
			return ( o ?? String.Empty ).ToString();
		}

		/// <summary>
		/// Converts a null object to a blank string, or returns the 'ToString()' value
		/// </summary>
		public static string SafeToString( this object o, string nullStr ) {
			return ( o ?? nullStr ).ToString();
		}

		/// <summary>
		/// Safely resizes a string to a maximum length (trimmed).
		/// </summary>
		public static string SafeSize( this string str, int maximumSize ) {
			return SafeSize( str, maximumSize, true );
		}

		/// <summary>
		/// Safely resizes a string to a maximum length.
		/// </summary>
		public static string SafeSize( this string str, int maximumSize, bool trim ) {
			if ( str != null ) {
				if ( trim )
					str = str.Trim();
				if ( str.Length > maximumSize && maximumSize > 0 )
					str = str.Substring( 0, maximumSize );
			}
			return str;
		}

		/// <summary>
		/// Safely resizes a string to a maximum length, adds 'postfix' string if size is met or exceeded before resize
		/// </summary>
		public static string SafeSize( this string str, int maximumSize, string postfix ) {
			if ( str != null ) {
				if ( str.Length > maximumSize && maximumSize > 0 )
					str = str.Substring( 0, maximumSize ) + postfix.SafeToString();
			}
			return str;
		}

		/// <summary>
		/// Validates whether a string conforms to a certain form
		/// </summary>
		/// <returns>TRUE if the string conforms, FALSE if not</returns>
		public static bool Validate( object obj, StringForm formOfString ) {
			return Validate( obj.ToString(), formOfString );
		}

		public static int SafeIndexOf( this IEnumerable<string> strs, string match ) {
			if ( strs != null && match != null ) {
				int index = 0;
				foreach ( string s in strs ) {
					if ( Strings.SafeEquals( s, match ) ) {
						return index;
					}
					index++;
				}
			}
			return -1;
		}

		/// <summary>
		/// Validates whether a string conforms to a certain form
		/// </summary>
		/// <returns>TRUE if the string conforms, FALSE if not</returns>
		public static bool Validate( this string str, StringForm formOfString ) {
			if ( str != null && str.Trim().Length > 0 ) {
				switch ( formOfString ) {
					case StringForm.Email:
						string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
										  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
										  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
						Regex re = new Regex( strRegex );
						if ( re.IsMatch( str ) && str.Length < 255 ) //emails shouldn't be longer than 254 chars
							return true;
						break;
					case StringForm.URL:
						return System.Uri.IsWellFormedUriString( str, UriKind.Absolute );
				}
			}
			return false;
		}

		/// <summary>
		/// Performs a basic string format
		/// </summary>
		/// <returns>Formatted string</returns>
		public static string Convert( object obj, StringConversion conversion ) {
			return Convert( obj.ToString(), conversion );
		}

		/// <summary>
		/// Performs a basic string format
		/// </summary>
		/// <returns>Formatted string</returns>
		public static string Convert( this string str, StringConversion conversion ) {
			bool strAsBool = false;
			if ( str != null ) {
				if ( str.ToLower().Trim().ContainsAny( "t", "1", "y" ) )
					strAsBool = true;
				switch ( conversion ) {
					case StringConversion.EscapeXHTMLText:
						str = str.Replace( "&", "&amp;" );
						str = str.Replace( "<", "&lt;" );
						str = str.Replace( ">", "&gt;" );
						str = str.Replace( "\"", "&quot;" );
						str = str.Replace( "'", "&apos;" );
						str = str.Replace( "�", "&copy;" );
						str = str.Replace( "�", "&trade;" );
						str = str.Replace( "�", "&reg;" );
						str = str.Replace( "�", "&plusmn;" );
						str = str.Replace( "�", "&micro;" );
						break;
					case StringConversion.UnEscapeXHTMLText:
						str = str.Replace( "&amp;", "&" );
						str = str.Replace( "&lt;", "<" );
						str = str.Replace( "&gt;", ">" );
						str = str.Replace( "&quot;", "\"" );
						str = str.Replace( "&apos;", "'" );
						str = str.Replace( "&copy;", "�" );
						str = str.Replace( "&trade;", "�" );
						str = str.Replace( "&reg;", "�" );
						str = str.Replace( "&plusmn;", "�" );
						str = str.Replace( "&micro;", "�" );
						break;
					case StringConversion.Money:
						str = ToDecimal( str ).ToString( "C" );
						break;
					case StringConversion.MoneyAutoD:
						decimal strValue = ToDecimal( str );
						if ( Math.Floor( strValue ) == strValue ) {
							str = strValue.ToString( "C0" );
						} else {
							str = strValue.ToString( "C" );
						}
						break;
					case StringConversion.Money5D:
						str = ToDecimal( str ).ToString( "C5" );
						break;
					case StringConversion.Percent:
						decimal sx = ToDecimal( str );
						str = sx.ToString( "P" );
						break;
					case StringConversion.ZipCode:
						str = Perform( str, StringOp.AllowOnlyNumbers );
						if ( str.Length > 5 && str.Length != 7 )
							str = str.Insert( 5, "-" );
						if ( str.Length > 10 )
							str = str.Substring( 0, 10 );
						break;
					case StringConversion.PhoneNumber:
						string temp = Perform( str, StringOp.AllowOnlyNumbers );
						if ( temp.Length == 10 ) {
							str = ToDouble( temp ).ToString( Formats.PhoneNumber );
						} else if ( temp.Length > 10 ) {
							string form = Formats.PhoneNumberExtensions;
							for ( int x = 0; x < temp.Length - 10; x++ )
								form += "#";
							str = ToDouble( temp ).ToString( form );
						} else if ( temp.Length == 7 ) {
							str = ToDouble( temp ).ToString( Formats.PhoneNumberShort );
						}
						break;
					case StringConversion.Number:
						str = ToDecimal( str ).ToString();
						break;
					case StringConversion.NumberNoDecimals:
						str = ToDecimal( str ).ToString( "N0" );
						break;
					case StringConversion.NumberOneDecimal:
						str = ToDecimal( str ).ToString( "N1" );
						break;
					case StringConversion.NumberTwoDecimals:
						str = ToDecimal( str ).ToString( "N2" );
						break;
					case StringConversion.NumberThreeDecimals:
						str = ToDecimal( str ).ToString( "N3" );
						break;
					case StringConversion.NumberFourDecimals:
						str = ToDecimal( str ).ToString( "N4" );
						break;
					case StringConversion.URL:
						if ( str.ToLower().Trim().StartsWith( "http://" ) == false )
							str = "http://" + str;
						break;
					case StringConversion.YesNo:
						str = ( strAsBool ) ? "Yes" : "No";
						break;
					case StringConversion.OnOff:
						str = ( strAsBool ) ? "On" : "Off";
						break;
					case StringConversion.AllowedDenied:
						str = ( strAsBool ) ? "Allowed" : "Denied";
						break;
					case StringConversion.URLEncode:
						str = System.Web.HttpUtility.UrlEncode( str );
						break;
					case StringConversion.URLDecode:
						str = System.Web.HttpUtility.UrlDecode( str );
						break;
					case StringConversion.HtmlCharacterCodes:
						StringBuilder sb = new StringBuilder();
						for ( int x = 0; x < str.Length; x++ ) {
							sb.Append( "&#" );
							sb.Append( ( (byte)str[x] ).ToString() );
							sb.Append( ";" );
						}
						str = sb.ToString();
						break;
				}
			}
			return str;
		}

		/// <summary>
		/// Performs a basic string manipulation operation
		/// </summary>
		/// <returns>Operated string</returns>
		public static string Perform( object obj, StringOp operation ) {
			return Perform( obj.ToString(), operation );
		}

		/// <summary>
		/// Performs a basic string manipulation operation
		/// </summary>
		/// <returns>Operated string</returns>
		public static string Perform( this string str, StringOp operation ) {
			if ( str != null ) {
				switch ( operation ) {
					case StringOp.AllowOnlyAlpha:
						str = Regex.Replace( str, "[^A-z]", "" ).Replace( "_", "" );
						break;
					case StringOp.AllowOnlyAlphaNumeric:
						str = Regex.Replace( str, "[^A-z0-9]", "" ).Replace( "_", "" );
						break;
					case StringOp.AllowOnlyNumbers:
						str = Regex.Replace( str, "[^0-9]", "" );
						break;
					case StringOp.AllowOnlyNumeric:
						str = Regex.Replace( str, @"[^0-9.\-'('')']", "" );
						if ( str.Contains( "(" ) && str.Contains( ")" ) ) //negative parenthesis
							str = str.Insert( 0, "-" );
						str = str.Replace( "(", "" ).Replace( ")", "" );
						break;
					case StringOp.AllowOnlySpecial:
						str = Regex.Replace( str, "[^\\W]", "" );
						break;
					case StringOp.AllowOnlyGoodHTML:
						str = Regex.Replace( str, @"</?(((?!" + Strings.ListValues( GoodHtmlTags, "|" ) + @"|/)[^>]*))*>", "" );
						break;
					case StringOp.StripAlpha:
						str = Regex.Replace( str, "[A-z]", "" );
						break;
					case StringOp.StripNumbers:
						str = Regex.Replace( str, "[0-9]", "" );
						break;
					case StringOp.StripQuotes:
						str = str.Replace( "\"", "" ).Replace( "'", "" );
						break;
					case StringOp.StripSpecial:
						str = Regex.Replace( str, "[\\W]", "" );
						break;
					case StringOp.StripWhiteSpace:
						str = str.Replace( " ", "" ).Replace( "\t", "" );
						break;
					case StringOp.UnMnemonic:
						str = str.Replace( "&", "&&" );
						break;
					case StringOp.StripHTML:
						str = Regex.Replace( str, @"<(.|\n)*?>", string.Empty );
						break;
				}
			}
			return str;
		}

		/// <summary>
		/// Performs a basic string format, and manipulation operation
		/// </summary>
		/// <returns>Formatted & Operated string</returns>
		public static string Format( object obj, StringConversion conversion, StringOp operation ) {
			string str = Convert( obj.ToString(), conversion );
			return Perform( str, operation );
		}

		/// <summary>
		/// Performs a basic string format, and manipulation operation
		/// </summary>
		/// <returns>Formatted & Operated string</returns>
		public static string Format( string str, StringConversion conversion, StringOp operation ) {
			str = Convert( str, conversion );
			return Perform( str, operation );
		}

		/// <summary>
		/// Converts a string containing rectangle information to a Rectangle
		/// <para>
		///	Supports: "x,y,w,h" or "x y w h" or "x;y;w;h";
		/// </para>
		/// </summary>
		public static Rectangle ToRectangle( this string str ) {
			if ( str != null ) {
				if ( str.ToLower() == "empty" || str.ToLower() == "0" || String.IsNullOrEmpty( str ) == false )
					return Rectangle.Empty;
				str = str.Perform( StringOp.StripAlpha );
				string[] parts = str.Split( ',' );
				if ( parts.Length < 2 )
					parts = str.Split( ';' );
				if ( parts.Length < 2 )
					parts = str.Split( ' ' );
				if ( parts.Length >= 2 && parts.Length <= 3 ) {
					return new Rectangle( 0, 0, parts[0].ToInt32(), parts[1].ToInt32() );
				} else if ( parts.Length >= 4 ) {
					return new Rectangle( parts[0].ToInt32(), parts[1].ToInt32(), parts[2].ToInt32(), parts[3].ToInt32() );
				}
			}
			return Rectangle.Empty;
		}

		/// <summary>
		/// Converts a string containing float rectangle information to a RectangleF
		/// <para>
		///	Supports: "x,y,w,h" or "x y w h" or "x;y;w;h";
		/// </para>
		/// </summary>
		public static RectangleF ToRectangleF( this string str ) {
			if ( str != null ) {
				if ( str.ToLower() == "empty" || str.ToLower() == "0" || String.IsNullOrEmpty( str ) == false )
					return RectangleF.Empty;
				str = str.Perform( StringOp.StripAlpha );
				string[] parts = str.Split( ',' );
				if ( parts.Length < 2 )
					parts = str.Split( ';' );
				if ( parts.Length < 2 )
					parts = str.Split( ' ' );
				if ( parts.Length == 2 ) {
					return new RectangleF( 0, 0, parts[0].ToSingle(), parts[1].ToSingle() );
				} else if ( parts.Length >= 4 ) {
					return new RectangleF( parts[0].ToSingle(), parts[1].ToSingle(), parts[2].ToSingle(), parts[3].ToSingle() );
				}
			}
			return RectangleF.Empty;
		}

		/// <summary>
		/// Returns an guid from a string
		/// </summary>
		public static Guid ToGuid( this string str ) {
			if ( String.IsNullOrEmpty( str ) == false ) {
				try {
					return new Guid( str );
				} catch ( FormatException ) {
					return Guid.Empty;
				} catch ( OverflowException ) {
					return Guid.Empty;
				}
			}
			return Guid.Empty;
		}

		/// <summary>
		/// Returns an int32 from a string (potentially nullable)
		/// </summary>
		public static int? ToNullableInt32( this string str ) {
			if ( String.IsNullOrEmpty( str ) )
				return null;
			return str.ToInt32();
		}

		/// <summary>
		/// Returns an float from a string (potentially nullable)
		/// </summary>
		public static float? ToNullableSingle( this string str ) {
			if ( String.IsNullOrEmpty( str ) )
				return null;
			return System.Convert.ToSingle( str.ToDecimal() );
		}

		/// <summary>
		/// Returns an decimal from a string (potentially nullable)
		/// </summary>
		public static decimal? ToNullableDecimal( this string str ) {
			return ToNullableDecimal( str, false );
		}

		/// <summary>
		/// Returns an decimal from a string (potentially nullable)
		/// </summary>
		public static decimal? ToNullableDecimal( this string str, bool zeroIsNull ) {
			if ( String.IsNullOrEmpty( str ) )
				return null;
			if ( zeroIsNull && str.ToDecimal() == 0 )
				return null;
			return str.ToDecimal();
		}

		/// <summary>
		/// Returns a string converted to a nullable date time object
		/// </summary>
		public static DateTime? ToNullableDateTime( this string str ) {
			DateTime parse = DateTime.MinValue;
			if ( str.Length == 4 || str.Length == 5 ) {
				int mo = str.Last( 2 ).ToInt32();
				int yr = ( DateTime.Now.Year.ToString().First( 2 ) + str.First( 2 ) ).ToInt32();
				if ( mo > 0 && mo < 13 ) {
					try {
						parse = new DateTime( yr, mo, 1 );
					} catch { }
				}
			} else if ( str.Length == 8 && str.Contains( ' ' ) == false && str.Contains( '/' ) == false ) {
				int mo = str.First( 2 ).ToInt32();
				int day = str.Skip( 2 ).First( 2 ).ToInt32();
				int yr = str.Last( 4 ).ToInt32();
				if ( mo > 0 && mo < 13 ) {
					try {
						parse = new DateTime( yr, mo, day );
					} catch { }
				}
			} else {
				DateTime.TryParse( str ?? "", out parse );
			}
			if ( parse == DateTime.MinValue )
				return null;
			return parse;
		}

		/// <summary>
		/// Returns a string converted to a date time object
		/// </summary>
		public static DateTime ToDateTime( this string str ) {
			DateTime parse = DateTime.MinValue;
			if ( str.Length == 4 || str.Length == 5 ) {
				int mo = str.Last( 2 ).ToInt32();
				int yr = ( DateTime.Now.Year.ToString().First( 2 ) + str.First( 2 ) ).ToInt32();
				if ( mo > 0 && mo < 13 ) {
					try {
						parse = new DateTime( yr, mo, 1 );
					} catch { }
				}
			} else if ( str.Length == 8 && str.Contains( ' ' ) == false && str.Contains( '/' ) == false ) {
				int mo = str.First( 2 ).ToInt32();
				int day = str.Skip( 2 ).First( 2 ).ToInt32();
				int yr = str.Last( 4 ).ToInt32();
				if ( mo > 0 && mo < 13 ) {
					try {
						parse = new DateTime( yr, mo, day );
					} catch { }
				}
			} else {
				DateTime.TryParse( str ?? "", out parse );
			}
			return parse;
		}

		/// <summary>
		/// Converts any given string to a readable number
		/// </summary>
		public static string ToNumericString( this string str, bool allowDecimals ) {
			NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
			StringBuilder sb = new StringBuilder();
			bool isPositive = true;
			string decPlace = nfi.NumberDecimalSeparator;
			string negSign = nfi.NegativeSign;
			string dec = String.Empty;
			str = Strings.Perform( str, StringOp.StripAlpha );
			if ( String.IsNullOrEmpty( negSign ) )
				negSign = "-";
			if ( String.IsNullOrEmpty( decPlace ) ) {
				decPlace = ".";
			}
			//determine negative
			if ( str.Contains( '-' ) || str.Contains( '(' ) || str.Contains( ')' ) )
				isPositive = false;
			//extract decimal place
			int decimalIndex = str.IndexOf( decPlace );
			if ( decimalIndex > -1 ) {
				dec = str.Substring( decimalIndex + decPlace.Length );
				str = str.Substring( 0, decimalIndex );
			} else {
				decimalIndex = str.IndexOf( '.' );
				if ( decimalIndex > -1 ) {
					dec = str.Substring( decimalIndex + 1 );
					str = str.Substring( 0, decimalIndex );
				}
			}
			foreach ( char c in str ) {
				if ( char.IsNumber( c ) )
					sb.Append( c );
			}
			if ( allowDecimals && String.IsNullOrEmpty( dec ) == false ) {
				sb.Append( decPlace );
				foreach ( char c in dec ) {
					if ( char.IsNumber( c ) )
						sb.Append( c );
				}
			}
			if ( isPositive == false ) {
				if ( nfi.NumberNegativePattern < 3 ) {
					sb.Insert( 0, negSign );
				} else {
					sb.Append( negSign );
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns a int16 value from a string.
		/// </summary>
		public static short ToInt16( this string str ) {
			if ( str != null ) {
				str = Strings.ToNumericString( str, true );
				short retVal = 0;
				short.TryParse( str, out retVal );
				return retVal;
			}
			return 0;
		}

		/// <summary>
		/// Returns a int32 value from a string.
		/// </summary>
		public static int ToInt32( this string str ) {
			if ( str != null ) {
				str = Strings.ToNumericString( str, true );
				int retVal = 0;
				int.TryParse( str, out retVal );
				return retVal;
			}
			return 0;
		}

		/// <summary>
		/// Returns a int64 value from a string.
		/// </summary>
		public static long ToInt64( this string str ) {
			if ( str != null ) {
				str = Strings.ToNumericString( str, true );
				long retVal = 0;
				long.TryParse( str, out retVal );
				return retVal;
			}
			return 0;
		}

		/// <summary>
		/// Returns a bool value from a string.
		/// </summary>
		public static bool ToBoolean( this string str ) {
			if ( String.IsNullOrEmpty( str ) )
				return false;
			return str.ContainsAny( false, "1", "y", "true", "go", "on", "ok", "t" );
		}

		/// <summary>
		/// Returns a bool value from a string.
		/// </summary>
		public static bool ToBoolean( this string str, bool defaultEmptyValue ) {
			if ( String.IsNullOrEmpty( str ) )
				return defaultEmptyValue;
			return str.ContainsAny( false, "1", "y", "true", "go", "on", "ok", "t" );
		}

		/// <summary>
		/// Returns a byte value from a string.
		/// </summary>
		public static byte ToByte( this string str ) {
			if ( str != null ) {
				str = Strings.ToNumericString( str, true );
				byte retVal = 0;
				byte.TryParse( str, out retVal );
				return retVal;
			}
			return 0;
		}

		/// <summary>
		/// Converts a hex string to it's byte equivalents.
		/// </summary>
		public static byte[] ToBytes( this string hexStr ) {
			if ( hexStr != null ) {
				hexStr = hexStr.ToLower().Trim().Replace( "-", "" ).Replace( "0x", "" );
				byte[] bytes = new byte[hexStr.Length / 2];
				for ( int i = 0; i < hexStr.Length; i += 2 ) {
					bytes[i / 2] = System.Convert.ToByte( hexStr.Substring( i, 2 ), 16 );
				}
				return bytes;
			}
			return null;
		}

		/// <summary>
		/// Returns a float value from a string.
		/// </summary>
		public static float ToSingle( this string str ) {
			if ( str != null ) {
				str = Strings.ToNumericString( str, true );
				float result = 0;
				float.TryParse( str, NumberStyles.Any, CultureInfo.CurrentCulture, out result );
				return result;
			}
			return 0;
		}

		/// <summary>
		/// Gets a decimal value (extracted from object ToString()), 
		/// </summary>
		/// <param name="value">Object to extract a decimal value from</param>
		/// <returns>decimal value from object, if method cannot get a valid number, 0 is returned</returns>
		public static decimal ToDecimal( object value ) {
			return ToDecimal( value.ToString() );
		}

		/// <summary>
		/// Gets a decimal value (extracted from string), 
		/// </summary>
		/// <param name="str">String to extract a decimal value from</param>
		/// <returns>decimal value from string, if method cannot get a valid number, 0 is returned</returns>
		public static decimal ToDecimal( this string str ) {
			if ( str != null ) {
				Trace.WriteLine( "ToDecimal (orig): " + str );
				str = Strings.ToNumericString( str, true );
				Trace.WriteLine( "ToDecimal (new): " + str );
				decimal result = 0;
				decimal.TryParse( str, NumberStyles.Any, CultureInfo.CurrentCulture, out result );
				Trace.WriteLine( "ToDecimal (result): " + result.ToString() );
				return result;
			}
			return 0;
		}

		/// <summary>
		/// Gets a double value (extracted from object ToString()), 
		/// </summary>
		/// <param name="value">Object to extract a double value from</param>
		/// <returns>double value from object, if method cannot get a valid number, 0 is returned</returns>
		public static double ToDouble( object value ) {
			return ToDouble( value.ToString() );
		}

		/// <summary>
		/// Gets a double value (extracted from string), 
		/// </summary>
		/// <param name="str">String to extract a double value from</param>
		/// <returns>double value from string, if method cannot get a valid number, 0 is returned</returns>
		public static double ToDouble( this string str ) {
			if ( str != null ) {
				str = Strings.ToNumericString( str, true );
				double result = 0;
				double.TryParse( str, NumberStyles.AllowDecimalPoint | NumberStyles.AllowParentheses | NumberStyles.Integer, CultureInfo.CurrentCulture, out result );
				return result;
			}
			return 0;
		}

		/// <summary>
		/// Converts a string to a color (defaults to black)
		/// </summary>
		public static Color ToColor( this string str ) {
			return ToColor( str, Color.Black );
		}

		/// <summary>
		/// Converts a string to a color
		/// </summary>
		/// <param name="defaultColor">Color used if the string cannot be parsed</param>
		public static Color ToColor( this string str, Color defaultColor ) {
			if ( String.IsNullOrEmpty( str ) == false ) {
				Color temp = ColorTranslator.FromHtml( str );
				if ( temp != Color.Empty ) {
					if ( temp != Color.Empty )
						return temp;
				} else {
					temp = Color.FromName( str );
					if ( temp != Color.Empty )
						return temp;
				}
			}
			return defaultColor;
		}

		/// <summary>
		/// Attempts to automatically capitalize a string
		/// </summary>
		/// <param name="inputString">the string to capitalize</param>
		/// <returns>the formatted string</returns>
		public static string Capitilize( this string inputString ) {
			return Capitilize( inputString, true, true );
		}

		/// <summary>
		/// Attempts to automatically capitalize a string
		/// </summary>
		/// <param name="inputString">the string to capitalize</param>
		/// <param name="allWords">Attempt to capitalize all words?</param>
		/// <param name="forceLowerCase">if true force all non-capitilizing characters to lowercase</param>
		/// <returns>the formatted string</returns>
		public static string Capitilize( this string inputString, bool allWords, bool forceLowerCase ) {
			if ( inputString != null ) {
				if ( forceLowerCase )
					inputString = inputString.ToLower();
				if ( allWords ) {
					if ( inputString.Length > 0 ) {
						string[] str = inputString.Split( ' ' );
						inputString = "";
						for ( int x = 0; x < str.Length; x++ ) {
							if ( str[x] != "" ) {
								bool matchesSkipWord = false;
								if ( str[x].ToLower() == "and" )
									matchesSkipWord = true;
								if ( str[x].ToLower() == "in" )
									matchesSkipWord = true;
								if ( str[x].ToLower() == "of" )
									matchesSkipWord = true;
								if ( str[x].ToLower() == "vs" )
									matchesSkipWord = true;
								if ( str[x].ToLower() == "n" )
									matchesSkipWord = true;
								if ( ( str[x].Length > 1 ) && ( matchesSkipWord == false ) )
									str[x] = str[x].Substring( 0, 1 ).ToUpper() + str[x].Substring( 1, str[x].Length - 1 );
								if ( x != 0 )
									inputString += " ";
								inputString += str[x];
							} else
								inputString += " ";
						}
						return inputString;
					} else
						return inputString.ToUpper();
				} else {
					if ( inputString.Length > 1 )
						return inputString.Substring( 0, 1 ).ToUpper() + inputString.Substring( 1, inputString.Length - 1 );
					else
						return inputString.ToUpper();
				}
			}
			return inputString;
		}

		#endregion

	}
}
