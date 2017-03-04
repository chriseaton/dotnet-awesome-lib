/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Drawing.Printing;
using System.Text;

/***********************************************************
 * BASED OFF THE CODE ON http://www.codeproject.com/KB/grid/PrintDataGridView.aspx
 * By Afrasiab Cheraghi; MODIFIED By: Christopher Eaton
 **************************************************************/

namespace Awesome.Library.Utilities {

	public class DataGridViewPrinter {

		private StringFormat StrFormat;  // Holds content of a TextBox Cell to write by DrawString
		private StringFormat StrFormatComboBox; // Holds content of a Boolean Cell to write by DrawImage
		private Button CellButton;       // Holds the Contents of Button Cell
		private CheckBox CellCheckBox;   // Holds the Contents of CheckBox Cell 
		private ComboBox CellComboBox;   // Holds the Contents of ComboBox Cell
		private int TotalWidth;          // Summation of Columns widths
		private int RowPos;              // Position of currently printing row 
		private bool NewPage;            // Indicates if a new page reached
		private int PageNo;              // Number of pages to print
		private ArrayList ColumnLefts = new ArrayList();  // Left Coordinate of Columns
		private ArrayList ColumnWidths = new ArrayList(); // Width of Columns
		private ArrayList ColumnTypes = new ArrayList();  // DataType of Columns
		private int CellHeight;          // Height of DataGrid Cell
		private int RowsPerPage;         // Number of Rows per Page
		private PrintDocument m_PrintDoc = new PrintDocument();  // PrintDocumnet Object used for printing
		private string m_PrintTitle = "";  // Header of pages
		private DataGridView dgv;        // Holds DataGridView Object to print its contents
		private List<string> m_SelectedColumns = new List<string>();   // The Columns Selected by user to print.
		private List<string> AvailableColumns = new List<string>();  // All Columns avaiable in DataGrid 
		private bool m_PrintAllRows = true;   // True = print all rows,  False = print selected rows    
		private bool m_FitToPageWidth = true; // True = Fits selected columns to page width ,  False = Print columns as showed    
		private int HeaderHeight = 0;

		#region " Properties "

		public string Title {
			get { return m_PrintTitle; }
			set { m_PrintTitle = value; }
		}

		public PrintDocument PrintDocument {
			get { return m_PrintDoc; }
		}

		public string DateTimeStringFormat { get; set; }

		/// <summary>
		///  True = print all rows,  False = print selected rows  
		/// </summary>
		public bool PrintAllRows {
			get { return m_PrintAllRows; }
			set { m_PrintAllRows = value; }
		}

		/// <summary>
		/// True = Fits selected columns to page width ,  False = Print columns as showed    
		/// </summary>
		public bool FitToPageWidth {
			get { return m_FitToPageWidth; }
			set { m_FitToPageWidth = value; }
		}

		public List<string> SelectedColumns {
			get { return m_SelectedColumns; }
			set { m_SelectedColumns = value; }
		}

		public Font BodyFont { get; set; }

		public Font HeaderFont { get; set; }

		#endregion

		#region " Constructor(s) "

		public DataGridViewPrinter() {
			RowsPerPage = 0;
			m_PrintDoc.BeginPrint += new PrintEventHandler( PrintDoc_BeginPrint );
			m_PrintDoc.PrintPage += new PrintPageEventHandler( PrintDoc_PrintPage );
			this.BodyFont = new Font( "Arial", 10, FontStyle.Regular, GraphicsUnit.Point );
			this.HeaderFont = new Font( "Arial", 10, FontStyle.Bold, GraphicsUnit.Point );
		}

		public DataGridViewPrinter( DataGridView dataGridViewToPrint, string pageTitle ) : this( dataGridViewToPrint, pageTitle, true, true, null ) { }

		public DataGridViewPrinter( DataGridView dataGridViewToPrint, string pageTitle, bool printAllRows, bool fitToWidth, IEnumerable<string> selectedColumns )
			: this() {
			// Getting DataGridView object to print
			dgv = dataGridViewToPrint;
			// Getting all Coulmns Names in the DataGridView
			AvailableColumns.Clear();
			foreach ( DataGridViewColumn c in dgv.Columns ) {
				if ( !c.Visible ) continue;
				AvailableColumns.Add( c.HeaderText );
			}
			m_PrintTitle = pageTitle;
			m_PrintAllRows = printAllRows;
			m_FitToPageWidth = fitToWidth;
			if ( selectedColumns != null ) {
				m_SelectedColumns = new List<string>( selectedColumns );
			} else { //all columns
				m_SelectedColumns = new List<string>();
				foreach ( DataGridViewColumn GridCol in dgv.Columns ) {
					m_SelectedColumns.Add( GridCol.HeaderText );
				}
			}
			this.m_PrintDoc.DocumentName = pageTitle;
			this.BodyFont = dataGridViewToPrint.Font;
			this.HeaderFont = new Font( dataGridViewToPrint.Font, FontStyle.Bold );
		}

		#endregion

		#region " Methods "

		private void PrintDoc_BeginPrint( object sender, PrintEventArgs e ) {
			// Formatting the Content of Text Cell to print
			StrFormat = new StringFormat();
			StrFormat.Alignment = StringAlignment.Near;
			StrFormat.LineAlignment = StringAlignment.Center;
			StrFormat.Trimming = StringTrimming.EllipsisCharacter;
			// Formatting the Content of Combo Cells to print
			StrFormatComboBox = new StringFormat();
			StrFormatComboBox.LineAlignment = StringAlignment.Center;
			StrFormatComboBox.FormatFlags = StringFormatFlags.NoWrap;
			StrFormatComboBox.Trimming = StringTrimming.EllipsisCharacter;
			ColumnLefts.Clear();
			ColumnWidths.Clear();
			ColumnTypes.Clear();
			CellHeight = 0;
			RowsPerPage = 0;
			HeaderHeight = 0;
			// For various column types
			CellButton = new Button();
			CellCheckBox = new CheckBox();
			CellComboBox = new ComboBox();
			// Calculating Total Widths
			TotalWidth = 0;
			foreach ( DataGridViewColumn GridCol in dgv.Columns ) {
				if ( !GridCol.Visible ) continue;
				if ( !this.m_SelectedColumns.Contains( GridCol.HeaderText ) ) continue;
				TotalWidth += GridCol.Width;
			}
			PageNo = 1;
			NewPage = true;
			RowPos = 0;
		}

		private void PrintDoc_PrintPage( object sender, PrintPageEventArgs e ) {
			int tmpWidth, i;
			int tmpTop = e.MarginBounds.Top;
			int tmpLeft = e.MarginBounds.Left;
			// Before starting first page, it saves Width & Height of Headers and CoulmnType
			if ( PageNo == 1 ) {
				foreach ( DataGridViewColumn GridCol in dgv.Columns ) {
					if ( !GridCol.Visible ) continue;
					// Skip if the current column not selected
					if ( !this.m_SelectedColumns.Contains( GridCol.HeaderText ) ) continue;
					// Detemining whether the columns are fitted to page or not.
					if ( m_FitToPageWidth )
						tmpWidth = (int)( Math.Floor( (double)( (double)GridCol.Width /
								   (double)TotalWidth * (double)TotalWidth *
								   ( (double)e.MarginBounds.Width / (double)TotalWidth ) ) ) );
					else
						tmpWidth = GridCol.Width;

					int tempHH = (int)( e.Graphics.MeasureString( GridCol.HeaderText,
								GridCol.InheritedStyle.Font, tmpWidth ).Height ) + 11;
					if ( HeaderHeight < tempHH )
						HeaderHeight = tempHH;
					// Save width & height of headres and ColumnType
					ColumnLefts.Add( tmpLeft );
					ColumnWidths.Add( tmpWidth );
					ColumnTypes.Add( GridCol.GetType() );
					tmpLeft += tmpWidth;
				}
			}
			// Printing Current Page, Row by Row
			while ( RowPos <= dgv.Rows.Count - 1 ) {
				DataGridViewRow GridRow = dgv.Rows[RowPos];
				if ( GridRow.IsNewRow || ( !m_PrintAllRows && !GridRow.Selected ) ) {
					RowPos++;
					continue;
				}
				CellHeight = GridRow.Height;
				if ( tmpTop + CellHeight >= e.MarginBounds.Height + e.MarginBounds.Top ) {
					DrawFooter( e, RowsPerPage );
					NewPage = true;
					PageNo++;
					e.HasMorePages = true;
					return;
				} else {
					if ( NewPage ) {
						// Draw Header
						e.Graphics.DrawString( m_PrintTitle, this.HeaderFont, Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
						e.Graphics.MeasureString( m_PrintTitle, this.HeaderFont, e.MarginBounds.Width ).Height - 13 );
						String s = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();
						e.Graphics.DrawString( s, this.HeaderFont,
								Brushes.Black, e.MarginBounds.Left + ( e.MarginBounds.Width -
								e.Graphics.MeasureString( s, this.HeaderFont, e.MarginBounds.Width ).Width ), e.MarginBounds.Top -
								e.Graphics.MeasureString( m_PrintTitle, this.HeaderFont, e.MarginBounds.Width ).Height - 13 );
						// Draw Columns
						tmpTop = e.MarginBounds.Top;
						i = 0;
						foreach ( DataGridViewColumn GridCol in dgv.Columns ) {
							if ( !GridCol.Visible ) continue;
							if ( !this.m_SelectedColumns.Contains( GridCol.HeaderText ) )
								continue;
							e.Graphics.FillRectangle( new SolidBrush( Color.Gainsboro ),
								new Rectangle( (int)ColumnLefts[i], tmpTop,
								(int)ColumnWidths[i], HeaderHeight ) );
							e.Graphics.DrawRectangle( Pens.Gray,
								new Rectangle( (int)ColumnLefts[i], tmpTop,
								(int)ColumnWidths[i], HeaderHeight ) );
							e.Graphics.DrawString( GridCol.HeaderText, GridCol.InheritedStyle.Font,
								new SolidBrush( GridCol.InheritedStyle.ForeColor ),
								new RectangleF( (int)ColumnLefts[i], tmpTop,
								(int)ColumnWidths[i], HeaderHeight ), StrFormat );
							i++;
						}
						NewPage = false;
						tmpTop += HeaderHeight;
					}
					// Draw Columns Contents
					i = 0;
					foreach ( DataGridViewCell cell in GridRow.Cells ) {
						if ( cell.Visible ) {
							if ( ( (Type)ColumnTypes[i] ).Name == "DataGridViewTextBoxColumn" || ( (Type)ColumnTypes[i] ).Name == "DataGridViewLinkColumn" ) {
								string value = String.Empty;
								if ( cell.Value is DateTime && String.IsNullOrEmpty( this.DateTimeStringFormat ) == false ) {
									value = ( (DateTime)cell.Value ).ToString( this.DateTimeStringFormat );
								} else {
									value = ( cell.Value ?? cell.InheritedStyle.NullValue ).ToString();
									if ( String.IsNullOrEmpty( cell.InheritedStyle.Format ) == false ) {
										value = String.Format( "{0:" + cell.InheritedStyle.Format + "}", cell.Value ?? cell.InheritedStyle.NullValue );
									}
								}
								SizeF tempSF = e.Graphics.MeasureString( value, this.BodyFont, (int)ColumnWidths[i] );
								if ( tempSF.Height > CellHeight )
									CellHeight = (int)tempSF.Height;
							}
							i++;
						}
					}
					i = 0;
					foreach ( DataGridViewCell cell in GridRow.Cells ) {
						if ( !cell.OwningColumn.Visible ) continue;
						if ( !m_SelectedColumns.Contains( cell.OwningColumn.HeaderText ) )
							continue;
						// For the TextBox Column
						if ( ( (Type)ColumnTypes[i] ).Name == "DataGridViewTextBoxColumn" || ( (Type)ColumnTypes[i] ).Name == "DataGridViewLinkColumn" ) {
							string value = String.Empty;
							if ( cell.Value is DateTime && String.IsNullOrEmpty( this.DateTimeStringFormat ) == false ) {
								value = ( (DateTime)cell.Value ).ToString( this.DateTimeStringFormat );
							} else {
								value = ( cell.Value ?? cell.InheritedStyle.NullValue ).ToString();
								if ( String.IsNullOrEmpty( cell.InheritedStyle.Format ) == false ) {
									value = String.Format( "{0:" + cell.InheritedStyle.Format + "}", cell.Value ?? cell.InheritedStyle.NullValue );
								}
							}
							e.Graphics.DrawString( value, this.BodyFont,
									new SolidBrush( cell.InheritedStyle.ForeColor ),
									new RectangleF( (int)ColumnLefts[i], (float)tmpTop,
									(int)ColumnWidths[i], (float)CellHeight ), StrFormat );
						}
							// For the Button Column
						else if ( ( (Type)ColumnTypes[i] ).Name == "DataGridViewButtonColumn" ) {
							CellButton.Text = ( cell.Value ?? "" ).ToString();
							CellButton.Size = new Size( (int)ColumnWidths[i], CellHeight );
							Bitmap bmp = new Bitmap( CellButton.Width, CellButton.Height );
							CellButton.DrawToBitmap( bmp, new Rectangle( 0, 0,
									bmp.Width, bmp.Height ) );
							e.Graphics.DrawImage( bmp, new Point( (int)ColumnLefts[i], tmpTop ) );
						}
							// For the CheckBox Column
						else if ( ( (Type)ColumnTypes[i] ).Name == "DataGridViewCheckBoxColumn" ) {
							CellCheckBox.Size = new Size( 14, 14 );
							if ( cell.Value != null ) {
								CellCheckBox.Checked = (bool)cell.Value;
							} else {
								CellCheckBox.Checked = false;
							}
							Bitmap bmp = new Bitmap( (int)ColumnWidths[i], CellHeight );
							Graphics tmpGraphics = Graphics.FromImage( bmp );
							tmpGraphics.FillRectangle( Brushes.White, new Rectangle( 0, 0,
									bmp.Width, bmp.Height ) );
							CellCheckBox.DrawToBitmap( bmp,
									new Rectangle( (int)( ( bmp.Width - CellCheckBox.Width ) / 2 ),
									(int)( ( bmp.Height - CellCheckBox.Height ) / 2 ),
									CellCheckBox.Width, CellCheckBox.Height ) );
							e.Graphics.DrawImage( bmp, new Point( (int)ColumnLefts[i], tmpTop ) );
						}
							// For the ComboBox Column
						else if ( ( (Type)ColumnTypes[i] ).Name == "DataGridViewComboBoxColumn" ) {
							CellComboBox.Size = new Size( (int)ColumnWidths[i], CellHeight );
							Bitmap bmp = new Bitmap( CellComboBox.Width, CellComboBox.Height );
							CellComboBox.DrawToBitmap( bmp, new Rectangle( 0, 0,
									bmp.Width, bmp.Height ) );
							e.Graphics.DrawImage( bmp, new Point( (int)ColumnLefts[i], tmpTop ) );
							e.Graphics.DrawString( cell.Value.ToString(), this.BodyFont,
									new SolidBrush( cell.InheritedStyle.ForeColor ),
									new RectangleF( (int)ColumnLefts[i] + 1, tmpTop, (int)ColumnWidths[i]
									- 16, CellHeight ), StrFormatComboBox );
						}
							// For the Image Column
						else if ( ( (Type)ColumnTypes[i] ).Name == "DataGridViewImageColumn" ) {
							Rectangle CelSize = new Rectangle( (int)ColumnLefts[i],
									tmpTop, (int)ColumnWidths[i], CellHeight );
							Size ImgSize = ( (Image)( cell.FormattedValue ) ).Size;
							e.Graphics.DrawImage( (Image)cell.FormattedValue,
									new Rectangle( (int)ColumnLefts[i] + (int)( ( CelSize.Width - ImgSize.Width ) / 2 ),
									tmpTop + (int)( ( CelSize.Height - ImgSize.Height ) / 2 ),
									( (Image)( cell.FormattedValue ) ).Width, ( (Image)( cell.FormattedValue ) ).Height ) );

						}
						// Drawing Cells Borders 
						e.Graphics.DrawRectangle( Pens.Gray, new Rectangle( (int)ColumnLefts[i], tmpTop, (int)ColumnWidths[i], CellHeight ) );
						i++;

					}
					tmpTop += CellHeight;
				}
				RowPos++;
				// For the first page it calculates Rows per Page
				if ( PageNo == 1 ) RowsPerPage++;
			}
			if ( RowsPerPage == 0 ) return;
			// Write Footer (Page Number)
			DrawFooter( e, RowsPerPage );
			e.HasMorePages = false;
		}

		private void DrawFooter( PrintPageEventArgs e, int RowsPerPage ) {
			double cnt = 0;
			// Detemining rows number to print
			if ( m_PrintAllRows ) {
				if ( dgv.Rows[dgv.Rows.Count - 1].IsNewRow )
					cnt = dgv.Rows.Count - 2; // When the DataGridView doesn't allow adding rows
				else
					cnt = dgv.Rows.Count - 1; // When the DataGridView allows adding rows
			} else
				cnt = dgv.SelectedRows.Count;
			// Writing the Page Number on the Bottom of Page
			string PageNum = PageNo.ToString() + " of " + Math.Ceiling( (double)( cnt / RowsPerPage ) ).ToString();
			e.Graphics.DrawString( PageNum, this.BodyFont, Brushes.Black,
				e.MarginBounds.Left + ( e.MarginBounds.Width -
				e.Graphics.MeasureString( PageNum, dgv.Font,
				e.MarginBounds.Width ).Width ) / 2, e.MarginBounds.Top +
				e.MarginBounds.Height + 31 );
		}

		#endregion

	}
}
