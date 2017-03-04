/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Awesome.Library.Utilities {

	public enum RotateSizeMode {
		SquaredSize,
		KeepOriginal,
		AutoSize
	}

	public enum ImageResizeMode {
		/// <summary>
		/// Stretches an image to fit in the specified bounds
		/// </summary>
		Stretch = 0,
		/// <summary>
		/// Scales the image to fit to the specified maximum size bounds
		/// </summary>
		Scale = 1,
		/// <summary>
		/// Scales the image to fit in the size bounds, then centers it within an image of the specified bounds
		/// </summary>
		ScaleCenter = 2,
		/// <summary>
		/// Scales the image to fit in the bounds wholly centered, even if part of the image is clipped off
		/// </summary>
		ScaleFit = 3
	}

	public static class Images {

		/// <summary>
		/// Returns the internet content type associated with a file name
		/// </summary>
		public static string ContentType( string path ) {
			string contentType = String.Empty;
			string extension = Path.GetExtension( path ).ToLower();
			switch ( extension ) {
				case ".gif":
					contentType = "image/gif";
					break;
				case ".jpeg":
				case ".jpg":
					contentType = "image/jpeg";
					break;
				case ".png":
					contentType = "image/png";
					break;
				case ".tiff":
					contentType = "image/tiff ";
					break;
				case ".ief":
					contentType = "image/ief";
					break;
				case ".ico":
					contentType = "image/vnd.microsoft.icon";
					break;
			}
			return contentType;
		}

		/// <summary>
		/// Resize an image to new width and height
		/// </summary>
		/// <param name="img">The image to be resized</param>
		/// <param name="ImageResizeMode">The mode to use when resizing the image</param>
		/// <param name="bounds">The new image width and height</param>
		/// <returns>The resized image</returns>
		public static Image ResizeImage( this Image img, ImageResizeMode mode, Size bounds, Color baseColor ) {
			return ResizeImage( img, mode, bounds, baseColor, InterpolationMode.High, SmoothingMode.Default );
		}

		/// <summary>
		/// Resize an image to new width and height
		/// </summary>
		/// <param name="img">The image to be resized</param>
		/// <param name="ImageResizeMode">The mode to use when resizing the image</param>
		/// <param name="bounds">The new image width and height</param>
		/// <param name="interpolateMode">The interpolation mode to be used when resizing the image</param>
		/// <returns>The resized image</returns>
		public static Image ResizeImage( this Image img, ImageResizeMode mode, Size bounds, Color baseColor, InterpolationMode interpolateMode, SmoothingMode smoothingMode ) {
			if ( mode == ImageResizeMode.Scale ) {
				bounds = Images.AspectSize( img, bounds.Width, bounds.Height );
			}
			Image sizedImg = new Bitmap( bounds.Width, bounds.Height );
			Graphics g = Graphics.FromImage( sizedImg );
			g.InterpolationMode = interpolateMode;
			g.SmoothingMode = smoothingMode;
			if ( baseColor != null && baseColor != Color.Empty && baseColor != Color.Transparent ) {
				g.FillRectangle( new SolidBrush( baseColor ), 0, 0, bounds.Width, bounds.Height );
			}
			if ( mode == ImageResizeMode.Stretch || mode == ImageResizeMode.Scale ) {
				g.DrawImage( img, 0, 0, bounds.Width, bounds.Height );
			} else if ( mode == ImageResizeMode.ScaleCenter || mode == ImageResizeMode.ScaleFit ) {
				Size aspectSize = Images.AspectSize( img, bounds.Width, bounds.Height, ( mode == ImageResizeMode.ScaleFit ) );
				int xloc = ( ( aspectSize.Width + bounds.Width ) / 2 ) - aspectSize.Width;
				int yloc = ( ( aspectSize.Height + bounds.Height ) / 2 ) - aspectSize.Height;
				g.DrawImage( img, xloc, yloc, aspectSize.Width, aspectSize.Height );
			}
			return sizedImg;
		}

		/// <summary>
		/// Finds the appropriate size of an image if scaled down to fit maxWidth x maxHeight
		/// </summary>
		public static Size AspectSize( this Image img, int maxWidth, int maxHeight ) {
			return AspectSize( img, maxWidth, maxHeight, false );
		}

		/// <summary>
		/// Finds the appropriate size of an image if scaled down to fit maxWidth x maxHeight
		/// </summary>
		private static Size AspectSize( Image img, int maxWidth, int maxHeight, bool oversize ) {
			Size retVal = new Size();
			float wRatio = maxWidth / (float)img.Width;
			float hRatio = maxHeight / (float)img.Height;
			if ( oversize == false ) {
				if ( wRatio < hRatio )
					hRatio = wRatio;
				else
					wRatio = hRatio;
			} else {
				if ( wRatio > hRatio )
					hRatio = wRatio;
				else
					wRatio = hRatio;
			}
			retVal.Width = Convert.ToInt32( img.Width * wRatio );
			retVal.Height = Convert.ToInt32( img.Height * hRatio );
			return retVal;
		}

		/// <summary>
		/// Compares two images pixel by pixel
		/// </summary>
		/// <returns>True if the images appear the same, False otherwise</returns>
		public static bool Compare( this Image img1, Image img2 ) {
			return Compare( new Bitmap( img1 ), new Bitmap( img2 ) );
		}

		/// <summary>
		/// Compares two images pixel by pixel
		/// </summary>
		/// <returns>True if the images appear the same, False otherwise</returns>
		public static bool Compare( this Bitmap bmp1, Bitmap bmp2 ) {
			bool flag = true;
			Color col1, col2;
			if ( bmp1.Width == bmp2.Width && bmp1.Height == bmp2.Height ) {
				for ( int i = 0; i < bmp1.Width; i++ ) {
					for ( int j = 0; j < bmp1.Height; j++ ) {
						col1 = bmp1.GetPixel( i, j );
						col2 = bmp2.GetPixel( i, j );
						if ( col1 != col2 ) {
							flag = false;
							break;
						}
					}
					if ( flag == false )
						break;
				}
				return flag;
			} else
				return false;
		}

		/// <summary>
		/// Clones the image to an open memory stream.
		/// </summary>
		public static MemoryStream GetMemoryStream( this Image img ) {
			MemoryStream ms = new MemoryStream();
			img.Save( ms, ImageFormat.Png );
			ms.Seek( 0, SeekOrigin.Begin );
			return ms;
		}

		/// <summary>
		/// Converts an image into a base 64 string for transport
		/// </summary>
		/// <param name="img">The image to be converted</param>
		/// <param name="format">The format to use for image encoding (should use a lossless format like PNG)</param>
		/// <returns>the base64 string representation of the image.</returns>
		public static string ToBase64String( this Image img, ImageFormat format ) {
			string base64 = String.Empty;
			if ( img != null ) {
				MemoryStream memStream = new MemoryStream();
				img.Save( memStream, format );
				base64 = Convert.ToBase64String( memStream.ToArray() );
				memStream.Close();
				memStream = null;
			}
			return base64;
		}

		/// <summary>
		/// Converts a base64 string of an image back into an image
		/// </summary>
		/// <param name="input">the base 64 string containing the image</param>
		/// <returns>The image from the base 64 string</returns>
		public static Image FromBase64String( string input ) {
			if ( String.IsNullOrEmpty( input ) == false ) {
				byte[] imgData = Convert.FromBase64String( input );
				MemoryStream memStream = new MemoryStream( imgData );
				return Image.FromStream( memStream );
			}
			return null;
		}

		/// <summary>
		/// Converts an image to a byte[]
		/// </summary>
		public static byte[] ToByteArray( this Image img, ImageFormat format ) {
			MemoryStream ms = new MemoryStream();
			img.Save( ms, format );
			return ms.ToArray();
		}

		/// <summary>
		/// Converts a byte[] to an image
		/// </summary>
		public static Image FromByteArray( byte[] imgBytes ) {
			if ( imgBytes != null ) {
				MemoryStream ms = new MemoryStream( imgBytes );
				Image returnImage = Image.FromStream( ms );
				return returnImage;
			}
			return null;
		}

		/// <summary>
		/// Creates a color thumbnail image of the specified color
		/// </summary>
		public static Image GenerateColorBox( Size size, Color color ) {
			return GenerateColorBox( size, color, -70 );
		}

		/// <summary>
		/// Creates a color thumbnail image of the specified color
		/// </summary>
		public static Image GenerateColorBox( Size size, Color color, int gradientShift ) {
			Image i = new Bitmap( size.Width, size.Height );
			Graphics g = Graphics.FromImage( i );
			Rectangle r = new Rectangle( 0, 0, size.Width - 1, size.Height - 1 );
			g.FillRectangle( new LinearGradientBrush( r, color, color.ChangeBrightness( gradientShift ), LinearGradientMode.ForwardDiagonal ), r );
			g.DrawRectangle( new Pen( color.ChangeBrightness( gradientShift - 20 ), 1 ), r );
			g.Flush();
			g.Dispose();
			return i;
		}

		/// <summary>
		/// Changes the brightness of a color
		/// </summary>
		public static Color ChangeBrightness( this Color color, int shift ) {
			byte r = color.R;
			byte g = color.G;
			byte b = color.B;
			if ( r + shift >= 0 && r + shift < 256 )
				r = (byte)( r + shift );
			else if ( r + shift < 0 )
				r = 0;
			else
				r = 255;
			if ( g + shift >= 0 && g + shift < 256 )
				g = (byte)( g + shift );
			else if ( g + shift < 0 )
				g = 0;
			else
				g = 255;
			if ( b + shift >= 0 && b + shift < 256 )
				b = (byte)( b + shift );
			else if ( b + shift < 0 )
				b = 0;
			else
				b = 255;
			return Color.FromArgb( color.A, r, g, b );
		}

		/// <summary>
		/// Create an icon from the given image
		/// </summary>
		public static Icon ToIcon( this Image img, int size, bool keepAspectRatio ) {
			return ToIcon( img, size, keepAspectRatio, InterpolationMode.HighQualityBicubic );
		}

		/// <summary>
		/// Create an icon from the given image
		/// </summary>
		public static Icon ToIcon( this Image img, int size, bool keepAspectRatio, InterpolationMode interpolateMode ) {
			if ( img != null ) {
				Bitmap square = new Bitmap( size, size );
				Graphics g = Graphics.FromImage( square );
				int x, y, w, h;
				if ( !keepAspectRatio || img.Height == img.Width ) {
					x = y = 0;
					w = h = size;
				} else {
					float r = (float)img.Width / (float)img.Height;
					if ( r > 1 ) {
						w = size;
						h = (int)( (float)size / r );
						x = 0;
						y = ( size - h ) / 2;
					} else {
						w = (int)( (float)size * r );
						h = size;
						y = 0;
						x = ( size - w ) / 2;
					}
				}
				g.InterpolationMode = interpolateMode;
				g.DrawImage( img, x, y, w, h );
				g.Flush();
				return Icon.FromHandle( square.GetHicon() );
			}
			return null;
		}

		/// <summary>
		/// This method scans all pixels in order to locate the non-transparent bounds of the actual image.
		/// </summary>
		public static Rectangle GetPixelBounds( this Bitmap bmp, params Color[] transparentColors ) {
			Rectangle bounds = new Rectangle( bmp.Width, bmp.Height, 0, 0 );
			for ( int x = 0; x < bmp.Width; x++ ) {
				for ( int y = 0; y < bmp.Height; y++ ) {
					Color pixel = bmp.GetPixel( x, y );
					bool transparent = ( pixel.A == 0 );
					if ( transparent == false && transparentColors != null ) {
						foreach ( Color c in transparentColors ) {
							if ( pixel.R == c.R && pixel.G == c.G && pixel.B == c.B ) {
								transparent = true;
								break;
							}
						}
					}
					if ( transparent == false ) {
						if ( x < bounds.X ) {
							bounds.X = x;
						}
						if ( x > bounds.Width ) {
							bounds.Width = x;
						}
						if ( y < bounds.Y ) {
							bounds.Y = y;
						}
						if ( y > bounds.Height ) {
							bounds.Height = y;
						}
					}
				}
			}
			return bounds;
		}

		public static Image Crop( this Image img, Rectangle cropTo ) {
			Bitmap bmp = new Bitmap( cropTo.Width - cropTo.X, cropTo.Height - cropTo.Y, img.PixelFormat );
			Graphics g = Graphics.FromImage( bmp );
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.InterpolationMode = InterpolationMode.NearestNeighbor;
			g.SmoothingMode = SmoothingMode.None;
			g.DrawImage( img, 0, 0, cropTo, GraphicsUnit.Pixel );
			g.Dispose();
			return bmp;
		}

		/// <summary>
		/// Rotate an image either clockwise or counter-clockwise. This method uses extensive checking to ensure a high quality rotation, so it is not recommended for use in painting methods.
		/// </summary>
		/// <param name="interpolateMode">The interpolation mode to be used when resizing the image</param>
		/// <returns>The rotated image</returns>
		public static Image Rotate( this Image img, int degrees, bool autoCorrecting, RotateSizeMode sizeMode, InterpolationMode interpolateMode, SmoothingMode smoothingMode ) {
			degrees = degrees.SafeRange( -360, 360 );
			img = Image.FromStream( img.GetMemoryStream() );
			Bitmap bmp = null;
			if ( sizeMode == RotateSizeMode.SquaredSize || sizeMode == RotateSizeMode.AutoSize ) {
				int sqr = (int)Math.Sqrt( img.Width * img.Width + img.Height * img.Height );
				bmp = new Bitmap( sqr, sqr );
			} else {
				bmp = new Bitmap( img.Width, img.Height );
			}
			Graphics g = Graphics.FromImage( bmp );
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.InterpolationMode = interpolateMode;
			g.SmoothingMode = smoothingMode;
			if ( autoCorrecting ) {
				if ( degrees > 0 ) {
					if ( degrees < 90 ) {
						DrawRotate( g, bmp.Size, img, degrees );
					} else if ( degrees < 180 ) {
						img.RotateFlip( RotateFlipType.Rotate90FlipNone );
						DrawRotate( g, bmp.Size, img, degrees - 90 );
					} else if ( degrees < 270 ) {
						img.RotateFlip( RotateFlipType.Rotate180FlipNone );
						DrawRotate( g, bmp.Size, img, degrees - 180 );
					} else if ( degrees == 360 ) {
						DrawRotate( g, bmp.Size, img, 0 );
					} else {
						img.RotateFlip( RotateFlipType.Rotate270FlipNone );
						DrawRotate( g, bmp.Size, img, degrees - 270 );
					}
				} else {
					if ( degrees > -90 ) {
						DrawRotate( g, bmp.Size, img, degrees );
					} else if ( degrees > -180 ) {
						img.RotateFlip( RotateFlipType.Rotate180FlipNone );
						DrawRotate( g, bmp.Size, img, degrees + 180 );
					} else if ( degrees > -270 ) {
						img.RotateFlip( RotateFlipType.Rotate180FlipNone );
						DrawRotate( g, bmp.Size, img, degrees + 180 );
					} else if ( degrees == -360 ) {
						DrawRotate( g, bmp.Size, img, 0 );
					} else {
						img.RotateFlip( RotateFlipType.Rotate90FlipNone );
						DrawRotate( g, bmp.Size, img, degrees + 270 );
					}
				}
			} else {
				DrawRotate( g, bmp.Size, img, degrees );
			}
			g.Dispose();
			if ( sizeMode == RotateSizeMode.AutoSize ) {
				Rectangle r = GetPixelBounds( bmp, null );
				return Crop( bmp, r );
			}
			return bmp;
		}

		/// <summary>
		/// Performs a rotation transform. This method may lose a large amount of quality for rotations greater than 90 degrees.
		/// </summary>
		/// <param name="img">the image to be rotated</param>
		/// <param name="degrees">the angle (in degrees).</param>
		private static void DrawRotate( Graphics g, Size graphicsBounds, Image img, int degrees ) {
			PointF drawPoint = new PointF( ( graphicsBounds.Width - img.Width ) / 2F, ( graphicsBounds.Height - img.Height ) / 2F );
			if ( degrees != 0 ) {
				Bitmap bmp = new Bitmap( img.Width, img.Height );
				//now we set the rotation point to the center of our image
				float halfW = ( (float)graphicsBounds.Width / 2F );
				float halfH = ( (float)graphicsBounds.Height / 2F );
				g.TranslateTransform( halfW, halfH );
				//now rotate the image
				g.RotateTransform( degrees );
				g.TranslateTransform( halfW * -1, halfH * -1 );
				//now draw our new image onto the graphics object
			}
			g.DrawImage( img, new RectangleF( drawPoint, img.Size ), new RectangleF( 0, 0, img.Width, img.Height ), GraphicsUnit.Pixel );
		}

	}

}
