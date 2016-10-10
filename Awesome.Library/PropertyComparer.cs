/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Awesome.Library {

	public class PropertyComparer<T> : IComparer<T> {

		private static Type MyType = typeof( T );

		private readonly PropertyDescriptor m_propertyDescription;
		private readonly ListSortDirection m_sortDirection;

		#region " Properties "

		public ListSortDirection SortDirection {
			get { return m_sortDirection; }
		}

		#endregion

		#region " Constructor(s) "

		public PropertyComparer( PropertyDescriptor property, ListSortDirection direction ) {
			m_propertyDescription = property;
			m_sortDirection = direction;
		}

		#endregion

		#region " Methods "

		public int Compare( T x, T y ) {
			object xValue = GetPropertyValue( x, m_propertyDescription.Name );
			object yValue = GetPropertyValue( y, m_propertyDescription.Name );
			if ( this.SortDirection == ListSortDirection.Ascending ) {
				return CompareAscending( xValue, yValue );
			} else {
				return CompareDescending( xValue, yValue );
			}
		}

		public bool Equals( T xWord, T yWord ) {
			return xWord.Equals( yWord );
		}

		public int GetHashCode( T obj ) {
			return obj.GetHashCode();
		}

		private int CompareAscending( object xValue, object yValue ) {
			if ( xValue == null && yValue == null ) {
				return 0;
			}
			if ( xValue == null )
				return -1;
			if ( yValue == null )
				return 1;
			int result;
			// If values implement IComparer
			if ( xValue is IComparable ) {
				result = ( (IComparable)xValue ).CompareTo( yValue );
			}
				// If values don't implement IComparer but are equivalent
			else if ( xValue.Equals( yValue ) ) {
				result = 0;
			}
				// Values don't implement IComparer and are not equivalent, so compare as string values
			else result = xValue.ToString().CompareTo( yValue.ToString() );
			return result;
		}

		private int CompareDescending( object xValue, object yValue ) {
			// Return result adjusted for ascending or descending sort order ie
			// multiplied by 1 for ascending or -1 for descending
			return CompareAscending( xValue, yValue ) * -1;
		}

		private object GetPropertyValue( T value, string property ) {
			// Get property
			PropertyInfo propertyInfo = MyType.GetProperty( property );
			// Return value
			return propertyInfo.GetValue( value, null );
		}

		#endregion

	}

}
