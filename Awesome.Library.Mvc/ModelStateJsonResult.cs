using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awesome.Library.Mvc {

	[Serializable()]
	public class ModelStateJsonResult : Dictionary<string, string[]> {

		#region " Properties "

		public bool Errors {
			get { return this.Values.Any( a => a != null && a.Length > 0 ); }
		}

		#endregion

		#region " Constructor(s) "

		public ModelStateJsonResult() : base() { }

		public ModelStateJsonResult( IDictionary<string, string[]> dictionary ) : base( dictionary ) { }

		#endregion

	}
}
