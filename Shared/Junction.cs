using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

#if USE_JUNCTIONS
/*
LICENSE

Copyright (c) 2009, Michael Lehman (http://www.machinegods.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
* Neither the name of the Michael Lehman, machinegods.com, nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace MachineGods.FS
{

    //-----------------------------------------------------------------------
    public struct ReparsePoint
    {
        public string Path;
        public string Target;
        public bool IsReparsePoint;
        public Junctions.ReparseTagType TagType;
        public int Err;
        public string ErrMsg;
    }

    //-----------------------------------------------------------------------

    public class Junctions
    {
        #region PInvoke Defines

        internal const int INVALID_HANDLE_VALUE = -1;
        internal const int MAX_PATH = 260;
        internal const int MAX_ALTERNATE = 14;

        internal const int ERROR_NO_MORE_FILES = 18;

        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;

        internal const uint CREATE_NEW = 1;
        internal const uint CREATE_ALWAYS = 2;
        internal const uint OPEN_EXISTING = 3;

        internal const uint FILE_SHARE_NONE = 0x00000000;
        internal const uint FILE_SHARE_READ = 0x00000001;
        internal const uint FILE_SHARE_WRITE = 0x00000002;
        internal const uint FILE_SHARE_DELETE = 0x00000004;

        internal const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        internal const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;

        internal const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
        internal const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
        internal const uint FILE_FLAG_WRITE_THROUGH = 0x08000000;

        internal const uint FSCTL_GET_REPARSE_POINT = 0x000900a8;

        [StructLayout(LayoutKind.Sequential)]
        internal struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATA
        {
            public FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ALTERNATE)]
            public string cAlternate;
        }

        internal enum FINDEX_SEARCH_OPS
        {
            NameMatch,
            LimitToDirectories,
            LimitToDevices
        }

        internal enum FINDEX_INFO_LEVELS
        {
            FindExInfoStandard,
            FindExInfoMaxInfoLevel
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern IntPtr FindFirstFileExW(string lpFileName, FINDEX_INFO_LEVELS
        fInfoLevelId, out WIN32_FIND_DATA lpFindFileData, FINDEX_SEARCH_OPS fSearchOp,
        IntPtr lpSearchFilter, uint dwAdditionalFlags);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindFirstFileW(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        internal static extern bool FindNextFileW(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll")]
        internal static extern bool FindClose(IntPtr hFindFile);

        public enum ReparseTagType : uint
        {
            IO_REPARSE_TAG_MOUNT_POINT = (0xA0000003),
            IO_REPARSE_TAG_HSM = (0xC0000004),
            IO_REPARSE_TAG_SIS = (0x80000007),
            IO_REPARSE_TAG_DFS = (0x8000000A),
            IO_REPARSE_TAG_SYMLINK = (0xA000000C),
            IO_REPARSE_TAG_DFSR = (0x80000012),
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct REPARSE_DATA_BUFFER
        {
            internal uint ReparseTag;
            internal ushort ReparseDataLength;
            ushort Reserved;
            internal ushort SubstituteNameOffset;
            internal ushort SubstituteNameLength;
            internal ushort PrintNameOffset;
            internal ushort PrintNameLength;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            internal string PathBuffer;
        }

        internal const int ReparseHeaderSize = sizeof(uint) + sizeof(ushort) + sizeof(ushort);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool DeviceIoControl(
        IntPtr hDevice,
        uint dwIoControlCode,
        IntPtr lpInBuffer,
        uint nInBufferSize,
        IntPtr lpOutBuffer,
        uint nOutBufferSize,
        out uint lpBytesReturned,
        IntPtr lpOverlapped
        );

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr CreateFileW(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr SecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        IntPtr hTemplateFile
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        #endregion PInvoke Defines

        //-----------------------------------------------------------------------

        ///
        /// Returns true if the path is a reparse point of the type IO_REPARSE_TAG_MOUNT_POINT.
        ///
        ///
        ///
        public static bool IsMountPointReparsePoint(string path)
        {
            bool isMountPoint = false;
            path = path.Trim();

            //skip the "." directories . and ..
            if (path.EndsWith(".")) return false;

            //make sure the path does not end in a "\" or "\*".
            if (path.EndsWith(@"\*")) path = path.Substring(0, path.Length - 2);
            if (path.EndsWith(@"\")) path = path.Substring(0, path.Length - 1);

            //convert the path to a unicode path to ensure we don't hit length and character restrictions
            //make sure the converted path ends with "\*" for using with the FindFirstFile/FindNextFile calls
            //The managed classes don't support this, so we have to use Win32 API directly for all our processes
            string wPath = path;
            if (!wPath.StartsWith(@"\\?\")) wPath = @"\\?\" + wPath;

            //the find data holds the attributes of the file/folder found, including if it's a reparse point
            WIN32_FIND_DATA findData;

            //In theory, the FINDEX_SEARCH_OPS.LimitToDirectories will cause FindFirstFileExW to only return
            //directories... Unfortunatly, it's a great theory that doesn't work most of the time.
            //So we have to test every object returned to determine if it is indeed a directory
            IntPtr pathHndl = FindFirstFileExW(wPath, FINDEX_INFO_LEVELS.FindExInfoStandard, out findData,
            FINDEX_SEARCH_OPS.LimitToDirectories, IntPtr.Zero, 0);

            if (pathHndl.ToInt32() == INVALID_HANDLE_VALUE) return false;

            if ((findData.dwFileAttributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            {
                if ((findData.dwReserved0 & (uint)ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT) == (uint)ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT)
                    isMountPoint = true;
            }

            FindClose(pathHndl);

            return isMountPoint;
        }

        //-----------------------------------------------------------------------

        public static List GetReparsePoints(string path, bool recursive)
        {
            List lRps = new List();

            path = path.Trim();

            //skip the "." directories . and ..
            if (path.EndsWith(".")) return lRps;

            //make sure the path does not end in a "\" or "\*".
            //We will make sure our converted path right below does however
            if (path.EndsWith(@"\*")) path = path.Substring(0, path.Length - 2);
            if (path.EndsWith(@"\")) path = path.Substring(0, path.Length - 1);

            //convert the path to a unicode path to ensure we don't hit length and character restrictions
            //make sure the converted path ends with "\*" for using with the FindFirstFile/FindNextFile calls
            //The managed classes don't support this, so we have to use Win32 API directly for all our processes
            string wPath = path;
            if (!wPath.StartsWith(@"\\?\")) wPath = @"\\?\" + wPath;
            wPath = wPath + @"\*";

            //the find data holds the attributes of the file/folder found, including if it's a reparse point
            WIN32_FIND_DATA findData;

            //In theory, the FINDEX_SEARCH_OPS.LimitToDirectories will cause FindFirstFileExW to only return
            //directories... Unfortunatly, it's a great theory that doesn't work most of the time.
            //So we have to test every object returned to determine if it is indeed a directory
            IntPtr pathHndl = FindFirstFileExW(wPath, FINDEX_INFO_LEVELS.FindExInfoStandard, out findData,
            FINDEX_SEARCH_OPS.LimitToDirectories, IntPtr.Zero, 0);

            if (pathHndl.ToInt32() == INVALID_HANDLE_VALUE) return lRps;

            bool isDir = false;
            bool isMountPoint = false;

            if ((findData.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                isDir = true;

            if ((findData.dwFileAttributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
            {
                if ((findData.dwReserved0 & (uint)ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT) == (uint)ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT)
                {
                    isMountPoint = true;

                    ReparsePoint rp = new ReparsePoint();
                    rp.Path = path + "\\" + findData.cFileName;
                    rp.TagType = ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT;
                    rp.IsReparsePoint = true;
                    rp.Err = 0;
                    rp.ErrMsg = "";
                    GetTarget(ref rp);
                    lRps.Add(rp);
                }
            }

            //Recurse
            //We only want to recurse down reparse points if we're told to, to avoid looping
            if ((isDir && !isMountPoint) || (isMountPoint && recursive))
            {
                List tlRps = GetReparsePoints(path + "\\" + findData.cFileName, recursive);
                if (tlRps != null) lRps.AddRange(tlRps);
            }

            //Get all the rest of the files/directories
            while (true)
            {
                if (FindNextFileW(pathHndl, out findData))
                {
                    if (pathHndl.ToInt32() == INVALID_HANDLE_VALUE) return lRps;

                    isDir = false;
                    isMountPoint = false;

                    if ((findData.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                        isDir = true;

                    if ((findData.dwFileAttributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        if ((findData.dwReserved0 & (uint)ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT) == (uint)ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT)
                        {
                            isMountPoint = true;

                            ReparsePoint rp = new ReparsePoint();
                            rp.Path = path + "\\" + findData.cFileName;
                            rp.TagType = ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT;
                            rp.IsReparsePoint = true;
                            rp.Err = 0;
                            rp.ErrMsg = "";
                            GetTarget(ref rp);
                            lRps.Add(rp);
                        }
                    }

                    //Recurse
                    //We only want to recurse down reparse points if we're told to, to avoid looping
                    if ((isDir && !isMountPoint) || (isMountPoint && recursive))
                    {
                        List tlRps = GetReparsePoints(path + "\\" + findData.cFileName, recursive);
                        if (tlRps != null) lRps.AddRange(tlRps);
                    }
                }
                else //findNextFile failed
                {
                    break;
                }

            }

            FindClose(pathHndl);
            return lRps;
        }

        //-----------------------------------------------------------------------
        // Gets the target path for a reparse point
        private static void GetTarget(ref ReparsePoint rp)
        {

            string wPath = rp.Path;
            if (!wPath.StartsWith(@"\\?\")) wPath = @"\\?\" + wPath;

            if (((uint)rp.TagType & (uint)ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT) != (uint)ReparseTagType.IO_REPARSE_TAG_MOUNT_POINT)
            {
                rp.Target = "";
                return;
            }

            try
            {
                // We need a handle to the reparse point to pass to DeviceIocontrol down below.
                // CreateFile is how we get that handle.
                // We want to play nice with others, so we note that we're only reading it, and we want to
                // allow others to be able to access (but not delete) it as well while we have the handle (the
                // GENERIC_READ and FILE_SHARE_READ | FILE_SHARE_WRITE values)
                //
                // Biggest key is the flag FILE_FLAG_OPEN_REPARSE_POINT, which tells CreateFile that we want
                // a handle to the reparse point itself and not to the target of the reparse point
                //
                // CreateFile will return INVALID_HANDLE_VALUE with a last error of 5 - Access Denied
                // if the FILE_FLAG_BACKUP_SEMANTICS is not specified when opening a directory/reparse point
                // It's noted in the directory section of the CreateFile MSDN page
                IntPtr pathHndl = CreateFileW(wPath, GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero,
                OPEN_EXISTING, FILE_FLAG_BACKUP_SEMANTICS | FILE_FLAG_OPEN_REPARSE_POINT, IntPtr.Zero);

                if (pathHndl.ToInt32() == INVALID_HANDLE_VALUE)
                {
                    rp.Err = Marshal.GetLastWin32Error();
                    rp.ErrMsg = "Invalid Handle returned by CreateFile";
                    rp.Target = "";
                    return;
                }

                uint lenDataReturned = 0;
                REPARSE_DATA_BUFFER rpDataBuf = new REPARSE_DATA_BUFFER();

                //Allocate a buffer to get the "user defined data" out of the reaprse point.
                //MSDN page on FSCTL_GET_REPARSE_POINT discusses size calculation
                IntPtr pMem = Marshal.AllocHGlobal(Marshal.SizeOf(rpDataBuf) + ReparseHeaderSize);

                //DeviceIocontrol takes a handle to a file/directory/device etc, that's obtained via CreateFile
                // In our case, it's a handle to the directory that's marked a reparse point
                //We pass in the FSCTL_GET_REPARSE_POINT flag to getll DeviceIoControl that we want to get data about a reparse point
                //There is no In buffer. pMem is our out buffer that will hold the returned REPARSE_DATA_BUFFER
                //lenDataReturned is of course how much data was copied into our buffer pMem.
                //We're doing a simple call.. no asyncronous stuff going on, so Overlapped is a null pointer
                if (!DeviceIoControl(pathHndl, FSCTL_GET_REPARSE_POINT, IntPtr.Zero, 0, pMem,
                (uint)Marshal.SizeOf(rpDataBuf) + ReparseHeaderSize, out lenDataReturned, IntPtr.Zero))
                {
                    rp.ErrMsg = "Call to DeviceIoControl failed";
                    rp.Err = Marshal.GetLastWin32Error();
                    rp.Target = "";
                }
                else
                {
                    rpDataBuf = (REPARSE_DATA_BUFFER)Marshal.PtrToStructure(pMem, rpDataBuf.GetType());
                    rp.Target = rpDataBuf.PathBuffer;
                }
                Marshal.FreeHGlobal(pMem);
                CloseHandle(pathHndl);

            }
            catch (Exception e)
            {
                rp.Err = -1;
                rp.ErrMsg = e.Message;
                rp.Target = "";
            }

            return;
        }
    }
}
#endif
