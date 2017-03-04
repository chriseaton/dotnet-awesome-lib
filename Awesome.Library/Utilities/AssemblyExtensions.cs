/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Awesome.Library.Utilities {

	public static class AssemblyExtensions {

		/// <summary>
		/// Retrieve the Guid of an Assembly
		/// </summary>
		public static Guid GetGuid( this Assembly asm ) {
			string id = GetAssemblyAttributeValue( asm, "System.Runtime.InteropServices.GuidAttribute" );
			if ( String.IsNullOrEmpty( id ) == false ) {
				return new Guid( id );
			}
			return Guid.Empty;
		}

		/// <summary>
		/// Retrieve the copyright of an Assembly
		/// </summary>
		public static string GetCopyright( this Assembly asm ) {
			return GetAssemblyAttributeValue( asm, "System.Reflection.AssemblyCopyrightAttribute" );
		}

		/// <summary>
		/// Retrieve the title of an Assembly
		/// </summary>
		public static string GetTitle( this Assembly asm ) {
			return GetAssemblyAttributeValue( asm, "System.Reflection.AssemblyTitle" );
		}

		/// <summary>
		/// Retrieve the description of an Assembly
		/// </summary>
		public static string GetDescription( this Assembly asm ) {
			return GetAssemblyAttributeValue( asm, "System.Reflection.AssemblyDescription" );
		}

		/// <summary>
		/// Retrieve the company of an Assembly
		/// </summary>
		public static string GetCompany( this Assembly asm ) {
			return GetAssemblyAttributeValue( asm, "System.Reflection.AssemblyCompany" );
		}

		/// <summary>
		/// Retrieve the company of an Assembly
		/// </summary>
		public static string GetProduct( this Assembly asm ) {
			return GetAssemblyAttributeValue( asm, "System.Reflection.AssemblyProduct" );
		}

		/// <summary>
		/// Retrieve the attribute value of an Assembly
		/// </summary>
		private static string GetAssemblyAttributeValue( this Assembly asm, string attributeTypeName ) {
			IList<CustomAttributeData> asmAttributes = CustomAttributeData.GetCustomAttributes( asm );
			if ( asmAttributes != null && asmAttributes.Count > 0 ) {
				foreach ( CustomAttributeData cad in asmAttributes ) {
					string attrName = cad.ToString();
					if ( cad.ConstructorArguments != null && cad.ConstructorArguments.Count > 0 ) {
						if ( attrName.Contains( attributeTypeName ) ) {
							return cad.ConstructorArguments[0].Value.SafeToString();
						}
					}
				}
			}
			return String.Empty;
		}

	}

}
