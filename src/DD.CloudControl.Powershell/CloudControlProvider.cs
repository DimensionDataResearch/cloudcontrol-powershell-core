using System;
using System.Management.Automation;
using System.Management.Automation.Provider;

namespace DD.CloudControl.Powershell
{
    /// <summary>
    ///     The Powershell provider for Dimension Data CloudControl.
    /// </summary>
    /// <remarks>
    ///     This provider is used to centralise access to module connection state.
    /// </remarks>
    [CmdletProvider(ProviderName, ProviderCapabilities.None)]
    public class CloudControlProvider
        : CmdletProvider
    {
        /// <summary>
        ///     The CloudControl Powershell provider name.
        /// </summary>
        public const string ProviderName = "DimensionData.CloudControl";

        /// <summary>
        ///     Create a new CloudControl provider.
        /// </summary>
        public CloudControlProvider()
        {
        }

        /// <summary>
        ///     The provider state data.
        /// </summary>
        public CloudControlProviderState ProviderState { get; private set; }

        /// <summary>
        ///     Start the provider.
        /// </summary>
        /// <param name="providerInfo">
        ///     The initial provider information.
        /// </param>
        /// <returns>
        ///     The CloudControl provider state.
        /// </returns>
        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            if (providerInfo == null)
                throw new ArgumentNullException(nameof(providerInfo));

            if (ProviderState != null)
                throw new InvalidOperationException("The CloudControl Powershell provider has already been started.");

            ProviderState = new CloudControlProviderState(providerInfo);

            return ProviderState;
        }
    }
}
