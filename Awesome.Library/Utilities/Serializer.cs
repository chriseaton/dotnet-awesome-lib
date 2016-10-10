/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://gitlab.com/chriseaton/awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Awesome.Library.Utilities {

	public enum SerializationMode {
		None = 0,
		Binary = 1,
		DataContract = 2,
		XML = 3
	}

	public static class Serializer {

		/// <summary>
		/// XML serializes an object to a file on disk
		/// </summary>
		/// <typeparam name="T">The object type</typeparam>
		/// <param name="value">Object to be serialized</param>
		/// <param name="fileName">File in which to write the serialized object</param>
		public static void ToXML<T>( T value, string fileName ) {
			XmlSerializer x = new XmlSerializer( typeof( T ) );
			TextWriter w = new StreamWriter( fileName );
			x.Serialize( w, value );
			w.Close();
		}

		/// <summary>
		/// XML Deserializes a xml file representing an object
		/// </summary>
		/// <typeparam name="T">The object type</typeparam>
		/// <param name="fileName">File to load the object from</param>
		/// <returns>Deserialized object from file</returns>
		public static T FromXML<T>( string fileName ) {
			try {
				XmlSerializer x = new XmlSerializer( typeof( T ) );
				TextReader w = new StreamReader( fileName );
				T s = (T)x.Deserialize( w );
				w.Close();
				return s;
			} catch ( FileNotFoundException ) {
				return default( T );
			}
		}

		/// <summary>
		/// Converts a serializable object into a serialized representation of the object
		/// </summary>
		/// <param name="value">The object to be serialized</param>
		/// <returns>Byte array representation of the object</returns>
		public static byte[] ToBytes( object value, SerializationMode mode ) {
			MemoryStream ms = new MemoryStream();
			if ( mode == SerializationMode.Binary ) {
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize( ms, value );
			} else if ( mode == SerializationMode.DataContract ) {
				DataContractSerializer x = new DataContractSerializer( value.GetType() );
				x.WriteObject( ms, value );
			} else if ( mode == SerializationMode.XML ) {
				XmlSerializer x = new XmlSerializer( value.GetType() );
				x.Serialize( ms, value );
			}
			ms.Seek( 0, SeekOrigin.Begin );
			return ms.ToArray();
		}

		/// <summary>
		/// Writes the serialized values to a file
		/// </summary>
		/// <param name="value">The object to be serialized</param>
		/// <returns>Byte array representation of the object</returns>
		public static void ToFile( object value, string fileName, SerializationMode mode ) {
			Stream streamWrite = File.Create( fileName );
			if ( mode == SerializationMode.Binary ) {
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize( streamWrite, value );
			} else if ( mode == SerializationMode.DataContract ) {
				DataContractSerializer x = new DataContractSerializer( value.GetType() );
				x.WriteObject( streamWrite, value );
			} else if ( mode == SerializationMode.XML ) {
				XmlSerializer x = new XmlSerializer( value.GetType() );
				x.Serialize( streamWrite, value );
			}
			streamWrite.Close();
		}

		/// <summary>
		/// Converts a serialized object (represented in bytes), from a file, back to a programmable object
		/// </summary>
		/// <typeparam name="T">The object type</typeparam>
		/// <param name="objBytes">The bytes containing the serialized object</param>
		/// <returns>Object from bytes.</returns>
		public static T FromFile<T>( string filename, SerializationMode mode ) {
			T retVal = default( T );
			if ( File.Exists( filename ) ) {
				Stream streamRead = File.OpenRead( filename );
				if ( mode == SerializationMode.Binary ) {
					IFormatter formatter = new BinaryFormatter();
					retVal = (T)formatter.Deserialize( streamRead );
				} else if ( mode == SerializationMode.DataContract ) {
					DataContractSerializer x = new DataContractSerializer( typeof( T ) );
					retVal = (T)x.ReadObject( streamRead );
				} else if ( mode == SerializationMode.XML ) {
					XmlSerializer x = new XmlSerializer( typeof( T ) );
					retVal = (T)x.Deserialize( streamRead );
				}
			}
			return retVal;
		}

		/// <summary>
		/// Converts a serialized object (represented in bytes) back to a programmable object
		/// </summary>
		/// <typeparam name="T">The object type</typeparam>
		/// <param name="objBytes">The bytes containing the serialized object</param>
		/// <returns>Object from bytes.</returns>
		public static T FromBytes<T>( byte[] objBytes, SerializationMode mode ) {
			MemoryStream ms = new MemoryStream( objBytes );
			T retVal = default( T );
			ms.Seek( 0, SeekOrigin.Begin );
			if ( mode == SerializationMode.Binary ) {
				IFormatter formatter = new BinaryFormatter();
				retVal = (T)formatter.Deserialize( ms );
			} else if ( mode == SerializationMode.DataContract ) {
				DataContractSerializer x = new DataContractSerializer( typeof( T ) );
				retVal = (T)x.ReadObject( ms );
			} else if ( mode == SerializationMode.XML ) {
				XmlSerializer x = new XmlSerializer( typeof( T ) );
				retVal = (T)x.Deserialize( ms );
			}
			return retVal;
		}

		/// <summary>
		/// Converts a serialized object (represented in bytes) back to a programmable object
		/// </summary>
		/// <param name="objType">The type to deserialize to</param>
		/// <param name="objBytes">The bytes containing the serialized object</param>
		/// <returns>Object from bytes.</returns>
		public static object FromBytes( Type objType, byte[] objBytes, SerializationMode mode ) {
			MemoryStream ms = new MemoryStream( objBytes );
			object retVal = null;
			ms.Seek( 0, SeekOrigin.Begin );
			if ( mode == SerializationMode.Binary ) {
				IFormatter formatter = new BinaryFormatter();
				retVal = formatter.Deserialize( ms );
			} else if ( mode == SerializationMode.DataContract ) {
				DataContractSerializer x = new DataContractSerializer( objType );
				retVal = x.ReadObject( ms );
			} else if ( mode == SerializationMode.XML ) {
				XmlSerializer x = new XmlSerializer( objType );
				retVal = x.Deserialize( ms );
			}
			return retVal;
		}

		/// <summary>
		/// Attempts to clone an object via serialization
		/// </summary>
		public static T Clone<T>( T obj, SerializationMode mode ) {
			if ( obj != null ) {
				byte[] oBytes = Serializer.ToBytes( obj, mode );
				return (T)Serializer.FromBytes( typeof( object ), oBytes, mode );
			}
			return default( T );
		}

	}

}
