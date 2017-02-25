using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.NetworkDomains
{
    using Client;
    using Client.Models.Network;

    /// <summary>
    ///     Cmdlet that creates a new network domain.
    /// </summary>
    [OutputType(typeof(NetworkDomain))]
    [Cmdlet(VerbsCommon.New, Nouns.NetworkDomain, SupportsShouldProcess = true)]
    [CmdletSynopsis("Creates a new network domain")]
    [CmdletDescription(@"
        A network domain is the top-level container for resources in CloudControl.
    ")]
    public class NewCloudControlNetworkDomain
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The Id of the datacenter where the network domain will be created.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "The Id of the datacenter where the network domain will be created")]
        public string DatacenterId { get; set; }

        /// <summary>
        ///     A name for the new network domain.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, HelpMessage = "A name for the new network domain")]
        public string Name { get; set; }

        /// <summary>
        ///     A description for the new network domain.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(HelpMessage = "A description for the new network domain")]
        public string Description { get; set; }

        /// <summary>
        ///     The type of network domain to create.
        /// </summary>
        /// <seealso cref="NetworkDomainType"/>
        [Parameter(HelpMessage = "The type of network domain to create (default is essentials)")]
        public NetworkDomainType Type { get; set; } = NetworkDomainType.Essentials;

        /// <summary>
        ///     Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> that can be used to cancel Cmdlet processing.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected override async Task ProcessRecordAsync(CancellationToken cancellationToken)
        {
            if (!ShouldProcess(target: $"{Type} network domain '{Name}' in '{DatacenterId}'", action: "Create"))
                return;

            WriteVerbose(
                $"Create {Type} network domain '{Name}' in datacenter '{DatacenterId}'."
            );

            WriteVerbose("Initiating deployment of network domain...");

            CloudControlClient client = GetClient();
            Guid networkDomainId = await client.CreateNetworkDomain(DatacenterId, Name, Description, Type, cancellationToken);

            WriteVerbose($"Deployment initiated for network domain '{networkDomainId}'.");

            NetworkDomain networkDomain = await client.GetNetworkDomain(networkDomainId, cancellationToken);
            if (networkDomain == null)
            {
                WriteError(
                    Errors.ResourceNotFoundById<NetworkDomain>(networkDomainId)
                );

                return;
            }

            WriteObject(networkDomain);
        }
    }
}