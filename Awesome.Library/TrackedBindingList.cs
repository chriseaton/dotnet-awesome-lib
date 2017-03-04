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
using Awesome.Library.Linq;
using Awesome.Library.Utilities;

namespace Awesome.Library {

	[Serializable()]
	public class TrackedBindingList<T> : BindingList<T>, IBindingListView {

		private List<T> m_DeletedItems = new List<T>();
		private List<T> m_NewItems = new List<T>();
		private List<T> m_UpdatedItems = new List<T>();
		private List<T> m_OriginalItems = null;
		private bool m_Sortable = true;
		private bool m_IsSorted = false;
		private ListSortDirection m_SortDirection;
		private PropertyDescriptor m_Property;
		private string m_Filter = null;
		private ListSortDescriptionCollection m_SortDescriptions = null;

		#region " Properties "

		protected override bool SupportsSortingCore {
			get { return this.Sortable; }
		}

		protected override bool IsSortedCore {
			get { return m_IsSorted; }
		}

		public bool Sortable {
			get { return m_Sortable; }
			set { m_Sortable = value; }
		}

		public DateTime DateCreated { get; set; }

		public DateTime? DateChanged { get; set; }

		public T[] DeletedItems {
			get { return m_DeletedItems.ToArray(); }
		}

		public T[] NewItems {
			get { return m_NewItems.ToArray(); }
		}

		public T[] UpdatedItems {
			get { return m_UpdatedItems.ToArray(); }
		}

		/// <summary>
		/// Returns an array that contains both the New and Updated items.
		/// </summary>
		public T[] NewAndUpdatedItems {
			get {
				List<T> items = new List<T>();
				if ( this.UpdatedItems != null )
					items.AddRange( this.UpdatedItems );
				if ( this.NewItems != null )
					items.AddRange( this.NewItems );
				return items.ToArray();
			}
		}

		protected override ListSortDirection SortDirectionCore {
			get { return m_SortDirection; }
		}

		protected override PropertyDescriptor SortPropertyCore {
			get { return m_Property; }
		}

		public bool SupportsAdvancedSorting {
			get { return true; }
		}

		public ListSortDescriptionCollection SortDescriptions {
			get { return m_SortDescriptions; }
			protected set { m_SortDescriptions = value; }
		}

		public bool SupportsFiltering {
			get { return true; }
		}

		/// <summary>
		/// Filter format: 
		/// <para>
		/// City = @0 and Orders.Count >= @1;'London',10
		/// </para>
		/// </summary>
		public string Filter {
			get { return m_Filter; }
			set {
				if ( m_Filter == value )
					return;
				m_Filter = value;
				RunFilter();
			}
		}

		#endregion

		#region " Constructor(s) "

		public TrackedBindingList()
			: base() {
			this.DateCreated = DateTime.Now;
		}

		public TrackedBindingList( bool sortable )
			: this() {
			this.Sortable = true;
		}

		public TrackedBindingList( IList<T> list )
			: base( list ) {
			this.DateCreated = DateTime.Now;
		}

		public TrackedBindingList( IList<T> list, bool makeEditable )
			: this( ( makeEditable ? new List<T>( list ) : list ), makeEditable, true ) {
		}

		public TrackedBindingList( IList<T> list, bool makeEditable, bool sortable )
			: this( ( makeEditable ? new List<T>( list ) : list ) ) {
			this.Sortable = true;
		}

		#endregion

		#region " Methods "

		public void AddRange( IEnumerable<T> items ) {
			if ( m_OriginalItems != null ) {
				m_OriginalItems.AddRange( items );
			}
			foreach ( T item in items ) {
				this.Add( item );
			}
		}

		public void ApplySort( ListSortDescriptionCollection sorts ) {
			throw new NotImplementedException();
		}

		private void RunFilter() {
			if ( m_OriginalItems == null ) {//looks like a new filter is being applied against an unfiltered list
				m_OriginalItems = new List<T>( this.Items );
			}
			if ( String.IsNullOrEmpty( m_Filter ) ) {
				this.Items.Clear();
				m_OriginalItems.ForEach( e => this.Items.Add( e ) );
				m_OriginalItems = null; //clear out the originals now that they have been reloaded (no need to keep 2 copies of the same thing).
			} else {
				this.Items.Clear();
				string[] parts = m_Filter.Split( ';' );
				if ( parts.Length > 1 ) {
					List<object> values = new List<object>();
					string[] parse = parts[1].Split( ',' );
					for ( int x = 0; x < parse.Length; x++ ) {
						if ( parse[x].Trim().Length > 0 ) {
							if ( parse[x].Contains( '"' ) || parse[x].Contains( '\'' ) ) {//looks like a string value
								values.Add( parse[x].Trim().Trim( '\'' ).Trim( '"' ) );
							} else if ( parse[x].SafeEquals( "True" ) || parse[x].SafeEquals( "False" ) ) {
								values.Add( parse[x].ToBoolean() );
							} else {
								values.Add( parse[x].ToDecimal() );
							}
						}
					}
					IEnumerable<T> filteredItems = m_OriginalItems.AsQueryable().Where( parts[0].Trim(), values.ToArray() );
					foreach ( T i in filteredItems ) {
						this.Items.Add( i );
					}
				}
			}

		}

		public void RemoveFilter() {
			this.Filter = null;
		}

		protected override void ApplySortCore( PropertyDescriptor prop, ListSortDirection direction ) {
			if ( this.Sortable ) {
				// Get list to sort
				List<T> items = this.Items as List<T>;
				// Apply and set the sort, if items to sort
				if ( items != null ) {
					PropertyComparer<T> pc = new PropertyComparer<T>( prop, direction );
					items.Sort( pc );
					m_SortDirection = direction;
					m_Property = prop;
					m_IsSorted = true;
				} else {
					m_IsSorted = false;
				}
				// Let bound controls know they should refresh their views
				this.OnListChanged(
				  new ListChangedEventArgs( ListChangedType.Reset, -1 ) );
			}
		}

		protected override void RemoveSortCore() {
			m_IsSorted = false;
		}

		/// <summary>
		/// Clears out the item tracks (but not the base item list)
		/// </summary>
		public void ClearTracks() {
			m_NewItems.Clear();
			m_DeletedItems.Clear();
			m_UpdatedItems.Clear();
		}

		protected override void ClearItems() {
			if ( m_OriginalItems != null )
				m_OriginalItems.Clear();
			m_NewItems.Clear();
			m_DeletedItems.Clear();
			m_UpdatedItems.Clear();
			base.ClearItems();
		}

		public T[] ToArray() {
			T[] arr = new T[this.Count];
			for ( int x = 0; x < this.Count; x++ )
				arr[x] = base.Items[x];
			return arr;
		}

		public override void CancelNew( int itemIndex ) {
			//if ( itemIndex > -1 && itemIndex < base.Items.Count ) {
			//    T item = base.Items[itemIndex];
			//    RemoveFromChangeSets( item );
			//}
			base.CancelNew( itemIndex );
		}

		protected override void RemoveItem( int index ) {
			if ( index < base.Items.Count ) {
				T item = base.Items[index];
				if ( m_OriginalItems != null )
					m_OriginalItems.Remove( item );
				bool wasNewItem = m_NewItems.Contains( item );
				RemoveFromChangeSets( item );
				if ( wasNewItem == false ) //don't track deleted new items
					m_DeletedItems.Add( item );
			}
			base.RemoveItem( index );
		}

		protected override void OnListChanged( ListChangedEventArgs e ) {
			if ( e.NewIndex > -1 && e.NewIndex < base.Items.Count ) {
				this.DateChanged = DateTime.Now;
				T item = base.Items[e.NewIndex];
				if ( e.ListChangedType == ListChangedType.ItemAdded ) {
					RemoveFromChangeSets( item );
					m_NewItems.Add( item );
					if ( m_OriginalItems != null )
						m_OriginalItems.Add( item );
				} else if ( e.ListChangedType == ListChangedType.ItemChanged ) {
					if ( m_NewItems.Contains( item ) == false ) {
						RemoveFromChangeSets( item );
						m_UpdatedItems.Add( item );
					}
					if ( m_OriginalItems != null && m_OriginalItems.Contains( item ) == false ) {
						m_OriginalItems.Add( item );
					}
				} else if ( e.ListChangedType == ListChangedType.ItemDeleted ) {
					//bool wasNewItem = m_NewItems.Contains( item );
					//RemoveFromChangeSets( item );
					//if ( wasNewItem == false ) //don't track deleted new items
					//    m_DeletedItems.Add( item );
				}
			}
			base.OnListChanged( e );
		}

		private void RemoveFromChangeSets( T item ) {
			if ( m_NewItems.Contains( item ) )
				m_NewItems.Remove( item );
			if ( m_DeletedItems.Contains( item ) )
				m_DeletedItems.Remove( item );
			if ( m_UpdatedItems.Contains( item ) )
				m_UpdatedItems.Remove( item );
		}

		#endregion

	}

}
