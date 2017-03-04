/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Drawing;
using System.Drawing.Printing;
using Microsoft.Win32;

namespace Awesome.Library.Utilities {

	[RegistryPermission( SecurityAction.Demand, ViewAndModify = "HKEY_CURRENT_USER\\Software\\Microsoft\\Internet Explorer\\PageSetup" )]
	public class HtmlPrinter {

		private static WebBrowser m_Browser = null;

		private string OriginalFooter = null;
		private string OriginalHeader = null;
		private Margins OriginalMargins = null;

		private Margins m_Margins = new Margins( 50, 50, 75, 75 );
		private int m_FontZoom = 0;

		#region " Properties "

		protected static WebBrowser Browser {
			get {
				if ( HtmlPrinter.m_Browser == null ) {
					m_Browser = new WebBrowser();
					m_Browser.AllowWebBrowserDrop = false;
					m_Browser.CausesValidation = false;
					m_Browser.ScriptErrorsSuppressed = true;
					m_Browser.ScrollBarsEnabled = false;
					m_Browser.WebBrowserShortcutsEnabled = false;
					m_Browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler( Browser_DocumentCompleted );
				}
				return HtmlPrinter.m_Browser;
			}
			set { HtmlPrinter.m_Browser = value; }
		}

		public string Header { get; set; }

		public string Footer { get; set; }

		public Margins Margins { get; set; }

		public string Html {
			get { return HtmlPrinter.Browser.DocumentText; }
			set {
				HtmlPrinter.LoadingComplete = false;
				string htm = value;
				//apply zoom factor;
				string customStyle = "<style>";
				int headIndex = value.ToLower().IndexOf( "</head" );
				if ( this.FontZoom != 0 ) {
					customStyle += "body { zoom: " + ( 100 + ( this.FontZoom * 12 ) ).ToString() + "%; }" + Environment.NewLine;
				}
				if ( this.Grayscale ) {
					customStyle += "*, body, div, font, td, a, li, span { color: #000 !important; }" + Environment.NewLine;
				}
				if ( customStyle.Length > "<style>".Length ) {
					customStyle += "</style>";
					value = value.Insert( headIndex, customStyle );
				}
				HtmlPrinter.Browser.DocumentText = value;

			}
		}

		/// <summary>
		/// This property must be set before setting the 'Html' property. It requires a proper opening and closing head tag.
		/// </summary>
		public int FontZoom {
			get { return m_FontZoom; }
			set { m_FontZoom = value.Minimum( -8 ).Maximum( 8 ); }
		}

		/// <summary>
		/// This property must be set before setting the 'Html' property. It requires a proper opening and closing head tag.
		/// </summary>
		public bool Grayscale { get; set; }

		protected static bool LoadingComplete { get; set; }

		#endregion

		#region " Constructor(s) "

		public HtmlPrinter() { }

		public HtmlPrinter( string html ) {
			this.Html = html;
		}

		public HtmlPrinter( int fontZoom, bool grayscale, string html ) {
			this.FontZoom = fontZoom;
			this.Grayscale = grayscale;
			this.Html = html;
		}

		#endregion

		#region " UI Events "

		private static void Browser_DocumentCompleted( object sender, WebBrowserDocumentCompletedEventArgs e ) {
			HtmlPrinter.LoadingComplete = true;
		}

		#endregion

		#region " Methods "

		public void Print() {
			EnsureLoaded();
			HtmlPrinter.Browser.Print();
			Cleanup();
		}

		public void PrintPreview() {
			EnsureLoaded();
			HtmlPrinter.Browser.ShowPrintPreviewDialog();
			Cleanup();
		}

		public void PrintDialog() {
			EnsureLoaded();
			HtmlPrinter.Browser.ShowPrintDialog();
			Cleanup();
		}

		private void EnsureLoaded() {
			WaitForBrowserLoad();
			RegistryKey oKey = Registry.CurrentUser.OpenSubKey( "Software\\Microsoft\\Internet Explorer\\PageSetup", true );
			//set original values
			OriginalFooter = (string)oKey.GetValue( "footer", null );
			OriginalHeader = (string)oKey.GetValue( "header", null );
			OriginalMargins = new Margins();
			OriginalMargins.Left = (int)( oKey.GetValue( "margin_left", "0.75" ).ToString().ToSingle() * 100 );
			OriginalMargins.Right = (int)( oKey.GetValue( "margin_right", "0.75" ).ToString().ToSingle() * 100 );
			OriginalMargins.Top = (int)( oKey.GetValue( "margin_top", "0.75" ).ToString().ToSingle() * 100 );
			OriginalMargins.Bottom = (int)( oKey.GetValue( "margin_bottom", "0.75" ).ToString().ToSingle() * 100 );
			//write in temporary settings
			oKey.SetValue( "footer", this.Footer ?? "" );
			oKey.SetValue( "header", this.Header ?? "" );
			if ( this.Margins != null ) {
				oKey.SetValue( "margin_left", ( this.Margins.Left / 100F ).ToString() );
				oKey.SetValue( "margin_right", ( this.Margins.Right / 100F ).ToString() );
				oKey.SetValue( "margin_top", ( this.Margins.Top / 100F ).ToString() );
				oKey.SetValue( "margin_bottom", ( this.Margins.Bottom / 100F ).ToString() );
			}
			oKey.Close();
		}

		private void Cleanup() {
			RegistryKey oKey = Registry.CurrentUser.OpenSubKey( "Software\\Microsoft\\Internet Explorer\\PageSetup", true );
			oKey.SetValue( "footer", OriginalFooter ?? "" );
			oKey.SetValue( "header", OriginalHeader ?? "" );
			if ( OriginalMargins != null ) {
				oKey.SetValue( "margin_left", ( OriginalMargins.Left / 100F ).ToString() );
				oKey.SetValue( "margin_right", ( OriginalMargins.Right / 100F ).ToString() );
				oKey.SetValue( "margin_top", ( OriginalMargins.Top / 100F ).ToString() );
				oKey.SetValue( "margin_bottom", ( OriginalMargins.Bottom / 100F ).ToString() );
			}
			oKey.Close();
		}

		protected void WaitForBrowserLoad() {
			if ( HtmlPrinter.LoadingComplete ) {
				return;
			}
			for ( int x = 0; x < 150 && !HtmlPrinter.LoadingComplete; x++ ) {
				System.Threading.Thread.Sleep( 100 );
				Application.DoEvents();
			}
		}

		#endregion

	}

}
