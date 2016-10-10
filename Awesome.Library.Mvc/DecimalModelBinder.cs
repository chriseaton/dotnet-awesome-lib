/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Web.Mvc;

namespace Awesome.Library.Mvc {

	/// <summary>
	/// Adds support for parsing more complex decimal values from the a form.
	/// </summary>
	/// <remarks>
	/// You must add the following to the Application_Start method in your global.asax:
	/// ModelBinders.Binders.Add( typeof( decimal ), new PMI.Core.Mvc.DecimalModelBinder() );
	/// </remarks>
	public class DecimalModelBinder : IModelBinder {

		public object BindModel( ControllerContext controllerContext, ModelBindingContext bindingContext ) {
			ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue( bindingContext.ModelName );
			ModelState modelState = new ModelState { Value = valueResult };
			object actualValue = null;
			try {
				if ( !string.IsNullOrWhiteSpace( valueResult.AttemptedValue ) ) {
					actualValue = Decimal.Parse( valueResult.AttemptedValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowParentheses | NumberStyles.AllowThousands | NumberStyles.Integer | NumberStyles.Number | NumberStyles.Currency, CultureInfo.CurrentCulture );
				}
			} catch ( FormatException e ) {
				modelState.Errors.Add( e );
			}

			bindingContext.ModelState.Add( bindingContext.ModelName, modelState );
			return actualValue;
		}

	}

	/*******************************************************
	 * YOU SHOULD ADD THE FOLLOWING TO YOUR JAVASCRIPT 
	 * MAIN FILE IF YOU ARE USING JQUERY VALIDATION
	 *******************************************************/
	//jQuery.extend(jQuery.validator.methods, {
	//    number: function (value, element) {
	//        var result = this.optional(element) || /^\$?([0-9]{1,3},([0-9]{3},)*[0-9]{3}|[0-9]+)(.[0-9][0-9])?$/.test(value);
	//        if (result == false) {
	//            alert(value + ': ' + result);
	//        }
	//        return result;
	//    }
	//});

}
