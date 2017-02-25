using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.NetworkDomains
{
    using Client;
    using Client.Models;
    using Client.Models.Network;

    /// <summary>
    ///     Cmdlet that updates an existing network domain.
    /// </summary>
    [OutputType(typeof(NetworkDomain))]
    [Cmdlet(VerbsData.Edit, Nouns.NetworkDomain, DefaultParameterSetName = "By Id", SupportsShouldProcess = true)]
    [CmdletSynopsis("Updates an existing network domain")]
    [CmdletDescription(@"
        A network domain is the top-level container for resources in CloudControl.
    ")]
    public class EditCloudControlNetworkDomain
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The network domain to update.
        /// </summary>
        [ValidateNotNull]
        [Parameter(ParameterSetName="From NetworkDomain", Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The network domain to update")]
        public NetworkDomain NetworkDomain { get; set; }

        /// <summary>
        ///     The Id of the network domain to update.
        /// </summary>
        [Parameter(ParameterSetName="By Id", Mandatory = true, Position = 0, HelpMessage = "The Id of the network domain to update.")]
        public Guid Id { get; set; }

        /// <summary>
        ///     The new name for the network domain.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(Position = 1, HelpMessage = "The new name for the network domain")]
        public string Name { get; set; }

        /// <summary>
        ///     A description for the new network domain.
        /// </summary>
        [ValidateNotNull]
        [Parameter(Position = 2, HelpMessage = "The new description for the network domain")]
        public string Description { get; set; }

        /// <summary>
        ///     The new type for the network domain.
        /// </summary>
        /// <seealso cref="NetworkDomainType"/>
        [Parameter(Position = 1, HelpMessage = "The new type for the network domain")]
        public NetworkDomainType Type { get; set; }

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
            CloudControlClient client = GetClient();

            NetworkDomain networkDomain;
            switch (ParameterSetName)
            {
                case "From NetworkDomain":
                {
                    networkDomain = NetworkDomain;

                    break;
                }
                case "By Id":
                {
                    networkDomain = await client.GetNetworkDomain(Id, cancellationToken);
                    if (networkDomain == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundById<NetworkDomain>(Id)
                        );

                        return;
                    }

                    break;
                }
                default:
                {
                    ThrowTerminatingError(
                        Errors.UnrecognizedParameterSet(this)
                    );

                    return;
                }
            }

            if (!ShouldProcess(target: $"network domain '{networkDomain.Id}' ('{networkDomain.Name}') in '{networkDomain.DatacenterId}'", action: "update"))
                return;

            ApiResponseV2 editResponse = await client.EditNetworkDomain(networkDomain, cancellationToken);
            if (!editResponse.IsSuccess())
            {
                WriteError(
                    Errors.CloudControlApi(client, editResponse)
                );

                return;
            }

            NetworkDomain updatedNetworkDomain = await client.GetNetworkDomain(networkDomain.Id, cancellationToken);
            if (updatedNetworkDomain == null)
            {
                WriteError(
                    Errors.ResourceNotFoundById<NetworkDomain>(networkDomain.Id)
                );

                return;
            }

            WriteObject(updatedNetworkDomain);
        }
    }
}