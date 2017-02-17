using System.Collections.Generic;
using System.Management.Automation;

namespace DD.CloudControl.Powershell
{
    using Client;

    /// <summary>
    ///     State data for the CloudControl Powershell provider.
    /// </summary>
    public sealed class CloudControlProviderState
        : ProviderInfo
    {
        /// <summary>
        ///     Create new <see cref="CloudControlProviderState"/>.
        /// </summary>
        /// <param name="providerInfo">
        ///     The <see cref="ProviderInfo"/> on which the <see cref="CloudControlProviderState"/> is based.
        /// </param>
        public CloudControlProviderState(ProviderInfo providerInfo)
            : base(providerInfo)
        {
            CloudControlProviderState existingProviderState = providerInfo as CloudControlProviderState;
            if (existingProviderState == null)
                return;

            DefaultConnectionName = existingProviderState.DefaultConnectionName;
        }

        /// <summary>
		///		The name of the default connection if any.
		/// </summary>
		public string DefaultConnectionName { get; set; }

        /// <summary>
        ///     Connection settings by connection name.
        /// </summary>
        public Dictionary<string, ConnectionSettings> Connections { get; } = new Dictionary<string, ConnectionSettings>();

        /// <summary>
        ///     Clients by connection name.
        /// </summary>
        public Dictionary<string, CloudControlClient> Clients { get; } = new Dictionary<string, CloudControlClient>();
    }
}
