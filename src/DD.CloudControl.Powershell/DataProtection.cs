using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

namespace DD.CloudControl.Powershell
{
    /// <summary>
    ///     Data protection services for the CloudControl Powershell module.
    /// </summary>
    public static class DataProtection
    {
        /// <summary>
        ///     Services used by the settings store (e.g. data protection).
        /// </summary>
        static readonly IServiceProvider StoreServices;

        /// <summary>
        ///     Type initialiser for data protection.
        /// </summary>
        static DataProtection()
        {
            ServiceCollection storeServices = new ServiceCollection();
            storeServices.AddDataProtection(protection =>
            {
                protection.ApplicationDiscriminator = "DD.CloudControl.Powershell";
            });

            StoreServices = storeServices.BuildServiceProvider();
        }

        /// <summary>
        ///     Create and use an <see cref="IDataProtector"/>.
        /// </summary>
        /// <param name="action">
        ///     A delegate that uses the data protector.
        /// </param>
        public static void Use(Action<IDataProtector> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            using (IServiceScope scope = StoreServices.CreateScope())
            {
                IDataProtector protector = scope.ServiceProvider
                    .GetDataProtectionProvider()
                    .CreateProtector("DD.CloudControl.Powershell.Connection.Credentials");

                action(protector);
            }
        }

        /// <summary>
        ///     Create and use an <see cref="IDataProtector"/>.
        /// </summary>
        /// <param name="action">
        ///     A delegate that uses the data protector.
        /// </param>
        public static TResult Use<TResult>(Func<IDataProtector, TResult> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            using (IServiceScope scope = StoreServices.CreateScope())
            {
                IDataProtector protector = scope.ServiceProvider
                    .GetDataProtectionProvider()
                    .CreateProtector("DD.CloudControl.Powershell.Connection.Credentials");

                return action(protector);
            }
        }

        /// <summary>
        ///     Protect a string value.
        /// </summary>
        /// <param name="protector">
        ///     The data protector.
        /// </param>
        /// <param name="value">
        ///     The value to protect.
        /// </param>
        /// <returns>
        ///     The protected value (Base64-encoded).
        /// </returns>
        public static string Protect(this IDataProtector protector, string value)
        {
            if (protector == null)
                throw new ArgumentNullException(nameof(protector));

            return Convert.ToBase64String(
                protector.Protect(
                    Encoding.UTF8.GetBytes(value)
                )
            );
        }

        /// <summary>
        ///     Protect a string value.
        /// </summary>
        /// <param name="protector">
        ///     The data protector.
        /// </param>
        /// <param name="value">
        ///     The value to unprotect (Base64-encoded).
        /// </param>
        /// <returns>
        ///     The unprotected value.
        /// </returns>
        public static string Unprotect(this IDataProtector protector, string value)
        {
            if (protector == null)
                throw new ArgumentNullException(nameof(protector));

            return Encoding.UTF8.GetString(
                protector.Unprotect(
                    Convert.FromBase64String(value)
                )
            );
        }
    }
}