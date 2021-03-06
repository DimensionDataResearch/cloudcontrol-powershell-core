using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace DD.CloudControl.Powershell
{
    using Client;

	/// <summary>
	///		Extension methods for Powershell <see cref="SessionState"/>
	/// </summary>
	public static class SessionStateExtensions
	{
		/// <summary>
		///		Read persisted connection settings for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state to populaye with persisted connection settings.
		/// </param>
		/// <exception cref="InvalidOperationException">
		///		The CloudControl Powershell provider is not loaded in the current session.
		/// </exception>
		public static void ReadConnections(this SessionState sessionState)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			Dictionary<string, ConnectionSettings> connections = sessionState.GetProviderState().Connections;
			foreach (ConnectionSettings connection in SettingsStore.ReadConnectionSettings())
                connections[connection.Name] = connection;
		}

		/// <summary>
		/// 	Persist connection settings for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state containing connection settings to persist.
		/// </param>
		public static void WriteConnections(this SessionState sessionState)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			SettingsStore.WriteConnectionSettings(
				sessionState.Connections()
			);
		}

		/// <summary>
		///		Get a dictionary of <see cref="ConnectionSettings"/> (keyed by connection name) for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state from which to retrieve the CloudControl connection container table.
		/// </param>
		/// <returns>
		///		The dictionary.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		///		The CloudControl Powershell provider is not loaded in the current session.
		/// </exception>
		public static Dictionary<string, ConnectionSettings> Connections(this SessionState sessionState)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			return sessionState.GetProviderState().Connections;
		}

		/// <summary>
		///		Get a dictionary of <see cref="CloudControlClient"/>s (keyed by connection name) for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state from which to retrieve the CloudControl connection container table.
		/// </param>
		/// <returns>
		///		The dictionary.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		///		The CloudControl Powershell provider is not loaded in the current session.
		/// </exception>
		public static Dictionary<string, CloudControlClient> Clients(this SessionState sessionState)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			return sessionState.GetProviderState().Clients;
		}

		/// <summary>
		///		Get the name of the default CloudControl connection (if configured) for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state from which to retrieve the CloudControl connection container table.
		/// </param>
		/// <returns>
		///		The default connection name, or <c>null</c> if no default CloudControl connection has been configured for the current runspace.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		///		The CloudControl Powershell provider is not loaded in the current session.
		/// </exception>
		public static string GetDefaultCloudControlConnectionName(this SessionState sessionState)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			return sessionState.GetProviderState().Connections.Values
				.Where(
					connection => connection.IsDefault
				)
				.Select(
					connection => connection.Name
				)
				.FirstOrDefault();
		}

        /// <summary>
		///		Get the default CloudControl connection (if configured) for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state from which to retrieve the CloudControl connection container table.
		/// </param>
		/// <returns>
		///		The default connection, or <c>null</c> if no default CloudControl connection has been configured for the current runspace.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		///		The CloudControl Powershell provider is not loaded in the current session.
		/// </exception>
		public static ConnectionSettings GetDefaultCloudControlConnection(this SessionState sessionState)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			return sessionState.GetProviderState().Connections.Values
				.FirstOrDefault(
					connection => connection.IsDefault
				);
		}

		/// <summary>
		///		Get the name of the default CloudControl connection (if configured) for the current runspace.
		/// </summary>
		/// <param name="sessionState">
		///		The session state from which to retrieve the CloudControl connection container table.
		/// </param>
		/// <param name="defaultConnectionName">
		///		The default connection name, or <c>null</c> if there should not be a default CloudControl connection for the current runspace.
		/// </param>
		/// <exception cref="InvalidOperationException">
		///		The CloudControl Powershell provider is not loaded in the current session.
		/// </exception>
		public static void SetDefaultCloudControlConnection(this SessionState sessionState, string defaultConnectionName)
		{
			if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			foreach (ConnectionSettings connection in sessionState.GetProviderState().Connections.Values)
				connection.IsDefault = (connection.Name == defaultConnectionName);
		}

        /// <summary>
        ///     Get CloudControl provider state from session state.
        /// </summary>
        /// <param name="sessionState">
        ///     The session state.
        /// </param>
        /// <param name="isRequired">
		/// 	Throw an exception if valid provider state cannot be found?
		/// </param>
		/// <returns>
        ///     The <see cref="CloudControlProviderState"/> (or <c>null</c> if provider state was not found in session state and <paramref name="isRequired"/> is <c>false</c>).
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The session state does not contain CloudControl provider state and and <paramref name="isRequired"/> is <c>true</c>.
        /// </exception>
        static CloudControlProviderState GetProviderState(this SessionState sessionState, bool isRequired = true)
        {
            if (sessionState == null)
				throw new ArgumentNullException(nameof(sessionState));

			switch (sessionState.Provider.GetOne(CloudControlProvider.ProviderName))
			{
				case CloudControlProviderState providerState:
				{
					return providerState;
				}
				default:
				{
					if (isRequired)
						throw new InvalidOperationException("Cannot find CloudControl Powershell provider state in current session."); // AF: Should not happen under normal circumstances.
					
					return null;
				}
			}
        }
	}
}
