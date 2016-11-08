using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Awesome.Library.Mvc {

	public static class ModelStateDictionaryExtensions {

		/// <summary>
		/// Returns a simple string dictionary that lists model keys and errors.
		/// </summary>
		public static Dictionary<string, string[]> ToErrorDictionary( this ModelStateDictionary modelState ) {
			return modelState.ToDictionary(
				kvp => kvp.Key,
				kvp => ( from n in kvp.Value.Errors
						 where String.IsNullOrWhiteSpace( n.ErrorMessage ) == false
						 select n.ErrorMessage ).ToArray()
			);
		}

		/// <summary>
		/// Returns a simple string dictionary that lists model keys and errors.
		/// </summary>
		public static Dictionary<string, string[]> ToJsonResult( this ModelStateDictionary modelState ) {
			return new ModelStateJsonResult( modelState.ToDictionary(
				kvp => kvp.Key,
				kvp => ( from n in kvp.Value.Errors
						 where String.IsNullOrWhiteSpace( n.ErrorMessage ) == false
						 select n.ErrorMessage ).ToArray()
			) );
		}

	}
}
