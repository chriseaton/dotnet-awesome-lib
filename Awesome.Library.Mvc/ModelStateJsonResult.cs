using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awesome.Library.Mvc {

	[Serializable()]
	public class ModelStateJsonResult {

		#region " Properties "

		public string Message {
			get { return String.Format( "{0} Error(s) resulted from processing the provided model. See the \"Errors\" property for details.", this.ErrorCount ); }
		}

		public int ErrorCount {
			get { return this.Errors.Count; }
		}

		public Dictionary<string, string[]> Errors { get; set; }

		#endregion

		#region " Constructor(s) "

		public ModelStateJsonResult( Dictionary<string, string[]> errors ) {
			this.Errors = errors ?? new Dictionary<string, string[]>();
		}

		#endregion

	}
}
