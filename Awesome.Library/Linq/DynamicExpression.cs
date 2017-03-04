//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Awesome.Library.Linq {

	public static class DynamicExpression {

		public static Expression Parse( Type resultType, string expression, params object[] values ) {
			ExpressionParser parser = new ExpressionParser( null, expression, values );
			return parser.Parse( resultType );
		}

		public static LambdaExpression ParseLambda( Type itType, Type resultType, string expression, params object[] values ) {
			return ParseLambda( new ParameterExpression[] { Expression.Parameter( itType, "" ) }, resultType, expression, values );
		}

		public static LambdaExpression ParseLambda( ParameterExpression[] parameters, Type resultType, string expression, params object[] values ) {
			ExpressionParser parser = new ExpressionParser( parameters, expression, values );
			return Expression.Lambda( parser.Parse( resultType ), parameters );
		}

		public static Expression<Func<T, S>> ParseLambda<T, S>( string expression, params object[] values ) {
			return (Expression<Func<T, S>>)ParseLambda( typeof( T ), typeof( S ), expression, values );
		}

		public static Type CreateClass( params DynamicProperty[] properties ) {
			return ClassFactory.Instance.GetDynamicClass( properties );
		}

		public static Type CreateClass( IEnumerable<DynamicProperty> properties ) {
			return ClassFactory.Instance.GetDynamicClass( properties );
		}

	}

}
