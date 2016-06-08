/*
 * PiSearch
 * WinFileFlagsAndAttributes - File Flags and attributes used by the Windows API when opening/creating a file
 *  For a full list of possible values, see http://msdn.microsoft.com/en-us/library/windows/desktop/aa363858%28v=vs.85%29.aspx
 * By Josh Keegan 03/01/2014
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.IO
{
    // Naming doesn't match convention for C# enum, but is consistent with lower level Windows implementation
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum WinFileFlagsAndAttributes : uint
    {
        FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,
        FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,
        FILE_FLAG_NO_BUFFERING = 0x20000000,
        FILE_FLAG_OPEN_NO_RECALL = 0x00100000,
        FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000,
        FILE_FLAG_OVERLAPPED = 0x40000000,
        FILE_FLAG_POSIX_SEMANTICS = 0x0100000,
        FILE_FLAG_RANDOM_ACCESS = 0x10000000,
        FILE_FLAG_SESSION_AWARE = 0x00800000,
        FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
        FILE_FLAG_WRITE_THROUGH = 0x80000000,
    }
}
