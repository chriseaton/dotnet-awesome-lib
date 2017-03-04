/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Awesome.Library.Utilities {

	public static class Numerics {

		public static byte SafeValue( this byte? value ) {
			if ( value.HasValue ) return value.Value;
			return 0;
		}

		public static int SafeValue( this int? value ) {
			if ( value.HasValue ) return value.Value;
			return 0;
		}

		public static short SafeValue( this short? value ) {
			if ( value.HasValue ) return value.Value;
			return 0;
		}

		public static decimal SafeValue( this decimal? value ) {
			if ( value.HasValue ) return value.Value;
			return 0;
		}

		public static double SafeValue( this double? value ) {
			if ( value.HasValue ) return value.Value;
			return 0;
		}

		public static long SafeValue( this long? value ) {
			if ( value.HasValue ) return value.Value;
			return 0;
		}

		public static float SafeValue( this float? value ) {
			if ( value.HasValue ) return value.Value;
			return 0;
		}

		public static byte Minimum( this byte value, byte minimum ) {
			if ( value < minimum )
				return minimum;
			return value;
		}

		public static int Minimum( this int value, int minimum ) {
			if ( value < minimum )
				return minimum;
			return value;
		}

		public static short Minimum( this short value, short minimum ) {
			if ( value < minimum )
				return minimum;
			return value;
		}

		public static decimal Minimum( this decimal value, decimal minimum ) {
			if ( value < minimum )
				return minimum;
			return value;
		}

		public static double Minimum( this double value, double minimum ) {
			if ( value < minimum )
				return minimum;
			return value;
		}

		public static long Minimum( this long value, long minimum ) {
			if ( value < minimum )
				return minimum;
			return value;
		}

		public static float Minimum( this float value, float minimum ) {
			if ( value < minimum )
				return minimum;
			return value;
		}

		public static byte Maximum( this byte value, byte maximum ) {
			if ( value > maximum )
				return maximum;
			return value;
		}

		public static int Maximum( this int value, int maximum ) {
			if ( value > maximum )
				return maximum;
			return value;
		}

		public static short Maximum( this short value, short maximum ) {
			if ( value > maximum )
				return maximum;
			return value;
		}

		public static decimal Maximum( this decimal value, decimal maximum ) {
			if ( value > maximum )
				return maximum;
			return value;
		}

		public static double Maximum( this double value, double maximum ) {
			if ( value > maximum )
				return maximum;
			return value;
		}

		public static long Maximum( this long value, long maximum ) {
			if ( value > maximum )
				return maximum;
			return value;
		}

		public static float Maximum( this float value, float maximum ) {
			if ( value > maximum )
				return maximum;
			return value;
		}

		public static byte SafeRange( this byte value, byte minimum, byte maximum ) {
			if ( value > maximum )
				return maximum;
			else if ( value < minimum )
				return minimum;
			return value;
		}

		public static int SafeRange( this int value, int minimum, int maximum ) {
			if ( value > maximum )
				return maximum;
			else if ( value < minimum )
				return minimum;
			return value;
		}

		public static short SafeRange( this short value, short minimum, short maximum ) {
			if ( value > maximum )
				return maximum;
			else if ( value < minimum )
				return minimum;
			return value;
		}

		public static float SafeRange( this float value, float minimum, float maximum ) {
			if ( value > maximum )
				return maximum;
			else if ( value < minimum )
				return minimum;
			return value;
		}

		public static decimal SafeRange( this decimal value, decimal minimum, decimal maximum ) {
			if ( value > maximum )
				return maximum;
			else if ( value < minimum )
				return minimum;
			return value;
		}

		public static double SafeRange( this double value, double minimum, double maximum ) {
			if ( value > maximum )
				return maximum;
			else if ( value < minimum )
				return minimum;
			return value;
		}

		public static long SafeRange( this long value, long minimum, long maximum ) {
			if ( value > maximum )
				return maximum;
			else if ( value < minimum )
				return minimum;
			return value;
		}

		/// <summary>
		/// Xors all bytes in an array against another array, using each value in array 2 on each value in array 1.
		/// </summary>
		public static byte[] MatrixXor( this byte[] array1, byte[] array2 ) {
			if ( array1 != null && array2 != null ) {
				for ( int x = 0; x < array1.Length; x++ ) {
					for ( int z = 0; z < array2.Length; z++ ) {
						array1[x] ^= array2[z];
					}
				}
				return array1;
			}
			return null;
		}

	}

}
