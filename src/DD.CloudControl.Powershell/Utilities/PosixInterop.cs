using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DD.CloudControl.Powershell.Utilities
{
    /// <summary>
    ///     POSIX functionality.
    /// </summary>
    public static class PosixInterop
    {
        /// <summary>
        ///     Apply POSIX-style permissions to a file or directory.
        /// </summary>
        /// <param name="filePath">
        ///     The path to the target file or directory.
        /// </param>
        /// <param name="permissions">
        ///     The permissions to apply.
        /// </param>
        public static void SetPosixPermissions(this FileInfo file, PosixPermissions permissions)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            SetPosixPermissions(file.FullName, permissions);
        }

        /// <summary>
        ///     Apply POSIX-style permissions to a file or directory.
        /// </summary>
        /// <param name="filePath">
        ///     The path to the target file or directory.
        /// </param>
        /// <param name="permissions">
        ///     The permissions to apply.
        /// </param>
        public static void SetPosixPermissions(string filePath, PosixPermissions permissions)
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Must supply a valid file path.", nameof(filePath));

            if (!OS.IsLinux && !OS.IsMac)
                throw new PlatformNotSupportedException("The current platform does not support POSIX-style permissions.");

            int result = chmod(filePath, permissions);
            if (result != 0)
                throw new IOException($"Failed to execute chmod for '{filePath}' with permissions {permissions.ToOctal()} (error code was {Marshal.GetLastWin32Error()}).");
        }

        /// <summary>
        ///     Convert POSIX permissions to an octal string representation.
        /// </summary>
        /// <param name="permissions">
        ///     The permissions.
        /// </param>
        /// <returns>
        ///     The octal representation (e.g. "0700").
        /// </returns>
        public static string ToOctal(this PosixPermissions permissions)
        {
            string octal = Convert.ToString(
                (int)permissions, toBase: 8
            );

            return octal.PadLeft(4, '0');
        }

        /// <summary>
        ///     Apply POSIX-style permissions to a file or directory.
        /// </summary>
        /// <param name="path">
        ///     The path to the target file or directory.
        /// </param>
        /// <param name="permissions">
        ///     The new permissions to apply.
        /// </param>
        /// <returns>
        ///     0, if successful; otherwise, -1 (should set LastError, but doesn't).
        /// </returns>
        [DllImport("libc", SetLastError = true)]
        static extern int chmod(string path, PosixPermissions permissions);
    }

    /// <summary>
    ///     POSIX file permissions.
    /// </summary>
    [Flags]
    public enum PosixPermissions : int
    {
        /// <summary>
        ///     User can read.
        /// </summary>
        UserRead = 0x100,

        /// <summary>
        ///     User can write.
        /// </summary>
        UserWrite = 0x80,

        /// <summary>
        ///     User can execute.
        /// </summary>
        UserExecute = 0x40,

        /// <summary>
        ///     User can read and write.
        /// </summary>
        UserReadWrite = UserRead | UserWrite,

        /// <summary>
        ///     User can read, write, and execute.
        /// </summary>
        UserReadWriteExecute = UserReadWrite | UserExecute,

        /// <summary>
        ///     Group can read.
        /// </summary>
        GroupRead = 0x20,

        /// <summary>
        ///     Group can write.
        /// </summary>
        GroupWrite = 0x10,

        /// <summary>
        ///     Group can execute.
        /// </summary>
        GroupExecute = 0x8,

        /// <summary>
        ///     Group can read and write.
        /// </summary>
        GroupReadWrite = GroupRead | GroupWrite,

        /// <summary>
        ///     Group can read, write, and execute.
        /// </summary>
        GroupReadWriteExecute = GroupReadWrite | GroupExecute,

        /// <summary>
        ///     Others can read.
        /// </summary>
        OtherRead = 0x4,

        /// <summary>
        ///     Others can write.
        /// </summary>
        OtherWrite = 0x2,

        /// <summary>
        ///     Others can execute.
        /// </summary>
        OtherExecute = 0x1,

        /// <summary>
        ///     Others can read and write.
        /// </summary>
        OtherReadWrite = OtherRead | OtherWrite,

        /// <summary>
        ///     Others can read, write, and execute.
        /// </summary>
        OtherReadWriteExecute = OtherReadWrite | OtherExecute

        // TODO: AllRead, AllWrite, AllExecute, AllReadWrite, AllReadWriteExecute
    }
}