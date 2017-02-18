using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Accounts
{
    using Client;
    using Client.Models.Network;

    /// <summary>
    ///     Cmdlet that retrieves information about one or more CloudControl network domains.
    /// </summary>
    [OutputType(typeof(NetworkDomain))]
    [Cmdlet(VerbsCommon.Get, Nouns.NetworkDomain)]
    public class GetCloudControlNetworkDomain
        : CloudControlCmdlet
    {
        /// <summary>
        ///     The Id of the network domain to retrieve.
        /// </summary>
        [Parameter(ParameterSetName = "By Id", Mandatory = true, HelpMessage = "The Id of the network domain to retrieve")]
        public string Id { get; set; }

        /// <summary>
        ///     The Id of the target datacenter.
        /// </summary>
        [Parameter(ParameterSetName = "By Name", Mandatory = true, HelpMessage = "The Id of the target datacenter")]
        public string DatacenterId { get; set; }

        /// <summary>
        ///     The name the network domain to retrieve.
        /// </summary>
        [Parameter(ParameterSetName = "By Name", Mandatory = true, HelpMessage = "The name the network domain to retrieve")]
        public string Name { get; set; }

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
                case "By Id":
                {
                    networkDomain = await client.GetNetworkDomain(Id, cancellationToken);
                    if (networkDomain == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundById<NetworkDomain>(Id)
                        );
                    }
                    else
                        WriteObject(networkDomain);

                    break;
                }
                case "By Name":
                {
                    networkDomain = await client.GetNetworkDomainByName(Name, DatacenterId);
                    if (networkDomain == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundByName<NetworkDomain>(Name, 
                                message: $"No network domain named '{Name}' was found in datacenter '{DatacenterId}'."
                            )
                        );
                    }
                    else
                        WriteObject(networkDomain);
                    
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
        }
    }
}
