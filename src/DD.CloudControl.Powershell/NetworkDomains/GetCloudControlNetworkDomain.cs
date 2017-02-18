using System.Management.Automation;
using System.Linq;
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

            switch (ParameterSetName)
            {
                case "By Id":
                {
                    NetworkDomain networkDomain = await client.GetNetworkDomain(Id, cancellationToken);
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
                    // HACKY!
                    // TODO: Implement GetNetworkDomainByName.

                    Paging paging = new Paging();

                    NetworkDomains networkDomains;
                    while (true)
                    {
                        networkDomains = await client.ListNetworkDomains(DatacenterId, paging, cancellationToken);
                        if (networkDomains.IsEmpty)
                            break; // No more pages.
                        
                        NetworkDomain matchingNetworkDomain = networkDomains.FirstOrDefault(
                            networkDomain => networkDomain.Name == Name
                        );
                        if (matchingNetworkDomain != null)
                        {
                            WriteObject(matchingNetworkDomain);

                            return; // We're done.
                        }

                        paging.Next();
                    }

                    break;
                }
                default:
                {
                    ThrowTerminatingError(
                        Errors.UnrecognizedParameterSet(this)
                    );

                    break;
                }
            }
        }
    }
}
