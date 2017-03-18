using PSReptile;
using System;
using System.Management.Automation;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Servers
{
	using Client;
    using Client.Models.Network;
    using Client.Models.Server;

    /// <summary>
    ///     Cmdlet that creates a new server.
    /// </summary>
    [OutputType(typeof(Server))]
    [Cmdlet(VerbsCommon.New, Nouns.Vlan, SupportsShouldProcess = true)]
    [CmdletSynopsis("Creates a new server")]
	public class NewCloudControlServer
		: CloudControlCmdlet
	{
		/// <summary>
		/// 	A name for the new server.
		/// </summary>
		[ValidateNotNullOrEmpty]
		[Parameter(Position = 0, Mandatory = true, HelpMessage = "A name for the new server")]
		public string Name { get; set; }

		/// <summary>
		/// 	A description for the new server.
		/// </summary>
		[Parameter(HelpMessage = "A description for the new server")]
		public string Description { get; set; }

		/// <summary>
		/// 	The Id of the network domain where the server will be deployed.
		/// </summary>
		[Parameter(Mandatory = true, HelpMessage = "The Id of the network domain where the server will be deployed")]
		public Guid NetworkDomainId { get; set; }

		/// <summary>
		/// 	The Id of the VLAN to which the server's primary network adapter will be attached.
		/// </summary>
		[Parameter(ParameterSetName = "From VLAN", Mandatory = true, HelpMessage = "The Id of the VLAN to which the server's primary network adapter will be attached")]
		public Guid VlanId { get; set; }

		/// <summary>
		/// 	The IPv4 address for the server's primary network adapter.
		/// </summary>
		[ValidateNotNull]
		[Parameter(ParameterSetName = "From IPv4 address", Mandatory = true, HelpMessage = "The IPv4 address for the server's primary network adapter")]
		public IPAddress IPv4 { get; set; }

		/// <summary>
		/// 	The name or Id of the image from which the server will be deployed.
		/// </summary>
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true, HelpMessage = "The name or Id of the image from which the server will be deployed")]
		public string Image { get; set; }

		/// <summary>
		/// 	Optional configuration for the server's disks.
		/// </summary>
		[ValidateNotNull]
		[Parameter(HelpMessage = "Optional configuration for the server's disks")]
		public VirtualMachineDisk[] Disks { get; set; }

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

			NetworkDomain targetNetworkDomain = await client.GetNetworkDomain(NetworkDomainId, cancellationToken);
			if (targetNetworkDomain == null)
			{
				WriteError(
					Errors.ResourceNotFoundById<NetworkDomain>(NetworkDomainId)
				);

				return;
			}

			ThrowNotImplemented();
		}
	}
}