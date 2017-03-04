/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace Awesome.Library {

	public class SortableBindingList<T> : BindingList<T> {

		private bool m_isSorted;
		private ListSortDirection m_SortDirection;
		private PropertyDescriptor m_Property;

		#region " Properties "

		protected override bool SupportsSortingCore {
			get { return true; }
		}

		protected override bool IsSortedCore {
			get { return m_isSorted; }
		}

		protected override ListSortDirection SortDirectionCore {
			get { return m_SortDirection; }
		}

		protected override PropertyDescriptor SortPropertyCore {
			get { return m_Property; }
		}

		#endregion

		#region " Constructor(s) "

		public SortableBindingList() : base() { }

		public SortableBindingList( List<T> list ) : base( list ) { }

		public SortableBindingList( IList<T> list )
			: this( ( list == null ? new List<T>() : new List<T>( list ) ) ) {
		}

		#endregion

		#region " Methods "

		protected override void ApplySortCore( PropertyDescriptor property, ListSortDirection direction ) {
			// Get list to sort
			List<T> items = this.Items as List<T>;
			// Apply and set the sort, if items to sort
			if ( items != null ) {
				PropertyComparer<T> pc = new PropertyComparer<T>( property, direction );
				items.Sort( pc );
				m_SortDirection = direction;
				m_Property = property;
				m_isSorted = true;
			} else {
				m_isSorted = false;
			}
			// Let bound controls know they should refresh their views
			this.OnListChanged( new ListChangedEventArgs( ListChangedType.Reset, -1 ) );
		}

		protected override void RemoveSortCore() {
			m_isSorted = false;
		}

		#endregion

	}
}
