using System.Management.Automation;

namespace DD.CloudControl.Powershell
{
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
    }
}