/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Resources;

namespace Awesome.Library.Utilities {

	/// <summary>
	/// Class used to retrieve embedded resource files
	/// </summary>
	public static class Resourcer {

		/// <summary>
		/// Retrieves the content of an embedded resource file as a string
		/// </summary>
		/// <param name="resourceName">The full name of the resource in the current assembly</param>
		/// <returns>A string containing the contents of an embedded resource file. NULL if there is a problem locating or processing the resource.</returns>
		internal static string GetStringFromResources( string resourceName ) {
			Assembly assem = Assembly.GetExecutingAssembly();
			using ( Stream stream = assem.GetManifestResourceStream( resourceName ) ) {
				try {
					using ( StreamReader reader = new StreamReader( stream ) ) {
						return reader.ReadToEnd();
					}
				} catch ( Exception ) { }
			}
			return null;
		}

		/// <summary>
		/// Gets a string from an owning type by name and for culture (defaults if not found)
		/// </summary>
		public static string GetString( string resName, Type owningType, CultureInfo ci ) {
			ResourceManager rm = new ResourceManager( owningType );
			ResourceSet x = rm.GetResourceSet( ci, true, true );
			return x.GetString( resName );
		}

		/// <summary>
		/// Retrieves the content of an embedded resource file as a string
		/// </summary>
		/// <param name="resourceName">The full name of the resource in the current assembly</param>
		/// <param name="targetAsm">The assembly to load the resource from</param>
		/// <returns>A string containing the contents of an embedded resource file. NULL if there is a problem locating or processing the resource.</returns>
		public static string GetStringFromResources( string resourceName, Assembly targetAsm ) {
			Assembly assem = targetAsm;
			using ( Stream stream = assem.GetManifestResourceStream( resourceName ) ) {
				try {
					using ( StreamReader reader = new StreamReader( stream ) ) {
						return reader.ReadToEnd();
					}
				} catch ( Exception ) { }
			}
			return null;
		}

		/// <summary>
		/// Gets all objects of a specific type from a specified embedded resource.
		/// Objects are returned in array sorted Alphabetically by their resource name.
		/// </summary>
		/// <typeparam name="T">The object type to seek</typeparam>
		/// <param name="resourceName">The full resource name to scan</param>
		/// <param name="targetAsm">The assembly containing the resource</param>
		/// <returns>Objects of specified type 'T'</returns>
		public static T[] GetObjects<T>( string resourceName, Assembly targetAsm ) {
			SortedDictionary<string, T> sortedDic = new SortedDictionary<string, T>();
			List<T> arr = new List<T>();
			using ( Stream stream = targetAsm.GetManifestResourceStream( resourceName ) ) {
				ResourceReader reader = new ResourceReader( stream );
				IDictionaryEnumerator id = reader.GetEnumerator();
				foreach ( DictionaryEntry d in reader ) {
					if ( d.Value is T ) {
						sortedDic.Add( d.Key.ToString(), (T)d.Value );
					}
				}
				reader.Close();
			}
			foreach ( T obj in sortedDic.Values ) {
				arr.Add( obj );
			}
			return arr.ToArray();
		}

		/// <summary>
		/// Gets an object from the resource stream by specified type and key
		/// </summary>
		/// <typeparam name="T">The object type to seek</typeparam>
		/// <param name="resourceName">The full resource name to scan</param>
		/// <param name="targetAsm">The assembly containing the resource</param>
		/// <param name="key">The key to match (case insensitive)</param>
		/// <returns>First object of specified type 'T' with matching key value</returns>
		public static T GetObjectByKey<T>( string resourceName, string key, Assembly targetAsm ) {
			T obj = default( T );
			using ( Stream stream = targetAsm.GetManifestResourceStream( resourceName ) ) {
				ResourceReader reader = new ResourceReader( stream );
				IDictionaryEnumerator id = reader.GetEnumerator();
				foreach ( DictionaryEntry d in reader ) {
					if ( d.Value is T && d.Key.ToString().ToLowerInvariant().Trim() == key.ToLowerInvariant().Trim() ) {
						obj = (T)d.Value;
						break;
					}
				}
				reader.Close();
			}
			return obj;
		}

	}

}
