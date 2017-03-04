/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Management;

namespace Awesome.Library.Utilities {

	public static class Execution {

		#region " Properties "

		/// <summary>
		/// Returns true if the debug flag is on
		/// </summary>
		public static bool DebugMode {
			get {
				if ( Execution.InTestServer ) {
					return true;
				} else {
#if DEBUG
					return true;
#else
			return false;
#endif
				}
			}
		}

		/// <summary>
		/// Returns true if code is executing in the designer
		/// </summary>
		public static bool InDesigner {
			get { return ( System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv" ); }
			//get { return LicenseManager.UsageMode == LicenseUsageMode.Designtime; }
		}

		public static bool InTestServer {
			get {
				if ( System.Diagnostics.Process.GetCurrentProcess().ProcessName == "WebDev.WebServer" ) {
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets a wide range of current system statistics
		/// </summary>
		public static string SystemSummary {
			get {
				StringBuilder sb = new StringBuilder();
				sb.AppendLine( "OS Version: " + System.Environment.OSVersion.SafeToString() );
				sb.AppendLine( "Processor Count: " + System.Environment.ProcessorCount.SafeToString() );
				sb.AppendLine( "Working Set: " + System.Environment.WorkingSet.SafeToString( "N0" ) );
				sb.AppendLine( "Boot Mode: " + SystemInformation.BootMode.SafeToString() );
				sb.AppendLine( "Network Connection: " + SystemInformation.Network.SafeToString() );
				sb.AppendLine( "Primary Monitor Size: " + SystemInformation.PrimaryMonitorSize.SafeToString() );
				sb.AppendLine( "Working Area: " + SystemInformation.WorkingArea.SafeToString() );
				sb.AppendLine( "Terminal Server Session: " + SystemInformation.TerminalServerSession.SafeToString() );
				sb.AppendLine( "User Domain Name: " + SystemInformation.UserDomainName.SafeToString() );
				sb.AppendLine( "Battery Life Remaining: " + SystemInformation.PowerStatus.BatteryLifePercent.SafeToString( "P2" ) );
				sb.AppendLine( "High Contrast Mode: " + SystemInformation.HighContrast.SafeToString() );
				return sb.ToString();
			}
		}

		/// <summary>
		/// Returns TRUE if the operating system is x64 compatible.
		/// </summary>
		public static bool IsX64OperatingSystem {
			get {
				try {
					ManagementClass class1 = new ManagementClass( "Win32_OperatingSystem" );
					foreach ( ManagementObject ob in class1.GetInstances() ) {
						int index = 1;
						foreach ( PropertyData pd in ob.Properties ) {
							if ( pd.Name == "OSArchitecture" )
								return ( pd.Value.SafeToString().ToLower() == "64-bit" );
							index++;
						}
					}
				} catch { }
				return false;
			}
		}

		#endregion

		#region " Methods "

		/// <summary>
		/// Writes a timestamped line to the debug output stream
		/// </summary>
		public static void DebugOut( Exception ex ) {
			Debug.WriteLine( DateTime.Now.ToString( "MM/dd/yyyy hh:mm:ss.ffff tt" ) + ":\n" + ex.ToString(), "Error" );
		}

		/// <summary>
		/// Writes a timestamped line to the debug output stream
		/// </summary>
		public static void DebugOut( string message ) {
			Debug.WriteLine( DateTime.Now.ToString( "MM/dd/yyyy hh:mm:ss.ffff tt" ) + ": " + message );
		}

		/// <summary>
		/// Writes a timestamped line to the debug output stream
		/// </summary>
		public static void DebugOut( string message, DateTime timestamp ) {
			Debug.WriteLine( timestamp.ToString( "MM/dd/yyyy hh:mm:ss.ffff tt" ) + ": " + message );
		}

		/// <summary>
		/// Writes a timestamped line to the trace output stream
		/// </summary>
		public static void TraceOut( Exception ex ) {
			Trace.WriteLine( DateTime.Now.ToString( "MM/dd/yyyy hh:mm:ss.ffff tt" ) + ":\n" + ex.ToString(), "Error" );
		}

		/// <summary>
		/// Writes a timestamped line to the trace output stream
		/// </summary>
		public static void TraceOut( string message ) {
			Trace.WriteLine( DateTime.Now.ToString( "MM/dd/yyyy hh:mm:ss.ffff tt" ) + ": " + message );
		}

		/// <summary>
		/// Writes a timestamped line to the trace output stream
		/// </summary>
		public static void TraceOut( string message, DateTime timestamp ) {
			Trace.WriteLine( timestamp.ToString( "MM/dd/yyyy hh:mm:ss.ffff tt" ) + ": " + message );
		}

		/// <summary>
		/// Tries to get the execution path to the default browser on the system.
		/// </summary>
		public static string GetDefaultBrowserPath() {
			string key = @"http\shell\open\command";
			Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey( key, false );
			string path = registryKey.GetValue( null, null ).SafeToString();
			// get default browser path
			char chPathEnd = ' ';
			int diff = 0;
			if ( path[0] == '"' ) {
				chPathEnd = '"';
				diff = 1;
			}
			for ( int i = 1; i < path.Length; i++ )
				if ( path[i] == chPathEnd )
					return path.Substring( diff, i - diff );
			return path;
		}

		#endregion

	}

}
