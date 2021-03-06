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
    ///     Cmdlet that destroys an existing network domain.
    /// </summary>
    [OutputType(typeof(ApiResponseV2))]
    [Cmdlet(VerbsCommon.Remove, Nouns.NetworkDomain, DefaultParameterSetName = "By Id", SupportsShouldProcess = true)]
    [CmdletSynopsis("Destroys a network domain")]
    public class RemoveCloudControlNetworkDomain
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The network domain to destroy.
        /// </summary>
        [ValidateNotNull]
        [Parameter(ParameterSetName="From network domain", Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The network domain to destroy")]
        public NetworkDomain NetworkDomain { get; set; }

        /// <summary>
        ///     The Id of the network domain to destroy.
        /// </summary>
        [Parameter(ParameterSetName="By Id", Mandatory = true, Position = 0, HelpMessage = "The Id of the network domain to destroy")]
        public Guid Id { get; set; }

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
                case "From network domain":
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

            if (!ShouldProcess(target: $"network domain '{networkDomain.Id}' ('{networkDomain.Name}') in '{networkDomain.DatacenterId}'", action: "Destroy"))
                return;

            ApiResponseV2 apiResponse = await client.DeleteNetworkDomain(networkDomain.Id, cancellationToken);
            if (!apiResponse.IsSuccess())
            {
                WriteError(
                    Errors.CloudControlApi(client, apiResponse)
                );
            }
            else
                WriteObject(apiResponse);
        }
    }
}