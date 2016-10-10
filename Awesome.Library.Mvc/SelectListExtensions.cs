/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace Awesome.Library.Mvc {

	public static class SelectListExtensions {

		/// <summary>
		/// Returns a SelectListItem enumeration for the given dictionary
		/// </summary>
		public static IEnumerable<SelectListItem> ToSelectList( this IDictionary dict, params SelectListItem[] prefixItems ) {
			return ToSelectList( dict, null, prefixItems );
		}

		/// <summary>
		/// Returns a SelectListItem enumeration for the given dictionary
		/// </summary>
		public static IEnumerable<SelectListItem> ToSelectList( this IDictionary dict, object selected, params SelectListItem[] prefixItems ) {
			if ( prefixItems != null ) {
				foreach ( var item in prefixItems ) {
					yield return item;
				}
			}
			if ( dict != null ) {
				foreach ( var k in dict.Keys ) {
					yield return new SelectListItem() { Text = dict[k].ToString(), Value = k.ToString(), Selected = ( selected != null && selected.ToString() == k.ToString() ) };
				}
			}
		}

		/// <summary>
		/// Returns a SelectListItem enumeration for the given enumerable list of items
		/// </summary>
		public static IEnumerable<SelectListItem> ToSelectList<TModel, TProperty>( this IEnumerable<TModel> list, Expression<Func<TModel, TProperty>> textExpression, Expression<Func<TModel, TProperty>> valueExpression, params SelectListItem[] prefixItems ) {
			return ToSelectList<TModel, TProperty>( list, textExpression, valueExpression, null, prefixItems );
		}

		/// <summary>
		/// Returns a SelectListItem enumeration for the given enumerable list of items
		/// </summary>
		public static IEnumerable<SelectListItem> ToSelectList<TModel, TProperty>( this IEnumerable<TModel> list, Expression<Func<TModel, TProperty>> textExpression, Expression<Func<TModel, TProperty>> valueExpression, object selected, params SelectListItem[] prefixItems ) {
			if ( prefixItems != null ) {
				foreach ( var item in prefixItems ) {
					yield return item;
				}
			}
			if ( list != null ) {
				Func<TModel, TProperty> textF = textExpression.Compile();
				Func<TModel, TProperty> valueF = valueExpression.Compile();
				foreach ( var item in list ) {
					var value = valueF( item );
					yield return new SelectListItem() { Text = textF( item ).ToString(), Value = value.ToString(), Selected = ( selected != null && selected.ToString() == value.ToString() ) };
				}
			}
		}

	}

}
