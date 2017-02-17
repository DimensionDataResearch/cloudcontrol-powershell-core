using System;
using System.Management.Automation;

namespace DD.CloudControl.Powershell
{
	/// <summary>
	///		Extension methods for Powershell <see cref="SessionState"/>
	/// </summary>
	public static class SessionStateExtensions
	{
        /// <summary>
		///		Get the name of the default DMS connection (if configured) for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state from which to retrieve the DMS connection container table.
		/// </param>
		/// <returns>
		///		The default connection name, or <c>null</c> if no default DMS connection has been configured for the current runspace.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		///		The DMS Powershell provider is not loaded in the current session.
		/// </exception>
		public static string GetDefaultCloudControlConnectionName(this SessionState sessionState)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			return sessionState.GetProviderState().DefaultConnectionName;
		}

		/// <summary>
		///		Get the name of the default DMS connection (if configured) for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state from which to retrieve the DMS connection container table.
		/// </param>
		/// <param name="defaultConnectionName">
		///		The default connection name, or <c>null</c> if there should not be a default DMS connection for the current runspace.
		/// </param>
		/// <exception cref="InvalidOperationException">
		///		The DMS Powershell provider is not loaded in the current session.
		/// </exception>
		public static void SetDefaultCloudControlConnectionName(this SessionState sessionState, string defaultConnectionName)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			sessionState.GetProviderState().DefaultConnectionName = defaultConnectionName;
		}

        /// <summary>
        ///     Get CloudControl provider state from session state.
        /// </summary>
        /// <param name="sessionState">
        ///     The session state.
        /// </param>
        /// <returns>
        ///     The <see cref="CloudControlProviderState"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The session state does not contain CloudControl provider state.
        /// </exception>
        static CloudControlProviderState GetProviderState(this SessionState sessionState)
        {
            if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			CloudControlProviderState providerState = sessionState.Provider.GetOne(CloudControlProvider.ProviderName) as CloudControlProviderState;
			if (providerState == null)
				throw new InvalidOperationException("Cannot find CloudControl Powershell provider state in current session."); // AF: Should not happen under normal circumstances.
            
            return providerState;
        }
	}
}
