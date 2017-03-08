using PSReptile;
using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace DD.CloudControl.Powershell.Servers
{
	using Client;
	using Client.Models;
	using Client.Models.Network;
	using Client.Models.Server;

	/// <summary>
	///     Cmdlet that destroys a server.
	/// </summary>
	[OutputType(typeof(ApiResponseV2))]
	[Cmdlet(VerbsCommon.Remove, Nouns.Server, DefaultParameterSetName = "By Id", SupportsShouldProcess = true)]
	[CmdletSynopsis("Destroys a server")]
	public class RemoveCloudControlVan
		: CloudControlCmdlet
	{
		/// <summary>
		///     The Id of the server to destroy.
		/// </summary>
		[Parameter(ParameterSetName = "By Id", Mandatory = true, Position = 0, HelpMessage = "The Id of the VLAN to destroy")]
		public Guid Id { get; set; }

		/// <summary>
		///     The server to destroy.
		/// </summary>
		[ValidateNotNull]
		[Parameter(ParameterSetName = "From server", Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "The VLAN to destroy")]
		public Server Server { get; set; }

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

			Server server;
			switch (ParameterSetName)
			{
				case "By Id":
				{
					server = await client.GetServer(Id, cancellationToken);
					if (server == null)
					{
						WriteError(
							Errors.ResourceNotFoundById<Server>(Id)
						);

						return;
					}

					break;
				}
				case "From server":
				{
					server = Server;

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

			Guid networkDomainId = server.Network.NetworkDomainId;
			NetworkDomain networkDomain = await client.GetNetworkDomain(networkDomainId);
			if (networkDomain == null)
			{
				WriteError(
					Errors.ResourceNotFoundById<NetworkDomain>(networkDomainId)
				);

				return;
			}

			if (!ShouldProcess(action: "Destroy", target: $"server '{server.Name}' ('{server.Id}') in network domain '{networkDomain.Name}'"))
				return;

			ApiResponseV2 apiResponse = await client.DeleteServer(server.Id, cancellationToken);
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