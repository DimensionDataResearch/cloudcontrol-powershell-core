using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Servers
{
    using Client;
    using Client.Models.Network;
    using Client.Models.Server;

    /// <summary>
    ///     Cmdlet that retrieves information about one or more servers.
    /// </summary>
    [OutputType(typeof(Server))]
    [Cmdlet(VerbsCommon.Get, Nouns.Server, DefaultParameterSetName = "By Id", SupportsPaging = true)]
    [CmdletSynopsis("Retrieves information about one or more servers")]
	[CmdletDescription("In CloudControl, a Server is a virtual machine.")]
    public class GetCloudControlServer
        : CloudControlCmdlet
	{
		/// <summary>
        ///     The Id of the server to retrieve.
        /// </summary>
        [Parameter(ParameterSetName = "By Id", Position = 0, Mandatory = true, HelpMessage = "The Id of the network domain to retrieve")]
        public Guid Id { get; set; }

        /// <summary>
        ///     Retrieve all servers in the network domain with the specified Id.
        /// </summary>
        [Parameter(ParameterSetName = "By network domain Id", Mandatory = true, HelpMessage = "Retrieve all servers in the network domain with the specified Id")]
        public Guid NetworkDomainId { get; set; }

		/// <summary>
        ///     Retrieve all servers in the specified network domain.
        /// </summary>
        [ValidateNotNull]
		[Parameter(ParameterSetName = "By network domain", Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Retrieve all servers in the specified network domain")]
        public NetworkDomain NetworkDomain { get; set; }

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
                    Server server = await client.GetServer(Id, cancellationToken);
                    if (server == null)
                    {
                        WriteError(
                            Errors.ResourceNotFoundById<Server>(Id)
                        );
                    }
                    else
                        WriteObject(server);

                    break;
                }
				case "By network domain Id":
                {
                    Paging paging = GetPagingConfiguration();
                    
                    Servers servers = await client.ListServers(NetworkDomainId, paging, cancellationToken);
                    WriteObject(servers.Items,
                        enumerateCollection: true
                    );

                    break;
                }
				case "By network domain":
                {
                    Paging paging = GetPagingConfiguration();
                    
                    Servers servers = await client.ListServers(NetworkDomain.Id, paging, cancellationToken);
                    WriteObject(servers.Items,
                        enumerateCollection: true
                    );

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