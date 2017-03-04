/********************************************
 * MIT License
 * (c) Christopher Eaton, 2012
 * https://github.com/chriseaton/dotnet-awesome-lib
 ********************************************/
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Drawing;

namespace Awesome.Library.Utilities.DriveInfo {

		/// <summary>
		/// Represents the different types of drives that may exist in a system.
		/// </summary>
		public enum VolumeTypes {
			Unknown,	// The drive type cannot be determined. 
			Invalid,	// The root path is invalid. For example, no volume is mounted at the path. 
			Removable,	// The disk can be removed from the drive. 
			Fixed,		// The disk cannot be removed from the drive. 
			Remote,		// The drive is a remote (network) drive. 
			CDROM,		// The drive is a CD-ROM drive. 
			RAMDisk		// The drive is a RAM disk. 
		};

		/// <summary>
		/// Represents the different supporting flags that may be set on a file system.
		/// </summary>
		[Flags]
		public enum VolumeFlags {
			Unknown					= 0x0,
			CaseSensitive			= 0x00000001,
			Compressed				= 0x00008000,
			PersistentACLS			= 0x00000008,
			PreservesCase			= 0x00000002,
			ReadOnly				= 0x00080000,
			SupportsEncryption		= 0x00020000,
			SupportsFileCompression	= 0x00000010,
			SupportsNamedStreams	= 0x00040000,
			SupportsObjectIDs		= 0x00010000,
			SupportsQuotas			= 0x00000020,
			SupportsReparsePoints	= 0x00000080,
			SupportsSparseFiles		= 0x00000040,
			SupportsUnicodeOnVolume	= 0x00000004
		};

		public class InvalidVolumeException : ApplicationException {
			public InvalidVolumeException(Uri VolUri) : base("Volume information could not be accessed for the path '" + VolUri.LocalPath + "'.\nVerify that the path is valid and ends in a trailing backslash."){}
		}

		public class InvalidVolumeTypeException : ApplicationException {
			public InvalidVolumeTypeException() : base("The chosen volume is of the wrong type."){}
		}

		public class VolumeAccessException : ApplicationException {
			public VolumeAccessException() : base("Access to the volume could not be established. It may be offline or disconnected."){}
		}

		/// <summary>
		/// Presents information about a volume.
		/// </summary>
		public class VolumeInfo {
			/**********************************************************
			* Private Constants
			*********************************************************/
			private const int NAMESIZE = 80;
			private const int MAX_PATH = 256;
			private const int FILE_ATTRIBUTE_NORMAL = 128;
			private const int SHGFI_USEFILEATTRIBUTES = 16;
			private const int SHGFI_ICON = 256;
			private const int SHGFI_LARGEICON = 0;
			private const int SHGFI_SMALLICON = 1;

			/**********************************************************
			* Private Structures
			*********************************************************/
			[StructLayout(LayoutKind.Sequential)]
				private class UniversalNameInfo { 
				public string NetworkPath=null;
			}

			[StructLayout ( LayoutKind.Sequential, CharSet=CharSet.Ansi )]
				public struct SHFILEINFOA { 
				public IntPtr   hIcon; 
				public int      iIcon; 
				public uint   dwAttributes; 
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAX_PATH)]
				public string szDisplayName; 
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=NAMESIZE)]
				public string szTypeName; 
			};

			[StructLayout ( LayoutKind.Sequential, CharSet=CharSet.Unicode )]
				public struct SHFILEINFO { 
				public IntPtr   hIcon; 
				public int      iIcon; 
				public uint   dwAttributes; 
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAX_PATH)]
				public string szDisplayName; 
				[MarshalAs(UnmanagedType.ByValTStr, SizeConst=NAMESIZE)]
				public string szTypeName; 
			};

			/**********************************************************
			* Private Enums
			*********************************************************/
			private enum UniInfoLevels {
				Universal=1,
				Remote=2
			};

			/**********************************************************
			 * Method Imports
			 *********************************************************/
			[DllImport("mpr.dll")]
			private static extern UInt32 WNetGetUniversalName( string driveLetter, UniInfoLevels InfoLevel, IntPtr Ptr, ref UInt32 UniSize );
			[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
			[return:MarshalAs(UnmanagedType.U4)] 
			private static extern int GetDriveType(string driveLetter);
			[DllImport("Shell32.dll")]
			private static extern IntPtr SHGetFileInfo( string pszPath,	uint dwFileAttributes, ref SHFILEINFO psfi,	uint cbFileInfo, uint uFlags );
			[DllImport("kernel32.dll")]
			private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber, ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags, StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);
			[DllImport("kernel32.dll", CharSet=CharSet.Auto)]
			public static extern bool GetDiskFreeSpaceEx(string lpDirectoryName,[MarshalAs(UnmanagedType.U8)] ref long lpFreeBytesAvailable,[MarshalAs(UnmanagedType.U8)] ref long lpTotalNumberOfBytes,[MarshalAs(UnmanagedType.U8)] ref long lpTotalNumberOfFreeBytes); 
			
			/**********************************************************
			 * Member Variables
			 *********************************************************/
			private Uri uri;
			private Icon largeIcon;
			private Icon smallIcon;
			private string volLabel;
			private VolumeTypes volType;
			private UInt32 serNum;
			private UInt32 maxCompLen;
			private VolumeFlags volFlags;
			private string fsName;
			private long bytesAvailable = 0;
			private long bytesTotal = 0;
			private long bytesFree = 0;

			/**********************************************************
			 * Constructors
			 *********************************************************/
			public VolumeInfo(Uri uri) {
				// Make sure we were passed something
				if (uri == null) throw new ArgumentNullException();

				// Make sure we can handle this type of uri
				if (!uri.IsFile) throw new InvalidVolumeException(uri);

				// Make sure Uri is trailed properly
				if (!uri.LocalPath.EndsWith("\\")) throw new InvalidVolumeException(uri);

				// Store the Uri
				this.uri = uri;

				// Build information. 
				Refresh();
			}

			/**********************************************************
			 * Utility Methods
			 *********************************************************/
			private bool FlagSet(VolumeFlags Flag) {
				return ((volFlags & Flag) == Flag);
			}

			/**********************************************************
			 * Methods
			 *********************************************************/

			public void RefreshFreeSpace() {
				if (uri != null) {
					if (GetDiskFreeSpaceEx(uri.LocalPath, ref this.bytesAvailable, ref this.bytesTotal, ref this.bytesFree) == false) {
						bytesFree = bytesTotal = bytesAvailable = 0;
					}
				} else {
					bytesFree = bytesTotal = bytesAvailable = 0;
				}
			}

			public void Refresh() {
				// Set defaults
				largeIcon = null;
				smallIcon = null;
				volLabel = "";
				volType = VolumeTypes.Invalid;
				serNum = 0;
				maxCompLen = 0;
				volFlags = VolumeFlags.Unknown;
				fsName = "";

				// Get the volume type
				volType = (VolumeTypes)GetDriveType(uri.LocalPath);
				
				// If not successful, throw an exception
				if (volType == VolumeTypes.Invalid) throw new InvalidVolumeException(uri);

				// Declare Receiving Variables
				StringBuilder VolLabel = new StringBuilder(256);	// Label
				UInt32 VolFlags = new UInt32();
				StringBuilder FSName = new StringBuilder(256);	// File System Name
			
				// Attempt to retreive the information
				long Ret = GetVolumeInformation(uri.LocalPath, VolLabel, (UInt32)VolLabel.Capacity, ref serNum, ref maxCompLen, ref VolFlags, FSName, (UInt32)FSName.Capacity);
				// if (Ret != 0) throw new VolumeAccessException();

				// Move to regular variables
				volLabel = VolLabel.ToString();
				volFlags = (VolumeFlags)VolFlags;
				fsName = FSName.ToString();

				// Attempt to get icons
				largeIcon = GetIcon(true);
				smallIcon = GetIcon(false);
			}

			private Icon GetIcon(bool Large) {
				// Holder
				Icon Ret = null;

				// Attempt
				try {
					// Create structure
					SHFILEINFO shfi = new SHFILEINFO();

					// Calc Flags
					uint flgs = SHGFI_USEFILEATTRIBUTES | SHGFI_ICON;
					if (!Large) flgs |= SHGFI_SMALLICON;

					// Call method
					SHGetFileInfo( uri.LocalPath, FILE_ATTRIBUTE_NORMAL, ref shfi, (uint)Marshal.SizeOf(shfi), flgs );
				
					// Return the icon
					Ret = Icon.FromHandle(shfi.hIcon);
				}
				catch{}

				// Return icon.
				return Ret;
			}

			/**********************************************************
			 * Properties
			 *********************************************************/
			public Uri Uri {
				get {
					return uri;
				}
			}

			public VolumeTypes VolumeType {
				get {
					return volType;
				}
			}

			public string UncPath {
				get {
					// Make sure it's the right type
					if (volType != VolumeTypes.Remote) throw new InvalidVolumeTypeException();

					// If it is a Unc path, just return the root
					if (uri.IsUnc) return uri.LocalPath;
				
					// It's a mapped drive letter, we have to perform the lookup
					// Allocate Memory
					uint Sze = 255;
					IntPtr Buff = Marshal.AllocCoTaskMem((int)Sze);

					// Call API to perform lookup
					uint Ret = WNetGetUniversalName(uri.LocalPath, UniInfoLevels.Universal, Buff, ref Sze);

					if (Ret != 0) {
						Marshal.FreeCoTaskMem(Buff);
						throw new VolumeAccessException();
					}
				
					// Get the result
					UniversalNameInfo Result = (UniversalNameInfo)Marshal.PtrToStructure(Buff,typeof(UniversalNameInfo));

					// Free the memory
					Marshal.FreeCoTaskMem(Buff);
				
					// Get result
					string sRes = Result.NetworkPath;
					if (!sRes.EndsWith("\\")) sRes += "\\";

					// Return the result
					return sRes;
				}
			}

			public Icon LargeIcon {
				get {
					return largeIcon;
				}
			}

			public Icon SmallIcon {
				get {
					return smallIcon;
				}
			}

			public string Label {
				get {
					return volLabel;
				}
			}

			public UInt32 SerialNumber {
				get {
					return serNum;
				}
			}

			public UInt32 MaxComponentLen {
				get {
					return maxCompLen;
				}
			}

			public VolumeFlags Flags {
				get {
					return volFlags;
				}
			}
		
			public bool CaseSensitive {
				get {
					return FlagSet(VolumeFlags.CaseSensitive);
				}
			}

			public bool Compressed {
				get {
					return FlagSet(VolumeFlags.Compressed);
				}
			}

			public bool PersistentACLS {
				get {
					return FlagSet(VolumeFlags.PersistentACLS);
				}
			}


			public bool PreservesCase {
				get {
					return FlagSet(VolumeFlags.PreservesCase);
				}
			}

			public bool ReadOnly {
				get {
					return FlagSet(VolumeFlags.ReadOnly);
				}
			}

			public bool SupportsEncryption {
				get {
					return FlagSet(VolumeFlags.SupportsEncryption);
				}
			}

			public bool SupportsFileCompression {
				get {
					return FlagSet(VolumeFlags.SupportsFileCompression);
				}
			}

			public bool SupportsNamedStreams {
				get {
					return FlagSet(VolumeFlags.SupportsNamedStreams);
				}
			}
		
			public bool SupportsObjectIDs {
				get {
					return FlagSet(VolumeFlags.SupportsObjectIDs);
				}
			}
		
			public bool SupportsQuotas {
				get {
					return FlagSet(VolumeFlags.SupportsQuotas);
				}
			}
		
			public bool SupportsReparsePoints {
				get {
					return FlagSet(VolumeFlags.SupportsReparsePoints);
				}
			}
		
			public bool SupportsSparseFiles {
				get {
					return FlagSet(VolumeFlags.SupportsSparseFiles);
				}
			}
		
			public bool SupportsUnicodeOnVolume {
				get {
					return FlagSet(VolumeFlags.SupportsUnicodeOnVolume);
				}
			}

			public long BytesAvailable {
				get {
					return this.bytesAvailable;
				}
			}

			public long BytesTotal {
				get {
					return this.bytesTotal;
				}
			}

			public long BytesFree {
				get {
					return this.bytesFree;
				}
			}
	
			/**********************************************************
			 * Static Creators
			 *********************************************************/
			static public VolumeInfo CurrentVolume {
				get {
					return new VolumeInfo(new Uri(Directory.GetDirectoryRoot(Directory.GetCurrentDirectory())));
				}
			}
	}
}

