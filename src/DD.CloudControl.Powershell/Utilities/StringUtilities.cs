using System;
using System.Runtime.InteropServices;
using System.Security;

namespace DD.CloudControl.Powershell.Utilities
{
    /// <summary>
    ///     Utility functions for working with strings.
    /// </summary>
    public static class StringUtilities
    {
        /// <summary>
        ///     Convert the string to a <see cref="SecureString"/>.
        /// </summary>
        /// <param name="str">
        ///     The string to convert.
        /// </param>
        /// <param name="readOnly">
        ///     Make the resulting <see cref="SecureString"/> read-only?
        /// </param>
        /// <returns>
        ///     The <see cref="SecureString"/>.
        /// </returns>
        public static SecureString ToSecureString(this string str, bool readOnly = true)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));

            SecureString secureString = new SecureString();
            for (int index = 0; index < str.Length; index++)
                secureString.AppendChar(str[index]);

            if (readOnly)
                secureString.MakeReadOnly();

            return secureString;
        }

        /// <summary>
        ///     Convert a <see cref="SecureString"/> to a string.
        /// </summary>
        /// <param name="secureString">
        ///     The <see cref="SecureString"/> to convert.
        /// </param>
        /// <returns>
        ///     The string.
        /// </returns>
        public static string ToInsecureString(this SecureString secureString)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = SecureStringMarshal.SecureStringToGlobalAllocUnicode(secureString);
                
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
